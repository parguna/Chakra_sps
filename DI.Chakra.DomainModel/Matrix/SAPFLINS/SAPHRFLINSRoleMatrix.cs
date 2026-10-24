using System.Linq;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;

namespace DI.Chakra.DomainModel
{

    public class SAPHRFLINSRoleMatrix : DomainModelBase<DomainModel.SAPHRFLINSRoleMatrix, EntityModel.SAPHRFLINSRoleMatrix>, IXLData, IApprover
    {
        public string Remarks { get; set; }

        public string Application { get; set; }

        public string Company { get; set; }

        public string CostCenter { get; set; }

        public string PurchasingOrg { get; set; }

        public string PurchasingGrp { get; set; }

        public string Plant { get; set; }

        public string MRPControl { get; set; }

        public string StorageLocation { get; set; }

        public string Module { get; set; }

        public string SubModule { get; set; }

        public string BusinessRole { get; set; }

        public string TechnicalRule { get; set; }

        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }


        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }

        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }

        public string MainSupp { get; set; }

        public Nullable<bool> IsModified { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }
        public Nullable<bool> IsUpdatedbyExcelUpload { get; set; }
        public Nullable<int> ReferenceId { get; set; }


        public bool IsValid { get; set; }

        public string ValidationMessage { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public string SystemName
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>(ApplicationType.HRMS_FLINS);
            }
        }

        public static string GetUserType(string businessRole)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).Get(matrix => matrix.BusinessRole == businessRole).Select(matrix => matrix.Remarks).FirstOrDefault();
            }
        }

        public static Dictionary<long, string> GetCompany(string searchString, string businessRoleID, int EndpointUserId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                Dictionary<long, string> data = new Dictionary<long, string>();
                if (EndpointUserId == 0)
                {
                    var result = new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).Get(
                        item => item.BusinessRole == businessRoleID && (string.IsNullOrEmpty(searchString) ? 1 == 1 : item.Company.Contains(searchString)) && (item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now))
                            .Select(x => new { Key = x.Id, Value = x.Company })
                            .Distinct().OrderBy(i => i.Value).ToDictionary(o => o.Key, o => o.Value);
                    foreach (var items in result)
                    {
                        if (!data.ContainsValue(items.Value))
                        {
                            data.Add(items.Key, items.Value);
                        }
                    }
                }
                else
                {
                    List<string> SapRoles = new SAPUserInfoRepository(dataAccessTransaction).GetSAPRolesByEndpointUserId(EndpointUserId);
                    // List<string> SapRoles =  new SAPRoleInfoRepository(dataAccessTransaction).GetByEndPointSystemId(EndpointUserId).Select(role => role.ROLE_CODE).ToList();

                    var result = new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).Get(matrix =>
                                         SapRoles.Contains(matrix.TechnicalRule) && matrix.MainSupp == "main" && matrix.BusinessRole == businessRoleID)
                                         .Select(x => new { Key = x.Id, Value = x.Company })
                                         .Distinct().OrderBy(i => i.Value).ToDictionary(o => o.Key, o => o.Value);


                    foreach (var items in result)
                    {
                        if (!data.ContainsValue(items.Value))
                        {
                            data.Add(items.Key, items.Value);
                        }
                    }

                }

                return data;
            }

        }

        public static Dictionary<long, string> GetOnlyPurchaseGroup(string CostCenter, string businessRoleID, string Company, string PurchaseOrg, string PurchaseGroup, string Planet, string StorageLocation, string MRPControl, long EndpointUserId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var repository = new SAPHRFLINSRoleMatrixRepository(dataAccessTransaction);
                Dictionary<long, string> data = new Dictionary<long, string>();
                data = repository.GetOnlyPurchaseGroupTechnicalRole(businessRoleID, Company, CostCenter, PurchaseOrg, Planet, StorageLocation, MRPControl, EndpointUserId);
                return data;
            }
        }


        public static Dictionary<long, string> GetOnlyCostCenter(string CostCenter, string businessRoleID, string Company, string PurchaseOrg, string PurchaseGroup, string Planet, string StorageLocation, string MRPControl, long EndpointUserId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var repository = new SAPHRFLINSRoleMatrixRepository(dataAccessTransaction);
                Dictionary<long, string> data = new Dictionary<long, string>();
                data = repository.GetOnlyCostCenterTechnicalRole(businessRoleID, Company, CostCenter, PurchaseOrg, Planet, StorageLocation, MRPControl, EndpointUserId);
                return data;
            }
        }


        public static Dictionary<long, string> GetMandatory2DataSource(string mandatory1string, string mandatory2string, string CostCenter, string businessRoleID, string Company, string PurchaseOrg, string PurchaseGroup, string Planet, string StorageLocation, string MRPControl, long EndpointUserId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                Dictionary<long, string> data = new Dictionary<long, string>();
                var repository = new SAPHRFLINSRoleMatrixRepository(dataAccessTransaction);
                switch (mandatory2string)
                {
                    case "Org":
                        data = repository.GetPurchaseOrg(PurchaseOrg, businessRoleID, Company, CostCenter, PurchaseGroup, Planet, StorageLocation, MRPControl, EndpointUserId);
                        break;
                    case "CostCenter":
                        data = repository.GetCostCenter(CostCenter, businessRoleID, Company, PurchaseOrg, PurchaseGroup, Planet, StorageLocation, MRPControl, mandatory1string, EndpointUserId);
                        break;
                    case "PurchaseGroup":
                        data = repository.GetPurchaseGroup(PurchaseGroup, businessRoleID, Company, CostCenter, PurchaseOrg, Planet, StorageLocation, MRPControl, mandatory1string, EndpointUserId);
                        break;
                    case "Plant":
                        data = repository.GetPlanet(Planet, businessRoleID, Company, CostCenter, PurchaseGroup, PurchaseOrg, StorageLocation, MRPControl, EndpointUserId);
                        break;
                    case "StorageLocation":
                        data = repository.GetStoreLocations(StorageLocation, businessRoleID, Company, CostCenter, PurchaseGroup, Planet, PurchaseOrg, MRPControl, EndpointUserId);
                        break;
                    case "MRPController":
                        data = repository.GetMRPController(MRPControl, businessRoleID, Company, CostCenter, PurchaseGroup, Planet, StorageLocation, PurchaseOrg, EndpointUserId);
                        break;

                }

                return data;
            }
        }

        public static IList<SAPHRFLINSRoleMatrix> BindModule(long userID)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPHRFLINSRoleMatrixRepository(dataAccessTransaction).BindModule(userID));
            }
        }

        public static IList<SAPHRFLINSRoleMatrix> BindSubModule(string moduleName, long UserID)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPHRFLINSRoleMatrixRepository(dataAccessTransaction).BindSubModule(moduleName, UserID));
            }
        }

        public static IList<string> GetHrFlinsTechnicalRolesByAdhocRequestObject(string costCenter, string companyCode, string businessRole, string purchasingGroup, string Plant, string MRPController, string StorageLocation, bool RetainCompanyRoles)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPHRFLINSRoleMatrixRepository(dataAccessTransaction).GetHrFlinsTechnicalRolesByAdhocRequestObject(costCenter, companyCode, businessRole, purchasingGroup, Plant, MRPController, StorageLocation, RetainCompanyRoles);
            }
        }
        public static List<SAPHRFLINSRoleMatrix> GetAllExistingBusinessRoles(long EndpointUserId, string CostCenterPurchaseGroup)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var roles = new SAPHRFLINSRoleMatrixRepository(dataAccessTransaction).getExistingBusinessRole(EndpointUserId, CostCenterPurchaseGroup);
                var onlyBusinessRoles = roles.Select(i => i.BusinessRole).Distinct().ToList();
                List<string> SapRoles = new SAPUserInfoRepository(dataAccessTransaction).GetSAPRolesByEndpointUserId(EndpointUserId);
                // List<string> SapRoles =  new SAPRoleInfoRepository(dataAccessTransaction).GetByEndPointSystemId(EndpointUserId).Select(role => role.ROLE_CODE).ToList();

                var result = new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).Get(matrix =>
                                     SapRoles.Contains(matrix.TechnicalRule) && matrix.MainSupp == "main" && onlyBusinessRoles.Contains(matrix.BusinessRole)).ToList();
                return ORMapList(result);
            }
        }



        public static List<SAPHRFLINSRoleMatrix> GetEndpointUserBusinessRolesIncudeSupplementary(long EndpointUserId, string CostCenterPurchaseGroup)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var roles = new SAPHRFLINSRoleMatrixRepository(dataAccessTransaction).getExistingBusinessRole(EndpointUserId, CostCenterPurchaseGroup);
                var onlyBusinessRoles = roles.Select(i => i.BusinessRole).Distinct().ToList();
                var onlyBusinessRolesCompany = roles.Select(i => new { i.BusinessRole, i.Company }).Distinct().ToList();
                List<string> SapRoles = new SAPUserInfoRepository(dataAccessTransaction).GetSAPRolesByEndpointUserId(EndpointUserId);
                //List<string> SapRoles =  new SAPRoleInfoRepository(dataAccessTransaction).GetByEndPointSystemId(EndpointUserId).Select(role => role.ROLE_CODE).ToList();

                var Supplementaryresult = new RepositoryBase<EntityModel.SAPHRFLINSSuppRoleMatrix>(dataAccessTransaction).Get(matrix =>
                                     SapRoles.Contains(matrix.Role)).Select(s=>new { Rule = s.Rule, Company = s.Company, CostCenter = s.CostCenter, PurchaseGrp = s.PurchasingGrp }).ToList();

                //var technicalRules = Supplementaryresult.Select(s => s.Rule);

                //var companies = Supplementaryresult.Select(s => s.Company);

              

                var result1 = new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).Get(matrix =>
                                     SapRoles.Contains(matrix.TechnicalRule) && matrix.MainSupp == "main" && onlyBusinessRoles.Contains(matrix.BusinessRole)).ToList();

                //var result2 = new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).Get(matrix => matrix.MainSupp == "supplement" &&
                //                     onlyBusinessRoles.Contains(matrix.BusinessRole)).Where(matrix => Supplementaryresult.Any(s => s.Rule == matrix.TechnicalRule && s.Company == matrix.Company)).Select(s=>s).ToList();
                var set1 = new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).Get(matrix => matrix.MainSupp == "supplement" && onlyBusinessRoles.Contains(matrix.BusinessRole)).ToList();

                var set2 = (from mainRole in set1
                            from sup in Supplementaryresult
                            where mainRole.TechnicalRule == sup.Rule && mainRole.Company.ToUpper() == sup.Company.ToUpper()
                            && onlyBusinessRolesCompany.Exists(i => i.BusinessRole == mainRole.BusinessRole && i.Company == mainRole.Company)
                            select mainRole).ToList();
                //var result2 = new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).Get(matrix => matrix.MainSupp == "supplement" && onlyBusinessRoles.Contains(matrix.BusinessRole)).Where(matrix => Supplementaryresult.Any(s => s.Rule == matrix.TechnicalRule && s.Company == matrix.Company)).Select(s => s).ToList();
                var result2 = set2;
                return ORMapList(result1.Union(result2).ToList());
            }
        }

        public static List<EntityModel.SAPHRFLINSRoleMatrix> getAllExistingBusinessRoles(long EndpointUserId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPHRFLINSRoleMatrixRepository(dataAccessTransaction).getExistingBusinessRole(EndpointUserId);
            }
        }

        public static List<EntityModel.SAPHRFLINSRoleMatrix> getRolesSharingSameRule(string businessRole, string company)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPHRFLINSRoleMatrixRepository(dataAccessTransaction).getRolesSharingSameRule(businessRole,   company);
            }
        }


        public static List<EntityModel.SAPHRFLINSRoleMatrix> GetAllExistingBusinessRolesOnlyPurchaseGroup(long EndpointUserId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var existingBusinessRole = new SAPHRFLINSRoleMatrixRepository(dataAccessTransaction).getExistingBusinessRole(EndpointUserId, null);
                return existingBusinessRole;
            }
        }
        public static Dictionary<long, string> GetBusinessRoles(string searchString, string module, string subModuleID, long UserID)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPHRFLINSRoleMatrixRepository(dataAccessTransaction).GetBusinessRole(searchString, module, subModuleID, UserID);
            }
        }

        public static SAPHRFLINSRoleMatrix GetFLINSMatrixByRequest(AdHocSAPHRFLINSRequestDetail requestDetail)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new SAPHRFLINSRoleMatrixRepository(dataAccessTransaction).
                        GetFLINSMatrixByRequest(requestDetail.Entitlement, requestDetail.Company));
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static SAPHRFLINSRoleMatrix GetByRoleCode(string roleCode)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new SAPHRFLINSRoleMatrixRepository(dataAccessTransaction).GetByRoleCode(roleCode));
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool checkBusinessRoleHasSupplementaryRole(string BusinessRole)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPHRFLINSRoleMatrixRepository(dataAccessTransaction).checkBusinessRoleHasSupplementaryRole(BusinessRole);
            }
        }

        public static List<SAPHRFLINSRoleMatrix> GetByBussinessRoleAndCompany(string bussinessRole, string company)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMapList(new SAPHRFLINSRoleMatrixRepository(dataAccessTransaction).GetByBussinessRoleAndCompany(bussinessRole, company));
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SAPHRFLINSRoleMatrix> GetByBusinessRole(string bussinessRole, string company, bool skipMainRole = false)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (!skipMainRole)
                    {
                        return ORMapList(new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction)
                        .Get(item => item.BusinessRole == bussinessRole && item.Company == company).ToList());
                    }
                    else
                    {
                        return ORMapList(new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction)
                        .Get(item => item.BusinessRole == bussinessRole && item.Company == company && item.MainSupp != "main").ToList());
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SAPHRFLINSRoleMatrix> GetByBusinessRolesForRetain(string bussinessRole, string company)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMapList(new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction)
                    .Get(item => item.BusinessRole == bussinessRole && item.Company == company && item.MainSupp != "main").ToList());
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SAPHRFLINSRoleMatrix> GetByBusinessRole(string bussinessRole, bool skipMainRole)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (!skipMainRole)
                    {
                        return ORMapList(new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction)
                            .Get(item => item.BusinessRole == bussinessRole).ToList());
                    }
                    else
                    {
                        return ORMapList(new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction)
                            .Get(item => item.BusinessRole == bussinessRole && item.MainSupp != "main").ToList());
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void Add(SAPHRFLINSRoleMatrix obj)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).Insert(obj.ORUnMap());
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
                    new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).ExecuteQuery("Truncate table DI_SAP_HRFLINSRoleMatrix");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static void ManualUpdate(SAPHRFLINSRoleMatrix obj)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                dataAccessTransaction.User = obj.LastModifiedBy;
                new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).Update(obj.ORUnMap());
                dataAccessTransaction.Commit();
            }
        }
        public static SAPHRFLINSRoleMatrix GetSAPHRFLINSRoleMatrixbyObject(SAPHRFLINSRoleMatrix obj)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).Get(sap => sap.Company == obj.Company
                    && sap.CostCenter == obj.CostCenter && sap.PurchasingOrg == obj.PurchasingOrg && sap.PurchasingGrp == obj.PurchasingGrp && sap.Plant == obj.Plant
                    && sap.MRPControl == obj.MRPControl && sap.StorageLocation == obj.StorageLocation && sap.TechnicalRule == obj.TechnicalRule && sap.BusinessRole == obj.BusinessRole).FirstOrDefault());
            }
        }

        public static bool IsExist(SAPHRFLINSRoleMatrix obj)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).Any(sap => sap.Company == obj.Company
                   && sap.CostCenter == obj.CostCenter && sap.PurchasingOrg == obj.PurchasingOrg && sap.PurchasingGrp == obj.PurchasingGrp && sap.Plant == obj.Plant
                   && sap.MRPControl == obj.MRPControl && sap.StorageLocation == obj.StorageLocation && sap.TechnicalRule == obj.TechnicalRule && sap.BusinessRole == obj.BusinessRole);
            }
        }
        public static List<SAPHRFLINSRoleMatrix> IsEmployeeConfiguredAsApprover(string employeeId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).Get(sap => sap.Level1Approver == employeeId
                        || sap.Level2Approver == employeeId
                        || sap.Level3Approver == employeeId).ToList());
            }
        }
        public static List<SAPHRFLINSRoleMatrix> GetAllSAPHRFLINSRoleMatrices(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                    }
                }
            }
            catch { throw; }
        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
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
