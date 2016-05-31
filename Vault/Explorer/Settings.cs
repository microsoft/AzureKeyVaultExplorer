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
using System.Drawing;

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
        [Description("Interval for secret to stay in the clipboard once copied to the clipboard.")]
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
        [DefaultSettingValue("Courier New, 9.75pt")]
        [DisplayName("Secret font")]
        [Description("Font to use for secret value and name in the secret dialog.")]
        [Browsable(true)]
        [Category("Secret dialog")]
        public Font SecretFont
        {
            get
            {
                return ((Font)(this[nameof(SecretFont)]));
            }
            set
            {
                this[nameof(SecretFont)] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("true")]
        [DisplayName("Show line numbers")]
        [Description("Display or hide line numbering in the secret dialog.")]
        [Browsable(true)]
        [Category("Secret dialog")]
        public bool ShowLineNumbers
        {
            get
            {
                return ((bool)(this[nameof(ShowLineNumbers)]));
            }
            set
            {
                this[nameof(ShowLineNumbers)] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("true")]
        [DisplayName("Convert tabs to spaces")]
        [Description("Convert tabs to spaces in the secret dialog.")]
        [Browsable(true)]
        [Category("Secret dialog")]
        public bool ConvertTabsToSpaces
        {
            get
            {
                return ((bool)(this[nameof(ConvertTabsToSpaces)]));
            }
            set
            {
                this[nameof(ConvertTabsToSpaces)] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("4")]
        [DisplayName("Tab indent size")]
        [Description("Tab indent size in the secret dialog.")]
        [Browsable(true)]
        [Category("Secret dialog")]
        public int TabIndent
        {
            get
            {
                return ((int)(this[nameof(TabIndent)]));
            }
            set
            {
                this[nameof(TabIndent)] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue(@".\")]
        [DisplayName("Root location")]
        [Description("Relative or absolute path to root folder where .json files are located.\nEnvironment variables are supported and expanded accordingly.")]
        [Browsable(true)]
        [Category("Vaults configuration")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string JsonConfigurationFilesRoot
        {
            get
            {
                return ((string)(this[nameof(JsonConfigurationFilesRoot)]));
            }
            set
            {
                this[nameof(JsonConfigurationFilesRoot)] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue(@"Vaults.json")]
        [DisplayName("Vaults file name")]
        [Description("Relative or absolute path to .json file with vaults definitions and access.\nEnvironment variables are supported and expanded accordingly.")]
        [Browsable(true)]
        [Category("Vaults configuration")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
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
        [DefaultSettingValue(@"VaultAliases.json")]
        [DisplayName("Vault aliases file name")]
        [Description("Relative or absolute path to .json file with vault aliases.\nEnvironment variables are supported and expanded accordingly.")]
        [Browsable(true)]
        [Category("Vaults configuration")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
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
        [DefaultSettingValue(@"SecretKinds.json")]
        [DisplayName("Secret kinds file name")]
        [Description("Relative or absolute path to .json file with secret kinds definitions.\nEnvironment variables are supported and expanded accordingly.")]
        [Browsable(true)]
        [Category("Vaults configuration")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
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
