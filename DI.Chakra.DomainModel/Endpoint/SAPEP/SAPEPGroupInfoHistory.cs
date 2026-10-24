using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPEPGroupInfoHistory : DomainModelBase<SAPEPGroupInfoHistory, EntityModel.SAPEPGroupInfoHistory>
    {
        public int HistoryId { get; set; }

        

        public Nullable<int> EndPointSystemId { get; set; }

        public string GroupId { get; set; }

        public string GroupName { get; set; }

        public string GroupDescription { get; set; }

        public string Extar1 { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public int? Version { get; set; }
    }
}
