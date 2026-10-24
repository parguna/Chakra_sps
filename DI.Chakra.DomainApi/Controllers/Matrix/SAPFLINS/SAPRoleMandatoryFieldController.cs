using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using DI.Chakra.DomainModel;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPRoleMandatoryField")]
    public class SAPRoleMandatoryFieldController : BaseController<DomainModel.SAPRoleMandatoryField, EntityModel.SAPRoleMandatoryField, Dto.SAPRoleMandatoryField>
    {

        [HttpGet]
        [Route("MandatoryFieldIdentification")]
        public Dictionary<string, string> MandatoryFieldIdentification(string businessRole)
        {
            return DomainModel.SAPRoleMandatoryField.MandatoryFieldIdentification(businessRole);
        }

        [HttpGet]
        [Route("checkBusinessRoleCompanyOnlyMandatory")]
        public bool checkBusinessRoleCompanyOnlyMandatory(string BusinessRole)
        {
            return DomainModel.SAPRoleMandatoryField.checkBusinessRoleCompanyOnlyMandatory(BusinessRole);
        }
        //[HttpGet]
        //[Route("GetAllExistingBusinessRolesOnlyPurchaseGroup")]
        //public List<EntityModel.SAPHRFLINSRoleMatrix> GetAllExistingBusinessRolesOnlyPurchaseGroup(long EndpointUserId)
        //{
        //    return  DomainModel.SAPHRFLINSRoleMatrix.GetAllExistingBusinessRolesOnlyPurchaseGroup(EndpointUserId);
        //}
        [HttpPost]
        [Route("checkBusinessRule")]
        public Dto.SAPFlinsMatrixBusinessRules checkBusinessRule(long EndpointUserId, string BusinessRole,string Company, [FromBody] List<Dto.AdHocSAPHRFLINSRequestDetail> addedBusinessRole)
        {
            string costCenterTechnicalRule = string.Empty;
            var retValue = new Dto.SAPFlinsMatrixBusinessRules();
            var donotConsiderCompany = string.IsNullOrWhiteSpace(Company);
            retValue.IsCompanyMandatory = DomainModel.SAPRoleMandatoryField.checkBusinessRoleCompanyOnlyMandatory(BusinessRole);
            retValue.HasSupplimentaryRole = DomainModel.SAPHRFLINSRoleMatrix.checkBusinessRoleHasSupplementaryRole(BusinessRole);
            var currentBusinessRoles = DomainModel.SAPHRFLINSRoleMatrix.GetByBusinessRole(BusinessRole, false);
            var existingBusinessRole = DomainModel.SAPHRFLINSRoleMatrix.getAllExistingBusinessRoles(EndpointUserId);
            
            foreach (var b in addedBusinessRole)
            {
                var temp = new List<EntityModel.SAPHRFLINSRoleMatrix>();
                existingBusinessRole.ForEach(br =>
                {
                    if (br.BusinessRole == b.Entitlement && (string.IsNullOrWhiteSpace(b.Company) || br.Company == b.Company))
                    {
                        temp.Add(br);
                    }
                });

                temp.ForEach(b1 => existingBusinessRole.Remove(b1));
            }


            var mainsuppGroupByCompany = (from grp in existingBusinessRole.Where(b => b.MainSupp.Trim().Equals("main", System.StringComparison.OrdinalIgnoreCase)).Select(b => b) group grp by   grp.BusinessRole  into newGroup orderby newGroup.Key select newGroup).ToList();

            var costCenterBusinessRole = existingBusinessRole.FirstOrDefault(b => {
                return b.BusinessRole == BusinessRole && (donotConsiderCompany || b.Company == Company) && b.CostCenter.Trim().Equals("X", System.StringComparison.OrdinalIgnoreCase) && b.MainSupp.Trim().Equals("Supplement", System.StringComparison.OrdinalIgnoreCase);
            });

            if (costCenterBusinessRole != null)
            {
                costCenterTechnicalRule = costCenterBusinessRole.TechnicalRule;
            }

                existingBusinessRole = existingBusinessRole.Where(b => {
                return !(b.BusinessRole == BusinessRole && (donotConsiderCompany || b.Company == Company));
            }).Select(b=>b).ToList();

            


            retValue.CanClearAllPurchaseGrp = !(existingBusinessRole.Any(b => !string.IsNullOrWhiteSpace(b.PurchasingGrp) && b.PurchasingGrp.Trim().Equals("X", System.StringComparison.InvariantCultureIgnoreCase)  && !b.BusinessRole.Trim().Equals(BusinessRole, System.StringComparison.InvariantCultureIgnoreCase)))  && mainsuppGroupByCompany.Any(g => g.Key == BusinessRole && g.Count() <= 1);
                                                       

            retValue.ShouldClearAllPurchaseGroup = existingBusinessRole.All(b => 
                                                         string.IsNullOrWhiteSpace(b.PurchasingGrp) || !b.PurchasingGrp.Trim().Equals("X", System.StringComparison.InvariantCultureIgnoreCase) )
                                                      &&  mainsuppGroupByCompany.Any(g => g.Key == BusinessRole && g.Count() <= 1) ;


            //retValue.CanClearAllCostCenter = mainsuppGroupByCompany.All(b => b.Key.Count() == 1) && !existingBusinessRole.Where(b => b.MainSupp.Trim().Equals("Supplement", System.StringComparison.OrdinalIgnoreCase)).Any(b => !string.IsNullOrWhiteSpace(b.CostCenter) && b.CostCenter.Trim() == "X");

            //retValue.ShouldClearAllCostcenter = existingBusinessRole.Where(b => b.MainSupp.Trim().Equals("Supplement", System.StringComparison.OrdinalIgnoreCase)).All(b => string.IsNullOrWhiteSpace(b.CostCenter) || b.CostCenter.Trim() != "X" && mainsuppGroupByCompany.All(grp => grp.Key == BusinessRole && grp.Key.Count() <= 1));



            //retValue.CanClearAllPurchaseGrp = !existingBusinessRole.Any(b => !string.IsNullOrWhiteSpace(b.PurchasingGrp) && b.PurchasingGrp.Trim().Equals("X", System.StringComparison.InvariantCultureIgnoreCase));

            //retValue.ShouldClearAllPurchaseGroup = existingBusinessRole.All(b => string.IsNullOrWhiteSpace(b.PurchasingGrp) || !b.PurchasingGrp.Trim().Equals("X", System.StringComparison.InvariantCultureIgnoreCase));

            retValue.CanClearAllCostCenter = !existingBusinessRole.Any(b => !string.IsNullOrWhiteSpace(b.CostCenter) && 
                                                                             b.CostCenter.Trim() == "X" && 
                                                                             (!donotConsiderCompany && b.Company == Company) && 
                                                                             b.TechnicalRule == costCenterTechnicalRule);

            retValue.ShouldClearAllCostcenter = existingBusinessRole.All(b =>
            {
                if (string.IsNullOrWhiteSpace(b.CostCenter))
                {
                    return true;
                }
                else if (b.CostCenter.Trim() != "X")
                {
                    return true;
                }
                else { 
                    var flag =  !(b.CostCenter.Trim() == "X" && b.TechnicalRule == costCenterTechnicalRule && (donotConsiderCompany || b.Company == Company));
                    return flag; 
                }

            });




            if (currentBusinessRoles.Any(b => !string.IsNullOrWhiteSpace(b.StorageLocation)))
            {
                var locations = currentBusinessRoles.Select(b => b.StorageLocation);
                retValue.CanClearAllStorageLocation = !existingBusinessRole.Any(b => !string.IsNullOrWhiteSpace(b.StorageLocation) && locations.Contains(b.StorageLocation));
                retValue.ShouldClearAllStorageLocation = existingBusinessRole.All(b => string.IsNullOrWhiteSpace(b.StorageLocation) || !locations.Contains(b.StorageLocation));
            }
            else
            {
                retValue.CanClearAllStorageLocation = true;
                retValue.ShouldClearAllStorageLocation = true;
            }

            if (currentBusinessRoles.Any(b => !string.IsNullOrWhiteSpace(b.Plant)))
            {
                var plants = currentBusinessRoles.Select(b => b.Plant);
                retValue.CanClearAllPlant = !existingBusinessRole.Any(b => !string.IsNullOrWhiteSpace(b.Plant) && plants.Contains(b.Plant));
                retValue.ShouldClearAllPlant = existingBusinessRole.All(b => string.IsNullOrWhiteSpace(b.Plant) || !plants.Contains(b.Plant));
            }
            else
            {
                retValue.CanClearAllPlant = true;
                retValue.ShouldClearAllPlant = true;
            }

            if (currentBusinessRoles.Any(b => !string.IsNullOrWhiteSpace(b.PurchasingOrg)))
            {
                var PurchasingOrgs = currentBusinessRoles.Select(b => b.PurchasingOrg);
                retValue.CanClearAllPurchaseOrg = !existingBusinessRole.Any(b => !string.IsNullOrWhiteSpace(b.PurchasingOrg) && PurchasingOrgs.Contains(b.Plant));
                retValue.ShouldClearAllPurchaseOrg = existingBusinessRole.All(b => string.IsNullOrWhiteSpace(b.PurchasingOrg) || !PurchasingOrgs.Contains(b.Plant));
            }
            else
            {
                retValue.CanClearAllPurchaseOrg = true;
                retValue.ShouldClearAllPurchaseOrg = true;
            }

            if (currentBusinessRoles.Any(b => !string.IsNullOrWhiteSpace(b.MRPControl)))
            {
                var MRPControllers = currentBusinessRoles.Select(b => b.PurchasingOrg);
                retValue.CanClearAllMRP = !existingBusinessRole.Any(b => !string.IsNullOrWhiteSpace(b.MRPControl) && MRPControllers.Contains(b.MRPControl));
                retValue.ShouldClearAllMRP = existingBusinessRole.All(b => string.IsNullOrWhiteSpace(b.MRPControl) || !MRPControllers.Contains(b.MRPControl));
            }
            else
            {
                retValue.CanClearAllMRP = true;
                retValue.ShouldClearAllMRP = true;
            }


            return retValue;
        }

        [HttpGet]
        [Route("GetAllSAPRoleMandatoryFields")]
        public List<Dto.SAPRoleMandatoryField> GetAllSAPRoleMandatoryFields(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SAPRoleMandatoryField.GetAllSAPRoleMandatoryFields(isValidRecords, isAllRecords));
        }
    }
}