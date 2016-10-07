// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

namespace VaultLibrary
{
    using System;

    public class SecretException : AggregateException
    {
        public SecretException(string message, params Exception[] innerExceptions) : base(message, innerExceptions)
        {
        }
    }
}
