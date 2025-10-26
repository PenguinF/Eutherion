#region License
/*********************************************************************************
 * Union.cs
 *
 * Copyright (c) 2004-2025 Henk Nicolai
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

using System.Diagnostics.CodeAnalysis;

namespace System
{
    /// <summary>
    /// Encapsulates a value which can have two different types.
    /// </summary>
    /// <typeparam name="T1">
    /// The first type of the value.
    /// </typeparam>
    /// <typeparam name="T2">
    /// The second type of the value.
    /// </typeparam>
    /// <remarks>
    /// This type is deliberately implemented without equality or hash code implementations,
    /// as these would have to make assumptions on equality of the contained object or value.
    /// </remarks>
    public abstract class Union<T1, T2>
#if !NET472
        where T1 : notnull
        where T2 : notnull
#endif
    {
        private sealed class ValueOfType1 : Union<T1, T2>
        {
            public readonly T1 Value;

            public ValueOfType1(T1 value) => Value = value;

            public override bool IsOption1() => true;

            public override bool IsOption1(out T1 value)
            {
                value = Value;
                return true;
            }

            public override T1 ToOption1() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption1 != null ? whenOption1(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(1);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption1 != null) whenOption1(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType2 : Union<T1, T2>
        {
            public readonly T2 Value;

            public ValueOfType2(T2 value) => Value = value;

            public override bool IsOption2() => true;

            public override bool IsOption2(out T2 value)
            {
                value = Value;
                return true;
            }

            public override T2 ToOption2() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption2 != null ? whenOption2(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(2);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption2 != null) whenOption2(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Defines methods to test <see cref="Union{T1, T2}"/> values for equality and generate their hash codes.
        /// </summary>
        public class EqualityComparer
        {
        }

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2}"/> with a value of the first type.
        /// </summary>
        public static Union<T1, T2> Option1(T1 value) => new ValueOfType1(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2}"/> with a value of the second type.
        /// </summary>
        public static Union<T1, T2> Option2(T2 value) => new ValueOfType2(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2>(T1 value) => new ValueOfType1(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2>(T2 value) => new ValueOfType2(value);

        private Union() { }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2}"/> contains a value of the first type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2}"/> contains a value of the first type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption1() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2}"/> contains a value of the second type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2}"/> contains a value of the second type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption2() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2}"/> contains a value of the first type.
        /// </summary>
        /// <param name="value">
        /// The value of the first type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2}"/> contains a value of the first type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption1([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T1 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2}"/> contains a value of the second type.
        /// </summary>
        /// <param name="value">
        /// The value of the second type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2}"/> contains a value of the second type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption2([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T2 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Casts this <see cref="Union{T1, T2}"/> to a value of the first type.
        /// </summary>
        /// <returns>
        /// The value of the first type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2}"/> does not contain a value of the first type.
        /// </exception>
        public virtual T1 ToOption1() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2}"/> to a value of the second type.
        /// </summary>
        /// <returns>
        /// The value of the second type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2}"/> does not contain a value of the second type.
        /// </exception>
        public virtual T2 ToOption2() => throw new InvalidCastException();

        /// <summary>
        /// Invokes a <see cref="Func{T, TResult}"/> based on the type of the value and returns its result.
        /// </summary>
        /// <typeparam name="TResult">
        /// Type of the value to return.
        /// </typeparam>
        /// <param name="whenOption1">
        /// The <see cref="Func{T1, TResult}"/> to invoke when the value is of the first type.
        /// </param>
        /// <param name="whenOption2">
        /// The <see cref="Func{T2, TResult}"/> to invoke when the value is of the second type.
        /// </param>
        /// <param name="otherwise">
        /// The <see cref="Func{TResult}"/> to invoke if no function is specified for the type of the value.
        /// If <paramref name="whenOption1"/> and <paramref name="whenOption2"/> are given, this parameter is not used.
        /// </param>
        /// <returns>
        /// The result of the invoked <see cref="Func{T, TResult}"/>.
        /// </returns>
        /// <exception cref="InvalidPatternMatchException">
        /// No function argument was defined that matches the value.
        /// </exception>
        public abstract TResult Match<TResult>(
            [AllowNull] Func<T1, TResult> whenOption1 = null,
            [AllowNull] Func<T2, TResult> whenOption2 = null,
            [AllowNull] Func<TResult> otherwise = null)
#if !NET472
            where TResult : notnull
#endif
            ;

        /// <summary>
        /// Invokes an <see cref="Action{T}"/> based on the type of the value.
        /// </summary>
        /// <param name="whenOption1">
        /// The <see cref="Action{T1}"/> to invoke when the value is of the first type.
        /// </param>
        /// <param name="whenOption2">
        /// The <see cref="Action{T2}"/> to invoke when the value is of the second type.
        /// </param>
        /// <param name="otherwise">
        /// The <see cref="Action"/> to invoke if no action is specified for the type of the value.
        /// If <paramref name="whenOption1"/> and <paramref name="whenOption2"/> are given, this parameter is not used.
        /// </param>
        public abstract void Match(
            [AllowNull] Action<T1> whenOption1 = null,
            [AllowNull] Action<T2> whenOption2 = null,
            [AllowNull] Action otherwise = null);
    }

    /// <summary>
    /// Encapsulates a value which can have three different types.
    /// </summary>
    /// <typeparam name="T1">
    /// The first type of the value.
    /// </typeparam>
    /// <typeparam name="T2">
    /// The second type of the value.
    /// </typeparam>
    /// <typeparam name="T3">
    /// The third type of the value.
    /// </typeparam>
    /// <remarks>
    /// This type is deliberately implemented without equality or hash code implementations,
    /// as these would have to make assumptions on equality of the contained object or value.
    /// </remarks>
    public abstract class Union<T1, T2, T3>
#if !NET472
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
#endif
    {
        private sealed class ValueOfType1 : Union<T1, T2, T3>
        {
            public readonly T1 Value;

            public ValueOfType1(T1 value) => Value = value;

            public override bool IsOption1() => true;

            public override bool IsOption1(out T1 value)
            {
                value = Value;
                return true;
            }

            public override T1 ToOption1() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption1 != null ? whenOption1(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(1);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption1 != null) whenOption1(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType2 : Union<T1, T2, T3>
        {
            public readonly T2 Value;

            public ValueOfType2(T2 value) => Value = value;

            public override bool IsOption2() => true;

            public override bool IsOption2(out T2 value)
            {
                value = Value;
                return true;
            }

            public override T2 ToOption2() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption2 != null ? whenOption2(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(2);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption2 != null) whenOption2(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType3 : Union<T1, T2, T3>
        {
            public readonly T3 Value;

            public ValueOfType3(T3 value) => Value = value;

            public override bool IsOption3() => true;

            public override bool IsOption3(out T3 value)
            {
                value = Value;
                return true;
            }

            public override T3 ToOption3() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption3 != null ? whenOption3(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(3);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption3 != null) whenOption3(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Defines methods to test <see cref="Union{T1, T2, T3}"/> values for equality and generate their hash codes.
        /// </summary>
        public class EqualityComparer
        {
        }

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3}"/> with a value of the first type.
        /// </summary>
        public static Union<T1, T2, T3> Option1(T1 value) => new ValueOfType1(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3}"/> with a value of the second type.
        /// </summary>
        public static Union<T1, T2, T3> Option2(T2 value) => new ValueOfType2(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3}"/> with a value of the third type.
        /// </summary>
        public static Union<T1, T2, T3> Option3(T3 value) => new ValueOfType3(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3>(T1 value) => new ValueOfType1(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3>(T2 value) => new ValueOfType2(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3>(T3 value) => new ValueOfType3(value);

        private Union() { }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3}"/> contains a value of the first type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3}"/> contains a value of the first type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption1() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3}"/> contains a value of the second type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3}"/> contains a value of the second type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption2() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3}"/> contains a value of the third type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3}"/> contains a value of the third type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption3() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3}"/> contains a value of the first type.
        /// </summary>
        /// <param name="value">
        /// The value of the first type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3}"/> contains a value of the first type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption1([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T1 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3}"/> contains a value of the second type.
        /// </summary>
        /// <param name="value">
        /// The value of the second type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3}"/> contains a value of the second type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption2([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T2 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3}"/> contains a value of the third type.
        /// </summary>
        /// <param name="value">
        /// The value of the third type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3}"/> contains a value of the third type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption3([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T3 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3}"/> to a value of the first type.
        /// </summary>
        /// <returns>
        /// The value of the first type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3}"/> does not contain a value of the first type.
        /// </exception>
        public virtual T1 ToOption1() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3}"/> to a value of the second type.
        /// </summary>
        /// <returns>
        /// The value of the second type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3}"/> does not contain a value of the second type.
        /// </exception>
        public virtual T2 ToOption2() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3}"/> to a value of the third type.
        /// </summary>
        /// <returns>
        /// The value of the third type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3}"/> does not contain a value of the third type.
        /// </exception>
        public virtual T3 ToOption3() => throw new InvalidCastException();

        /// <summary>
        /// Invokes a <see cref="Func{T, TResult}"/> based on the type of the value and returns its result.
        /// </summary>
        /// <typeparam name="TResult">
        /// Type of the value to return.
        /// </typeparam>
        /// <param name="whenOption1">
        /// The <see cref="Func{T1, TResult}"/> to invoke when the value is of the first type.
        /// </param>
        /// <param name="whenOption2">
        /// The <see cref="Func{T2, TResult}"/> to invoke when the value is of the second type.
        /// </param>
        /// <param name="whenOption3">
        /// The <see cref="Func{T3, TResult}"/> to invoke when the value is of the third type.
        /// </param>
        /// <param name="otherwise">
        /// The <see cref="Func{TResult}"/> to invoke if no function is specified for the type of the value.
        /// If <paramref name="whenOption1"/>, <paramref name="whenOption2"/> and <paramref name="whenOption3"/> are given, this parameter is not used.
        /// </param>
        /// <returns>
        /// The result of the invoked <see cref="Func{T, TResult}"/>.
        /// </returns>
        /// <exception cref="InvalidPatternMatchException">
        /// No function argument was defined that matches the value.
        /// </exception>
        public abstract TResult Match<TResult>(
            [AllowNull] Func<T1, TResult> whenOption1 = null,
            [AllowNull] Func<T2, TResult> whenOption2 = null,
            [AllowNull] Func<T3, TResult> whenOption3 = null,
            [AllowNull] Func<TResult> otherwise = null)
#if !NET472
            where TResult : notnull
#endif
            ;

        /// <summary>
        /// Invokes an <see cref="Action{T}"/> based on the type of the value.
        /// </summary>
        /// <param name="whenOption1">
        /// The <see cref="Action{T1}"/> to invoke when the value is of the first type.
        /// </param>
        /// <param name="whenOption2">
        /// The <see cref="Action{T2}"/> to invoke when the value is of the second type.
        /// </param>
        /// <param name="whenOption3">
        /// The <see cref="Action{T3}"/> to invoke when the value is of the third type.
        /// </param>
        /// <param name="otherwise">
        /// The <see cref="Action"/> to invoke if no action is specified for the type of the value.
        /// If <paramref name="whenOption1"/>, <paramref name="whenOption2"/> and <paramref name="whenOption3"/> are given, this parameter is not used.
        /// </param>
        public abstract void Match(
            [AllowNull] Action<T1> whenOption1 = null,
            [AllowNull] Action<T2> whenOption2 = null,
            [AllowNull] Action<T3> whenOption3 = null,
            [AllowNull] Action otherwise = null);
    }

    /// <summary>
    /// Encapsulates a value which can have four different types.
    /// </summary>
    /// <typeparam name="T1">
    /// The first type of the value.
    /// </typeparam>
    /// <typeparam name="T2">
    /// The second type of the value.
    /// </typeparam>
    /// <typeparam name="T3">
    /// The third type of the value.
    /// </typeparam>
    /// <typeparam name="T4">
    /// The fourth type of the value.
    /// </typeparam>
    /// <remarks>
    /// This type is deliberately implemented without equality or hash code implementations,
    /// as these would have to make assumptions on equality of the contained object or value.
    /// </remarks>
    public abstract class Union<T1, T2, T3, T4>
#if !NET472
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
#endif
    {
        private sealed class ValueOfType1 : Union<T1, T2, T3, T4>
        {
            public readonly T1 Value;

            public ValueOfType1(T1 value) => Value = value;

            public override bool IsOption1() => true;

            public override bool IsOption1(out T1 value)
            {
                value = Value;
                return true;
            }

            public override T1 ToOption1() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption1 != null ? whenOption1(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(1);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption1 != null) whenOption1(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType2 : Union<T1, T2, T3, T4>
        {
            public readonly T2 Value;

            public ValueOfType2(T2 value) => Value = value;

            public override bool IsOption2() => true;

            public override bool IsOption2(out T2 value)
            {
                value = Value;
                return true;
            }

            public override T2 ToOption2() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption2 != null ? whenOption2(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(2);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption2 != null) whenOption2(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType3 : Union<T1, T2, T3, T4>
        {
            public readonly T3 Value;

            public ValueOfType3(T3 value) => Value = value;

            public override bool IsOption3() => true;

            public override bool IsOption3(out T3 value)
            {
                value = Value;
                return true;
            }

            public override T3 ToOption3() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption3 != null ? whenOption3(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(3);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption3 != null) whenOption3(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType4 : Union<T1, T2, T3, T4>
        {
            public readonly T4 Value;

            public ValueOfType4(T4 value) => Value = value;

            public override bool IsOption4() => true;

            public override bool IsOption4(out T4 value)
            {
                value = Value;
                return true;
            }

            public override T4 ToOption4() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption4 != null ? whenOption4(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(4);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption4 != null) whenOption4(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Defines methods to test <see cref="Union{T1, T2, T3, T4}"/> values for equality and generate their hash codes.
        /// </summary>
        public class EqualityComparer
        {
        }

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4}"/> with a value of the first type.
        /// </summary>
        public static Union<T1, T2, T3, T4> Option1(T1 value) => new ValueOfType1(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4}"/> with a value of the second type.
        /// </summary>
        public static Union<T1, T2, T3, T4> Option2(T2 value) => new ValueOfType2(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4}"/> with a value of the third type.
        /// </summary>
        public static Union<T1, T2, T3, T4> Option3(T3 value) => new ValueOfType3(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4}"/> with a value of the fourth type.
        /// </summary>
        public static Union<T1, T2, T3, T4> Option4(T4 value) => new ValueOfType4(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4>(T1 value) => new ValueOfType1(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4>(T2 value) => new ValueOfType2(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4>(T3 value) => new ValueOfType3(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4>(T4 value) => new ValueOfType4(value);

        private Union() { }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4}"/> contains a value of the first type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4}"/> contains a value of the first type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption1() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4}"/> contains a value of the second type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4}"/> contains a value of the second type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption2() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4}"/> contains a value of the third type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4}"/> contains a value of the third type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption3() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4}"/> contains a value of the fourth type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4}"/> contains a value of the fourth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption4() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4}"/> contains a value of the first type.
        /// </summary>
        /// <param name="value">
        /// The value of the first type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4}"/> contains a value of the first type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption1([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T1 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4}"/> contains a value of the second type.
        /// </summary>
        /// <param name="value">
        /// The value of the second type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4}"/> contains a value of the second type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption2([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T2 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4}"/> contains a value of the third type.
        /// </summary>
        /// <param name="value">
        /// The value of the third type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4}"/> contains a value of the third type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption3([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T3 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4}"/> contains a value of the fourth type.
        /// </summary>
        /// <param name="value">
        /// The value of the fourth type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4}"/> contains a value of the fourth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption4([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T4 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4}"/> to a value of the first type.
        /// </summary>
        /// <returns>
        /// The value of the first type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4}"/> does not contain a value of the first type.
        /// </exception>
        public virtual T1 ToOption1() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4}"/> to a value of the second type.
        /// </summary>
        /// <returns>
        /// The value of the second type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4}"/> does not contain a value of the second type.
        /// </exception>
        public virtual T2 ToOption2() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4}"/> to a value of the third type.
        /// </summary>
        /// <returns>
        /// The value of the third type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4}"/> does not contain a value of the third type.
        /// </exception>
        public virtual T3 ToOption3() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4}"/> to a value of the fourth type.
        /// </summary>
        /// <returns>
        /// The value of the fourth type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4}"/> does not contain a value of the fourth type.
        /// </exception>
        public virtual T4 ToOption4() => throw new InvalidCastException();

        /// <summary>
        /// Invokes a <see cref="Func{T, TResult}"/> based on the type of the value and returns its result.
        /// </summary>
        /// <typeparam name="TResult">
        /// Type of the value to return.
        /// </typeparam>
        /// <param name="whenOption1">
        /// The <see cref="Func{T1, TResult}"/> to invoke when the value is of the first type.
        /// </param>
        /// <param name="whenOption2">
        /// The <see cref="Func{T2, TResult}"/> to invoke when the value is of the second type.
        /// </param>
        /// <param name="whenOption3">
        /// The <see cref="Func{T3, TResult}"/> to invoke when the value is of the third type.
        /// </param>
        /// <param name="whenOption4">
        /// The <see cref="Func{T4, TResult}"/> to invoke when the value is of the fourth type.
        /// </param>
        /// <param name="otherwise">
        /// The <see cref="Func{TResult}"/> to invoke if no function is specified for the type of the value.
        /// If <paramref name="whenOption1"/>, <paramref name="whenOption2"/>, <paramref name="whenOption3"/> and <paramref name="whenOption4"/> are given, this parameter is not used.
        /// </param>
        /// <returns>
        /// The result of the invoked <see cref="Func{T, TResult}"/>.
        /// </returns>
        /// <exception cref="InvalidPatternMatchException">
        /// No function argument was defined that matches the value.
        /// </exception>
        public abstract TResult Match<TResult>(
            [AllowNull] Func<T1, TResult> whenOption1 = null,
            [AllowNull] Func<T2, TResult> whenOption2 = null,
            [AllowNull] Func<T3, TResult> whenOption3 = null,
            [AllowNull] Func<T4, TResult> whenOption4 = null,
            [AllowNull] Func<TResult> otherwise = null)
#if !NET472
            where TResult : notnull
#endif
            ;

        /// <summary>
        /// Invokes an <see cref="Action{T}"/> based on the type of the value.
        /// </summary>
        /// <param name="whenOption1">
        /// The <see cref="Action{T1}"/> to invoke when the value is of the first type.
        /// </param>
        /// <param name="whenOption2">
        /// The <see cref="Action{T2}"/> to invoke when the value is of the second type.
        /// </param>
        /// <param name="whenOption3">
        /// The <see cref="Action{T3}"/> to invoke when the value is of the third type.
        /// </param>
        /// <param name="whenOption4">
        /// The <see cref="Action{T4}"/> to invoke when the value is of the fourth type.
        /// </param>
        /// <param name="otherwise">
        /// The <see cref="Action"/> to invoke if no action is specified for the type of the value.
        /// If <paramref name="whenOption1"/>, <paramref name="whenOption2"/>, <paramref name="whenOption3"/> and <paramref name="whenOption4"/> are given, this parameter is not used.
        /// </param>
        public abstract void Match(
            [AllowNull] Action<T1> whenOption1 = null,
            [AllowNull] Action<T2> whenOption2 = null,
            [AllowNull] Action<T3> whenOption3 = null,
            [AllowNull] Action<T4> whenOption4 = null,
            [AllowNull] Action otherwise = null);
    }

    /// <summary>
    /// Encapsulates a value which can have five different types.
    /// </summary>
    /// <typeparam name="T1">
    /// The first type of the value.
    /// </typeparam>
    /// <typeparam name="T2">
    /// The second type of the value.
    /// </typeparam>
    /// <typeparam name="T3">
    /// The third type of the value.
    /// </typeparam>
    /// <typeparam name="T4">
    /// The fourth type of the value.
    /// </typeparam>
    /// <typeparam name="T5">
    /// The fifth type of the value.
    /// </typeparam>
    /// <remarks>
    /// This type is deliberately implemented without equality or hash code implementations,
    /// as these would have to make assumptions on equality of the contained object or value.
    /// </remarks>
    public abstract class Union<T1, T2, T3, T4, T5>
#if !NET472
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
        where T5 : notnull
#endif
    {
        private sealed class ValueOfType1 : Union<T1, T2, T3, T4, T5>
        {
            public readonly T1 Value;

            public ValueOfType1(T1 value) => Value = value;

            public override bool IsOption1() => true;

            public override bool IsOption1(out T1 value)
            {
                value = Value;
                return true;
            }

            public override T1 ToOption1() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption1 != null ? whenOption1(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(1);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption1 != null) whenOption1(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType2 : Union<T1, T2, T3, T4, T5>
        {
            public readonly T2 Value;

            public ValueOfType2(T2 value) => Value = value;

            public override bool IsOption2() => true;

            public override bool IsOption2(out T2 value)
            {
                value = Value;
                return true;
            }

            public override T2 ToOption2() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption2 != null ? whenOption2(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(2);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption2 != null) whenOption2(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType3 : Union<T1, T2, T3, T4, T5>
        {
            public readonly T3 Value;

            public ValueOfType3(T3 value) => Value = value;

            public override bool IsOption3() => true;

            public override bool IsOption3(out T3 value)
            {
                value = Value;
                return true;
            }

            public override T3 ToOption3() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption3 != null ? whenOption3(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(3);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption3 != null) whenOption3(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType4 : Union<T1, T2, T3, T4, T5>
        {
            public readonly T4 Value;

            public ValueOfType4(T4 value) => Value = value;

            public override bool IsOption4() => true;

            public override bool IsOption4(out T4 value)
            {
                value = Value;
                return true;
            }

            public override T4 ToOption4() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption4 != null ? whenOption4(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(4);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption4 != null) whenOption4(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType5 : Union<T1, T2, T3, T4, T5>
        {
            public readonly T5 Value;

            public ValueOfType5(T5 value) => Value = value;

            public override bool IsOption5() => true;

            public override bool IsOption5(out T5 value)
            {
                value = Value;
                return true;
            }

            public override T5 ToOption5() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption5 != null ? whenOption5(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(5);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption5 != null) whenOption5(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Defines methods to test <see cref="Union{T1, T2, T3, T4, T5}"/> values for equality and generate their hash codes.
        /// </summary>
        public class EqualityComparer
        {
        }

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5}"/> with a value of the first type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5> Option1(T1 value) => new ValueOfType1(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5}"/> with a value of the second type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5> Option2(T2 value) => new ValueOfType2(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5}"/> with a value of the third type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5> Option3(T3 value) => new ValueOfType3(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5}"/> with a value of the fourth type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5> Option4(T4 value) => new ValueOfType4(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5}"/> with a value of the fifth type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5> Option5(T5 value) => new ValueOfType5(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5>(T1 value) => new ValueOfType1(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5>(T2 value) => new ValueOfType2(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5>(T3 value) => new ValueOfType3(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5>(T4 value) => new ValueOfType4(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5>(T5 value) => new ValueOfType5(value);

        private Union() { }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5}"/> contains a value of the first type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5}"/> contains a value of the first type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption1() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5}"/> contains a value of the second type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5}"/> contains a value of the second type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption2() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5}"/> contains a value of the third type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5}"/> contains a value of the third type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption3() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5}"/> contains a value of the fourth type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5}"/> contains a value of the fourth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption4() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5}"/> contains a value of the fifth type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5}"/> contains a value of the fifth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption5() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5}"/> contains a value of the first type.
        /// </summary>
        /// <param name="value">
        /// The value of the first type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5}"/> contains a value of the first type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption1([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T1 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5}"/> contains a value of the second type.
        /// </summary>
        /// <param name="value">
        /// The value of the second type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5}"/> contains a value of the second type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption2([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T2 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5}"/> contains a value of the third type.
        /// </summary>
        /// <param name="value">
        /// The value of the third type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5}"/> contains a value of the third type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption3([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T3 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5}"/> contains a value of the fourth type.
        /// </summary>
        /// <param name="value">
        /// The value of the fourth type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5}"/> contains a value of the fourth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption4([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T4 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5}"/> contains a value of the fifth type.
        /// </summary>
        /// <param name="value">
        /// The value of the fifth type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5}"/> contains a value of the fifth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption5([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T5 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5}"/> to a value of the first type.
        /// </summary>
        /// <returns>
        /// The value of the first type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5}"/> does not contain a value of the first type.
        /// </exception>
        public virtual T1 ToOption1() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5}"/> to a value of the second type.
        /// </summary>
        /// <returns>
        /// The value of the second type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5}"/> does not contain a value of the second type.
        /// </exception>
        public virtual T2 ToOption2() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5}"/> to a value of the third type.
        /// </summary>
        /// <returns>
        /// The value of the third type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5}"/> does not contain a value of the third type.
        /// </exception>
        public virtual T3 ToOption3() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5}"/> to a value of the fourth type.
        /// </summary>
        /// <returns>
        /// The value of the fourth type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5}"/> does not contain a value of the fourth type.
        /// </exception>
        public virtual T4 ToOption4() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5}"/> to a value of the fifth type.
        /// </summary>
        /// <returns>
        /// The value of the fifth type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5}"/> does not contain a value of the fifth type.
        /// </exception>
        public virtual T5 ToOption5() => throw new InvalidCastException();

        /// <summary>
        /// Invokes a <see cref="Func{T, TResult}"/> based on the type of the value and returns its result.
        /// </summary>
        /// <typeparam name="TResult">
        /// Type of the value to return.
        /// </typeparam>
        /// <param name="whenOption1">
        /// The <see cref="Func{T1, TResult}"/> to invoke when the value is of the first type.
        /// </param>
        /// <param name="whenOption2">
        /// The <see cref="Func{T2, TResult}"/> to invoke when the value is of the second type.
        /// </param>
        /// <param name="whenOption3">
        /// The <see cref="Func{T3, TResult}"/> to invoke when the value is of the third type.
        /// </param>
        /// <param name="whenOption4">
        /// The <see cref="Func{T4, TResult}"/> to invoke when the value is of the fourth type.
        /// </param>
        /// <param name="whenOption5">
        /// The <see cref="Func{T5, TResult}"/> to invoke when the value is of the fifth type.
        /// </param>
        /// <param name="otherwise">
        /// The <see cref="Func{TResult}"/> to invoke if no function is specified for the type of the value.
        /// If <paramref name="whenOption1"/>, <paramref name="whenOption2"/>, <paramref name="whenOption3"/>, <paramref name="whenOption4"/> and <paramref name="whenOption5"/> are given, this parameter is not used.
        /// </param>
        /// <returns>
        /// The result of the invoked <see cref="Func{T, TResult}"/>.
        /// </returns>
        /// <exception cref="InvalidPatternMatchException">
        /// No function argument was defined that matches the value.
        /// </exception>
        public abstract TResult Match<TResult>(
            [AllowNull] Func<T1, TResult> whenOption1 = null,
            [AllowNull] Func<T2, TResult> whenOption2 = null,
            [AllowNull] Func<T3, TResult> whenOption3 = null,
            [AllowNull] Func<T4, TResult> whenOption4 = null,
            [AllowNull] Func<T5, TResult> whenOption5 = null,
            [AllowNull] Func<TResult> otherwise = null)
#if !NET472
            where TResult : notnull
#endif
            ;

        /// <summary>
        /// Invokes an <see cref="Action{T}"/> based on the type of the value.
        /// </summary>
        /// <param name="whenOption1">
        /// The <see cref="Action{T1}"/> to invoke when the value is of the first type.
        /// </param>
        /// <param name="whenOption2">
        /// The <see cref="Action{T2}"/> to invoke when the value is of the second type.
        /// </param>
        /// <param name="whenOption3">
        /// The <see cref="Action{T3}"/> to invoke when the value is of the third type.
        /// </param>
        /// <param name="whenOption4">
        /// The <see cref="Action{T4}"/> to invoke when the value is of the fourth type.
        /// </param>
        /// <param name="whenOption5">
        /// The <see cref="Action{T5}"/> to invoke when the value is of the fifth type.
        /// </param>
        /// <param name="otherwise">
        /// The <see cref="Action"/> to invoke if no action is specified for the type of the value.
        /// If <paramref name="whenOption1"/>, <paramref name="whenOption2"/>, <paramref name="whenOption3"/>, <paramref name="whenOption4"/> and <paramref name="whenOption5"/> are given, this parameter is not used.
        /// </param>
        public abstract void Match(
            [AllowNull] Action<T1> whenOption1 = null,
            [AllowNull] Action<T2> whenOption2 = null,
            [AllowNull] Action<T3> whenOption3 = null,
            [AllowNull] Action<T4> whenOption4 = null,
            [AllowNull] Action<T5> whenOption5 = null,
            [AllowNull] Action otherwise = null);
    }

    /// <summary>
    /// Encapsulates a value which can have six different types.
    /// </summary>
    /// <typeparam name="T1">
    /// The first type of the value.
    /// </typeparam>
    /// <typeparam name="T2">
    /// The second type of the value.
    /// </typeparam>
    /// <typeparam name="T3">
    /// The third type of the value.
    /// </typeparam>
    /// <typeparam name="T4">
    /// The fourth type of the value.
    /// </typeparam>
    /// <typeparam name="T5">
    /// The fifth type of the value.
    /// </typeparam>
    /// <typeparam name="T6">
    /// The sixth type of the value.
    /// </typeparam>
    /// <remarks>
    /// This type is deliberately implemented without equality or hash code implementations,
    /// as these would have to make assumptions on equality of the contained object or value.
    /// </remarks>
    public abstract class Union<T1, T2, T3, T4, T5, T6>
#if !NET472
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
        where T5 : notnull
        where T6 : notnull
#endif
    {
        private sealed class ValueOfType1 : Union<T1, T2, T3, T4, T5, T6>
        {
            public readonly T1 Value;

            public ValueOfType1(T1 value) => Value = value;

            public override bool IsOption1() => true;

            public override bool IsOption1(out T1 value)
            {
                value = Value;
                return true;
            }

            public override T1 ToOption1() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<T6, TResult> whenOption6 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption1 != null ? whenOption1(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(1);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action<T6> whenOption6 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption1 != null) whenOption1(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType2 : Union<T1, T2, T3, T4, T5, T6>
        {
            public readonly T2 Value;

            public ValueOfType2(T2 value) => Value = value;

            public override bool IsOption2() => true;

            public override bool IsOption2(out T2 value)
            {
                value = Value;
                return true;
            }

            public override T2 ToOption2() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<T6, TResult> whenOption6 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption2 != null ? whenOption2(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(2);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action<T6> whenOption6 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption2 != null) whenOption2(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType3 : Union<T1, T2, T3, T4, T5, T6>
        {
            public readonly T3 Value;

            public ValueOfType3(T3 value) => Value = value;

            public override bool IsOption3() => true;

            public override bool IsOption3(out T3 value)
            {
                value = Value;
                return true;
            }

            public override T3 ToOption3() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<T6, TResult> whenOption6 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption3 != null ? whenOption3(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(3);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action<T6> whenOption6 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption3 != null) whenOption3(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType4 : Union<T1, T2, T3, T4, T5, T6>
        {
            public readonly T4 Value;

            public ValueOfType4(T4 value) => Value = value;

            public override bool IsOption4() => true;

            public override bool IsOption4(out T4 value)
            {
                value = Value;
                return true;
            }

            public override T4 ToOption4() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<T6, TResult> whenOption6 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption4 != null ? whenOption4(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(4);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action<T6> whenOption6 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption4 != null) whenOption4(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType5 : Union<T1, T2, T3, T4, T5, T6>
        {
            public readonly T5 Value;

            public ValueOfType5(T5 value) => Value = value;

            public override bool IsOption5() => true;

            public override bool IsOption5(out T5 value)
            {
                value = Value;
                return true;
            }

            public override T5 ToOption5() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<T6, TResult> whenOption6 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption5 != null ? whenOption5(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(5);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action<T6> whenOption6 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption5 != null) whenOption5(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType6 : Union<T1, T2, T3, T4, T5, T6>
        {
            public readonly T6 Value;

            public ValueOfType6(T6 value) => Value = value;

            public override bool IsOption6() => true;

            public override bool IsOption6(out T6 value)
            {
                value = Value;
                return true;
            }

            public override T6 ToOption6() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<T6, TResult> whenOption6 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption6 != null ? whenOption6(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(6);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action<T6> whenOption6 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption6 != null) whenOption6(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Defines methods to test <see cref="Union{T1, T2, T3, T4, T5, T6}"/> values for equality and generate their hash codes.
        /// </summary>
        public class EqualityComparer
        {
        }

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5, T6}"/> with a value of the first type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5, T6> Option1(T1 value) => new ValueOfType1(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5, T6}"/> with a value of the second type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5, T6> Option2(T2 value) => new ValueOfType2(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5, T6}"/> with a value of the third type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5, T6> Option3(T3 value) => new ValueOfType3(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5, T6}"/> with a value of the fourth type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5, T6> Option4(T4 value) => new ValueOfType4(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5, T6}"/> with a value of the fifth type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5, T6> Option5(T5 value) => new ValueOfType5(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5, T6}"/> with a value of the sixth type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5, T6> Option6(T6 value) => new ValueOfType6(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T1 value) => new ValueOfType1(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T2 value) => new ValueOfType2(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T3 value) => new ValueOfType3(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T4 value) => new ValueOfType4(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T5 value) => new ValueOfType5(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T6 value) => new ValueOfType6(value);

        private Union() { }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the first type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the first type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption1() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the second type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the second type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption2() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the third type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the third type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption3() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the fourth type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the fourth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption4() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the fifth type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the fifth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption5() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the sixth type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the sixth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption6() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the first type.
        /// </summary>
        /// <param name="value">
        /// The value of the first type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the first type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption1([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T1 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the second type.
        /// </summary>
        /// <param name="value">
        /// The value of the second type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the second type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption2([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T2 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the third type.
        /// </summary>
        /// <param name="value">
        /// The value of the third type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the third type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption3([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T3 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the fourth type.
        /// </summary>
        /// <param name="value">
        /// The value of the fourth type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the fourth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption4([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T4 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the fifth type.
        /// </summary>
        /// <param name="value">
        /// The value of the fifth type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the fifth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption5([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T5 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the sixth type.
        /// </summary>
        /// <param name="value">
        /// The value of the sixth type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> contains a value of the sixth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption6([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T6 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> to a value of the first type.
        /// </summary>
        /// <returns>
        /// The value of the first type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> does not contain a value of the first type.
        /// </exception>
        public virtual T1 ToOption1() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> to a value of the second type.
        /// </summary>
        /// <returns>
        /// The value of the second type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> does not contain a value of the second type.
        /// </exception>
        public virtual T2 ToOption2() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> to a value of the third type.
        /// </summary>
        /// <returns>
        /// The value of the third type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> does not contain a value of the third type.
        /// </exception>
        public virtual T3 ToOption3() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> to a value of the fourth type.
        /// </summary>
        /// <returns>
        /// The value of the fourth type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> does not contain a value of the fourth type.
        /// </exception>
        public virtual T4 ToOption4() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> to a value of the fifth type.
        /// </summary>
        /// <returns>
        /// The value of the fifth type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> does not contain a value of the fifth type.
        /// </exception>
        public virtual T5 ToOption5() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> to a value of the sixth type.
        /// </summary>
        /// <returns>
        /// The value of the sixth type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5, T6}"/> does not contain a value of the sixth type.
        /// </exception>
        public virtual T6 ToOption6() => throw new InvalidCastException();

        /// <summary>
        /// Invokes a <see cref="Func{T, TResult}"/> based on the type of the value and returns its result.
        /// </summary>
        /// <typeparam name="TResult">
        /// Type of the value to return.
        /// </typeparam>
        /// <param name="whenOption1">
        /// The <see cref="Func{T1, TResult}"/> to invoke when the value is of the first type.
        /// </param>
        /// <param name="whenOption2">
        /// The <see cref="Func{T2, TResult}"/> to invoke when the value is of the second type.
        /// </param>
        /// <param name="whenOption3">
        /// The <see cref="Func{T3, TResult}"/> to invoke when the value is of the third type.
        /// </param>
        /// <param name="whenOption4">
        /// The <see cref="Func{T4, TResult}"/> to invoke when the value is of the fourth type.
        /// </param>
        /// <param name="whenOption5">
        /// The <see cref="Func{T5, TResult}"/> to invoke when the value is of the fifth type.
        /// </param>
        /// <param name="whenOption6">
        /// The <see cref="Func{T6, TResult}"/> to invoke when the value is of the sixth type.
        /// </param>
        /// <param name="otherwise">
        /// The <see cref="Func{TResult}"/> to invoke if no function is specified for the type of the value.
        /// If <paramref name="whenOption1"/>, <paramref name="whenOption2"/>, <paramref name="whenOption3"/>, <paramref name="whenOption4"/>, <paramref name="whenOption5"/> and <paramref name="whenOption6"/> are given, this parameter is not used.
        /// </param>
        /// <returns>
        /// The result of the invoked <see cref="Func{T, TResult}"/>.
        /// </returns>
        /// <exception cref="InvalidPatternMatchException">
        /// No function argument was defined that matches the value.
        /// </exception>
        public abstract TResult Match<TResult>(
            [AllowNull] Func<T1, TResult> whenOption1 = null,
            [AllowNull] Func<T2, TResult> whenOption2 = null,
            [AllowNull] Func<T3, TResult> whenOption3 = null,
            [AllowNull] Func<T4, TResult> whenOption4 = null,
            [AllowNull] Func<T5, TResult> whenOption5 = null,
            [AllowNull] Func<T6, TResult> whenOption6 = null,
            [AllowNull] Func<TResult> otherwise = null)
#if !NET472
            where TResult : notnull
#endif
            ;

        /// <summary>
        /// Invokes an <see cref="Action{T}"/> based on the type of the value.
        /// </summary>
        /// <param name="whenOption1">
        /// The <see cref="Action{T1}"/> to invoke when the value is of the first type.
        /// </param>
        /// <param name="whenOption2">
        /// The <see cref="Action{T2}"/> to invoke when the value is of the second type.
        /// </param>
        /// <param name="whenOption3">
        /// The <see cref="Action{T3}"/> to invoke when the value is of the third type.
        /// </param>
        /// <param name="whenOption4">
        /// The <see cref="Action{T4}"/> to invoke when the value is of the fourth type.
        /// </param>
        /// <param name="whenOption5">
        /// The <see cref="Action{T5}"/> to invoke when the value is of the fifth type.
        /// </param>
        /// <param name="whenOption6">
        /// The <see cref="Action{T6}"/> to invoke when the value is of the sixth type.
        /// </param>
        /// <param name="otherwise">
        /// The <see cref="Action"/> to invoke if no action is specified for the type of the value.
        /// If <paramref name="whenOption1"/>, <paramref name="whenOption2"/>, <paramref name="whenOption3"/>, <paramref name="whenOption4"/>, <paramref name="whenOption5"/> and <paramref name="whenOption6"/> are given, this parameter is not used.
        /// </param>
        public abstract void Match(
            [AllowNull] Action<T1> whenOption1 = null,
            [AllowNull] Action<T2> whenOption2 = null,
            [AllowNull] Action<T3> whenOption3 = null,
            [AllowNull] Action<T4> whenOption4 = null,
            [AllowNull] Action<T5> whenOption5 = null,
            [AllowNull] Action<T6> whenOption6 = null,
            [AllowNull] Action otherwise = null);
    }

    /// <summary>
    /// Encapsulates a value which can have seven different types.
    /// </summary>
    /// <typeparam name="T1">
    /// The first type of the value.
    /// </typeparam>
    /// <typeparam name="T2">
    /// The second type of the value.
    /// </typeparam>
    /// <typeparam name="T3">
    /// The third type of the value.
    /// </typeparam>
    /// <typeparam name="T4">
    /// The fourth type of the value.
    /// </typeparam>
    /// <typeparam name="T5">
    /// The fifth type of the value.
    /// </typeparam>
    /// <typeparam name="T6">
    /// The sixth type of the value.
    /// </typeparam>
    /// <typeparam name="T7">
    /// The seventh type of the value.
    /// </typeparam>
    /// <remarks>
    /// This type is deliberately implemented without equality or hash code implementations,
    /// as these would have to make assumptions on equality of the contained object or value.
    /// </remarks>
    public abstract class Union<T1, T2, T3, T4, T5, T6, T7>
#if !NET472
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
        where T5 : notnull
        where T6 : notnull
        where T7 : notnull
#endif
    {
        private sealed class ValueOfType1 : Union<T1, T2, T3, T4, T5, T6, T7>
        {
            public readonly T1 Value;

            public ValueOfType1(T1 value) => Value = value;

            public override bool IsOption1() => true;

            public override bool IsOption1(out T1 value)
            {
                value = Value;
                return true;
            }

            public override T1 ToOption1() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<T6, TResult> whenOption6 = null,
                [AllowNull] Func<T7, TResult> whenOption7 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption1 != null ? whenOption1(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(1);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action<T6> whenOption6 = null,
                [AllowNull] Action<T7> whenOption7 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption1 != null) whenOption1(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType2 : Union<T1, T2, T3, T4, T5, T6, T7>
        {
            public readonly T2 Value;

            public ValueOfType2(T2 value) => Value = value;

            public override bool IsOption2() => true;

            public override bool IsOption2(out T2 value)
            {
                value = Value;
                return true;
            }

            public override T2 ToOption2() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<T6, TResult> whenOption6 = null,
                [AllowNull] Func<T7, TResult> whenOption7 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption2 != null ? whenOption2(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(2);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action<T6> whenOption6 = null,
                [AllowNull] Action<T7> whenOption7 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption2 != null) whenOption2(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType3 : Union<T1, T2, T3, T4, T5, T6, T7>
        {
            public readonly T3 Value;

            public ValueOfType3(T3 value) => Value = value;

            public override bool IsOption3() => true;

            public override bool IsOption3(out T3 value)
            {
                value = Value;
                return true;
            }

            public override T3 ToOption3() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<T6, TResult> whenOption6 = null,
                [AllowNull] Func<T7, TResult> whenOption7 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption3 != null ? whenOption3(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(3);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action<T6> whenOption6 = null,
                [AllowNull] Action<T7> whenOption7 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption3 != null) whenOption3(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType4 : Union<T1, T2, T3, T4, T5, T6, T7>
        {
            public readonly T4 Value;

            public ValueOfType4(T4 value) => Value = value;

            public override bool IsOption4() => true;

            public override bool IsOption4(out T4 value)
            {
                value = Value;
                return true;
            }

            public override T4 ToOption4() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<T6, TResult> whenOption6 = null,
                [AllowNull] Func<T7, TResult> whenOption7 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption4 != null ? whenOption4(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(4);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action<T6> whenOption6 = null,
                [AllowNull] Action<T7> whenOption7 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption4 != null) whenOption4(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType5 : Union<T1, T2, T3, T4, T5, T6, T7>
        {
            public readonly T5 Value;

            public ValueOfType5(T5 value) => Value = value;

            public override bool IsOption5() => true;

            public override bool IsOption5(out T5 value)
            {
                value = Value;
                return true;
            }

            public override T5 ToOption5() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<T6, TResult> whenOption6 = null,
                [AllowNull] Func<T7, TResult> whenOption7 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption5 != null ? whenOption5(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(5);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action<T6> whenOption6 = null,
                [AllowNull] Action<T7> whenOption7 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption5 != null) whenOption5(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType6 : Union<T1, T2, T3, T4, T5, T6, T7>
        {
            public readonly T6 Value;

            public ValueOfType6(T6 value) => Value = value;

            public override bool IsOption6() => true;

            public override bool IsOption6(out T6 value)
            {
                value = Value;
                return true;
            }

            public override T6 ToOption6() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<T6, TResult> whenOption6 = null,
                [AllowNull] Func<T7, TResult> whenOption7 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption6 != null ? whenOption6(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(6);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action<T6> whenOption6 = null,
                [AllowNull] Action<T7> whenOption7 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption6 != null) whenOption6(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType7 : Union<T1, T2, T3, T4, T5, T6, T7>
        {
            public readonly T7 Value;

            public ValueOfType7(T7 value) => Value = value;

            public override bool IsOption7() => true;

            public override bool IsOption7(out T7 value)
            {
                value = Value;
                return true;
            }

            public override T7 ToOption7() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<T6, TResult> whenOption6 = null,
                [AllowNull] Func<T7, TResult> whenOption7 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption7 != null ? whenOption7(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(7);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action<T6> whenOption6 = null,
                [AllowNull] Action<T7> whenOption7 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption7 != null) whenOption7(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Defines methods to test <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> values for equality and generate their hash codes.
        /// </summary>
        public class EqualityComparer
        {
        }

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> with a value of the first type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5, T6, T7> Option1(T1 value) => new ValueOfType1(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> with a value of the second type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5, T6, T7> Option2(T2 value) => new ValueOfType2(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> with a value of the third type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5, T6, T7> Option3(T3 value) => new ValueOfType3(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> with a value of the fourth type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5, T6, T7> Option4(T4 value) => new ValueOfType4(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> with a value of the fifth type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5, T6, T7> Option5(T5 value) => new ValueOfType5(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> with a value of the sixth type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5, T6, T7> Option6(T6 value) => new ValueOfType6(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> with a value of the seventh type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5, T6, T7> Option7(T7 value) => new ValueOfType7(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T1 value) => new ValueOfType1(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T2 value) => new ValueOfType2(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T3 value) => new ValueOfType3(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T4 value) => new ValueOfType4(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T5 value) => new ValueOfType5(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T6 value) => new ValueOfType6(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T7 value) => new ValueOfType7(value);

        private Union() { }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the first type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the first type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption1() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the second type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the second type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption2() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the third type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the third type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption3() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the fourth type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the fourth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption4() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the fifth type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the fifth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption5() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the sixth type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the sixth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption6() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the seventh type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the seventh type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption7() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the first type.
        /// </summary>
        /// <param name="value">
        /// The value of the first type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the first type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption1([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T1 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the second type.
        /// </summary>
        /// <param name="value">
        /// The value of the second type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the second type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption2([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T2 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the third type.
        /// </summary>
        /// <param name="value">
        /// The value of the third type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the third type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption3([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T3 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the fourth type.
        /// </summary>
        /// <param name="value">
        /// The value of the fourth type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the fourth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption4([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T4 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the fifth type.
        /// </summary>
        /// <param name="value">
        /// The value of the fifth type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the fifth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption5([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T5 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the sixth type.
        /// </summary>
        /// <param name="value">
        /// The value of the sixth type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the sixth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption6([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T6 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the seventh type.
        /// </summary>
        /// <param name="value">
        /// The value of the seventh type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> contains a value of the seventh type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption7([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T7 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> to a value of the first type.
        /// </summary>
        /// <returns>
        /// The value of the first type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> does not contain a value of the first type.
        /// </exception>
        public virtual T1 ToOption1() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> to a value of the second type.
        /// </summary>
        /// <returns>
        /// The value of the second type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> does not contain a value of the second type.
        /// </exception>
        public virtual T2 ToOption2() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> to a value of the third type.
        /// </summary>
        /// <returns>
        /// The value of the third type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> does not contain a value of the third type.
        /// </exception>
        public virtual T3 ToOption3() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> to a value of the fourth type.
        /// </summary>
        /// <returns>
        /// The value of the fourth type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> does not contain a value of the fourth type.
        /// </exception>
        public virtual T4 ToOption4() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> to a value of the fifth type.
        /// </summary>
        /// <returns>
        /// The value of the fifth type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> does not contain a value of the fifth type.
        /// </exception>
        public virtual T5 ToOption5() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> to a value of the sixth type.
        /// </summary>
        /// <returns>
        /// The value of the sixth type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> does not contain a value of the sixth type.
        /// </exception>
        public virtual T6 ToOption6() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> to a value of the seventh type.
        /// </summary>
        /// <returns>
        /// The value of the seventh type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5, T6, T7}"/> does not contain a value of the seventh type.
        /// </exception>
        public virtual T7 ToOption7() => throw new InvalidCastException();

        /// <summary>
        /// Invokes a <see cref="Func{T, TResult}"/> based on the type of the value and returns its result.
        /// </summary>
        /// <typeparam name="TResult">
        /// Type of the value to return.
        /// </typeparam>
        /// <param name="whenOption1">
        /// The <see cref="Func{T1, TResult}"/> to invoke when the value is of the first type.
        /// </param>
        /// <param name="whenOption2">
        /// The <see cref="Func{T2, TResult}"/> to invoke when the value is of the second type.
        /// </param>
        /// <param name="whenOption3">
        /// The <see cref="Func{T3, TResult}"/> to invoke when the value is of the third type.
        /// </param>
        /// <param name="whenOption4">
        /// The <see cref="Func{T4, TResult}"/> to invoke when the value is of the fourth type.
        /// </param>
        /// <param name="whenOption5">
        /// The <see cref="Func{T5, TResult}"/> to invoke when the value is of the fifth type.
        /// </param>
        /// <param name="whenOption6">
        /// The <see cref="Func{T6, TResult}"/> to invoke when the value is of the sixth type.
        /// </param>
        /// <param name="whenOption7">
        /// The <see cref="Func{T7, TResult}"/> to invoke when the value is of the seventh type.
        /// </param>
        /// <param name="otherwise">
        /// The <see cref="Func{TResult}"/> to invoke if no function is specified for the type of the value.
        /// If <paramref name="whenOption1"/>, <paramref name="whenOption2"/>, <paramref name="whenOption3"/>, <paramref name="whenOption4"/>, <paramref name="whenOption5"/>, <paramref name="whenOption6"/> and <paramref name="whenOption7"/> are given, this parameter is not used.
        /// </param>
        /// <returns>
        /// The result of the invoked <see cref="Func{T, TResult}"/>.
        /// </returns>
        /// <exception cref="InvalidPatternMatchException">
        /// No function argument was defined that matches the value.
        /// </exception>
        public abstract TResult Match<TResult>(
            [AllowNull] Func<T1, TResult> whenOption1 = null,
            [AllowNull] Func<T2, TResult> whenOption2 = null,
            [AllowNull] Func<T3, TResult> whenOption3 = null,
            [AllowNull] Func<T4, TResult> whenOption4 = null,
            [AllowNull] Func<T5, TResult> whenOption5 = null,
            [AllowNull] Func<T6, TResult> whenOption6 = null,
            [AllowNull] Func<T7, TResult> whenOption7 = null,
            [AllowNull] Func<TResult> otherwise = null)
#if !NET472
            where TResult : notnull
#endif
            ;

        /// <summary>
        /// Invokes an <see cref="Action{T}"/> based on the type of the value.
        /// </summary>
        /// <param name="whenOption1">
        /// The <see cref="Action{T1}"/> to invoke when the value is of the first type.
        /// </param>
        /// <param name="whenOption2">
        /// The <see cref="Action{T2}"/> to invoke when the value is of the second type.
        /// </param>
        /// <param name="whenOption3">
        /// The <see cref="Action{T3}"/> to invoke when the value is of the third type.
        /// </param>
        /// <param name="whenOption4">
        /// The <see cref="Action{T4}"/> to invoke when the value is of the fourth type.
        /// </param>
        /// <param name="whenOption5">
        /// The <see cref="Action{T5}"/> to invoke when the value is of the fifth type.
        /// </param>
        /// <param name="whenOption6">
        /// The <see cref="Action{T6}"/> to invoke when the value is of the sixth type.
        /// </param>
        /// <param name="whenOption7">
        /// The <see cref="Action{T7}"/> to invoke when the value is of the seventh type.
        /// </param>
        /// <param name="otherwise">
        /// The <see cref="Action"/> to invoke if no action is specified for the type of the value.
        /// If <paramref name="whenOption1"/>, <paramref name="whenOption2"/>, <paramref name="whenOption3"/>, <paramref name="whenOption4"/>, <paramref name="whenOption5"/>, <paramref name="whenOption6"/> and <paramref name="whenOption7"/> are given, this parameter is not used.
        /// </param>
        public abstract void Match(
            [AllowNull] Action<T1> whenOption1 = null,
            [AllowNull] Action<T2> whenOption2 = null,
            [AllowNull] Action<T3> whenOption3 = null,
            [AllowNull] Action<T4> whenOption4 = null,
            [AllowNull] Action<T5> whenOption5 = null,
            [AllowNull] Action<T6> whenOption6 = null,
            [AllowNull] Action<T7> whenOption7 = null,
            [AllowNull] Action otherwise = null);
    }

    /// <summary>
    /// Encapsulates a value which can have eight different types.
    /// </summary>
    /// <typeparam name="T1">
    /// The first type of the value.
    /// </typeparam>
    /// <typeparam name="T2">
    /// The second type of the value.
    /// </typeparam>
    /// <typeparam name="T3">
    /// The third type of the value.
    /// </typeparam>
    /// <typeparam name="T4">
    /// The fourth type of the value.
    /// </typeparam>
    /// <typeparam name="T5">
    /// The fifth type of the value.
    /// </typeparam>
    /// <typeparam name="T6">
    /// The sixth type of the value.
    /// </typeparam>
    /// <typeparam name="T7">
    /// The seventh type of the value.
    /// </typeparam>
    /// <typeparam name="T8">
    /// The eighth type of the value.
    /// </typeparam>
    /// <remarks>
    /// This type is deliberately implemented without equality or hash code implementations,
    /// as these would have to make assumptions on equality of the contained object or value.
    /// </remarks>
    public abstract class Union<T1, T2, T3, T4, T5, T6, T7, T8>
#if !NET472
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
        where T5 : notnull
        where T6 : notnull
        where T7 : notnull
        where T8 : notnull
#endif
    {
        private sealed class ValueOfType1 : Union<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            public readonly T1 Value;

            public ValueOfType1(T1 value) => Value = value;

            public override bool IsOption1() => true;

            public override bool IsOption1(out T1 value)
            {
                value = Value;
                return true;
            }

            public override T1 ToOption1() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<T6, TResult> whenOption6 = null,
                [AllowNull] Func<T7, TResult> whenOption7 = null,
                [AllowNull] Func<T8, TResult> whenOption8 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption1 != null ? whenOption1(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(1);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action<T6> whenOption6 = null,
                [AllowNull] Action<T7> whenOption7 = null,
                [AllowNull] Action<T8> whenOption8 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption1 != null) whenOption1(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType2 : Union<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            public readonly T2 Value;

            public ValueOfType2(T2 value) => Value = value;

            public override bool IsOption2() => true;

            public override bool IsOption2(out T2 value)
            {
                value = Value;
                return true;
            }

            public override T2 ToOption2() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<T6, TResult> whenOption6 = null,
                [AllowNull] Func<T7, TResult> whenOption7 = null,
                [AllowNull] Func<T8, TResult> whenOption8 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption2 != null ? whenOption2(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(2);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action<T6> whenOption6 = null,
                [AllowNull] Action<T7> whenOption7 = null,
                [AllowNull] Action<T8> whenOption8 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption2 != null) whenOption2(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType3 : Union<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            public readonly T3 Value;

            public ValueOfType3(T3 value) => Value = value;

            public override bool IsOption3() => true;

            public override bool IsOption3(out T3 value)
            {
                value = Value;
                return true;
            }

            public override T3 ToOption3() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<T6, TResult> whenOption6 = null,
                [AllowNull] Func<T7, TResult> whenOption7 = null,
                [AllowNull] Func<T8, TResult> whenOption8 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption3 != null ? whenOption3(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(3);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action<T6> whenOption6 = null,
                [AllowNull] Action<T7> whenOption7 = null,
                [AllowNull] Action<T8> whenOption8 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption3 != null) whenOption3(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType4 : Union<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            public readonly T4 Value;

            public ValueOfType4(T4 value) => Value = value;

            public override bool IsOption4() => true;

            public override bool IsOption4(out T4 value)
            {
                value = Value;
                return true;
            }

            public override T4 ToOption4() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<T6, TResult> whenOption6 = null,
                [AllowNull] Func<T7, TResult> whenOption7 = null,
                [AllowNull] Func<T8, TResult> whenOption8 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption4 != null ? whenOption4(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(4);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action<T6> whenOption6 = null,
                [AllowNull] Action<T7> whenOption7 = null,
                [AllowNull] Action<T8> whenOption8 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption4 != null) whenOption4(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType5 : Union<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            public readonly T5 Value;

            public ValueOfType5(T5 value) => Value = value;

            public override bool IsOption5() => true;

            public override bool IsOption5(out T5 value)
            {
                value = Value;
                return true;
            }

            public override T5 ToOption5() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<T6, TResult> whenOption6 = null,
                [AllowNull] Func<T7, TResult> whenOption7 = null,
                [AllowNull] Func<T8, TResult> whenOption8 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption5 != null ? whenOption5(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(5);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action<T6> whenOption6 = null,
                [AllowNull] Action<T7> whenOption7 = null,
                [AllowNull] Action<T8> whenOption8 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption5 != null) whenOption5(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType6 : Union<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            public readonly T6 Value;

            public ValueOfType6(T6 value) => Value = value;

            public override bool IsOption6() => true;

            public override bool IsOption6(out T6 value)
            {
                value = Value;
                return true;
            }

            public override T6 ToOption6() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<T6, TResult> whenOption6 = null,
                [AllowNull] Func<T7, TResult> whenOption7 = null,
                [AllowNull] Func<T8, TResult> whenOption8 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption6 != null ? whenOption6(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(6);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action<T6> whenOption6 = null,
                [AllowNull] Action<T7> whenOption7 = null,
                [AllowNull] Action<T8> whenOption8 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption6 != null) whenOption6(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType7 : Union<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            public readonly T7 Value;

            public ValueOfType7(T7 value) => Value = value;

            public override bool IsOption7() => true;

            public override bool IsOption7(out T7 value)
            {
                value = Value;
                return true;
            }

            public override T7 ToOption7() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<T6, TResult> whenOption6 = null,
                [AllowNull] Func<T7, TResult> whenOption7 = null,
                [AllowNull] Func<T8, TResult> whenOption8 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption7 != null ? whenOption7(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(7);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action<T6> whenOption6 = null,
                [AllowNull] Action<T7> whenOption7 = null,
                [AllowNull] Action<T8> whenOption8 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption7 != null) whenOption7(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        private sealed class ValueOfType8 : Union<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            public readonly T8 Value;

            public ValueOfType8(T8 value) => Value = value;

            public override bool IsOption8() => true;

            public override bool IsOption8(out T8 value)
            {
                value = Value;
                return true;
            }

            public override T8 ToOption8() => Value;

            public override TResult Match<TResult>(
                [AllowNull] Func<T1, TResult> whenOption1 = null,
                [AllowNull] Func<T2, TResult> whenOption2 = null,
                [AllowNull] Func<T3, TResult> whenOption3 = null,
                [AllowNull] Func<T4, TResult> whenOption4 = null,
                [AllowNull] Func<T5, TResult> whenOption5 = null,
                [AllowNull] Func<T6, TResult> whenOption6 = null,
                [AllowNull] Func<T7, TResult> whenOption7 = null,
                [AllowNull] Func<T8, TResult> whenOption8 = null,
                [AllowNull] Func<TResult> otherwise = null)
                => whenOption8 != null ? whenOption8(Value)
                : otherwise != null ? otherwise()
                : throw Union.InvalidPatternMatchArgumentException(8);

            public override void Match(
                [AllowNull] Action<T1> whenOption1 = null,
                [AllowNull] Action<T2> whenOption2 = null,
                [AllowNull] Action<T3> whenOption3 = null,
                [AllowNull] Action<T4> whenOption4 = null,
                [AllowNull] Action<T5> whenOption5 = null,
                [AllowNull] Action<T6> whenOption6 = null,
                [AllowNull] Action<T7> whenOption7 = null,
                [AllowNull] Action<T8> whenOption8 = null,
                [AllowNull] Action otherwise = null)
            {
                if (whenOption8 != null) whenOption8(Value);
                else otherwise?.Invoke();
            }

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Defines methods to test <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> values for equality and generate their hash codes.
        /// </summary>
        public class EqualityComparer
        {
        }

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> with a value of the first type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5, T6, T7, T8> Option1(T1 value) => new ValueOfType1(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> with a value of the second type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5, T6, T7, T8> Option2(T2 value) => new ValueOfType2(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> with a value of the third type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5, T6, T7, T8> Option3(T3 value) => new ValueOfType3(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> with a value of the fourth type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5, T6, T7, T8> Option4(T4 value) => new ValueOfType4(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> with a value of the fifth type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5, T6, T7, T8> Option5(T5 value) => new ValueOfType5(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> with a value of the sixth type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5, T6, T7, T8> Option6(T6 value) => new ValueOfType6(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> with a value of the seventh type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5, T6, T7, T8> Option7(T7 value) => new ValueOfType7(value);

        /// <summary>
        /// Creates a new <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> with a value of the eighth type.
        /// </summary>
        public static Union<T1, T2, T3, T4, T5, T6, T7, T8> Option8(T8 value) => new ValueOfType8(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value) => new ValueOfType1(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T2 value) => new ValueOfType2(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T3 value) => new ValueOfType3(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T4 value) => new ValueOfType4(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T5 value) => new ValueOfType5(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T6 value) => new ValueOfType6(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T7 value) => new ValueOfType7(value);

        /// <summary>Converts a value to a Union instance.</summary>
        public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T8 value) => new ValueOfType8(value);

        private Union() { }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the first type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the first type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption1() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the second type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the second type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption2() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the third type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the third type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption3() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the fourth type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the fourth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption4() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the fifth type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the fifth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption5() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the sixth type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the sixth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption6() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the seventh type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the seventh type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption7() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the eighth type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the eighth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption8() => false;

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the first type.
        /// </summary>
        /// <param name="value">
        /// The value of the first type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the first type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption1([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T1 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the second type.
        /// </summary>
        /// <param name="value">
        /// The value of the second type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the second type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption2([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T2 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the third type.
        /// </summary>
        /// <param name="value">
        /// The value of the third type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the third type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption3([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T3 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the fourth type.
        /// </summary>
        /// <param name="value">
        /// The value of the fourth type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the fourth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption4([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T4 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the fifth type.
        /// </summary>
        /// <param name="value">
        /// The value of the fifth type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the fifth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption5([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T5 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the sixth type.
        /// </summary>
        /// <param name="value">
        /// The value of the sixth type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the sixth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption6([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T6 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the seventh type.
        /// </summary>
        /// <param name="value">
        /// The value of the seventh type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the seventh type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption7([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T7 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Checks if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the eighth type.
        /// </summary>
        /// <param name="value">
        /// The value of the eighth type, if this function returns <see langword="true"/>; otherwise a default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> contains a value of the eighth type; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsOption8([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T8 value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> to a value of the first type.
        /// </summary>
        /// <returns>
        /// The value of the first type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> does not contain a value of the first type.
        /// </exception>
        public virtual T1 ToOption1() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> to a value of the second type.
        /// </summary>
        /// <returns>
        /// The value of the second type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> does not contain a value of the second type.
        /// </exception>
        public virtual T2 ToOption2() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> to a value of the third type.
        /// </summary>
        /// <returns>
        /// The value of the third type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> does not contain a value of the third type.
        /// </exception>
        public virtual T3 ToOption3() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> to a value of the fourth type.
        /// </summary>
        /// <returns>
        /// The value of the fourth type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> does not contain a value of the fourth type.
        /// </exception>
        public virtual T4 ToOption4() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> to a value of the fifth type.
        /// </summary>
        /// <returns>
        /// The value of the fifth type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> does not contain a value of the fifth type.
        /// </exception>
        public virtual T5 ToOption5() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> to a value of the sixth type.
        /// </summary>
        /// <returns>
        /// The value of the sixth type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> does not contain a value of the sixth type.
        /// </exception>
        public virtual T6 ToOption6() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> to a value of the seventh type.
        /// </summary>
        /// <returns>
        /// The value of the seventh type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> does not contain a value of the seventh type.
        /// </exception>
        public virtual T7 ToOption7() => throw new InvalidCastException();

        /// <summary>
        /// Casts this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> to a value of the eighth type.
        /// </summary>
        /// <returns>
        /// The value of the eighth type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Occurs when this <see cref="Union{T1, T2, T3, T4, T5, T6, T7, T8}"/> does not contain a value of the eighth type.
        /// </exception>
        public virtual T8 ToOption8() => throw new InvalidCastException();

        /// <summary>
        /// Invokes a <see cref="Func{T, TResult}"/> based on the type of the value and returns its result.
        /// </summary>
        /// <typeparam name="TResult">
        /// Type of the value to return.
        /// </typeparam>
        /// <param name="whenOption1">
        /// The <see cref="Func{T1, TResult}"/> to invoke when the value is of the first type.
        /// </param>
        /// <param name="whenOption2">
        /// The <see cref="Func{T2, TResult}"/> to invoke when the value is of the second type.
        /// </param>
        /// <param name="whenOption3">
        /// The <see cref="Func{T3, TResult}"/> to invoke when the value is of the third type.
        /// </param>
        /// <param name="whenOption4">
        /// The <see cref="Func{T4, TResult}"/> to invoke when the value is of the fourth type.
        /// </param>
        /// <param name="whenOption5">
        /// The <see cref="Func{T5, TResult}"/> to invoke when the value is of the fifth type.
        /// </param>
        /// <param name="whenOption6">
        /// The <see cref="Func{T6, TResult}"/> to invoke when the value is of the sixth type.
        /// </param>
        /// <param name="whenOption7">
        /// The <see cref="Func{T7, TResult}"/> to invoke when the value is of the seventh type.
        /// </param>
        /// <param name="whenOption8">
        /// The <see cref="Func{T8, TResult}"/> to invoke when the value is of the eighth type.
        /// </param>
        /// <param name="otherwise">
        /// The <see cref="Func{TResult}"/> to invoke if no function is specified for the type of the value.
        /// If <paramref name="whenOption1"/>, <paramref name="whenOption2"/>, <paramref name="whenOption3"/>, <paramref name="whenOption4"/>, <paramref name="whenOption5"/>, <paramref name="whenOption6"/>, <paramref name="whenOption7"/> and <paramref name="whenOption8"/> are given, this parameter is not used.
        /// </param>
        /// <returns>
        /// The result of the invoked <see cref="Func{T, TResult}"/>.
        /// </returns>
        /// <exception cref="InvalidPatternMatchException">
        /// No function argument was defined that matches the value.
        /// </exception>
        public abstract TResult Match<TResult>(
            [AllowNull] Func<T1, TResult> whenOption1 = null,
            [AllowNull] Func<T2, TResult> whenOption2 = null,
            [AllowNull] Func<T3, TResult> whenOption3 = null,
            [AllowNull] Func<T4, TResult> whenOption4 = null,
            [AllowNull] Func<T5, TResult> whenOption5 = null,
            [AllowNull] Func<T6, TResult> whenOption6 = null,
            [AllowNull] Func<T7, TResult> whenOption7 = null,
            [AllowNull] Func<T8, TResult> whenOption8 = null,
            [AllowNull] Func<TResult> otherwise = null)
#if !NET472
            where TResult : notnull
#endif
            ;

        /// <summary>
        /// Invokes an <see cref="Action{T}"/> based on the type of the value.
        /// </summary>
        /// <param name="whenOption1">
        /// The <see cref="Action{T1}"/> to invoke when the value is of the first type.
        /// </param>
        /// <param name="whenOption2">
        /// The <see cref="Action{T2}"/> to invoke when the value is of the second type.
        /// </param>
        /// <param name="whenOption3">
        /// The <see cref="Action{T3}"/> to invoke when the value is of the third type.
        /// </param>
        /// <param name="whenOption4">
        /// The <see cref="Action{T4}"/> to invoke when the value is of the fourth type.
        /// </param>
        /// <param name="whenOption5">
        /// The <see cref="Action{T5}"/> to invoke when the value is of the fifth type.
        /// </param>
        /// <param name="whenOption6">
        /// The <see cref="Action{T6}"/> to invoke when the value is of the sixth type.
        /// </param>
        /// <param name="whenOption7">
        /// The <see cref="Action{T7}"/> to invoke when the value is of the seventh type.
        /// </param>
        /// <param name="whenOption8">
        /// The <see cref="Action{T8}"/> to invoke when the value is of the eighth type.
        /// </param>
        /// <param name="otherwise">
        /// The <see cref="Action"/> to invoke if no action is specified for the type of the value.
        /// If <paramref name="whenOption1"/>, <paramref name="whenOption2"/>, <paramref name="whenOption3"/>, <paramref name="whenOption4"/>, <paramref name="whenOption5"/>, <paramref name="whenOption6"/>, <paramref name="whenOption7"/> and <paramref name="whenOption8"/> are given, this parameter is not used.
        /// </param>
        public abstract void Match(
            [AllowNull] Action<T1> whenOption1 = null,
            [AllowNull] Action<T2> whenOption2 = null,
            [AllowNull] Action<T3> whenOption3 = null,
            [AllowNull] Action<T4> whenOption4 = null,
            [AllowNull] Action<T5> whenOption5 = null,
            [AllowNull] Action<T6> whenOption6 = null,
            [AllowNull] Action<T7> whenOption7 = null,
            [AllowNull] Action<T8> whenOption8 = null,
            [AllowNull] Action otherwise = null);
    }
}
