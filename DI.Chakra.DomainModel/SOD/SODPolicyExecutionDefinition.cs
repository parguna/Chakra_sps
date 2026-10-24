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
    public class SODPolicyExecutionDefinition : DomainModelBase<SODPolicyExecutionDefinition, EntityModel.SODPolicyExecutionDefinition>
    {
        public Nullable<long> CompliancePolicyId { get; set; }

        public string UniqueRunCode { get; set; }

        public SODOutputType OutputType { get; set; }

        public Nullable<System.DateTime> RunDate { get; set; }

        public Nullable<bool> RunAtObjectLevel { get; set; }

        public CompliancePolicy CompliancePolicy { get; set; }

        public List<SODEndPointPolicySelection> EndpointSelection { get; set; }

        public List<SODException> Exceptions { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        //public override void Save()
        //{
        //    bool isNew = this.IsNew;

        //    using (DataAccessTransaction tranaction = new DataAccessTransaction())
        //    {
        //        base.Save(tranaction);

        //        if (!isNew && this.Exceptions != null && this.EndpointSelection !=null)
        //        {
        //            List<SODException> dbfunctions = ORMap(new RepositoryBase<EntityModel.SODPolicyExecutionDefinition>(tranaction)
        //                .Get(g => g.Id == this.Id, g => g.Exceptions).Single()).Exceptions;

        //            List<SODException> newfuntions = dbfunctions
        //                .Except(this.Exceptions, new IdBaseEqualityComparer<SODException>()).ToList();

        //            newfuntions.ForEach(functn => functn.Delete(tranaction));

        //            this.Exceptions.ForEach(functn => functn.Save());

        //            List<SODEndPointPolicySelection> soddbfunctions = ORMap(new RepositoryBase<EntityModel.SODPolicyExecutionDefinition>(tranaction)
        //                .Get(g => g.Id == this.Id, g => g.EndpointSelection).Single()).EndpointSelection;

        //            List<SODEndPointPolicySelection> newfunction = soddbfunctions
        //                .Except(this.EndpointSelection, new IdBaseEqualityComparer<SODEndPointPolicySelection>()).ToList();

        //            newfunction.ForEach(functn => functn.Delete(tranaction));

        //            this.EndpointSelection.ForEach(functn => functn.Save());

        //        }
        //        tranaction.Commit();
        //    }
        //}
        public override long Save(string currentUser)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                if (this.Id == 0)
                {
                    this.Id = new SODPolicyExceutionDefinitionRepository(transaction).Insert(ORUnMap());
                    //base.Save(transaction);
                }
                else
                {
                    new SODPolicyExceutionDefinitionRepository(transaction).Update(ORUnMap());
                }

                transaction.Commit();
            }
            return 0;
        }
        public static Dictionary<long, string> GetBySearchString(string searchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new SODPolicyExceutionDefinitionRepository(dataAccessTransaction).GetBySearchString(CheckNull.AsteriskToString(searchString));
            }
        }

        public static List<SODPolicyExecutionDefinition> GetAllByIncludeCompliancePolicy()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SODPolicyExecutionDefinition>(transaction)
                    .GetAll(excdef => excdef.CompliancePolicy).ToList());
            }
        }

        public static bool HasPolicy(long policyId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SODPolicyExecutionDefinition>(dataAccessTransaction)
                    .Any(pExec => pExec.CompliancePolicyId == policyId);
            }
        }
        public static bool DoesExists(string UniqueRunCode)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SODPolicyExecutionDefinition>(dataAccessTransaction)
                    .Any(pExec => pExec.UniqueRunCode == UniqueRunCode);
            }
        }
        public static bool HasException(long exceptionId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SODPolicyExecutionDefinition>(dataAccessTransaction)
                    .Any(pExec => pExec.Exceptions.Any(ex => ex.Id == exceptionId));
            }
        }

        public static SODPolicyExecutionDefinition GetByIdIncludingCompliance(long id)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SODPolicyExecutionDefinition>(transaction)
                .Get(excdef => excdef.Id == id, excdef => excdef.CompliancePolicy, excef => excef.EndpointSelection, 
                excef => excef.Exceptions).FirstOrDefault());
            }
        }

        public static void SODPolicyDeleteById(long SODPolicyDeleteById)
        {
            SODEndPointPolicySelection.DeleteSODEndPointPolicySelection(SODPolicyDeleteById);
            SODPolicyExecutionDefinition.Delete(SODPolicyDeleteById);
        }
    }
}
