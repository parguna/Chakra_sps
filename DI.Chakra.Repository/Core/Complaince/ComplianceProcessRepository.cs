using System;
using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;

using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.EntityModel.Presentation;

namespace DI.Chakra.Repository
{
    public class ComplianceProcessRepository : RepositoryBase<ComplianceProcess>
    {
        public ComplianceProcessRepository(DataAccessTransaction transaction) : base(transaction) { }

        public void DeleteComplianceProcess(long id)
        {
            //Group dbGroup = this.CurrentContext.Group.Include(group => group.Users).Where(group => group.Id == groupId).Single();
            //dbGroup.Users.Remove(dbGroup.Users.Find(user => user.Id == userId));
            //CurrentContext.SaveChanges();

            ComplianceProcess complianceProcess = CurrentContext.ComplianceProcess.Where(item => item.Id == id).FirstOrDefault();
            if (complianceProcess != null)
            {
                //Process process = CurrentContext.pro.Where(item => item.ProcessTypeKeyId == id && item.ProcessType == ProcessType.Compliance).FirstOrDefault();
                //if (process != null)
                //{
                //    CurrentContext.Process.Remove(process);
                //}
                CurrentContext.ComplianceProcess.Remove(complianceProcess);
                CurrentContext.SaveChanges();
            }

        }

        public List<PresentationComplianceProcess> GetComplianceProcessJobs(int startRow, int PageSize, string filter) 
        {
            List<PresentationComplianceProcess> jobs = new List<PresentationComplianceProcess>();

            jobs = (from compliance in CurrentContext.ComplianceProcess
                        join definition in CurrentContext.SODPolicyExecutionDefinition
                        on compliance.PolicyExecutionDefinitionId equals definition.Id
                        join policy in CurrentContext.CompliancePolicy
                        on definition.CompliancePolicyId equals policy.Id
                        join pr in CurrentContext.Process
                        on compliance.Id equals pr.ProcessTypeKeyId
                        join s in CurrentContext.Scheduler
                        on pr.Scheduler.Id equals s.Id
                        where pr.ProcessType == ProcessType.Compliance
                        select new PresentationComplianceProcess
                        {
                            ProcessName = compliance.ProcessName,
                            Description = compliance.Description,
                            PolicyName = policy.CompliancePolicyCode,
                            occurence = s.Occurence.Value,
                            ComplianceProcessId = compliance.Id,
                            Status = pr.Status,
                            SchedulerDateTime = s.StartDate + " " + s.StartTime,
                        }).ToList();

            foreach (PresentationComplianceProcess process in jobs)
            {
                process.StartDateTime = Convert.ToDateTime(process.SchedulerDateTime);
            }


            return jobs;
        }
    }
}
