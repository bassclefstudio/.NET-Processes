using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Processes
{
    /// <summary>
    /// Manages calls to a single command-line application and provides methods for dealing with input and output in a more streamlined manner.
    /// </summary>
    public class CommandLineProcess : IManagedProcess
    {
        /// <inheritdoc/>
        public Process MyProcess { get; private set; }

        private SourceStream<string> outputStream;
        /// <inheritdoc/>
        public IStream<string> OutputStream => outputStream;

        /// <summary>
        /// Creates a new <see cref="CommandLineProcess"/> for the specified <paramref name="programName"/>.
        /// </summary>
        /// <param name="programName">The path to the program being executed, or the name if it is included in the PATH.</param>
        /// <param name="shellExecute">A <see cref="bool"/> indicating whether the process should be started from the default shell.</param>
        public CommandLineProcess(string programName, bool shellExecute = false)
        {
            MyProcess = new Process();
            outputStream = new SourceStream<string>();

            // Requests execution of the provided program without the shell.
            MyProcess.StartInfo.UseShellExecute = shellExecute;
            MyProcess.StartInfo.FileName = programName;

            MyProcess.StartInfo.RedirectStandardInput = true;
            MyProcess.StartInfo.RedirectStandardOutput = true;
            MyProcess.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
        }

        #region Methods

        /// <inheritdoc/>
        public async Task<int> CallAsync(string arguments)
        {
            MyProcess.Refresh();
            MyProcess.StartInfo.Arguments = arguments;
            MyProcess.Start();
            while (!MyProcess.StandardOutput.EndOfStream)
            {
                string o = await MyProcess.StandardOutput.ReadLineAsync();
                outputStream.EmitValue(new StreamValue<string>(o));
            }
            await Task.Run(() =>
            {
                MyProcess.WaitForExit();
            });
            return MyProcess.ExitCode;
        }

        /// <inheritdoc/>
        public async Task WriteLineAsync(string input)
        {
            await MyProcess.StandardInput.WriteLineAsync(input);
            await MyProcess.StandardInput.FlushAsync();
        }

        /// <inheritdoc/>
        public async Task WriteAsync(string input)
        {
            await MyProcess.StandardInput.WriteAsync(input);
            await MyProcess.StandardInput.FlushAsync();
        }

        #endregion

        /// <inheritdoc/>
        public void Dispose()
        {
            if (MyProcess != null)
            {
                MyProcess.Dispose();
                MyProcess = null;
            }
        }
    }
}
