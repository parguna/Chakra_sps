using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAP_PARAMETERINFO")]
    public class SAPParameterInfo:EntityBase,ICreatable,IUpdatable
    {
        public string PARAM_CODE { get; set; }

        public Nullable<int> EndPointSystemId { get; set; }

        public string PARAM_DESC { get; set; }

        public string PARAM_VALUE { get; set; }

        public string TargetSysId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public IList<SAPUserinfo> SAPUserinfos { get; set; }

        public int? Version { get; set; }

    }
}
