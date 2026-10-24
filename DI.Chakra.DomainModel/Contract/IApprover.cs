using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public interface IApprover
    {
        string BusinessRole { get; }
        string SystemName { get; }
        string Level1Approver { get; set; }
        ApprovalUserType Level1ApproverType { get; set; }
        string Level2Approver { get; set; }
        ApprovalUserType Level2ApproverType { get; set; }
        string Level3Approver { get; set; }
        ApprovalUserType Level3ApproverType { get; set; }
    }
}
