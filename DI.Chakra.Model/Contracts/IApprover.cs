
using DI.Chakra.Infrastructure.Enums;
using System;
namespace DI.Chakra.Model
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
