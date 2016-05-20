using Microsoft.Azure.KeyVault;
using System;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public class AddSecretEventArgs : EventArgs
    {
        public readonly Secret Secret;

        public AddSecretEventArgs(Secret secret)
        {
            Secret = secret;
        }
    }
}

