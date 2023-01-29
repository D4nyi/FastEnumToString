using System.Runtime.CompilerServices;

[assembly: System.CLSCompliant(false)]
[assembly: System.Reflection.AssemblyVersion(Assembly.CorrectVersion)]
[assembly: System.Reflection.AssemblyInformationalVersion(Assembly.Version)]
[assembly: System.Reflection.AssemblyFileVersion(Assembly.CorrectVersion)]

[assembly: InternalsVisibleTo("FastEnumToString.Test", AllInternalsVisible = true)]

internal readonly ref struct Assembly
{
    internal const string Version = "2.0.0";
    internal const string CorrectVersion = "2.0.0";
}