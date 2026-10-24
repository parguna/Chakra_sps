using System;

namespace DI.Chakra.Model
{
    public class FlowConfigurationGridData
    {
        public int ConfigurationId { get; set; }
        public string EmployeeEvent { get; set; }
        public bool IsAdminCheckRequired { get; set; }
        public bool IsApprovalRequired { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
