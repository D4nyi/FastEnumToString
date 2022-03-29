using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FastEnumToString
{
    internal static class Extensions
    {
        internal static bool HasAttribute(this SyntaxList<AttributeListSyntax> attributeList)
        {
            return attributeList.Any(x => x.Attributes.Any(y =>
            {
                string attributeName = y.Name.ToString();
                return attributeName == "System.FlagsAttribute"
                    || attributeName == "System.Flags"
                    || attributeName == "FlagsAttribute"
                    || attributeName == "Flags";
            }));
        }
    }
}
