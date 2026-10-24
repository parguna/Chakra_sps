using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure.Enums
{
    public enum NotificationStatus : byte
    {
        [Description("All")]
        All = 1,

        [Description("UnRead")]
        UnRead,

        [Description("Read")]
        Read,

        [Description("Replied")]
        Replied
    }
}
