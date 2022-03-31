using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FastEnumToString
{
    internal static class Extensions
    {
        private static readonly Func<AttributeSyntax, bool> _filterOverrideAttribute = x =>
        {
            string attributeName = x.Name.ToString();
            return attributeName == "FastEnumToString.OverrideToStringDefaultAttribute"
                || attributeName == "FastEnumToString.OverrideToStringDefault"
                || attributeName == "OverrideToStringDefaultAttribute"
                || attributeName == "OverrideToStringDefault";
        };

        private static IEnumerable<AttributeSyntax> Filter(SyntaxList<AttributeListSyntax> attributeList)
        {
            return attributeList.SelectMany(x => x.Attributes);
        }

        internal static bool HasFlagsOrExcludeAttribute(this SyntaxList<AttributeListSyntax> attributeList)
        {
            return Filter(attributeList).Any(x =>
            {
                string attributeName = x.Name.ToString();
                return attributeName == "System.FlagsAttribute"
                    || attributeName == "System.Flags"
                    || attributeName == "FlagsAttribute"
                    || attributeName == "Flags"

                    || attributeName == "FastEnumToString.ExcludeToStringAttribute"
                    || attributeName == "FastEnumToString.ExcludeToString"
                    || attributeName == "ExcludeToStringAttribute"
                    || attributeName == "ExcludeToString";
            });
        }

        internal static bool HasOverrideAttribute(this SyntaxList<AttributeListSyntax> attributeList)
        {
            return Filter(attributeList).Any(_filterOverrideAttribute);
        }

        internal static AttributeSyntax GetOverrideAttribute(this SyntaxList<AttributeListSyntax> attributeList)
        {
            return Filter(attributeList).FirstOrDefault(_filterOverrideAttribute);
        }
    }
}
