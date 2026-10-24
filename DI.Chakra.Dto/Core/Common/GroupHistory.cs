using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DI.Chakra.Dto
{
    public class GroupHistory :DtoBase
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public string GroupType { get; set; }

        public string GroupOwner { get; set; }

        public string GroupDescription { get; set; }

        public bool IsDefaultGroup { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
