// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CertificateNotFoundException.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>
//   Indicates the needed certificate for Azure AD authentication is missing
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VaultLibrary
{
    using System;

    [Serializable]
    public class CertificateNotFoundException : Exception
    {
        public CertificateNotFoundException(string message) : base(message)
        {
        }
    }
}
