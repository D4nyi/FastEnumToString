//HintName: EnumStringConverter.g.cs
// <auto-generated/>

namespace FastEnumToString
{
    [global::System.CodeDom.Compiler.GeneratedCode("FastEnumToString", "1.2.0")]
    public static class EnumStringConverter
    {
        public static string FastToString(this SnapshotTesting.Color enumValue) => enumValue switch
        {
                SnapshotTesting.Color.Red => nameof(SnapshotTesting.Color.Red),
                SnapshotTesting.Color.Green => nameof(SnapshotTesting.Color.Green),
                SnapshotTesting.Color.Blue => nameof(SnapshotTesting.Color.Blue),
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(enumValue), enumValue, $"Value: '{(int)enumValue}' cannot be found in the provided enum type!")
        };
    }
}