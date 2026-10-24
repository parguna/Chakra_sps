using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class DiscoveryProcess : DomainModelBase<DiscoveryProcess, EntityModel.DiscoveryProcess>
    {
        public string ProcessName { get; set; }

        public string Description { get; set; }

        public long Endpointsystemid { get; set; }

        public bool Default { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public bool DisableDeltaDiscovery { get; set; }

        public EndPoint Endpoint { get; set; }

        public Process process { get; set; }

        public List<EndpointObjects> EndpointObjects { get; set; }

        public override string ToString()
        {
            return this.ProcessName.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is DiscoveryProcess)
            {
                DiscoveryProcess other = (DiscoveryProcess)obj;
                return this.Id.Equals(other.Id);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^ this.Endpointsystemid.GetHashCode();
        }

        public static bool DoesProcessNameAlreadyExists(string processName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.DiscoveryProcess>(dataAccessTransaction)
                       .Any(disprocess => disprocess.ProcessName == processName);
            }
        }

        public static DiscoveryProcess GetByEndpointsystemIdDefault(long endpointsystemid)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.DiscoveryProcess>(dataAccessTransaction)
                      .Get(disprocess => disprocess.Endpointsystemid == endpointsystemid & disprocess.Default).FirstOrDefault());
            }
        }

        public static List<DiscoveryProcess> GetByEndpointsystemId(long endpointsystemid)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.DiscoveryProcess>(dataAccessTransaction)
                    .Get(disprocess => disprocess.Endpointsystemid == endpointsystemid).ToList());
            }
        }

        public static DiscoveryProcess GetByIdIncludeEndpoint(long id)
        {
            DiscoveryProcess discProc = new DiscoveryProcess();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                discProc = ORMap(new RepositoryBase<EntityModel.DiscoveryProcess>(dataAccessTransaction)
                     .Get(disprocess => disprocess.Id == id, disprocess => disprocess.Endpoint, disprocess => disprocess.EndpointObjects,
                      disprocess => disprocess.Endpoint.EndPointSystemType).FirstOrDefault());

                if (discProc != null)
                {
                    discProc.process = Process.GetByProcessTypeKeyId(discProc.Id, Infrastructure.Enums.ProcessType.Discovery);
                }
            }

            return discProc;
        }

        //public override void Save()
        //{
        //    bool isNew = this.IsNew;
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        base.Save(dataAccessTransaction);

        //        if (!isNew && this.EndpointObjects != null)
        //        {

        //            DiscoveryProcess d = ORMap(new RepositoryBase<EntityModel.DiscoveryProcess>(dataAccessTransaction)
        //           .GetById(Id,
        //           attn => attn.EndpointObjects));
        //            // List<EndpointObjects> dbEndpointObjectsDetails = DiscoveryProcess.GetById(Id, dp => dp.en);

        //           // dbEndpointObjectsDetails.ForEach(prd => prd.Delete(dataAccessTransaction));

        //           // this.EndpointObjects.ForEach(prd => prd.Save(dataAccessTransaction));
        //        }
        //        if (isNew)
        //            this.process.ProcessTypeKeyId = Id;

        //        this.process.Save();

        //        dataAccessTransaction.Commit();
        //    }
        //}

        /// <summary>
        /// ///
        /// </summary>
        public override long Save(string currentUser)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                if (this.Id == 0)
                {
                    Process p = this.process;
                    this.process = null;
                    base.Save(transaction);

                    p.ProcessTypeKeyId = this.Id;
                    p.Save(transaction);
                }
                else
                {
                    new DiscoveryProcessRepository(transaction).Update(ORUnMap());
                    this.process.Save(transaction);
                    this.process.Scheduler.Save(transaction);
                }

                transaction.Commit();
            }
            return 0;
        }



        public static bool DeleteDiscoveryProcess(long processId, string User)
        {
            bool canThrowError = false;
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                try
                {
                    dataAccessTransaction.User = User;
                    DiscoveryProcess discoveryProcess = DiscoveryProcess.GetById(processId, true);
                    new DiscoveryProcessRepository(dataAccessTransaction).Update(discoveryProcess.ORUnMap());
                    discoveryProcess.Delete(dataAccessTransaction);
                    if (discoveryProcess != null)
                    {
                        Process process = Process.GetByProcessTypeKeyId(processId, Infrastructure.Enums.ProcessType.Discovery);
                        List<ProcessHistory> processHistory = ProcessHistory.GetAllProcessHistory(process.Id);
                        if (process != null)
                        {
                            process.Delete();
                        }
                        Scheduler scheduler = Scheduler.GetById(process.Scheduler_SchedulerId);
                        if (scheduler != null)
                        {
                            scheduler.Delete();
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
        public static DiscoveryProcess GetByIdIncludeProcessAndEndpoint(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {

                return ORMap(new RepositoryBase<EntityModel.DiscoveryProcess>(dataAccessTransaction).
                        Get(dp =>
                            dp.Id == id,
                            dp => dp.Endpoint,
                            dp => dp.Endpoint.EndPointSystemType,
                            dp => dp.Endpoint.Owner,
                            dp => dp.process
                            ).
                            FirstOrDefault());
            }
        }

        public static DiscoveryProcess GetByIdIncludingProcessAndEndpoint(long id)
        {
            DiscoveryProcess discprocess;
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                discprocess = ORMap(new RepositoryBase<EntityModel.DiscoveryProcess>(dataAccessTransaction).
                        Get(dp =>
                            dp.Id == id,
                            dp => dp.Endpoint,
                            dp => dp.Endpoint.EndPointSystemType,
                            dp => dp.Endpoint.Owner
                            ).
                            FirstOrDefault());
                Process process = Process.GetByProcessTypeKeyId(id, Infrastructure.Enums.ProcessType.Discovery);
                discprocess.process = process;
            }
            return discprocess;
        }
        public static List<EndpointObjects> GetByDiscoveryProcessId(long discoveryProcessId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.DiscoveryProcess>(dataAccessTransaction).Get(dp => dp.Id == discoveryProcessId, dp => dp.EndpointObjects).SingleOrDefault()).EndpointObjects;
            }
        }

        public static DiscoveryProcess GetDefaultDiscoveryProcess(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.DiscoveryProcess>(dataAccessTransaction)
                      .Get(disprocess => disprocess.Endpointsystemid == endpointSystemId && disprocess.Default).FirstOrDefault());
            }
        }


    }
}
