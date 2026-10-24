using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SMS_UserInfo")]
    public class SMSUserInfo : EntityBase, ICreatable, IUpdatable
    {
        public SMSUserInfo()
        {
            this.SMSAccessLevels = new List<SMSAccessLevel>();        
        }
        public long EndPointSystemId { get; set; }

        #region SMS Data
        public int UserId { get; set; }
        //first name
        public string FIRSTNAME { get; set; }
        //last name
        public string LASTNAME { get; set; }
        //known as
        public string MIDNAME { get; set; }
        //PERNR
        public string SSNO { get; set; }

        //department/branch
        public string Addr1 { get; set; }
        //section
        public string City { get; set; }
        //nric
        public string Zip { get; set; }
        //AD ID
        public string OPHONE { get; set; }
        //email id
        public string EMAIL { get; set; }

        //lenel company id
        public int DEPT { get; set; }

        public string Extension { get; set; }

        #endregion

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public List<SMSAccessLevel> SMSAccessLevels { get; set; }    

        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }
    }
}
