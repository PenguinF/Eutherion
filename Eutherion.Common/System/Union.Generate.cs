#region License
/*********************************************************************************
 * Union.Generate.cs
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

using System.Collections.Generic;
using System.Linq;

namespace System
{
    internal static class Union
    {
        private static string TypeParameter(int typeIndex)
            => $"T{typeIndex}";

        private static string WhenOptionParamName(int typeIndex)
            => $"whenOption{typeIndex}";

        private static readonly string OtherwiseParamName = "otherwise";

        internal static ArgumentException InvalidPatternMatchArgumentException(int typeIndex)
#if NET5_0_OR_GREATER
            => new($"{nameof(Union)} value is of type {TypeParameter(typeIndex)}, but both {WhenOptionParamName(typeIndex)} and {OtherwiseParamName} parameters are null.");
#else
            => new ArgumentException($"{nameof(Union)} value is of type {TypeParameter(typeIndex)}, but both {WhenOptionParamName(typeIndex)} and {OtherwiseParamName} parameters are null.");
#endif

#if DEBUG
        public const int MaxOptionsToGenerate = 8;

        public static string GeneratedCode { get; private set; }

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

        private static string OptionMethodName(int typeIndex)
            => $"Option{typeIndex}";

        private static string IsOptionMethodName(int typeIndex)
            => $"IsOption{typeIndex}";

        private static string ToOptionMethodName(int typeIndex)
            => $"ToOption{typeIndex}";

        private static string WhenOptionParamRef(int typeIndex)
            => $@"<paramref name=""{WhenOptionParamName(typeIndex)}""/>";

        private static string MatchMethodFuncOverloadParameter(int typeIndex)
            => $"[AllowNull] Func<{TypeParameter(typeIndex)}, TResult> {WhenOptionParamName(typeIndex)} = null,";

        private static string MatchMethodActionOverloadParameter(int typeIndex)
            => $"[AllowNull] Action<{TypeParameter(typeIndex)}> {WhenOptionParamName(typeIndex)} = null,";

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
    public abstract class {ClassName}<{TypeParameters(optionCount)}>
" + $@"#if !NET472{ConcatList(optionCount, GenericNotNullConstraintForTypeParam)}
" + $@"#endif
    {{";

        private static Func<int, string> SubClass(int optionCount)
            => typeIndex => $@"
        private sealed class {SubClassName(typeIndex)} : {ClassName}<{TypeParameters(optionCount)}>
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
                [AllowNull] Func<TResult> {OtherwiseParamName} = null)
                => {WhenOptionParamName(typeIndex)} != null ? {WhenOptionParamName(typeIndex)}(Value)
                : {OtherwiseParamName} != null ? {OtherwiseParamName}()
                : throw {ClassName}.InvalidPatternMatchArgumentException({typeIndex});

            public override void Match({ConcatList(optionCount, paramOption => $@"
                {MatchMethodActionOverloadParameter(paramOption)}")}
                [AllowNull] Action {OtherwiseParamName} = null)
            {{
                if ({WhenOptionParamName(typeIndex)} != null) {WhenOptionParamName(typeIndex)}(Value);
                else {OtherwiseParamName}?.Invoke();
            }}

            public override string ToString() => Value.ToString() ?? string.Empty;
        }}
";

        private static Func<int, string> PublicStaticConstructor(int optionCount)
            => typeIndex => $@"
        /// <summary>
        /// Creates a new <see cref=""{ClassName}{{{TypeParameters(optionCount)}}}""/> with a value of the {Ordinal(typeIndex)} type.
        /// </summary>
        public static {ClassName}<{TypeParameters(optionCount)}> {OptionMethodName(typeIndex)}({TypeParameter(typeIndex)} value) => new {SubClassName(typeIndex)}(value);
";

        private static Func<int, string> ImplicitCastOperator(int optionCount)
            => typeIndex => $@"
        /// <summary>Converts a value to a {ClassName} instance.</summary>
        public static implicit operator {ClassName}<{TypeParameters(optionCount)}>({TypeParameter(typeIndex)} value) => new {SubClassName(typeIndex)}(value);
";

        private static string PrivateConstructor()
            => $@"
        private {ClassName}() {{ }}
";

        private static Func<int, string> IsOptionMethod(int optionCount)
            => typeIndex => $@"
        /// <summary>
        /// Checks if this <see cref=""{ClassName}{{{TypeParameters(optionCount)}}}""/> contains a value of the {Ordinal(typeIndex)} type.
        /// </summary>
        /// <returns>
        /// True if this <see cref=""{ClassName}{{{TypeParameters(optionCount)}}}""/> contains a value of the {Ordinal(typeIndex)} type; otherwise false.
        /// </returns>
        public virtual bool {IsOptionMethodName(typeIndex)}() => false;
";

        private static Func<int, string> IsOptionMethodWithParameter(int optionCount)
            => typeIndex => $@"
        /// <summary>
        /// Checks if this <see cref=""{ClassName}{{{TypeParameters(optionCount)}}}""/> contains a value of the {Ordinal(typeIndex)} type.
        /// </summary>
        /// <param name=""value"">
        /// The value of the {Ordinal(typeIndex)} type, if this function returns true; otherwise a default value.
        /// </param>
        /// <returns>
        /// True if this <see cref=""{ClassName}{{{TypeParameters(optionCount)}}}""/> contains a value of the {Ordinal(typeIndex)} type; otherwise false.
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
        /// Casts this <see cref=""{ClassName}{{{TypeParameters(optionCount)}}}""/> to a value of the {Ordinal(typeIndex)} type.
        /// </summary>
        /// <returns>
        /// The value of the {Ordinal(typeIndex)} type.
        /// </returns>
        /// <exception cref=""InvalidCastException"">
        /// Occurs when this <see cref=""{ClassName}{{{TypeParameters(optionCount)}}}""/> does not contain a value of the {Ordinal(typeIndex)} type.
        /// </exception>
        public virtual {TypeParameter(typeIndex)} {ToOptionMethodName(typeIndex)}() => throw new InvalidCastException();
";

        private static string MatchMethodFuncOverloadSummary()
            => $@"
        /// <summary>
        /// Invokes a <see cref=""Func{{T, TResult}}""/> based on the type of the value and returns its result.
        /// </summary>
        /// <typeparam name=""TResult"">
        /// Type of the value to return.
        /// </typeparam>";

        private static string MatchMethodFuncOverloadSummaryParameters(int typeIndex)
            => $@"
        /// <param name=""{WhenOptionParamName(typeIndex)}"">
        /// The <see cref=""Func{{{TypeParameter(typeIndex)}, TResult}}""/> to invoke when the value is of the {Ordinal(typeIndex)} type.
        /// </param>";

        private static string MatchMethodFuncOverload(int optionCount)
            => $@"
        /// <param name=""{OtherwiseParamName}"">
        /// The <see cref=""Func{{TResult}}""/> to invoke if no function is specified for the type of the value.
        /// If {CommaSeparatedList(optionCount - 1, WhenOptionParamRef)} and {WhenOptionParamRef(optionCount)} are given, this parameter is not used.
        /// </param>
        /// <returns>
        /// The result of the invoked <see cref=""Func{{T, TResult}}""/>.
        /// </returns>
        /// <exception cref=""ArgumentException"">
        /// No function argument was defined that matches the value.
        /// </exception>
        public abstract TResult Match<TResult>({ConcatList(optionCount, typeIndex => $@"
            {MatchMethodFuncOverloadParameter(typeIndex)}")}
            [AllowNull] Func<TResult> {OtherwiseParamName} = null)
#if !NET472
            where TResult : notnull
#endif
            ;
";

        private static string MatchMethodActionOverloadSummary()
            => $@"
        /// <summary>
        /// Invokes an <see cref=""Action{{T}}""/> based on the type of the value.
        /// </summary>";

        private static string MatchMethodActionOverloadSummaryParameters(int typeIndex)
            => $@"
        /// <param name=""{WhenOptionParamName(typeIndex)}"">
        /// The <see cref=""Action{{{TypeParameter(typeIndex)}}}""/> to invoke when the value is of the {Ordinal(typeIndex)} type.
        /// </param>";

        private static string MatchMethodActionOverload(int optionCount)
            => $@"
        /// <param name=""{OtherwiseParamName}"">
        /// The <see cref=""Action""/> to invoke if no action is specified for the type of the value.
        /// If {CommaSeparatedList(optionCount - 1, WhenOptionParamRef)} and {WhenOptionParamRef(optionCount)} are given, this parameter is not used.
        /// </param>
        public abstract void Match({ConcatList(optionCount, typeIndex => $@"
            {MatchMethodActionOverloadParameter(typeIndex)}")}
            [AllowNull] Action {OtherwiseParamName} = null);
";

        private static string ClassBody(int optionCount)
            => string.Concat(
                ConcatList(optionCount, SubClass(optionCount)),
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
                MatchMethodActionOverload(optionCount));

        private static string ClassFooter()
            => $@"    }}
";

        private static string UnionClass(int optionCount)
            => $"{ClassSummary(optionCount)}{ClassHeader(optionCount)}{ClassBody(optionCount)}{ClassFooter()}";

        private static string GenerateCode()
            => $@"
namespace System
{{{string.Concat(GenerateList(2, MaxOptionsToGenerate - 1, UnionClass))}}}
";

        // Generates code for the Union<> classes.
        public static void Generate()
        {
            GeneratedCode = GenerateCode();
        }
#endif
    }
}
