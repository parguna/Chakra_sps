using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class NPACDesignationInfo : DomainModelBase<NPACDesignationInfo, EntityModel.NPACDesignationInfo>, IEndpointEntitlementInfo, IXLData
    {
        public NPACDesignationInfo()
        {
        }
        public long EndPointSystemId { get; set; }
        public string NPACDesignation { get; set; }
        public string HRDesignationName { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public EndPoint EndPoint { get; set; }
        public string EntitlementName
        {
            get { return this.NPACDesignation; }
        }

        public string EntitlementDesc
        {
            get { return this.HRDesignationName; }
        }
        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Role; }
        }

        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }

        public static string GetNPACDesignationByHRPositionName(string positionCode)
        {
            NPACDesignationInfo desigInfo = new NPACDesignationInfo();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                desigInfo = ORMap(new RepositoryBase<EntityModel.NPACDesignationInfo>(dataAccessTransaction)
                      .Get(grp => grp.HRDesignationName == positionCode).FirstOrDefault());
            }

            if (desigInfo != null)
                return desigInfo.NPACDesignation;

            return "";
        }
        public static List<NPACDesignationInfo> GetAllNPACDesignations(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.NPACDesignationInfo>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new RepositoryBase<EntityModel.NPACDesignationInfo>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.NPACDesignationInfo>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                    }
                }
            }
            catch { throw; }
        }
        public static bool TruncateTable()
        {
            bool returnValue = false;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.NPACDesignationInfo>(dataAccessTransaction).ExecuteQuery("Truncate table DI_NPAC_DesignationInfo");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static NPACDesignationInfo GetNPACXLProvisioningMatrixbyObject(NPACDesignationInfo obj)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.NPACDesignationInfo>(dataAccessTransaction)
                        .Get(cnfg => cnfg.HRDesignationName == obj.HRDesignationName && cnfg.NPACDesignation == obj.NPACDesignation).FirstOrDefault());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.NPACDesignationInfo>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.NPACDesignationInfo>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
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
    }
}
