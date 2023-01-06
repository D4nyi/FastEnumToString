﻿using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace FastEnumToString
{
    [Generator]
    public sealed class EnumToStringGenerator : IIncrementalGenerator
    {
        private const string GeneratedFileName = "EnumStringConverter.g.cs";
        private const string AttributesFile = "FastToStringAttributes.g.cs";
        private const string Attributes = $@"// <auto-generated/>

namespace FastEnumToString
{{
    /// <summary>
    /// Choose the default behviour of the `FastToString` method if no matching value is found.<br/>
    /// <strong>You may override the global default behaviour!</strong>
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode(""{nameof(FastEnumToString)}"", ""{Assembly.Version}"")]
    public enum ToStringDefault
    {{
        /// <summary>
        /// The global default value, has no effect
        /// </summary>
        Default,
        /// <summary>
        /// The first value in the enum will be used if no matching value is found
        /// </summary>
        First,
        /// <summary>
        /// Will throw an <see cref=""global::System.ArgumentOutOfRangeException""/> if no matching value is found
        /// </summary>
        Throw
    }}

    /// <summary>
    /// Marks an enum to generate an optimized `FastToString` for it
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode(""{nameof(FastEnumToString)}"", ""{Assembly.Version}"")]
    [global::System.AttributeUsage(global::System.AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public sealed class ToStringAttribute : global::System.Attribute
    {{
        /// <summary>
        /// Only for marking
        /// </summary>
        public ToStringAttribute() {{ }}

        /// <summary>
        /// Marks and overriding the global default behaviour
        /// </summary>
        /// <param name=""toStringDefault"">The behaviour of the `FastToString` method</param>
        public ToStringAttribute(ToStringDefault toStringDefault) {{ }}
    }}
}}";

        private readonly EnumProcessor _processor;

        public EnumToStringGenerator()
        {
            _processor = new EnumProcessor();
        }

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
//#if DEBUG
//            if (!System.Diagnostics.Debugger.IsAttached)
//            {
//                System.Diagnostics.Debugger.Launch();
//            }
//#endif
            // Add the marker attribute to the compilation
            context.RegisterPostInitializationOutput(static ctx =>
                ctx.AddSource(AttributesFile, SourceText.From(Attributes, Encoding.UTF8)));

            // Do a simple filter for enums
            IncrementalValuesProvider<EnumToGenerate?> enumDeclarations =
            context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: IsSyntaxTargetForGeneration, // select enums with attributes
                    transform: GetSemanticTargetForGeneration) // select the enum with the [ToString] attribute
                .Where(static x => x is not null && x.HasValue); // filter out attributed enums that we don't care about

            // Combine the selected enums with the `Compilation`
            //IncrementalValueProvider<(Compilation, ImmutableArray<EnumToGenerate?>)> compilationAndEnums
            //    = context.CompilationProvider.Combine(enumDeclarations.Collect());

            IncrementalValueProvider<(AnalyzerConfigOptionsProvider Options, ImmutableArray<EnumToGenerate?> Enums)> asd =
                context.AnalyzerConfigOptionsProvider.Combine(enumDeclarations.Collect());

            // Generate the source using the compilation and enums
            context.RegisterSourceOutput(asd,
                (spc, source) =>
                Execute(source.Options, source.Enums, spc));
        }

        private static bool IsSyntaxTargetForGeneration(SyntaxNode node, CancellationToken ct)
            => node is EnumDeclarationSyntax enumDeclaration &&
               enumDeclaration.AttributeLists.Count > 0 &&
               enumDeclaration.AttributeLists.HasToStringAttribute();

        private EnumToGenerate? GetSemanticTargetForGeneration(GeneratorSyntaxContext context, CancellationToken ct)
        {
            // stop if we're asked to
            ct.ThrowIfCancellationRequested();

            // Get the semantic representation of the enum syntax
            if (
                context.SemanticModel.GetDeclaredSymbol(context.Node, ct) is not INamedTypeSymbol enumSymbol ||
                context.Node is not EnumDeclarationSyntax enumDeclaration)
            {
                // something went wrong, bail out
                return null;
            }

            // Create an EnumToGenerate for use in the generation phase
            return _processor.CreateEnumMetadata(enumSymbol, enumDeclaration, context.SemanticModel.Compilation, ct);
        }

        private void Execute(AnalyzerConfigOptionsProvider options, ImmutableArray<EnumToGenerate?> enums, SourceProductionContext context)
        {
            if (enums.IsDefaultOrEmpty)
            {
                // nothing to do yet
                return;
            }

            options.GlobalOptions
                .TryGetValue("build_property.FastEnumDefaultBehaviour", out string? defaultBehaviourValue);

            int defaultBehaviour = ParseDefaultBehaviourValue(defaultBehaviourValue);

            string result = _processor.Process(enums, defaultBehaviour);

            context.AddSource(GeneratedFileName, SourceText.From(result, Encoding.UTF8));
        }

        private static int ParseDefaultBehaviourValue(string? defaultBehaviour)
        {
            if (String.IsNullOrWhiteSpace(defaultBehaviour))
            {
                return 2;
            }

            defaultBehaviour = defaultBehaviour!.Trim();

            return defaultBehaviour.Equals("first", StringComparison.OrdinalIgnoreCase) ? 1 : 2;
        }
    }
}