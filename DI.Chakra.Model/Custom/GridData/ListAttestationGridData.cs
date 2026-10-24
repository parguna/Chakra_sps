using System;
using System.Collections.Generic;
using DI.Chakra.Infrastructure.Enums;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class ListAttestationGridData
    {
        public long AttestationId { get; set; }
        public string AttestationName { get; set; }
        public string AttestationDescription { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> RunDate { get; set; }
        public string  AttesationCreatedBy { get; set; }
        public string AttestationType { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int ResponseTimeInDays { get; set; }
        public Occurence occurence { get; set; }
        public string EndPointSystem { get; set; }
        
    }
}
