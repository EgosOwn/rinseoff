using NUnit.Framework;
using System.Text;
using System.Collections.Generic;

using rinseoff;
namespace testEncrypted
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestEncrypt()
        {
            var key = Sodium.SecretBox.GenerateKey();
            var secret = Encoding.UTF8.GetBytes("Hello world");
            var encrypted = RinseOff.encrypt_secret_bytes(secret, key);
            var encryptedList = new List<byte>();
            encryptedList.AddRange(encrypted);

            Assert.AreNotEqual(secret, encrypted);
            var nonce = encryptedList.GetRange(0, 24).ToArray();
            var cipher = encryptedList.GetRange(24, encryptedList.Count - 24).ToArray();

            var decrypted = Sodium.SecretBox.Open(cipher, nonce, key);
            Assert.AreEqual(decrypted, secret);
            
        }
    }
}