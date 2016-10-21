// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using Microsoft.Azure;
using Microsoft.Azure.Management.KeyVault;
using Microsoft.Azure.Management.KeyVault.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Vault.Library;

namespace Microsoft.Vault.Explorer
{
    public partial class SubscriptionsManagerDialog : Form
    {
        const string ApiVersion = "api-version=2016-07-01";
        const string ManagmentEndpoint = "https://management.azure.com/";

        private AccountItem _currentAccountItem;
        private AuthenticationResult _currentAuthResult;
        private KeyVaultManagementClient _currentKeyVaultMgmtClient;
        private readonly HttpClient _httpClient;

        public VaultAlias CurrentVaultAlias { get; private set; }

        public SubscriptionsManagerDialog()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            foreach (string domainHint in Settings.Default.DomainHintsList)
            {
                uxComboBoxAccounts.Items.Add(new AccountItem(domainHint));
            }
            uxComboBoxAccounts.SelectedIndex = 0;
        }

        private UxOperation NewUxOperationWithProgress(params ToolStripItem[] controlsToToggle) => new UxOperation(null, uxStatusLabel, uxProgressBar, uxButtonCancelOperation, controlsToToggle);

        private async void uxComboBoxAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentAccountItem = (AccountItem)uxComboBoxAccounts.SelectedItem;
            if (null == _currentAccountItem) return;
            using (var op = NewUxOperationWithProgress(uxComboBoxAccounts))
            {
                var vaui = new VaultAccessUserInteractive(_currentAccountItem.DomainHint);
                _currentAuthResult = vaui.AcquireToken(_currentAccountItem.AuthContext, ManagmentEndpoint);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_currentAuthResult.AccessTokenType, _currentAuthResult.AccessToken);
                var hrm = await _httpClient.GetAsync($"{ManagmentEndpoint}subscriptions?{ApiVersion}", op.CancellationToken);
                var json = await hrm.Content.ReadAsStringAsync();
                var subs = JsonConvert.DeserializeObject<SubscriptionsResponse>(json);

