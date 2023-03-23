// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

namespace Microsoft.Vault.Library
{
    using System.Text.RegularExpressions;

    public static class Consts
    {
        /// <summary>
        ///  Sha1 of zero bytes file
        /// </summary>
        public const string Sha1EmptyFile = "da39a3ee5e6b4b0d3255bfef95601890afd80709";

        /// <summary>
        /// Sha256 of zero bytes file
        /// </summary>
        public const string Sha256EmptyFile = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";

        public const long KB = 1024;

        public const long MB = 1048576;

        public const long GB = 1073741824;

        public const long TB = 1099511627776;

        public const long PB = 1125899906842624;

        public static Regex ValidVaultNameRegex = new Regex("^[0-9a-zA-Z-]{3,24}$", RegexOptions.Singleline | RegexOptions.Compiled);

        public static Regex ValidSecretNameRegex = new Regex("^[0-9a-zA-Z-]{1,127}$", RegexOptions.Singleline | RegexOptions.Compiled);

        public static Regex ValidBase64Regex = new Regex("^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$", RegexOptions.Singleline | RegexOptions.Compiled);

        public static Regex ValidVaultItemHttpsUriRegex = new Regex(@"^https:\/\/(?<VaultName>[0-9a-z-]{3,24}).vault.azure.net(:443)?\/(?<Collection>keys|secrets|certificates)\/(?<Name>[0-9a-z-]{1,127})(\/(?<Version>[0-9a-f]{32}))?$", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static Regex ValidVaultItemVaultUriRegex = new Regex(@"^vault:(\/?\/?(?<VaultName>[0-9a-z-]{3,24})\/((?<Collection>keys|secrets|certificates)\/(?<Name>[0-9a-z-]{1,127})(\/(?<Version>[0-9a-f]{32}))?)?)?$", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public const int MaxSecretValueLength = 25 * 1024; // 25 KB

        public const int MaxNumberOfTags = 15;

        public const int MaxTagNameLength = 256;

        public const int MaxTagValueLength = 256;

        public const string Md5Key = "Md5";

        public const string ChangedByKey = "ChangedBy";

        public const string SecretKindKey = "SecretKind";

        internal const string VaultsJsonConfig = "Vaults.json";

        internal const string AzureVaultUriFormat = "https://{0}.vault.azure.net";

        internal const string SecretsEndpoint = "secrets";

        internal const string CertificatesEndpoint = "certificates";

        internal const int ListSecretsMaxResults = 25;

        internal const int GetSecretVersionsMaxResults = 25;

        internal const int ListCertificatesMaxResults = 25;

        internal const int GetCertificateVersionsMaxResults = 25;

        internal const string VaultTokenCacheDirectory = @"%AppData%\Microsoft\Vault";

        public const string VaultTokenCacheFileName = VaultTokenCacheDirectory + @"\TokenCache_{0}_cc73e24a-63f3-4d9f-878c-53b85fecd872.dat"; // {0} - DomainHint
    }
}
