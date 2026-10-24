using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class SAPHANAGroupInfo : DtoBase
    {
        

        public long EndPointSystemId { get; set; }

        public string GROUP_CODE { get; set; }

        public string GROUP_DESC { get; set; }

        public Nullable<int> TargetSysId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public int? Version { get; set; }

        public IList<SAPHANAUserinfo> SAPUserinfos { get; set; }

        public string EntitlementName
        {
            get { return GROUP_CODE; }
        }

        public string EntitlementDesc
        {
            get { return GROUP_DESC; }
        }
        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Group; }
        }
        //public SAPGroupInfo()
        //{
        //    SAPUserinfos = new List<SAPUserinfo>();
        //}
    }
}
