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

        private SourceStream<ProcessOutput> outputStream;
        /// <inheritdoc/>
        public IStream<ProcessOutput> OutputStream => outputStream;

        /// <summary>
        /// Creates a new <see cref="CommandLineProcess"/> for the specified <paramref name="programName"/>.
        /// </summary>
        /// <param name="programName">The path to the program being executed, or the name if it is included in the PATH.</param>
        /// <param name="shellExecute">A <see cref="bool"/> indicating whether the process should be started from the default shell.</param>
        public CommandLineProcess(string programName, bool shellExecute = false)
        {
            MyProcess = new Process();
            outputStream = new SourceStream<ProcessOutput>();

            // Requests execution of the provided program without the shell.
            MyProcess.StartInfo.UseShellExecute = shellExecute;
            MyProcess.StartInfo.FileName = programName;

            MyProcess.StartInfo.RedirectStandardInput = true;
            MyProcess.StartInfo.RedirectStandardOutput = true;
            MyProcess.StartInfo.WorkingDirectory = Environment.CurrentDirectory;

            MyProcess.Exited += ProcessExited;
            MyProcess.ErrorDataReceived += HandleError;
            MyProcess.OutputDataReceived += HandleStandardOutput;
        }

        #region Methods

        private TaskCompletionSource<int> ExitedSource { get; set; }

        /// <inheritdoc/>
        public async Task<int> CallAsync(string arguments)
        {
            MyProcess.Refresh();
            MyProcess.StartInfo.Arguments = arguments;
            ExitedSource = new TaskCompletionSource<int>();
            MyProcess.Start();
            return await ExitedSource.Task;
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
        #region ProcessEvents

        private void ProcessExited(object sender, EventArgs e)
        {
            if (ExitedSource != null)
            {
                MyProcess.WaitForExit();
                ExitedSource.SetResult(MyProcess.ExitCode);
            }
        }

        private void HandleStandardOutput(object sender, DataReceivedEventArgs e)
        {
            outputStream.EmitValue(new ProcessOutput(e.Data, OutputType.Standard));
        }

        private void HandleError(object sender, DataReceivedEventArgs e)
        {
            outputStream.EmitValue(new ProcessOutput(e.Data, OutputType.Error));
        }

        #endregion

        /// <inheritdoc/>
        public void Dispose()
        {
            if (MyProcess != null)
            {
                MyProcess.Exited -= ProcessExited;
                MyProcess.ErrorDataReceived -= HandleError;
                MyProcess.OutputDataReceived -= HandleStandardOutput;
                MyProcess.Dispose();
                MyProcess = null;
            }
        }
    }
}
