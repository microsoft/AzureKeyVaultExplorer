// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Vault.Library;
using Microsoft.Vault.Core;

namespace Microsoft.Vault.Explorer
{
    [JsonObject]
    public class SecretKind : ToolStripMenuItem
    {
        [JsonProperty]
        public readonly string Alias;

        [JsonProperty]
        public readonly string Description;

        [JsonProperty]
        public readonly Regex NameRegex;

        [JsonProperty]
        public readonly Regex ValueRegex;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public readonly string ValueTemplate;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public readonly string CertificateFormat;

        [JsonIgnore]
        public bool IsCertificate => !string.IsNullOrEmpty(CertificateFormat);

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public readonly string[] RequiredCustomTags;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public readonly string[] OptionalCustomTags;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public readonly TimeSpan DefaultExpiration;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public readonly TimeSpan MaxExpiration;

        public SecretKind() : base("Custom")
        {
            Alias = "Custom";
            ToolTipText = Description = "The name must be a string 1-127 characters in length containing only 0-9, a-z, A-Z, and -.";
            NameRegex = Consts.ValidSecretNameRegex;
            ValueRegex = new Regex("^.{0,1048576}$", RegexOptions.Singleline | RegexOptions.Compiled);
            ValueTemplate = "";
            CertificateFormat = null;
            RequiredCustomTags = new string[0];
            OptionalCustomTags = new string[0];
            MaxExpiration = TimeSpan.MaxValue;
        }

        public SecretKind(string alias) : base(alias)
        {
            Alias = alias;
            ToolTipText = Description = "The name must be a string 1-127 characters in length containing only 0-9, a-z, A-Z, and -.";
            NameRegex = Consts.ValidSecretNameRegex;
            ValueRegex = new Regex("^.{0,1048576}$", RegexOptions.Singleline | RegexOptions.Compiled);
            ValueTemplate = "";
            CertificateFormat = null;
            RequiredCustomTags = new string[0];
            OptionalCustomTags = new string[0];
            MaxExpiration = TimeSpan.MaxValue;
        }

        [JsonConstructor]
        public SecretKind(string alias, string description, string nameRegex, string valueRegex, string valueTemplate, 
            string certificateFormat, string[] requiredCustomTags, string[] optionalCustomTags,
            TimeSpan defaultExpiration, TimeSpan maxExpiration) : base(alias)
        {
            Alias = alias;
            ToolTipText = Description = description;
            NameRegex = new Regex(nameRegex, RegexOptions.Singleline | RegexOptions.Compiled);
            ValueRegex = new Regex(valueRegex, RegexOptions.Singleline | RegexOptions.Compiled);
            ValueTemplate = valueTemplate;
            CertificateFormat = certificateFormat;
            RequiredCustomTags = requiredCustomTags ?? new string[0];
            OptionalCustomTags = optionalCustomTags ?? new string[0];
            DefaultExpiration = defaultExpiration;
            MaxExpiration = default(TimeSpan) == maxExpiration ? TimeSpan.MaxValue : maxExpiration;
            if (DefaultExpiration > MaxExpiration)
            {
                throw new ArgumentOutOfRangeException("DefaultExpiration or MaxExpiration", $"DefaultExpiration value must be less than MaxExpiration in secret kind {alias}");
            }
            if (RequiredCustomTags.Length + OptionalCustomTags.Length > Consts.MaxNumberOfTags)
            {
                throw new ArgumentOutOfRangeException("Total CustomTags.Length", $"Too many custom tags for secret kind {alias}, maximum number of tags for secret is only {Consts.MaxNumberOfTags}");
            }
        }

        public override string ToString() => Text + " secret name" + Utils.DropDownSuffix;
    }

    [JsonDictionary]
    public class SecretKinds : Dictionary<string, SecretKind>
    {
        public SecretKinds() : base() { }

        [JsonConstructor]
        public SecretKinds(IDictionary<string, SecretKind> secretKinds) : base(secretKinds, StringComparer.CurrentCultureIgnoreCase)
        {
            foreach (string secretKindName in Keys)
            {
                Guard.ArgumentNotNullOrWhitespace(secretKindName, nameof(secretKindName));
            }
        }
    }
}
