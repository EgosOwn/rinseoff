using NUnit.Framework;
using System.Text;
using System.Collections.Generic;
using Sodium;

using rinseoff;
namespace testDecrypted
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void testDecrypted()
        {
            var key = Sodium.SecretBox.GenerateKey();
            var nonce = Sodium.SecretBox.GenerateNonce();
            var msg = Encoding.Default.GetBytes("hey");
            var encrypted = Sodium.SecretBox.Create(msg, nonce, key);
            var combined = new List<byte>();
            combined.AddRange(nonce);
            combined.AddRange(encrypted);

            Assert.AreEqual(
                RinseOff.decrypt_secret_bytes(combined.ToArray(), key),
                msg
            );
        }
    }
}