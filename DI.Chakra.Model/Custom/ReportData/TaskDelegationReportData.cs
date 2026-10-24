using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.ReportData
{
    public class TaskDelegationReportData
    {
        public long DelegationId { get; set; }
        public string ActionUserName { get; set; }
        public string DelegatedFromUserName { get; set; }
        public string DelegatedToUserName { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string DelegateAttestationTasks { get; set; }
        public string DelegateProvisioningTasks { get; set; }
        public bool Active { get; set; }
        public string DelegationCreatedBy { get; set; }
        public long AccessRequestId { get; set; }
        public DateTime DelegationDate { get; set; }
        public string DelegationComments { get; set; }
    }
}
