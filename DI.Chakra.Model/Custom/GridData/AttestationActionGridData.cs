using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.GridData
{
    public class AttestationActionGridData
    {
        public long AttestationReviewId { get; set; }
        public string AttestationUserName { get; set; }
        public string Institution { get; set; }
        public string EndPointType { get; set; }
        public string HostName { get; set; }
        public string ActionId { get; set; }
        public string Comments { get; set; }
        public string ActionTakenBy { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; } 
    }
}
