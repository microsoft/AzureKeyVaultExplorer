using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultLibrary
{
    using System;

    public class VaultAccessException : AggregateException
    {
        public VaultAccessException(string message, params Exception[] innerExceptions) : base(message, innerExceptions)
        {
        }
    }
}
