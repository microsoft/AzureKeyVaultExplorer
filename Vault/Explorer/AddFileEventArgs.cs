using System;

namespace VaultExplorer
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
