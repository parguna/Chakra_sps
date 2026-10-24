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
    [Table("DI_SAP_ObjectValues")]
    public class SAPObjectValues : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }

        public string SapObjectType { get; set; }
        public string SapObjectCode { get; set; }
        public String SapOkFlag { get; set; }
        public string ModificationID { get; set; }
        public string OrgName { get; set; }

        public long EndPointSystemId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string TenantId { get; set; }
        public int? Version { get; set; }


    }
}
