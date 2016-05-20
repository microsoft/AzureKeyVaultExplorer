using Microsoft.Azure.KeyVault;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PS.Common.Vault.Explorer
{
    /// <summary>
    /// Represents .secret file
    /// </summary>
    [JsonObject]
    public class SecretFile
    {
        [JsonProperty]
        public readonly string CreatedBy;

        [JsonProperty]
        public readonly DateTimeOffset CreationTime;

        [JsonProperty]
        public readonly byte[] Data;

        public SecretFile(Secret s)
        {
            CreatedBy = $"{Environment.UserDomainName}\\{Environment.UserName}";
            CreationTime = DateTimeOffset.UtcNow;
            Data = ProtectedData.Protect(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(s, Formatting.Indented)), null, DataProtectionScope.CurrentUser);
        }

        public Secret Deserialize()
        {
            return JsonConvert.DeserializeObject<Secret>(Encoding.UTF8.GetString(ProtectedData.Unprotect(Data, null, DataProtectionScope.CurrentUser)));
        }

        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// --------------------------------------------------------------------------------------");
            sb.AppendLine("// Vault Explorer encrypted secret file");
            sb.AppendLine("// Do not edit manually!!!");
            sb.AppendLine("// This file can be opened only by the user who saved the secret");
            sb.AppendLine("// --------------------------------------------------------------------------------------");
            sb.AppendLine();
            sb.Append(JsonConvert.SerializeObject(this, Formatting.Indented));
            return sb.ToString();
        }
    }
}
