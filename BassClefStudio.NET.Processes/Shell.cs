using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using System.Linq;

namespace BassClefStudio.NET.Processes
{
    /// <summary>
    /// Provides extension methods and resources for using <see cref="IProcess"/>es to execute shell commands.
    /// </summary>
    public static class Shell
    {
        #region ShellLocations

        /// <summary>
        /// The default location to find the bash shell program.
        /// </summary>
        public const string DefaultBashLocation = "/bin/bash";

        /// <summary>
        /// The default location to find the PowerShell program.
        /// </summary>
        public const string DefaultPSLocation = "powershell.exe";

        /// <summary>
        /// The default location to find the Powershell Core (v6+) program.
        /// </summary>
        public const string DefaultPSCoreLocation = "pwsh.exe";

        /// <summary>
        /// The default location to find the Windows command line (CMD).
        /// </summary>
        public const string DefaultCmdLocation = "cmd.exe";

        #endregion
        #region Parsers

        /// <summary>
        /// Parses the expected input from a <see cref="IProcess"/>' standard output stream and returns the resulting value.
        /// </summary>
        /// <typeparam name="T">The type of output value of the parser.</typeparam>
        /// <param name="process">The <see cref="IProcess"/> to collect output from.</param>
        /// <param name="parser">The <see cref="Parser{TToken, T}"/> consuming output.</param>
        /// <returns>The <typeparamref name="T"/> tokenized output from the <paramref name="parser"/>.</returns>
        public static async Task<T> ParseOutputAsync<T>(this IProcess process, Parser<char, T> parser)
        {
            return await Task.Run(() => parser.ParseOrThrow(process.StandardOutput));
        }

        #endregion
        #region CommonCommands

        /// <summary>
        /// A <see cref="Parser{TToken, T}"/> which parses lines of text until the <see cref="End"/> of the stream.
        /// </summary>
        public static Parser<char, IEnumerable<string>> ParseLines { get; }
            = Try(Not(EndOfLine).Then(Any)).AtLeastOnceString().SeparatedAndOptionallyTerminated(EndOfLine).Before(End);

        /// <summary>
        /// Starts a new <see cref="IProcess"/> for the given <paramref name="path"/> program.
        /// </summary>
        /// <param name="arguments">The command arguments to pass to the shell.</param>
        /// <param name="path">The path to the shell application.</param>
        /// <returns>The initialized <see cref="IProcess"/> process.</returns>
        public static IProcess StartProcess(this string path, string arguments)
        {
            IProcess process = new CommandLineProcess(path);
            process.Start(arguments);
            return process;
        }

        /// <summary>
        /// Runs a command on the given <paramref name="path"/> shell program and returns the resulting output.
        /// </summary>
        /// <param name="arguments">The command arguments to pass to the shell.</param>
        /// <param name="path">The path to the shell application.</param>
        /// <returns>An array of <see cref="string"/> lines from the standard output.</returns>
        public static async Task<string[]> RunCommandAsync(this string arguments, string path)
        {
            using (IProcess process = path.StartProcess(arguments))
            {
                var lines = await process.ParseOutputAsync(ParseLines);
                await process.WaitCompletionAsync();
                return lines.ToArray();
            }
        }

        /// <summary>
        /// Starts a new PowerShell (Windows-only) <see cref="IProcess"/>.
        /// </summary>
        /// <param name="arguments">The command arguments to pass to the shell.</param>
        /// <returns>The initialized <see cref="IProcess"/> for the given command.</returns>
        public static IProcess StartPowerShell(this string arguments) => StartProcess(DefaultPSLocation, arguments);

        /// <summary>
        /// Runs a command in PowerShell (Windows-only) and returns the resulting output.
        /// </summary>
        /// <param name="arguments">The command arguments to pass to the shell.</param>
        /// <returns>An array of <see cref="string"/> lines from the standard output.</returns>
        public static async Task<string[]> RunPowerShellAsync(this string arguments) => await RunCommandAsync(arguments, DefaultPSLocation);

        /// <summary>
        /// Starts a new PowerShell Core <see cref="IProcess"/>.
        /// </summary>
        /// <param name="arguments">The command arguments to pass to the shell.</param>
        /// <returns>The initialized <see cref="IProcess"/> for the given command.</returns>
        public static IProcess StartPowerShellCore(this string arguments) => StartProcess(DefaultPSCoreLocation, arguments);

        /// <summary>
        /// Runs a command in PowerShell Core and returns the resulting output.
        /// </summary>
        /// <param name="arguments">The command arguments to pass to the shell.</param>
        /// <returns>An array of <see cref="string"/> lines from the standard output.</returns>
        public static async Task<string[]> RunPowerShellCoreAsync(this string arguments) => await RunCommandAsync($"-c {arguments}", DefaultPSCoreLocation);

        /// <summary>
        /// Starts a new Bash <see cref="IProcess"/>.
        /// </summary>
        /// <param name="arguments">The command arguments to pass to the shell.</param>
        /// <returns>The initialized <see cref="IProcess"/> for the given command.</returns>
        public static IProcess StartBash(this string arguments) => StartProcess(DefaultBashLocation, arguments);

        /// <summary>
        /// Runs a command in Bash and returns the resulting output.
        /// </summary>
        /// <param name="arguments">The command arguments to pass to the shell.</param>
        /// <returns>An array of <see cref="string"/> lines from the standard output.</returns>
        public static async Task<string[]> RunBashAsync(this string arguments) => await RunCommandAsync(arguments, DefaultBashLocation);

        /// <summary>
        /// Starts a new CMD.exe <see cref="IProcess"/>.
        /// </summary>
        /// <param name="arguments">The command arguments to pass to the shell.</param>
        /// <returns>The initialized <see cref="IProcess"/> for the given command.</returns>
        public static IProcess StartCmd(this string arguments) => StartProcess(DefaultCmdLocation, arguments);

        /// <summary>
        /// Runs a command in CMD.exe and returns the resulting output.
        /// </summary>
        /// <param name="arguments">The command arguments to pass to the shell.</param>
        /// <returns>An array of <see cref="string"/> lines from the standard output.</returns>
        public static async Task<string[]> RunCmdAsync(this string arguments) => await RunCommandAsync(arguments, DefaultCmdLocation);

        #endregion
    }
}
