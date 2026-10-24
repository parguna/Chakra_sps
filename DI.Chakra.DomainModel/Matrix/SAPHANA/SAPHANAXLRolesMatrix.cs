using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class SAPHANAXLRolesMatrix : DomainModelBase<DomainModel.SAPHANAXLRolesMatrix, EntityModel.SAPHANAXLRolesMatrix>, IXLData, IApprover
    {
        public long EndpointSystemId { get; set; }
        public string Remarks { get; set; }
        public string Application { get; set; }
        public string Module { get; set; }
        public string SubModule { get; set; }
        public string RoleName { get; set; }
        public string TechnicalRole { get; set; }
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }
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

        public string BusinessRole
        {
            get { return this.RoleName; }
        }
        public string SystemName
        {
            get;set;
        }
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
                    //new RepositoryBase<EntityModel.SAPHANAXLRolesMatrix>(dataAccessTransaction).ExecuteQuery("Truncate table DI_SAPHANA_XL_RolesMatrix");
                    //dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static SAPHANAXLRolesMatrix GetSAPHANAXLRoleMatrixbyObject(SAPHANAXLRolesMatrix obj)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.SAPHANAXLRolesMatrix>(dataAccessTransaction)
                        .Get(cnfg =>cnfg.EndpointSystemId == obj.EndpointSystemId &&  cnfg.TechnicalRole == obj.TechnicalRole && cnfg.Application == obj.Application).FirstOrDefault());
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static SAPHANAXLRolesMatrix GetSAPHANARoleMatrix(string role, long endpointSystemId)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.SAPHANAXLRolesMatrix>(dataAccessTransaction)
                        .Get(cnfg => cnfg.EndpointSystemId == endpointSystemId && cnfg.RoleName == role).FirstOrDefault());
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static bool IsEmployeeConfiguredAsApprover(string employeeId)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return new RepositoryBase<EntityModel.SAPHANAXLRolesMatrix>(dataAccessTransaction)
                        .Any(sap => sap.Level1Approver == employeeId
                       || sap.Level2Approver == employeeId
                       || sap.Level3Approver == employeeId);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<SAPHANAXLRolesMatrix> GetAllSAPHANAXLRoleMatrix(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.SAPHANAXLRolesMatrix>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new RepositoryBase<EntityModel.SAPHANAXLRolesMatrix>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.SAPHANAXLRolesMatrix>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                    }
                }
            }
            catch { throw; }
        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.SAPHANAXLRolesMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.SAPHANAXLRolesMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
                if (lastModifiedDate != null && lastModifiedDate.LastModifiedDate != null && lastModifiedDate.LastModifiedDate > lastCreatedDate?.CreatedDate)
                {
                    return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastModifiedDate.LastModifiedBy, lastModifiedDate.LastModifiedDate);
                }
                else
                {
                    if (lastCreatedDate != null && lastCreatedDate.CreatedDate != null)
                    {
                        return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastCreatedDate.CreatedBy, lastCreatedDate.CreatedDate);
                    }
                    else
                        return string.Empty;
                }
            }
        }

        public static IList<SAPHANAXLRolesMatrix> BindRolesSAPHANA(long endpointSystemId)
        {
            try
            {
                
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    var result = new RepositoryBase<EntityModel.SAPHANAXLRolesMatrix>(dataAccessTransaction).Get(source => source.EndpointSystemId == endpointSystemId &&
                                                                                                                            !string.IsNullOrEmpty(source.RoleName)).OrderBy(source => source.RoleName); 
                    List<SAPHANAXLRolesMatrix> data = new List<SAPHANAXLRolesMatrix>();
                    foreach (var items in result)
                    {
                        var check = data.Count(v => v.RoleName == items.RoleName);
                        if (check == 0)
                        {
                            data.Add(ORMap(items));
                        }
                    }
                    return data;
                }
            }
            catch
            {
                throw;
            }
        }
        public static IList<SAPHANAXLRolesMatrix> BindViewDetailsEBS(string Role)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var result =  new RepositoryBase<EntityModel.SAPHANAXLRolesMatrix>(dataAccessTransaction).Get(source=> source.RoleName == Role && !string.IsNullOrEmpty(source.RoleName)).OrderBy(source => source.RoleName);
                List<SAPHANAXLRolesMatrix> data = new List<SAPHANAXLRolesMatrix>();
                foreach (var items in result)
                {
                    var check = data.Count(v => v.TechnicalRole == items.TechnicalRole);
                    if (check == 0)
                    {
                        data.Add(ORMap(items));
                    }
                }
                return data;
            }
        }

        public static List<string> GetTechnicalRolesForSAPHANA(string roleName, string applicationName)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return new RepositoryBase<EntityModel.SAPHANAXLRolesMatrix>(dataAccessTransaction).Get(item => item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now
                            && item.RoleName == roleName && item.Application == applicationName).Select(role => role.TechnicalRole).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<string> GetTechnicalRolesForSAPHANA(List<string> roleNames, string applicationName)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return new RepositoryBase<EntityModel.SAPHANAXLRolesMatrix>(dataAccessTransaction).Get(item => item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now
                            && roleNames.Contains(item.RoleName) && item.Application == applicationName).Select(role => role.TechnicalRole).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
