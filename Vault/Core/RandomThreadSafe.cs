// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

namespace Microsoft.Vault.Core
{
    using System;
    using System.Threading;

    public class RandomThreadSafe
    {
        /// <summary>
        /// Random class instance per thread (TLS) and with lazy creation
        /// </summary>
        private static readonly ThreadLocal<Lazy<CryptoRandomGenerator>> _random = new ThreadLocal<Lazy<CryptoRandomGenerator>>(() => new Lazy<CryptoRandomGenerator>(() => new CryptoRandomGenerator()));

        public static CryptoRandomGenerator Instance
        {
            get
            {
                CryptoRandomGenerator result = _random.Value.Value;
                if (null == result)
                {
                    throw new OutOfMemoryException("Could not allocate crypto random number generator.");
                }
                return result;
            }
        }
    }
}
