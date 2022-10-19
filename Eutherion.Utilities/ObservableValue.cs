#region License
/*********************************************************************************
 * ObservableValue.cs
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

using System;
using System.Collections.Generic;

namespace Eutherion
{
    /// <summary>
    /// Contains a value and adorns it with events to observe updates to it.
    /// </summary>
    /// <typeparam name="TValue">
    /// The type of value to observe.
    /// </typeparam>
    public class ObservableValue<TValue>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ObservableValue{TValue}"/> with a default equality comparer.
        /// </summary>
        /// <param name="initialValue">
        /// The initial value.
        /// </param>
        /// <returns>
        /// The new <see cref="ObservableValue{TValue}"/> instance.
        /// </returns>
        public static ObservableValue<TValue> Create(TValue initialValue)
            => Create(initialValue, EqualityComparer<TValue>.Default);

        /// <summary>
        /// Initializes a new instance of <see cref="ObservableValue{TValue}"/>.
        /// </summary>
        /// <param name="initialValue">
        /// The initial value.
        /// </param>
        /// <param name="equalityComparer">
        /// The equality comparer to determine if two values are equal.
        /// </param>
        /// <returns>
        /// The new <see cref="ObservableValue{TValue}"/> instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="equalityComparer"/> is null.
        /// </exception>
        public static ObservableValue<TValue> Create(TValue initialValue, IEqualityComparer<TValue> equalityComparer)
        {
            if (equalityComparer == null) throw new ArgumentNullException(nameof(equalityComparer));
            return new ObservableValue<TValue>(initialValue, equalityComparer.Equals);
        }

        private readonly Func<TValue, TValue, bool> equal;

        private TValue value;

        /// <summary>
        /// Initializes a new instance of <see cref="ObservableValue{TValue}"/>.
        /// </summary>
        /// <param name="initialValue">
        /// The initial value.
        /// </param>
        /// <param name="equalityComparer">
        /// The equality comparer to determine if two values are equal.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="equalityComparer"/> is null.
        /// </exception>
        public ObservableValue(TValue initialValue, Func<TValue, TValue, bool> equalityComparer)
        {
            value = initialValue;
            equal = equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer));
        }

        /// <summary>
        /// Gets or sets the observed value.
        /// </summary>
        public TValue Value
        {
            get => value;
            set
            {
                if (!equal(this.value, value))
                {
                    OnValueChanging(this.value);
                    this.value = value;
                    OnValueChanged(value);
                }
            }
        }

        /// <summary>
        /// Occurs before a change to <see cref="Value"/>.
        /// The old value before the change is passed as a parameter.
        /// </summary>
#if NET472
        public event Action<TValue> ValueChanging;

        private event Action<TValue> ValueChangedEvent;
#else
        public event Action<TValue>? ValueChanging;

        private event Action<TValue>? ValueChangedEvent;
#endif

        /// <summary>
        /// Occurs after a change to <see cref="Value"/>.
        /// The new value after the change is passed as a parameter.
        /// When a handler is added for this event, it is called immediately.
        /// </summary>
#if NET472
        public event Action<TValue> ValueChanged
#else
        public event Action<TValue>? ValueChanged
#endif
        {
            add
            {
                ValueChangedEvent += value;
                value?.Invoke(Value);
            }
            remove
            {
                ValueChangedEvent -= value;
            }
        }

        /// <summary>
        /// Raises the <see cref="ValueChanging"/> event.
        /// </summary>
        protected virtual void OnValueChanging(TValue oldValue) => ValueChanging?.Invoke(oldValue);

        /// <summary>
        /// Raises the <see cref="ValueChanged"/> event.
        /// </summary>
        protected virtual void OnValueChanged(TValue newValue) => ValueChangedEvent?.Invoke(newValue);
    }
}
