using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Processes
{
    /// <summary>
    /// An value describing the <see cref="string"/> outputs received from an <see cref="IManagedProcess"/>' various output sources.
    /// </summary>
    public struct ProcessOutput
    {
        /// <summary>
        /// The type of output that has been received.
        /// </summary>
        public OutputType OutputType { get; }

        /// <summary>
        /// The literal <see cref="string"/> output value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Creates a new <see cref="ProcessOutput"/>.
        /// </summary>
        /// <param name="value">The literal <see cref="string"/> output value.</param>
        /// <param name="outputType">The type of output that has been received.</param>
        public ProcessOutput(string value, OutputType outputType)
        {
            Value = value;
            OutputType = outputType;
        }
    }

    /// <summary>
    /// An enum defining the types of output that can be managed by the <see cref="ProcessOutput"/> event.
    /// </summary>
    public enum OutputType
    {
        /// <summary>
        /// The output comes from the standard output stream.
        /// </summary>
        Standard = 0,
        /// <summary>
        /// The output comes from the process' error stream.
        /// </summary>
        Error = 1
    }
}
