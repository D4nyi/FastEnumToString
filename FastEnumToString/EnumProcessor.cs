﻿using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FastEnumToString
{
    internal class EnumProcessor
    {
        #region Private Members
        private const string UsingTemplate = "using {0};\r\n";
        private const string SwicthArm = "                {0}.{1} => nameof({0}.{1}),\r\n";
        private const string SwitchDefaultThrow = "                _ => throw new global::System.ArgumentOutOfRangeException(nameof(enumValue), enumValue, $\"{nameof(enumValue)} cannot be found in the provided enum type!\")";
        private const string SwitchDefault = "                _ => nameof({0}.{1})";
        private const string GeneratedCodeAttr =
            "[global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"" + nameof(FastEnumToString) + "." + nameof(EnumToStringGenerator) + "\", \"" + Assembly.Version + "\")]";

        private const string MethodTemplate = @"
        {0} static string FastToString(this {1} enumValue)
        {{
            return enumValue switch
            {{
{2}
            }};
        }}
";
        private const string ClassTemplate = @"// <auto-generated />

{0}
namespace {1}
{{
    {2}
    public static class EnumStringConverter
    {{{3}    }}
}}";
        #endregion

        private readonly GeneratorExecutionContext _context;
        private readonly string _rootNamespace;
        private readonly HashSet<string> _namespaces = new HashSet<string>();
        public static string Empty => $"//<auto-generated />\r\n\r\n// Code will be generated here, yet we found no enums.";

        public FallbackType Fallback { get; set; }
        public bool IncludeFlags { get; set; }

        public EnumProcessor(GeneratorExecutionContext context, string rootNamespace)
        {
            _context = context;
            _rootNamespace = rootNamespace;

        }

        /// <summary>
        /// Processes the collected enum and generates the extension methods for them.
        /// </summary>
        /// <param name="context">The generator context in which the enums are located and where the source should be added</param>
        /// <param name="enums">The collection of found enums</param>
        /// <returns>The generated static class in which the extension methods are located</returns>
        public string Process(IReadOnlyCollection<EnumDeclarationSyntax> enums)
        {
            string enumBodies = ProcessEnum(enums);

            string usings = String.Join("", _namespaces);
            string @class = String.Format(ClassTemplate, usings, _rootNamespace, GeneratedCodeAttr, enumBodies);
            return @class;
        }

        private string ProcessEnum(IReadOnlyCollection<EnumDeclarationSyntax> enums)
        {
            var methods = new StringBuilder();

            foreach (EnumDeclarationSyntax @enum in enums)
            {
                ISymbol symbol = _context
                    .Compilation
                    .GetSemanticModel(@enum.SyntaxTree)
                    .GetDeclaredSymbol(@enum, _context.CancellationToken);

                if (symbol.ContainingNamespace.ToString() != "<global namespace>")
                {
                    _ = _namespaces.Add(String.Format(UsingTemplate, symbol.ContainingNamespace));
                }
                string modifier = @enum.Modifiers.Count > 0 ? @enum.Modifiers[0].Text : "internal";

                string body = CreateBody(@enum.Members, symbol.Name);

                methods.AppendFormat(MethodTemplate, modifier, symbol.Name, body);
            }

            return methods.ToString();
        }

        /// <summary>
        /// Generates the extension method body for an enum
        /// </summary>
        /// <param name="enumMembers">Collection of the enum members</param>
        /// <param name="enumName">Name of the enum</param>
        /// <returns>The genrated extension method body</returns>
        private string CreateBody(SeparatedSyntaxList<EnumMemberDeclarationSyntax> enumMembers, string enumName)
        {
            StringBuilder builder = new StringBuilder();

            // it's read once instead of in every iteration
            int count = enumMembers.Count;
            for (int i = 0; i < count; i++)
            {
                builder.AppendFormat(
                    SwicthArm,
                    enumName,
                    enumMembers[i].Identifier.Text);
            }

            if (Fallback == FallbackType.Throw || enumMembers.Count == 0)
            {
                builder.Append(SwitchDefaultThrow);
            }
            else
            {
                builder.AppendFormat(SwitchDefault, enumName, enumMembers[0].Identifier.Text);
            }

            return builder.ToString();
        }
    }
}
