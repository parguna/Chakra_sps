using DI.Chakra.Infrastructure;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{

    public class ProvisioningConfigMatrixSAPPosition : DomainModelBase<DomainModel.ProvisioningConfigMatrixSAPPosition, EntityModel.ProvisioningConfigMatrixSAPPosition>, IXLData
    {
        public string PositionText { get; set; }
        public string DefaultRole { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }

        public bool IsValid { get; set; }

        public string ValidationMessage { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public static string GetSAPRolesBasedOnUserPosition(User user)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    var matrices = ORMapList(new ProvisioningConfigMatrixSAPPositionRepository(dataAccessTransaction).GetSAPRolesBasedOnUserPosition
                        (user.PositionName));

                    return string.Join(",", matrices.Select(item => item.DefaultRole));
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static void Add(ProvisioningConfigMatrixSAPPosition obj)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                new ProvisioningConfigMatrixSAPPositionRepository(dataAccessTransaction).Insert(obj.ORUnMap());
                dataAccessTransaction.Commit();
            }
        }

        public static bool TruncateTable()
        {
            bool returnValue = false;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new ProvisioningConfigMatrixSAPPositionRepository(dataAccessTransaction).ExecuteQuery("Truncate table DI_M_ProvisioningConfigMatrixSAPPosition");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch
            {
            }
            return returnValue;
        }
        public static void ManualUpdate(ProvisioningConfigMatrixSAPPosition obj)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                dataAccessTransaction.User = obj.LastModifiedBy;
                new ProvisioningConfigMatrixSAPPositionRepository(dataAccessTransaction).Update(obj.ORUnMap());
                dataAccessTransaction.Commit();
            }
        }
        public static IList<ProvisioningConfigMatrixSAPPosition> GetAllSAPRoleBasedOnPosition(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new ProvisioningConfigMatrixSAPPositionRepository(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new ProvisioningConfigMatrixSAPPositionRepository(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new ProvisioningConfigMatrixSAPPositionRepository(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                    }
                }
            }
            catch { throw; }
        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.ProvisioningConfigMatrixSAPPosition>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.ProvisioningConfigMatrixSAPPosition>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
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
    }
}
