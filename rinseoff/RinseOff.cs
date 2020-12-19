using System;
using Sodium;
using System.IO;
using System.Collections.Generic;
namespace rinseoff
{
    public class RinseOff
    {
        public static void generateKeyFile(string path){
            var key = Sodium.SodiumCore.GetRandomBytes(32);
            File.WriteAllBytes(path, key);
        }

        public static byte[] encrypt_secret_bytes(byte[] secret, byte[] key){
            byte[] nonce = Sodium.SecretBox.GenerateNonce();
            var ciphertext = SecretBox.Create(secret, nonce, key);
            var combined = new List<byte>();
            combined.AddRange(nonce);
            combined.AddRange(ciphertext);
            return combined.ToArray();
        }

        
    }
}
