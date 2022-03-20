using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FastEnumToString
{
    internal sealed class EnumToStringSyntaxReciever : ISyntaxReceiver
    {
        private readonly HashSet<EnumDeclarationSyntax> _enums;
        private readonly HashSet<EnumDeclarationSyntax> _flags;
        
        public IReadOnlyCollection<EnumDeclarationSyntax> Enums { get => _enums; }
        public IReadOnlyCollection<EnumDeclarationSyntax> Flags { get => _flags; }

        public EnumToStringSyntaxReciever()
        {
            _enums = new HashSet<EnumDeclarationSyntax>();
            _flags = new HashSet<EnumDeclarationSyntax>();
        }

        /// <inheritdoc/>
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (!(syntaxNode is EnumDeclarationSyntax enumDeclarationSyntax))
            {
                return;
            }

            if (enumDeclarationSyntax.AttributeLists.HasAttribute())
            {
                _ = _flags.Add(enumDeclarationSyntax);
            }
            else
            {
                _ = _enums.Add(enumDeclarationSyntax);
            }
        }
    }
}
