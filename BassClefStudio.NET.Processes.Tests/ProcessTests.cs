using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pidgin;

namespace BassClefStudio.NET.Processes.Tests
{
    [TestClass]
    public class ProcessTests
    {
        [TestMethod]
        public async Task TestShellAsync()
        {
            string message = "Hello World!";
            string[] output = null;

            try
            {
                Console.WriteLine("Running in Powershell:");
                output = await $"Write-Host \"{message}\"".RunPowerShellCoreAsync();
                Console.WriteLine("Powershell call complete.");

                Assert.AreEqual(1, output.Length, "Output is unexpected number of lines.");
                Assert.AreEqual(message, output[0], "Output value is unexpected.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Powershell failed. {ex}");
            }
        }
    }
}
