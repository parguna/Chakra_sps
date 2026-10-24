using AutoMapper;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationWorkflowApproverGridData
    {
        public long WorkflowConfigurationDetailId { get; set; }

        public byte SequenceNumber { get; set; }

        public ApprovalUserType ApprovalUserType { get; set; }

        public string Approver { get; set; }

        public int ResponseTimeInDays { get; set; }

        public string IsReminderRequired { get; set; }

        public string IsEscalationRequired { get; set; }

        public string AdditionalInfo { get; set; }

        public static PresentationWorkflowApproverGridData ORMap(EntityModel.Presentation.PresentationWorkflowApproverGridData presentationWorkflowApproverGridDataEntityModel)
        {
            if (presentationWorkflowApproverGridDataEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationWorkflowApproverGridData, PresentationWorkflowApproverGridData>(presentationWorkflowApproverGridDataEntityModel);
        }

        public static List<PresentationWorkflowApproverGridData> ORMapList(IList<EntityModel.Presentation.PresentationWorkflowApproverGridData> presentationWorkflowApproverGridDataEntityModels)
        {
            return presentationWorkflowApproverGridDataEntityModels.Select(ORMap).ToList();
        }

        public static IList<PresentationWorkflowApproverGridData> GetApproversList(List<WorkflowConfigurationDetail> workflowConfigurationDetailList)
        {
            IList<PresentationWorkflowApproverGridData> approverList = new List<PresentationWorkflowApproverGridData>();
            PresentationWorkflowApproverGridData approver;

            foreach (var listItem in workflowConfigurationDetailList)
            {
                approver = new PresentationWorkflowApproverGridData();

                approver.WorkflowConfigurationDetailId = listItem.Id;

                approver.ApprovalUserType = listItem.ApprovalUserType; // listItem.ApprovalUserType.ToString();

                if (listItem.ApprovalUserType == ApprovalUserType.User)
                {
                    User user = User.GetById(listItem.Approver);
                    if (user != null)
                        approver.Approver = user.DisplayName;
                }
                else if (listItem.ApprovalUserType == ApprovalUserType.Role || listItem.ApprovalUserType == ApprovalUserType.UseApplicationConfiguration)
                {
                    IList<RoleTypes> roleTypes = RoleTypes.GetByIsActive();
                    approver.Approver = roleTypes.First(roltyp => roltyp.Id == listItem.Approver).DisplayName;
                }
                else if (listItem.ApprovalUserType == ApprovalUserType.Group)
                {
                    Group group = Group.GetById(listItem.Approver);
                    if (group != null)
                        approver.Approver = group.GroupName;
                }
                approver.ResponseTimeInDays = listItem.ResponseTimeInDays;

                if (listItem.IsReminderRequired)
                {
                    approver.IsReminderRequired = string.Format("{0} (Every {1} days)", "Yes", listItem.ReminderIntervalInDays.Value);
                }
                else
                {
                    approver.IsReminderRequired = "No";
                }

                if (listItem.IsEscalationRequired)
                {
                    approver.IsEscalationRequired = string.Format("{0} (After {1} days)", "Yes", listItem.DaysToWaitAfterEscalation.Value);
                }
                else
                {
                    approver.IsEscalationRequired = "No";
                }

                approver.SequenceNumber = listItem.SequenceNumber;

                approver.AdditionalInfo = listItem.IsActionRequired ? "Action Task" : "Approval Task";

                if (listItem.ApprovalUserType == ApprovalUserType.Group)
                {
                    approver.AdditionalInfo += ", " + (listItem.IsMockGroup ? "Pseudo Group" : "Real Group");
                    approver.AdditionalInfo += ", " + (listItem.RequiresApprovalFromAll ? "Requires approval from all persons from this group" : "Requires approval from anyone person from this group");
                }

                if (listItem.ApprovalUserType == ApprovalUserType.UseApplicationConfiguration)
                {
                    approver.AdditionalInfo += ", " + (listItem.RequiresApprovalFromAll ? "Requires approval from all persons from this level" : "Requires approval from anyone person from this level");
                }

                approverList.Add(approver);
            }
            return approverList.OrderBy(item => item.SequenceNumber).ToList();
        }
    }
}
