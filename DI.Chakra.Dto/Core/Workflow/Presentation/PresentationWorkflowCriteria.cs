using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class PresentationWorkFlowCriteria
    {
        public AccessType AccessType { get; set; }
        public string UserName { get; set; }
        public long TaskId { get; set; }
        public string Comments { get; set; }
        public string AccountCreatedNotificationTO { get; set; }
        public string ConflictRemarks { get; set; }
    }
}
