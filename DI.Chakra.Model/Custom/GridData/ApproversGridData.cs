using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class ApproversGridData
    {
        public int ApproverLevelId { get; set; }
        public string ApproverName { get; set; }
        public string ApproverType { get; set; }
        public AccessRequestStatus RequestStatus { get; set; }
        public StatusType Action { get; set; }
        public string Status { get; set; }
        public System.DateTime ActionDate { get; set; }
        public string Comments { get; set; }
        public byte WorkflowState { get; set; }
        public string LevelApprover {
            get {
                return "Level - " + WorkflowState;
            }
        }


    }
}
