using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Repository
{
    public class AccountReconcilationRepository : RepositoryBase<AccountReconcilation>
    {
        public AccountReconcilationRepository(DataAccessTransaction transaction) : base(transaction) { }
        

        public void ExecuteQry(string cmd)
        {
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
            CurrentContext.SaveChanges();
        }


        public List<AccountReconcilation> GetByAccountTypeAndEndpointSystemIdAndAccountName(long userId, AccountType accountType, long endpointSystemId, string accountName)
        {
            var accountReconQuery = CurrentContext.AccountReconcilations.Where(accountRecon =>
                         accountRecon.EndpointSystemId == endpointSystemId && accountRecon.AccountType == accountType && !accountRecon.IsLocked && !accountRecon.IsDeleted &&
                        accountRecon.AccountName.Equals(accountName, StringComparison.CurrentCultureIgnoreCase)).AsQueryable();

            if (accountType == AccountType.Service || accountType == AccountType.Shared)
            {
                var UserIds = CurrentContext.User.Join(CurrentContext.User, otheruser => otheruser.OrganisationCode, usrid => usrid.OrganisationCode, (otheruser, usrid) => new { UserList = otheruser, currentUser = usrid })
                                          .Where(bothuser => bothuser.currentUser.Id == userId).Select(bothuser => bothuser.UserList.Id).ToList();

                accountReconQuery = accountReconQuery.Where(a => UserIds.Contains(a.UserId.Value)).Select(a => a);
            }
            else
            {
                accountReconQuery = accountReconQuery.Where(a => a.UserId.Value == userId).Select(a => a);
            }

            return accountReconQuery.ToList();

        }
        public List<PresentationAccountReconcilationReport> GetResolvedReconcilations(DateTime? FromDate, DateTime? ToDate, ResolvedStatus status, bool IsResolved)
        {
            List<PresentationAccountReconcilationReport> result = new List<PresentationAccountReconcilationReport>();
            if (FromDate.HasValue && ToDate.HasValue)
            {
                var accountList = (from Account in CurrentContext.AccountReconcilations.Include("EndPoint")
                                   where Account.IsResolved == IsResolved && Account.ResolvedStatus == status && Account.CreatedDate >= FromDate && Account.CreatedDate <= ToDate
                                   && Account.ExceptionType != ExceptionTypes.None
                                   select new PresentationAccountReconcilationReport
                                   {
                                       ReconciliationId = Account.Id,
                                       AccountName = Account.AccountName,
                                       DisplayName = Account.FullName,
                                       EndpointSystemId = Account.EndpointSystemId,
                                       EndpointUserId = Account.EndpointUserId,
                                       CreatedDate = Account.CreatedDate,
                                       UserId = Account.UserId,
                                       ExceptionType = Account.ExceptionType,
                                       ReconcilationType = ReconcilationType.Account,
                                       ResolvedStatus = status,
                                       ResolvedBy = Account.ResolvedBy,
                                       Comments = Account.Comments,
                                       EndPointName = Account.EndPoint.DisplayName
                                   }
                      ).ToList();
                var entitleMentList = (from Entitlement in CurrentContext.EntitlementReconcilations.Include("EndPoint")
                                       where Entitlement.IsResolved == IsResolved && Entitlement.ResolvedStatus == status && Entitlement.CreatedDate >= FromDate && Entitlement.CreatedDate <= ToDate
                                       && Entitlement.ExceptionType != ExceptionTypes.None
                                       select new PresentationAccountReconcilationReport
                                       {
                                           ReconciliationId = Entitlement.Id,
                                           AccountName = Entitlement.AccountName,
                                           DisplayName = Entitlement.Entitlement,
                                           EntitlementType = ((EntitlementType)Entitlement.EntitlementType).ToString(),
                                           Entitlement = Entitlement.Entitlement,
                                           EndpointSystemId = Entitlement.EndpointSystemId,
                                           EndpointUserId = Entitlement.EndpointUserId,
                                           CreatedDate = Entitlement.CreatedDate,
                                           UserId = Entitlement.UserId,
                                           ExceptionType = Entitlement.ExceptionType,
                                           ReconcilationType = ReconcilationType.Entitlement,
                                           ResolvedStatus = status,
                                           ResolvedBy = Entitlement.ResolvedBy,
                                           Comments = Entitlement.Comments,
                                           EndPointName = Entitlement.EndPoint.DisplayName
                                       }
                 ).ToList();
                result = accountList.Union(entitleMentList).ToList();
            }
            else
            {
                var accountList = (from Account in CurrentContext.AccountReconcilations
                                   where Account.IsResolved == IsResolved && Account.ResolvedStatus == status
                                   && Account.ExceptionType != ExceptionTypes.None
                                   select new PresentationAccountReconcilationReport
                                   {
                                       ReconciliationId = Account.Id,
                                       AccountName = Account.AccountName,
                                       DisplayName = Account.FullName,
                                       EndpointSystemId = Account.EndpointSystemId,
                                       EndpointUserId = Account.EndpointUserId,
                                       CreatedDate = Account.CreatedDate,
                                       UserId = Account.UserId,
                                       ExceptionType = Account.ExceptionType,
                                       ReconcilationType = ReconcilationType.Account,
                                       ResolvedStatus = status,
                                       ResolvedBy = Account.ResolvedBy,
                                       Comments = Account.Comments,
                                       EndPointName = Account.EndPoint.DisplayName
                                   }
                     ).ToList();
                var entitleMentList = (from Entitlement in CurrentContext.EntitlementReconcilations
                                       where Entitlement.IsResolved == IsResolved && Entitlement.ResolvedStatus == status
                                       && Entitlement.ExceptionType != ExceptionTypes.None
                                       select new PresentationAccountReconcilationReport
                                       {
                                           ReconciliationId = Entitlement.Id,
                                           AccountName = Entitlement.AccountName,
                                           DisplayName = Entitlement.Entitlement,
                                           EntitlementType = ((EntitlementType)Entitlement.EntitlementType).ToString(),
                                           Entitlement = Entitlement.Entitlement,
                                           EndpointSystemId = Entitlement.EndpointSystemId,
                                           EndpointUserId = Entitlement.EndpointUserId,
                                           CreatedDate = Entitlement.CreatedDate,
                                           UserId = Entitlement.UserId,
                                           ExceptionType = Entitlement.ExceptionType,
                                           ReconcilationType = ReconcilationType.Entitlement,
                                           ResolvedStatus = status,
                                           ResolvedBy = Entitlement.ResolvedBy,
                                           Comments = Entitlement.Comments,
                                           EndPointName = Entitlement.EndPoint.DisplayName
                                       }
                 ).ToList();
                result = accountList.Union(entitleMentList).ToList();
            }
            return result;
        }
        public List<PresentationAccountReconcilationReport> GetAddedExceptions(int endpointSystemId, ReconcilationType reconcilationType, bool? isResolved = null, bool? isDeleted = null)
        {
            List<PresentationAccountReconcilationReport> result = new List<PresentationAccountReconcilationReport>();
            if (reconcilationType == ReconcilationType.Account)
            {
                result = (from Account in CurrentContext.AccountReconcilations
                          where Account.EndpointSystemId == endpointSystemId && Account.ExceptionType == ExceptionTypes.Added && (isResolved == null ? 1 == 1 : Account.IsResolved == (bool)isResolved) && (isDeleted == null ? 1 == 1 : Account.IsDeleted == (bool)isDeleted)
                          select new PresentationAccountReconcilationReport
                          {
                              ReconciliationId = Account.Id,
                              AccountName = Account.AccountName,
                              DisplayName = Account.FullName,
                              EndpointSystemId = endpointSystemId,
                              EndpointUserId = Account.EndpointUserId,
                              CreatedDate = (isResolved.HasValue && isResolved.Value) ? (Account.ResolvedDate.HasValue ? Account.ResolvedDate.Value : Account.CreatedDate) : (Account.ExceptionOccuredDate.HasValue ? Account.ExceptionOccuredDate.Value : Account.CreatedDate),
                              UserId = Account.UserId,
                              ExceptionType = ExceptionTypes.Added,
                              ReconcilationType = ReconcilationType.Account,
                              Comments = Account.Comments
                          }
                    ).ToList();
            }
            else if (reconcilationType == ReconcilationType.Entitlement)
            {
                result = (from Entitlement in CurrentContext.EntitlementReconcilations
                          join accont in CurrentContext.AccountReconcilations
                          on Entitlement.AccountReconcilationId equals accont.Id
                          where Entitlement.EndpointSystemId == endpointSystemId && Entitlement.ExceptionType == ExceptionTypes.Added && (isResolved == null ? 1 == 1 : Entitlement.IsResolved == (bool)isResolved) && (isDeleted == null ? 1 == 1 : Entitlement.IsDeleted == (bool)isDeleted)
                          select new PresentationAccountReconcilationReport
                          {
                              ReconciliationId = Entitlement.Id,
                              AccountName = Entitlement.AccountName,
                              DisplayName = accont.FullName,
                              EntitlementType = ((EntitlementType)Entitlement.EntitlementType).ToString(),
                              Entitlement = Entitlement.Entitlement,
                              EndpointSystemId = endpointSystemId,
                              EndpointUserId = Entitlement.EndpointUserId,
                              CreatedDate = (isResolved.HasValue && isResolved.Value) ? (Entitlement.ResolvedDate.HasValue ? Entitlement.ResolvedDate.Value : Entitlement.CreatedDate) : (Entitlement.ExceptionOccuredDate.HasValue ? Entitlement.ExceptionOccuredDate.Value : Entitlement.CreatedDate),
                              UserId = Entitlement.UserId,
                              ExceptionType = ExceptionTypes.Added,
                              ReconcilationType = ReconcilationType.Entitlement,
                              Comments = Entitlement.Comments
                          }
                  ).ToList();
            }
            else if (reconcilationType == ReconcilationType.All)
            {
                var accountList = (from Account in CurrentContext.AccountReconcilations
                                   where Account.EndpointSystemId == endpointSystemId && Account.ExceptionType == ExceptionTypes.Added && (isResolved == null ? 1 == 1 : Account.IsResolved == (bool)isResolved) && (isDeleted == null ? 1 == 1 : Account.IsDeleted == (bool)isDeleted)
                                   select new PresentationAccountReconcilationReport
                                   {
                                       ReconciliationId = Account.Id,
                                       AccountName = Account.AccountName,
                                       DisplayName = Account.FullName,
                                       EndpointSystemId = endpointSystemId,
                                       EndpointUserId = Account.EndpointUserId,
                                       CreatedDate = (isResolved.HasValue && isResolved.Value) ? (Account.ResolvedDate.HasValue ? Account.ResolvedDate.Value : Account.CreatedDate) : (Account.ExceptionOccuredDate.HasValue ? Account.ExceptionOccuredDate.Value : Account.CreatedDate),
                                       UserId = Account.UserId,
                                       ExceptionType = ExceptionTypes.Added,
                                       ReconcilationType = ReconcilationType.Account,
                                       Comments = Account.Comments
                                   }
                  ).ToList();
                var entitleMentList = (from Entitlement in CurrentContext.EntitlementReconcilations
                                       join accont in CurrentContext.AccountReconcilations
                                        on Entitlement.AccountReconcilationId equals accont.Id
                                       where Entitlement.EndpointSystemId == endpointSystemId && Entitlement.ExceptionType == ExceptionTypes.Added && (isResolved == null ? 1 == 1 : Entitlement.IsResolved == (bool)isResolved) && (isDeleted == null ? 1 == 1 : Entitlement.IsDeleted == (bool)isDeleted)
                                       select new PresentationAccountReconcilationReport
                                       {
                                           ReconciliationId = Entitlement.Id,
                                           AccountName = Entitlement.AccountName,
                                           DisplayName = accont.FullName,
                                           EntitlementType = ((EntitlementType)Entitlement.EntitlementType).ToString(),
                                           Entitlement = Entitlement.Entitlement,
                                           EndpointSystemId = endpointSystemId,
                                           EndpointUserId = Entitlement.EndpointUserId,
                                           CreatedDate = (isResolved.HasValue && isResolved.Value) ? (Entitlement.ResolvedDate.HasValue ? Entitlement.ResolvedDate.Value : Entitlement.CreatedDate) : (Entitlement.ExceptionOccuredDate.HasValue ? Entitlement.ExceptionOccuredDate.Value : Entitlement.CreatedDate),
                                           UserId = Entitlement.UserId,
                                           ExceptionType = ExceptionTypes.Added,
                                           ReconcilationType = ReconcilationType.Entitlement,
                                           Comments = Entitlement.Comments
                                       }
                 ).ToList();

                result = accountList.Union(entitleMentList).ToList();

            }
            return result;
        }


        public IList<PresentationAccountReconcilationReport> GetRemovedExceptions(long endpointSystemId, ReconcilationType reconcilationType, bool? isResolved = null, bool? isDeleted = null)
        {
            List<PresentationAccountReconcilationReport> result = new List<PresentationAccountReconcilationReport>();
            if (reconcilationType == ReconcilationType.Account)
            {

                result = (from accountReconcile in CurrentContext.AccountReconcilations
                          join usr in CurrentContext.User
                              on accountReconcile.UserId equals usr.Id into detail
                          from usr in detail.DefaultIfEmpty()
                          where accountReconcile.EndpointSystemId == endpointSystemId &&
                          (isResolved == null ? 1 == 1 : accountReconcile.IsResolved == (bool)isResolved) && (isDeleted == null ? 1 == 1 : accountReconcile.IsDeleted == (bool)isDeleted)
                          && accountReconcile.ExceptionType == ExceptionTypes.Removed
                          //&&
                          //!CurrentContext.ADSUserInfos.Any(item => item.Id == accountReconcile.EndpointUserId &&
                          //    item.EndPointSystemId == accountReconcile.EndpointSystemId)
                          select new PresentationAccountReconcilationReport
                          {
                              ReconciliationId = accountReconcile.Id,
                              AccountName = accountReconcile.AccountName,
                              DisplayName = usr.DisplayName,
                              EndpointSystemId = endpointSystemId,
                              EndpointUserId = accountReconcile.EndpointUserId,
                              CreatedDate = (isResolved.HasValue && isResolved.Value) ? (accountReconcile.ResolvedDate.HasValue ? accountReconcile.ResolvedDate.Value : accountReconcile.CreatedDate) : (accountReconcile.ExceptionOccuredDate.HasValue ? accountReconcile.ExceptionOccuredDate.Value : accountReconcile.CreatedDate),
                              UserId = accountReconcile.UserId,
                              ExceptionType = ExceptionTypes.Removed,
                              ReconcilationType = ReconcilationType.Account,
                              Comments = accountReconcile.Comments
                          }
                           ).ToList();
            }
            else if (reconcilationType == ReconcilationType.Entitlement)
            {
                result = (from entitleMentReconcile in CurrentContext.EntitlementReconcilations
                          join usr in CurrentContext.User
                              on entitleMentReconcile.UserId equals usr.Id into detail
                          from usr in detail.DefaultIfEmpty()
                          where entitleMentReconcile.EndpointSystemId == endpointSystemId &&
                          (isResolved == null ? 1 == 1 : entitleMentReconcile.IsResolved == (bool)isResolved) && (isDeleted == null ? 1 == 1 : entitleMentReconcile.IsDeleted == (bool)isDeleted)
                          && entitleMentReconcile.ExceptionType == ExceptionTypes.Removed
                          //&&
                          //!CurrentContext.ADSUserInfos.Any(item => item.Id == entitleMentReconcile.EndpointUserId &&
                          //    item.EndPointSystemId == entitleMentReconcile.EndpointSystemId)
                          select new PresentationAccountReconcilationReport
                          {
                              ReconciliationId = entitleMentReconcile.Id,
                              AccountName = entitleMentReconcile.AccountName,
                              DisplayName = usr.DisplayName,
                              Entitlement = entitleMentReconcile.Entitlement,
                              EntitlementType = ((EntitlementType)entitleMentReconcile.EntitlementType).ToString(),
                              EndpointSystemId = endpointSystemId,
                              EndpointUserId = entitleMentReconcile.EndpointUserId,
                              CreatedDate = (isResolved.HasValue && isResolved.Value) ? (entitleMentReconcile.ResolvedDate.HasValue ? entitleMentReconcile.ResolvedDate.Value : entitleMentReconcile.CreatedDate) : (entitleMentReconcile.ExceptionOccuredDate.HasValue ? entitleMentReconcile.ExceptionOccuredDate.Value : entitleMentReconcile.CreatedDate),
                              UserId = entitleMentReconcile.UserId,
                              ExceptionType = ExceptionTypes.Removed,
                              ReconcilationType = ReconcilationType.Entitlement,
                              Comments = entitleMentReconcile.Comments
                          }
                             ).ToList();
            }
            else if (reconcilationType == ReconcilationType.All)
            {
                var accountList = (from accountReconcile in CurrentContext.AccountReconcilations
                                   join usr in CurrentContext.User
                                       on accountReconcile.UserId equals usr.Id into detail
                                   from usr in detail.DefaultIfEmpty()
                                   where accountReconcile.EndpointSystemId == endpointSystemId &&
                                   (isResolved == null ? 1 == 1 : accountReconcile.IsResolved == (bool)isResolved) && (isDeleted == null ? 1 == 1 : accountReconcile.IsDeleted == (bool)isDeleted)
                                   && accountReconcile.ExceptionType == ExceptionTypes.Removed
                                   //&&
                                   //!CurrentContext.ADSUserInfos.Any(item => item.Id == accountReconcile.EndpointUserId &&
                                   //    item.EndPointSystemId == accountReconcile.EndpointSystemId)
                                   select new PresentationAccountReconcilationReport
                                   {
                                       ReconciliationId = accountReconcile.Id,
                                       AccountName = accountReconcile.AccountName,
                                       DisplayName = usr.DisplayName,
                                       EndpointSystemId = endpointSystemId,
                                       EndpointUserId = accountReconcile.EndpointUserId,
                                       CreatedDate = (isResolved.HasValue && isResolved.Value) ? (accountReconcile.ResolvedDate.HasValue ? accountReconcile.ResolvedDate.Value : accountReconcile.CreatedDate) : (accountReconcile.ExceptionOccuredDate.HasValue ? accountReconcile.ExceptionOccuredDate.Value : accountReconcile.CreatedDate),
                                       UserId = accountReconcile.UserId,
                                       ExceptionType = ExceptionTypes.Removed,
                                       ReconcilationType = ReconcilationType.Account,
                                       Comments = accountReconcile.Comments
                                   }
                           ).ToList();

                var entitleMentList = (from entitleMentReconcile in CurrentContext.EntitlementReconcilations
                                       join usr in CurrentContext.User
                                           on entitleMentReconcile.UserId equals usr.Id into detail
                                       from usr in detail.DefaultIfEmpty()
                                       where entitleMentReconcile.EndpointSystemId == endpointSystemId &&
                                       (isResolved == null ? 1 == 1 : entitleMentReconcile.IsResolved == (bool)isResolved) && (isDeleted == null ? 1 == 1 : entitleMentReconcile.IsDeleted == (bool)isDeleted)
                                       && entitleMentReconcile.ExceptionType == ExceptionTypes.Removed
                                       //&&
                                       //!CurrentContext.ADSUserInfos.Any(item => item.Id == entitleMentReconcile.EndpointUserId &&
                                       //    item.EndPointSystemId == entitleMentReconcile.EndpointSystemId)
                                       select new PresentationAccountReconcilationReport
                                       {
                                           ReconciliationId = entitleMentReconcile.Id,
                                           AccountName = entitleMentReconcile.AccountName,
                                           DisplayName = usr.DisplayName,
                                           Entitlement = entitleMentReconcile.Entitlement,
                                           EntitlementType = ((EntitlementType)entitleMentReconcile.EntitlementType).ToString(),
                                           EndpointSystemId = endpointSystemId,
                                           EndpointUserId = entitleMentReconcile.EndpointUserId,
                                           CreatedDate = (isResolved.HasValue && isResolved.Value) ? (entitleMentReconcile.ResolvedDate.HasValue ? entitleMentReconcile.ResolvedDate.Value : entitleMentReconcile.CreatedDate) : (entitleMentReconcile.ExceptionOccuredDate.HasValue ? entitleMentReconcile.ExceptionOccuredDate.Value : entitleMentReconcile.CreatedDate),
                                           UserId = entitleMentReconcile.UserId,
                                           ExceptionType = ExceptionTypes.Removed,
                                           ReconcilationType = ReconcilationType.Entitlement,
                                           Comments = entitleMentReconcile.Comments
                                       }
                            ).ToList();

                result = accountList.Union(entitleMentList).ToList();
            }
            return result;
        }

        public IList<PresentationResolvedAccountReconcilationReport> GetResolvedExceptions()
        {
            return (from AccountReconciliation in CurrentContext.AccountReconcilations
                    join endpoint in CurrentContext.EndPoint on AccountReconciliation.EndpointSystemId equals endpoint.Id
                    where AccountReconciliation.IsResolved
                    select new PresentationResolvedAccountReconcilationReport
                    {
                        AccountName = AccountReconciliation.AccountName,
                        Comments = AccountReconciliation.Comments,
                        EndpointSystemName = endpoint.DisplayName,
                        EndpointSystemType = endpoint.EndPointSystemType.DisplayName,
                        EnteredDate = AccountReconciliation.ResolvedDate.HasValue ? AccountReconciliation.ResolvedDate.Value : AccountReconciliation.CreatedDate,
                        ResolvedBy = AccountReconciliation.ResolvedBy,
                        ExceptionType = AccountReconciliation.ExceptionType,
                    }).ToList();
        }



        public List<PresentationAccountReconcilationReport> GetAttributeExceptions(int endpointSystemId, bool? isResolved = null, bool? isDeleted = null)
        {
            return (from Account in CurrentContext.AccountReconcilations
                    where Account.EndpointSystemId == endpointSystemId && Account.ExceptionType == ExceptionTypes.Attribute && (isResolved == null ? 1 == 1 : Account.IsResolved == (bool)isResolved) && (isDeleted == null ? 1 == 1 : Account.IsDeleted == (bool)isDeleted)
                    select new PresentationAccountReconcilationReport
                    {
                        ReconciliationId = Account.Id,
                        AccountName = Account.AccountName,
                        DisplayName = Account.FullName,
                        EndpointSystemId = endpointSystemId,
                        EndpointUserId = Account.EndpointUserId,
                        CreatedDate = Account.CreatedDate,
                        UserId = Account.UserId,
                        ExceptionType = ExceptionTypes.Attribute
                    }
                    ).ToList();
        }

        public AccountReconcilation GetByEndpoint(InhouseUserInfo obj)
        {

            AccountReconcilation accRecon = (from accRec in this.CurrentContext.AccountReconcilations
                                             join ePoint in this.CurrentContext.EndPoint
                                             on accRec.EndpointSystemId equals ePoint.Id
                                             where accRec.UserId == obj.Id
                                             select accRec).FirstOrDefault();
            return accRecon;
        }
        public void FlushAccountReconcilation(List<DI.Chakra.EntityModel.AccountReconcilation> accountReconcilations)
        {
            SQLBulkUploader<AccountReconcilation> sqlAccountReconcilation = null;
            string tableName = "DI_M_AccountReconcilation";
            sqlAccountReconcilation = new SQLBulkUploader<AccountReconcilation>(tableName);
            sqlAccountReconcilation.Flush(accountReconcilations);

        }

        public IList<PresentationAccountReconcilationReport> GetEntitlementDetailsForAccessRequest(long accessRequestId)
        {
            var accountList = (from accRec in this.CurrentContext.AccountReconcilations
                               where accRec.AccessRequestId == accessRequestId
                               select new PresentationAccountReconcilationReport
                               {
                                   AccountName = accRec.AccountName,
                                   EntitlementType = accRec.AccountType.ToString(),
                                   IsDeleted = accRec.IsDeleted ? "Delete" : "Add"
                               }).ToList();

            var entitleList = (from entitleRec in this.CurrentContext.EntitlementReconcilations
                               where entitleRec.AccessRequestId == accessRequestId
                               select new PresentationAccountReconcilationReport
                               {
                                   AccountName = entitleRec.AccountName,
                                   Entitlement = entitleRec.Entitlement,
                                   EntitlementType = ((EntitlementType)entitleRec.EntitlementType).ToString(),
                                   IsDeleted = entitleRec.IsDeleted ? "Delete" : "Add"
                               }).ToList();
            return accountList.Union(entitleList).ToList();
        }


        public Dictionary<long, long?> GetAllAccountReconcilationByEndpointSystemId(long endpointSystemId, AccountType accountType)
        {
            IQueryable<AccountReconcilation> query = this.CurrentContext.AccountReconcilations.Where(AccountRecon => !AccountRecon.IsDeleted && !AccountRecon.IsLocked && AccountRecon.AccountType == accountType && AccountRecon.EndpointSystemId == endpointSystemId && AccountRecon.UserId != null);


            return query.Select(AccountRecon => new { Key = AccountRecon.Id, Value = AccountRecon.UserId }).ToDictionary(key => key.Key, Value => Value.Value);
        }




        public Dictionary<long, string> GetByUserIdsDoesNotExist(List<long> userIds, EndpointType EndpointSystemType, long endpointSystemId, AccountType accountType, ApplicationType applicationType, bool isLocked, DI.Chakra.Infrastructure.Enums.ActionType actionType, bool considerLock = true)
        {

            //string str = $"UserIds : {string.Join(",", userIds)}  -  ";
            //str = str + $"EndpointSystemType : {(int)endpointSystemId}  - ";
            //str = str + $"accountType : {(int)accountType}  - ";
            //str = str + $"applicationType : {(int)applicationType}  - ";
            //var str1 = isLocked ? "True" : "False";

            //str = str + $"isLocked : {str1  }  - ";


            //DILog.ProvisioningLogger.WriteLog(null, endpointSystemId, LogLevelType.Info, str);

            var query = this.CurrentContext.AccountReconcilations.Where(AccountRecon => AccountRecon.AccountType == AccountType.User &&
                                                                        AccountRecon.UserId.HasValue && !AccountRecon.IsDeleted &&
                                                                        userIds.Contains(AccountRecon.UserId.Value)).Select(AccountRecon => AccountRecon);
            if (endpointSystemId != 0)
            {
                query = from accountRec in query
                        where accountRec.EndpointSystemId == endpointSystemId
                        select accountRec;

                //DILog.ProvisioningLogger.WriteLog(null, endpointSystemId, LogLevelType.Info, $"Inside endpointSystemId - {endpointSystemId}");
            }

            else if (applicationType != ApplicationType.None)
            {
                var endpointId = CurrentContext.EndPoint.Where(e => e.ApplicationType == applicationType && !e.IsTrustedSource).FirstOrDefault();

                query = from accountRec in query
                        where accountRec.EndpointSystemId == endpointId.Id
                        select accountRec;

                //DILog.ProvisioningLogger.WriteLog(null, endpointSystemId, LogLevelType.Info, $" Inside applicationType != ApplicationType.None - {applicationType}");

            }
            else if (EndpointSystemType != EndpointType.None)
            {
                query = from accountRec in query
                        join endpoint in CurrentContext.EndPoint on accountRec.EndpointSystemId equals endpoint.Id
                        where endpoint.EndpointSystemTypeId == (int)EndpointSystemType
                        select accountRec;

                //DILog.ProvisioningLogger.WriteLog(null, endpointSystemId, LogLevelType.Info, $"Inside  EndpointSystemType != EndpointType.None - {EndpointSystemType}");

            }

            var activeCountsCount = (from acctRecon in query
                                     where !acctRecon.IsDeleted && !acctRecon.IsLocked
                                     group acctRecon.Id by acctRecon.UserId into accts
                                     where accts.Count() >= 1
                                     select new { UserID = accts.Key, RecordCount = accts.Count() }).ToDictionary(d => d.UserID, d => d.RecordCount);

            if (considerLock)
            {
                query = from accountRec in query
                        where accountRec.IsLocked == isLocked
                        select accountRec;

                // DILog.ProvisioningLogger.WriteLog(null, endpointSystemId, LogLevelType.Info, $"Inside consider lock");
            }

            DILog.ProvisioningLogger.WriteLog(null, endpointSystemId, LogLevelType.DebugStep, $"GetByUserIdsDoesNotExists :  {query.ToTraceQuery()}");

            var ret1 = query.Select(o => new { Key = o.UserId.Value, Value = o.AccountName });
            var ret = new Dictionary<long, string>();
            foreach (var o in ret1)
            {
                if ((considerLock && activeCountsCount.ContainsKey(o.Key)) && (actionType == DI.Chakra.Infrastructure.Enums.ActionType.AmendmentAdd || actionType == DI.Chakra.Infrastructure.Enums.ActionType.AmendmentRemove || actionType == DI.Chakra.Infrastructure.Enums.ActionType.DeleteAccount))
                    continue;

                if (!ret.ContainsKey(o.Key))
                {
                    ret.Add(o.Key, o.Value);
                }
            }

            //DILog.ProvisioningLogger.WriteLog(null, endpointSystemId, LogLevelType.Info, $"No. of records {ret.Count()}");

            return ret;
        }

        public string CheckDuplicateRequestExist(List<long> userIds, long endpointSystemId, ApplicationType applicationType)
        {
            StringBuilder returnValue = new StringBuilder();
            string userid = string.Join(",", userIds);
            List<long> endPointSystemIDs = new List<long>() { endpointSystemId };


            EndPoint dependentEp = null;
            if (applicationType == ApplicationType.MSSL_CRM)
                dependentEp = this.CurrentContext.EndPoint.FirstOrDefault(ep => ep.ApplicationType == ApplicationType.MSSL_ISU);
            else if (applicationType == ApplicationType.MSSL_ISU)
                dependentEp = this.CurrentContext.EndPoint.FirstOrDefault(ep => ep.ApplicationType == ApplicationType.MSSL_CRM);
            else if (applicationType == ApplicationType.EBS_CRM)
                dependentEp = this.CurrentContext.EndPoint.FirstOrDefault(ep => ep.ApplicationType == ApplicationType.EBS_ISU);
            else if (applicationType == ApplicationType.EBS_ISU)
                dependentEp = this.CurrentContext.EndPoint.FirstOrDefault(ep => ep.ApplicationType == ApplicationType.EBS_CRM);

            if (dependentEp != null)
                endPointSystemIDs.Add(dependentEp.Id);

            var endPointSystemID = string.Join(",", endPointSystemIDs);

            string sqlQuery = $@"select 'Bulk Request' as Source, b.Id as RequestId , u.DisplayName
from DI_M_BulkRequest b   inner join DI_M_BulkRequestDetail br on b.Id = br.BulkRequestId inner join DI_M_User u on u.Id = br.UserId
  where b.EndpointSystemId  in ({endPointSystemID}) and b.AdhocActionType = 1  and br.UserId in ({userid}) and b.RequestStatus not in (3,5) and  b.ProgressStatus not in (2,8,6,5)
                                  union 
                                 
 select 'Adhoc Request' as Source, ar.Id as RequestId, u.DisplayName   from DI_M_AccessRequest ar 
    inner join DI_M_AdHocAccessRequest adr on ar.AdHocAccessRequestId=adr.Id  inner join DI_M_User u on u.Id = adr.RequesteeUserId
    where adr.EndpointSystemId in ({endPointSystemID}) and adr.AdhocActionType = 1 and adr.AccountType = 1 and adr.RequesteeUserId in ({userid}) and ar.RequestStatus not in (3,5) and ar.ProgressStatus not in (2,8,6,5)  ";

            System.Data.DataTable dt = GetDataTableByQuery(sqlQuery);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (System.Data.DataRow r in dt.Rows)
                {
                    returnValue.Append($" {r[2].ToString()} (Source:{ r[0].ToString()} - Id : {r[1].ToString()}) and <br/>");
                }
            }
            var s = returnValue.ToString();

            if (!string.IsNullOrWhiteSpace(s) && s.Count() > 10)
                s = returnValue.ToString().Substring(0, s.Count() - 10);

            return s;
        }

        public List<PresentationAccountReconcilationReport> GetBySystem(long systemId, Nullable<AccountType> accType, MappingType mappingTypes = MappingType.None, bool employeeHasMoreAccounts = false, bool eagerLoadAllProperties = true, bool isLocked = false, bool isDeleted = false)
        {
            List<PresentationAccountReconcilationReport> list = new List<PresentationAccountReconcilationReport>();
            List<PresentationAccountReconcilationReport> returnList = new List<PresentationAccountReconcilationReport>();
            if (employeeHasMoreAccounts)
            {
                if (systemId > 0)
                {
                    list = (from accon in this.CurrentContext.AccountReconcilations
                            join user in CurrentContext.User
                            on accon.UserId equals user.Id into acconUser
                            from user in acconUser.DefaultIfEmpty()
                            where accon.EndpointSystemId == systemId
                            && accon.IsLocked == isLocked
                            && accon.IsDeleted == isDeleted
                            select new PresentationAccountReconcilationReport
                            {
                                ReconciliationId = accon.Id,
                                AccountName = accon.AccountName,
                                DisplayName = accon.FullName,
                                EndpointSystemId = accon.EndpointSystemId,
                                EndpointUserId = accon.EndpointUserId,
                                CreatedDate = accon.CreatedDate,
                                UserId = accon.UserId,
                                ExceptionType = accon.ExceptionType,
                                ReconcilationType = ReconcilationType.Account,
                                ResolvedBy = accon.ResolvedBy,
                                Comments = accon.Comments,
                                EndPointName = accon.EndPoint.DisplayName,
                                UserDisplayName = user != null ? user.DisplayName : "",
                                IsLocked = accon.IsLocked,
                                IsDeletedBoolean = accon.IsDeleted,
                                AccountType = accon.AccountType
                            }).ToList();
                }
                else
                {
                    list = (from accon in this.CurrentContext.AccountReconcilations
                            join user in CurrentContext.User
                            on accon.UserId equals user.Id into acconUser
                            from user in acconUser.DefaultIfEmpty()
                            where accon.IsLocked == isLocked
                            && accon.IsDeleted == isDeleted
                            select new PresentationAccountReconcilationReport
                            {
                                ReconciliationId = accon.Id,
                                AccountName = accon.AccountName,
                                DisplayName = accon.FullName,
                                EndpointSystemId = accon.EndpointSystemId,
                                EndpointUserId = accon.EndpointUserId,
                                CreatedDate = accon.CreatedDate,
                                UserId = accon.UserId,
                                ExceptionType = accon.ExceptionType,
                                ReconcilationType = ReconcilationType.Account,
                                ResolvedBy = accon.ResolvedBy,
                                Comments = accon.Comments,
                                EndPointName = accon.EndPoint.DisplayName,
                                UserDisplayName = user != null ? user.DisplayName : "",
                                IsLocked = accon.IsLocked,
                                IsDeletedBoolean = accon.IsDeleted,
                                AccountType = accon.AccountType
                            }).ToList();
                }
            }
            else
            {
                if (systemId > 0)
                {
                    var query = (from accon in this.CurrentContext.AccountReconcilations
                                 join user in CurrentContext.User
                                 on accon.UserId equals user.Id into acconUser
                                 from user in acconUser.DefaultIfEmpty()
                                 where accon.EndpointSystemId == systemId
                                 && accon.IsLocked == isLocked
                                 && accon.IsDeleted == isDeleted
                                 && (mappingTypes == MappingType.None || accon.MappingType == mappingTypes)
                                 select
                                 new PresentationAccountReconcilationReport
                                 {
                                     ReconciliationId = accon.Id,
                                     AccountName = accon.AccountName,
                                     DisplayName = accon.FullName,
                                     EndpointSystemId = accon.EndpointSystemId,
                                     EndpointUserId = accon.EndpointUserId,
                                     CreatedDate = accon.CreatedDate,
                                     UserId = accon.UserId,
                                     ExceptionType = accon.ExceptionType,
                                     ReconcilationType = ReconcilationType.Account,
                                     ResolvedBy = accon.ResolvedBy,
                                     Comments = accon.Comments,
                                     EndPointName = accon.EndPoint.DisplayName,
                                     UserDisplayName = user != null ? user.DisplayName : "",
                                     IsLocked = accon.IsLocked,
                                     IsDeletedBoolean = accon.IsDeleted,
                                     AccountType = accon.AccountType
                                 });




                    list = query.ToList();
                }
                else
                {
                    list = (from accon in this.CurrentContext.AccountReconcilations
                            join user in CurrentContext.User
                            on accon.UserId equals user.Id into acconUser
                            from user in acconUser.DefaultIfEmpty()
                            where accon.IsLocked == isLocked
                            && accon.IsDeleted == isDeleted
                            && (mappingTypes == MappingType.None || accon.MappingType == mappingTypes)
                            select new PresentationAccountReconcilationReport
                            {
                                ReconciliationId = accon.Id,
                                AccountName = accon.AccountName,
                                DisplayName = accon.FullName,
                                EndpointSystemId = accon.EndpointSystemId,
                                EndpointUserId = accon.EndpointUserId,
                                CreatedDate = accon.CreatedDate,
                                UserId = accon.UserId,
                                ExceptionType = accon.ExceptionType,
                                ReconcilationType = ReconcilationType.Account,
                                ResolvedBy = accon.ResolvedBy,
                                Comments = accon.Comments,
                                EndPointName = accon.EndPoint.DisplayName,
                                UserDisplayName = user != null ? user.DisplayName : "",
                                IsLocked = accon.IsLocked,
                                IsDeletedBoolean = accon.IsDeleted,
                                AccountType = accon.AccountType
                            }).ToList();
                }
            }

            if (accType != null && accType.HasValue)
            {
                list = list.Where(i => i.AccountType == accType).ToList();
            }
            if (!employeeHasMoreAccounts)
                return list;

            var multiAcForOneOwner = (from p in list
                                      group p by p.UserId into grps
                                      where grps.Count() > 1 && grps.Key != null
                                      select grps).ToList();

            if (multiAcForOneOwner.Count <= 0)
                return new List<PresentationAccountReconcilationReport>();



            foreach (var gAcs in multiAcForOneOwner)
            {
                foreach (var recon in gAcs)
                {
                    returnList.Add(recon);
                }
            }
            if (accType != null && accType.HasValue)
            {
                returnList = returnList.Where(i => i.AccountType == accType).ToList();
            }
            return returnList;
        }
        public PresentationGridPagedData<PresentationAccountReconcilationReport> GetLazyLoadAllSystem(int startRowIndex, int maximumRows, string FilterExpression, string sortExpression, long systemId, Nullable<AccountType> accType, long mappingTypes)
        {
            MappingType mType = MappingType.None;
            if (mappingTypes != 3)
            {
                mType = (MappingType)mappingTypes;
            }

            var result = new List<PresentationAccountReconcilationReport>();
            IQueryable<PresentationAccountReconcilationReport> query = null;
            List<PresentationAccountReconcilationReport> returnList = new List<PresentationAccountReconcilationReport>();
            var finalResult = new PresentationGridPagedData<PresentationAccountReconcilationReport>();

            if (systemId > 0)
            {
                query = (from accon in this.CurrentContext.AccountReconcilations
                         join user in CurrentContext.User on accon.UserId equals user.Id into acconUser
                         join offAction in CurrentContext.OffboardingAccountAction on (long)accon.OffboardingAccountAction equals offAction.Id into offActions
                         from user in acconUser.DefaultIfEmpty()
                         from offAction in offActions.DefaultIfEmpty()
                         where accon.EndpointSystemId == systemId
                         && (mappingTypes == 3 ||
                         ((mappingTypes == 1 && accon.UserId != null && accon.MappingType != MappingType.Manual) ||
                         (mappingTypes == 2 && (accon.UserId != null && accon.MappingType == MappingType.Manual))
                         || (mappingTypes == 0 && accon.UserId == null)
                         )
                         )
                         select
                         new PresentationAccountReconcilationReport
                         {
                             ReconciliationId = accon.Id,
                             AccountName = accon.AccountName,
                             DisplayName = accon.FullName,
                             EndpointSystemId = accon.EndpointSystemId,
                             EndpointUserId = accon.EndpointUserId,
                             CreatedDate = accon.CreatedDate,
                             UserId = accon.UserId,
                             ExceptionType = accon.ExceptionType,
                             ReconcilationType = ReconcilationType.Account,
                             ResolvedBy = accon.ResolvedBy,
                             Comments = accon.Comments,
                             EndPointName = accon.EndPoint.DisplayName,
                             UserDisplayName = user != null ? user.DisplayName : "",
                             IsLocked = accon.IsLocked,
                             IsDeletedBoolean = accon.IsDeleted,
                             AccountType = accon.AccountType,
                             OffboardingAction = offAction.Desc
                         });
            }
            else
            {
                query = (from accon in this.CurrentContext.AccountReconcilations
                         join user in CurrentContext.User
                         on accon.UserId equals user.Id into acconUser
                         join offAction in CurrentContext.OffboardingAccountAction on (long)accon.OffboardingAccountAction equals offAction.Id into offActions
                         from user in acconUser.DefaultIfEmpty()
                         from offAction in offActions.DefaultIfEmpty()
                         where (mappingTypes == 3 ||
                         ((mappingTypes == 1 && accon.UserId != null && accon.MappingType != MappingType.Manual) ||
                         (mappingTypes == 2 && (accon.UserId != null && accon.MappingType == MappingType.Manual))
                         || (mappingTypes == 0 && accon.UserId == null)
                         ))
                         select new PresentationAccountReconcilationReport
                         {
                             ReconciliationId = accon.Id,
                             AccountName = accon.AccountName,
                             DisplayName = accon.FullName,
                             EndpointSystemId = accon.EndpointSystemId,
                             EndpointUserId = accon.EndpointUserId,
                             CreatedDate = accon.CreatedDate,
                             UserId = accon.UserId,
                             ExceptionType = accon.ExceptionType,
                             ReconcilationType = ReconcilationType.Account,
                             ResolvedBy = accon.ResolvedBy,
                             Comments = accon.Comments,
                             EndPointName = accon.EndPoint.DisplayName,
                             UserDisplayName = user != null ? user.DisplayName : "",
                             IsLocked = accon.IsLocked,
                             IsDeletedBoolean = accon.IsDeleted,
                             AccountType = accon.AccountType,
                             OffboardingAction = offAction.Desc
                         });
            }

            if (accType != null && accType.HasValue)
            {
                query = query.Where(i => i.AccountType == accType);
            }
            if (!string.IsNullOrWhiteSpace(FilterExpression))
            {
                query = query.Where(FilterExpression);
            }
            finalResult.TotalCount = query.Count();
            if (!string.IsNullOrWhiteSpace(sortExpression))
            {
                query = query.OrderBy(sortExpression);
            }
            else
            {
                query = query.OrderByDescending(a => a.AccountName);
            }

            if (maximumRows == -1)
                finalResult.Data = query.ToList();
            else
                finalResult.Data = query.Skip(startRowIndex).Take(maximumRows).ToList();

            return finalResult;
        }
        public List<AccountReconcilation> GetActiveAccountRecons(long diUserId, long endpointSystemId, AccountType accountType)
        {
            return (from accountRecon in CurrentContext.AccountReconcilations
                    where accountRecon.EndpointSystemId == endpointSystemId && accountRecon.UserId == diUserId && !accountRecon.IsDeleted &&
                    accountRecon.AccountType == accountType && !accountRecon.IsLocked
                    select accountRecon).ToList();
        }

        public List<PresentationUserAccount> GetAccountForSelfService(long userId,List<AccountType> accountTypes)
        {
            return (from acc in CurrentContext.AccountReconcilations
                    join endpoint in CurrentContext.EndPoint on acc.EndpointSystemId equals endpoint.Id
                    join epType in CurrentContext.EndpointSystemTypes on endpoint.EndpointSystemTypeId equals epType.Id
                    where acc.UserId == userId && acc.IsDeleted == false && (endpoint.IsPasswordResetAllowed || endpoint.IsUnlockAllowed) && endpoint.ApplicationType != ApplicationType.InhouseSystems && endpoint.IsActive
                    && acc.AccountName.Length > 0 && acc.UserId != null && accountTypes.Contains(acc.AccountType)
                            select new PresentationUserAccount
                            {
                                AcountReconcilationId = acc.Id,
                                EndpointUserId = acc.EndpointUserId,
                                UserName = acc.AccountName,
                                FullName = acc.FullName,
                                AccountLocked = acc.IsLocked,
                                IsAllowPasswordReset = endpoint.IsPasswordResetAllowed,
                                IsAllowAccountUnlock = endpoint.IsUnlockAllowed,
                                AccountType = acc.AccountType,
                                EndpointSystemId = acc.EndpointSystemId,
                                EndPointSystemType = epType.EndpointSystemTypeName,
                                EndpointSystemName = endpoint.DisplayName,
                                ApplicationType = endpoint.ApplicationType,
                                Reserved3 = endpoint.Description
                            }
                    ).OrderBy(i=> i.EndpointSystemName).ToList();
        }

        public PresentationUserAccount GetUserAccountDetailByEndPointSystemId(long userId, long endpointSystemId, long endPointUserId)
        {
            return (from acc in CurrentContext.AccountReconcilations
                    join endpoint in CurrentContext.EndPoint on acc.EndpointSystemId equals endpoint.Id
                    join epType in CurrentContext.EndpointSystemTypes on endpoint.EndpointSystemTypeId equals epType.Id
                    where acc.EndpointSystemId == endpointSystemId && acc.UserId == userId && acc.IsDeleted == false &&
                    acc.EndpointUserId == endPointUserId
                    select new PresentationUserAccount
                    {
                        AcountReconcilationId = acc.Id,
                        EndpointUserId = acc.EndpointUserId,
                        UserName = acc.AccountName,
                        FullName = acc.FullName,
                        LastLogonDate = null,
                        AccountType = acc.AccountType,
                        EndpointSystemId = acc.EndpointSystemId,
                        EndPointSystemType = epType.EndpointSystemTypeName,
                        EndpointSystemName = endpoint.DisplayName
                    }).FirstOrDefault();
        }

        public List<long> GetIDMSAccountOnlyForAttestation(ApplicationType applicationType,long endpointSystemId,bool includeUnMapped, List<string> IDMSAttestationRoleToExclude)
        {
            if(IDMSAttestationRoleToExclude != null && IDMSAttestationRoleToExclude.Count > 0)
            {
                return (from acc in CurrentContext.AccountReconcilations
                        join ent in CurrentContext.EntitlementReconcilations on acc.Id equals ent.AccountReconcilationId
                        join endpoint in CurrentContext.EndPoint on acc.EndpointSystemId equals endpoint.Id
                        where (includeUnMapped ? true : acc.UserId != null) && acc.EndpointSystemId == endpointSystemId && endpoint.ApplicationType == applicationType && ent.IsDeleted == false && acc.IsDeleted == false
                        && (ent.EntitlementType == (int)EntitlementType.Group || (ent.EntitlementType == (int)EntitlementType.Role && !IDMSAttestationRoleToExclude.Contains(ent.Entitlement)))
                        select acc.Id
                 ).Distinct().ToList();
            }
            else
            {
                return (from acc in CurrentContext.AccountReconcilations
                        join ent in CurrentContext.EntitlementReconcilations on acc.Id equals ent.AccountReconcilationId
                        join endpoint in CurrentContext.EndPoint on acc.EndpointSystemId equals endpoint.Id
                        where (includeUnMapped ? true : acc.UserId != null) && acc.EndpointSystemId == endpointSystemId && endpoint.ApplicationType == applicationType && ent.IsDeleted == false && acc.IsDeleted == false
                        && (ent.EntitlementType == (int)EntitlementType.Group || (ent.EntitlementType == (int)EntitlementType.Role && ent.Entitlement != "User" && ent.Entitlement != "Manager"))
                        select acc.Id
                 ).Distinct().ToList();
            }
            
        }
    }
}