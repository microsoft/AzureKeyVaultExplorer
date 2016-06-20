using Microsoft.Azure.KeyVault;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public enum SecretFileType
    {
        Key,
        Secret,
        Certificate // Key Vault Certificate
    }

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
        [JsonConverter(typeof(StringEnumConverter))]
        public readonly SecretFileType Type;

        [JsonProperty]
        public readonly byte[] Data;

        private SecretFile(SecretFileType type, object o)
        {
            CreatedBy = $"{Environment.UserDomainName}\\{Environment.UserName}";
            CreationTime = DateTimeOffset.UtcNow;
            Type = type;
            Data = ProtectedData.Protect(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(o, Formatting.Indented)), null, DataProtectionScope.CurrentUser);
        }

        [JsonConstructor]
        public SecretFile() { }

        public SecretFile(Secret s) : this(SecretFileType.Secret, s) { }

        public SecretFile(CertificateBundle cb) : this(SecretFileType.Certificate, cb) { }

        private string GetValueForDeserialization() => Encoding.UTF8.GetString(ProtectedData.Unprotect(Data, null, DataProtectionScope.CurrentUser));

        public Secret DeserializeAsSecret() => JsonConvert.DeserializeObject<Secret>(GetValueForDeserialization());

        public CertificateBundle DeserializeAsCertificateBundle() => JsonConvert.DeserializeObject<CertificateBundle>(GetValueForDeserialization());

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
