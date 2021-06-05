using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Processes
{
    /// <summary>
    /// Provides extension methods for using <see cref="CommandLineProcess"/> to execute commands in the shell.
    /// </summary>
    public static class ShellExtensions
    {
        /// <summary>
        /// The default location to find the bash shell program.
        /// </summary>
        public const string DefaultBashLocation = "/bin/bash";

        /// <summary>
        /// Executes the given <paramref name="command"/> in the Bash shell using <see cref="IManagedProcess"/>.
        /// </summary>
        /// <param name="command">The <see cref="string"/> contents of the command to execute.</param>
        /// <param name="bashLocation">Specifies an alternative place to look for the Bash executable besides <see cref="DefaultBashLocation"/>.</param>
        /// <returns>A collection of <see cref="string"/> lines returned from the standard output streams.</returns>
        public static async Task<string[]> BashAsync(this string command, string bashLocation = DefaultBashLocation)
        {
            List<string> outputs = new List<string>();
            using(CommandLineProcess cmd = new CommandLineProcess(bashLocation))
            {
                cmd.OutputStream
                    .BindResult(o => outputs.Add(o))
                    .Start();
                string escapedArgs = command.Replace("\"", "\\\"");
                await cmd.CallAsync($"-c \"{escapedArgs}\"");
                return outputs.ToArray();
            }
        }

        /// <summary>
        /// The default location to find the bash shell program.
        /// </summary>
        public const string DefaultPSLocation = "powershell.exe";

        /// <summary>
        /// Executes the given <paramref name="command"/> in Powershell using <see cref="IManagedProcess"/>.
        /// </summary>
        /// <param name="command">The <see cref="string"/> contents of the command to execute.</param>
        /// <param name="psLocation">Specifies an alternative place to look for the Powershell executable besides <see cref="DefaultPSLocation"/>.</param>
        /// <returns>A collection of <see cref="string"/> lines returned from the standard output streams.</returns>
        public static async Task<string[]> PowershellAsync(this string command, string psLocation = DefaultPSLocation)
        {
            List<string> outputs = new List<string>();
            using (CommandLineProcess cmd = new CommandLineProcess(psLocation))
            {
                cmd.OutputStream
                    .BindResult(o => outputs.Add(o))
                    .Start();
                string escapedArgs = command.Replace("\"", "\\\"");
                await cmd.CallAsync($"\"{escapedArgs}\"");
                return outputs.ToArray();
            }
        }

        /// <summary>
        /// The default location to find the bash shell program.
        /// </summary>
        public const string DefaultCmdLocation = "cmd.exe";

        /// <summary>
        /// Executes the given <paramref name="command"/> in the Command Prompt using <see cref="IManagedProcess"/>.
        /// </summary>
        /// <param name="command">The <see cref="string"/> contents of the command to execute.</param>
        /// <param name="cmdLocation">Specifies an alternative place to look for the cmd.exe executable besides <see cref="DefaultCmdLocation"/>.</param>
        /// <returns>A collection of <see cref="string"/> lines returned from the standard output streams.</returns>
        public static async Task<string[]> CmdAsync(this string command, string cmdLocation = DefaultCmdLocation)
        {
            List<string> outputs = new List<string>();
            using (CommandLineProcess cmd = new CommandLineProcess(cmdLocation))
            {
                cmd.OutputStream
                    .BindResult(o => outputs.Add(o))
                    .Start();
                await cmd.CallAsync($"/c {command}");
                return outputs.ToArray();
            }
        }
    }
}
