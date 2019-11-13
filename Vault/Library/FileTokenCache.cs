// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Vault.Library
{
    public class FileTokenCache : TokenCache
    {
        public string FileName;
        private static readonly object FileLock = new object();

        public FileTokenCache() : this("microsoft.com") { }

        /// <summary>
        /// Initializes the cache against a local file.
        /// If the file is already present, it loads its content in the ADAL cache
        /// </summary>
        /// <param name="domainHint">For example: microsoft.com or gme.gbl</param>
        public FileTokenCache(string domainHint)
        {
            FileName = Environment.ExpandEnvironmentVariables(string.Format(Consts.VaultTokenCacheFileName, domainHint));
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));
            this.AfterAccess = AfterAccessNotification;
            this.BeforeAccess = BeforeAccessNotification;
            BeforeAccessNotification(null);
        }

        /// <summary>
        /// Gets all login names for which there is a token cached locally.
        /// </summary>
        public static string[] GetAllFileTokenCacheLoginNames()
        {
            string[] paths = Directory.GetFiles(Environment.ExpandEnvironmentVariables(Consts.VaultTokenCacheDirectory));
            for (int i = 0; i < paths.Length; i++)
            {
                //Gets filename from path.
                paths[i] = paths[i].Split('\\').Last();

                //Gets login name from filename.
                paths[i] = paths[i].Split('_')[1];
            }
            return paths;
        }

        /// <summary>
        /// Empties all persistent stores.
        /// </summary>
        public static void ClearAllFileTokenCaches()
        {
            string[] tokenNames = GetAllFileTokenCacheLoginNames();
            foreach(string token in tokenNames)
            {
                new FileTokenCache(token).Clear();
            }
        }

        /// <summary>
        /// Renames the cache.
        /// </summary>
        /// <param name="newName"></param>
        public void Rename(string newName)
        {
            newName = Environment.ExpandEnvironmentVariables(string.Format(Consts.VaultTokenCacheFileName, newName));
            if (File.Exists(newName))
            {
                File.Delete(newName);
            }
            File.Move(FileName, newName);

            FileName = newName;
            this.AfterAccess = AfterAccessNotification;
            this.BeforeAccess = BeforeAccessNotification;
            BeforeAccessNotification(null);
        }


        /// <summary>
        /// Empties the persistent store
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            File.Delete(FileName);
        }

        /// <summary>
        /// Triggered right before ADAL needs to access the cache
        /// Reload the cache from the persistent store in case it changed since the last access
        /// </summary>
        /// <param name="args"></param>
        void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (FileLock)
            {
                this.Deserialize(File.Exists(FileName) ? ProtectedData.Unprotect(File.ReadAllBytes(FileName), null, DataProtectionScope.CurrentUser) : null);
            }
        }

        /// <summary>
        /// Triggered right after ADAL accessed the cache
        /// </summary>
        /// <param name="args"></param>
        void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (this.HasStateChanged)
            {
                lock (FileLock)
                {
                    // reflect changes in the persistent store
                    File.WriteAllBytes(FileName, ProtectedData.Protect(this.Serialize(), null, DataProtectionScope.CurrentUser));
                    // once the write operation took place, restore the HasStateChanged bit to false
                    this.HasStateChanged = false;
                }
            }
        }
    }
}
