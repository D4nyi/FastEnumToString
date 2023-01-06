using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FastEnumToString
{
    internal static class Extensions
    {
        private static readonly Func<AttributeSyntax, bool> _filterToStringAttribute = x =>
            x.Name.ToString().Contains("ToString");

        private static IEnumerable<AttributeSyntax> Filter(SyntaxList<AttributeListSyntax> attributeList)
            => attributeList.SelectMany(static x => x.Attributes);

        internal static bool HasToStringAttribute(this SyntaxList<AttributeListSyntax> attributeList)
            => Filter(attributeList).Any(_filterToStringAttribute);

        internal static AttributeSyntax? GetToStringAttribute(this SyntaxList<AttributeListSyntax> attributeList)
            => Filter(attributeList).FirstOrDefault(_filterToStringAttribute);

        internal static bool HasAttributeArgument(this AttributeSyntax? syntax)
            => syntax is not null && syntax.ArgumentList is not null && syntax.ArgumentList.Arguments.Count > 0;

        internal static bool GenericParameterCanBeRetrieved(this INamedTypeSymbol? symbol)
            => symbol is not null && symbol.IsGenericType && !symbol.TypeArguments.IsDefaultOrEmpty;
    }
}