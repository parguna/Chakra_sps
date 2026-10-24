using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository 
{
    public class SAPEamNovaUserInfoRepository : RepositoryBase<SAPEamNovaUserInfo>
    {
        readonly DataAccessTransaction currentTransaction;
        readonly string _saveUser;
        public SAPEamNovaUserInfoRepository(DataAccessTransaction transaction, string saveUser) : base(transaction) {
            currentTransaction = transaction;
            _saveUser = saveUser;
        }

        public SAPEamNovaUserInfo GetLockedAccount(long diuserId, long endpointSystemId, AccountType accountType)
        {

            return (from adUser in CurrentContext.SAPEamNovaUserInfo
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.EndpointSystemId == endpointSystemId && accountRecon.UserId == diuserId &&
                    accountRecon.AccountType == accountType &&  accountRecon.IsLocked
                    select adUser).FirstOrDefault();
        }

        public SAPEamNovaUserInfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId, AccountType accountType)
        {

            var user = (from adUser in CurrentContext.SAPEamNovaUserInfo
                     
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId
                 && accountRecon.AccountType == accountType
                    select adUser).FirstOrDefault();

            user.SAPEamNovaUserRoles = this.CurrentContext.SAPEamNovaUserRoleInfo.Where(r => r.SAPEamNovaUserInfoId == user.Id).ToList();

            return user;
        }


        public List<SAPEamNovaUserInfo> GetAllByDIUserId(long dIUserId, long endPointSystemId, List<AccountType> accountTypes)
        {

            var users =  (from adUser in CurrentContext.SAPEamNovaUserInfo 
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId 
                    where accountRecon.UserId == dIUserId && accountRecon.IsDeleted == false && accountRecon.EndpointSystemId == endPointSystemId && accountTypes.Contains(accountRecon.AccountType )
                    select adUser).ToList();

            foreach (var u in users)
            {
                u.SAPEamNovaUserRoles = this.CurrentContext.SAPEamNovaUserRoleInfo.Where(r => r.SAPEamNovaUserInfoId == u.Id).ToList();
            }

            return users;
        }

        public void ExecuteQuery(string query)
        {
            CurrentContext.Database.ExecuteSqlCommand(query);
        }

        public Dictionary<long, string> GetByEndPointSystemId(long endPointSystemId)
        {
            return CurrentContext.SAPEamNovaUserInfo.Where(ep => ep.EndPointSystemId == endPointSystemId).ToDictionary(item => item.Id, item => item.FLINS_UserLogonName);
        }

        public Dictionary<long, string> GetByEndPointSystemId(long endPointSystemId, long DiUserID)
        {
            return CurrentContext.SAPEamNovaUserInfo.Where(ep => ep.EndPointSystemId == endPointSystemId && ep.UserId == DiUserID).ToDictionary(item => item.Id, item => item.FLINS_UserLogonName);
        }
        public override long Insert(SAPEamNovaUserInfo user)
        {          
            long id = base.Insert(user);

            //foreach (var detail in user.SAPEamNovaUserRoles)
            //{
            //    detail.SAPEamNovaUserInfoId = id;
            //    this.CurrentContext.Set<SAPEamNovaUserRoleInfo>().Add(detail);
            //}
            //this.CurrentContext.SaveChanges(user);
            return id;
        }

        public override void Update(SAPEamNovaUserInfo user)
        {
            var existingParent = this.GetById(user.Id, r => r.SAPEamNovaUserRoles);
            try
            {
                if (existingParent != null)
                {
                    if (user is IUpdatable)
                    {
                        ((ICreatable)user).CreatedDate = ((ICreatable)existingParent).CreatedDate;
                        ((ICreatable)user).CreatedBy = ((ICreatable)existingParent).CreatedBy;

                        ((IUpdatable)user).LastModifiedDate = currentTransaction.transactionDateTime;
                        ((IUpdatable)user).LastModifiedBy = currentTransaction.User;
                    }
                    // Update parent
                    this.CurrentContext.Entry(existingParent).CurrentValues.SetValues(user);

                    // Delete children
                    foreach (var existingChild in existingParent.SAPEamNovaUserRoles.ToList())
                    {
                        if (!user.SAPEamNovaUserRoles.Any(c => c.Id == existingChild.Id))
                            CurrentContext.SAPEamNovaUserRoleInfo.Remove(existingChild);
                    }

                    // Update and Insert children
                    foreach (var childModel in user.SAPEamNovaUserRoles)
                    {
                        var existingChild = existingParent.SAPEamNovaUserRoles
                            .Where(c => c.Id == childModel.Id)
                            .SingleOrDefault();

                        if (existingChild != null)
                            // Update child
                            CurrentContext.Entry(existingChild).CurrentValues.SetValues(childModel);
                        else
                        {
                            // Insert child
                            var newChild = new SAPEamNovaUserRoleInfo
                            {
                                SAPEamNovaUserInfoId = user.Id,
                                RoleName = childModel.RoleName,
                                FLINS_TCodes = childModel.FLINS_TCodes,
                                FLINS_BI_TCodes = childModel.FLINS_BI_TCodes,
                                AD_Groups = childModel.AD_Groups,
                                IsActive = childModel.IsActive,
                                CreatedDate = DateTime.Now,
                                LastModifiedDate = DateTime.Now,
                                CreatedBy = _saveUser,
                                LastModifiedBy = _saveUser
                            };
                            CurrentContext.SAPEamNovaUserRoleInfo.Add(childModel);
                            //existingParent.SAPEamNovaUserRoles.Add(newChild);

                        }
                    }

                    CurrentContext.SaveChanges();
                }
            }
            catch (Exception e) { }
        }
        public Dictionary<long ,string> GetJobByMatrixType(long enpointId,string searchString,AdHocActionType actionType,long userId,NovaMatrixType matrixType)
        {
            Dictionary<long, string> result = new Dictionary<long, string>();
            User user = null;
            
            if (actionType == AdHocActionType.NewAccount)
            {
                //UserId for NewAccount Refers to User and others it refers to EndpointUserId
                #region NewAccount
                user = CurrentContext.User.FirstOrDefault(i=>i.Id == userId);
                if(matrixType == NovaMatrixType.User)
                {
                    var companyCode = CurrentContext.Companies.FirstOrDefault(i=> i.Code == user.CompanyCode);
                    var sectionCode = CurrentContext.SAPSectionMappingInfo.FirstOrDefault(i => i.HRMSSectionCode == user.SectionCode);
                    if(companyCode != null && sectionCode != null)
                    {
                        var userRoles = CurrentContext.NOVAUserRoleMatrices.Where(i => i.SectionCode == sectionCode.SAPSectionCode && i.CompanyCode == companyCode.Code)
                            .Select(i=> new
                            {
                                i.JobDescription,
                                i.Position,
                                i.Id
                            });
                        foreach (var userRole in userRoles)
                        {
                            result.Add(userRole.Id,$"{userRole.Position} - {userRole.JobDescription}");
                        }
                    }
                }
                else if(matrixType == NovaMatrixType.Management)
                {
                   
                }
                #endregion
            }
            else if (actionType == AdHocActionType.AmendmentAdd)
            {
                #region AmendmentAdd
                user = CurrentContext.User.FirstOrDefault(i => i.Id == userId);
                if (matrixType == NovaMatrixType.User)
                {
                    
                }
                else if (matrixType == NovaMatrixType.Management)
                {
                    var query = $@"
                            SELECT Id,Position,JobDescription FROM 
                            (
                                SELECT Id,Position,JobDescription FROM  DI_NOVA_ManagementRoleMatrix 
                                WHERE Position NOT IN
                                (
                                    SELECT Entitlement FROM DI_M_EntitlementReconcilation  
                                    WHERE EndpointSystemId ={enpointId} AND EndpointUserId ={userId} AND ISDELETED=0 AND EntitlementType = 1
                                ) AS A WHERE A.Position NOT IN
                                (
                                    SELECT Position FROM 
                                    (
                                        SELECT Cast(ISNULL(EntitlementDetail,'') as xml).value('(/AdHocNovaRequest/RequestDetails/AdHocNovaRequestDetails/RoleName)[1]', 'varchar(max)') as Position
                                        FROM DI_M_AdHocAccessRequest AD inner join DI_M_AccessRequest A on A.AdHocAccessRequestId = Ad.Id
							            WHERE  (ISNULL(EntitlementDetail,''))<>'' AND EndpointSystemId={enpointId} AND A.ProgressStatus NOT IN(2,5,8,10)
                                    ) AS PositionDetails WHERE Position Is NOT NULL        
                            ) AS A
                            ";
                    var managementRoles = CurrentContext.Database.SqlQuery<NOVAManagementRoleMatrix>(query);
                    foreach (var managementRole in managementRoles)
                    {
                        result.Add(managementRole.Id, $"{managementRole.Position} - {managementRole.JobDescription}");
                    }
                }
                else if (matrixType == NovaMatrixType.Exceptional)
                {
                    var query = $@"
                            SELECT Id,Position,JobDescription FROM 
                            (
                                SELECT Id,Position,JobDescription FROM  DI_NOVA_ExceptionalRoleMatrix 
                                WHERE Position NOT IN
                                (
                                    SELECT Entitlement FROM DI_M_EntitlementReconcilation  
                                    WHERE EndpointSystemId ={enpointId} AND EndpointUserId ={userId} AND ISDELETED=0 AND EntitlementType = 1
                                ) AS A 
                            ) AS A    
                            WHERE A.Position NOT IN
                                (
                                    SELECT Position FROM 
                                    (
                                        SELECT Cast(ISNULL(EntitlementDetail,'') as xml).value('(/AdHocNovaRequest/RequestDetails/AdHocNovaRequestDetails/RoleName)[1]', 'varchar(max)') as Position
                                        FROM DI_M_AdHocAccessRequest AD inner join DI_M_AccessRequest A on A.AdHocAccessRequestId = Ad.Id
							            WHERE  (ISNULL(EntitlementDetail,''))<>'' AND EndpointSystemId={enpointId} AND A.ProgressStatus NOT IN(2,5,8,10)
                                    ) AS PositionDetails WHERE Position Is NOT NULL        
                            )
                            ";
                    var exceptionalRoles = CurrentContext.Database.SqlQuery<NOVAExceptionalRoleMatrix>(query);
                    foreach (var exceptionalRole in exceptionalRoles)
                    {
                        result.Add(exceptionalRole.Id, $"{exceptionalRole.Position} - {exceptionalRole.JobDescription}");
                    }
                }
                #endregion
            }
            else if (actionType == AdHocActionType.AmendmentRemove)
            {
                #region AmendmentRemove
                user = CurrentContext.User.FirstOrDefault(i => i.Id == userId);
                if (matrixType == NovaMatrixType.User)
                {
                    var companyCode = CurrentContext.Companies.FirstOrDefault(i => i.Code == user.CompanyCode);
                    var sectionCode = CurrentContext.SAPSectionMappingInfo.FirstOrDefault(i => i.HRMSSectionCode == user.SectionCode);
                    if (companyCode != null && sectionCode != null)
                    {
                        var query = $@"
                             SELECT Id,Position,JobDescription FROM  DI_NOVA_UserRoleMatrix 
                             WHERE SectionCode ='{sectionCode.SAPSectionCode.Replace("'", "''")}' AND CompanyCode ='{companyCode.Code.Replace("'", "''")}'
                             AND Position IN
                            (
                                SELECT Entitlement FROM DI_M_EntitlementReconcilation  
                                WHERE EndpointSystemId ={enpointId} AND EndpointUserId ={userId} AND ISDELETED=0 AND EntitlementType = 1
                            )
                            ";
                        var userRoles = CurrentContext.Database.SqlQuery<NOVAUserRoleMatrix>(query);
                        foreach (var userRole in userRoles)
                        {
                            result.Add(userRole.Id, $"{userRole.Position} - {userRole.JobDescription}");
                        }
                    }
                }
                else if (matrixType == NovaMatrixType.Management)
                {
                    var query = $@"
                                SELECT Id,Position,JobDescription FROM  DI_NOVA_ManagementRoleMatrix 
                                WHERE Position IN
                                (
                                    SELECT Entitlement FROM DI_M_EntitlementReconcilation  
                                    WHERE EndpointSystemId ={enpointId} AND EndpointUserId ={userId} AND ISDELETED=0 AND EntitlementType = 1
                                ) AS A
                            ";
                    var managementRoles = CurrentContext.Database.SqlQuery<NOVAManagementRoleMatrix>(query);
                    foreach (var managementRole in managementRoles)
                    {
                        result.Add(managementRole.Id, $"{managementRole.Position} - {managementRole.JobDescription}");
                    }
                }
                else if (matrixType == NovaMatrixType.Exceptional)
                {
                    var query = $@"
                                SELECT Id,Position,JobDescription FROM  DI_NOVA_ExceptionalRoleMatrix 
                                WHERE Position NOT IN
                                (
                                    SELECT Entitlement FROM DI_M_EntitlementReconcilation  
                                    WHERE EndpointSystemId ={enpointId} AND EndpointUserId ={userId} AND ISDELETED=0 AND EntitlementType = 1
                                ) AS A 
                            ";
                    var exceptionalRoles = CurrentContext.Database.SqlQuery<NOVAExceptionalRoleMatrix>(query);
                    foreach (var exceptionalRole in exceptionalRoles)
                    {
                        result.Add(exceptionalRole.Id, $"{exceptionalRole.Position} - {exceptionalRole.JobDescription}");
                    }
                }
                #endregion
            }
            return result;
        }
    }
}
