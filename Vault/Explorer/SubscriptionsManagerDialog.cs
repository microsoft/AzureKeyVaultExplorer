using Microsoft.Azure;
using Microsoft.Azure.Management.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
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

namespace Microsoft.PS.Common.Vault.Explorer
{
    public partial class SubscriptionsManagerDialog : Form
    {
        const string ApiVersion = "api-version=2016-07-01";
        const string Authority = "https://login.windows.net/72f988bf-86f1-41af-91ab-2d7cd011db47";
        const string ManagmentEndpoint = "https://management.azure.com/";

        private readonly AuthenticationContext _authContext;
        private AuthenticationResult _currentAuthResult;
        private readonly HttpClient _httpClient;

        public SubscriptionsManagerDialog()
        {
            InitializeComponent();
            uxComboBoxAccounts.Items.Add(new AccountItem("microsoft.com"));
            uxComboBoxAccounts.Items.Add(new AccountItem("gme.gbl"));
            _authContext = new AuthenticationContext(Authority, new FileTokenCache());
            _httpClient = new HttpClient();
        }

        private UxOperation NewUxOperationWithProgress(params ToolStripItem[] controlsToToggle) => new UxOperation(null, uxStatusLabel, uxProgressBar, uxButtonCancelOperation, controlsToToggle);

        private async void uxComboBoxAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            AccountItem ai = (AccountItem)uxComboBoxAccounts.SelectedItem;
            if (null == ai) return;
            using (var op = NewUxOperationWithProgress(uxComboBoxAccounts))
            {
                var vaui = new VaultAccessUserInteractive(ai.DomainHint);
                _currentAuthResult = vaui.AcquireToken(_authContext, ManagmentEndpoint);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_currentAuthResult.AccessTokenType, _currentAuthResult.AccessToken);
                var hrm = await _httpClient.GetAsync($"{ManagmentEndpoint}subscriptions?{ApiVersion}", op.CancellationToken);
                var json = await hrm.Content.ReadAsStringAsync();
                var subs = JsonConvert.DeserializeObject<SubscriptionsResponse>(json);

                uxListViewSubscriptions.Items.Clear();
                foreach (var s in (from s in subs.Subscriptions orderby s.DisplayName select s))
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
                var hrm = await _httpClient.GetAsync($"{ManagmentEndpoint}subscriptions/{s.Subscription.SubscriptionId}/resources?{ApiVersion}&$filter=resourceType eq 'Microsoft.KeyVault/vaults'", op.CancellationToken);
                var json = await hrm.Content.ReadAsStringAsync();
                var rr = JsonConvert.DeserializeObject<ResourcesResponse>(json);
                uxListViewVaults.Items.Clear();
                foreach (var r in (from r in rr.Resources orderby r.Name select r))
                {
                    uxListViewVaults.Items.Add(new ListViewItemVault(s.Subscription.SubscriptionId, r));
                }
            }
        }

        private void uxListViewVaults_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListViewItemVault v = uxListViewVaults.SelectedItems.Count > 0 ? (ListViewItemVault)uxListViewVaults.SelectedItems[0] : null;
            if (null == v) return;
            using (var op = NewUxOperationWithProgress(uxComboBoxAccounts))
            {
                var tvcc = new TokenCloudCredentials(v.SubscriptionId.ToString(), _currentAuthResult.AccessToken);
                var kvmc = new KeyVaultManagementClient(tvcc);
                var vgr = kvmc.Vaults.Get(v.VaultResource.GroupName, v.Name);
                uxPropertyGridVault.SelectedObject = new PropertyObjectVault(vgr.Vault);
            }
        }
    }

    #region Aux UI related classes

    public class AccountItem
    {
        public readonly string DomainHint;

        public AccountItem(string domainHint) { DomainHint = domainHint; }

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
        public readonly Resource VaultResource;
        public readonly Guid SubscriptionId;

        public ListViewItemVault(Guid subscriptionId, Resource r) : base(r.Name)
        {
            SubscriptionId = subscriptionId;
            VaultResource = r;
            Name = r.Name;
            SubItems.Add(r.GroupName);
            ToolTipText = $"Location: {r.Location}";
            ImageIndex = 1;
        }
    }

    public class PropertyObjectVault
    {
        private readonly Azure.Management.KeyVault.Vault _vault;

        public PropertyObjectVault(Azure.Management.KeyVault.Vault vault)
        {
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

        [DisplayName("Custom Tags")]
        [ReadOnly(true)]
        public ObservableTagItemsCollection Tags { get; private set; }

        [DisplayName("Sku")]
        [ReadOnly(true)]
        public string Sku => _vault.Properties.Sku.Name;
        
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
        public string PermissionsToKeys => string.Join(",", _ape.PermissionsToKeys);

        [Description("Permissions to secrets")]
        public string PermissionsToSecrets => string.Join(",", _ape.PermissionsToSecrets);

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

    [JsonObject]
    public class ResourcesResponse
    {
        [JsonProperty(PropertyName = "value")]
        public Resource[] Resources { get; set; }
    }

    [JsonObject]
    public class Resource
    {
        private static Regex s_resourceNameRegex = new Regex(@".*\/resourceGroups\/(?<GroupName>[a-zA-Z._-]{1,64})\/", RegexOptions.CultureInvariant | RegexOptions.Compiled);

        public readonly string Id;
        public readonly string Name;
        public readonly string Type;
        public readonly string Location;
        public readonly string Kind;
        public readonly string GroupName;

        [JsonConstructor]
        public Resource(string id, string name, string type, string location, string kind)
        {
            Id = id;
            Name = name;
            Type = type;
            Location = location;
            Kind = kind;
            GroupName = s_resourceNameRegex.Match(id).Groups["GroupName"].Value;
        }
    }

    #endregion
}
