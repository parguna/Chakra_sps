using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Logger;

namespace DI.Chakra.Repository
{
    public class SAPHRFLINSRoleMatrixRepository : RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>
    {
        public SAPHRFLINSRoleMatrixRepository(DataAccessTransaction transaction) : base(transaction)
        {
        }


        public Dictionary<long, string> GetBusinessRole(string searchString, string module, string subModuleID, long UserID)
        {
            try
            {
                IList<string> listofEligibilityRules = ListofEligibiltyRules(UserID);
                var result = CurrentContext.SAPHRFLINSRoleMatrix.Where(item => listofEligibilityRules.Contains(item.Remarks) && (string.IsNullOrEmpty(searchString) ? 1 == 1 : item.BusinessRole.Contains(searchString)) && item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now &&
                        (string.IsNullOrEmpty(module) ? 1 == 1 : item.Module == module) && (string.IsNullOrEmpty(subModuleID) ? 1 == 1 : item.SubModule == subModuleID))
                        .Select(x => new { Key = x.Id, Value = x.BusinessRole })
                        .Distinct().ToDictionary(o => o.Key, o => o.Value);
                Dictionary<long, string> data = new Dictionary<long, string>();
                foreach (var items in result)
                {
                    if (!data.ContainsValue(items.Value))
                    {
                        data.Add(items.Key, items.Value);
                    }
                }
                return data;

            }
            catch
            {
                throw;
            }
        }

        private IList<string> ListofEligibiltyRules(long UserID)
        {
            IList<string> listofEligibilityRules = new List<string>();
            try
            {

                listofEligibilityRules = (from Item in CurrentContext.SAPRoleMatrix
                                          where (Item.ValidFrom <= DateTime.Now && Item.ValidTo >= DateTime.Now &&
                                          (string.IsNullOrEmpty(Item.Company) && string.IsNullOrEmpty(Item.PersonnelArea) && string.IsNullOrEmpty(Item.PersonnelSubArea) && string.IsNullOrEmpty(Item.Position) && string.IsNullOrEmpty(Item.OrgUnit) && string.IsNullOrEmpty(Item.CostCenter)))
                                          select Item.EligibilityRule).Distinct().ToList();
                var Result = (from Item in CurrentContext.SAPRoleMatrix
                              where (Item.ValidFrom <= DateTime.Now && Item.ValidTo >= DateTime.Now) &&
                         (
                         Item.Company == (CurrentContext.User.Where(c => c.Id == UserID).FirstOrDefault().CompanyCode) ||
                         Item.PersonnelArea == (CurrentContext.User.Where(c => c.Id == UserID).FirstOrDefault().DepartmentCode) ||
                         Item.PersonnelSubArea == (CurrentContext.User.Where(c => c.Id == UserID).FirstOrDefault().BranchCode) ||
                         Item.Position == (CurrentContext.User.Where(c => c.Id == UserID).FirstOrDefault().PositionCode) ||
                         Item.OrgUnit == (CurrentContext.User.Where(c => c.Id == UserID).FirstOrDefault().OrganisationCode) ||
                         Item.CostCenter == (CurrentContext.User.Where(c => c.Id == UserID).FirstOrDefault().CostCenter))
                              select Item.EligibilityRule).Distinct().ToList();
                foreach (string value in Result)
                {
                    var check = listofEligibilityRules.Count(v => v == value);
                    if (check == 0)
                    {
                        listofEligibilityRules.Add(value);
                    }
                }


                return listofEligibilityRules;
            }
            catch
            {
                return new List<string>();
            }
        }

