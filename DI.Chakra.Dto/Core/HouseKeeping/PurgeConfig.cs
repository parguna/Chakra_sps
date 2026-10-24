using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class PurgeConfig:DtoBase
    {
        public string TableName { get; set; }

        public string ObjectName { get; set; }

        public string Description { get; set; }

        public int DaysToKeep { get; set; }

        public string Status { get; set; }

        public PurgeMode PurgeMode { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public Nullable<System.DateTime> LastRunOn { get; set; }
    }
}
