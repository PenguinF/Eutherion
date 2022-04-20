#region License
/*********************************************************************************
 * Maybe.cs
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

#if !NET472
#nullable enable
#endif

using System.Diagnostics.CodeAnalysis;

namespace System
{
    /// <summary>
    /// Represents an optional value.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the optional value.
    /// </typeparam>
    /// <remarks>
    /// This type is deliberately implemented without equality or hash code implementations,
    /// as these would have to make assumptions on equality of the contained object or value.
    /// It is safe to use equality operators on <see cref="Nothing"/>, but one could use the <see cref="IsNothing"/> property as well.
    /// </remarks>
    public abstract class Maybe<T>
#if !NET472
        where T : notnull
#endif
    {
        private sealed class NothingValue : Maybe<T>
        {
            public override bool IsNothing => true;

            public override TResult Match<TResult>(
                Func<TResult> whenNothing,
                Func<T, TResult> whenJust)
                => whenNothing();

            public override void Match(
                [AllowNull] Action whenNothing,
                [AllowNull] Action<T> whenJust)
                => whenNothing?.Invoke();

            public override Maybe<TResult> Bind<TResult>(Func<T, Maybe<TResult>> whenJust)
                => Maybe<TResult>.Nothing;

            public override string ToString()
                => string.Empty;
        }

        private sealed class JustValue : Maybe<T>
        {
            public readonly T Value;

            public JustValue(T value)
                => Value = value;

            public override bool IsJust(out T value)
            {
                value = Value;
                return true;
            }

            public override TResult Match<TResult>(
                Func<TResult> whenNothing,
                Func<T, TResult> whenJust)
                => whenJust(Value);

            public override void Match(
                [AllowNull] Action whenNothing,
                [AllowNull] Action<T> whenJust)
                => whenJust?.Invoke(Value);

            public override Maybe<TResult> Bind<TResult>(Func<T, Maybe<TResult>> whenJust)
                => whenJust(Value);

            public override string ToString()
                => Value.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Represents the <see cref="Maybe{T}"/> without a value.
        /// </summary>
        public static Maybe<T> Nothing { get; } = new NothingValue();

        /// <summary>
        /// Creates a <see cref="Maybe{T}"/> instance which contains a value.
        /// </summary>
        /// <param name="value">
        /// The value to wrap.
        /// </param>
        /// <returns>
        /// The <see cref="Maybe{T}"/> which contains the value.
        /// </returns>
        public static Maybe<T> Just(T value) => new JustValue(value);

        /// <summary>
        /// Converts a value to a Maybe instance.
        /// </summary>
        public static implicit operator Maybe<T>(T value) => new JustValue(value);

        private Maybe() { }

        /// <summary>
        /// Returns if this <see cref="Maybe{T}"/> is empty, i.e. does not contain a value.
        /// </summary>
        public virtual bool IsNothing => false;

        /// <summary>
        /// Returns if this <see cref="Maybe{T}"/> contains a value, and if it does, returns this value.
        /// </summary>
        /// <param name="value">
        /// The value contained in this <see cref="Maybe{T}"/>, or a default value if empty.
        /// </param>
        /// <returns>
        /// Whether or not this <see cref="Maybe{T}"/> contains a value.
        /// </returns>
        public virtual bool IsJust([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out T value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Invokes a function based on whether this <see cref="Maybe{T}"/> contains a value, and returns its result.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of value to return.
        /// </typeparam>
        /// <param name="whenNothing">
        /// The <see cref="Func{TResult}"/> to invoke when the value is <see cref="Nothing"/>.
        /// </param>
        /// <param name="whenJust">
        /// The <see cref="Func{T, TResult}"/> to invoke when the value is <see cref="Just(T)"/>.
        /// </param>
        /// <returns>
        /// The result of the invoked function.
        /// </returns>
        public abstract TResult Match<TResult>(
            Func<TResult> whenNothing,
            Func<T, TResult> whenJust)
#if !NET472
            where TResult : notnull
#endif
            ;

        /// <summary>
        /// Invokes an action based on whether this <see cref="Maybe{T}"/> contains a value.
        /// </summary>
        /// <param name="whenNothing">
        /// The <see cref="Action"/> to invoke when the value is <see cref="Nothing"/>.
        /// </param>
        /// <param name="whenJust">
        /// The <see cref="Action{T}"/> to invoke when the value is <see cref="Just(T)"/>.
        /// </param>
        public abstract void Match(
            [AllowNull] Action whenNothing = null,
            [AllowNull] Action<T> whenJust = null);

        /// <summary>
        /// If this <see cref="Maybe{T}"/> contains a value, applies a function to it
        /// to return a <see cref="Maybe{T}"/> of another target type.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <param name="whenJust">
        /// The function to apply if this <see cref="Maybe{T}"/> contains a value.
        /// </param>
        /// <returns>
        /// The <see cref="Maybe{T}"/> of the result type.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// <paramref name="whenJust"/> is null.
        /// </exception>
        public abstract Maybe<TResult> Bind<TResult>(Func<T, Maybe<TResult>> whenJust)
#if !NET472
            where TResult : notnull
#endif
            ;

        /// <summary>
        /// If this <see cref="Maybe{T}"/> contains a value, applies a function to it
        /// to return a <see cref="Maybe{T}"/> of another target type.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <param name="whenJust">
        /// The function to apply if this <see cref="Maybe{T}"/> contains a value.
        /// </param>
        /// <returns>
        /// The <see cref="Maybe{T}"/> of the result type.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// <paramref name="whenJust"/> is null.
        /// </exception>
        public Maybe<TResult> Select<TResult>(Func<T, TResult> whenJust)
#if !NET472
            where TResult : notnull
#endif
            => Bind(sourceValue => Maybe<TResult>.Just(whenJust(sourceValue)));

        /// <summary>
        /// If this <see cref="Maybe{T}"/> contains a value, applies two functions to it
        /// to return a flattened <see cref="Maybe{T}"/> of another target type, i.e.
        /// a <see cref="Maybe{T}"/> which does not wrap another <see cref="Maybe{T}"/>.
        /// </summary>
        /// <typeparam name="TIntermediate">
        /// The type of the wrapped intermediate value returned by <paramref name="whenJust"/>.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <param name="whenJust">
        /// The function to apply if this <see cref="Maybe{T}"/> contains a value.
        /// </param>
        /// <param name="resultSelector">
        /// The function to apply to flatten the result.
        /// </param>
        /// <returns>
        /// The flattened <see cref="Maybe{T}"/> of the result type.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// <paramref name="whenJust"/> and/or <paramref name="resultSelector"/> are null.
        /// </exception>
        public Maybe<TResult> SelectMany<TIntermediate, TResult>(
            Func<T, Maybe<TIntermediate>> whenJust,
            Func<T, TIntermediate, TResult> resultSelector)
#if !NET472
            where TIntermediate : notnull
            where TResult : notnull
#endif
            => Bind(sourceValue => whenJust(sourceValue).Bind(intermediateValue => Maybe<TResult>.Just(resultSelector(sourceValue, intermediateValue))));
    }
}
