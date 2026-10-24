using DI.Chakra.Infrastructure;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPXLMSSLCriticalTCodeMatrix : DomainModelBase<DomainModel.SAPXLMSSLCriticalTCodeMatrix, EntityModel.SAPXLMSSLCriticalTCodeMatrix>, IXLData
    {
        public string Transaction { get; set; }
        public string TransactionDescription { get; set; }
        public string TransactionOwner { get; set; }
        public DateTime? ValidFrom
        {
            get;
            set;
        }
        public DateTime? ValidTo
        {
            get;
            set;
        }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public static bool TruncateTable()
        {
            bool returnValue = false;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.SAPXLMSSLCriticalTCodeMatrix>(dataAccessTransaction).ExecuteQuery("Truncate table DI_SAP_MSSL_XL_CriticalTCodeMatrix");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static SAPXLMSSLCriticalTCodeMatrix GetSAPXLMSSLCriticalTCodeMatrixbyObject(SAPXLMSSLCriticalTCodeMatrix obj)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.SAPXLMSSLCriticalTCodeMatrix>(dataAccessTransaction)
                        .Get(cnfg => cnfg.Transaction == obj.Transaction).FirstOrDefault());
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<SAPXLMSSLCriticalTCodeMatrix> GetAllSAPMSSLXLCriticalTCodesMatrix(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.SAPXLMSSLCriticalTCodeMatrix>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new RepositoryBase<EntityModel.SAPXLMSSLCriticalTCodeMatrix>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.SAPXLMSSLCriticalTCodeMatrix>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                    }
                }
            }
            catch { throw; }
        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.SAPXLMSSLCriticalTCodeMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.SAPXLMSSLCriticalTCodeMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
                if (lastModifiedDate != null && lastModifiedDate.LastModifiedDate != null && lastModifiedDate.LastModifiedDate > lastCreatedDate?.CreatedDate)
                {
                    return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastModifiedDate.LastModifiedBy, lastModifiedDate.LastModifiedDate);
                    //string userDisplayName = User.GetByUserName(lastModifiedDate.LastModifiedBy)?.DisplayName;
                    //return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, userDisplayName, lastModifiedDate.LastModifiedDate);
                }
                else
                {
                    if (lastCreatedDate != null && lastCreatedDate.CreatedDate != null)
                    {
                        //string userDisplayName = User.GetByUserName(lastModifiedDate.CreatedBy)?.DisplayName;
                        //return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, userDisplayName, lastCreatedDate.CreatedDate);
                        return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastCreatedDate.CreatedBy, lastCreatedDate.CreatedDate);
                    }
                    else
                        return string.Empty;
                }
            }
        }
        public static bool IsRecordExists(string transaction, long id)
        {
            bool isExists = false;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {

                    if (id > 0)
                    {
                        var obj = new RepositoryBase<EntityModel.SAPXLMSSLCriticalTCodeMatrix>(dataAccessTransaction).Get(item => item.Id != id && item.Transaction == transaction).FirstOrDefault();
                        isExists = (obj != null);
                    }
                    else
                    {
                        isExists = new RepositoryBase<EntityModel.SAPXLMSSLCriticalTCodeMatrix>(dataAccessTransaction).Get(item => item.Transaction == transaction).FirstOrDefault() != null;
                    }
                }
            }
            catch { isExists = false; }

            return isExists;
        }
    }
}
