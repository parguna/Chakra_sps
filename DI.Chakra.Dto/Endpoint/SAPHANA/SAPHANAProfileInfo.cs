using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
     public class SAPHANAProfileInfo:DtoBase
    {
        

        public long EndPointSystemId { get; set; }

        public string PROFILE_CODE { get; set; }

        public string PROFILE_DESC { get; set; }

        public string TargetSysId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public int? Version { get; set; }

        public IList<SAPHANAUserinfo> SAPUserinfos { get; set; }

    }
}
