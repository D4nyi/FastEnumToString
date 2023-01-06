﻿//HintName: FastToStringAttributes.g.cs
// <auto-generated/>

namespace FastEnumToString
{
    /// <summary>
    /// Choose the default behviour of the `FastToString` method if no matching value is found.<br/>
    /// <strong>You may override the global default behaviour!</strong>
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("FastEnumToString", "1.2.0")]
    public enum ToStringDefault
    {
        /// <summary>
        /// The global default value, has no effect
        /// </summary>
        Default,
        /// <summary>
        /// The first value in the enum will be used if no matching value is found
        /// </summary>
        First,
        /// <summary>
        /// Will throw an <see cref="global::System.ArgumentOutOfRangeException"/> if no matching value is found
        /// </summary>
        Throw
    }

    /// <summary>
    /// Marks an enum to generate an optimized `FastToString` for it
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("FastEnumToString", "1.2.0")]
    [global::System.AttributeUsage(global::System.AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public sealed class ToStringAttribute : global::System.Attribute
    {
        /// <summary>
        /// Only for marking
        /// </summary>
        public ToStringAttribute() { }

        /// <summary>
        /// Marks and overriding the global default behaviour
        /// </summary>
        /// <param name="toStringDefault">The behaviour of the `FastToString` method</param>
        public ToStringAttribute(ToStringDefault toStringDefault) { }
    }
}