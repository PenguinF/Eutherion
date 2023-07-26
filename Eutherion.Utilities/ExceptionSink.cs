#region License
/*********************************************************************************
 * ExceptionSink.cs
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

namespace Eutherion
{
    /// <summary>
    /// Provides an anchor method to suppress caught exceptions, and an event to trace them.
    /// </summary>
    public sealed class ExceptionSink
    {
        private static readonly SafeLazy<ExceptionSink> LazyInstance
#if NET5_0_OR_GREATER
            = new(() => new ExceptionSink());
#else
            = new SafeLazy<ExceptionSink>(() => new ExceptionSink());
#endif

        /// <summary>
        /// Gets the singleton instance of this class.
        /// </summary>
        public static ExceptionSink Instance => LazyInstance.Value;

        private ExceptionSink() { }

        /// <summary>
        /// Raises the <see cref="TracingException"/> event for an exception, with the result of
        /// suppressing the exception if no event handler is registered.
        /// </summary>
        /// <param name="exception">
        /// The exception to trace or suppress.
        /// </param>
        public void Trace(Exception exception)
        {
            if (exception != null)
            {
                TracingException?.Invoke(exception);
            }
        }

        /// <summary>
        /// Occurs when <see cref="Trace(Exception)"/> is called for a non-null <see cref="Exception"/>.
        /// </summary>
        /// <remarks>
        /// Remember that because this is an event with a lifetime equal to that of the application domain it's in,
        /// and because any object that handles the event inherits that lifetime, there is a potential for memory leaks.
        /// To prevent this from happening, make any object that handles the event eligible for garbage collection
        /// by unregistering the event.
        /// </remarks>
#if NET472
        public event Action<Exception> TracingException;
#else
        public event Action<Exception>? TracingException;
#endif
    }

    /// <summary>
    /// Defines a method to trace or suppress exceptions.
    /// </summary>
    public static class ExceptionSinkExtensions
    {
        /// <summary>
        /// Sends an <see cref="Exception"/> to <see cref="ExceptionSink.Instance"/>.
        /// </summary>
        /// <param name="exception">
        /// The <see cref="Exception"/> to trace or suppress.
        /// </param>
        public static void Trace(this Exception exception) => ExceptionSink.Instance.Trace(exception);
    }
}
