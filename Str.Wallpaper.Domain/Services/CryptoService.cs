using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Security.Cryptography;

using Str.Wallpaper.Domain.Contracts;


namespace Str.Wallpaper.Domain.Services {

  [Export(typeof(ICryptoService))]
  public sealed class CryptoService : ICryptoService {

    #region ICryptoService Implementation

    public Tuple<string, string> EncryptStringAes(string PlainText, string SharedSecret, string NaCl = null) {
      if (String.IsNullOrEmpty(PlainText))    throw new ArgumentNullException(nameof(PlainText));
      if (String.IsNullOrEmpty(SharedSecret)) throw new ArgumentNullException(nameof(SharedSecret));

      string outStr; // Encrypted string to return

      AesManaged aesAlg = null; // AesManaged object used to encrypt the data.

      byte[] nacl = NaCl == null ? getRandomSalt() : base64ToBytes(NaCl);

      try {
        //
        // generate the key from the shared secret and the salt
        //
        Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(SharedSecret, nacl);
        //
        // Create a AesManaged object
        //
        aesAlg     = new AesManaged();
        aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
        //
        // Create a decryptor to perform the stream transform.
        //
        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using(MemoryStream msEncrypt = new MemoryStream()) {
          //
          // prepend the IV
          //
          msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
          msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

          using(CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
            using(StreamWriter swEncrypt = new StreamWriter(csEncrypt)) {
              swEncrypt.Write(PlainText);
            }
          }

          outStr = bytesToBase64(msEncrypt.ToArray());
        }
      }
      finally {
        aesAlg?.Clear();
      }

      return Tuple.Create(bytesToBase64(nacl), outStr);
    }

    public string DecryptStringAes(string CipherText, string SharedSecret, string NaCl) {
      if (String.IsNullOrEmpty(CipherText))   throw new ArgumentNullException(nameof(CipherText));
      if (String.IsNullOrEmpty(SharedSecret)) throw new ArgumentNullException(nameof(SharedSecret));

      string plaintext;

      AesManaged aesAlg = null;

      byte[] nacl = base64ToBytes(NaCl);

      try {
        //
        // generate the key from the shared secret and the salt
        //
        Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(SharedSecret, nacl);

        byte[] bytes = base64ToBytes(CipherText);

        using(MemoryStream msDecrypt = new MemoryStream(bytes)) {
          aesAlg = new AesManaged();

          aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

          aesAlg.IV = readByteArray(msDecrypt);

          ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

          using(CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
            using(StreamReader srDecrypt = new StreamReader(csDecrypt))
              plaintext = srDecrypt.ReadToEnd();
          }
        }
      }
      finally {
        aesAlg?.Clear();
      }

      return plaintext;
    }

    #endregion ICryptoService Implementation

    #region Private Methods

    private static byte[] readByteArray(Stream stream) {
      byte[] rawLength = new byte[sizeof(int)];

      if (stream.Read(rawLength, 0, rawLength.Length) != rawLength.Length) {
        throw new SystemException("Stream did not contain a properly formatted byte array");
      }

      byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];

      if (stream.Read(buffer, 0, buffer.Length) != buffer.Length) {
        throw new SystemException("Did not read byte array properly");
      }

      return buffer;
    }

    private static byte[] getRandomSalt() {
      RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();

      byte[] randomSalt = new byte[32]; // 256 bits

      random.GetBytes(randomSalt);

      return randomSalt;
    }

    private static string bytesToBase64(byte[] toConvert) {
      return Convert.ToBase64String(toConvert);
    }

    private static byte[] base64ToBytes(string base64) {
      return Convert.FromBase64String(base64);
    }

    #endregion Private Methods

  }

}
