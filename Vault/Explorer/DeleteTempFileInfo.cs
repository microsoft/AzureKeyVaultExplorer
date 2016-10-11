// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Vault.Explorer
{
    /// <summary>
    /// Deletes file if file location is under %TEMP% folder, to be used with using pattern
    /// </summary>
    public class DeleteTempFileInfo : IDisposable
    {
        public FileInfo FileInfoObject { get; set; }

        public void Dispose()
        {
            if (FileInfoObject != null)
            {
                if (FileInfoObject.DirectoryName.StartsWith(Path.GetTempPath().TrimEnd('\\'), StringComparison.CurrentCultureIgnoreCase))
                {
                    FileInfoObject.Delete();
                }
                FileInfoObject = null;
            }
        }
    }
}
