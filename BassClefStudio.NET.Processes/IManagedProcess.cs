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
        /// Calls the given command asynchronously and monitors standard I/O.
        /// </summary>
        /// <param name="command">The shell script to execute.</param>
        /// <returns>The exit code returned by the exiting process.</returns>
        Task<int> CallCommandAsync(string command);

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