                uxListViewSubscriptions.Items.Clear();
                uxListViewVaults.Items.Clear();
                uxPropertyGridVault.SelectedObject = null;
                foreach (var s in subs.Subscriptions)
                {
                    uxListViewSubscriptions.Items.Add(new ListViewItemSubscription(s));
                }
            }
        }

        private async void uxListViewSubscriptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListViewItemSubscription s = uxListViewSubscriptions.SelectedItems.Count > 0 ? (ListViewItemSubscription)uxListViewSubscriptions.SelectedItems[0] : null;
            if (null == s) return;
            using (var op = NewUxOperationWithProgress(uxComboBoxAccounts))
            {
                var tvcc = new TokenCredentials(_currentAuthResult.AccessToken);
                _currentKeyVaultMgmtClient = new KeyVaultManagementClient(tvcc) { SubscriptionId = s.Subscription.SubscriptionId.ToString() };
                var vaults = await _currentKeyVaultMgmtClient.Vaults.ListAsync(null, op.CancellationToken);
                uxListViewVaults.Items.Clear();
                foreach (var v in vaults)
                {
                    uxListViewVaults.Items.Add(new ListViewItemVault(v));
                }
            }
        }

        private async void uxListViewVaults_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListViewItemSubscription s = uxListViewSubscriptions.SelectedItems.Count > 0 ? (ListViewItemSubscription)uxListViewSubscriptions.SelectedItems[0] : null;
            ListViewItemVault v = uxListViewVaults.SelectedItems.Count > 0 ? (ListViewItemVault)uxListViewVaults.SelectedItems[0] : null;
            uxButtonOK.Enabled = false;
            if ((null == s) || (null == v)) return;
            using (var op = NewUxOperationWithProgress(uxComboBoxAccounts))
            {
                var vault = await _currentKeyVaultMgmtClient.Vaults.GetAsync(v.GroupName, v.Name);
                uxPropertyGridVault.SelectedObject = new PropertyObjectVault(s.Subscription, v.GroupName, vault);
                uxButtonOK.Enabled = true;
                CurrentVaultAlias = new VaultAlias(v.Name, new string[] { v.Name }, new string[] { "Custom" }) { DomainHint = _currentAccountItem.DomainHint };
            }
        }
    }

    #region Aux UI related classes

    public class AccountItem
    {
        const string Authority = "https://login.windows.net/72f988bf-86f1-41af-91ab-2d7cd011db47";

        public readonly AuthenticationContext AuthContext;

        public readonly string DomainHint;

        public AccountItem(string domainHint)
        {
            AuthContext = new AuthenticationContext(Authority, new FileTokenCache(domainHint));
            DomainHint = domainHint;
        }

        public override string ToString() => $"{Environment.UserName}@{DomainHint}";
    }

    public class ListViewItemSubscription : ListViewItem
    {
        public readonly Subscription Subscription;

        public ListViewItemSubscription(Subscription s) : base(s.DisplayName)
        {
            Subscription = s;
            Name = s.DisplayName;
            SubItems.Add(s.SubscriptionId.ToString());
            ToolTipText = $"State: {s.State}";
            ImageIndex = 0;
        }
    }

    public class ListViewItemVault : ListViewItem
    {
        // https://azure.microsoft.com/en-us/documentation/articles/guidance-naming-conventions/
        private static Regex s_resourceNameRegex = new Regex(@".*\/resourceGroups\/(?<GroupName>[a-zA-Z0-9_\-\.]{1,64})\/", RegexOptions.CultureInvariant | RegexOptions.Compiled);

        public readonly Microsoft.Azure.Management.KeyVault.Models.Vault Vault;
        public readonly string GroupName;

        public ListViewItemVault(Microsoft.Azure.Management.KeyVault.Models.Vault vault) : base(vault.Name)
        {
            Vault = vault;
            Name = vault.Name;
            GroupName = s_resourceNameRegex.Match(vault.Id).Groups["GroupName"].Value;
            SubItems.Add(GroupName);
            ToolTipText = $"Location: {vault.Location}";
            ImageIndex = 1;
        }
    }

    public class PropertyObjectVault
    {
        private readonly Subscription _subscription;
        private readonly string _resourceGroupName;
        private readonly Microsoft.Azure.Management.KeyVault.Models.Vault _vault;

        public PropertyObjectVault(Subscription s, string resourceGroupName, Microsoft.Azure.Management.KeyVault.Models.Vault vault)
        {
            _subscription = s;
            _resourceGroupName = resourceGroupName;
            _vault = vault;
            Tags = new ObservableTagItemsCollection();
            if (null != _vault.Tags) foreach (var kvp in _vault.Tags) Tags.Add(new TagItem(kvp));
            AccessPolicies = new ObservableAccessPoliciesCollection();
            int i = -1;
            foreach (var ape in _vault.Properties.AccessPolicies) AccessPolicies.Add(new AccessPolicyEntryItem(++i, ape));
        }

        [DisplayName("Name")]
        [ReadOnly(true)]
        public string Name => _vault.Name;

        [DisplayName("Location")]
        [ReadOnly(true)]
        public string Location => _vault.Location;

        [DisplayName("Uri")]
        [ReadOnly(true)]
        public string Uri => _vault.Properties.VaultUri;

        [DisplayName("Subscription Name")]
        [ReadOnly(true)]
        public string SubscriptionName => _subscription.DisplayName;

        [DisplayName("Subscription Id")]
        [ReadOnly(true)]
        public Guid SubscriptionId => _subscription.SubscriptionId;

        [DisplayName("Resource Group Name")]
        [ReadOnly(true)]
        public string ResourceGroupName => _resourceGroupName;

        [DisplayName("Custom Tags")]
        [ReadOnly(true)]
        public ObservableTagItemsCollection Tags { get; private set; }

        [DisplayName("Sku")]
        [ReadOnly(true)]
        public SkuName Sku => _vault.Properties.Sku.Name;
        
        [DisplayName("Access Policies")]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableCollectionObjectConverter))]
        public ObservableAccessPoliciesCollection AccessPolicies { get; }
    }

    [Editor(typeof(ExpandableCollectionEditor<ObservableAccessPoliciesCollection, AccessPolicyEntryItem>), typeof(UITypeEditor))]
    public class ObservableAccessPoliciesCollection : ObservableCustomCollection<AccessPolicyEntryItem>
    {
        public ObservableAccessPoliciesCollection() : base() { }

        public ObservableAccessPoliciesCollection(IEnumerable<AccessPolicyEntryItem> collection) : base(collection) { }

        protected override PropertyDescriptor GetPropertyDescriptor(AccessPolicyEntryItem item) =>
            new ReadOnlyPropertyDescriptor($"[{item.Index}]", item);
    }

    [Editor(typeof(ExpandableObjectConverter), typeof(UITypeEditor))]
    public class AccessPolicyEntryItem
    {
        private static string[] EmptyList = new string[] { };
        private AccessPolicyEntry _ape;

        public AccessPolicyEntryItem(int index, AccessPolicyEntry ape)
        {
            Index = index;
            _ape = ape;
        }

        [JsonIgnore]
        public int Index { get; }

        [Description("Application ID of the client making request on behalf of a principal")]
        public Guid? ApplicationId => _ape.ApplicationId;

        [Description("Object ID of the principal")]
        public Guid ObjectId => _ape.ObjectId;

        [Description("Permissions to keys")]
        public string PermissionsToKeys => string.Join(",", _ape.Permissions.Keys ?? EmptyList);

        [Description("Permissions to secrets")]
        public string PermissionsToSecrets => string.Join(",", _ape.Permissions.Secrets ?? EmptyList);

        [Description("Permissions to certificates")]
        public string PermissionsToCertificates => string.Join(",", _ape.Permissions.Certificates ?? EmptyList);

        [Description("Tenant ID of the principal")]
        public Guid TenantId => _ape.TenantId;

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    #endregion

    #region Managment endpoint JSON response classes

    [JsonObject]
    public class SubscriptionsResponse
    {
        [JsonProperty(PropertyName = "value")]
        public Subscription[] Subscriptions { get; set; }
    }

    [JsonObject]
    public class Subscription
    {
        public string Id { get; set; }
        public Guid SubscriptionId { get; set; }
        public string DisplayName { get; set; }
        public string State { get; set; }
        public string AuthorizationSource { get; set; }
    }

    #endregion
}