        public IList<SAPHRFLINSRoleMatrix> BindSubModule(string moduleName, long UserID)
        {
            try
            {
                IList<string> listofEligibilityRules = ListofEligibiltyRules(UserID);
                var result = (from source in CurrentContext.SAPHRFLINSRoleMatrix where listofEligibilityRules.Contains(source.Remarks) && source.Module == moduleName && (source.ValidFrom <= DateTime.Now && source.ValidTo >= DateTime.Now) select source);
                IList<SAPHRFLINSRoleMatrix> data = new List<SAPHRFLINSRoleMatrix>();
                foreach (var items in result)
                {
                    var check = data.Count(v => v.SubModule == items.SubModule);
                    if (check == 0)
                    {
                        data.Add(items);
                    }
                }
                return data;
            }
            catch
            {
                throw;
            }
        }
        public IList<SAPHRFLINSRoleMatrix> BindModule(long userID)
        {
            try
            {
                IList<string> listofEligibilityRules = ListofEligibiltyRules(userID);
                var Matrices = (from Item in CurrentContext.SAPHRFLINSRoleMatrix
                                where listofEligibilityRules.Contains(Item.Remarks)
                                select Item).ToList();
                IList<SAPHRFLINSRoleMatrix> data = new List<SAPHRFLINSRoleMatrix>();
                foreach (var items in Matrices)
                {
                    var check = data.Count(v => v.Module == items.Module);
                    if (check == 0)
                    {
                        data.Add(items);
                    }
                }
                return data;

            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetCompany(string searchString, string businessRoleID, int EndpointUserId)
        {
            try
            {

                Dictionary<long, string> data = new Dictionary<long, string>();
                if (EndpointUserId == 0)
                {
                    var result = CurrentContext.SAPHRFLINSRoleMatrix.Where(item => item.BusinessRole == businessRoleID && (string.IsNullOrEmpty(searchString) ? 1 == 1 : item.Company.Contains(searchString)) && (item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now))
                            .Select(x => new { Key = x.Id, Value = x.Company })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
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
                    List<string> SapRoles = (from user in CurrentContext.SAPUserinfos
                                             where user.Id == EndpointUserId
                                             from role in user.SAPRoleInfo
                                             select role.ROLE_CODE).ToList();

                    var result = (from matrix in CurrentContext.SAPHRFLINSRoleMatrix
                                  where SapRoles.Contains(matrix.TechnicalRule) && matrix.MainSupp == "main" && matrix.BusinessRole == businessRoleID
                                  select matrix)
                            .Select(x => new { Key = x.Id, Value = x.Company })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);

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
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetPurchaseOrg(string searchString, string businessRoleID, string company, string costCenter, string purchaseGroup, string planet, string storeLocation, string mRPController, long EndpointUserId)
        {
            try
            {

                Dictionary<long, string> data = new Dictionary<long, string>();
                if (EndpointUserId == 0)
                {
                    var result = CurrentContext.SAPHRFLINSRoleMatrix.Where(item => (string.IsNullOrEmpty(searchString) ? 1 == 1 : item.PurchasingOrg.Contains(searchString)) && (item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now) &&
                                                                (string.IsNullOrEmpty(businessRoleID) || item.BusinessRole == businessRoleID) &&
                                                                 (string.IsNullOrEmpty(company) || item.Company == company) &&
                                                                 (string.IsNullOrEmpty(costCenter) || item.CostCenter == costCenter) &&
                                                                 (string.IsNullOrEmpty(purchaseGroup) || item.PurchasingGrp == purchaseGroup) &&
                                                                 (string.IsNullOrEmpty(planet) || item.Plant == planet) &&
                                                                 (string.IsNullOrEmpty(storeLocation) || item.StorageLocation == storeLocation) &&
                                                                 (string.IsNullOrEmpty(mRPController) || item.MRPControl == mRPController)

                        )
                            .Select(x => new { Key = x.Id, Value = x.PurchasingOrg })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
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
                    List<string> SapRoles = (from user in CurrentContext.SAPUserinfos
                                             where user.Id == EndpointUserId
                                             from role in user.SAPRoleInfo
                                             select role.ROLE_CODE).ToList();

                    var result = CurrentContext.SAPHRFLINSRoleMatrix.Where(item => (string.IsNullOrEmpty(searchString) ? 1 == 1 : item.PurchasingOrg.Contains(searchString)) && (item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now) &&
                                                (string.IsNullOrEmpty(businessRoleID) || item.BusinessRole == businessRoleID) &&
                                                    (string.IsNullOrEmpty(company) || item.Company == company) &&
                                                    (string.IsNullOrEmpty(costCenter) || item.CostCenter == costCenter) &&
                                                    (string.IsNullOrEmpty(purchaseGroup) || item.PurchasingGrp == purchaseGroup) &&
                                                    (string.IsNullOrEmpty(planet) || item.Plant == planet) &&
                                                    (string.IsNullOrEmpty(storeLocation) || item.StorageLocation == storeLocation) &&
                                                    (string.IsNullOrEmpty(mRPController) || item.MRPControl == mRPController &&
                                                    SapRoles.Contains(item.TechnicalRule) && item.MainSupp == "main"))
                            .Select(x => new { Key = x.Id, Value = x.PurchasingOrg })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
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
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetCostCenter(string searchString, string businessRoleID, string company, string purchaseOrg, string purchaseGroup, string planet, string storeLocation, string mRPController, string mandatorystring, long EndpointUserId)
        {
            try
            {

                var technicalRule = (from c in CurrentContext.SAPHRFLINSRoleMatrix
                                     where
                                          (c.ValidFrom <= DateTime.Now && c.ValidTo >= DateTime.Now) && c.CostCenter == "X" &&
                                         (string.IsNullOrEmpty(businessRoleID) || c.BusinessRole == businessRoleID) &&
                                        (string.IsNullOrEmpty(company) || c.Company == company) &&
                                        (string.IsNullOrEmpty(purchaseOrg) || c.PurchasingOrg == purchaseOrg) &&
                                        (string.IsNullOrEmpty(purchaseGroup) || c.PurchasingGrp == purchaseGroup) &&
                                        (string.IsNullOrEmpty(planet) || c.Plant == planet) &&
                                        (string.IsNullOrEmpty(storeLocation) || c.StorageLocation == storeLocation) &&
                                        (string.IsNullOrEmpty(mRPController) || c.MRPControl == mRPController)
                                     select c.TechnicalRule).FirstOrDefault();
                Dictionary<long, string> data = new Dictionary<long, string>();

                if (EndpointUserId == 0)
                {
                    var result = CurrentContext.SAPHRFLINSSuppRoleMatrix.Where(item => item.Rule == technicalRule && (string.IsNullOrEmpty(searchString) ? 1 == 1 : item.CostCenter.Contains(searchString))
                                     && (item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now) &&
                                            (mandatorystring == "Company" ? item.Company == company : 1 == 1) &&
                                            (mandatorystring == "PurchaseOrg" ? item.PurchasingOrg == purchaseOrg : 1 == 1) &&
                                            (mandatorystring == "PurchaseGroup" ? item.PurchasingGrp == purchaseGroup : 1 == 1) &&
                                            (mandatorystring == "Plant" ? item.Plant == planet : 1 == 1) &&
                                            (mandatorystring == "StorageLocation" ? item.StorageLocation == storeLocation : 1 == 1) &&
                                            (mandatorystring == "MRPControl" ? item.MRPControl == mRPController : 1 == 1)
                        )
                                .Select(x => new { Key = x.Id, Value = x.CostCenter })
                                .Distinct().ToDictionary(o => o.Key, o => o.Value);
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
                    var query = (from user in CurrentContext.SAPUserinfos
                                 where user.Id == EndpointUserId
                                 from role in user.SAPRoleInfo
                                 select role.ROLE_CODE);

                    List<string> SapRoles = query.ToList();


                    var query1 = (from suppMatrix in CurrentContext.SAPHRFLINSSuppRoleMatrix
                                  where SapRoles.Contains(suppMatrix.Role)
                                  where (string.IsNullOrEmpty(company) ? 1 == 1 : suppMatrix.Company == company)
                                  && suppMatrix.Rule == technicalRule && suppMatrix.CostCenter != string.Empty
                                  && (string.IsNullOrEmpty(searchString) ? 1 == 1 : suppMatrix.CostCenter.Contains(searchString))
                                  && (suppMatrix.ValidFrom <= DateTime.Now && suppMatrix.ValidTo >= DateTime.Now)
                                  select suppMatrix
                                    );
                    var result = query1.Select(x => new { Key = x.Id, Value = x.CostCenter })
                                .Distinct().ToDictionary(o => o.Key, o => o.Value);

                    foreach (var items in result)
                    {
                        if (!data.ContainsValue(items.Value))
                        {
                            data.Add(items.Key, items.Value);
                        }
                    }
                }
                if(data != null && data.Count > 0)
                {
                    Dictionary<long, string> result = new Dictionary<long, string>();
                    foreach (var item in data.OrderBy(i => i.Value))
                    {
                        result.Add(item.Key, item.Value);
                    }
                    data = result;
                }
                return data;

            }
            catch
            {
                throw;
            }
        }


        public Dictionary<long, string> GetOnlyPurchaseGroupTechnicalRole(string businessRoleID, string company, string costCenter, string purchaseOrg, string planet, string storeLocation, string mRPController, long EndpointUserId)
        {
            try
            {

                Dictionary<long, string> data = new Dictionary<long, string>();
                List<string> SapRoles = (from user in CurrentContext.SAPUserinfos
                                         where user.Id == EndpointUserId
                                         from role in user.SAPRoleInfo
                                         select role.ROLE_CODE).ToList();


                var technicalRule = (from c in CurrentContext.SAPHRFLINSRoleMatrix
                                     where (c.ValidFrom <= DateTime.Now && c.ValidTo >= DateTime.Now) && c.PurchasingGrp == "X" &&
                                     (string.IsNullOrEmpty(businessRoleID) || c.BusinessRole == businessRoleID) &&
                                        (string.IsNullOrEmpty(company) || c.Company == company) &&
                                        (string.IsNullOrEmpty(costCenter) || c.CostCenter == costCenter) &&
                                        (string.IsNullOrEmpty(purchaseOrg) || c.PurchasingOrg == purchaseOrg) &&
                                        (string.IsNullOrEmpty(planet) || c.Plant == planet) &&
                                        (string.IsNullOrEmpty(storeLocation) || c.StorageLocation == storeLocation) &&
                                        (string.IsNullOrEmpty(mRPController) || c.MRPControl == mRPController)
                                     select c.TechnicalRule).FirstOrDefault();

                var result = (from suppMatrix in CurrentContext.SAPHRFLINSSuppRoleMatrix
                              where SapRoles.Contains(suppMatrix.Role)
                              where suppMatrix.PurchasingGrp != string.Empty
                              && (suppMatrix.ValidFrom <= DateTime.Now && suppMatrix.ValidTo >= DateTime.Now)
                              select suppMatrix
                                 )
                            .Select(x => new { Key = x.Id, Value = x.Role })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                foreach (var items in result)
                {
                    if (!data.ContainsValue(items.Value))
                    {
                        data.Add(items.Key, items.Value);
                    }
                }

                return data;

            }
            catch
            {
                throw;
            }
        }




        public Dictionary<long, string> GetPurchaseGroup(string searchString, string businessRoleID, string company, string costCenter, string purchaseOrg, string planet, string storeLocation, string mRPController, string mandatorystring, long EndpointUserId)
        {
            try
            {

                Dictionary<long, string> data = new Dictionary<long, string>();
                if (EndpointUserId == 0)
                {
                    var technicalRule = (from c in CurrentContext.SAPHRFLINSRoleMatrix
                                         where (c.ValidFrom <= DateTime.Now && c.ValidTo >= DateTime.Now) && c.PurchasingGrp == "X" &&
                                         (string.IsNullOrEmpty(businessRoleID) || c.BusinessRole == businessRoleID) &&
                                            (string.IsNullOrEmpty(company) || c.Company == company) &&
                                            (string.IsNullOrEmpty(costCenter) || c.CostCenter == costCenter) &&
                                            (string.IsNullOrEmpty(purchaseOrg) || c.PurchasingOrg == purchaseOrg) &&
                                            (string.IsNullOrEmpty(planet) || c.Plant == planet) &&
                                            (string.IsNullOrEmpty(storeLocation) || c.StorageLocation == storeLocation) &&
                                            (string.IsNullOrEmpty(mRPController) || c.MRPControl == mRPController)
                                         select c.TechnicalRule).FirstOrDefault();
                    var result = CurrentContext.SAPHRFLINSSuppRoleMatrix.Where(item => item.Rule == technicalRule && (string.IsNullOrEmpty(searchString) ? 1 == 1 : item.PurchasingGrp.Contains(searchString)) && (item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now))
                                .Select(x => new { Key = x.Id, Value = x.PurchasingGrp })
                                .Distinct().ToDictionary(o => o.Key, o => o.Value);
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
                    List<string> SapRoles = (from user in CurrentContext.SAPUserinfos
                                             where user.Id == EndpointUserId
                                             from role in user.SAPRoleInfo
                                             select role.ROLE_CODE).ToList();


                    var technicalRule = (from c in CurrentContext.SAPHRFLINSRoleMatrix
                                         where (c.ValidFrom <= DateTime.Now && c.ValidTo >= DateTime.Now) && c.PurchasingGrp == "X" &&
                                         (string.IsNullOrEmpty(businessRoleID) || c.BusinessRole == businessRoleID) &&
                                            (string.IsNullOrEmpty(company) || c.Company == company) &&
                                            (string.IsNullOrEmpty(costCenter) || c.CostCenter == costCenter) &&
                                            (string.IsNullOrEmpty(purchaseOrg) || c.PurchasingOrg == purchaseOrg) &&
                                            (string.IsNullOrEmpty(planet) || c.Plant == planet) &&
                                            (string.IsNullOrEmpty(storeLocation) || c.StorageLocation == storeLocation) &&
                                            (string.IsNullOrEmpty(mRPController) || c.MRPControl == mRPController)
                                         select c.TechnicalRule).FirstOrDefault();

                    var result = (from suppMatrix in CurrentContext.SAPHRFLINSSuppRoleMatrix
                                  where SapRoles.Contains(suppMatrix.Role)
                                  where suppMatrix.PurchasingGrp != string.Empty
                                  && (string.IsNullOrEmpty(searchString) ? 1 == 1 : suppMatrix.PurchasingGrp.Contains(searchString))
                                  && (suppMatrix.ValidFrom <= DateTime.Now && suppMatrix.ValidTo >= DateTime.Now)
                                  select suppMatrix
                                     )
                                .Select(x => new { Key = x.Id, Value = x.PurchasingGrp })
                                .Distinct().ToDictionary(o => o.Key, o => o.Value);
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
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetPlanet(string searchString, string businessRoleID, string company, string costCenter, string purchaseGroup, string purchaseOrg, string storeLocation, string mRPController, long EndpointUserId)
        {
            try
            {

                Dictionary<long, string> data = new Dictionary<long, string>();
                if (EndpointUserId == 0)
                {

                    var result = CurrentContext.SAPHRFLINSRoleMatrix.Where(item => (string.IsNullOrEmpty(searchString) ? 1 == 1 : item.Plant.Contains(searchString)) && (item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now) &&
                        (string.IsNullOrEmpty(businessRoleID) || item.BusinessRole == businessRoleID) &&
                                                                 (string.IsNullOrEmpty(company) || item.Company == company) &&
                                                                 (string.IsNullOrEmpty(costCenter) || item.CostCenter == costCenter) &&
                                                                 (string.IsNullOrEmpty(purchaseGroup) || item.PurchasingGrp == purchaseGroup) &&
                                                                 (string.IsNullOrEmpty(purchaseOrg) || item.PurchasingOrg == purchaseOrg) &&
                                                                 (string.IsNullOrEmpty(storeLocation) || item.StorageLocation == storeLocation) &&
                                                                 (string.IsNullOrEmpty(mRPController) || item.MRPControl == mRPController)
                        )
                            .Select(x => new { Key = x.Id, Value = x.Plant })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
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
                    List<string> SapRoles = (from user in CurrentContext.SAPUserinfos
                                             where user.Id == EndpointUserId
                                             from role in user.SAPRoleInfo
                                             select role.ROLE_CODE).ToList();

                    var result = (from matrix in CurrentContext.SAPHRFLINSRoleMatrix
                                  where SapRoles.Contains(matrix.TechnicalRule)
                                  where matrix.BusinessRole == businessRoleID && matrix.Plant != String.Empty && matrix.Company == company && matrix.MainSupp == "main"
                                  select matrix)
                     .Select(x => new { Key = x.Id, Value = x.Plant })
                     .Distinct().ToDictionary(o => o.Key, o => o.Value);
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
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetStoreLocations(string searchString, string businessRoleID, string company, string costCenter, string purchaseGroup, string planet, string purchaseOrg, string mRPController, long EndpointUserId)
        {
            try
            {

                Dictionary<long, string> data = new Dictionary<long, string>();
                if (EndpointUserId == 0)
                {
                    var result = CurrentContext.SAPHRFLINSRoleMatrix.Where(item => (string.IsNullOrEmpty(searchString) ? 1 == 1 : item.StorageLocation.Contains(searchString)) && (item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now) &&
                        (string.IsNullOrEmpty(businessRoleID) || item.BusinessRole == businessRoleID) &&
                                                                 (string.IsNullOrEmpty(company) || item.Company == company) &&
                                                                 (string.IsNullOrEmpty(costCenter) || item.CostCenter == costCenter) &&
                                                                 (string.IsNullOrEmpty(purchaseGroup) || item.PurchasingGrp == purchaseGroup) &&
                                                                 (string.IsNullOrEmpty(planet) || item.Plant == planet) &&
                                                                 (string.IsNullOrEmpty(purchaseOrg) || item.PurchasingOrg == purchaseOrg) &&
                                                                 (string.IsNullOrEmpty(mRPController) || item.MRPControl == mRPController)
                        )
                            .Select(x => new { Key = x.Id, Value = x.StorageLocation })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
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
                    List<string> SapRoles = (from user in CurrentContext.SAPUserinfos
                                             where user.Id == EndpointUserId
                                             from role in user.SAPRoleInfo
                                             select role.ROLE_CODE).ToList();

                    var result = CurrentContext.SAPHRFLINSRoleMatrix.Where(item => (string.IsNullOrEmpty(searchString) ? 1 == 1 : item.StorageLocation.Contains(searchString)) && (item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now) &&
                     (string.IsNullOrEmpty(businessRoleID) || item.BusinessRole == businessRoleID) &&
                                                              (string.IsNullOrEmpty(company) || item.Company == company) &&
                                                              (string.IsNullOrEmpty(costCenter) || item.CostCenter == costCenter) &&
                                                              (string.IsNullOrEmpty(purchaseGroup) || item.PurchasingGrp == purchaseGroup) &&
                                                              (string.IsNullOrEmpty(planet) || item.Plant == planet) &&
                                                              (string.IsNullOrEmpty(purchaseOrg) || item.PurchasingOrg == purchaseOrg) &&
                                                              (string.IsNullOrEmpty(mRPController) || item.MRPControl == mRPController) &&
                                                              SapRoles.Contains(item.TechnicalRule) && item.MainSupp == "main"

                     )
                         .Select(x => new { Key = x.Id, Value = x.StorageLocation })
                         .Distinct().ToDictionary(o => o.Key, o => o.Value);
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
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetMRPController(string searchString, string businessRoleID, string company, string costCenter, string purchaseGroup, string planet, string storeLocation, string purchaseOrg, long EndpointUserId)
        {
            try
            {
                Dictionary<long, string> data = new Dictionary<long, string>();
                if (EndpointUserId == 0)
                {
                    var result = CurrentContext.SAPHRFLINSRoleMatrix.Where(item => (string.IsNullOrEmpty(searchString) ? 1 == 1 : item.MRPControl.Contains(searchString)) && (item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now) &&
                        (string.IsNullOrEmpty(businessRoleID) || item.BusinessRole == businessRoleID) &&
                                                                 (string.IsNullOrEmpty(company) || item.Company == company) &&
                                                                 (string.IsNullOrEmpty(costCenter) || item.CostCenter == costCenter) &&
                                                                 (string.IsNullOrEmpty(purchaseGroup) || item.PurchasingGrp == purchaseGroup) &&
                                                                 (string.IsNullOrEmpty(planet) || item.Plant == planet) &&
                                                                 (string.IsNullOrEmpty(storeLocation) || item.StorageLocation == storeLocation) &&
                                                                 (string.IsNullOrEmpty(purchaseOrg) || item.PurchasingOrg == purchaseOrg)
                        )
                            .Select(x => new { Key = x.Id, Value = x.MRPControl })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
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
                    List<string> SapRoles = (from user in CurrentContext.SAPUserinfos
                                             where user.Id == EndpointUserId
                                             from role in user.SAPRoleInfo
                                             select role.ROLE_CODE).ToList();

                    var result = CurrentContext.SAPHRFLINSRoleMatrix.Where(item => (string.IsNullOrEmpty(searchString) ? 1 == 1 : item.MRPControl.Contains(searchString)) && (item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now) &&
                    (string.IsNullOrEmpty(businessRoleID) || item.BusinessRole == businessRoleID) &&
                                                             (string.IsNullOrEmpty(company) || item.Company == company) &&
                                                             (string.IsNullOrEmpty(costCenter) || item.CostCenter == costCenter) &&
                                                             (string.IsNullOrEmpty(purchaseGroup) || item.PurchasingGrp == purchaseGroup) &&
                                                             (string.IsNullOrEmpty(planet) || item.Plant == planet) &&
                                                             (string.IsNullOrEmpty(storeLocation) || item.StorageLocation == storeLocation) &&
                                                             (string.IsNullOrEmpty(purchaseOrg) || item.PurchasingOrg == purchaseOrg) &&
                                                             SapRoles.Contains(item.TechnicalRule) && item.MainSupp == "main"
                    )
                        .Select(x => new { Key = x.Id, Value = x.MRPControl })
                        .Distinct().ToDictionary(o => o.Key, o => o.Value);
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
            catch
            {
                throw;
            }
        }

        public List<SAPHRFLINSRoleMatrix> getExistingBusinessRole(long EndpointUserId, string CostCenterPurchaseGroup = null)
        {
            List<string> SapRoles = (from user in CurrentContext.SAPUserinfos
                                     where user.Id == EndpointUserId
                                     from role in user.SAPRoleInfo
                                     select role.ROLE_CODE).ToList();

            var onlmainQuery = (from businessRole in CurrentContext.SAPHRFLINSRoleMatrix
                                where SapRoles.Contains(businessRole.TechnicalRule) && businessRole.MainSupp == "main"
                                select businessRole);

            var onlymain = onlmainQuery.ToList();

            var companyList = onlymain.Select(c => c.Company).ToList();
            var businessList = onlymain.Select(c => c.BusinessRole).ToList();

            var supplementaryRoleQuery = from SupplementbusinessRole in CurrentContext.SAPHRFLINSRoleMatrix
                                         join mainrole in onlmainQuery on new { b = SupplementbusinessRole.BusinessRole, c = SupplementbusinessRole.Company } equals
                                         new { b = mainrole.BusinessRole, c = mainrole.Company }
                                         where
                                         SupplementbusinessRole.MainSupp == "supplement"
                                         select SupplementbusinessRole;

            //var supplementaryRoles = from supplementary in CurrentContext.SAPHRFLINSSuppRoleMatrix where SapRoles.Contains(supplementary.Role)  select supplementary;



            var main_with_supplementry = (from supp in supplementaryRoleQuery
                                              //join supplementary in supplementaryRoles on supp.TechnicalRule equals supplementary.Rule 
                                          select supp).ToList();

            main_with_supplementry = main_with_supplementry.Union(onlymain).ToList();

            if (!string.IsNullOrWhiteSpace(CostCenterPurchaseGroup) && CostCenterPurchaseGroup.Trim().Equals("PurchaseGroup", StringComparison.OrdinalIgnoreCase))
            {
                main_with_supplementry = main_with_supplementry.Where(i => !string.IsNullOrWhiteSpace(i.PurchasingGrp) && i.PurchasingGrp.Equals("X")).ToList();
            }

            if (!string.IsNullOrWhiteSpace(CostCenterPurchaseGroup) && CostCenterPurchaseGroup.Trim().Equals("CostCenter", StringComparison.OrdinalIgnoreCase))
            {
                main_with_supplementry = main_with_supplementry.Where(i => !string.IsNullOrWhiteSpace(i.CostCenter) && i.CostCenter.Equals("X")).ToList();
            }


            return main_with_supplementry;
        }

        public List<string> GetHrFlinsTechnicalRolesByAdhocRequestObject(string costCenter, string companyCode, string businessRole, string purchasingGroup, string Plant, string MRPController, string StorageLocation, bool RetainCompanyRoles)
        {
            List<string> hrFlinsRoles = new List<string>();
            if (string.IsNullOrWhiteSpace(companyCode))
            {
                companyCode = string.Empty;
            }

            if (!RetainCompanyRoles)
            {
                SAPRoleMandatoryField mandatoryField = CurrentContext.SAPRoleMandatoryField
                    .FirstOrDefault(MandatoryField => MandatoryField.BusinessRole == businessRole);


                string SqlQuery = string.Empty;
                if (mandatoryField == null)
                {
                    hrFlinsRoles = CurrentContext.SAPHRFLINSRoleMatrix
                       .Where(RoleMatrix => RoleMatrix.BusinessRole == businessRole)
                       .Select(role => role.TechnicalRule).ToList();

                }
                //else if (mandatoryField.MandatoryField2 == string.Empty)
                else if (string.IsNullOrEmpty(mandatoryField.MandatoryField2) || (!string.IsNullOrEmpty(mandatoryField.MandatoryField2) && mandatoryField.MandatoryField2.Trim().Length == 0))
                {
                    var hrFlinsRoleMatrix = CurrentContext.SAPHRFLINSRoleMatrix
                       .Where(RoleMatrix => RoleMatrix.BusinessRole == businessRole && RoleMatrix.Company == companyCode);

                    List<string> hrFlinsRolesWithoutSupplementary = hrFlinsRoleMatrix
                                    .Where(RoleMatrix => RoleMatrix.CostCenter.Trim() == "" && RoleMatrix.PurchasingOrg.Trim() == "")
                                    .Select(role => role.TechnicalRule)
                                    .ToList();

                    List<SAPHRFLINSRoleMatrix> roleMatrixes = hrFlinsRoleMatrix.ToList();

                    List<string> hrFlinsSupplementaryRolesCostCenter = GetHrFlinsSupplementaryRoles(roleMatrixes, companyCode, costCenter, purchasingGroup, "CostCenter");
                    List<string> hrFlinsSupplementaryRolesPurchasingGroup = GetHrFlinsSupplementaryRoles(roleMatrixes, companyCode, costCenter, purchasingGroup, "PurchasingGroup");

                    hrFlinsRoles = hrFlinsRolesWithoutSupplementary.Union(hrFlinsSupplementaryRolesCostCenter).ToList();
                    hrFlinsRoles = hrFlinsRoles.Union(hrFlinsSupplementaryRolesPurchasingGroup).ToList();

                }
                else if (mandatoryField.MandatoryField2 == Constants.MandatoryField_CostCenter)
                {
                    var hrFlinsRoleMatrix = CurrentContext.SAPHRFLINSRoleMatrix
                        .Where(RoleMatrix => RoleMatrix.BusinessRole == businessRole && RoleMatrix.Company == companyCode);

                    List<string> hrFlinsRolesWithoutSupplementary = hrFlinsRoleMatrix
                                    .Where(RoleMatrix => RoleMatrix.CostCenter.Trim() == "" || RoleMatrix.CostCenter == null)
                                    .Select(role => role.TechnicalRule)
                                    .ToList();

                    List<string> hrFlinsSupplementaryRolesCostCenter = GetHrFlinsSupplementaryRoles(hrFlinsRoleMatrix.ToList(), companyCode, costCenter, purchasingGroup, "CostCenter");

                    hrFlinsRoles = hrFlinsRolesWithoutSupplementary.Union(hrFlinsSupplementaryRolesCostCenter).ToList();
                }
                else if (mandatoryField.MandatoryField2 == Constants.MandatoryField_PurchaseGroup)
                {
                    var hrFlinsRoleMatrix = CurrentContext.SAPHRFLINSRoleMatrix
                       .Where(RoleMatrix => RoleMatrix.BusinessRole == businessRole && RoleMatrix.Company == companyCode);

                    List<string> hrFlinsRolesWithoutSupplementary = hrFlinsRoleMatrix
                                    .Where(RoleMatrix => RoleMatrix.PurchasingGrp.Trim() == "" || RoleMatrix.PurchasingGrp == null)
                                    .Select(role => role.TechnicalRule)
                                    .ToList();

                    List<string> hrFlinsSupplementaryRolesPurchasingGroup = GetHrFlinsSupplementaryRoles(hrFlinsRoleMatrix.ToList(), companyCode, costCenter, purchasingGroup, "PurchaseGroup");

                    hrFlinsRoles = hrFlinsRolesWithoutSupplementary.Union(hrFlinsSupplementaryRolesPurchasingGroup).ToList();

                }

                else if (mandatoryField.MandatoryField2 == Constants.MandatoryField_Plant)
                {
                    string[] lstPlants = Plant.Split(',');

                    hrFlinsRoles = (from roleMatrix in CurrentContext.SAPHRFLINSRoleMatrix
                                    where (roleMatrix.BusinessRole == businessRole && roleMatrix.Company == companyCode)
                                    join plants in lstPlants
                                     on roleMatrix.Plant equals plants
                                    select roleMatrix.TechnicalRule).ToList();
                }
                else if (mandatoryField.MandatoryField2 == Constants.MandatoryField_StoreLocation)
                {
                    string[] lstStorageLocations = StorageLocation.Split(',');

                    hrFlinsRoles = (from roleMatrix in CurrentContext.SAPHRFLINSRoleMatrix
                                    where (roleMatrix.BusinessRole == businessRole && roleMatrix.Company == companyCode)
                                    join storageLocation in lstStorageLocations
                                     on roleMatrix.StorageLocation equals storageLocation
                                    select roleMatrix.TechnicalRule).ToList();
                }
                else if (mandatoryField.MandatoryField2 == Constants.MandatoryField_MRPController)
                {
                    string[] lstMrpController = MRPController.Split(',');

                    hrFlinsRoles = (from roleMatrix in CurrentContext.SAPHRFLINSRoleMatrix
                                    where (roleMatrix.BusinessRole == businessRole && roleMatrix.Company == companyCode)
                                    join mrpController in lstMrpController
                                     on roleMatrix.MRPControl equals mrpController
                                    select roleMatrix.TechnicalRule).ToList();
                }
            }
            else
            {
                SAPRoleMandatoryField mandatoryField = CurrentContext.SAPRoleMandatoryField
                   .FirstOrDefault(MandatoryField => MandatoryField.BusinessRole == businessRole);


                string SqlQuery = string.Empty;
                if (mandatoryField == null)
                {
                    hrFlinsRoles = CurrentContext.SAPHRFLINSRoleMatrix
                       .Where(RoleMatrix => RoleMatrix.BusinessRole == businessRole && RoleMatrix.MainSupp != "main")
                       .Select(role => role.TechnicalRule).ToList();

                }
                //else if (mandatoryField.MandatoryField2 == string.Empty)
                else if (string.IsNullOrEmpty(mandatoryField.MandatoryField2) || (!string.IsNullOrEmpty(mandatoryField.MandatoryField2) && mandatoryField.MandatoryField2.Trim().Length == 0))
                {
                    var hrFlinsRoleMatrix = CurrentContext.SAPHRFLINSRoleMatrix
                       .Where(RoleMatrix => RoleMatrix.BusinessRole == businessRole && RoleMatrix.Company == companyCode && RoleMatrix.MainSupp != "main");

                    List<string> hrFlinsRolesWithoutSupplementary = hrFlinsRoleMatrix
                                    .Where(RoleMatrix => RoleMatrix.CostCenter.Trim() == "" && RoleMatrix.PurchasingOrg.Trim() == "")
                                    .Select(role => role.TechnicalRule)
                                    .ToList();

                    List<SAPHRFLINSRoleMatrix> roleMatrixes = hrFlinsRoleMatrix.ToList();

                    List<string> hrFlinsSupplementaryRolesCostCenter = GetHrFlinsSupplementaryRoles(roleMatrixes, companyCode, costCenter, purchasingGroup, "CostCenter");
                    List<string> hrFlinsSupplementaryRolesPurchasingGroup = GetHrFlinsSupplementaryRoles(roleMatrixes, companyCode, costCenter, purchasingGroup, "PurchasingGroup");

                    hrFlinsRoles = hrFlinsRolesWithoutSupplementary.Union(hrFlinsSupplementaryRolesCostCenter).ToList();
                    hrFlinsRoles = hrFlinsRoles.Union(hrFlinsSupplementaryRolesPurchasingGroup).ToList();

                }
                else if (mandatoryField.MandatoryField2 == Constants.MandatoryField_CostCenter)
                {
                    var hrFlinsRoleMatrix = CurrentContext.SAPHRFLINSRoleMatrix
                        .Where(RoleMatrix => RoleMatrix.BusinessRole == businessRole && RoleMatrix.Company == companyCode && RoleMatrix.MainSupp != "main");

                    List<string> hrFlinsRolesWithoutSupplementary = hrFlinsRoleMatrix
                                    .Where(RoleMatrix => RoleMatrix.CostCenter.Trim() == "" || RoleMatrix.CostCenter == null)
                                    .Select(role => role.TechnicalRule)
                                    .ToList();

                    List<string> hrFlinsSupplementaryRolesCostCenter = GetHrFlinsSupplementaryRoles(hrFlinsRoleMatrix.ToList(), companyCode, costCenter, purchasingGroup, "CostCenter");

                    hrFlinsRoles = hrFlinsRolesWithoutSupplementary.Union(hrFlinsSupplementaryRolesCostCenter).ToList();
                }
                else if (mandatoryField.MandatoryField2 == Constants.MandatoryField_PurchaseGroup)
                {
                    var hrFlinsRoleMatrix = CurrentContext.SAPHRFLINSRoleMatrix
                       .Where(RoleMatrix => RoleMatrix.BusinessRole == businessRole && RoleMatrix.Company == companyCode && RoleMatrix.MainSupp != "main");

                    List<string> hrFlinsRolesWithoutSupplementary = hrFlinsRoleMatrix
                                    .Where(RoleMatrix => RoleMatrix.PurchasingGrp.Trim() == "" || RoleMatrix.PurchasingGrp == null)
                                    .Select(role => role.TechnicalRule)
                                    .ToList();

                    List<string> hrFlinsSupplementaryRolesPurchasingGroup = GetHrFlinsSupplementaryRoles(hrFlinsRoleMatrix.ToList(), companyCode, costCenter, purchasingGroup, "PurchaseGroup");

                    hrFlinsRoles = hrFlinsRolesWithoutSupplementary.Union(hrFlinsSupplementaryRolesPurchasingGroup).ToList();

                }

                else if (mandatoryField.MandatoryField2 == Constants.MandatoryField_Plant)
                {
                    string[] lstPlants = Plant.Split(',');

                    hrFlinsRoles = (from roleMatrix in CurrentContext.SAPHRFLINSRoleMatrix
                                    where (roleMatrix.BusinessRole == businessRole && roleMatrix.Company == companyCode && roleMatrix.MainSupp != "main")
                                    join plants in lstPlants
                                     on roleMatrix.Plant equals plants
                                    select roleMatrix.TechnicalRule).ToList();
                }
                else if (mandatoryField.MandatoryField2 == Constants.MandatoryField_StoreLocation)
                {
                    string[] lstStorageLocations = StorageLocation.Split(',');

                    hrFlinsRoles = (from roleMatrix in CurrentContext.SAPHRFLINSRoleMatrix
                                    where (roleMatrix.BusinessRole == businessRole && roleMatrix.Company == companyCode && roleMatrix.MainSupp != "main")
                                    join storageLocation in lstStorageLocations
                                     on roleMatrix.StorageLocation equals storageLocation
                                    select roleMatrix.TechnicalRule).ToList();
                }
                else if (mandatoryField.MandatoryField2 == Constants.MandatoryField_MRPController)
                {
                    string[] lstMrpController = MRPController.Split(',');

                    hrFlinsRoles = (from roleMatrix in CurrentContext.SAPHRFLINSRoleMatrix
                                    where (roleMatrix.BusinessRole == businessRole && roleMatrix.Company == companyCode && roleMatrix.MainSupp != "main")
                                    join mrpController in lstMrpController
                                     on roleMatrix.MRPControl equals mrpController
                                    select roleMatrix.TechnicalRule).ToList();
                }
            }

            return hrFlinsRoles.Distinct().ToList();
        }

        List<string> GetHrFlinsSupplementaryRoles(List<SAPHRFLINSRoleMatrix> roleMatrixes, string companyCode, string costCenter, string purchasingGroup,
                                                                                                    string MandatoryFieldName)
        {
            List<string> SupplementaryRoles = new List<string>();
            if (MandatoryFieldName == Constants.MandatoryField_CostCenter)
            {
                string[] lstCostCenter = costCenter?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (SAPHRFLINSRoleMatrix roleMatrix in roleMatrixes)
                {
                    if (roleMatrix.CostCenter != null && roleMatrix.CostCenter.Trim() == "X")
                    {
                        // company 
                        List<SAPHRFLINSSuppRoleMatrix> supplementaryRoles = CurrentContext.SAPHRFLINSSuppRoleMatrix
                                         .Where(suppRole => suppRole.Rule == roleMatrix.TechnicalRule && suppRole.Company == roleMatrix.Company)
                                         .ToList();

                        foreach (SAPHRFLINSSuppRoleMatrix supplementaryRole in supplementaryRoles)
                        {
                            if (supplementaryRole.CostCenter != null && lstCostCenter.FirstOrDefault(sRole => sRole.Trim() == supplementaryRole.CostCenter.Trim()) != null)
                            {
                                SupplementaryRoles.Add(supplementaryRole.Role);
                            }
                        }
                    }
                }
            }
            else if (MandatoryFieldName == Constants.MandatoryField_PurchaseGroup)
            {

                string[] lstPurchasingGroup = purchasingGroup.Split(',');

                foreach (SAPHRFLINSRoleMatrix roleMatrix in roleMatrixes)
                {
                    if (roleMatrix.PurchasingGrp != null && roleMatrix.PurchasingGrp.Trim() == "X")
                    {
                        List<SAPHRFLINSSuppRoleMatrix> supplementaryRoles = CurrentContext.SAPHRFLINSSuppRoleMatrix
                                         .Where(suppRole => suppRole.Rule == roleMatrix.TechnicalRule)
                                         .ToList();

                        foreach (SAPHRFLINSSuppRoleMatrix supplementaryRole in supplementaryRoles)
                        {
                            if (supplementaryRole.PurchasingGrp != null && lstPurchasingGroup.FirstOrDefault(sRole => sRole.Trim() == supplementaryRole.PurchasingGrp.Trim()) != null)
                            {
                                SupplementaryRoles.Add(supplementaryRole.Role);
                            }
                        }
                    }
                }
            }

            return SupplementaryRoles;
        }

        public SAPHRFLINSRoleMatrix GetFLINSMatrixByRequest(string role, string company)
        {
            try
            {
                return CurrentContext.SAPHRFLINSRoleMatrix.FirstOrDefault(item => item.BusinessRole == role &&
                       (item.Company ?? string.Empty) == (company ?? string.Empty) &&
                       item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now);
            }
            catch
            {
                throw;
            }
        }

        public SAPHRFLINSRoleMatrix GetByRoleCode(string roleCode)
        {
            try
            {
                return CurrentContext.SAPHRFLINSRoleMatrix.FirstOrDefault(item => item.TechnicalRule.Equals(roleCode, StringComparison.CurrentCultureIgnoreCase));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool checkBusinessRoleHasSupplementaryRole(string BusinessRole)
        {
            try
            {

                var rules = CurrentContext.SAPHRFLINSRoleMatrix.Where(item => item.BusinessRole.Equals(BusinessRole, StringComparison.CurrentCultureIgnoreCase)
                                    && item.MainSupp.Equals("supplement", StringComparison.CurrentCultureIgnoreCase)).Select(e=> e.TechnicalRule);

                var query = CurrentContext.SAPHRFLINSSuppRoleMatrix.Count(item => (item.Rule!=null || item.Rule !="") && rules.Contains(item.Rule));
                return query > 0;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public List<SAPHRFLINSRoleMatrix> getRolesSharingSameRule(string businessRole, string company)
        {
            try
            {
                var query =
      from role in CurrentContext.SAPHRFLINSRoleMatrix
      join otherRole in CurrentContext.SAPHRFLINSRoleMatrix on role.TechnicalRule equals otherRole.TechnicalRule
      where role.BusinessRole == businessRole && role.Company == company
      select otherRole;
                return query.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SAPHRFLINSRoleMatrix> GetByBussinessRoleAndCompany(string bussinessRole, string company)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(company))
                {
                    company = string.Empty;
                }

                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return CurrentContext.SAPHRFLINSRoleMatrix.Where(item => item.BusinessRole == bussinessRole &&
                       item.Company == company &&
                       (item.CostCenter ?? string.Empty) == string.Empty &&
                       (item.MRPControl ?? string.Empty) == string.Empty &&
                       (item.Plant ?? string.Empty) == string.Empty &&
                       (item.PurchasingGrp ?? string.Empty) == string.Empty &&
                       (item.PurchasingOrg ?? string.Empty) == string.Empty &&
                       (item.StorageLocation ?? string.Empty) == string.Empty &&
                       item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now).ToList();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<long, string> GetOnlyCostCenterTechnicalRole(string businessRoleID, string company, string costCenter, string purchaseOrg, string planet, string storeLocation, string mRPController, long EndpointUserId)
        {
            try
            {

                Dictionary<long, string> data = new Dictionary<long, string>();
                List<string> SapRoles = (from user in CurrentContext.SAPUserinfos
                                         where user.Id == EndpointUserId
                                         from role in user.SAPRoleInfo
                                         select role.ROLE_CODE).ToList();


                var technicalRule = (from c in CurrentContext.SAPHRFLINSRoleMatrix
                                     where (c.ValidFrom <= DateTime.Now && c.ValidTo >= DateTime.Now) && c.CostCenter == "X" &&
                                     (string.IsNullOrEmpty(businessRoleID) || c.BusinessRole == businessRoleID) &&
                                        (string.IsNullOrEmpty(company) || c.Company == company) &&
                                        // (string.IsNullOrEmpty(costCenter) || c.CostCenter == costCenter) &&
                                        (string.IsNullOrEmpty(purchaseOrg) || c.PurchasingOrg == purchaseOrg) &&
                                        (string.IsNullOrEmpty(planet) || c.Plant == planet) &&
                                        (string.IsNullOrEmpty(storeLocation) || c.StorageLocation == storeLocation) &&
                                        (string.IsNullOrEmpty(mRPController) || c.MRPControl == mRPController)
                                     select c.TechnicalRule).FirstOrDefault();

                var result = (from suppMatrix in CurrentContext.SAPHRFLINSSuppRoleMatrix
                              where SapRoles.Contains(suppMatrix.Role)
                              where suppMatrix.CostCenter != string.Empty
                              && (suppMatrix.ValidFrom <= DateTime.Now && suppMatrix.ValidTo >= DateTime.Now)
                              select suppMatrix
                                 )
                            .Select(x => new { Key = x.Id, Value = x.Role })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                foreach (var items in result)
                {
                    if (!data.ContainsValue(items.Value))
                    {
                        data.Add(items.Key, items.Value);
                    }
                }

                return data;

            }
            catch
            {
                throw;
            }
        }

    }
}
