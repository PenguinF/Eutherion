#region License
/*********************************************************************************
 * NullAnalysisAttributes.cs
 *
 * Copyright (c) 2004-2022 Henk Nicolai
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
**********************************************************************************/
#endregion

#if NET472
namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    /// Defines the [AllowNull] attribute for .NET targets in which it does not exist yet.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = false)]
    public sealed class AllowNullAttribute : Attribute
    {
        /// <summary>
        /// Creates a new <see cref="AllowNullAttribute"/> instance.
        /// </summary>
        public AllowNullAttribute() { }
    }

    /// <summary>
    /// Defines the [DoesNotReturn] attribute for .NET targets in which it does not exist yet.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class DoesNotReturnAttribute : Attribute
    {
        /// <summary>
        /// Creates a new <see cref="DoesNotReturnAttribute"/> instance.
        /// </summary>
        public DoesNotReturnAttribute() { }
    }

    /// <summary>
    /// Defines the [MaybeNullWhen] attribute for .NET targets in which it does not exist yet.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class MaybeNullWhenAttribute : Attribute
    {
        /// <summary>
        /// Creates a new <see cref="MaybeNullWhenAttribute"/> instance.
        /// </summary>
        public MaybeNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;

        /// <summary>
        /// Gets the return value.
        /// </summary>
        public bool ReturnValue { get; }
    }

    /// <summary>
    /// Defines the [NotNull] attribute for .NET targets in which it does not exist yet.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
    public sealed class NotNullAttribute : Attribute
    {
        /// <summary>
        /// Creates a new <see cref="NotNullAttribute"/> instance.
        /// </summary>
        public NotNullAttribute() {}
    }

    /// <summary>
    /// Defines the [NotNullWhen] attribute for .NET targets in which it does not exist yet.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class NotNullWhenAttribute : Attribute
    {
        /// <summary>
        /// Creates a new <see cref="NotNullWhenAttribute"/> instance.
        /// </summary>
        public NotNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;

        /// <summary>
        /// Gets the return value.
        /// </summary>
        public bool ReturnValue { get; }
    }
}
#endif
