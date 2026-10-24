using System;
using System.Collections.Generic;

namespace DI.Chakra.Dto.Presentation
{
    [Serializable]
    public class UserAccessDetailMatrixGridData
    {
        public string UserName { get; set; }
        public string EndpointSystemName { get; set; }
        public string EndpointHostName { get; set; }
        public long EndpointSystemID { get; set; }
        public long? EndpointSystemTypeId { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string AccountType { get; set; }
        public bool IsActive { get; set; }
        public string Type { get; set; } // User, Group, Role, TCode
        public int ParentId { get; set; }
        public DateTime? LastLogonDate { get; set; }
        public string Institution { get; set; }
        public string Description { get; set; }
        public string ApplicationType { get; set; }
        public string Entitlement { get; set; }
        public string EntitlementType { get; set; }
        public long AccountReconcilationId { get; set; }
        public string Company { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Branch { get; set; }
        public string Section { get; set; }
        public string Unit { get; set; }
        public string ReportingOfficer { get; set; }

    }

    public class UserAccessDetailMatrixInput
    {
        public List<long> UserIds { get; set; }
        public UserAccessDetailMatrixInput()
        {
            UserIds = new List<long>();
        }
    }
}
