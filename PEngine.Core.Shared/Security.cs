using System;
using System.Text;
using System.Security.Cryptography;

namespace PEngine.Core.Shared
{
  public class Security
  {
    public static string Encrypt(string plainText)
    {
      if (!string.IsNullOrEmpty(plainText))
      {
        var sha256 = System.Security.Cryptography.SHA256.Create();
        return BytesToHex(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(plainText)));
      }
      return string.Empty;
    }

    public static bool EncryptAndCompare(string plainText, string targetHash)
    {
      if (!string.IsNullOrEmpty(targetHash) && !string.IsNullOrEmpty(plainText))
      {
        var sha256 = System.Security.Cryptography.SHA256.Create();
        var computedHash = BytesToHex(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(plainText)));
        return computedHash == targetHash;
      }
      return true;
    }

    public static string BytesToHex(byte[] data)
    {
      StringBuilder retvalue = new StringBuilder();
      foreach (byte dbyte in data)
      {
          retvalue.Append(dbyte.ToString("x2").ToUpper());
      }
      return retvalue.ToString();
    }
  }
}