namespace FastEnumToString.IntegrationTests;

public sealed class EnumExtensionsTests
{
    [Theory]
    [InlineData(Color.Red)]
    [InlineData(Color.Green)]
    //[InlineData((Color)15)]
    //[InlineData((Color)0)]
    public void FastToStringIsSameAsToString(Color value)
    {
        string expected = value.ToString();
        string actual = value.FastToString();

        Assert.Equal(expected, actual);
    }


    [Theory]
    [InlineData((Color)15)]
    [InlineData((Color)0)]
    public void FastToStringIsFirstValueWhenNotFound(Color value)
    {
        string expected = Color.Red.ToString();
        string actual = value.FastToString();

        Assert.Equal(expected, actual);
    }
}