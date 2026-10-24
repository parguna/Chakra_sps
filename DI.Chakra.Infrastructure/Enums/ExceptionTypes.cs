using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure.Enums
{
    public enum ExceptionTypes : byte
    {
        None,
        Added,
        Removed,
        Attribute
    }

    public enum ReconcilationType : byte
    {
        Account,
        Entitlement,
        All

    }

    public enum ResolvedStatus : byte
    {
        None = 0,
        Manual = 1,
        Automatic
       
    }
}
