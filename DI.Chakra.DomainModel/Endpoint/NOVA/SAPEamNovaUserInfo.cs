using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.DomainModel
{
    public class SAPEamNovaUserInfo : DomainModelBase<SAPEamNovaUserInfo, EntityModel.SAPEamNovaUserInfo>, IEndpointUserInfo
    {
        public SAPEamNovaUserInfo()
        {
            this.SAPEamNovaUserRoles = new List<SAPEamNovaUserRoleInfo>();
        }
        public long EndPointSystemId { get; set; }
        public long UserId { get; set; }
        public string FLINS_UserLogonName { get; set; }
        public long FLINS_SAPUserInfoId { get; set; }
        public string FLINS_BI_UserLogonName { get; set; }
        public long FLINS_BI_SAPUserInfoId { get; set; }
        public string AD_UserLogonName { get; set; }
        public long ADS_UserInfoId { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
        public EndPoint EndPoint { get; set; }
        public List<SAPEamNovaUserRoleInfo> SAPEamNovaUserRoles { get; set; }


        public string DisplayName
        {
            get
            {
                return $"FLINS Account:{FLINS_UserLogonName}, FLINS BI Account:{FLINS_BI_UserLogonName}, AD Account:{AD_UserLogonName}";
            }
        }

        public string AccountName
        {
            get
            {
                return FLINS_UserLogonName;
            }
        }
        public string AccountPassword { get; }

        public List<IEndpointEntitlementInfo> Entitlements
        {
            get
            {
                List<IEndpointEntitlementInfo> li = new List<IEndpointEntitlementInfo>();
                if (SAPEamNovaUserRoles != null)
                {
                    foreach (var item in SAPEamNovaUserRoles)
                    {
                        li.Add(item);
                    }
                }
                return li;
            }
        }

        public static SAPEamNovaUserInfo GetByIdIncludeRoles(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPEamNovaUserInfo>(dataAccessTransaction).GetById(id, nova => nova.SAPEamNovaUserRoles));
            }
        }

        public static IList<SAPEamNovaUserInfo> GetDeletedUsers(long EndpointId, Process objDiscoveryProcess)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<SAPEamNovaUserInfo> listDeletedUsers = new List<SAPEamNovaUserInfo>();
                if (objDiscoveryProcess != null)
                {
                    listDeletedUsers = ORMapList(new RepositoryBase<EntityModel.SAPEamNovaUserInfo>(dataAccessTransaction)
                       .Get(objUser => objUser.EndPointSystemId == EndpointId &&
                           (objUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt ||
                            (objUser.CreatedDate < objDiscoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null)
                        )).ToList());
                }
                return listDeletedUsers;
            }
        }

        public static Dictionary<long, string> GetAllByEndPointSystemId(long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPEamNovaUserInfoRepository(dataAccessTransaction, "").GetByEndPointSystemId(endPointSystemId);
            }
        }

        public static Dictionary<long, string> GetAllByEndPointSystemId(long endPointSystemId, long DiUserId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPEamNovaUserInfoRepository(dataAccessTransaction, "").GetByEndPointSystemId(endPointSystemId, DiUserId);
            }
        }
        public static void RemoveNovaUser(List<SAPEamNovaUserInfo> list, string userName)
        {
            foreach (SAPEamNovaUserInfo user in list)
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new SAPEamNovaUserInfoRepository(dataAccessTransaction, userName).ExecuteQuery(string.Format("DELETE FROM DI_SAP_EamNovaUserRoleInfo WHERE SAPEamNovaUserInfoId =" + user.Id ));
                    dataAccessTransaction.Commit();
                }
                user.Delete();
            }
        }

        public static void SaveUsers(IList<SAPEamNovaUserInfo> list, long EndpointSystemId, string currentUser, AccountType accountType)
        {
            SAPEamNovaUserInfo existingUser = null;
            AccountReconcilation acctRecon = null;
            IList<SAPEamNovaUserInfo> listExistingusers = new List<SAPEamNovaUserInfo>();

            try
            {
                List<AccountReconcilation> exsitngAccountrecon = AccountReconcilation.GetAllByEndpointIdAndAccountType(EndpointSystemId, accountType).ToList();
                if (exsitngAccountrecon == null)
                {
                    exsitngAccountrecon = new List<AccountReconcilation>();
                }
                listExistingusers = SAPEamNovaUserInfo.GetAll();
                var endpoint = EndPoint.GetById(EndpointSystemId);
                var endpointSystemTypeId = endpoint.EndpointSystemTypeId;
                var flinsEp = EndPoint.GetByApplicationType(ApplicationType.HRMS_FLINS);

                foreach (SAPEamNovaUserInfo usr in list)
                {
                    try
                    {
                        User diUser = DomainModel.User.GetById(usr.UserId);
                        existingUser = listExistingusers.Where(t => t.FLINS_UserLogonName!=null && t.FLINS_UserLogonName.Trim().ToUpper() == usr.FLINS_UserLogonName.Trim().ToUpper()
                        ).FirstOrDefault();

                        if (existingUser == null)
                        { 
                            usr.CreatedBy = currentUser;
                            usr.CreatedDate = DateTime.Now;
                            usr.Save(currentUser); 
                            listExistingusers.Add(usr);
                        }
                        else
                        {
                            usr.Id = existingUser.Id;
                            usr.LastModifiedBy = currentUser;
                            usr.LastModifiedDate = DateTime.Now;
                            usr.Save(currentUser);
                        }

                        try
                        {

                            var flinsAccountRecon = AccountReconcilation.GetByEndpointUserId(usr.FLINS_SAPUserInfoId, flinsEp.Id);

                            acctRecon = exsitngAccountrecon.Where(a => a.EndpointSystemId == EndpointSystemId 
                                             && a.AccountName != null
                                             && a.AccountName.Trim().ToUpper() == usr.FLINS_UserLogonName.Trim().ToUpper()
                                             && a.AccountType == accountType).FirstOrDefault();

                            bool isExist = false;
                            if (acctRecon == null)
                            {
                                acctRecon = new AccountReconcilation();
                                AccountReconcilation.UpdateException(acctRecon, true);
                                acctRecon.EndpointSystemId = EndpointSystemId;
                                acctRecon.EndpointUserId = usr.Id;
                                acctRecon.UserId = usr.UserId; 
                               
                            }
                            else
                            {
                                isExist = true;
                                if (acctRecon.IsDeleted)
                                {
                                    AccountReconcilation.UpdateException(acctRecon, true);
                                }
                                acctRecon.LastModifiedDate = DateTime.Now;
                                acctRecon.LastModifiedBy = currentUser;
                            }
                            acctRecon.EndpointUserId = usr.Id;
                            acctRecon.IsDeleted = false;
                            acctRecon.IsLocked = false;
                            acctRecon.AccountName = usr.FLINS_UserLogonName;
                            acctRecon.AccountType = accountType;
                            acctRecon.OffboardingAccountAction = (acctRecon.AccountType == AccountType.User || acctRecon.AccountType == AccountType.Secondary) ? OffboardingAccountActionType.Delete : OffboardingAccountActionType.Supervisor;
                            if (flinsAccountRecon != null)
                            {
                                acctRecon.FullName = flinsAccountRecon.FullName;
                                acctRecon.ValidFromDate = flinsAccountRecon.ValidFromDate;
                                acctRecon.ValidToDate = flinsAccountRecon.ValidToDate;
                            }
                            acctRecon.Save(currentUser);

                            if (!isExist)
                            {
                                exsitngAccountrecon.Add(acctRecon);
                            }
                        }
                        catch (Exception ex)
                        {
                            DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while EAM NOVA SaveUsers AccountRecon.", ex);
                            throw;
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while EAM NOVA SaveUsers.", ex);
                        throw;
                    }
                }


            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while EAM NOVA SaveUsers.", ex);
                throw;
            }
        }


        public override long Save(string user)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(user))
            {
                if (this.Id == 0)
                {
                    this.Id = new SAPEamNovaUserInfoRepository(dataAccessTransaction, user).Insert(ORUnMap());

                }
                else
                {
                    var existingParent = GetById(this.Id, true);
                    foreach (var existingChild in existingParent.SAPEamNovaUserRoles)
                    {
                        if (!this.SAPEamNovaUserRoles.Any(c => c.Id == existingChild.Id))
                            existingChild.Delete(dataAccessTransaction);
                    }
                    foreach (var child in this.SAPEamNovaUserRoles)
                    {
                        child.SAPEamNovaUserInfoId = this.Id;
                        child.Save(dataAccessTransaction);
                    }
                    base.Save(dataAccessTransaction);
                    //new SAPEamNovaUserInfoRepository(dataAccessTransaction, user).Update(ORUnMap());
                }
                dataAccessTransaction.Commit();
            }
            return Id;
        }


        public static SAPEamNovaUserInfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new SAPEamNovaUserInfoRepository(transaction, string.Empty).GetPrimaryByDIUserId(dIUserId, endPointSystemId, accountType));
            }
        }

        public static List<SAPEamNovaUserInfo> GetAllByDIUserId(long dIUserId, long endPointSystemId,List<AccountType> accountTypes)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPEamNovaUserInfoRepository(transaction, string.Empty).GetAllByDIUserId(dIUserId, endPointSystemId,accountTypes));
            }
        }
        public void Delete(long AccessRequestId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction("System"))
            {
                AccountReconcilation accountReconcilation = AccountReconcilation.GetByEndpointUserId(this.Id, this.EndPointSystemId);
                if (accountReconcilation != null)
                {
                    accountReconcilation.ValidToDate = DateTime.Now;
                    accountReconcilation.IsDeleted = true;
                    accountReconcilation.IsLocked = true;
                    accountReconcilation.DeletedDate = dataAccessTransaction.transactionDateTime;
                    accountReconcilation.LastModifiedBy = "System";
                    accountReconcilation.AccessRequestId = AccessRequestId;
                    accountReconcilation.EndpointUserId = 0;  
                    accountReconcilation.Save(dataAccessTransaction);

                    List<EntitlementReconcilation> entReconList = EntitlementReconcilation.GetByEndpointUserId(this.Id, this.EndPointSystemId);
                    foreach (var detail in entReconList)
                    {
                        detail.IsDeleted = true;
                        detail.DeletedDate = dataAccessTransaction.transactionDateTime;
                        detail.LastModifiedBy = "System";
                        detail.LastModifiedDate = dataAccessTransaction.transactionDateTime;
                        detail.AccessRequestId = AccessRequestId;
                        detail.EntitlementId = 0;
                        detail.Save(dataAccessTransaction);
                    }
                }



                var strDeleteQuery = $"DELETE FROM DI_SAP_EamNovaUserInfo WHERE ID = { Id }";

                var strDeleteQueryChild = $"DELETE FROM DI_SAP_EamNovaUserRoleInfo WHERE SAPEamNovaUserInfoId = {Id }";

                SAPEamNovaUserInfo.ExecuteQuery(strDeleteQueryChild);
                SAPEamNovaUserInfo.ExecuteQuery(strDeleteQuery);

                //foreach (SAPEamNovaUserRoleInfo role in this.SAPEamNovaUserRoles)
                //{
                //    role.Delete(dataAccessTransaction);
                //}

                // this.Delete(dataAccessTransaction);
                dataAccessTransaction.Commit();
            }
        }
        public static Dictionary<long, string> GetByMatrixType(string searchString, AdHocActionType adhocActionType, long userId, long endpointId, NovaMatrixType matrixType,string SAPSectionCode, string departmentCode = null)
        {
            Dictionary<long, string> results = new Dictionary<long, string>();
            if (matrixType == NovaMatrixType.User)
            {
                using (var trans = new DataAccessTransaction())
                {
                    results = new NOVAUserRoleMatrixRepository(trans).GetUserMatrixByActionType(endpointId, userId, searchString, adhocActionType,SAPSectionCode);
                }

            }
            else if (matrixType == NovaMatrixType.Management)
            {
                using (var trans = new DataAccessTransaction())
                {
                    results = new NOVAManagementRoleMatrixRepository(trans).GetManagementMatrixByActionType(endpointId, userId, searchString, adhocActionType, departmentCode);
                }
            }
            return results;
        }


        public static User GetUser(long id)
        {
            using (var trans = new DataAccessTransaction())
            {
                return AutoMapper.Mapper.Map<User>( new NOVAUserRoleMatrixRepository(trans).GetUser(id));
            }
        }

    }
}
