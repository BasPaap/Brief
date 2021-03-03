using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bas.Brief
{
    public static class BriefInitializer
    {
        public static void Initialize(string userName, string password)
        {
            var credentials = new Credentials(userName, password);

            var credentialsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), Brief.ApplicationDataFolderName, Brief.CredentialsFileName);
            var jsonCredentials = JsonSerializer.Serialize(credentials);
            File.WriteAllText(credentialsPath, jsonCredentials);
        }
    }
}
