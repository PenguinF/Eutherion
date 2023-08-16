#region License
/*********************************************************************************
 * IFuncExtensions.cs
 *
 * Copyright (c) 2004-2023 Henk Nicolai
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

using Eutherion.Threading;
using System;
using System.Threading.Tasks;

namespace Eutherion
{
    /// <summary>
    /// Defines extension methods to convert a <see cref="Func{TResult}"/> to a <see cref="IFunc{TResult}"/> and vice versa.
    /// </summary>
    // It's actually a nice way of listing the several ways in which a value can be evaluated:
    // Func<T>             - A value that is evaluated on every call.
    // Task<T>             - A value that is evaluated asynchronously.
    // Lazy<T>/SafeLazy<T> - A value that is evaluated on the first call, and is then read-only and stored in memory.
    // SafeLazyObject<T>   - An object that is initialized on the first call, and is then read-only and stored in memory.
    // Box<T>              - A value in memory (that can change).
    // ObservableValue<T>  - A value in memory that can change and has events to observe those changes.
    public static class IFuncExtensions
    {
        private struct EncapsulatedFunc<TResult> : IFunc<TResult>
        {
            public readonly Func<TResult> Fn;

            public EncapsulatedFunc(Func<TResult> fn) => Fn = fn;

            public TResult Eval() => Fn();
        }

        private struct EncapsulatedTask<TResult> : IFunc<TResult>
        {
            public readonly Task<TResult> Task;

            public EncapsulatedTask(Task<TResult> task) => Task = task;

            public TResult Eval() => Task.Result;
        }

        private struct EncapsulatedLazy<TResult> : IFunc<TResult>
        {
            public readonly Lazy<TResult> Lazy;

            public EncapsulatedLazy(Lazy<TResult> lazy) => Lazy = lazy;

            public TResult Eval() => Lazy.Value;
        }

        private struct EncapsulatedBox<TResult> : IFunc<TResult>
        {
            public readonly Box<TResult> Box;

            public EncapsulatedBox(Box<TResult> box) => Box = box;

            public TResult Eval() => Box.Value;
        }

        private struct EncapsulatedSafeLazy<TResult> : IFunc<TResult>
        {
            public readonly SafeLazy<TResult> Lazy;

            public EncapsulatedSafeLazy(SafeLazy<TResult> lazy) => Lazy = lazy;

            public TResult Eval() => Lazy.Value;
        }

        private struct EncapsulatedLazyObject<TResult> : IFunc<TResult>
            where TResult : class
        {
            public readonly SafeLazyObject<TResult> LazyObject;

            public EncapsulatedLazyObject(SafeLazyObject<TResult> lazyObject) => LazyObject = lazyObject;

            public TResult Eval() => LazyObject.Object;
        }

        private struct EncapsulatedObservableValue<TResult> : IFunc<TResult>
        {
            public readonly ObservableValue<TResult> ObservableValue;

            public EncapsulatedObservableValue(ObservableValue<TResult> observableValue) => ObservableValue = observableValue;

            public TResult Eval() => ObservableValue.Value;
        }

        /// <summary>
        /// Converts a <see cref="Func{TResult}"/> to an <see cref="IFunc{TResult}"/>.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the return value of the function.
        /// </typeparam>
        /// <param name="func">
        /// The <see cref="Func{TResult}"/> to convert.
        /// </param>
        /// <returns>
        /// The <see cref="IFunc{TResult}"/> wrapping <paramref name="func"/>.
        /// </returns>
        public static IFunc<TResult> ToIFunc<TResult>(this Func<TResult> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            return new EncapsulatedFunc<TResult>(func);
        }

        /// <summary>
        /// Converts a <see cref="Task{TResult}"/> to an <see cref="IFunc{TResult}"/>.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the return value of the function.
        /// </typeparam>
        /// <param name="task">
        /// The <see cref="Task{TResult}"/> to convert.
        /// </param>
        /// <returns>s
        /// The <see cref="IFunc{TResult}"/> wrapping <paramref name="task"/>.
        /// </returns>
        public static IFunc<TResult> ToIFunc<TResult>(this Task<TResult> task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            return new EncapsulatedTask<TResult>(task);
        }

        /// <summary>
        /// Converts a <see cref="Lazy{TResult}"/> to an <see cref="IFunc{TResult}"/>.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the return value of the function.
        /// </typeparam>
        /// <param name="lazy">
        /// The <see cref="Lazy{TResult}"/> to convert.
        /// </param>
        /// <returns>
        /// The <see cref="IFunc{TResult}"/> wrapping <paramref name="lazy"/>.
        /// </returns>
        public static IFunc<TResult> ToIFunc<TResult>(this Lazy<TResult> lazy)
        {
            if (lazy == null) throw new ArgumentNullException(nameof(lazy));
            return new EncapsulatedLazy<TResult>(lazy);
        }

        /// <summary>
        /// Converts a <see cref="Box{TResult}"/> to an <see cref="IFunc{TResult}"/>.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the return value of the function.
        /// </typeparam>
        /// <param name="box">
        /// The <see cref="Box{TResult}"/> to convert.
        /// </param>
        /// <returns>
        /// The <see cref="IFunc{TResult}"/> wrapping <paramref name="box"/>.
        /// </returns>
        public static IFunc<TResult> ToIFunc<TResult>(this Box<TResult> box)
        {
            if (box == null) throw new ArgumentNullException(nameof(box));
            return new EncapsulatedBox<TResult>(box);
        }

        /// <summary>
        /// Converts a <see cref="SafeLazy{TResult}"/> to an <see cref="IFunc{TResult}"/>.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the return value of the function.
        /// </typeparam>
        /// <param name="lazy">
        /// The <see cref="SafeLazy{TResult}"/> to convert.
        /// </param>
        /// <returns>
        /// The <see cref="IFunc{TResult}"/> wrapping <paramref name="lazy"/>.
        /// </returns>
        public static IFunc<TResult> ToIFunc<TResult>(this SafeLazy<TResult> lazy)
        {
            if (lazy == null) throw new ArgumentNullException(nameof(lazy));
            return new EncapsulatedSafeLazy<TResult>(lazy);
        }

        /// <summary>
        /// Converts a <see cref="SafeLazyObject{TResult}"/> to an <see cref="IFunc{TResult}"/>.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the return value of the function.
        /// </typeparam>
        /// <param name="lazyObject">
        /// The <see cref="SafeLazyObject{TResult}"/> to convert.
        /// </param>
        /// <returns>
        /// The <see cref="IFunc{TResult}"/> wrapping <paramref name="lazyObject"/>.
        /// </returns>
        public static IFunc<TResult> ToIFunc<TResult>(this SafeLazyObject<TResult> lazyObject) where TResult : class
        {
            return new EncapsulatedLazyObject<TResult>(lazyObject);
        }

        /// <summary>
        /// Converts a <see cref="ObservableValue{TResult}"/> to an <see cref="IFunc{TResult}"/>.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the return value of the function.
        /// </typeparam>
        /// <param name="observableValue">
        /// The <see cref="ObservableValue{TResult}"/> to convert.
        /// </param>
        /// <returns>
        /// The <see cref="IFunc{TResult}"/> wrapping <paramref name="observableValue"/>.
        /// </returns>
        public static IFunc<TResult> ToIFunc<TResult>(this ObservableValue<TResult> observableValue)
        {
            if (observableValue == null) throw new ArgumentNullException(nameof(observableValue));
            return new EncapsulatedObservableValue<TResult>(observableValue);
        }

        /// <summary>
        /// Converts a <see cref="IFunc{TResult}"/> to an <see cref="Func{TResult}"/>.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the return value of the function.
        /// </typeparam>
        /// <param name="func">
        /// The <see cref="IFunc{TResult}"/> to convert.
        /// </param>
        /// <returns>
        /// The <see cref="Func{TResult}"/> wrapping <paramref name="func"/>.
        /// </returns>
        public static Func<TResult> ToFunc<TResult>(this IFunc<TResult> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            // Prevent multiple levels of IFuncs wrapped within Funcs and so on.
            return func is EncapsulatedFunc<TResult> encapsulated ? encapsulated.Fn : func.Eval;
        }
    }
}
