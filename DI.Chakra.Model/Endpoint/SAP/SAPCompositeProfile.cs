using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.SAP
{
    [Table("DI_SAP_COMPOSITEPROFILEINFO")]
    public class SAPCompositeProfileInfo: IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string PROFILE_CODE { get; set; }
        public string PROFILE_DESC { get; set; }
        public string SUBProfile_Code { get; set; }
        public string TargetSysId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? Version { get; set; }

         

        public SAPCompositeProfileInfo()
        {
            
        }

        public static SAPCompositeProfileInfo SAPCompositeProfile()
        {
            SAPCompositeProfileInfo SAPProfileInfo = new SAPCompositeProfileInfo();

            return SAPProfileInfo;
        }
    
    }
}
