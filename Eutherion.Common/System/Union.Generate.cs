#region License
/*********************************************************************************
 * Union.Generate.cs
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

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace System
{
#if DEBUG
    /// <summary>
    /// Allow access to Union in DEBUG mode.
    /// </summary>
    public static class Union
#else
    internal static class Union
#endif
    {
        private static string TypeParameter(int typeIndex)
            => $"T{typeIndex}";

        private static string WhenOptionParameterName(int typeIndex)
            => $"whenOption{typeIndex}";

        private static readonly string OtherwiseParameterName = "otherwise";

        internal static InvalidPatternMatchException InvalidPatternMatchArgumentException(int typeIndex)
#if NET5_0_OR_GREATER
            => new($"{nameof(Union)} value is of type {TypeParameter(typeIndex)}, but both {WhenOptionParameterName(typeIndex)} and {OtherwiseParameterName} parameters are null.");
#else
            => new InvalidPatternMatchException($"{nameof(Union)} value is of type {TypeParameter(typeIndex)}, but both {WhenOptionParameterName(typeIndex)} and {OtherwiseParameterName} parameters are null.");
#endif

        internal static int ShiftHashCode(int hashCode, int positions)
        {
            // Use ValueTuple's algorithm to shift a hash code before it's combined.
            do
            {
                hashCode = ((hashCode << 5) | (int)((uint)hashCode >> 27)) + hashCode;
                positions--;
            }
            while (positions > 0);

            return hashCode;
        }

#if DEBUG
        internal const int MaxOptionsToGenerate = 8;

        private static string Cardinal(int number)
            => number == 1 ? "one"
            : number == 2 ? "two"
            : number == 3 ? "three"
            : number == 4 ? "four"
            : number == 5 ? "five"
            : number == 6 ? "six"
            : number == 7 ? "seven"
            : number == 8 ? "eight"
            : Convert.ToString(number);

        private static string Ordinal(int number)
            => number == 1 ? "first"
            : number == 2 ? "second"
            : number == 3 ? "third"
            : number == 5 ? "fifth"
            : number == 8 ? "eighth"
            : $"{Cardinal(number)}th";

        private static IEnumerable<string> GenerateList(int count, Func<int, string> generator)
            => GenerateList(1, count, generator);

        private static IEnumerable<string> GenerateList(int start, int count, Func<int, string> generator)
            => Enumerable.Range(start, count).Select(generator);

        private static string ConcatList(int count, Func<int, string> generator)
            => string.Concat(GenerateList(count, generator));

        private static string CommaSeparatedList(int count, Func<int, string> generator)
            => string.Join(", ", GenerateList(count, generator));

        private static readonly string ClassName = nameof(Union);

        private static string TypeParameters(int optionCount)
            => $"{CommaSeparatedList(optionCount, TypeParameter)}";

        private static string SubClassName(int typeIndex)
            => $"ValueOfType{typeIndex}";

        private static string EqualityComparerClassName
            => $"EqualityComparer";

        private static string OptionMethodName(int typeIndex)
            => $"Option{typeIndex}";

        private static string IsOptionMethodName(int typeIndex)
            => $"IsOption{typeIndex}";

        private static string ToOptionMethodName(int typeIndex)
            => $"ToOption{typeIndex}";

        private static string WhenOptionParamRef(int typeIndex)
            => $@"<paramref name=""{WhenOptionParameterName(typeIndex)}""/>";

        private static string MatchMethodFuncOverloadParameter(int typeIndex)
            => $"[AllowNull] Func<{TypeParameter(typeIndex)}, TResult> {WhenOptionParameterName(typeIndex)} = null,";

        private static string MatchMethodActionOverloadParameter(int typeIndex)
            => $"[AllowNull] Action<{TypeParameter(typeIndex)}> {WhenOptionParameterName(typeIndex)} = null,";

        private static string EqualityComparerPropertyName(int typeIndex)
            => $"EqualityComparer{typeIndex}";

        private static string EqualityComparerParameterName(int typeIndex)
            => $"equalityComparer{typeIndex}";

        private static string SingleDispatchedEqualsMethodName
            => $"Equals";

        private static string DoubleDispatchedEqualsMethodName(int typeIndex)
            => $"Equals{typeIndex}";

        private static string SingleDispatchedGetHashCodeMethodName
            => $"GetHashCode";

        private static string ParametrizedClassName(int optionCount)
            => $"{ClassName}<{TypeParameters(optionCount)}>";

        private static string ReferToClassName(int optionCount)
            => $"{ClassName}{{{TypeParameters(optionCount)}}}";

        private static string LangwordNull
            => $@"<see langword=""null""/>";

        private static string LangwordFalse
            => $@"<see langword=""false""/>";

        private static string LangwordTrue
            => $@"<see langword=""true""/>";

        private static string See(string reference)
            => $@"<see cref=""{reference}""/>";

        private static string LicenseHeaderAndUsingStatements()
            => $@"#region License
/*********************************************************************************
 * Union.cs
 *
 * Copyright (c) 2004-{DateTime.UtcNow.Year} Henk Nicolai
 *
 *    Licensed under the Apache License, Version 2.0 (the ""License"");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an ""AS IS"" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
**********************************************************************************/
#endregion

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
";

        private static string ClassSummary(int optionCount)
    => $@"
    /// <summary>
    /// Encapsulates a value which can have {Cardinal(optionCount)} different types.
    /// </summary>{ConcatList(optionCount, ClassSummaryTypeParam)}
    /// <remarks>
    /// This type is deliberately implemented without equality or hash code implementations,
    /// as these would have to make assumptions on equality of the contained object or value.
    /// </remarks>";

        private static string ClassSummaryTypeParam(int typeIndex)
            => $@"
    /// <typeparam name=""{TypeParameter(typeIndex)}"">
    /// The {Ordinal(typeIndex)} type of the value.
    /// </typeparam>";

        private static string GenericNotNullConstraintForTypeParam(int typeIndex)
            => $@"
        where {TypeParameter(typeIndex)} : notnull";

        private static string ClassHeader(int optionCount)
            => $@"
    public abstract class {ParametrizedClassName(optionCount)}
