using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeepIdentity.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeepIdentity.Model.SAP
{
    [Table("DI_SAP_USER_ROLETCODE_MAP_INFO")]
    public class SAPUserRoleTCodeMapInfo : IAggregateRoot
    {
        [Key]
        public int SAP_USER_ROLETCode_MAPID { get; set; }

        public string TenantId { get; set; }
        public int EndPointSystemId { get; set; }
        public string Role_Code { get; set; }
        public string TCode { get; set; }
        public string TCode_Desc { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
        
        public override string ToString()
        {
            //return this.name;
            return "";
        }




        public static SAPUserRoleTCodeMapInfo CreateSAPUserRoleTCodeMap()
        {
            SAPUserRoleTCodeMapInfo SAPUserRoleTCodeMap = new SAPUserRoleTCodeMapInfo();

            return SAPUserRoleTCodeMap;
        }
    }
}
