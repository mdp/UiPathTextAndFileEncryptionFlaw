# 

The following repository contains the technical documentation and supporting materials for this an encryption flaw in a popular UiPath encryption plugin. It includes the decompiled source for the plugin in question along with the nupkg itself and screenshots of the marketplace listing.

## Quick Technical Overview

I took the UiPathTeam.Encryption module and decompiled it using JetBrains dotPeek program. The relevant issues can be pulled from looking at the Encrypt File implementation:

```cs
private static void EncryptFileMethod(string inputFile, string outputFile, string password)
{
  while (password.Length <= 16)
    password += "x";
  if (string.IsNullOrEmpty(inputFile) && string.IsNullOrEmpty(outputFile))
    return;
  byte[] bytes = new UnicodeEncoding().GetBytes(password.Substring(0, 16));
  FileStream fileStream1 = new FileStream(outputFile, FileMode.Create);
  RijndaelManaged rijndaelManaged = new RijndaelManaged();
  rijndaelManaged.KeySize = 256;
  rijndaelManaged.BlockSize = 256;
  CryptoStream cryptoStream = new CryptoStream(
    (Stream) fileStream1,
    rijndaelManaged.CreateEncryptor(bytes, bytes), 
    CryptoStreamMode.Write);
  FileStream fileStream2 = new FileStream(inputFile, FileMode.Open);
  int num;
  while ((num = fileStream2.ReadByte()) != -1)
    cryptoStream.WriteByte((byte) num);
  fileStream2.Close();
  cryptoStream.Close();
  fileStream1.Close();
}
```

It should be noted that this plugin's “Text” encryption function, while still not advisable to use, doesn’t suffer from many of these same flaws. I assume at some point it was updated and the file encryption was either forgotten or left alone for one reason or another.
