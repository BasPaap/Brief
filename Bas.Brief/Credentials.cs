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
                EncryptedPasswordBytes = ProtectedData.Protect(passwordBytes, semiUniqueId, DataProtectionScope.CurrentUser);
            }
            else
            {
            }
        }

        public string GetDecryptedPassword()
        {   
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var decryptedPasswordBytes = ProtectedData.Unprotect(EncryptedPasswordBytes, semiUniqueId, DataProtectionScope.CurrentUser);
                char[] characters = new char[decryptedPasswordBytes.Length / sizeof(char)];
                System.Buffer.BlockCopy(decryptedPasswordBytes, 0, characters, 0, decryptedPasswordBytes.Length);
                return new string(characters);
            }
            else
            {
                throw new NotImplementedException();

                using Aes aes = Aes.Create();
                aes.Key = semiUniqueId;

            }
        }
    }
}
