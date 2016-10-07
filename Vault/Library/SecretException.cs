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
