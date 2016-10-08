using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Diagnostics;

namespace VaultLibrary
{
    [Cmdlet(VerbsCommon.Get, "VaultAliases")]
    public class GetVaultAliasesCommand : Cmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject("foo", true);
        }
    }
}
