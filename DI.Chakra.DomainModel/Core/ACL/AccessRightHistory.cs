using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class AccessRightHistory : DomainModelBase<AccessRightHistory, EntityModel.AccessRightHistory>
    {
        public int AccessRightsId { get; set; }
        public Nullable<long> RoleId { get; set; }
        public Nullable<int> TabMenuId { get; set; }
        public string UserAction { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
