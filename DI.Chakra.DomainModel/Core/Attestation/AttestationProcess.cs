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
    /// <summary>
    /// This class is used for setting up Attestation jobs. Its covers the management(CRUD) of Attestation Job
    /// </summary>
    public class AttestationProcess : DomainModelBase<AttestationProcess, EntityModel.AttestationProcess>
    {
        public string ProcessName { get; set; }

        public string Description { get; set; }

        public Nullable<long> AttestationId { get; set; }

        public Nullable<System.DateTime> ProcessStartAt { get; set; }

        public Nullable<System.DateTime> ProcessEndAt { get; set; }

        public string WorkflowStatus { get; set; }

        public Nullable<int> HistoryId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public Attestation Attestation { get; set; }

        public Process process { get; set; }


        public static AttestationProcess GetByAttestationId(long attestationId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AttestationProcess>(dataAccessTransaction)
                    .Get(attpro => attpro.AttestationId == attestationId).FirstOrDefault());
            }
        }

        public static AttestationProcess GetByIdIncludeAttestation(long id)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AttestationProcess>(dataAccessTransaction)
                                    .GetById(id, attprs => attprs.Attestation));
            }
                
        }

        public static AttestationProcess GetByIdIncludeProcess(long id)
        {
            AttestationProcess attProcess;
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                attProcess = ORMap(new RepositoryBase<EntityModel.AttestationProcess>(dataAccessTransaction)
                    .GetById(id, attprs => attprs.Attestation));
                if (attProcess != null)
                {
                    Process process = Process.GetByProcessTypeKeyId(id, Infrastructure.Enums.ProcessType.Attestation);
                    attProcess.process = process;
                }
                return attProcess;
            }
                
        }

        //public override void Delete()
        //{
        //    using (DataAccessTransaction tranaction = new DataAccessTransaction())
        //    {
        //        Process process = Process.GetByProcessType(this.Id, Infrastructure.Enums.ProcessType.Attestation);
        //        if(process != null)
        //            process.Delete();
        //        Delete(this.Id);
        //        tranaction.Commit();
        //    }
        //}

        public static bool DeleteAttestationProcess(long processId)
        {
            bool canThrowError = false;
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                try
                {
                    AttestationProcess attProcess = AttestationProcess.GetById(processId);
                    attProcess.Delete(dataAccessTransaction);
                    if (attProcess != null)
                    {
                        Process process = Process.GetByProcessTypeKeyId(processId, Infrastructure.Enums.ProcessType.Attestation);
                        if (process != null)
                        {
                            process.Delete(dataAccessTransaction);
                        }
                        Scheduler scheduler = Scheduler.GetById(process.Scheduler_SchedulerId);
                        if (scheduler != null)
                        {
                            scheduler.Delete(dataAccessTransaction);
                        }
                    }
                    dataAccessTransaction.Commit();
                }
                catch (System.Exception ex)
                {
                    canThrowError = true;
                }
            }
            return canThrowError;
        }

        public static bool DoesExists(string processName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.AttestationProcess>(dataAccessTransaction).Any(attPrs => attPrs.ProcessName.Equals(processName, StringComparison.CurrentCultureIgnoreCase));
            }
                
        }

        public static bool DoesExistsByWorkflowStatus(string processName, string workflowStatus)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.AttestationProcess>(dataAccessTransaction).Any(attPrs => attPrs.ProcessName == processName && attPrs.WorkflowStatus.Equals(workflowStatus, StringComparison.CurrentCultureIgnoreCase));
            }
                
        }

        public override long Save(string currentUser)
        {
            using (var trans = new DataAccessTransaction(currentUser))
            {
                if (this.Id == 0)
                {
                    Process p = this.process;
                    this.process = null;

                    base.Save(trans);

                    p.ProcessTypeKeyId = this.Id;
                    p.Save(trans);
                }
                else
                {

                    new AttestationProcessRepository(trans).Update(ORUnMap());
                    this.process.Save(trans);

                    if (this.process.Scheduler != null)
                        this.process.Scheduler.Save(trans);
                }
                trans.Commit();
            }
            return 0;
        }

        public static List<AttestationProcess> GetAttestationProcessByEndpointType(EndpointType endPointType)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new AttestationProcessRepository(dataAccessTransaction).GetAttestationProcessByEndpointType(endPointType).ToList());
            }
        }
        public static bool IsCampaignRunningForAttestation(long attestationId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new AttestationProcessRepository(dataAccessTransaction).IsCampaignRunningForAttestation(attestationId);
            }
        }
    }
}
