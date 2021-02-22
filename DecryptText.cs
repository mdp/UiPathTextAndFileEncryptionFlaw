// Decompiled with JetBrains decompiler
// Type: UiPathTeam.Encryption.DecryptText
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
  public class DecryptText : CodeActivity
  {
    [Category("Input")]
    [RequiredArgument]
    [Description("Text that needs to be decrypted")]
    public InArgument<string> InputString { get; set; }

    [Category("Input")]
    [RequiredArgument]
    [Description("Decryption key")]
    public InArgument<string> Key { get; set; }

    [Category("Output")]
    [Description("Decrypted text")]
    public OutArgument<string> Output { get; set; }

    protected override void Execute(CodeActivityContext context)
    {
      string str = DecryptText.DecryptString(this.InputString.Get((ActivityContext) context), this.Key.Get((ActivityContext) context));
      this.Output.Set((ActivityContext) context, str);
    }

    public static string DecryptString(string cipherText, string passPhrase)
    {
      if (string.IsNullOrEmpty(cipherText))
        return string.Empty;
      int num = 256;
      int iterations = 1000;
      byte[] numArray1 = Convert.FromBase64String(cipherText);
      byte[] array1 = ((IEnumerable<byte>) numArray1).Take<byte>(num / 8).ToArray<byte>();
      byte[] array2 = ((IEnumerable<byte>) numArray1).Skip<byte>(num / 8).Take<byte>(num / 8).ToArray<byte>();
      byte[] array3 = ((IEnumerable<byte>) numArray1).Skip<byte>(num / 8 * 2).Take<byte>(numArray1.Length - num / 8 * 2).ToArray<byte>();
      using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(passPhrase, array1, iterations))
      {
        byte[] bytes = rfc2898DeriveBytes.GetBytes(num / 8);
        using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
        {
          rijndaelManaged.BlockSize = 256;
          rijndaelManaged.Mode = CipherMode.CBC;
          rijndaelManaged.Padding = PaddingMode.PKCS7;
          using (ICryptoTransform decryptor = rijndaelManaged.CreateDecryptor(bytes, array2))
          {
            using (MemoryStream memoryStream = new MemoryStream(array3))
            {
              using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, decryptor, CryptoStreamMode.Read))
              {
                byte[] numArray2 = new byte[array3.Length];
                int count = cryptoStream.Read(numArray2, 0, numArray2.Length);
                memoryStream.Close();
                cryptoStream.Close();
                return Encoding.UTF8.GetString(numArray2, 0, count);
              }
            }
          }
        }
      }
    }
  }
}
