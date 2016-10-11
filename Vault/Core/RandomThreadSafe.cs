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
        private static readonly ThreadLocal<Lazy<Random>> _random = new ThreadLocal<Lazy<Random>>(() => new Lazy<Random>(() => new Random(Guid.NewGuid().GetHashCode())));

        public static Random Instance
        {
            get
            {
                Random result = _random.Value.Value;
                if (null == result)
                {
                    throw new OutOfMemoryException("Could not allocate random number generator.");
                }
                return result;
            }
        }
    }
}
