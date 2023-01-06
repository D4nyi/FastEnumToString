using FastEnumToString;
using ToStringExample;

using MyNamespace;

Color color = (Color)5;

Console.WriteLine(color);
Console.WriteLine(color.FastToString());
Console.WriteLine(NestingClass<int, List<int>>.NestedInClassEnum.None.FastToString());
Console.WriteLine(EnumStringConverter.FastToString(NestingClass<int, List<int>>.NestedInClassEnum.None));

namespace MyNamespace
{
    public class NestingClass<T, K>
        where T : struct
        where K : class, new()
    {
        [ToString]
        public enum NestedInClassEnum
        {
            None
        }
    }
    
    [ToString(ToStringDefault.First)]
    public enum Color
    {
        Red,
        Green,
        Blue,
    }

    [ToString, Flags] // Flags are not yet supported
    public enum Devices
    {
        Phone = 1,
        TV = 2,
        Watch = 4,
    }
}
