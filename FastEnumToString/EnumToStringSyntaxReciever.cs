using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FastEnumToString
{
    internal sealed class EnumToStringSyntaxReciever : ISyntaxReceiver
    {
        public ICollection<EnumDeclarationSyntax> FoundEnums { get; }

        public EnumToStringSyntaxReciever()
        {
            FoundEnums = new List<EnumDeclarationSyntax>();
        }

        /// <inheritdoc/>
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is EnumDeclarationSyntax enumDeclarationSyntax)
            {
                FoundEnums.Add(enumDeclarationSyntax);
            }
        }
    }
}
