using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class SAPHANATCodeInfo : DtoBase
    {
        public string TCODE_Code { get; set; }

        public string TCODE_Desc { get; set; }



        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public int? Version { get; set; }

        public IList<SAPSingleRoleInfo> SAPSingleRoleInfos { get; set; }

        public string Role { get; set; }
        public string Entitlement
        {
            get
            {
                return TCODE_Code;
            }
        }
    }
}
