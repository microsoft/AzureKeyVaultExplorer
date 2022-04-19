// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Azure.Identity;

namespace Microsoft.Vault.Library
{
    public class CachePersistence
    {
        public string FileName;
        private static readonly object FileLock = new object();
        private static readonly RNGCryptoServiceProvider RNGCryp = new RNGCryptoServiceProvider();
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

                var chainedTokenCredential = new ChainedTokenCredential(credential, new DefaultAzureCredential());
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

                // Create the original data to be encrypted
                var toEncrypt = Encoding.UTF8.GetBytes(authRecord.ToString());

                // Create some random entropy.
                byte[] entropy = new byte[16];

                // Fill the array with a random value.
                RNGCryp.GetBytes(entropy);

                // Serialize the AuthenticationRecord to disk so that it can be re-used across executions of this initialization code.
                var authRecordStream = new FileStream(FileName, FileMode.OpenOrCreate);

                // Encrypt a copy of the data to the stream.
                int bytesWritten = EncryptDataToStream(toEncrypt, entropy, DataProtectionScope.CurrentUser, authRecordStream);

                authRecordStream.Close();
            }
        }

        public static int EncryptDataToStream(byte[] Buffer, byte[] Entropy, DataProtectionScope Scope, Stream S)
        {
            if (Buffer == null)
                throw new ArgumentNullException("Buffer");
            if (Buffer.Length <= 0)
                throw new ArgumentException("Buffer");
            if (Entropy == null)
                throw new ArgumentNullException("Entropy");
            if (Entropy.Length <= 0)
                throw new ArgumentException("Entropy");
            if (S == null)
                throw new ArgumentNullException("S");

            int length = 0;

            // Encrypt the data and store the result in a new byte array. The original data remains unchanged.
            byte[] encryptedData = ProtectedData.Protect(Buffer, Entropy, Scope);

            // Write the encrypted data to a stream.
            if (S.CanWrite && encryptedData != null)
            {
                S.Write(encryptedData, 0, encryptedData.Length);

                length = encryptedData.Length;
            }

            // Return the length that was written to the stream.
            return length;
        }

        public static byte[] DecryptDataFromStream(byte[] Entropy, DataProtectionScope Scope, Stream S, int Length)
        {
            if (S == null)
                throw new ArgumentNullException("S");
            if (Length <= 0)
                throw new ArgumentException("Length");
            if (Entropy == null)
                throw new ArgumentNullException("Entropy");
            if (Entropy.Length <= 0)
                throw new ArgumentException("Entropy");

            byte[] inBuffer = new byte[Length];
            byte[] outBuffer;

            // Read the encrypted data from a stream.
            if (S.CanRead)
            {
                S.Read(inBuffer, 0, Length);

                outBuffer = ProtectedData.Unprotect(inBuffer, Entropy, Scope);
            }
            else
            {
                throw new IOException("Could not read the stream.");
            }

            // Return the length that was written to the stream.
            return outBuffer;
        }
    }
}
