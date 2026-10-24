using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SODAuthorizationReport 
    {
        public int SODAuthorizationReportID { get; set; }

        public long SODReportId { get; set; }

        public string AuthorizationCode { get; set; }

        public int LeftOrRight { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
