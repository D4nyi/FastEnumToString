﻿using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FastEnumToString
{
    [Generator]
    public class EnumToStringGenerator : ISourceGenerator
    {
        #region Private Members
        #region Diagnostic Descriptors
        private static readonly DiagnosticDescriptor Error =
            new DiagnosticDescriptor(
                id: "ETSG001",
                title: "An error has occured while generating enum extensions",
                messageFormat: "An error has occured while generationg source for enum extensions for {0}.{1}",
                category: "Compilation",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor Success =
            new DiagnosticDescriptor(
                id: "ETSG002",
                title: "Successfully generated enum extensions",
                messageFormat: "Successfully generated source for enum extensions with name {0}.{1}",
                category: "Compilation",
                defaultSeverity: DiagnosticSeverity.Info,
                isEnabledByDefault: true);
        #endregion

        private const string autoGenComment = "// <auto-generated />\r\n\r\n";
        private const string usingTemplate = "using {0};\r\n";
        private const string classStart = "\r\npublic static class EnumStringConverter\r\n{";
        private const string method = "\r\n    {0} static string FastToString(this {1} enumValue)";
        private const string bodyNSwitchStart = "\r\n    {\r\n        return enumValue switch\r\n        {\r\n";
        private const string swicthArm = "            {0}.{1} => nameof({0}.{1}),\r\n";
        private const string switchNBodyEnd = "            _ => throw new ArgumentOutOfRangeException(nameof(enumValue), enumValue, null)\r\n        };\r\n    }";
        #endregion


        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            //if (!System.Diagnostics.Debugger.IsAttached)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}
#endif
            context.RegisterForSyntaxNotifications(() => new EnumToStringSyntaxReciever());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is EnumToStringSyntaxReciever reciever))
            {
                return;
            }

            if (reciever.FoundEnums.Count == 0)
            {
                context.AddSource("EnumStringConverter.g.cs", "// <auto-generated />\r\npublic static class EnumStringConverter\r\n{\r\n    // Code will be generated here.\r\n}");
                return;
            }

            int methodLengthEstimation = bodyNSwitchStart.Length + switchNBodyEnd.Length + method.Length + 16 + 8 * (swicthArm.Length + 25);

            var namespaces = new HashSet<string>();
            var methods = new StringBuilder(methodLengthEstimation);

            foreach (EnumDeclarationSyntax syntax in reciever.FoundEnums)
            {
                ISymbol symbol = context
                    .Compilation
                    .GetSemanticModel(syntax.SyntaxTree)
                    .GetDeclaredSymbol(syntax, context.CancellationToken);

                try
                {
                    namespaces.Add(String.Format(usingTemplate, symbol.ContainingNamespace));

                    string modifier = syntax.Modifiers.Count > 0 ? syntax.Modifiers[0].ToString() : "internal";

                    methods
                        .AppendFormat(method, modifier, symbol.Name)
                        .Append(bodyNSwitchStart)
                        .Append(ProcessEnum(syntax.Members, symbol.Name))
                        .AppendLine(switchNBodyEnd);
                }
                catch (Exception ex)
                {
                    Location location = Location.Create(syntax.SyntaxTree, syntax.Span);
                    context.ReportDiagnostic(Diagnostic.Create(Error, location, symbol.Name, ex));
                }
            }

            string usings = String.Join("", namespaces);
            string generatedClass = new StringBuilder(autoGenComment.Length + usings.Length + classStart.Length + methods.Length + 1)
                .Append(autoGenComment)
                .Append(usings)
                .Append(classStart)
                .Append(methods)
                .Append('}')
                .ToString();


            context.ReportDiagnostic(Diagnostic.Create(Success, Location.None));
            // Add the source code to the compilation
            context.AddSource("EnumStringConverter.g.cs", generatedClass);
        }

        private static string ProcessEnum(SeparatedSyntaxList<EnumMemberDeclarationSyntax> enumMembers, string enumName)
        {
            // Since the names length can vary an additional value has been added
            // to get as close as possible to the real length
            int suggestedStartCapacity = enumMembers.Count * (swicthArm.Length + 25);

            StringBuilder builder = new StringBuilder(suggestedStartCapacity);

            for (int i = 0; i < enumMembers.Count; i++)
            {
                builder.AppendFormat(
                    swicthArm,
                    enumName,
                    enumMembers[i].Identifier.ToString());
            }

            return builder.ToString();
        }
    }
}

