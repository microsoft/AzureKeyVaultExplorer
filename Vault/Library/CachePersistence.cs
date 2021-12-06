// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using System;
using System.IO;
using System.Linq;
using Azure.Identity;

namespace Microsoft.Vault.Library
{
    public class CachePersistence
    {
        public string FileName;
        private static readonly object FileLock = new object();
        const string TOKEN_CACHE_NAME = "MyTokenCache";

        public CachePersistence(string domainHint)
        {
            FileName = Environment.ExpandEnvironmentVariables(string.Format(Consts.VaultTokenCacheFileName, domainHint));
        }
        /// <summary>
        /// Initializes the cache against a local file.
        /// If the file is already present, it loads its content in the MSAL cache
        /// </summary>
        /// <param name="domainHint">For example: microsoft.com or gme.gbl</param>
        public CachePersistence(string domainHint, AuthenticationRecord authRecord)
        {
            FileName = Environment.ExpandEnvironmentVariables(string.Format(Consts.VaultTokenCacheFileName, domainHint));
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));
            AfterAccessNotification(authRecord);
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
        public void Rename(string newName, AuthenticationRecord authRecord)
        {
            newName = Environment.ExpandEnvironmentVariables(string.Format(Consts.VaultTokenCacheFileName, newName));
            if (File.Exists(newName))
            {
                File.Delete(newName);
            }
            File.Move(FileName, newName);

            FileName = newName;
            this.BeforeAccessNotification(authRecord);
            this.AfterAccessNotification(authRecord);
            BeforeAccessNotification(null);
        }

        /// <summary>
        /// Triggered right before MSAL needs to access the cache
        /// Reload the cache from the persistent store in case it changed since the last access
        /// </summary>
        void BeforeAccessNotification(AuthenticationRecord authRecord)
        {
            lock (FileLock)
            {
                // Load the previously serialized AuthenticationRecord from disk and deserialize it.
                var authRecordStream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                authRecord = AuthenticationRecord.Deserialize(authRecordStream);

                // Construct a new client with our TokenCachePersistenceOptions with the addition of the AuthenticationRecord property.
                // This tells the credential to use the same token cache in addition to which account to try and fetch from cache when GetToken is called.
                var credential = new InteractiveBrowserCredential(
                    new InteractiveBrowserCredentialOptions
                    {
                        TokenCachePersistenceOptions = new TokenCachePersistenceOptions { Name = TOKEN_CACHE_NAME },
                        AuthenticationRecord = authRecord
                    });
            }
        }

        /// <summary>
        /// Triggered right after MSAL accessed the cache
        /// </summary>
        void AfterAccessNotification(AuthenticationRecord authRecord)
        {
            lock (FileLock)
            {
                // Construct a credential with TokenCachePersistenceOptions specified to ensure that the token cache is persisted to disk.
                // We can also optionally specify a name for the cache to avoid having it cleared by other applications.
                var credential = new InteractiveBrowserCredential(
                    new InteractiveBrowserCredentialOptions { TokenCachePersistenceOptions = new TokenCachePersistenceOptions { Name = TOKEN_CACHE_NAME } });

                // Call AuthenticateAsync to fetch a new AuthenticationRecord.
                authRecord = credential.Authenticate();

                // Serialize the AuthenticationRecord to disk so that it can be re-used across executions of this initialization code.
                var authRecordStream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
                authRecord.SerializeAsync(authRecordStream);
            }
        }
    }
}
