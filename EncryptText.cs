// Decompiled with JetBrains decompiler
// Type: UiPathTeam.Encryption.EncryptText
// Assembly: Encryption.Activities, Version=1.0.6844.28622, Culture=neutral, PublicKeyToken=null
// MVID: 9413F43C-DB44-4B40-BF85-4077182CCC47

using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace UiPathTeam.Encryption
{
  public class EncryptText : CodeActivity
  {
    [Category("Input")]
    [RequiredArgument]
    [Description("Text that needs to be encrypted")]
    public InArgument<string> InputString { get; set; }

    [Category("Input")]
    [RequiredArgument]
    [Description("Encryption key")]
    public InArgument<string> Key { get; set; }

    [Category("Output")]
    [Description("Encrypted text")]
    public OutArgument<string> Output { get; set; }

    protected override void Execute(CodeActivityContext context)
    {
      string str = EncryptText.EncryptString(this.InputString.Get((ActivityContext) context), this.Key.Get((ActivityContext) context));
      this.Output.Set((ActivityContext) context, str);
    }

    private static string EncryptString(string plainText, string passPhrase)
    {
      if (string.IsNullOrEmpty(plainText))
        return string.Empty;
      int num = 256;
      int iterations = 1000;
      byte[] salt = EncryptText.Generate256BitsOfRandomEntropy();
      byte[] rgbIV = EncryptText.Generate256BitsOfRandomEntropy();
      byte[] bytes1 = Encoding.UTF8.GetBytes(plainText);
      using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(passPhrase, salt, iterations))
      {
        byte[] bytes2 = rfc2898DeriveBytes.GetBytes(num / 8);
        using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
        {
          rijndaelManaged.BlockSize = 256;
          rijndaelManaged.Mode = CipherMode.CBC;
          rijndaelManaged.Padding = PaddingMode.PKCS7;
          using (ICryptoTransform encryptor = rijndaelManaged.CreateEncryptor(bytes2, rgbIV))
          {
            using (MemoryStream memoryStream = new MemoryStream())
            {
              using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, encryptor, CryptoStreamMode.Write))
              {
                cryptoStream.Write(bytes1, 0, bytes1.Length);
                cryptoStream.FlushFinalBlock();
                byte[] array = ((IEnumerable<byte>) ((IEnumerable<byte>) salt).Concat<byte>((IEnumerable<byte>) rgbIV).ToArray<byte>()).Concat<byte>((IEnumerable<byte>) memoryStream.ToArray()).ToArray<byte>();
                memoryStream.Close();
                cryptoStream.Close();
                return Convert.ToBase64String(array);
              }
            }
          }
        }
      }
    }

    private static byte[] Generate256BitsOfRandomEntropy()
    {
      byte[] data = new byte[32];
      using (RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider())
        cryptoServiceProvider.GetBytes(data);
      return data;
    }
  }
}
