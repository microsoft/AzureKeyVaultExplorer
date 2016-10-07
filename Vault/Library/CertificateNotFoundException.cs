// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

namespace VaultLibrary
{
    using System;

    /// <summary>
    /// Indicates the needed certificate for Azure AD authentication is missing
    /// </summary>
    [Serializable]
    public class CertificateNotFoundException : Exception
    {
        public CertificateNotFoundException(string message) : base(message)
        {
        }
    }
}
