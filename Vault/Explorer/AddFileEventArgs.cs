using System;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public class AddFileEventArgs : EventArgs
    {
        public readonly string FileName;

        public AddFileEventArgs(string filename)
        {
            FileName = filename;
        }
    }
}
