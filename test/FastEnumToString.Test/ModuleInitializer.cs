﻿using System.Runtime.CompilerServices;

namespace FastEnumToString.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
    }
}