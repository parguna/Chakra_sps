using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_NPAC_LevelInfo")]
    public class NPACLevelInfo : EntityBase, ICreatable, IUpdatable
    {
        public NPACLevelInfo()
        {
            this.NPACUserInfos = new List<NPACUserInfo>();
        }
        public long EndPointSystemId { get; set; }
        public Nullable<int> Level { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public IList<NPACUserInfo> NPACUserInfos { get; set; }

        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }

    }
}
