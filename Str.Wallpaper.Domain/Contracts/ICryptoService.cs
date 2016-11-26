using System;


namespace Str.Wallpaper.Domain.Contracts {

  public interface ICryptoService {

    /// <summary>
    /// Encrypt the given string using AES.  The string can be decrypted using
    /// DecryptStringAES().  The sharedSecret parameters must match.
    /// </summary>
    /// <param name="PlainText">The text to encrypt.</param>
    /// <param name="SharedSecret">A password used to generate a key for encryption.</param>
    /// <param name="NaCl">A previously created Salt.</param>
    /// <returns>A Tuple&lt;string, string&gt; of Salt and Encrypted String.</returns>
    Tuple<string, string> EncryptStringAes(string PlainText, string SharedSecret, string NaCl = null);

    /// <summary>
    /// Decrypt the given string.  Assumes the string was encrypted using
    /// EncryptStringAES(), using an identical sharedSecret.
    /// </summary>
    /// <param name="CipherText">The text to decrypt.</param>
    /// <param name="SharedSecret">A password used to generate a key for decryption.</param>
    /// <param name="NaCl">The Salt value used to encrypt the original value.</param>
    string DecryptStringAes(string CipherText, string SharedSecret, string NaCl);

  }

}
