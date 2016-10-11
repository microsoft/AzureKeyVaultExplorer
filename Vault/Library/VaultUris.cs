// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

namespace Microsoft.Vault.Library
{
    using Microsoft.Vault.Core;
    using System;
    using System.Text.RegularExpressions;

    public enum VaultUriCollection
    {
        Keys,
        Secrets,
        Certificates
    }

    public static class VaultCollectionUtils
    {
        public static string ToCollectionName(this VaultUriCollection ve)
        {
            switch (ve)
            {
                case VaultUriCollection.Keys:
                    return "keys";
                case VaultUriCollection.Secrets:
                    return "secrets";
                case VaultUriCollection.Certificates:
                    return "certificates";
                default:
                    throw new ArgumentException($"Invalid VaultEndpoint {ve}");
            }
        }
    }

    public enum Action
    {
        Default
    }

    /// <summary>
    /// Either https:// or vault:// uri to vault, vault/collection, vault/collcetion/item or vault/collcetion/item/version
    /// </summary>
    public abstract class VaultUriBase : Uri
    {
        public readonly string VaultName;

        public readonly VaultUriCollection Collection;

        public readonly string ItemName;

        public readonly string Version;

        public readonly Action Action = Action.Default;

        public VaultUriBase(Regex uriRegex, string uriString) : base(uriString?.Replace(@"\", "/"))
        {
            Guard.ArgumentNotNull(uriRegex, nameof(uriRegex));
            Guard.ArgumentNotNullOrEmptyString(uriString, nameof(uriString));

            var m = uriRegex.Match(ToString());
            if (false == m.Success)
            {
                throw new ArgumentException($"Invalid vault URI {uriString}, URI must satisfy the following regex: {uriRegex}", nameof(uriString));
            }
            VaultName = m.Groups["VaultName"].Value;
            VaultUriCollection vuc;
            Enum.TryParse(m.Groups["Collection"].Value, true, out vuc);
            Collection = vuc;
            ItemName = m.Groups["Name"].Value;
            Version = string.IsNullOrEmpty(m.Groups["Version"].Value) ? null : m.Groups["Version"].Value;
        }

        public string VaultLink => $"vault://{VaultName}/{Collection.ToCollectionName()}/{ItemName}/{Version}".TrimEnd('/');

        public string HttpsLink => $"https://{VaultName}.vault.azure.net:443/{Collection.ToCollectionName()}/{ItemName}/{Version}".TrimEnd('/');
    }

    public class VaultHttpsUri : VaultUriBase
    {
        public VaultHttpsUri(string httpsUriString) : base(Consts.ValidVaultItemHttpsUriRegex, httpsUriString) { }
    }

    public class VaultLinkUri : VaultUriBase
    {
        public VaultLinkUri(string vaultUriString) : base(Consts.ValidVaultItemVaultUriRegex, vaultUriString) { }
    }
}
