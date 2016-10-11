// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using System;

namespace Microsoft.Vault.Explorer
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
