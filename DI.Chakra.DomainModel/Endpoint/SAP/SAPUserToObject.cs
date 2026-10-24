using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPUserToObject : DomainModelBase<SAPUserToObject, EntityModel.SAPUserToObject>
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
