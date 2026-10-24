using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.SAP
{
    [Table("DI_SAP_USEROBJECTMAPPING")]
    public class SAPUserToObject
    {
        [Key]
        public long Id { get; set; }
        public string UserLogonName { get; set; }
        public string RoleCode { get; set; }
        public string ProfileCode { get; set; }
        public string AuthorizationCode { get; set; }
        public string ObjectCode { get; set; }
        public string FieldCode { get; set; }
        public string FromValue { get; set; }
    }
}
