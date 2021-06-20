using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Processes
{
    /// <summary>
    /// Abstracts the wrapper around a given process that can be executed and monitored for input and output.
    /// </summary>
    public interface IProcess : IDisposable
    {
        /// <summary>
        /// The <see cref="StreamReader"/> containing the standard output (stdout) of the process as it runs.
        /// </summary>
        StreamReader StandardOutput { get; }

        /// <summary>
        /// Starts the execution of the <see cref="IProcess"/> with the given arguments.
        /// </summary>
        /// <param name="arguments">The <see cref="string"/> args to include with the process.</param>
        void Start(string arguments);


        /// <summary>
        /// Waits for the running <see cref="IProcess"/> to exit, then retrieves the <see cref="int"/> exit code.
        /// </summary>
        /// <returns>The exit code of the completed process.</returns>
        Task<int> WaitCompletionAsync();


        /// <summary>
        /// Writes a <see cref="string"/> line to the process' standard input stream.
        /// </summary>
        /// <param name="input">The <see cref="string"/> input to write.</param>
        Task WriteLineAsync(string input);

        /// <summary>
        /// Writes a <see cref="string"/> to the process' standard input stream.
        /// </summary>
        /// <param name="input">The <see cref="string"/> input to write.</param>
        Task WriteAsync(string input);
    }
}
