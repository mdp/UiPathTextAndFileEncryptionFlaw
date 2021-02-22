// Decompiled with JetBrains decompiler
// Type: UiPathTeam.Encryption.DecryptFile
// Assembly: Encryption.Activities, Version=1.0.6844.28622, Culture=neutral, PublicKeyToken=null
// MVID: 9413F43C-DB44-4B40-BF85-4077182CCC47

using System.Activities;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace UiPathTeam.Encryption
{
  public class DecryptFile : CodeActivity
  {
    [Category("Input")]
    [RequiredArgument]
    [Description("Path of the file that needs to be decrypted")]
    public InArgument<string> InputFilePath { get; set; }

    [Category("Input")]
    [RequiredArgument]
    [Description("Path where the decrypted file will be saved")]
    public InArgument<string> OutputFilePath { get; set; }

    [Category("Input")]
    [RequiredArgument]
    [Description("Decryption key")]
    public InArgument<string> Key { get; set; }

    protected override void Execute(CodeActivityContext context) => DecryptFile.DecryptFileMethod(this.InputFilePath.Get((ActivityContext) context), this.OutputFilePath.Get((ActivityContext) context), this.Key.Get((ActivityContext) context));

    private static void DecryptFileMethod(string inputFile, string outputFile, string password)
    {
      while (password.Length <= 16)
        password += "x";
      if (string.IsNullOrEmpty(inputFile) && string.IsNullOrEmpty(outputFile))
        return;
      byte[] bytes = new UnicodeEncoding().GetBytes(password.Substring(0, 16));
      FileStream fileStream1 = new FileStream(inputFile, FileMode.Open);
      RijndaelManaged rijndaelManaged = new RijndaelManaged();
      rijndaelManaged.KeySize = 256;
      rijndaelManaged.BlockSize = 256;
      CryptoStream cryptoStream = new CryptoStream((Stream) fileStream1, rijndaelManaged.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read);
      FileStream fileStream2 = new FileStream(outputFile, FileMode.Create);
      int num;
      while ((num = cryptoStream.ReadByte()) != -1)
        fileStream2.WriteByte((byte) num);
      fileStream2.Close();
      cryptoStream.Close();
      fileStream1.Close();
    }
  }
}