" + $@"#if !NET472{ConcatList(optionCount, GenericNotNullConstraintForTypeParam)}
" + $@"#endif
    {{";

        private static Func<int, string> SubClass(int optionCount)
            => typeIndex => $@"
        private sealed class {SubClassName(typeIndex)} : {ParametrizedClassName(optionCount)}
        {{
            public readonly {TypeParameter(typeIndex)} Value;

            public {SubClassName(typeIndex)}({TypeParameter(typeIndex)} value) => Value = value;

            public override bool {IsOptionMethodName(typeIndex)}() => true;

            public override bool {IsOptionMethodName(typeIndex)}(out {TypeParameter(typeIndex)} value)
            {{
                value = Value;
                return true;
            }}

            public override {TypeParameter(typeIndex)} {ToOptionMethodName(typeIndex)}() => Value;

            public override TResult Match<TResult>({ConcatList(optionCount, paramOption => $@"
                {MatchMethodFuncOverloadParameter(paramOption)}")}
                [AllowNull] Func<TResult> {OtherwiseParameterName} = null)
                => {WhenOptionParameterName(typeIndex)} != null ? {WhenOptionParameterName(typeIndex)}(Value)
                : {OtherwiseParameterName} != null ? {OtherwiseParameterName}()
                : throw {ClassName}.InvalidPatternMatchArgumentException({typeIndex});

            public override void Match({ConcatList(optionCount, paramOption => $@"
                {MatchMethodActionOverloadParameter(paramOption)}")}
                [AllowNull] Action {OtherwiseParameterName} = null)
            {{
                if ({WhenOptionParameterName(typeIndex)} != null) {WhenOptionParameterName(typeIndex)}(Value);
                else {OtherwiseParameterName}?.Invoke();
            }}

            private protected override bool {DoubleDispatchedEqualsMethodName(typeIndex)}(EqualityComparer equalityComparer, {TypeParameter(typeIndex)} x) => equalityComparer.{EqualityComparerPropertyName(typeIndex)}.Equals(x, Value);

            private protected override bool {SingleDispatchedEqualsMethodName}(EqualityComparer equalityComparer, {ParametrizedClassName(optionCount)} y) => y.{DoubleDispatchedEqualsMethodName(typeIndex)}(equalityComparer, Value);

            private protected override int {SingleDispatchedGetHashCodeMethodName}(EqualityComparer equalityComparer) => {(typeIndex == 1).Conditional(() => $"equalityComparer.{EqualityComparerPropertyName(typeIndex)}.GetHashCode(Value)", () => $"Union.ShiftHashCode(equalityComparer.{EqualityComparerPropertyName(typeIndex)}.GetHashCode(Value), {typeIndex - 1})")};

            public override string ToString() => Value?.ToString() ?? string.Empty;
        }}
";

        private static string EqualityComparerClass(int optionCount)
            => $@"
        /// <summary>
        /// Defines methods to test {See(ReferToClassName(optionCount))} values for equality and generate their hash codes.
        /// </summary>
        public class {EqualityComparerClassName} : IEqualityComparer<{ParametrizedClassName(optionCount)}>
        {{{ConcatList(optionCount, typeIndex => $@"
            /// <summary>
            /// Gets the equality comparer for values of the {Ordinal(typeIndex)} type.
            /// </summary>
            public IEqualityComparer<{TypeParameter(typeIndex)}> {EqualityComparerPropertyName(typeIndex)} {{ get; }}
")}
            /// <summary>
            /// Creates a new {See(EqualityComparerClassName)} from equality comparers for each of the possible types.
            /// </summary>{ConcatList(optionCount, typeIndex => $@"
            /// <param name=""{EqualityComparerParameterName(typeIndex)}"">
            /// The equality comparer for values of the {Ordinal(typeIndex)} type. If {LangwordNull}, {See("EqualityComparer{T}.Default")} is used.
            /// </param>")}
            public {EqualityComparerClassName}({ConcatList(optionCount, typeIndex => $@"
                [AllowNull] IEqualityComparer<{TypeParameter(typeIndex)}> {EqualityComparerParameterName(typeIndex)} = null{(typeIndex < optionCount).Conditional(() => ",", () => ")")}")}
            {{{ConcatList(optionCount, typeIndex => $@"
                {EqualityComparerPropertyName(typeIndex)} = {EqualityComparerParameterName(typeIndex)} ?? EqualityComparer<{TypeParameter(typeIndex)}>.Default;")}
            }}

            /// <summary>
            /// Tests whether the specified values are equal.
            /// </summary>
            /// <param name=""x"">
            /// The first value to compare.
            /// </param>
            /// <param name=""y"">
            /// The second value to compare.
            /// </param>
            /// <returns>
            /// {LangwordTrue} if the values are equal; otherwise, {LangwordFalse}.
            /// </returns>
            public bool Equals([AllowNull] {ParametrizedClassName(optionCount)} x, [AllowNull] {ParametrizedClassName(optionCount)} y)
            {{
                if (x is null) return y is null;
                if (y is null) return false;
                return x.{SingleDispatchedEqualsMethodName}(this, y);
            }}

            /// <summary>
            /// Generates a hash code for the specified value.
            /// </summary>
            /// <param name=""value"">
            /// The value for which a hash code must be generated.
            /// </param>
            /// <returns>
            /// A hash code for the specified value.
            /// </returns>
            public int GetHashCode([AllowNull] {ParametrizedClassName(optionCount)} value)
            {{
                if (value is null) return 0;
                return value.{SingleDispatchedGetHashCodeMethodName}(this);
            }}
        }}
";

        private static Func<int, string> PublicStaticConstructor(int optionCount)
            => typeIndex => $@"
        /// <summary>
        /// Creates a new {See(ReferToClassName(optionCount))} with a value of the {Ordinal(typeIndex)} type.
        /// </summary>
        public static {ParametrizedClassName(optionCount)} {OptionMethodName(typeIndex)}({TypeParameter(typeIndex)} value) => new {SubClassName(typeIndex)}(value);
";

        private static Func<int, string> ImplicitCastOperator(int optionCount)
            => typeIndex => $@"
        /// <summary>Converts a value to a {ClassName} instance.</summary>
        public static implicit operator {ParametrizedClassName(optionCount)}({TypeParameter(typeIndex)} value) => new {SubClassName(typeIndex)}(value);
";

        private static string PrivateConstructor()
            => $@"
        private {ClassName}() {{ }}
";

        private static Func<int, string> IsOptionMethod(int optionCount)
            => typeIndex => $@"
        /// <summary>
        /// Checks if this {See(ReferToClassName(optionCount))} contains a value of the {Ordinal(typeIndex)} type.
        /// </summary>
        /// <returns>
        /// {LangwordTrue} if this {See(ReferToClassName(optionCount))} contains a value of the {Ordinal(typeIndex)} type; otherwise {LangwordFalse}.
        /// </returns>
        public virtual bool {IsOptionMethodName(typeIndex)}() => false;
";

        private static Func<int, string> IsOptionMethodWithParameter(int optionCount)
            => typeIndex => $@"
        /// <summary>
        /// Checks if this {See(ReferToClassName(optionCount))} contains a value of the {Ordinal(typeIndex)} type.
        /// </summary>
        /// <param name=""value"">
        /// The value of the {Ordinal(typeIndex)} type, if this function returns {LangwordTrue}; otherwise a default value.
        /// </param>
        /// <returns>
        /// {LangwordTrue} if this {See(ReferToClassName(optionCount))} contains a value of the {Ordinal(typeIndex)} type; otherwise {LangwordFalse}.
        /// </returns>
        public virtual bool {IsOptionMethodName(typeIndex)}([AllowNull, NotNullWhen(true), MaybeNullWhen(false)] out {TypeParameter(typeIndex)} value)
        {{
            value = default;
            return false;
        }}
";

        private static Func<int, string> ToOptionMethod(int optionCount)
            => typeIndex => $@"
        /// <summary>
        /// Casts this {See(ReferToClassName(optionCount))} to a value of the {Ordinal(typeIndex)} type.
        /// </summary>
        /// <returns>
        /// The value of the {Ordinal(typeIndex)} type.
        /// </returns>
        /// <exception cref=""InvalidCastException"">
        /// Occurs when this {See(ReferToClassName(optionCount))} does not contain a value of the {Ordinal(typeIndex)} type.
        /// </exception>
        public virtual {TypeParameter(typeIndex)} {ToOptionMethodName(typeIndex)}() => throw new InvalidCastException();
";

        private static string MatchMethodFuncOverloadSummary()
            => $@"
        /// <summary>
        /// Invokes a {See("Func{T, TResult}")} based on the type of the value and returns its result.
        /// </summary>
        /// <typeparam name=""TResult"">
        /// Type of the value to return.
        /// </typeparam>";

        private static string MatchMethodFuncOverloadSummaryParameters(int typeIndex)
            => $@"
        /// <param name=""{WhenOptionParameterName(typeIndex)}"">
        /// The {See($@"Func{{{TypeParameter(typeIndex)}, TResult}}")} to invoke when the value is of the {Ordinal(typeIndex)} type.
        /// </param>";

        private static string MatchMethodFuncOverload(int optionCount)
            => $@"
        /// <param name=""{OtherwiseParameterName}"">
        /// The {See("Func{TResult}")} to invoke if no function is specified for the type of the value.
        /// If {CommaSeparatedList(optionCount - 1, WhenOptionParamRef)} and {WhenOptionParamRef(optionCount)} are given, this parameter is not used.
        /// </param>
        /// <returns>
        /// The result of the invoked {See("Func{T, TResult}")}.
        /// </returns>
        /// <exception cref=""{nameof(InvalidPatternMatchException)}"">
        /// No function argument was defined that matches the value.
        /// </exception>
        public abstract TResult Match<TResult>({ConcatList(optionCount, typeIndex => $@"
            {MatchMethodFuncOverloadParameter(typeIndex)}")}
            [AllowNull] Func<TResult> {OtherwiseParameterName} = null)
#if !NET472
            where TResult : notnull
#endif
            ;
";

        private static string MatchMethodActionOverloadSummary()
            => $@"
        /// <summary>
        /// Invokes an {See("Action{T}")} based on the type of the value.
        /// </summary>";

        private static string MatchMethodActionOverloadSummaryParameters(int typeIndex)
            => $@"
        /// <param name=""{WhenOptionParameterName(typeIndex)}"">
        /// The {See($@"Action{{{TypeParameter(typeIndex)}}}")} to invoke when the value is of the {Ordinal(typeIndex)} type.
        /// </param>";

        private static string MatchMethodActionOverload(int optionCount)
            => $@"
        /// <param name=""{OtherwiseParameterName}"">
        /// The {See("Action")} to invoke if no action is specified for the type of the value.
        /// If {CommaSeparatedList(optionCount - 1, WhenOptionParamRef)} and {WhenOptionParamRef(optionCount)} are given, this parameter is not used.
        /// </param>
        public abstract void Match({ConcatList(optionCount, typeIndex => $@"
            {MatchMethodActionOverloadParameter(typeIndex)}")}
            [AllowNull] Action {OtherwiseParameterName} = null);
";

        // Use a double dispatch mechanism for determining equality.
        private static string PrivateEqualityMethods(int optionCount)
            => $@"{ConcatList(optionCount, typeIndex => $@"
        private protected virtual bool {DoubleDispatchedEqualsMethodName(typeIndex)}(EqualityComparer equalityComparer, {TypeParameter(typeIndex)} y) => false;")}

        private protected abstract bool {SingleDispatchedEqualsMethodName}(EqualityComparer equalityComparer, {ParametrizedClassName(optionCount)} y);

        private protected abstract int {SingleDispatchedGetHashCodeMethodName}(EqualityComparer equalityComparer);
";

        private static string ClassBody(int optionCount)
            => string.Concat(
                ConcatList(optionCount, SubClass(optionCount)),
                EqualityComparerClass(optionCount),
                ConcatList(optionCount, PublicStaticConstructor(optionCount)),
                ConcatList(optionCount, ImplicitCastOperator(optionCount)),
                PrivateConstructor(),
                ConcatList(optionCount, IsOptionMethod(optionCount)),
                ConcatList(optionCount, IsOptionMethodWithParameter(optionCount)),
                ConcatList(optionCount, ToOptionMethod(optionCount)),
                MatchMethodFuncOverloadSummary(),
                ConcatList(optionCount, MatchMethodFuncOverloadSummaryParameters),
                MatchMethodFuncOverload(optionCount),
                MatchMethodActionOverloadSummary(),
                ConcatList(optionCount, MatchMethodActionOverloadSummaryParameters),
                MatchMethodActionOverload(optionCount),
                PrivateEqualityMethods(optionCount));

        private static string ClassFooter()
            => $@"    }}
";

        private static string UnionClass(int optionCount)
            => $"{ClassSummary(optionCount)}{ClassHeader(optionCount)}{ClassBody(optionCount)}{ClassFooter()}";

        private static string GenerateCode()
            => LicenseHeaderAndUsingStatements()
            + $@"
namespace System
{{{string.Concat(GenerateList(2, MaxOptionsToGenerate - 1, UnionClass))}}}
";

        /// <summary>
        /// Auto-generates Union.cs.
        /// </summary>
        public static void GenerateCodeAndOutputToFile(DirectoryInfo solutionDir)
        {
            try
            {
                // UTF8 encoding with preamble, Encoding.UTF8 doesn't do this.
                Encoding utf8WithPreamble = new UTF8Encoding(true, true);

                // Expect dir tree to look like this: [solution_dir]\Eutherion.Common\System
                DirectoryInfo unionCodeFileDir = solutionDir
                    .GetDirectories("Eutherion.Common")[0]
                    .GetDirectories("System")[0];

                File.WriteAllText(unionCodeFileDir.FullName + "\\Union.cs", GenerateCode(), utf8WithPreamble);
            }
            catch (Exception exception)
            {
                Debugger.Break();
                GC.KeepAlive(exception);
            }
        }
#endif
    }
}
