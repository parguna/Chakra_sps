using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAP_EamNovaUserInfo")]
   public  class SAPEamNovaUserInfo : EntityBase, ICreatable, IUpdatable
    {
        public long EndPointSystemId { get; set; }
        public long UserId { get; set; }
        public string FLINS_UserLogonName { get; set; }
        public long FLINS_SAPUserInfoId { get; set; }
        public string FLINS_BI_UserLogonName { get; set; }
        public long FLINS_BI_SAPUserInfoId { get; set; }
        public string AD_UserLogonName { get; set; }
        public long ADS_UserInfoId { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }        
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }

        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }
        public List<SAPEamNovaUserRoleInfo> SAPEamNovaUserRoles { get; set; }


    }
}
