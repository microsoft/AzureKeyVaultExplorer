// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Vault.Explorer
{
    /// <summary>
    /// Represents .kv-secret file
    /// </summary>
    [JsonObject]
    public class KeyVaultSecretFile : KeyVaultFile<SecretBundle>
    {
        public KeyVaultSecretFile() : base() { }
        public KeyVaultSecretFile(SecretBundle secret) : base(secret) { }
    }

    /// <summary>
    /// Represents .kv-certificate file
    /// </summary>
    [JsonObject]
    public class KeyVaultCertificateFile : KeyVaultFile<CertificateBundle>
    {
        public KeyVaultCertificateFile() : base() { }
        public KeyVaultCertificateFile(CertificateBundle cb) : base(cb) { }
    }

    [JsonObject]
    public abstract class KeyVaultFile<T> where T : class
    {
        [JsonProperty]
        public readonly string CreatedBy;

        [JsonProperty]
        public readonly DateTimeOffset CreationTime;

        [JsonProperty]
        public readonly byte[] Data;

        [JsonConstructor]
        public KeyVaultFile() { }

        protected KeyVaultFile(T obj)
        {
            CreatedBy = $"{Environment.UserDomainName}\\{Environment.UserName}";
            CreationTime = DateTimeOffset.UtcNow;
            Data = ProtectedData.Protect(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, Formatting.Indented)), null, DataProtectionScope.CurrentUser);
        }

        private string GetValueForDeserialization() => Encoding.UTF8.GetString(ProtectedData.Unprotect(Data, null, DataProtectionScope.CurrentUser));

        public T Deserialize() => JsonConvert.DeserializeObject<T>(GetValueForDeserialization());

        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// --------------------------------------------------------------------------------------");
            sb.AppendLine($"// {Utils.AppName} encrypted {typeof(T).Name}");
            sb.AppendLine($"// Do not edit manually!!!");
            sb.AppendLine($"// This file can be opened only by the user who saved the file");
            sb.AppendLine($"// --------------------------------------------------------------------------------------");
            sb.AppendLine();
            sb.Append(JsonConvert.SerializeObject(this, Formatting.Indented));
            return sb.ToString();
        }
    }
}
