using System;
using System.Collections.Generic;
using System.ServiceModel;
using Telerik.Web.UI;
namespace DI.Chakra.Web.Services
{
    [ServiceContract]
    interface IAutoCompleteDataService
    {
        [OperationContract]
          AutoCompleteBoxData GetAllReportMenu(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetSRMBusinesRoles(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetBulkSRMBusinesRoles(Dictionary<string, object> context); 

        [OperationContract]
        AutoCompleteBoxData GetAdhocLicenseInfo(Dictionary<string, object> context);


        [OperationContract]
        AutoCompleteBoxData GetLicenseInfoForBulkReq(Dictionary<string, object> context);


        [OperationContract]
        AutoCompleteBoxData GetO365Users(Dictionary<string, object> context);


      
        [OperationContract]
        AutoCompleteBoxData GetAllPositionEBS(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetExistingPositionEBS(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetAllRolesEBS(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetExistingRolesEBS(Dictionary<string, object> context);


        [OperationContract]
        AutoCompleteBoxData GetEntitlementGEMS(Dictionary<string, object> context);
        
        [OperationContract]
        AutoCompleteBoxData GetAllPositionMSSL(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetExistingPositionMSSL(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetExistingRolesMSSL(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetAllRolesMSSL(Dictionary<string, object> context);
        
        [OperationContract]
        AutoCompleteBoxData GetBusinessRoleFromSAPGRCRole(RadAutoCompleteContext context);

        [OperationContract]
        AutoCompleteBoxData GetBusinessRoleFromSAPPIRole(RadAutoCompleteContext context);

        [OperationContract]
        AutoCompleteBoxData GetBusinessRoleFromFlinsBiRole(RadAutoCompleteContext context);

        [OperationContract]
        AutoCompleteBoxData GetBulkBusinessRole(RadAutoCompleteContext context);

        [OperationContract]
        AutoCompleteBoxData GetUsersForBulkRequest(Dictionary<string, object> context); 

        [OperationContract]
        AutoCompleteBoxData GetADGroupsByCanRequest(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetBusinessRole(RadAutoCompleteContext context);

        [OperationContract]
        AutoCompleteBoxData GetDepartMents(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetSectionsByBranchCode(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetUnitsBySectionCode(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetDivisionsByDeptID(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetDisticntSectionFromUsers(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetCostCenterCodeAndTextByCompanyID(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetBusinessUnits(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetCostCenters(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetCompanies(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetInternalCompanies(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetExternalCompanies(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetLocations(Dictionary<string, object> context);


        [OperationContract]
        AutoCompleteBoxData GetRuleSetValues(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetActiveAndNonTerminatedUsers(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetApplicableApplications(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetSAPAuthorizationObjects(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetSapFields(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetSODPolicyDefinitions(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetADUserGroups(Dictionary<string, object> context);
        
        [OperationContract]
        AutoCompleteBoxData GetAllRiskLevel(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetAllSodExceptions(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetAllGroups(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetADOrganisationUnits(Dictionary<string, object> context);        
        [OperationContract]
        AutoCompleteBoxData GetCompliancePolicy(Dictionary<string, object> context);        
        [OperationContract]
        AutoCompleteBoxData GetAllEndPointsBySelecetedSystemTypes(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetAllAttestationEndPointsBySelecetedSystemTypes(Dictionary<string, object> context); 
        [OperationContract]
        AutoCompleteBoxData GetAllGroupsByEP(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetAllRolesByEP(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetAllSAPEntitlements(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetAllSAPRolesByEP(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetEndPoint(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetEndPointSystemTypes(Dictionary<string, object> context);
        //[OperationContract]
        //AutoCompleteBoxData GetInhouseSystemEntitlements(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetLNSOrganisationUnits(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetNonReportingUsersOfThisManagerID(Dictionary<string, object> context);        
        [OperationContract]
        AutoCompleteBoxData GetNOrganisations(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetNRulesetTypes(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetNRulesets(Dictionary<string, object> context);
        //[OperationContract]
        //AutoCompleteBoxData GetProvisioningFields(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetNDesignations(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetNEmployeeTypes(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetNEmployeeEventTypes(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetPortalGroups(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetPortalRoles(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetPosition(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetProvisioningWokflows(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData getReferenceUser(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData getReferencePermanentUser(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetReportingManager(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetReportingUsersOfThisManagerIDandAdmin(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetUsers(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetROUsers(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetRules(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetFunctions(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetSODConflictFunctions(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetUsersByGroup(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData setCostCenters(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData setOrganisatins(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetUserObjectNameList(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetInhouseSystemPermissionsByEndPointID(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetInhouseSystemCodeByEndPointID(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetEntitlementName(Dictionary<string, object> context);
        
        /// <summary>
        /// Auto complete Box Data for Risk score - written by Karthik
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [OperationContract]
        AutoCompleteBoxData GetAllRiskScore(Dictionary<string,object> context);

        /// <summary>
        /// Auto complete Box Data for Getting Function Items -written by Karthik
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [OperationContract]
        AutoCompleteBoxData GetFunctionItems(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetPageURLs(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetPages(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetMenus(Dictionary<string, object> context);

        /// <summary>
        /// Auto compete Box data for Getting RiskCodes
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [OperationContract]
        AutoCompleteBoxData GetAllRiskCodes(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetPasswordPolicyApplications(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetPasswordPolicies(Dictionary<string, object> context);


        [OperationContract]
        AutoCompleteBoxData GetOrganisationScopeUsers(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetUserPropertiesList(Dictionary<string, object> context);


        [OperationContract]
        AutoCompleteBoxData GetAllOrgUnits(Dictionary<string, object> context);


        [OperationContract]
        AutoCompleteBoxData GetFullTimeUsers(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetVendorUsers(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetUsersForSupervisor(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetEntitlementInHouse(Dictionary<string, object> context);
 
        [OperationContract]
        AutoCompleteBoxData GetIdmsGroups(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetIdmsRoles(Dictionary<string, object> context);
 
        [OperationContract]
        AutoCompleteBoxData GetFunctionsInhouse(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetEntitlementInHouseExisting(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetAllPermissionsExceptExisting(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetEpCeRoles(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetEpCeGroups(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetVPNApplications(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetVPNApplicationsForBulkRequest(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetAllTables(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetSkypeForBusinessResponseGroup(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetOrgCompanies(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetOrgDepartments(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetOrgDivisions(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetOrgBranches(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetOrgSections(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetOrgUnits(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetReportingUsersOfThisManagerIDandAdmin_New(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetAllEmployeesUnderRO(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetSkypeForBusinessReservedNumber(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetNovaJobByMatrixType(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetResponseGroupForBulkRequestService(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetGroupMembersbyUserId(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetUsersByGroupId(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetUsersForCustomReport(Dictionary<string, object> context);

        [OperationContract]
        AutoCompleteBoxData GetAllEndPoint(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetAllPositionSAPHANA(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetExistingPositionSAPHANA(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetAllRolesSAPHANA(Dictionary<string, object> context);
        [OperationContract]
        AutoCompleteBoxData GetExistingRolesSAPHANA(Dictionary<string, object> context);
    }
}
