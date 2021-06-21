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
            List<Exception> shellExs = new List<Exception>();
            string[] output = null;

            try
            {
                Console.WriteLine("Running in Powershell:");
                output = await $"Write-Host \"{message}\"".PowerShellCoreAsync();
                Console.WriteLine("Powershell call complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Powershell failed.");
                shellExs.Add(ex);
            }

            try
            {
                Console.WriteLine("Running in Bash:");
                output = await $"printf \"{message}\\n\"".BashAsync();
                Console.WriteLine("Bash call complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Bash failed.");
                shellExs.Add(ex);
            }

            if (shellExs.Count == 2)
            {
                foreach (var ex in shellExs)
                {
                    Console.WriteLine($"error: {ex}");
                }

                Assert.Fail("One or more of the shell tests failed to complete.");
            }

            Assert.AreEqual(1, output.Length, "Output is unexpected number of lines.");
            Assert.AreEqual(message, output[0], "Output value is unexpected.");
        }
    }
}
