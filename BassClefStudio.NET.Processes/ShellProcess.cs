using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Processes
{
    /// <summary>
    /// An <see cref="IManagedProcess"/> that manages the system's default shell.
    /// </summary>
    public class ShellProcess : IManagedProcess
    {
        /// <inheritdoc/>
        public Process MyProcess { get; private set; }

        private SourceStream<ProcessOutput> outputStream;
        /// <inheritdoc/>
        public IStream<ProcessOutput> OutputStream => outputStream;

        /// <summary>
        /// Creates a new <see cref="ShellProcess"/>.
        /// </summary>
        public ShellProcess()
        {
            MyProcess = new Process();
            outputStream = new SourceStream<ProcessOutput>();

            // Requests shell execution.
            MyProcess.StartInfo.UseShellExecute = true;
            MyProcess.StartInfo.RedirectStandardInput = false;
            MyProcess.StartInfo.RedirectStandardOutput = true;
            MyProcess.StartInfo.WorkingDirectory = Environment.CurrentDirectory;

            MyProcess.Exited += ProcessExited;
            MyProcess.ErrorDataReceived += HandleError;
            MyProcess.OutputDataReceived += HandleStandardOutput;
        }

        #region Methods

        private TaskCompletionSource<int> ExitedSource { get; set; }

        /// <inheritdoc/>
        public async Task<int> CallCommandAsync(string command)
        {
            MyProcess.Refresh();
            MyProcess.StartInfo.Arguments = command;
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
