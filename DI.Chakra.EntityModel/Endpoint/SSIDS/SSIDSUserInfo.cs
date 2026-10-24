using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SSIDS_UserInfo")]
    public class SSIDSUserInfo : EntityBase, ICreatable, IUpdatable
    {
        public SSIDSUserInfo()
        {
            this.SSIDSRolelnfos = new List<SSIDSRolelnfo>();
        }
        public long EndPointSystemId { get; set; }
        public string Emp_Id { get; set; }
        public string User_Id { get; set; }
        public string UserName { get; set; }
        public string Section { get; set; }
        public string Designation { get; set; }
        public string Company { get; set; }
        public string Branch { get; set; }
        public string OrgUnit { get; set; }
        public string Department { get; set; }
        public string Division { get; set; }
        public string OfficeTelePhone { get; set; }
        public string HandPhone { get; set; }
        public string Fax { get; set; }
        public string ImageLink { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? LastLogon { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Encrypt_Pwd { get; set; }
        public DateTime? ValidDate { get; set; }
        public DateTime? InValidDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }
        public IList<SSIDSRolelnfo> SSIDSRolelnfos { get; set; }
    }
}
