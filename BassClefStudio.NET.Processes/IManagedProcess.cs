using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Processes
{
    /// <summary>
    /// Represents a use-specific wrapper around <see cref="Process"/> that provides methods for dealing with input/output and common scenarios.
    /// </summary>
    public interface IManagedProcess : IDisposable
    {
        /// <summary>
        /// The <see cref="Process"/> used internally by the <see cref="IManagedProcess"/>.
        /// </summary>
        Process MyProcess { get; }

        /// <summary>
        /// Calls the process asynchronously and monitors standard I/O.
        /// </summary>
        /// <param name="arguments">The parameters provided to the process when it is launched.</param>
        /// <returns>The exit code returned by the process.</returns>
        Task<int> CallAsync(string arguments);

        /// <summary>
        /// Writes the given line to the attached <see cref="Process.StandardInput"/>.
        /// </summary>
        /// <param name="input">The <see cref="string"/> input to write.</param>
        Task WriteLineAsync(string input);

        /// <summary>
        /// Writes the given input to the attached <see cref="Process.StandardInput"/>.
        /// </summary>
        /// <param name="input">The <see cref="string"/> input to write.</param>
        Task WriteAsync(string input);

        /// <summary>
        /// An <see cref="IStream{T}"/> which emits whenever output is recived from the standard I/O.
        /// </summary>
        IStream<ProcessOutput> OutputStream { get; }
    }
}
