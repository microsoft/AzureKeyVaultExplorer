// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

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
    public class MemoryTokenCache : TokenCache
    {
        private static readonly object BufferLock = new object();
        private static byte[] _buffer; 

        /// <summary>
        /// Initializes the cache against an in memory buffer.
        /// </summary>
        public MemoryTokenCache()
        {            
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
            _buffer = null;
        }

        /// <summary>
        /// Triggered right before ADAL needs to access the cache
        /// Reload the cache from the persistent store in case it changed since the last access
        /// </summary>
        /// <param name="args"></param>
        void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (BufferLock)
            {
                this.Deserialize((_buffer != null) ? ProtectedData.Unprotect(_buffer, null, DataProtectionScope.LocalMachine) : null);
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
                lock (BufferLock)
                {
                    // reflect changes in the persistent store
                    _buffer = ProtectedData.Protect(this.Serialize(), null, DataProtectionScope.LocalMachine);
                    // once the write operation took place, restore the HasStateChanged bit to false
                    this.HasStateChanged = false;
                }
            }
        }
    }
}
