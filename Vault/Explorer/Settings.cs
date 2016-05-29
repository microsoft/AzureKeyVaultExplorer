using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using System.Drawing.Design;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public class Settings : ApplicationSettingsBase
    {
        private static Settings defaultInstance = ((Settings)(ApplicationSettingsBase.Synchronized(new Settings())));

        public static Settings Default
        {
            get
            {
                return defaultInstance;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("00:00:30")]
        [DisplayName("Clear secret from clipboard after")]
        [Description("Interval for secret to stay in the clipboard once copied to the clipboard")]
        [Browsable(true)]
        [Category("General")]
        public TimeSpan CopyToClipboardTimeToLive
        {
            get
            {
                return ((TimeSpan)(this[nameof(CopyToClipboardTimeToLive)]));
            }
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(nameof(CopyToClipboardTimeToLive));
                }
                this[nameof(CopyToClipboardTimeToLive)] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue(@".\Vaults.json")]
        [DisplayName("Vaults file location")]
        [Description("Relative or absolute path to .json file with the vaults")]
        [Browsable(true)]
        [Category("Vaults")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string VaultsJsonFileLocation
        {
            get
            {
                return ((string)(this[nameof(VaultsJsonFileLocation)]));
            }
            set
            {
                this[nameof(VaultsJsonFileLocation)] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue(@".\VaultAliases.json")]
        [DisplayName("Vault aliases file location")]
        [Description("Relative or absolute path to .json file with the vault aliases")]
        [Browsable(true)]
        [Category("Vaults")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string VaultAliasesJsonFileLocation
        {
            get
            {
                return ((string)(this[nameof(VaultAliasesJsonFileLocation)]));
            }
            set
            {
                this[nameof(VaultAliasesJsonFileLocation)] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue(@".\SecretKinds.json")]
        [DisplayName("Secret kinds file location")]
        [Description("Relative or absolute path to .json file with the secret kinds")]
        [Browsable(true)]
        [Category("Vaults")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string SecretKindsJsonFileLocation
        {
            get
            {
                return ((string)(this[nameof(SecretKindsJsonFileLocation)]));
            }
            set
            {
                this[nameof(SecretKindsJsonFileLocation)] = value;
            }
        }
    }
}
