using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAPHANA_USEROBJECTMAPPING")]
    public class SAPHANAUserToObject : EntityBase,ICreatable,IUpdatable
    {
        public string UserLogonName { get; set; }

        public string RoleCode { get; set; }

        public string ProfileCode { get; set; }

        public string AuthorizationCode { get; set; }

        public string ObjectCode { get; set; }

        public string FieldCode { get; set; }

        public string FromValue { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }


    }
}
