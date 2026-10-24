using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class Group : DtoBase
    {
        public Group()
        {
            this.Users = new List<User>();
        }
        public string GroupName { get; set; }

        public string GroupDescription { get; set; }

        public string EmailId { get; set; }

        public long? GroupOwner { get; set; }

        public bool IsDefaultGroup { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public List<User> Users { get; set; }

        public User Owner { get; set; }
    }
}
