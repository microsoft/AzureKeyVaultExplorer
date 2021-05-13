// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Microsoft.Vault.Explorer
{
    public abstract class CustomVersion : ToolStripMenuItem
    {
        public readonly int Index;
        public readonly DateTimeOffset? Created;
        public readonly DateTimeOffset? Updated;
        public readonly string ChangedBy;
        public readonly Uri Id;
        public readonly string Version;

        public CustomVersion(int index, DateTimeOffset? created, DateTimeOffset? updated, string changedBy, Uri id, string version) : base($"{Utils.NullableDateTimeToString(created)}")
        {
            Index = index;
            Created = created;
            Updated = updated;
            ChangedBy = changedBy;
            Id = id;
            Version = version;
            ToolTipText = string.Format("Creation time:\t\t{0}\nLast updated time:\t{1}\nChanged by:\t\t{2}\nVersion:\t        {3}",
                Utils.NullableDateTimeToString(Created),
                Utils.NullableDateTimeToString(Updated),
                ChangedBy,
                version);
        }

        public override string ToString() => ((0 == Index) ? "Current value" : $"Value from {Text}") + Utils.DropDownSuffix;
    }

    public class SecretVersion : CustomVersion
    {
        public readonly SecretProperties SecretItem;

        public SecretVersion(int index, SecretProperties secretItem) : base(index, secretItem.CreatedOn, secretItem.UpdatedOn, Microsoft.Vault.Library.Utils.GetChangedBy(secretItem.Tags), secretItem.Id, secretItem.Version)
        {
            SecretItem = secretItem;
        }
    }

    public class CertificateVersion : CustomVersion
    {
        public readonly CertificateProperties CertificateItem;

        public CertificateVersion(int index, CertificateProperties certificateItem) : base(index, certificateItem.CreatedOn, certificateItem.UpdatedOn, Microsoft.Vault.Library.Utils.GetChangedBy(certificateItem.Tags), certificateItem.Id, certificateItem.Version)
        {
            CertificateItem = certificateItem;
        }
    }
}
