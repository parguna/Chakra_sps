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
    public class NOVAExceptionalRoleMatrixRepository : RepositoryBase<NOVAExceptionalRoleMatrix>
    {
        public NOVAExceptionalRoleMatrixRepository(DataAccessTransaction transaction) : base(transaction)
        {
        }
        public Dictionary<string, string> GetCompanyCodeByEndPointSystemId(long endPointSystemId, AccountType accountType)
        {
            var query = CurrentContext.NOVAExceptionalRoleMatrices.Where(ep => ep.EndPointSystemId == endPointSystemId && ep.ValidTo >= DateTime.Now);

            if (accountType != AccountType.User)
                query = query.Where(a => !a.IsExclusiveForUserAccount);
                
            return query.Select(i => i.CompanyCode).Distinct().ToDictionary(item => item, item => item);
        }
        public List<NOVAExceptionalRoleMatrix> GetByCompanyCode(long endpointSystemId, string companyCode, AdHocActionType adhocActionType, long endpointUserId, AccountType accountType,long userId)
        {
            List<NOVAExceptionalRoleMatrix> results = new List<NOVAExceptionalRoleMatrix>();
            var matrices = CurrentContext.NOVAExceptionalRoleMatrices.Where(i => i.EndPointSystemId == endpointSystemId && i.CompanyCode == companyCode && i.ValidTo >=DateTime.Now);
            var user = CurrentContext.User.FirstOrDefault(i => i.Id == userId);

            if (accountType != AccountType.User)
                matrices = matrices.Where(a => !a.IsExclusiveForUserAccount);


            if (adhocActionType == AdHocActionType.NewAccount)
            {
                results = matrices.ToList();
            }
            else if(adhocActionType == AdHocActionType.AmendmentAdd || adhocActionType == AdHocActionType.AmendmentRemove)
            {
                AccountReconcilation endPointAccount = null;
                List<ManagementMatrix> exisitingRequests = new List<ManagementMatrix>();
                if (accountType == AccountType.User)
                {
                    endPointAccount = CurrentContext.AccountReconcilations.FirstOrDefault(i => i.EndpointSystemId == endpointSystemId && i.EndpointUserId == endpointUserId && i.UserId == user.Id && i.IsLocked == false);
                }
                else
                {
                    endPointAccount = CurrentContext.AccountReconcilations.FirstOrDefault(i => i.EndpointSystemId == endpointSystemId && i.EndpointUserId == endpointUserId && i.IsLocked == false);
                }
                var query = CurrentContext.NOVAExceptionalRoleMatrices.Where(i => i.CompanyCode == companyCode && i.EndPointSystemId == endpointSystemId && i.ValidTo >= DateTime.Now);
                if (accountType != AccountType.User)
                    query = query.Where(a => !a.IsExclusiveForUserAccount);
                var exceptionalMatrices = query.ToList();
                if (accountType == AccountType.User)
                {
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
							            WHERE  (ISNULL(EntitlementDetail,''))<>'' AND AD.EndpointSystemId={endpointSystemId} AND  A.EndpointSystemId={endpointSystemId} AND AD.AdhocActionType={((int)adhocActionType)} AND A.UserId={user.Id} AND A.ProgressStatus NOT IN(2,5,8,10)
                                    )AS AD CROSS APPLY XMLData.nodes('/AdHocNovaRequest/RequestDetails/AdHocNovaRequestDetails') xmlNode(dataObject)
                                ) AS PositionDetails  
                                  WHERE Position Is NOT NULL  AND NovaMatrixType='Exceptional'";
                    DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"EAM Roles Query: {exisitingRequestPositionQuery}");
                    exisitingRequests = CurrentContext.Database.SqlQuery<ManagementMatrix>(exisitingRequestPositionQuery).ToList();
                }
               
                var exisitingUserRolesQuery = (from excepMatrix in CurrentContext.NOVAExceptionalRoleMatrices
                                               from excepRole in CurrentContext.SAPEamNovaUserRoleInfo
                                               from entitlements in CurrentContext.EntitlementReconcilations
                                               where excepMatrix.CompanyCode == excepRole.Company 
                                                     && excepRole.Company == companyCode &&
                                                     excepRole.SAPEamNovaUserInfoId == endPointAccount.EndpointUserId
                                                     && excepMatrix.EndPointSystemId == endpointSystemId
                                                     && excepMatrix.ValidTo >= DateTime.Now && excepMatrix.FLINSRoles == excepRole.RoleName
                                                     && excepRole.NovaPositionMatrixType == Infrastructure.NovaMatrixType.Exceptional
                                                     && entitlements.EndpointSystemId == endpointSystemId && entitlements.EndpointUserId == endPointAccount.EndpointUserId
                                                     && entitlements.IsDeleted == false && entitlements.EntitlementType == 1
                                                     && entitlements.Entitlement == excepRole.RoleName
                                               select excepMatrix);


                DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"Query to get existing Roles :{exisitingUserRolesQuery.ToTraceQuery()}");

                var exisitingUserRoles =   exisitingUserRolesQuery.ToList();

                if (adhocActionType == AdHocActionType.AmendmentAdd)
                {
                    exceptionalMatrices = exceptionalMatrices.Except(exisitingUserRoles).ToList();
                    if(accountType == AccountType.User)
                    {
                        foreach (var exisitingRequest in exisitingRequests)
                        {
                            var dataExists = exceptionalMatrices.FirstOrDefault(i => i.CompanyCode == exisitingRequest.CompanyCode && i.FLINSRoles == exisitingRequest.Position);
                            if (dataExists != null && exceptionalMatrices.Count > 0)
                            {
                                exceptionalMatrices.Remove(dataExists);
                            }
                        }
                    }
                    
                    results = exceptionalMatrices;
                }
                else if(adhocActionType == AdHocActionType.AmendmentRemove)
                {
                    exceptionalMatrices = exceptionalMatrices.Intersect(exisitingUserRoles).ToList();
                    if (accountType == AccountType.User)
                    {
                        foreach (var exisitingRequest in exisitingRequests)
                        {

                            var dataExists = exceptionalMatrices.FirstOrDefault(i => i.CompanyCode == exisitingRequest.CompanyCode && i.FLINSRoles == exisitingRequest.Position);
                            if (dataExists != null && exceptionalMatrices.Count > 0)
                            {

                                DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"AmendRemove existing Roles :{dataExists.Position}");
                                exceptionalMatrices.Remove(dataExists);
                            }
                        }
                    }
                   
                }
                results = exceptionalMatrices;
            }
            
            return results;
        }
    }
}

