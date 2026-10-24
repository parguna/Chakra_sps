using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model.SAP
{
    [Table("DI_SAP_PROFILEINFO")]
    public class SAPProfileInfo : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string PROFILE_CODE { get; set; }
        public string PROFILE_DESC { get; set; }
        public string TargetSysId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? Version { get; set; }

        public virtual IList<SAPUserinfo> SAPUserinfos { get; set; }

        public SAPProfileInfo()
        {
            SAPUserinfos = new List<SAPUserinfo>();
        }

        public static SAPProfileInfo CreateSAPProfileInfo()
        {
            SAPProfileInfo SAPProfileInfo = new SAPProfileInfo();

            return SAPProfileInfo;
        }
    }
}
