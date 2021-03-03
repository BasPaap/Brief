using System;
using System.Collections.Generic;
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

        public Credentials()
        {
        }

        public Credentials(string userName, string password)
        {
            UserName = userName;

            var passwordBytes = new byte[password.Length * sizeof(char)];
            System.Buffer.BlockCopy(password.ToCharArray(), 0, passwordBytes, 0, passwordBytes.Length);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                EncryptedPasswordBytes = ProtectedData.Protect(passwordBytes, null, DataProtectionScope.CurrentUser);
            }
        }

        public string GetDecryptedPassword()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var decryptedPasswordBytes = ProtectedData.Unprotect(EncryptedPasswordBytes, null, DataProtectionScope.CurrentUser);
                char[] characters = new char[decryptedPasswordBytes.Length / sizeof(char)];
                System.Buffer.BlockCopy(decryptedPasswordBytes, 0, characters, 0, decryptedPasswordBytes.Length);
                return new string(characters);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
