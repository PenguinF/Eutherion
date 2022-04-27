#region License
/*********************************************************************************
 * UIntExtensions.cs
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

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Numerics
{
    /// <summary>
    /// Contains extension methods to manipulate vectors of bits.
    /// See also <seealso cref="BitOperations"/>.
    /// </summary>
    public static class UIntExtensions
    {
        /// <summary>
        /// Tests if a vector has any set bits.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Test(this uint vector) => vector != 0;

        /// <summary>
        /// Tests if a vector has any set bits.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Test(this ulong vector) => vector != 0;

        /// <summary>
        /// Tests if another vector has any bits in common with this one.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Test(this uint vector, uint otherVector) => (vector & otherVector) != 0;

        /// <summary>
        /// Tests if another vector has any bits in common with this one.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Test(this ulong vector, ulong otherVector) => (vector & otherVector) != 0;

        /// <summary>
        /// Tests if a vector is equal to zero or a power of two, i.e. has a maximum of one set bit.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMaxOneBit(this uint vector) => !vector.Test(vector - 1);

        /// <summary>
        /// Tests if a vector is equal to zero or a power of two, i.e. has a maximum of one set bit.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMaxOneBit(this ulong vector) => !vector.Test(vector - 1);

        /// <summary>
        /// Enumerates all bits in a given vector, i.e. for each set bit a vector with the same set bit and all other bits zeroed out.
        /// </summary>
        public static IEnumerable<uint> SetBits(this uint vector)
        {
            while (vector != 0)
            {
                // Select the least significant 1-bit using a well known trick.
                uint oneBit = vector & (0U - vector);
                yield return oneBit;

                // Zero the least significant 1-bit so the index of the next 1-bit can be yielded.
                vector ^= oneBit;
            }
        }

        /// <summary>
        /// Enumerates all bits in a given vector, i.e. for each set bit a vector with the same set bit and all other bits zeroed out.
        /// </summary>
        public static IEnumerable<ulong> SetBits(this ulong vector)
        {
            while (vector != 0)
            {
                // Select the least significant 1-bit using a well known trick.
                ulong oneBit = vector & (0U - vector);
                yield return oneBit;

                // Zero the least significant 1-bit so the index of the next 1-bit can be yielded.
                vector ^= oneBit;
            }
        }
    }
}
