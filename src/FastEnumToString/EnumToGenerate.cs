using System.Collections.Generic;

namespace FastEnumToString
{
    internal readonly struct EnumToGenerate
    {
        internal string Name { get; }
        internal string Modifier { get; }
        internal string GenericTypeParameters { get; }
        internal string GenericTypeConstraints { get; }
        internal int DefaultValue { get; }
        internal IReadOnlyList<string> Values { get; }

        internal EnumToGenerate(
            string name,
            string modifier,
            string genericTypeParameters,
            string genericTypeConstraints,
            int defaultValue,
            IReadOnlyList<string> values)
        {
            Name = name;
            Modifier = modifier;
            GenericTypeParameters = genericTypeParameters;
            GenericTypeConstraints = genericTypeConstraints;
            DefaultValue = defaultValue;
            Values = values;
        }
    }
}