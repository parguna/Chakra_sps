using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class WorkflowConfigurationDetail : DomainModelBase<WorkflowConfigurationDetail, EntityModel.WorkflowConfigurationDetail> 
    {
        public long WorkflowConfigurationId { get; set; }

        public byte SequenceNumber { get; set; }

        public ApprovalUserType ApprovalUserType { get; set; }

        public int Approver { get; set; }

        public int ResponseTimeInDays { get; set; }

        public bool IsActionRequired { get; set; }

        public bool RequiresApprovalFromAll { get; set; }

        public bool IsMockGroup { get; set; }

        public bool IsReminderRequired { get; set; }

        public Nullable<int> ReminderIntervalInDays { get; set; }

        public bool IsEscalationRequired { get; set; }

        public Nullable<int> DaysToWaitAfterEscalation { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public WorkflowConfiguration WorkflowConfigurations { get; set; }

        public static List<WorkflowConfigurationDetail> GetByWorkflowConfigurationId(long workflowConfigurationId)   
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.WorkflowConfigurationDetail>(dataAccessTransaction)
                      .Get(item => item.WorkflowConfigurationId == workflowConfigurationId).OrderBy(item => item.SequenceNumber).ToList());
            }
        }

        public static long GetDaysToComplete(long workflowConfigurationId)
        {
            var configurationDetails = GetByWorkflowConfigurationId(workflowConfigurationId).ToList();
            var days = configurationDetails.Sum(a => a.ResponseTimeInDays) + configurationDetails.Where(a => a.IsEscalationRequired).Sum(a => (int)a.DaysToWaitAfterEscalation);
            return days;
        }

        public static bool IsApprover(long UserId)
        {
            //using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var directUser = WorkflowConfigurationDetail.GetAll(true).Where(a => a.Approver == UserId && a.ApprovalUserType == ApprovalUserType.User).Any();

                var viaGroup = (from approverGroup in WorkflowConfigurationDetail.GetAll()
                                join grp in Group.GetAll() on approverGroup.Approver equals grp.Id
                                where approverGroup.ApprovalUserType == ApprovalUserType.Group && grp.Users.Select(a => a.Id).Contains(UserId)
                                select grp).Any();


                return directUser || viaGroup;
            }
        }
    }
}
