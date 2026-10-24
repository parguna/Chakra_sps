using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SODConflictFunctions : DomainModelBase<SODConflictFunctions, EntityModel.SODConflictFunctions>
    {
        public string SodConflictCode { get; set; }

        public string SodConflictDescription { get; set; }

        public string CompliancePolicyType { get; set; }

        public long RiskCodeId { get; set; }

        public Nullable<DateTime> LastGeneratedDate { get; set; }

        public Nullable<bool> IsActive { get; set; }

        public string Status { get; set; }

        public SODRiskCode SodRiskCode { get; set; }

        public List<SODFunction> Functions { get; set; }

        public List<CompliancePolicy> CompliancePolicies { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static bool DoesExists(string sodConflictCode)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SODConflictFunctions>(dataAccessTransaction)
                    .Any(conflictcode => conflictcode.SodConflictCode == sodConflictCode);
            }
        }

        public static bool HasRiskCode(long riskCodeId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SODConflictFunctions>(dataAccessTransaction)
                    .Any(confunc => confunc.RiskCodeId == riskCodeId);
            }
        }

        public static bool HasFunction(long functionId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SODConflictFunctions>(dataAccessTransaction)
                    .Any(confunc => confunc.Functions.Any(func => func.Id == functionId));
            }
        }

        //public override void Save()
        //{
        //    bool isNew = this.IsNew;

        //    using (DataAccessTransaction tranaction = new DataAccessTransaction())
        //    {
        //        base.Save(tranaction);

        //        if (!isNew && this.Functions != null)
        //        {
        //            List<SODFunction> dbfunctions = ORMap(new RepositoryBase<EntityModel.SODConflictFunctions>(tranaction)
        //                .Get(g => g.Id == this.Id, g => g.Functions).Single()).Functions;

        //            List<SODFunction> newfunction = dbfunctions.Except(this.Functions, new IdBaseEqualityComparer<SODFunction>()).ToList();

        //            newfunction.ForEach(user => user.Delete(tranaction));

        //            this.Functions.ForEach(user => user.Save());
        //        }

        //        tranaction.Commit();
        //    }
        //}

        public static List<SODConflictFunctions> GetAllByIncludeSodRiskCode()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SODConflictFunctions>(dataAccessTransaction)
                    .GetAll(confun => confun.SodRiskCode,confun=>confun.Functions).ToList());
            }
        }

        public static SODConflictFunctions GetByIdWithNavigation(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SODConflictFunctions>(dataAccessTransaction)
                    .GetById(id,confun => confun.CompliancePolicies, confun => confun.Functions));
            }
        }


        public static SODConflictFunctions GetByIdIncludingSODRiskCode(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SODConflictFunctions>(dataAccessTransaction)
                    .GetById(id,confun=>confun.SodRiskCode, confun => confun.CompliancePolicies, confun => confun.Functions));
            }
        }

        public static List<SODConflictFunctions> GetBySodConflictCodeIncludingFunctions(string sodConflictCode)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SODConflictFunctions>(tranaction)
                    .Get(confun => confun.SodConflictCode == sodConflictCode, 
                    confun => confun.Functions, confun => confun.Functions.Select(x=>x.SodFunctionItems),
                    confun=>confun.Functions.Select(x=>x.SodFunctionItems.Select(y=>y.EndpointType)),
                    confun=>confun.Functions.Select(x=>x.SodFunctionItems.Select(y=>y.endpoint)),
                    confun=>confun.Functions.Select(x=>x.SodFunctionItems.Select(y=>y.endpoint.EndPointSystemType))).ToList()); 
            }
        }
        public static SODConflictFunctions GetSodFunctionFromConflictFunction(string sodConflictCode)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SODConflictFunctions>(tranaction)
                    .Get(confun => confun.SodConflictCode == sodConflictCode,
                    confun => confun.Functions, confun => confun.Functions.Select(x => x.SodFunctionItems),
                    confun => confun.Functions.Select(x => x.SodFunctionItems.Select(y => y.EndpointType)),
                    confun => confun.Functions.Select(x => x.SodFunctionItems.Select(y => y.endpoint)),
                    confun => confun.Functions.Select(x => x.SodFunctionItems.Select(y => y.endpoint.EndPointSystemType))).FirstOrDefault());
            }
        }
        public static Dictionary<long, string> GetBySearchString(string searchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new SODConflictFunctionsRepository(dataAccessTransaction).GetBySearchString(searchString);
            }
        }

        public static Dictionary<long, string> GetBySodConflictCode(string searchString, string filterString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new SODConflictFunctionsRepository(dataAccessTransaction).GetBySodConflictCode(CheckNull.AsteriskToString(searchString), filterString);
            }
        }

        public static void DeleteBySodConflictCode(string sodConflictCode)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                List<SODConflictFunctions> sODConflictFunctionsList = ORMapList(new RepositoryBase<EntityModel.SODConflictFunctions>(tranaction)
                    .Get(functn => functn.SodConflictCode == sodConflictCode).ToList());

                sODConflictFunctionsList.ForEach(function => function.Delete(tranaction));

                tranaction.Commit();
            }
        }

        public override long Save(string currentUser)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                if (this.Id == 0)
                {
                    this.Id = new SODConflictFunctionsRepository(transaction).Insert(ORUnMap());
                    base.Save(transaction);
                }
                else
                {
                    new SODConflictFunctionsRepository(transaction).Update(ORUnMap());
                }

                transaction.Commit();
            }
            return 0;
        }
    }
}
