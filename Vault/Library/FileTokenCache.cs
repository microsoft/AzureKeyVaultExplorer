// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using Microsoft.Identity.Client;
using System;
using System.IO;
using System.Linq;

namespace Microsoft.Vault.Library
{
    public class CachePersistence
    {
        public static string FileName = Environment.ExpandEnvironmentVariables(string.Format(Consts.VaultTokenCacheFileName, "microsoft.com"));
        private static readonly TokenCache UsertokenCache = new TokenCache();
        private static readonly object FileLock = new object();

        public CachePersistence() : this("microsoft.com") { }

        /// <summary>
        /// Initializes the cache against a local file.
        /// If the file is already present, it loads its content in the ADAL cache
        /// </summary>
        /// <param name="domainHint">For example: microsoft.com or gme.gbl</param>
        public CachePersistence(string domainHint)
        {
            FileName = Environment.ExpandEnvironmentVariables(string.Format(Consts.VaultTokenCacheFileName, domainHint));
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));
        }

        public static TokenCache GetUserCache()
        {
            lock (FileLock)
            {
                UsertokenCache.SetBeforeAccess(BeforeAccessNotification);
                UsertokenCache.SetAfterAccess(AfterAccessNotification);
                return UsertokenCache;
            }
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
            foreach (string token in tokenNames)
            {
                new CachePersistence(token).Clear();
            }
        }

        /// <summary>
        /// Empties the persistent store
        /// </summary>
        public void Clear()
        {
            File.Delete(FileName);
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
            UsertokenCache.SetBeforeAccess(BeforeAccessNotification);
            UsertokenCache.SetAfterAccess(AfterAccessNotification);
            BeforeAccessNotification(null);
        }

        /// <summary>
        /// Triggered right before MSAL needs to access the cache
        /// Reload the cache from the persistent store in case it changed since the last access
        /// </summary>
        /// <param name="args"></param>
        public static void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (FileLock)
            {
                args.TokenCache.DeserializeMsalV3(File.Exists(FileName)
                    ? File.ReadAllBytes(FileName)
                    : null);
            }
        }

        /// <summary>
        /// Triggered right after MSAL accessed the cache
        /// </summary>
        /// <param name="args"></param>
        public static void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (args.HasStateChanged)
            {
                lock (FileLock)
                {
                    // reflect changes in the persistent store
                    File.WriteAllBytes(FileName, args.TokenCache.SerializeMsalV3());
                }
            }
        }
    }
}
