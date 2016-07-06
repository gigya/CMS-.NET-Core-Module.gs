using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gigya.Module.Connector.Encryption;

namespace Gigya.UnitTests.Encryption
{
    [TestClass]
    public class EncryptionTests
    {
        [TestMethod]
        public void CanEncryptAndDecrypt()
        {
            var plainText = "secret message!";

            var cipherText = Encryptor.Encrypt(plainText);

            Assert.IsNotNull(cipherText, "Cipher text is null");
            Assert.IsTrue(cipherText.Length > 0, "Cipher text has a length of 0");

            var decryptedText = Encryptor.Decrypt(cipherText);

            Assert.AreEqual(decryptedText, plainText, "Decprypted text doesn't match original");
        }


        [TestMethod]
        public void CanEncryptAndDecryptLongMessage()
        {
            var plainText = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.";

            var cipherText = Encryptor.Encrypt(plainText);

            Assert.IsNotNull(cipherText, "Cipher text is null");
            Assert.IsTrue(cipherText.Length > 0, "Cipher text has a length of 0");

            var decryptedText = Encryptor.Decrypt(cipherText);

            Assert.AreEqual(decryptedText, plainText, "Decprypted text doesn't match original");
        }

        [TestMethod]
        public void EncryptTwiceUniqueCipher()
        {
            var plainText = "hello";

            var cipher1 = Encryptor.Encrypt(plainText);
            var cipher2 = Encryptor.Encrypt(plainText);

            Assert.AreNotEqual(cipher1, cipher2, "Same string encrypted twice shouldn't be the same.");
        }
    }
}
