using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Logger;

namespace DI.Chakra.Repository
{
    public class NOVAUserRoleMatrixRepository : RepositoryBase<NOVAUserRoleMatrix>
    {
        public NOVAUserRoleMatrixRepository(DataAccessTransaction transaction) : base(transaction)
        {
        }


        public User GetUser(long id)
        {
            var user = CurrentContext.User.FirstOrDefault(u => u.Id == id);
            var orgSectionScope = CurrentContext.NOrganisations.FirstOrDefault(org => org.Name == user.OrgSection);
            if (orgSectionScope != null)
            {
                user.SectionCode = orgSectionScope.Code;
                user.SectionName = orgSectionScope.Name;
            }
            return user;
        }

        public Dictionary<long, string> GetUserMatrixByActionType(long endpointId, long userId, string searchString, AdHocActionType actionType, string SAPSectionCode)
        {
            var results = new Dictionary<long, string>();
            var user = CurrentContext.User.FirstOrDefault(i => i.Id == userId);
            var endPointAccount = CurrentContext.AccountReconcilations.FirstOrDefault(i => i.EndpointSystemId == endpointId && i.UserId == user.Id && i.IsLocked == false && i.IsDeleted == false);
            //var companyCode = CurrentContext.Companies.FirstOrDefault(i => i.Code == user.CompanyCode);
            //var orgSectionScope = CurrentContext.NOrganisations.FirstOrDefault(org=> org.Name == user.OrgSection);
            //string strsectioncode = string.Empty;
            //if (orgSectionScope != null)
            //    strsectioncode = orgSectionScope.Code;
            // var sectionCode = CurrentContext.SAPSectionMappingInfo.FirstOrDefault(i => i.HRMSSectionCode == user.OrganisationCode);
            switch (actionType)
            {
                case AdHocActionType.NewAccount:
                    {
                        if (!string.IsNullOrWhiteSpace(SAPSectionCode))
                        {
                            var userRoles = CurrentContext.NOVAUserRoleMatrices.Where(i => i.SectionCode.ToUpper().Trim() ==  SAPSectionCode.ToUpper().Trim()  // && i.CompanyCode == companyCode.Code
                            )
                                .Select(i => new
                                {
                                    i.JobDescription,
                                    i.Position,
                                    i.Id
                                });
                            if (!string.IsNullOrWhiteSpace(searchString))
                            {
                                userRoles = userRoles.Where(i => i.Position.Contains(searchString));
                            }
                            foreach (var userRole in userRoles)
                            {
                                results.Add(userRole.Id, $"{userRole.Position} - {userRole.JobDescription}");
                            }
                        }
                    }
                    break;
                case AdHocActionType.AmendmentAdd:
                    {
                        if (!string.IsNullOrWhiteSpace(SAPSectionCode))
                        {
                            var userRoleMatrices = (from userRoleMatrix in CurrentContext.NOVAUserRoleMatrices
                                                    where userRoleMatrix.SectionCode.ToUpper().Trim() ==  SAPSectionCode.ToUpper().Trim()  // && userRoleMatrix.CompanyCode == companyCode.Code
                                                   && userRoleMatrix.EndPointSystemId == endpointId
                                                   && userRoleMatrix.ValidTo >= DateTime.Now
                                                    select userRoleMatrix).ToList();

                            var exisitingUserRoles = from useRoleMatrix in CurrentContext.NOVAUserRoleMatrices
                                                     from userRole in CurrentContext.SAPEamNovaUserRoleInfo
                                                     from entitlements in CurrentContext.EntitlementReconcilations
                                                     where useRoleMatrix.SectionCode.ToUpper().Trim() ==  SAPSectionCode.ToUpper().Trim()  //&& useRoleMatrix.CompanyCode == companyCode.Code
                                                           && useRoleMatrix.EndPointSystemId == endpointId
                                                           && useRoleMatrix.ValidTo >= DateTime.Now && useRoleMatrix.Position == userRole.RoleName
                                                           && userRole.NovaPositionMatrixType == Infrastructure.NovaMatrixType.User
                                                           && entitlements.EndpointSystemId == endpointId && entitlements.EndpointUserId == endPointAccount.EndpointUserId
                                                           && entitlements.IsDeleted == false && entitlements.EntitlementType == 1 && userRole.NovaPositionMatrixType == Infrastructure.NovaMatrixType.User
                                                           && entitlements.Entitlement == userRole.RoleName
                                                     select useRoleMatrix;

                            DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"GetUserMatrixByActionType: AmendAdd for the user {userId} : Query to get existing Roles :{exisitingUserRoles.ToTraceQuery()}");


                            userRoleMatrices = userRoleMatrices.Except(exisitingUserRoles.ToList()).ToList();

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
                                  WHERE Position Is NOT NULL  AND NovaMatrixType='User'";


                            DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"GetUserMatrixByActionType: AmendAdd for the user {userId} : Query to get existing Roles :{exisitingRequestPositionQuery}");


                            var exisitingRequests = CurrentContext.Database.SqlQuery<ManagementMatrix>(exisitingRequestPositionQuery).ToList();

                            if (!string.IsNullOrWhiteSpace(searchString))
                            {
                                userRoleMatrices = userRoleMatrices.Where(i => i.Position.Contains(searchString)).ToList();
                            }



                            foreach (var exisitingRequest in exisitingRequests)
                            {
                                var dataExists = userRoleMatrices.FirstOrDefault(i => //i.CompanyCode == exisitingRequest.CompanyCode &&
                                i.SectionCode == exisitingRequest.SectionCode && i.Position == exisitingRequest.Position);
                                if (dataExists != null && userRoleMatrices.Count > 0)
                                {
                                    userRoleMatrices.Remove(dataExists);
                                }
                            }

                            foreach (var userRoleMatrice in userRoleMatrices)
                            {
                                results.Add(userRoleMatrice.Id, $"{userRoleMatrice.Position} - {userRoleMatrice.JobDescription}");
                            }
                        }
                    }
                    break;
                case AdHocActionType.AmendmentRemove:
                    {
                        //var userRoleMatrices = (from userRoleMatrix in CurrentContext.NOVAUserRoleMatrices
                        //                        where userRoleMatrix.SectionCode.ToUpper().Trim() == sectionCode.SAPSectionCode.ToUpper().Trim()  // && userRoleMatrix.CompanyCode == companyCode.Code
                        //                       && userRoleMatrix.EndPointSystemId == endpointId
                        //                       && userRoleMatrix.ValidTo >= DateTime.Now
                        //                        select userRoleMatrix).ToList();
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
                                  WHERE Position Is NOT NULL  AND NovaMatrixType='User'";

                        DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"GetUserMatrixByActionType: AmendRemove for the user {userId} : Query to get existing Roles :{exisitingRequestPositionQuery}");


