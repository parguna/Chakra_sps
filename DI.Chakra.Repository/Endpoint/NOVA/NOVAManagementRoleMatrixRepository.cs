using DI.Chakra.EntityModel;
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
    public class ManagementMatrix
    {
        public string Position { get; set; }
        public string DepartmentCode { get; set; }
        public string NovaMatrixType { get; set; }
        public string SectionCode { get; set; }
        public string CompanyCode { get; set; }
    }
    public class NOVAManagementRoleMatrixRepository: RepositoryBase<NOVAManagementRoleMatrix>
    {
        public NOVAManagementRoleMatrixRepository(DataAccessTransaction transaction) : base(transaction)
        {
        }
        public Dictionary<long, string> GetManagementMatrixByActionType(long endpointId, long userId, string searchString, AdHocActionType actionType,string departmentCode)
        {
            var results = new Dictionary<long, string>();
            var user = CurrentContext.User.FirstOrDefault(i => i.Id == userId);
            var endPointAccount = CurrentContext.AccountReconcilations.FirstOrDefault(i => i.EndpointSystemId == endpointId && i.UserId == user.Id && i.IsLocked == false);

            switch (actionType)
            {
                case AdHocActionType.NewAccount:
                    {
                        var managementRoles = CurrentContext.NOVAManagementRoleMatrices.Where(i=>i.EndPointSystemId == endpointId && i.Department == departmentCode)
                           .Select(i => new
                           {
                               i.JobDescription,
                               i.Position,
                               i.Id
                           });
                        if (!string.IsNullOrWhiteSpace(searchString))
                        {
                            managementRoles = managementRoles.Where(i => i.Position.Contains(searchString));
                        }
                        
                        foreach (var managementRole in managementRoles)
                        {
                            results.Add(managementRole.Id, $"{managementRole.Position} - {managementRole.JobDescription}");
                        }
                    }
                    break;
                case AdHocActionType.AmendmentAdd:
                    {
                        //Take all the matrix by DepartmentCode
                        var ManagementRoleMatrices = CurrentContext.NOVAManagementRoleMatrices.Where(i => i.Department == departmentCode && i.ValidTo >= DateTime.Now
                                                        && i.EndPointSystemId == endpointId).ToList();
                        //Take exisiting managemend role assigned to user
                        var query = (from ManagementRoleMatrix in CurrentContext.NOVAManagementRoleMatrices
                                     join userRole in CurrentContext.SAPEamNovaUserRoleInfo on ManagementRoleMatrix.Department equals userRole.DepartmentBranch
                                     join entitlements in CurrentContext.EntitlementReconcilations on userRole.Id equals entitlements.EntitlementId
                                     where ManagementRoleMatrix.Department == departmentCode  
                                        && ManagementRoleMatrix.EndPointSystemId == endpointId
                                        && ManagementRoleMatrix.ValidTo >= DateTime.Now && ManagementRoleMatrix.Position == userRole.RoleName
                                        && userRole.NovaPositionMatrixType == Infrastructure.NovaMatrixType.Management
                                        && entitlements.EndpointSystemId == endpointId && entitlements.EndpointUserId == endPointAccount.EndpointUserId
                                        && entitlements.IsDeleted == false && entitlements.EntitlementType == 1
                                        && entitlements.Entitlement == userRole.RoleName && userRole.IsActive
                                     select ManagementRoleMatrix);
                        var exisitingManagementRoles = query.ToList();
                        ManagementRoleMatrices = ManagementRoleMatrices.Except(exisitingManagementRoles).ToList();
                        //Take managemenet roles requested
                        var exisitingRequestPositionQuery = $@"SELECT NovaMatrixType,DepartmentCode,Position, SectionCode, CompanyCode  FROM 
                                (
                                    SELECT 
                                    xmlNode.dataObject.value('RoleName[1]','VARCHAR(max)') AS Position,
                                    xmlNode.dataObject.value('DepartmentCode[1]','VARCHAR(max)') AS DepartmentCode,
                                    xmlNode.dataObject.value('NovaMatrixType[1]','VARCHAR(max)') AS NovaMatrixType,
                                    xmlNode.dataObject.value('SectionCode[1]','VARCHAR(max)') AS SectionCode,
                                    xmlNode.dataObject.value('CompanyCode[1]','VARCHAR(max)') AS CompanyCode
                                    FROM 
                                    (   SELECT AD.Id,Cast(ISNULL(EntitlementDetail,'') as xml)as XMLData FROM
                                        DI_M_AdHocAccessRequest AD inner join DI_M_AccessRequest A on A.AdHocAccessRequestId = Ad.Id
							            WHERE  (ISNULL(EntitlementDetail,''))<>'' AND AD.EndpointSystemId={endpointId} AND  A.EndpointSystemId={endpointId} AND AD.AdhocActionType={((int)actionType)} AND A.UserId={user.Id} AND A.ProgressStatus NOT IN(2,5,8,10)
                                    )AS AD CROSS APPLY XMLData.nodes('/AdHocNovaRequest/RequestDetails/AdHocNovaRequestDetails') xmlNode(dataObject)
                                ) AS PositionDetails  
                                  WHERE Position Is NOT NULL  AND NovaMatrixType='Management'";
                        
                        var exisitingRequests = CurrentContext.Database.SqlQuery<ManagementMatrix>(exisitingRequestPositionQuery).ToList();
                        
                        foreach (var exisitingRequest in exisitingRequests)
                        {
                            var dataExists = ManagementRoleMatrices.FirstOrDefault(i => i.Department == exisitingRequest.DepartmentCode && i.Position == exisitingRequest.Position);
                            if (dataExists != null)
                            {
                                ManagementRoleMatrices.Remove(dataExists);
                            }
                        }
                        
                        if (!string.IsNullOrWhiteSpace(searchString))
                        {
                            ManagementRoleMatrices = ManagementRoleMatrices.Where(i=>i.Position.Contains(searchString)).ToList();
                        }
                        
                        foreach (var managementRole in ManagementRoleMatrices)
                        {
                            results.Add(managementRole.Id, $"{managementRole.Position} - {managementRole.JobDescription}");
                        }
                    }
                    break;
                case AdHocActionType.AmendmentRemove:
                    {
                        var ManagementRoleMatrices = CurrentContext.NOVAManagementRoleMatrices.Where(i => i.Department == departmentCode && i.ValidTo >= DateTime.Now
                                                       && i.EndPointSystemId == endpointId).ToList();

                        var managementRoles = (from ManagementRoleMatrix in CurrentContext.NOVAManagementRoleMatrices
                                              from userRole in CurrentContext.SAPEamNovaUserRoleInfo
                                              from entitlements in CurrentContext.EntitlementReconcilations
                                              where ManagementRoleMatrix.Department == departmentCode
                                                 && ManagementRoleMatrix.EndPointSystemId == endpointId
                                                 && ManagementRoleMatrix.ValidTo >= DateTime.Now && ManagementRoleMatrix.Position == userRole.RoleName
                                                 && userRole.NovaPositionMatrixType == Infrastructure.NovaMatrixType.Management
                                                 && entitlements.EndpointSystemId == endpointId && entitlements.EndpointUserId == endPointAccount.EndpointUserId
                                                 && entitlements.IsDeleted == false && entitlements.EntitlementType == 1
                                                 && entitlements.Entitlement == userRole.RoleName  && userRole.IsActive
                                              select ManagementRoleMatrix).ToList();
                        ManagementRoleMatrices = ManagementRoleMatrices.Intersect(managementRoles).ToList();
                        var exisitingRequestPositionQuery = $@"SELECT NovaMatrixType,DepartmentCode,Position, SectionCode, CompanyCode  FROM 
                                (
                                    SELECT 
                                    xmlNode.dataObject.value('RoleName[1]','VARCHAR(max)') AS Position,
                                    xmlNode.dataObject.value('DepartmentCode[1]','VARCHAR(max)') AS DepartmentCode,
                                    xmlNode.dataObject.value('NovaMatrixType[1]','VARCHAR(max)') AS NovaMatrixType,
                                    xmlNode.dataObject.value('SectionCode[1]','VARCHAR(max)') AS SectionCode,
                                    xmlNode.dataObject.value('CompanyCode[1]','VARCHAR(max)') AS CompanyCode
                                    FROM 
                                    (   SELECT AD.Id,Cast(ISNULL(EntitlementDetail,'') as xml)as XMLData FROM
                                        DI_M_AdHocAccessRequest AD inner join DI_M_AccessRequest A on A.AdHocAccessRequestId = Ad.Id
							            WHERE  (ISNULL(EntitlementDetail,''))<>'' AND AD.EndpointSystemId={endpointId} AND  A.EndpointSystemId={endpointId} AND AD.AdhocActionType={((int)actionType)} AND A.UserId={user.Id} AND A.ProgressStatus NOT IN(2,5,8,10)
                                    )AS AD CROSS APPLY XMLData.nodes('/AdHocNovaRequest/RequestDetails/AdHocNovaRequestDetails') xmlNode(dataObject)
                                ) AS PositionDetails  
                                  WHERE Position Is NOT NULL  AND NovaMatrixType='Management'";

                        var exisitingRequests = CurrentContext.Database.SqlQuery<ManagementMatrix>(exisitingRequestPositionQuery).ToList();
                        if (!string.IsNullOrWhiteSpace(searchString))
                        {
                            ManagementRoleMatrices = ManagementRoleMatrices.Where(i=>i.Position.Contains(searchString)).ToList();
                        }
                        foreach (var exisitingRequest in exisitingRequests)
                        {
                            var dataExists = managementRoles.FirstOrDefault(i => i.Department == exisitingRequest.DepartmentCode && i.Position == exisitingRequest.Position);
                            if (dataExists != null && managementRoles.Count() > 0)
                            {
                                ManagementRoleMatrices.Remove(dataExists);
                            }
                        }
                        foreach (var managementRole in ManagementRoleMatrices)
                        {
                            results.Add(managementRole.Id, $"{managementRole.Position} - {managementRole.JobDescription}");
                        }
                    }
                    break;
                default:
                    break;
            }
            return results;
        }
        public List<string> GetDapartment(long endpointId)
        {
            return CurrentContext.NOVAManagementRoleMatrices.Where(i=>i.EndPointSystemId == endpointId && i.ValidTo.Value >=DateTime.Now)
                .Select(i=> i.Department).Distinct().ToList();
        }
    }
}
