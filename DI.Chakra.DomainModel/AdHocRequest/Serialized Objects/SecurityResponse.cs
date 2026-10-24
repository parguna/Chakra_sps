
using System;
using System.Collections.Generic;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class SecurityResponse
    {
        public List<SecurityResponseDetail> SecurityResponseDetails { get; set; }
    }

    [Serializable]
    public class SecurityResponseDetail
    {
        public short QuestionID { get; set; }
        public string Question { get; set; }
        public UserResponse Compliance { get; set; }
        public string Comments { get; set; }
    }

    [Serializable]
    public class SecurityResponseGridData
    {
        public SecurityResponseGridData() { Compliance = 0; }
        public short QuestionID { get; set; }
        public string Question { get; set; }
        public Dictionary<byte, string> ComplianceData { get; set; }
        public byte Compliance { get; set; }
        public string Comments { get; set; }
        public string ComplianceResponse { get; set; }
    }
}
