﻿// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
using Azure.Security.KeyVault.Certificates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microsoft.Vault.Explorer
{
    public partial class CertificateDialog : ItemDialogBase<PropertyObjectCertificate, KeyVaultCertificate>
    {
        private CertificatePolicy _certificatePolicy; // There is one policy and multiple versions of kv certificate. A policy is a recipe to create a next version of the kv certificate.

        private CertificateDialog(ISession session, string title, ItemDialogBaseMode mode) : base(session, title, mode)
        {
            InitializeComponent();
        }

        /// <summary>
        /// New certificate from file
        /// </summary>
        public CertificateDialog(ISession session, FileInfo fi) : this(session, "New certificate", ItemDialogBaseMode.New)
        {
            KeyVaultCertificateWithPolicy cb = null;
            X509Certificate2 cert = null;
            ContentType contentType = ContentTypeUtils.FromExtension(fi.Extension);
            switch (contentType)
            {
                case ContentType.Certificate:
                    cert = new X509Certificate2(fi.FullName);
                    break;
                case ContentType.Pkcs12:
                    string password = null;
                    var pwdDlg = new PasswordDialog();
                    if (pwdDlg.ShowDialog() != DialogResult.OK)
                    {
                        DialogResult = DialogResult.Cancel;
                        return;
                    }
                    password = pwdDlg.Password;
                    cert = new X509Certificate2(fi.FullName, password, X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.Exportable);
                    break;
                case ContentType.KeyVaultCertificate:
                    var kvcf = Utils.LoadFromJsonFile<KeyVaultCertificateFile>(fi.FullName);
                    cb = kvcf.Deserialize();
                    cert = new X509Certificate2(cb.Cer);
                    break;
                default:
                    throw new ArgumentException($"Unsupported ContentType {contentType}");
            }
            NewCertificate(cb, cert);
        }

        /// <summary>
        /// New certificate from X509Certificate2
        /// </summary>
        public CertificateDialog(ISession session, X509Certificate2 cert) : this(session, "New certificate", ItemDialogBaseMode.New)
        {
            NewCertificate(null, cert);
        }

        /// <summary>
        /// Edit certificate
        /// </summary>
        public CertificateDialog(ISession session, string name, IEnumerable<CertificateProperties> versions) : this(session, $"Edit certificate {name}", ItemDialogBaseMode.Edit)
        {
            uxTextBoxName.ReadOnly = true;
            int i = 0;
            uxMenuVersions.Items.AddRange((from v in versions orderby v.CreatedOn descending select new CertificateVersion(i++, v)).ToArray());
            uxMenuVersions_ItemClicked(null, new ToolStripItemClickedEventArgs(uxMenuVersions.Items[0])); // Pass sender as NULL so _changed will be set to false
        }

        private void NewCertificate(KeyVaultCertificateWithPolicy cb, X509Certificate2 cert)
        {
            _certificatePolicy = cb?.Policy;
            _certificatePolicy = _certificatePolicy ?? new CertificatePolicy(cert.Issuer, cert.Subject)
            {
                Exportable = true,
                KeySize = 2048,
                KeyType = "RSA",
                ReuseKey = false,
                ContentType = CertificateContentType.Pkcs12
            };
            RefreshCertificateObject(cb, _certificatePolicy, cert);
            uxTextBoxName.Text = Utils.ConvertToValidSecretName(cert.GetNameInfo(X509NameType.SimpleName, false));
        }

        private void RefreshCertificateObject(KeyVaultCertificateWithPolicy cb, CertificatePolicy cp, X509Certificate2 certificate)
        {
            uxPropertyGridSecret.SelectedObject = PropertyObject = new PropertyObjectCertificate(cb, cp, certificate, SecretObject_PropertyChanged);
            uxTextBoxName.Text = PropertyObject.Name;
            uxToolTip.SetToolTip(uxLinkLabelSecretKind, PropertyObject.SecretKind.Description);
        }

        protected override void uxLinkLabelSecretKind_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            X509Certificate2UI.DisplayCertificate(PropertyObject.Certificate);
        }

        private void SecretObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _changed = true;
            InvalidateOkButton();
        }

        protected override async Task<KeyVaultCertificate> OnVersionChangeAsync(CustomVersion cv)
        {
            var cb = await _session.CurrentVault.GetCertificateAsync(cv.Name); // Pass NULL as a version to fetch current CertificatePolicy
            var cert = await _session.CurrentVault.GetCertificateWithExportableKeysAsync(cv.Name, cv.Version);
            if ((_certificatePolicy == null) && (cb.Policy != null)) // cb.Policy will be NULL when version is not current
            {
                _certificatePolicy = cb.Policy;
            }
            RefreshCertificateObject(cb, _certificatePolicy, cert);
            return cb;
        }

        private void CertificateDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
