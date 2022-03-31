using FastEnumToString;
using MyNamespace;
using Example;

Color c = (Color)5;
Console.WriteLine(c);
Console.WriteLine(c.FastToString());

namespace MyNamespace
{
    [OverrideToStringDefault(-1)]
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
