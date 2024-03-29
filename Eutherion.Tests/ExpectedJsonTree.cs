﻿#region License
/*********************************************************************************
 * ExpectedJsonTree.cs
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
using System.Collections;
using System.Collections.Generic;

namespace Eutherion.Text.Json.Tests
{
    public abstract class ExpectedJsonTree : IEnumerable<ExpectedJsonTree>
    {
        public abstract Type ExpectedType { get; }
        public readonly List<ExpectedJsonTree> ChildNodes = new();

        // To enable collection initializer syntax:
        public IEnumerator<ExpectedJsonTree> GetEnumerator() => ChildNodes.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public void Add(ExpectedJsonTree child) => ChildNodes.Add(child);
    }

    public class ExpectedJsonTree<T> : ExpectedJsonTree where T : JsonSyntax
    {
        public override Type ExpectedType => typeof(T);
    }
}
