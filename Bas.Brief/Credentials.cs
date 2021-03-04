using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bas.Brief
{
    class Credentials
    {
        public string UserName { get; set; }

        public byte[] EncryptedPasswordBytes { get; set; }
        public byte[] IV { get; set; }

        private byte[] semiUniqueId;

        public Credentials()
        {
            var executableDrive = new DriveInfo(System.Reflection.Assembly.GetEntryAssembly().Location);

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(RuntimeInformation.OSArchitecture);
            stringBuilder.Append(RuntimeInformation.ProcessArchitecture);
            stringBuilder.Append(Environment.MachineName);
            stringBuilder.Append(Environment.ProcessorCount);
            stringBuilder.Append(Environment.UserName);
            stringBuilder.Append(executableDrive.DriveFormat);
            stringBuilder.Append(executableDrive.DriveType);
            stringBuilder.Append(executableDrive.Name);
            stringBuilder.Append(executableDrive.RootDirectory);
            stringBuilder.Append(executableDrive.TotalSize);
                        
            var unhashedSemiUniqueId = new byte[stringBuilder.Length * sizeof(char)];
            System.Buffer.BlockCopy(stringBuilder.ToString().ToCharArray(), 0, unhashedSemiUniqueId, 0, unhashedSemiUniqueId.Length);

            var sha256Hash = SHA256.Create();
            semiUniqueId = sha256Hash.ComputeHash(unhashedSemiUniqueId);
        }

        public Credentials(string userName, string password) 
            : this()
        {
            UserName = userName;

            var passwordBytes = new byte[password.Length * sizeof(char)];
            System.Buffer.BlockCopy(password.ToCharArray(), 0, passwordBytes, 0, passwordBytes.Length);
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Encrypt the password using the DPAPI, which is tied to the user account. Use the semi-unique ID as extra entropy.
                EncryptedPasswordBytes = ProtectedData.Protect(passwordBytes, semiUniqueId, DataProtectionScope.CurrentUser);
            }
            else
            {
                // This is less secure than the Windows method because we need to use a predictable key that is 
                // not tied to the user account or machine, but there is no DPAPI on OSX or Linux so there is no other choice.
                using var aes = Aes.Create();
                aes.Key = semiUniqueId; // semiUniqueId is a SHA256 has and thus 256 bits.
                IV = aes.IV;
                var aesEncryptor = aes.CreateEncryptor();

                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write))
                    {
                        using (var binaryWriter = new BinaryWriter(cryptoStream))
                        {
                            binaryWriter.Write(passwordBytes);
                        }
                        EncryptedPasswordBytes = memoryStream.ToArray();
                    }
                }
            }
        }

        public string GetDecryptedPassword()
        {   
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Decrypt the password using the DPAPI, which is tied to the user account. Use the semi-unique ID as extra entropy.
                var decryptedPasswordBytes = ProtectedData.Unprotect(EncryptedPasswordBytes, semiUniqueId, DataProtectionScope.CurrentUser);
                char[] characters = new char[decryptedPasswordBytes.Length / sizeof(char)];
                System.Buffer.BlockCopy(decryptedPasswordBytes, 0, characters, 0, decryptedPasswordBytes.Length);
                return new string(characters);
            }
            else
            {
                // This is less secure because we need to use a predictable key, but there is no DPAPI on OSX or Linux so there is no other choice.
                using Aes aes = Aes.Create();
                aes.Key = semiUniqueId;
                aes.IV = IV;
                var aesDecryptor = aes.CreateDecryptor();

                using (var memoryStream = new MemoryStream(EncryptedPasswordBytes))
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Read))
                    {
                        using (var binaryReader = new BinaryReader(cryptoStream))
                        {
                            var decryptedPasswordBytes = binaryReader.ReadBytes(EncryptedPasswordBytes.Length);

                            char[] characters = new char[decryptedPasswordBytes.Length / sizeof(char)];
                            System.Buffer.BlockCopy(decryptedPasswordBytes, 0, characters, 0, decryptedPasswordBytes.Length);
                            return new string(characters);
                        }
                    }
                }
            }
        }
    }
}
