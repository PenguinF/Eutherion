#region License
/*********************************************************************************
 * ImplementationSetTests.cs
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

using Eutherion.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Eutherion.Tests
{
    public static class ImplementationSetExtensions
    {
        public static void AssertNotRegistered<DeclaringType, SubType>(this ImplementationSet<DeclaringType> set)
            where DeclaringType : notnull
            where SubType : DeclaringType
        {
            Assert.False(set.TryGet<SubType>(out var _));
        }

        public static void AssertNotRegistered<DeclaringType, SubType1, SubType2>(this ImplementationSet<DeclaringType> set)
            where DeclaringType : notnull
            where SubType1 : DeclaringType
            where SubType2 : DeclaringType
        {
            AssertNotRegistered<DeclaringType, SubType1>(set);
            AssertNotRegistered<DeclaringType, SubType2>(set);
        }

        public static void AssertElements<DeclaringType>(this ImplementationSet<DeclaringType> set, params DeclaringType[] expectedElements)
            where DeclaringType : notnull
        {
            AssertNotRegistered<DeclaringType, DeclaringType>(set);
            Assert.Equal(expectedElements.Length, set.Count);
            Assert.Collection(set, expectedElements.Select(
                expectedElement => new Action<DeclaringType>(actualElement => Assert.Equal(expectedElement, actualElement))).ToArray());
        }

        public static void AssertRegistered<DeclaringType, SubType>(this ImplementationSet<DeclaringType> set, SubType expectedElement)
            where DeclaringType : notnull
            where SubType : DeclaringType
        {
            Assert.True(set.TryGet<SubType>(out var actualElement));
            Assert.Equal(expectedElement, actualElement);
        }

        public static void AssertRegistered<DeclaringType, IntermediateType, SubType>(this ImplementationSet<DeclaringType> set, SubType expectedElement)
            where DeclaringType : notnull
            where IntermediateType : DeclaringType
            where SubType : IntermediateType
        {
            AssertRegistered(set, expectedElement);
            Assert.True(set.TryGet<IntermediateType>(out var actualElement));
            Assert.Equal(expectedElement, actualElement);
        }

        public static void AssertRegistered<DeclaringType, IntermediateType1, IntermediateType2, SubType>(this ImplementationSet<DeclaringType> set, SubType expectedElement)
            where DeclaringType : notnull
            where IntermediateType1 : DeclaringType
            where IntermediateType2 : DeclaringType
            where SubType : IntermediateType1, IntermediateType2
        {
            AssertRegistered<DeclaringType, IntermediateType1, SubType>(set, expectedElement);
            AssertRegistered<DeclaringType, IntermediateType2>(set, expectedElement);
        }

        public static void AssertRegistered<DeclaringType, IntermediateType1, IntermediateType2, IntermediateType3, SubType>(this ImplementationSet<DeclaringType> set, SubType expectedElement)
            where DeclaringType : notnull
            where IntermediateType1 : DeclaringType
            where IntermediateType2 : DeclaringType
            where IntermediateType3 : DeclaringType
            where SubType : IntermediateType1, IntermediateType2, IntermediateType3
        {
            AssertRegistered<DeclaringType, IntermediateType1, IntermediateType2, SubType>(set, expectedElement);
            AssertRegistered<DeclaringType, IntermediateType3>(set, expectedElement);
        }

        public static void AssertRegistered<DeclaringType, IntermediateType1, IntermediateType2, IntermediateType3, IntermediateType4, SubType>(this ImplementationSet<DeclaringType> set, SubType expectedElement)
            where DeclaringType : notnull
            where IntermediateType1 : DeclaringType
            where IntermediateType2 : DeclaringType
            where IntermediateType3 : DeclaringType
            where IntermediateType4 : DeclaringType
            where SubType : IntermediateType1, IntermediateType2, IntermediateType3, IntermediateType4
        {
            AssertRegistered<DeclaringType, IntermediateType1, IntermediateType2, IntermediateType3, SubType>(set, expectedElement);
            AssertRegistered<DeclaringType, IntermediateType4>(set, expectedElement);
        }

        public static void AssertRegistered<DeclaringType, IntermediateType1, IntermediateType2, IntermediateType3, IntermediateType4, IntermediateType5, SubType>(this ImplementationSet<DeclaringType> set, SubType expectedElement)
            where DeclaringType : notnull
            where IntermediateType1 : DeclaringType
            where IntermediateType2 : DeclaringType
            where IntermediateType3 : DeclaringType
            where IntermediateType4 : DeclaringType
            where IntermediateType5 : DeclaringType
            where SubType : IntermediateType1, IntermediateType2, IntermediateType3, IntermediateType4, IntermediateType5
        {
            AssertRegistered<DeclaringType, IntermediateType1, IntermediateType2, IntermediateType3, IntermediateType4, SubType>(set, expectedElement);
            AssertRegistered<DeclaringType, IntermediateType5>(set, expectedElement);
        }
    }

    public class ImplementationSetTests
    {
        private class TestClass_A { }
        private class TestClass_Aa : TestClass_A { }
        private class TestClass_Aa1 : TestClass_Aa { }
        private class TestClass_Ab : TestClass_A { }

        [Fact]
        public void NullArgumentThrows()
        {
            ImplementationSet<TestClass_A> set = new();
            Assert.Throws<ArgumentNullException>(() => set.Add(null!));
            Assert.Throws<ArgumentNullException>(() => set.AddRange(null!));
            Assert.Throws<ArgumentNullException>(() => new ImplementationSet<TestClass_A>(null!));
            Assert.Throws<ArgumentNullException>(() => new ImplementationSet<TestClass_A>((IEnumerable<TestClass_A>)null!));
        }

        [Fact]
        public void CannotAddInstanceOfDeclaredTypeParameter()
        {
            ImplementationSet<TestClass_A> set = new();
            Assert.Throws<ArgumentException>(() => set.Add(new TestClass_A()));
            set.AssertElements();
        }

        [Fact]
        public void CannotAddInstanceTwice()
        {
            TestClass_Aa a = new();
            ImplementationSet<TestClass_A> set = new() { a };
            Assert.Throws<ArgumentException>(() => set.Add(a));
            Assert.Throws<ArgumentException>(() => set.Add(new TestClass_Aa()));
            set.AssertElements(a);
            set.AssertRegistered(a);
        }

        [Fact]
        public void CannotAddInstancesOfBothBaseTypeAndSubType()
        {
            // Should throw in both cases.
            TestClass_Aa aa = new();
            TestClass_Aa1 aa1 = new();

            ImplementationSet<TestClass_A> set1 = new() { aa };
            Assert.Throws<ArgumentException>(() => set1.Add(aa1));
            set1.AssertElements(aa);
            set1.AssertRegistered(aa);

            ImplementationSet<TestClass_A> set2 = new() { aa1 };
            Assert.Throws<ArgumentException>(() => set2.Add(aa));
            set2.AssertElements(aa1);
            set2.AssertRegistered(aa1);
        }

        [Fact]
        public void Enumerate()
        {
            TestClass_Aa1 a1 = new();
            TestClass_Ab a2 = new();
            ImplementationSet<TestClass_A> set = new() { a1, a2 };

            TestClass_A[] expectedList = new TestClass_A[] { a1, a2 };

            int i = 0;
            foreach (TestClass_A value in set)
            {
                Assert.Same(expectedList[i], value);
                i++;
            }

            Assert.Equal(expectedList.Length, set.Count);
            Assert.Equal(expectedList.Length, i);
        }

        private interface ITestInterface_B { }
        private abstract class TestClass_B { }
        private class TestClass_Ba : TestClass_B, ITestInterface_B { }

        [Fact]
        public void UnrelatedTypesAreNotRegistered()
        {
            // This is to test that if a subtype has base types that are unrelated,
            // there is no observable difference in behavior compared to if the other base type hadn't been there.
            TestClass_Ba ba = new();

            ImplementationSet<ITestInterface_B> set1 = new() { ba };
            set1.AssertElements(ba);
            set1.AssertRegistered(ba);

            ImplementationSet<TestClass_B> set2 = new() { ba };
            set2.AssertElements(ba);
            set2.AssertRegistered(ba);
        }

        private interface ITestInterface_C { }
        private interface ITestInterface_Ca : ITestInterface_C { }
        private class TestClass_Ca : ITestInterface_Ca { }
        private interface ITestInterface_Cb : ITestInterface_C { }
        private class TestClass_Cb : ITestInterface_Cb { }
        private interface ITestInterface_Cc : ITestInterface_C { }
        private class TestClass_Cbc : ITestInterface_Cb, ITestInterface_Cc { }

        [Fact]
        public void DiamondInheritanceHierarchy()
        {
            // Tests that TestClass_Ca can be combined with either TestClass_Cb or TestClass_Cbc,
            // but TestClass_Cb and TestClass_Cbc cannot be combined as they have a common base type ITestInterface_Cb.
            TestClass_Ca ca = new();
            TestClass_Cb cb = new();
            TestClass_Cbc cbc = new();

            // Also test different ways in which to initialize a set here.
            ImplementationSet<ITestInterface_C> set1a = new() { ca, cb };
            set1a.AssertElements(ca, cb);
            set1a.AssertRegistered<ITestInterface_C, ITestInterface_Ca, TestClass_Ca>(ca);
            set1a.AssertRegistered<ITestInterface_C, ITestInterface_Cb, TestClass_Cb>(cb);

            ImplementationSet<ITestInterface_C> set1b = new() { cb, ca };
            set1b.AssertElements(cb, ca);
            set1b.AssertRegistered<ITestInterface_C, ITestInterface_Ca, TestClass_Ca>(ca);
            set1b.AssertRegistered<ITestInterface_C, ITestInterface_Cb, TestClass_Cb>(cb);

            ImplementationSet<ITestInterface_C> set2a = new(ca, cbc);
            set2a.AssertElements(ca, cbc);
            set2a.AssertRegistered<ITestInterface_C, ITestInterface_Ca, TestClass_Ca>(ca);
            set2a.AssertRegistered<ITestInterface_C, ITestInterface_Cb, ITestInterface_Cc, TestClass_Cbc>(cbc);

            ImplementationSet<ITestInterface_C> set2b = new(cbc, ca);
            set2b.AssertElements(cbc, ca);
            set2b.AssertRegistered<ITestInterface_C, ITestInterface_Cb, ITestInterface_Cc, TestClass_Cbc>(cbc);
            set2b.AssertRegistered<ITestInterface_C, ITestInterface_Ca, TestClass_Ca>(ca);

            ImplementationSet<ITestInterface_C> set3a = new(new TestClass_Cb[] { cb });
            Assert.Throws<ArgumentException>(() => set3a.Add(cbc));
            set3a.AssertElements(cb);
            set3a.AssertRegistered<ITestInterface_C, ITestInterface_Cb, TestClass_Cb>(cb);
            set3a.AssertNotRegistered<ITestInterface_C, ITestInterface_Cc>();

            ImplementationSet<ITestInterface_C> set3b = new(new TestClass_Cbc[] { cbc });
            Assert.Throws<ArgumentException>(() => set3b.Add(cb));
            set3b.AssertElements(cbc);
            set3b.AssertRegistered<ITestInterface_C, ITestInterface_Cb, ITestInterface_Cc, TestClass_Cbc>(cbc);
        }

        private interface ITestInterface_D { }
        private interface ITestInterface_D<T> : ITestInterface_D { }
        private interface ITestInterface_Da : ITestInterface_D { }
        private interface ITestInterface_Da<T> : ITestInterface_D<T>, ITestInterface_Da { }
        private class TestClass_Da<T> : ITestInterface_Da<T>, ITestInterface_Da, ITestInterface_D<T>, ITestInterface_D { }

        [Fact]
        public void DuplicateInterfaceDeclarationsAreAllowed()
        {
            TestClass_Da<int> ca1 = new();
            TestClass_Da<string> ca2 = new();

            // Should throw because ca1 and ca2 share ITestInterface_Da base type.
            ImplementationSet<ITestInterface_D> set1a = new(ca1);
            Assert.Throws<ArgumentException>(() => set1a.Add(ca2));
            set1a.AssertElements(ca1);
            set1a.AssertRegistered<ITestInterface_D, ITestInterface_D<int>, ITestInterface_Da, ITestInterface_Da<int>, TestClass_Da<int>>(ca1);
            set1a.AssertNotRegistered<ITestInterface_D, ITestInterface_D<string>, ITestInterface_Da<string>>();

            ImplementationSet<ITestInterface_D> set1b = new(ca2);
            Assert.Throws<ArgumentException>(() => set1b.Add(ca1));
            set1b.AssertElements(ca2);
            set1b.AssertRegistered<ITestInterface_D, ITestInterface_D<string>, ITestInterface_Da, ITestInterface_Da<string>, TestClass_Da<string>>(ca2);
            set1b.AssertNotRegistered<ITestInterface_D, ITestInterface_D<int>, ITestInterface_Da<int>>();

            // This is OK because ITestInterface_Da is the only shared base type.
            ImplementationSet<ITestInterface_Da> set2 = new(ca1, ca2);
            set2.AssertElements(ca1, ca2);
            set2.AssertRegistered<ITestInterface_Da, ITestInterface_Da<int>, TestClass_Da<int>>(ca1);
            set2.AssertRegistered<ITestInterface_Da, ITestInterface_Da<string>, TestClass_Da<string>>(ca2);
        }

        [Fact]
        public void ArrayBehavior()
        {
            // Arrays behave a bit weird in the .NET type system, so assert those in a special case to at least make this explicit.
            string[] stringArray = new string[] { "x" };
            int[][] intArrayArray = new int[][] { new int[] { 1 } };
            TestClass_A[] classAArray = new TestClass_A[] { new() };
            TestClass_Aa[] classAaArray = new TestClass_Aa[] { new() };

            ImplementationSet<object[]> set1 = new();
            set1.Add(stringArray);
            set1.Add(intArrayArray);
            set1.Add(classAArray);
            set1.Add(classAaArray);
            set1.AssertElements(stringArray, intArrayArray, classAArray, classAaArray);
            set1.AssertRegistered(stringArray);
            set1.AssertRegistered(intArrayArray);
            set1.AssertRegistered(classAArray);
            set1.AssertRegistered(classAaArray);

            ImplementationSet<IEnumerable<object>> set2 = new();
            set2.Add(stringArray);
            set2.Add(intArrayArray);
            set2.Add(classAArray);
            set2.Add(classAaArray);
            set2.AssertElements(stringArray, intArrayArray, classAArray, classAaArray);
            set2.AssertRegistered<IEnumerable<object>, IReadOnlyCollection<string>, IReadOnlyList<string>, IEnumerable<string>, ICollection<string>, IList<string>, string[]>(stringArray);
            set2.AssertRegistered<IEnumerable<object>, IReadOnlyCollection<int[]>, IReadOnlyList<int[]>, IEnumerable<int[]>, ICollection<int[]>, IList<int[]>, int[][]>(intArrayArray);
            set2.AssertRegistered<IEnumerable<object>, IReadOnlyCollection<TestClass_A>, IReadOnlyList<TestClass_A>, IEnumerable<TestClass_A>, ICollection<TestClass_A>, IList<TestClass_A>, TestClass_A[]>(classAArray);
            set2.AssertRegistered<IEnumerable<object>, IReadOnlyCollection<TestClass_Aa>, IReadOnlyList<TestClass_Aa>, IEnumerable<TestClass_Aa>, ICollection<TestClass_Aa>, IList<TestClass_Aa>, TestClass_Aa[]>(classAaArray);
        }

        private interface ITestInterface_In<in T> { }
        private class TestClass_E1_in<T> : ITestInterface_In<T> { }
        private class TestClass_E2_in<T> : ITestInterface_In<T> { }

        [Fact]
        public void Covariance()
        {
            TestClass_E1_in<TestClass_A> e1_in_a = new(); TestClass_E1_in<TestClass_Aa> e1_in_aa = new();
            TestClass_E2_in<TestClass_A> e2_in_a = new(); TestClass_E2_in<TestClass_Aa> e2_in_aa = new();

            List<string> listString = new();
            List<TestClass_Aa> listAa = new();
            List<TestClass_Ab> listAb = new();
            ReadOnlyList<string> readOnlyListString = ReadOnlyList<string>.Empty;
            ReadOnlyList<TestClass_Aa> readOnlyListAa = ReadOnlyList<TestClass_Aa>.Empty;
            ReadOnlyList<TestClass_Ab> readOnlyListAb = ReadOnlyList<TestClass_Ab>.Empty;
            string[] arrString = new string[1];
            TestClass_Aa[] arrAa = new TestClass_Aa[1];
            TestClass_Ab[] arrAb = new TestClass_Ab[1];

            // Covariance: different implementations are allowed; type parameters must be different and can even share base types.
            // This makes sense because you ask the ImplementationSet if it contains an IEnumerable<> of a specific target type,
            // regardless of what implementation was chosen to do this enumeration.
            // You could make an argument for restricting base types as well, but that seems beyond the scope of what ImplementationSet may be used for.
            {
                ImplementationSet<IEnumerable<object>> set_covariant = new(listString, readOnlyListAa, arrAb);
                Assert.Throws<ArgumentException>(() => set_covariant.Add(listAa));
                Assert.Throws<ArgumentException>(() => set_covariant.Add(listAb));
                Assert.Throws<ArgumentException>(() => set_covariant.Add(readOnlyListString));
                Assert.Throws<ArgumentException>(() => set_covariant.Add(readOnlyListAb));
                Assert.Throws<ArgumentException>(() => set_covariant.Add(arrString));
                Assert.Throws<ArgumentException>(() => set_covariant.Add(arrAa));

                set_covariant.AssertElements(listString, readOnlyListAa, arrAb);
                set_covariant.AssertRegistered<IEnumerable<object>, IReadOnlyCollection<string>, IReadOnlyList<string>, IEnumerable<string>, ICollection<string>, IList<string>, List<string>>(listString);
                set_covariant.AssertRegistered<IEnumerable<object>, IReadOnlyCollection<TestClass_Aa>, IReadOnlyList<TestClass_Aa>, IEnumerable<TestClass_Aa>, ReadOnlyList<TestClass_Aa>>(readOnlyListAa);
                set_covariant.AssertRegistered<IEnumerable<object>, IReadOnlyCollection<TestClass_Ab>, IReadOnlyList<TestClass_Ab>, IEnumerable<TestClass_Ab>, ICollection<TestClass_Ab>, IList<TestClass_Ab>, TestClass_Ab[]>(arrAb);
            }

            // Also try using the same implementing type.
            {
                ImplementationSet<IEnumerable<object>> set_covariant = new(readOnlyListString, readOnlyListAa, readOnlyListAb);
                Assert.Throws<ArgumentException>(() => set_covariant.Add(listString));
                Assert.Throws<ArgumentException>(() => set_covariant.Add(listAa));
                Assert.Throws<ArgumentException>(() => set_covariant.Add(listAb));
                Assert.Throws<ArgumentException>(() => set_covariant.Add(arrString));
                Assert.Throws<ArgumentException>(() => set_covariant.Add(arrAa));
                Assert.Throws<ArgumentException>(() => set_covariant.Add(arrAb));

                set_covariant.AssertElements(readOnlyListString, readOnlyListAa, readOnlyListAb);
                set_covariant.AssertRegistered<IEnumerable<object>, IReadOnlyCollection<string>, IReadOnlyList<string>, IEnumerable<string>, ReadOnlyList<string>>(readOnlyListString);
                set_covariant.AssertRegistered<IEnumerable<object>, IReadOnlyCollection<TestClass_Aa>, IReadOnlyList<TestClass_Aa>, IEnumerable<TestClass_Aa>, ReadOnlyList<TestClass_Aa>>(readOnlyListAa);
                set_covariant.AssertRegistered<IEnumerable<object>, IReadOnlyCollection<TestClass_Ab>, IReadOnlyList<TestClass_Ab>, IEnumerable<TestClass_Ab>, ReadOnlyList<TestClass_Ab>>(readOnlyListAb);
            }

            // More specific base types.
            {
                ImplementationSet<IReadOnlyCollection<TestClass_A>> set_covariant = new(listAa, readOnlyListAb);
                Assert.Throws<ArgumentException>(() => set_covariant.Add(listAb));
                Assert.Throws<ArgumentException>(() => set_covariant.Add(readOnlyListAa));
                Assert.Throws<ArgumentException>(() => set_covariant.Add(arrAa));
                Assert.Throws<ArgumentException>(() => set_covariant.Add(arrAb));

                set_covariant.AssertElements(listAa, readOnlyListAb);
                set_covariant.AssertRegistered<IReadOnlyCollection<TestClass_A>, IReadOnlyCollection<TestClass_Aa>, IReadOnlyList<TestClass_Aa>, List<TestClass_Aa>>(listAa);
                set_covariant.AssertRegistered<IReadOnlyCollection<TestClass_A>, IReadOnlyCollection<TestClass_Ab>, IReadOnlyList<TestClass_Ab>, ReadOnlyList<TestClass_Ab>>(readOnlyListAb);
            }
        }
    }
}
