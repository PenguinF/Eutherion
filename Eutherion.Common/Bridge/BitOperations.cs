#region License
/*********************************************************************************
 * BitUtilities.cs
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

namespace System.Numerics
{
#if NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
#else
    /// <summary>
    /// Provides a bridge to the .NET Core and .NET 5 BitOperations class, which provides utility methods for intrinsic bit-twiddling operations.
    /// </summary>
    public static class BitOperations
    {
        /// <summary>
        /// Gets the index of the single bit of the vector, or an undefined value if the number of set bits in the vector is not equal to one.
        /// </summary>
        public static int Log2(this uint oneBitVector)
        {
            // TODO: should probably use DeBruijn instead of doing heavy branching.

            // Constant masks.
            const uint m1 = 0x55555555;  // 010101010101...
            const uint m2 = 0x33333333;  // 001100110011...
            const uint m4 = 0x0f0f0f0f;  // 000011110000...
            const uint m8 = 0x00ff00ff;
            const uint m16 = 0x0000ffff;

            // Calculate the index of the single set bit by testing it against several predefined constants.
            // This index is built as a binary value.
            int index = ((oneBitVector & m16) == 0 ? 16 : 0) |
                        ((oneBitVector & m8) == 0 ? 8 : 0) |
                        ((oneBitVector & m4) == 0 ? 4 : 0) |
                        ((oneBitVector & m2) == 0 ? 2 : 0) |
                        ((oneBitVector & m1) == 0 ? 1 : 0);

            return index;
        }

        /// <summary>
        /// Gets the index of the single bit of the vector, or an undefined value if the number of set bits in the vector is not equal to one.
        /// </summary>
        public static int Log2(this ulong oneBitVector)
        {
            // Constant masks.
            const ulong m1 = 0x5555555555555555;  // 010101010101...
            const ulong m2 = 0x3333333333333333;  // 001100110011...
            const ulong m4 = 0x0f0f0f0f0f0f0f0f;  // 000011110000...
            const ulong m8 = 0x00ff00ff00ff00ff;
            const ulong m16 = 0x0000ffff0000ffff;
            const ulong m32 = 0x00000000ffffffff;

            // Calculate the index of the single set bit by testing it against several predefined constants.
            // This index is built as a binary value.
            int index = ((oneBitVector & m32) == 0 ? 32 : 0) |
                        ((oneBitVector & m16) == 0 ? 16 : 0) |
                        ((oneBitVector & m8) == 0 ? 8 : 0) |
                        ((oneBitVector & m4) == 0 ? 4 : 0) |
                        ((oneBitVector & m2) == 0 ? 2 : 0) |
                        ((oneBitVector & m1) == 0 ? 1 : 0);

            return index;
        }
    }
#endif
}