                        var exisitingRequests = CurrentContext.Database.SqlQuery<ManagementMatrix>(exisitingRequestPositionQuery).ToList();

                        var _searchString = !string.IsNullOrWhiteSpace(searchString) ? searchString : string.Empty;

                        var userRoles = (
                                        from useRoleMatrix in CurrentContext.NOVAUserRoleMatrices join 
                                         userRole in CurrentContext.SAPEamNovaUserRoleInfo on useRoleMatrix.Position equals userRole.RoleName 
                                        join 
                                            ent in CurrentContext.EntitlementReconcilations.Where(entitlements =>
                                            //where // useRoleMatrix.SectionCode.ToUpper().Trim() == sectionCode.SAPSectionCode.ToUpper().Trim()  
                                            // && useRoleMatrix.CompanyCode == companyCode.Code 
                                            // && useRoleMatrix.EndPointSystemId == endpointId
                                            // && useRoleMatrix.ValidTo >= DateTime.Now && useRoleMatrix.Position == userRole.RoleName  &&
                                            entitlements.EndpointSystemId == endpointId && !entitlements.IsDeleted
                                            && entitlements.EndpointUserId == endPointAccount.EndpointUserId
                                            && entitlements.IsDeleted == false && entitlements.EntitlementType == 1).Select(entitlements => entitlements)
                                            on new { EntitlementId = userRole.Id, Role = userRole.RoleName }  equals new
                                            {
                                                EntitlementId = ent.EntitlementId,
                                                Role = ent.Entitlement
                                            }
                                        where useRoleMatrix.SectionCode.ToUpper().Trim() == SAPSectionCode.ToUpper().Trim() && userRole.Section == SAPSectionCode &&
                                        userRole.SAPEamNovaUserInfoId == endPointAccount.EndpointUserId && 
                                        (_searchString == "" || ent.EntitlementDescription.Contains(_searchString))
                                        select  new
                                        {
                                            Id = useRoleMatrix.Id,
                                            SectionCode = userRole.Section,
                                            Position = ent.Entitlement,
                                            JobDescription = ent.EntitlementDescription
                                        }
                                        );

                        DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"GetUserMatrixByActionType: AmendRemove for the user {userId} ");


                        //userRoleMatrices = userRoleMatrices.Intersect(userRoles).ToList();
                        //if (!string.IsNullOrWhiteSpace(searchString))
                        //{
                        //    userRoleMatrices = userRoleMatrices.Where(i=> i.Position.Contains(searchString)).ToList();
                        //}
                        var userRoleMatrices = userRoles.Distinct().ToList();

