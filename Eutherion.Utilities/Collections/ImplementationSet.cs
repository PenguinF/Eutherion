#region License
/*********************************************************************************
 * ImplementationSet.cs
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Eutherion.Collections
{
    /// <summary>
    /// Contains a set of objects which implement a base type such as an interface or an abstract class.
    /// At most one instance of each implementation type is allowed.
    /// Instances of <see cref="ImplementationSet{TInterface}"/> can be declared with a collection initializer.
    /// </summary>
    /// <typeparam name="TInterface">
    /// The interface type implemented by each object in the set.
    /// </typeparam>
    public class ImplementationSet<TInterface> : IReadOnlyCollection<TInterface>
#if !NET472
        where TInterface : notnull
#endif
    {
        private static readonly Type InterfaceType = typeof(TInterface);

        private static IEnumerable<Type> AssignableTypes(Type actualType)
        {
#if NET472
            Type baseType = actualType;
#else
            Type? baseType = actualType;
#endif

            while (baseType != null && InterfaceType.IsAssignableFrom(baseType) && baseType != InterfaceType)
            {
                yield return baseType;

                foreach (var interfaceType in baseType.GetInterfaces().SelectMany(AssignableTypes))
                {
                    yield return interfaceType;
                }

                baseType = baseType.BaseType;
            }
        }

#if NET5_0_OR_GREATER
        private readonly Dictionary<Type, TInterface> Implementations = new();
        private readonly List<TInterface> DistinctImplementations = new();
#else
        private readonly Dictionary<Type, TInterface> Implementations = new Dictionary<Type, TInterface>();
        private readonly List<TInterface> DistinctImplementations = new List<TInterface>();
#endif

        /// <summary>
        /// Gets the number of implementations in the set.
        /// </summary>
        public int Count => DistinctImplementations.Count;

        /// <summary>
        /// Initializes a new empty instance of <see cref="ImplementationSet{TInterface}"/>.
        /// </summary>
        public ImplementationSet()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ImplementationSet{TInterface}"/> containing a set of implementations.
        /// </summary>
        /// <param name="implementations">
        /// The implementations to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="implementations"/> is null, -or- at least one of the elements of the enumeration is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Two or more of the elements of <paramref name="implementations"/> have the same type.
        /// </exception>
        public ImplementationSet(params TInterface[] implementations) => AddRange(implementations);

        /// <summary>
        /// Initializes a new instance of <see cref="ImplementationSet{TInterface}"/> containing a set of implementations.
        /// </summary>
        /// <param name="implementations">
        /// The implementations to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="implementations"/> is null, -or- at least one of the elements of the enumeration is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Two or more of the elements of <paramref name="implementations"/> have the same type.
        /// </exception>
        public ImplementationSet(IEnumerable<TInterface> implementations) => AddRange(implementations);

        /// <summary>
        /// Gets a <typeparamref name="TInterface"/> implementation of a specific subtype.
        /// </summary>
        /// <typeparam name="TImplementation">
        /// The type of <typeparamref name="TInterface"/> to get.
        /// </typeparam>
        /// <param name="implementation">
        /// The instance of <typeparamref name="TImplementation"/> if found, otherwise a default value.
        /// </param>
        /// <returns>
        /// True if an instance of <typeparamref name="TImplementation"/> could be found, otherwise false.
        /// </returns>
        public bool TryGet<TImplementation>([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out TImplementation implementation)
#if NET472
            where TImplementation : TInterface
#else
            where TImplementation : notnull, TInterface
#endif
        {
            if (Implementations.TryGetValue(typeof(TImplementation), out var untypedImplementation))
            {
                implementation = (TImplementation)untypedImplementation;
                return true;
            }
            else
            {
                implementation = default;
                return false;
            }
        }

        /// <summary>
        /// Gets a <typeparamref name="TInterface"/> implementation of a specific subtype.
        /// </summary>
        /// <typeparam name="TImplementation">
        /// The type of <typeparamref name="TInterface"/> to get.
        /// </typeparam>
        /// <returns>
        /// The instance of <typeparamref name="TImplementation"/> if found, otherwise a default value.
        /// </returns>
#if !NET472
        [return: MaybeNull]
#endif
        public TImplementation Get<TImplementation>()
#if NET472
            where TImplementation : TInterface
#else
            where TImplementation : notnull, TInterface
#endif
        {
            TryGet<TImplementation>(out var implementation);
            return implementation;
        }

        /// <summary>
        /// Adds a <typeparamref name="TInterface"/> implementation to this set.
        /// At most one instance of each implementation type is allowed.
        /// </summary>
        /// <param name="implementation">
        /// The implementation to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="implementation"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An instance of the same type as <paramref name="implementation"/> already exists in the set
        /// -or- <paramref name="implementation"/> is an instance of <typeparamref name="TInterface"/>.
        /// </exception>
        public void Add(TInterface implementation)
        {
            if (implementation == null) throw new ArgumentNullException(nameof(implementation));

            var actualType = implementation.GetType();
            if (InterfaceType == actualType)
            {
                throw new ArgumentException($"Attempt to add an instance of {InterfaceType.FullName}", nameof(implementation));
            }

            // The Distinct is necessary for when a type and its base type have the same interface declaration.
            // For example: List<T> -> IList<T> -> ... -> IEnumerable<T> -> IEnumerable
            //                      -> IList -> ... -> ICollection -> IEnumerable
            Type[] distinctAssignableTypes = AssignableTypes(actualType).Distinct().ToArray();
            if (distinctAssignableTypes.Any(x => Implementations.ContainsKey(x), out var duplicateType))
            {
                // This throws.
                Implementations.Add(duplicateType, implementation);
            }
            else
            {
                // Only register all base types if the entire operation will succeed.
                distinctAssignableTypes.ForEach(x => Implementations.Add(x, implementation));
                DistinctImplementations.Add(implementation);
            }
        }

        /// <summary>
        /// Adds a collection of <typeparamref name="TInterface"/> implementations to this set.
        /// At most one instance of each implementation type is allowed.
        /// </summary>
        /// <param name="implementations">
        /// The implementations to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="implementations"/> is null, -or- at least one of the elements of the enumeration is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// For at least one of the elements of <paramref name="implementations"/>, an instance of the same type already exists in the set.
        /// </exception>
        public void AddRange(IEnumerable<TInterface> implementations)
        {
            if (implementations == null) throw new ArgumentNullException(nameof(implementations));
            implementations.ForEach(Add);
        }

        /// <summary>
        /// Gets an enumerator that iterates through the <typeparamref name="TInterface"/> implementations of this set.
        /// </summary>
        /// <returns>
        /// The enumerator that iterates through the <typeparamref name="TInterface"/> implementations of this set.
        /// </returns>
        public IEnumerator<TInterface> GetEnumerator() => DistinctImplementations.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
