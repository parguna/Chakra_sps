using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SODException : DomainModelBase<SODException, EntityModel.SODException>
    {
        public string ExceptionCode { get; set; }

        public string ExceptionDesc { get; set; }

        public Nullable<long> RiskOwnerId { get; set; }

        public  User RiskOwner { get; set; }

        public  List<SODExceptionItem> ExceptionItems { get; set; }

        public  List<SODPolicyExecutionDefinition> SODPolicyExecutionDefinition { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        //public override void Save()
        //{
        //    bool isNew = this.IsNew;

        //    using (DataAccessTransaction tranaction = new DataAccessTransaction())
        //    {
        //        base.Save(tranaction);

        //        if (!isNew && this.ExceptionItems != null)
        //        {
        //            List<SODExceptionItem> dbfunctions = ORMap(new RepositoryBase<EntityModel.SODException>(tranaction)
        //                .Get(g => g.Id == this.Id, g => g.ExceptionItems).Single()).ExceptionItems;

        //            List<SODExceptionItem> newfunctions = dbfunctions
        //                .Except(this.ExceptionItems, new IdBaseEqualityComparer<SODExceptionItem>()).ToList();

        //            newfunctions.ForEach(functn => functn.Delete(tranaction));

        //            this.ExceptionItems.ForEach(functn => functn.Save(tranaction));
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
                    this.Id = new SODExceptionRepository(transaction).Insert(ORUnMap());
                    //base.Save(transaction);
                }
                else
                {
                    new SODExceptionRepository(transaction).Update(ORUnMap());
                    //List<SODExceptionItem> dbfunctions = ORMap(new RepositoryBase<EntityModel.SODException>(transaction)
                    //    .Get(g => g.Id == this.Id, g => g.ExceptionItems).Single()).ExceptionItems;

                    //List<SODExceptionItem> newfunctions = dbfunctions
                    //    .Except(this.ExceptionItems, new IdBaseEqualityComparer<SODExceptionItem>()).ToList();

                    //newfunctions.ForEach(functn => functn.Delete(transaction));

                    //this.ExceptionItems.ForEach(functn => functn.Save(transaction));
                }


                transaction.Commit();
            }
            return 0;
        }
        public static bool HasFunction(long functionId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SODException>(dataAccessTransaction)
                    .Any(ex => ex.ExceptionItems.Any(exItem => exItem.FunctionId1 == functionId || exItem.FunctionId2 == functionId));
            }
        }

        public static SODException GetByIdIncludeRiskOwner(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SODException>(tranaction)
                    .GetById(id, 
                    exptn => exptn.RiskOwner, 
                    exptn => exptn.ExceptionItems, 
                    exptn => exptn.ExceptionItems.Select(x => x.function1), 
                    exptn => exptn.ExceptionItems.Select(x => x.function2), 
                    exptn => exptn.ExceptionItems.Select(x => x.functionitem1), 
                    exptn => exptn.ExceptionItems.Select(x => x.functionitem2), 
                    exptn => exptn.ExceptionItems.Select(x => x.user)));
            }
        }

        public static SODException GetByExceptionCode(string exceptionCode)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SODException>(tranaction)
                    .Get(exptn => exptn.ExceptionCode == exceptionCode, 
                    exptn => exptn.RiskOwner, 
                    exptn => exptn.ExceptionItems, 
                    exptn => exptn.ExceptionItems.Select(x => x.function1), 
                    exptn => exptn.ExceptionItems.Select(x => x.function2), 
                    exptn => exptn.ExceptionItems.Select(x => x.functionitem1), 
                    exptn => exptn.ExceptionItems.Select(x => x.functionitem2), 
                    exptn => exptn.ExceptionItems.Select(x => x.user)).FirstOrDefault());
            }
        }

        public static Dictionary<long, string> GetBySearchString(string searchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new SODExceptionRepository(dataAccessTransaction).GetBySearchString(CheckNull.AsteriskToString(searchString));
            }
        }

        public static List<SODException> GetAllIncludingRiskOwner()
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SODException>(tranaction)
                    .GetAll(exptn => exptn.RiskOwner,
                    exptn => exptn.ExceptionItems,
                    exptn => exptn.ExceptionItems.Select(x => x.function1),
                    exptn => exptn.ExceptionItems.Select(x => x.function2),
                    exptn => exptn.ExceptionItems.Select(x => x.functionitem1),
                    exptn => exptn.ExceptionItems.Select(x => x.functionitem2),
                    exptn => exptn.ExceptionItems.Select(x => x.user)).ToList());
            }
        }

        public override void Delete()
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                SODExceptionItem exceptionitem = SODExceptionItem.GetByExceptionId(this.Id);
                exceptionitem.Delete(tranaction);
                              
                base.Delete(tranaction);

                tranaction.Commit();
            }
        }
    }
}
