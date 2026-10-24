using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SODFunction : DomainModelBase<SODFunction, EntityModel.SODFunction>
    {
        public string SodFunctionCode { get; set; }

        public string SodFunctionDescription { get; set; }

        public Nullable<bool> IsActive { get; set; }

        public List<SODFunctionItem> SodFunctionItems { get; set; }

        public List<SODConflictFunctions> ConflictFunctions { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static SODFunction GetByIdIncludeSodFunctionItem(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SODFunction>(tranaction)
                    .GetById(id, 
                    functionItem => functionItem.SodFunctionItems, 
                    functionItem => functionItem.SodFunctionItems.Select(x => x.endpoint), 
                    functionItem => functionItem.SodFunctionItems.Select(x => x.EndpointType), 
                    functionItem => functionItem.SodFunctionItems.Select(x => x.endpoint.EndPointSystemType)));
            }
        }

        public static List<SODFunction> GetAllIncludeSodFunctionItems()
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SODFunction>(tranaction)
                    .GetAll(functionItem => functionItem.SodFunctionItems, 
                    functionItem => functionItem.SodFunctionItems.Select(x => x.endpoint), 
                    functionItem => functionItem.SodFunctionItems.Select(x => x.EndpointType), 
                    functionItem => functionItem.SodFunctionItems.Select(x => x.endpoint.EndPointSystemType), 
                    functionItem => functionItem.ConflictFunctions).ToList());
            }
        }

        public static bool DoesExists(string sodFunctionCode)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SODFunction>(dataAccessTransaction)
                    .Any(functioncode => functioncode.SodFunctionCode == sodFunctionCode);
            }
        }

        public static Dictionary<long, string> GetBySearchString(string searchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new SODFunctionRepository(dataAccessTransaction).GetBySearchString(CheckNull.AsteriskToString(searchString));
            }
        }

        public static SODFunction GetBySodFunctionCodeIncludingSodFunctionItems(string sodFunctionCode)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SODFunction>(tranaction)
                    .Get(functionitem => functionitem.SodFunctionCode == sodFunctionCode, 
                    functionItem => functionItem.SodFunctionItems, 
                    functionItem => functionItem.SodFunctionItems.Select(x => x.endpoint), 
                    functionItem => functionItem.SodFunctionItems.Select(x => x.EndpointType), 
                    functionItem => functionItem.SodFunctionItems.Select(x => x.endpoint.EndPointSystemType)).FirstOrDefault());
            }
        }

        public static List<SODFunction> GetListBySodFunctionCodeIncludingSodFunctionItems(string sodFunctionCode)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SODFunction>(tranaction)
                    .Get(functionitem => functionitem.SodFunctionCode == sodFunctionCode,
                    functionItem => functionItem.SodFunctionItems,
                    functionItem => functionItem.SodFunctionItems.Select(x => x.endpoint),
                    functionItem => functionItem.SodFunctionItems.Select(x => x.EndpointType),
                    functionItem => functionItem.SodFunctionItems.Select(x => x.endpoint.EndPointSystemType)).ToList());
            }
        }
      
        public override long Save(string currentUser)
        {
            bool isNew = this.IsNew;

            using (DataAccessTransaction tranaction = new DataAccessTransaction(currentUser))
            {
                base.Save(tranaction);

                if (!isNew && this.SodFunctionItems != null)
                {
                    List<SODFunctionItem> dbfunctions = ORMap(new RepositoryBase<EntityModel.SODFunction>(tranaction)
                        .Get(g => g.Id == this.Id, g => g.SodFunctionItems).Single()).SodFunctionItems;

                    List<SODFunctionItem> newfunctions = dbfunctions
                        .Except(this.SodFunctionItems, new IdBaseEqualityComparer<SODFunctionItem>()).ToList();

                    newfunctions.ForEach(functn => functn.Delete(tranaction));

                    this.SodFunctionItems.ForEach(functn => functn.Save(tranaction));
                }

                tranaction.Commit();
            }
            return 0;
        }
    }
}