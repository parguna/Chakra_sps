using DI.Chakra.Infrastructure;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class NOVASoDExceptionsMatrix: DomainModelBase<NOVASoDExceptionsMatrix, EntityModel.NOVASoDExceptionsMatrix>, IXLData
    {
        public long EndPointSystemId { get; set; }
        public string Exceptions { get; set; }
        public string AffectedUsers { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.NOVASoDExceptionsMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.NOVASoDExceptionsMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
                if (lastModifiedDate != null && lastModifiedDate.LastModifiedDate != null && lastModifiedDate.LastModifiedDate > lastCreatedDate?.CreatedDate)
                {
                    return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastModifiedDate.LastModifiedBy, lastModifiedDate.LastModifiedDate);
                    //string userDisplayName = User.GetByUserName(lastModifiedDate.LastModifiedBy)?.DisplayName;
                    //return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, userDisplayName, lastModifiedDate.LastModifiedDate);
                }
                else
                {
                    if (lastCreatedDate != null && lastCreatedDate.CreatedDate != null)
                        return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastCreatedDate.CreatedBy, lastCreatedDate.CreatedDate);
                    else
                        return string.Empty;
                }
            }
        }

        public static IList<NOVASoDExceptionsMatrix> GetSODMatricesBySectionCode(long endpointId,long dIUserId, string sapSection)
        {
            var user = User.GetById(dIUserId);
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new NOVASoDExceptionsMatrixRepository(dataAccessTransaction).GetByUserId(endpointId, dIUserId,sapSection).ToList());
            }
        }

        public static NOVASoDExceptionsMatrix GetNOVASoDExceptionsMatrixbyObject(NOVASoDExceptionsMatrix obj)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.NOVASoDExceptionsMatrix>(dataAccessTransaction)
                        .Get(nova => nova.Exceptions == obj.Exceptions && nova.AffectedUsers == obj.AffectedUsers).FirstOrDefault());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool TruncateTable()
        {
            bool returnValue = false;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.NOVASoDExceptionsMatrix>(dataAccessTransaction).ExecuteQuery("Truncate table DI_NOVA_SoDExceptionsMatrix");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }

        public static List<NOVASoDExceptionsMatrix> GetNOVASoDExceptionsMatrices(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.NOVASoDExceptionsMatrix>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new RepositoryBase<EntityModel.NOVASoDExceptionsMatrix>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.NOVASoDExceptionsMatrix>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                    }
                }
            }
            catch { throw; }
        }
    }
}
