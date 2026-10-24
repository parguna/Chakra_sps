using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SODConflict : DomainModelBase<SODConflict, EntityModel.SODConflict>
    {
        public string SODConflictCode { get; set; }

        public string SODConflictDescription { get; set; }

        public Nullable<long> FunctionId1 { get; set; }

        public Nullable<long> FunctionId2 { get; set; }

        public long RiskCodeId { get; set; }

        public int CompliancePolicyId { get; set; }

        public Nullable<int> SodConflictType { get; set; }

        public SODRiskCode SodRiskId { get; set; }

        public SODFunction function1 { get; set; }

        public SODFunction function2 { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static bool DoesExists(string sodConflictCode)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SODConflict>(dataAccessTransaction)
                    .Any(conflictcode => conflictcode.SODConflictCode == sodConflictCode);
            }
        }

        public static void DeleteBySodConflictCode(string sodConflictCode)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                List<SODConflict> sODConflictList = ORMapList(new RepositoryBase<EntityModel.SODConflict>(tranaction)
                    .Get(functn => functn.SODConflictCode == sodConflictCode).ToList());

                sODConflictList.ForEach(function => function.Delete(tranaction));
                tranaction.Commit();
            }
        }

        public static  void DeleteConflictByFunction(long functionId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                List<SODConflict> sodConflict = ORMapList(new ConflictRepository(transaction).GetConflictPairByFunction(functionId));
                foreach(SODConflict conflict in sodConflict)
                {
                    conflict.Delete(transaction);
                }
                transaction.Commit();
            }
        }

        public static List<SODConflict> GetBySodConflictCodeIncludingSODRiskCode(string SodConflictCode)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SODConflict>(tranaction)
                    .Get(fun => fun.SODConflictCode == SodConflictCode,
                    fun => fun.SodRiskId,
                    fun=>fun.SodRiskId.sodrisklevel,
                    fun=>fun.SodRiskId.sodriskscore,
                    fun=>fun.function1,
                    fun=>fun.function1.SodFunctionItems,
                    fun=>fun.function1.SodFunctionItems.Select(x=>x.EndpointType),
                    fun=>fun.function1.SodFunctionItems.Select(x=>x.endpoint),
                    fun=>fun.function1.SodFunctionItems.Select(x=>x.endpoint.EndPointSystemType),
                    fun=>fun.function2,
                    fun=>fun.function2.SodFunctionItems,
                    fun => fun.function2.SodFunctionItems.Select(x => x.EndpointType),
                    fun => fun.function2.SodFunctionItems.Select(x => x.endpoint),
                    fun=>fun.function2.SodFunctionItems.Select(x=>x.endpoint.EndPointSystemType)).ToList());
            }
        }
       
    }
}
