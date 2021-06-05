using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Processes.Tests
{
    [TestClass]
    public class ProcessTests
    {
        [TestMethod]
        public async Task TestPowershellAsync()
        {
            string message = "Hello World!";
            string[] output = await $"echo \"{message}\"".PowershellAsync();
            Assert.AreEqual(1, output.Length, "Output is unexpected number of lines.");
            Assert.AreEqual(message, output[0], "Output value is unexpected.");
        }
    }
}
