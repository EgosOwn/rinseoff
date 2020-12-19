using NUnit.Framework;
using System.IO;

using rinseoff;
namespace tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestKeyGenBasic()
        {
            var f = Path.GetTempFileName();
            RinseOff.generateKeyFile(f);
            var k = File.ReadAllBytes(f);
            Assert.IsTrue(k.Length == 32);
            Assert.IsFalse(k[0] == k[1]);
            File.Delete(f);
        }
    }
}