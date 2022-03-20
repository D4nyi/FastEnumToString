using System;

using Microsoft.CodeAnalysis;

namespace FastEnumToString
{
    /// <summary>
    /// Generates a <em>FastToString</em> method for each enum in the referencing assembly.<br />
    /// From <see cref="ISourceGenerator"/>:<br />
    /// <inheritdoc/>
    /// </summary>
    [Generator(LanguageNames.CSharp)]
    public class EnumToStringGenerator : ISourceGenerator
    {
        private const string GeneratedFileName = "EnumStringConverter.g.cs";

        /// <inheritdoc/>
        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Launch();
            }
#endif
            context.RegisterForSyntaxNotifications(() => new EnumToStringSyntaxReciever());
        }

        /// <inheritdoc/>
        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is EnumToStringSyntaxReciever reciever))
            {
                return;
            }
            
            if (reciever.Enums.Count == 0)
            {
                context.AddSource(GeneratedFileName, EnumProcessor.Empty);
                return;
            }

            context
                .AnalyzerConfigOptions.GlobalOptions
                .TryGetValue("build_property.FastEnumFallbackValue", out string fallback);
            context
                .AnalyzerConfigOptions.GlobalOptions
                .TryGetValue("build_property.FastEnumIncludeFlags", out string includeFlagsStr);

            FallbackType fallbackType = ParseFallbackValue(fallback);
            bool includeFlags = ParseIncludeFlags(includeFlagsStr);

            EnumProcessor processor = new EnumProcessor(context)
            {
                Fallback = fallbackType,
                IncludeFlags = includeFlags
            };

            string generatedClass = processor.Process(reciever.Enums, reciever.Flags);

            context.AddSource(GeneratedFileName, generatedClass);
        }

        private static FallbackType ParseFallbackValue(string fallback)
        {
            if (String.IsNullOrWhiteSpace(fallback))
            {
                return FallbackType.Throw;
            }

            fallback = fallback.Trim().ToLower();
            switch (fallback)
            {
                case "default":
                    return FallbackType.Default;
                case "first":
                    return FallbackType.First;
                default:
                    return FallbackType.Throw;
            }
        }

        private static bool ParseIncludeFlags(string includeFlags)
        {
            return !String.IsNullOrWhiteSpace(includeFlags)
                && includeFlags.Trim().Equals("true", StringComparison.OrdinalIgnoreCase);
        }
    }
}

