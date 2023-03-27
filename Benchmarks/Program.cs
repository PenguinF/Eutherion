﻿#region License
/*********************************************************************************
 * Program.cs
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

using BenchmarkDotNet.Running;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Benchmarks")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

namespace Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            // Command line args examples:
            // --filter *
            // --filter JsonParserBenchmarks
            new BenchmarkSwitcher(typeof(Program).Assembly).Run(args);
        }
    }
}
