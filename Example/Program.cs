using FastEnumToString;

Console.WriteLine("Hello, World!");

namespace MyNamespace
{
    public enum Color
    {
        Red,
        Green,
        Blue
    }

    [ExcludeToString]
    public enum Devices
    {
        Phone,
        TV,
        Watch
    }
}
