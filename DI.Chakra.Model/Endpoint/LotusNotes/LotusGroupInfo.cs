using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Model
{
    [Table("DI_LNS_Groups")]
    public class LotusGroupInfo : IAggregateRoot , IEndpointEntitlementInfo
    {
        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string Owner { get; set; }
        public string Group { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public int? Version { get; set; }
        public bool? CanRequest { get; set; }

        public virtual IList<LotusUserInfo> LotusUserInfos { get; set; }
        public string EntitlementName
        {
            get { return Group; }
        }

        public string EntitlementDesc
        {
            get { return Description; }
        }
        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Group; }
        }
        public LotusGroupInfo()
        {
            LotusUserInfos = new List<LotusUserInfo>();
        }

        public string[] Members;

        public static LotusGroupInfo CreateLotusGroupInfo()
        {
            LotusGroupInfo lotusGroupInfo = new LotusGroupInfo();

            return lotusGroupInfo;
        }

    }
    
}
