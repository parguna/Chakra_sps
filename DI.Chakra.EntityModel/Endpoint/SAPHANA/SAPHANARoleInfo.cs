using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.EntityModel.Endpoint.SAP;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAPHANA_ROLEINFO")]
    [Serializable]
    public class SAPHANARoleInfo : EntityBase,ICreatable,IUpdatable
    {
        public long EndPointSystemId { get; set; }

        public string ROLE_CODE { get; set; }

        public string ROLE_DESC { get; set; }

        public string TargetSysId { get; set; }

        public string RoleType { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public List<SAPHANAUserinfo> SAPUserinfo { get; set; }

        public int? Version { get; set; }

        public string UserDefinedDesc { get; set; }
        public bool? CanRequest { get; set; }
       
    }
}