                        foreach (var exisitingRequest in exisitingRequests)
                        {
                            var dataExists = userRoleMatrices.FirstOrDefault(i => i.SectionCode == exisitingRequest.SectionCode
                                                // && i.CompanyCode == exisitingRequest.CompanyCode 
                                                && i.Position == exisitingRequest.Position);
                            if (dataExists != null && userRoleMatrices.Count > 0)
                            {
                                userRoleMatrices.Remove(dataExists);
                            }
                        }
                        foreach (var userRole in userRoleMatrices)
                        {
                            if (!results.Any(i => i.Key == userRole.Id))
                                results.Add(userRole.Id, $"{userRole.Position} [{userRole.JobDescription}]");
                        }
                    }
                    break;
                default:
                    break;
            }
            return results;
        }

        public List<string> GetSAPSection(long userId, AdHocActionType adhocActionType, long endpointId)
        {
            var list = new List<string>();

            if (adhocActionType == AdHocActionType.NewAccount)
            {
                return CurrentContext.NOVAUserRoleMatrices.Select(s => s.SectionCode).Distinct().OrderBy(i => i).ToList();
            }
            else
            {
                var endPointAccount = CurrentContext.AccountReconcilations.FirstOrDefault(i => i.EndpointSystemId == endpointId && i.UserId == userId && i.IsLocked == false && i.IsDeleted == false);


                var exisitingUserRolesSection = (
                                          from userRole in CurrentContext.SAPEamNovaUserRoleInfo
                                          join  entitlements in CurrentContext.EntitlementReconcilations on userRole.Id equals entitlements.EntitlementId
                                          where userRole.NovaPositionMatrixType == Infrastructure.NovaMatrixType.User
                                                && entitlements.EndpointSystemId == endpointId && entitlements.EndpointUserId == endPointAccount.EndpointUserId
                                                && entitlements.IsDeleted == false && entitlements.EntitlementType == 1
                                                && entitlements.Entitlement == userRole.RoleName
                                          select userRole.Section);

                list = exisitingUserRolesSection.Distinct().OrderBy(i => i).ToList();
                if (list == null || list.Count == 0) return CurrentContext.NOVAUserRoleMatrices.Select(s => s.SectionCode).Distinct().OrderBy(i => i).ToList();
            }


            return list;
        }
        public List<NOVAUserRoleMatrix> GetExisitingRequestAndRole(long endpointId, long userId, AdHocActionType actionType, string SAPSectionCode)
        {
            var results = new List<NOVAUserRoleMatrix>();
            var user = CurrentContext.User.FirstOrDefault(i => i.Id == userId);
            var endPointAccount = CurrentContext.AccountReconcilations.FirstOrDefault(i => i.EndpointSystemId == endpointId && i.UserId == user.Id && i.IsLocked == false && i.IsDeleted == false);
            //var companyCode = CurrentContext.Companies.FirstOrDefault(i => i.Code == user.CompanyCode);

            //var orgSectionScope = CurrentContext.NOrganisations.FirstOrDefault(org => org.Name == user.OrgSection);
            //string strsectioncode = string.Empty;
            //if (orgSectionScope != null)
            //    strsectioncode = orgSectionScope.Code;

            //var sectionCode = CurrentContext.SAPSectionMappingInfo.FirstOrDefault(i => i.HRMSSectionCode == user.OrganisationCode);

            if (!string.IsNullOrWhiteSpace(SAPSectionCode))
            {
                var userRoleMatrices = (from userRoleMatrix in CurrentContext.NOVAUserRoleMatrices
                                        where userRoleMatrix.SectionCode.ToUpper().Trim() == SAPSectionCode.ToUpper().Trim() // && userRoleMatrix.CompanyCode == companyCode.Code
                                       && userRoleMatrix.EndPointSystemId == endpointId
                                       && userRoleMatrix.ValidTo >= DateTime.Now
                                        select userRoleMatrix).ToList();

                var exisitingUserRoles = (from useRoleMatrix in CurrentContext.NOVAUserRoleMatrices
                                          from userRole in CurrentContext.SAPEamNovaUserRoleInfo
                                          from entitlements in CurrentContext.EntitlementReconcilations
                                          where useRoleMatrix.SectionCode.ToUpper().Trim() ==  SAPSectionCode.ToUpper().Trim() //&& useRoleMatrix.CompanyCode == companyCode.Code
                                                && useRoleMatrix.EndPointSystemId == endpointId
                                                && useRoleMatrix.ValidTo >= DateTime.Now && useRoleMatrix.Position == userRole.RoleName
                                                && userRole.NovaPositionMatrixType == Infrastructure.NovaMatrixType.User
                                                && entitlements.EndpointSystemId == endpointId && entitlements.EndpointUserId == endPointAccount.EndpointUserId
                                                && entitlements.IsDeleted == false && entitlements.EntitlementType == 1
                                                && entitlements.Entitlement == userRole.RoleName
                                          select useRoleMatrix).ToList();

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
                                  WHERE Position Is NOT NULL  AND NovaMatrixType='User'";
                var exisitingRequests = CurrentContext.Database.SqlQuery<ManagementMatrix>(exisitingRequestPositionQuery).ToList();

                foreach (var exisitingRequest in exisitingRequests)
                {
                    var dataExists = userRoleMatrices.FirstOrDefault(i => //i.CompanyCode == exisitingRequest.CompanyCode && 
                    i.SectionCode.ToUpper().Trim() == exisitingRequest.SectionCode.ToUpper().Trim() && i.Position == exisitingRequest.Position);
                    if (dataExists != null)
                    {
                        exisitingUserRoles.Add(dataExists);
                    }
                }
                results = exisitingUserRoles.Distinct().ToList();
            }
            return results;
        }
    }
}
