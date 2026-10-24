using System;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System.Linq;
using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;

namespace DI.Chakra.DomainModel
{
    public class ComplianceProcess : DomainModelBase<ComplianceProcess, EntityModel.ComplianceProcess>
    {
        public string ProcessName { get; set; }

        public string Description { get; set; }

        public long PolicyExecutionDefinitionId { get; set; }

        public bool Default { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }


        public  Process process { get; set; }
        public SODPolicyExecutionDefinition PolicyExecutionDefinition { get; set; }

        public static ComplianceProcess GetByProcessName(string processName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.ComplianceProcess>(dataAccessTransaction).
                    Get(cp => cp.ProcessName == processName).FirstOrDefault());
            }
        }

        public static bool DoesExistsByProcessName(string processName)  
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.ComplianceProcess>(dataAccessTransaction).
                    Any(comprocess => comprocess.ProcessName == processName);
            }
        }

        public static ComplianceProcess GetByIdIncludePolicyExecutionDefinition(long id)  
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.ComplianceProcess>(dataAccessTransaction)
                    .GetById(id,compProcess=>compProcess.PolicyExecutionDefinition, compProcess => compProcess.PolicyExecutionDefinition.CompliancePolicy));
            }
        }

        public new static void Delete(long processTypeKeyId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                ComplianceProcess complianceProcess = ComplianceProcess.GetById(processTypeKeyId);

                Process process = Mapper.Map<EntityModel.Process, Process>(new RepositoryBase<EntityModel.Process>(dataAccessTransaction).
                    Get(m => m.ProcessTypeKeyId == processTypeKeyId && m.ProcessType == ProcessType.Compliance).FirstOrDefault());

                complianceProcess.Delete(dataAccessTransaction);

                process.Delete(dataAccessTransaction);

                dataAccessTransaction.Commit();
            }
        }

        public static ComplianceProcess GetComplianceProcess(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.ComplianceProcess>(dataAccessTransaction)
                    .GetById(id,
                    comp=>comp.PolicyExecutionDefinition,
                    comp=>comp.PolicyExecutionDefinition.CompliancePolicy,
                    comp=>comp.PolicyExecutionDefinition.CompliancePolicy.Conflicts,
                    comp=>comp.PolicyExecutionDefinition.EndpointSelection,
                    comp=>comp.PolicyExecutionDefinition.Exceptions));
            }
        }

        public static ComplianceProcess GetComplianceProcessByPolicyExecutionId(long policyExecutionId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.ComplianceProcess>(dataAccessTransaction)
                    .Get(comp=>comp.PolicyExecutionDefinitionId==policyExecutionId,
                    comp => comp.PolicyExecutionDefinition,
                    comp => comp.PolicyExecutionDefinition.CompliancePolicy,
                    comp => comp.PolicyExecutionDefinition.CompliancePolicy.Conflicts,
                    comp => comp.PolicyExecutionDefinition.EndpointSelection,
                    comp => comp.PolicyExecutionDefinition.Exceptions).FirstOrDefault());
            }
        }
        public override long Save(string currentUser)
        {
            bool isNew = this.IsNew;
            using (var dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                base.Save(dataAccessTransaction);

                if (isNew)
                    this.process.ProcessTypeKeyId = Id;
                this.process.Save(dataAccessTransaction);

                dataAccessTransaction.Commit();
            }
            return 0;
        }

        

        public static void DeleteComplianceProcess(long processId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                ComplianceProcess complianceProcess = ComplianceProcess.GetById(processId);
                complianceProcess.Delete();
                if (complianceProcess != null)
                {
                    Process process = Process.GetByProcessTypeKeyId(processId, ProcessType.Compliance);
                    if (process != null)
                    {
                        process.Delete();
                    }
                    Scheduler scheduler = Scheduler.GetById(processId);
                    if (scheduler != null)
                    {
                        scheduler.Delete();
                    }
                }
                dataAccessTransaction.Commit();
            }
        }
    }
}
