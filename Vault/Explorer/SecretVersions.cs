// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Microsoft.Vault.Explorer
{
    public abstract class CustomVersion : ToolStripMenuItem
    {
        public readonly int Index;
        public readonly DateTime? Created;
        public readonly DateTime? Updated;
        public readonly string ChangedBy;
        public readonly ObjectIdentifier Id;

        public CustomVersion(int index, DateTime? created, DateTime? updated, string changedBy, ObjectIdentifier id) : base($"{Utils.NullableDateTimeToString(created)}")
        {
            Index = index;
            Created = created;
            Updated = updated;
            ChangedBy = changedBy;
            Id = id;
            ToolTipText = string.Format("Creation time:\t\t{0}\nLast updated time:\t{1}\nChanged by:\t\t{2}\nVersion:\t        {3}",
                Utils.NullableDateTimeToString(Created),
                Utils.NullableDateTimeToString(Updated),
                ChangedBy,
                Id.Version);
        }

        public override string ToString() => ((0 == Index) ? "Current value" : $"Value from {Text}") + Utils.DropDownSuffix;
    }

    public class SecretVersion : CustomVersion
    {
        public readonly SecretItem SecretItem;

        public SecretVersion(int index, SecretItem secretItem) : base(index, secretItem.Attributes.Created, secretItem.Attributes.Updated, Microsoft.Vault.Library.Utils.GetChangedBy(secretItem.Tags), secretItem.Identifier)
        {
            SecretItem = secretItem;
        }
    }

    public class CertificateVersion : CustomVersion
    {
        public readonly CertificateItem CertificateItem;

        public CertificateVersion(int index, CertificateItem certificateItem) : base(index, certificateItem.Attributes.Created, certificateItem.Attributes.Updated, Microsoft.Vault.Library.Utils.GetChangedBy(certificateItem.Tags), certificateItem.Identifier)
        {
            CertificateItem = certificateItem;
        }
    }
}
