// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Vault.Library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Microsoft.Vault.Explorer
{
    /// <summary>
    /// Secret object to edit via PropertyGrid
    /// </summary>
    public class PropertyObjectSecret : PropertyObject
    {
        /// <summary>
        /// Original secret
        /// </summary>
        private readonly SecretBundle _secret;

        private readonly CustomTags _customTags;

        [Category("General")]
        [DisplayName("Content Type")]
        [TypeConverter(typeof(ContentTypeEnumConverter))]
        public ContentType ContentType
        {
            get
            {
                return _contentType;
            }
            set
            {
                _contentType = value;
                NotifyPropertyChanged(nameof(ContentType));
            }
        }

        public PropertyObjectSecret(SecretBundle secret, PropertyChangedEventHandler propertyChanged) :
            base(secret.SecretIdentifier, secret.Tags, secret.Attributes.Enabled, secret.Attributes.Expires, secret.Attributes.NotBefore, propertyChanged)
        {
            _secret = secret;
            _contentType = ContentTypeEnumConverter.GetValue(secret.ContentType);
            _value = _contentType.FromRawValue(secret.Value);
            _customTags = Utils.LoadFromJsonFile<CustomTags>(Settings.Default.CustomTagsJsonFileLocation, isOptional: true);
        }

        protected override IEnumerable<TagItem> GetValueBasedCustomTags()
        {
            // Add tags based on all named groups in the value regex
            Match m = SecretKind.ValueRegex.Match(Value);
            if (m.Success)
            {
                for (int i = 0; i < m.Groups.Count; i++)
                {
                    string groupName = SecretKind.ValueRegex.GroupNameFromNumber(i);
                    if (groupName == i.ToString()) continue; // Skip unnamed groups
                    yield return new TagItem(groupName, m.Groups[i].Value);
                }
            }
        }

        public override void PopulateCustomTags()
        {
            if ((null == _customTags) || (_customTags.Count == 0)) return;
            // Add RequiredCustomTags and OptionalCustomTags
            foreach (var tagId in SecretKind.RequiredCustomTags.Concat(SecretKind.OptionalCustomTags))
            {
                if (false == _customTags.ContainsKey(tagId)) continue;
                Tags.AddOrKeep(_customTags[tagId].ToTagItem());
            }
        }

        // This method updates the SecretKind tag in Custom Tags
        public override void AddOrUpdateSecretKind(SecretKind sk)
        {
            TagItem newTag = new TagItem(Consts.SecretKindKey, sk.Alias);
            TagItem oldTag = this.Tags.GetOrNull(newTag);

            // Don't add the SecretKind to a secret that doesn't have any custom tags
            if ((null == _customTags) || (_customTags.Count == 0)) return;
            
            // Don't add the SecretKind to a secret that's defaulted to Custom
            if (sk.Alias == "Custom" && !this.Tags.Contains(newTag)) return;
            
            // Don't add the SecretKind to a secret that is defaulted to Custom and doesn't have any custom tags.
            if (oldTag == null && newTag.Value == "Custom") return;

            if (oldTag == null) // Add the SecretKind tag
            {
                Tags.AddOrReplace(newTag);
            }
            else if (oldTag.Value != newTag.Value) // Update the SecretKind tag
            {
                Tags.AddOrReplace(newTag);
            }
            else // Leave the SecretKind tag alone
            {
                Tags.AddOrReplace(oldTag);
            }
        }

        public override string AreCustomTagsValid()
        {
            if ((null == _customTags) || (_customTags.Count == 0)) return "";
            StringBuilder result = new StringBuilder();
            // Verify RequiredCustomTags
            foreach (var tagId in SecretKind.RequiredCustomTags)
            {
                if (false == _customTags.ContainsKey(tagId)) continue;
                var ct = _customTags[tagId];
                result.Append(ct.Verify(Tags.GetOrNull(ct.ToTagItem()), true));
            }
            // Verify OptionalCustomTags
            foreach (var tagId in SecretKind.OptionalCustomTags)
            {
                if (false == _customTags.ContainsKey(tagId)) continue;
                var ct = _customTags[tagId];
                result.Append(ct.Verify(Tags.GetOrNull(ct.ToTagItem()), false));
            }
            return result.ToString();
        }

        public override void PopulateExpiration()
        {
            // Set item expiration in case DefaultExpiration is not zero
            Expires = (default(TimeSpan) == SecretKind.DefaultExpiration) ? (DateTime?)null :
                DateTime.UtcNow.Add(SecretKind.DefaultExpiration);
        }

        public SecretAttributes ToSecretAttributes() => new SecretAttributes()
        {
            Enabled = Enabled,
            Expires = Expires,
            NotBefore = NotBefore
        };

        public override string GetKeyVaultFileExtension() => ContentType.KeyVaultSecret.ToExtension();

        public override DataObject GetClipboardValue()
        {
            var dataObj = base.GetClipboardValue();
            // We use SetData() and not SetText() to support correctly empty string "" as a value
            dataObj.SetData(DataFormats.UnicodeText, ContentType.IsCertificate() ? CertificateValueObject.FromValue(Value)?.Password : Value);
            return dataObj;
        }

        public override void SaveToFile(string fullName)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullName));
            switch (ContentTypeUtils.FromExtension(Path.GetExtension(fullName)))
            {
                case ContentType.KeyVaultSecret: // Serialize the entire secret as encrypted JSON for current user
                    File.WriteAllText(fullName, new KeyVaultSecretFile(_secret).Serialize());
                    break;
                case ContentType.KeyVaultCertificate:
                    throw new InvalidOperationException("One can't save key vault secret as key vault certificate");
                case ContentType.KeyVaultLink:
                    File.WriteAllText(fullName, GetLinkAsInternetShortcut());
                    break;
                case ContentType.Certificate:
                    File.WriteAllBytes(fullName, CertificateValueObject.FromValue(Value).Certificate.Export(X509ContentType.Cert));
                    break;
                case ContentType.Pkcs12:
                    File.WriteAllBytes(fullName, Convert.FromBase64String(CertificateValueObject.FromValue(Value).Data));
                    break;
                default:
                    File.WriteAllText(fullName, Value);
                    break;
            }
        }
    }
}
