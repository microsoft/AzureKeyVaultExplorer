// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

namespace Microsoft.Vault.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Assembly resolver used in Powershell modules to resolve assmblies locations
    /// </summary>
    public static class AssemblyResolver
    {
        public static void Register(string rootDir, IDictionary<string, string> assemblyResolveMap)
        {
            Guard.ArgumentNotNullOrWhitespace(rootDir, nameof(rootDir));
            Guard.ArgumentNotNull(assemblyResolveMap, nameof(assemblyResolveMap));

            // Assembly resolver
            AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
            {
                var an = new AssemblyName(e.Name).Name;

                string fileName;

                if (assemblyResolveMap.TryGetValue(an, out fileName))
                {
                    // Obtain the (full) file path
                    fileName = Directory.EnumerateFiles(rootDir, fileName,
                        SearchOption.AllDirectories).First();

                    return Assembly.LoadFrom(fileName);
                }

                return null;
            };
        }
    }
}
