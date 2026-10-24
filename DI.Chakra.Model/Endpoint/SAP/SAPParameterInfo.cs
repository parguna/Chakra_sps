using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Model.SAP
{
    [Table("DI_SAP_PARAMETERINFO")]
    public class SAPParameterInfo : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string PARAM_CODE { get; set; }
        public string TenantId { get; set; }
        public Nullable<int> EndPointSystemId { get; set; }
        public string PARAM_DESC { get; set; }
        public string PARAM_VALUE { get; set; }
        public string TargetSysId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public virtual IList<SAPUserinfo> SAPUserinfos { get; set; }
        public int? Version { get; set; }


        public SAPParameterInfo()
        {
            SAPUserinfos = new List<SAPUserinfo>();
        }
        public static SAPParameterInfo CreateSAPParameterInfo()
        {
            SAPParameterInfo SAPParameterInfo = new SAPParameterInfo();

            return SAPParameterInfo;
        }
    }
}

