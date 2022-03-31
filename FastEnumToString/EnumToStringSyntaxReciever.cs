using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FastEnumToString
{
    internal sealed class EnumToStringSyntaxReciever : ISyntaxReceiver
    {
        private readonly HashSet<EnumDeclarationSyntax> _enums;

        public IReadOnlyCollection<EnumDeclarationSyntax> Enums { get => _enums; }

        public EnumToStringSyntaxReciever()
        {
            _enums = new HashSet<EnumDeclarationSyntax>();
        }

        /// <inheritdoc/>
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is EnumDeclarationSyntax enumDeclarationSyntax
                && !enumDeclarationSyntax.AttributeLists.HasFlagsOrExcludeAttribute())
            {
                _ = _enums.Add(enumDeclarationSyntax);
            }
        }
    }
}
