using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationUserAccount
    {
        public long AcountReconcilationId { get; set; }

        public long EndpointUserId { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public Nullable<DateTime> LastLogonDate { get; set; }

        public AccountType AccountType { get; set; }

        public EndpointType EndPointSystemType { get; set; }

        public string EndpointSystemName { get; set; }

        public long EndpointSystemId { get; set; }

        public string Reserved1 { get; set; }

        public string Reserved2 { get; set; }

        public string Reserved3 { get; set; }

        public string AccountStatus { get; set; }

        public bool IsLocked { get; set; }
        public bool AccountLocked { get; set; }

        public bool IsAllowPasswordReset { get; set; }

        public bool IsAllowAccountUnlock { get; set; }
        public ApplicationType ApplicationType { get; set; }

        public bool AllowReset
        {
            get; set;
        } = true;

        private DataAccessTransaction dataAccessTransaction;

        public PresentationUserAccount(DataAccessTransaction dataAccessTransaction)
        {
            this.dataAccessTransaction = dataAccessTransaction;
        }
        public PresentationUserAccount()
        {

        }

        public static IList<PresentationUserAccount> GetUserAccountDetail(long userID, bool pwdReset)
        {
            IList<AccountReconcilation> lst = AccountReconcilation.GetByUser(userID, true);

            if (pwdReset)
                lst = lst.Where(item => item.EndPoint.IsPasswordResetAllowed == true).ToList();

            return (from acnt in lst
                    where acnt.EndPoint.IsPasswordResetAllowed = true
                    select new PresentationUserAccount
                    {
                        AcountReconcilationId = acnt.Id,
                        EndpointUserId = acnt.EndpointUserId,
                        UserName = acnt.AccountName,
                        FullName = acnt.FullName,
                        LastLogonDate = null,
                        AccountType = acnt.AccountType,
                        EndpointSystemId = acnt.EndpointSystemId,
                        EndPointSystemType = acnt.EndPoint.EndPointSystemType.EndpointSystemTypeName,
                        EndpointSystemName = acnt.EndPoint.DisplayName
                    }).ToList();
        }

        public static IList<PresentationUserAccount> GetUserSelfServiceAccountDetail(long userID, PasswordActionType selfServiceType)
        {

            var SelfServiceAccountType = MasterConfigHelper.SelfServiceAccountType;

            List<DI.Chakra.Infrastructure.Enums.AccountType> accountType = GetServiceAccountType(SelfServiceAccountType);

            using (var transcation = new DataAccessTransaction())
            {
                var data = ORMapList(new AccountReconcilationRepository(transcation).GetAccountForSelfService(userID, accountType));
                var excludeList = new List<PresentationUserAccount>();
                foreach (var item in data.Where(i=> i.ApplicationType == ApplicationType.SAP_SRP))
                {
                    if(data.Exists(i=> i.UserName.Equals(item.UserName, StringComparison.InvariantCultureIgnoreCase) && i.ApplicationType == ApplicationType.SAP_SUS))
                    {
                        item.AllowReset = true;
                    }
                    else
                    {
                        item.AllowReset = false;
                    }
                }

                return data;
            }
        }
        public static List<PresentationUserAccount> ORMapList(IList<EntityModel.Presentation.PresentationUserAccount> PresentationUserAccountEntityModel)
        {
            return PresentationUserAccountEntityModel.Select(ORMap).ToList();
        }
        public static PresentationUserAccount ORMap(EntityModel.Presentation.PresentationUserAccount PresentationUserAccountEntityModel)
        {
            if (PresentationUserAccountEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationUserAccount, PresentationUserAccount>(PresentationUserAccountEntityModel);
        }
        public static IList<PresentationUserAccount> GetAccountDetailByAccountType(long userID, AccountType accountType)
        {
            IList<AccountReconcilation> lst = AccountReconcilation.GetByUser(userID, true);

            return (from acnt in lst
                    where acnt.AccountType == accountType
                    select new PresentationUserAccount
                    {
                        AcountReconcilationId = acnt.Id,
                        EndpointUserId = acnt.EndpointUserId,
                        UserName = acnt.AccountName,
                        FullName = acnt.FullName,
                        LastLogonDate = null,
                        AccountType = acnt.AccountType,
                        EndpointSystemId = acnt.EndpointSystemId,
                        EndPointSystemType = acnt.EndPoint.EndPointSystemType.EndpointSystemTypeName,
                        EndpointSystemName = acnt.EndPoint.DisplayName
                    }).ToList();
        }
        public static IList<PresentationUserAccount> GetAllUserAccountDetail()
        {
            IList<AccountReconcilation> lst = AccountReconcilation.GetAllUser(true);

            lst = lst.Where(item => item.EndPoint.IsPasswordResetAllowed == true).ToList();

            return (from acnt in lst
                    select new PresentationUserAccount
                    {
                        AcountReconcilationId = acnt.Id,
                        EndpointUserId = acnt.EndpointUserId,
                        UserName = acnt.AccountName,
                        FullName = acnt.FullName,
                        LastLogonDate = null,
                        AccountType = acnt.AccountType,
                        EndpointSystemId = acnt.EndpointSystemId,
                        EndPointSystemType = acnt.EndPoint.EndPointSystemType.EndpointSystemTypeName,
                        EndpointSystemName = acnt.EndPoint.DisplayName
                    }).ToList();
        }
        public static IList<PresentationUserAccount> GetUserAccountDetailReset(long userID)
        {
            IList<AccountReconcilation> lst = AccountReconcilation.GetByUser(userID, true);

            return (from acnt in lst
                    select new PresentationUserAccount
                    {
                        AcountReconcilationId = acnt.Id,
                        EndpointUserId = acnt.EndpointUserId,
                        UserName = acnt.AccountName,
                        FullName = acnt.FullName,
                        LastLogonDate = null,
                        AccountType = acnt.AccountType,
                        EndpointSystemId = acnt.EndpointSystemId,
                        EndPointSystemType = acnt.EndPoint.EndPointSystemType.EndpointSystemTypeName,
                        EndpointSystemName = acnt.EndPoint.DisplayName
                    }).ToList();
        }

        public static PresentationUserAccount GetUserAccountDetailByEndPointSystemId(long userID, long endPointSystemId, long endPointUserId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new AccountReconcilationRepository(dataAccessTransaction).GetUserAccountDetailByEndPointSystemId(userID, endPointSystemId, endPointUserId));
            }
        }

        public static IList<PresentationUserAccount> GetADAccountDetail(long endpointUserId)
        {
            IList<PresentationUserAccount> presentationUserAccountList = new List<PresentationUserAccount>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                IList<ADSUserInfo> adsUserInfoList = ADSUserInfo.GetUnlockedAccountDetails(endpointUserId);
                foreach (ADSUserInfo adusr in adsUserInfoList)
                {
                    if (adusr != null)
                    {
                        PresentationUserAccount userAccount = new PresentationUserAccount
                        {
                            EndpointUserId = adusr.Id,
                            UserName = adusr.AccountName,
                            FullName = adusr.displayName,
                           // AccountType = adusr.AccountType,
                            LastLogonDate = adusr.lastLogon,
                            EndpointSystemId = adusr.EndPointSystemId,
                            EndPointSystemType = adusr.EndPointSystem.EndPointSystemType.EndpointSystemTypeName,
                            EndpointSystemName = adusr.EndPointSystem.DisplayName
                        };
                        presentationUserAccountList.Add(userAccount);
                    }
                }
            }
            return presentationUserAccountList;
        }

        //public static IList<PresentationUserAccount> GetUserAccountDetailByADApplication(long endpointSystemId)
        //{
        //    IList<PresentationUserAccount> presentationUserAccountList = new List<PresentationUserAccount>();
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        List<ADSUserInfo> adsUserInfoList = ADSUserInfo.GetUnlockedAccountsIncludingEndpointSystemType(endpointSystemId);
        //        foreach (ADSUserInfo adusr in adsUserInfoList)
        //        {
        //            if (adusr != null)
        //            {
        //                PresentationUserAccount UserAccount = new PresentationUserAccount
        //                {
        //                    EndpointUserId = adusr.Id,
        //                    UserName = adusr.AccountName,
        //                    FullName = adusr.displayName,
        //                    AccountType = adusr.AccountType,
        //                    LastLogonDate = adusr.lastLogon,
        //                    EndpointSystemId = adusr.EndPointSystemId,
        //                    EndPointSystemType = adusr.EndPointSystem.EndPointSystemType.EndpointSystemTypeName,
        //                    EndpointSystemName = adusr.EndPointSystem.DisplayName
        //                };
        //                presentationUserAccountList.Add(UserAccount);
        //            }
        //        }
        //    }
        //    return presentationUserAccountList;
        //}

        public static IList<PresentationUserAccount> GetInhouseAccountDetail(long endpointUserId)
        {
            IList<PresentationUserAccount> presentationUserAccountList = new List<PresentationUserAccount>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                IList<InhouseUserInfo> inhouseUserInfoList = InhouseUserInfo.GetActiveAccountsByDIUserId(endpointUserId);
                foreach (InhouseUserInfo inhusr in inhouseUserInfoList)
                {
                    if (inhusr != null)
                    {
                        PresentationUserAccount presentationUserAccount = new PresentationUserAccount
                        {
                            EndpointUserId = inhusr.Id,
                            UserName = inhusr.AccountName,
                            FullName = inhusr.DisplayName,
                            //AccountType = inhusr.AccountType,
                            LastLogonDate = inhusr.LastLoginDate,
                            EndpointSystemId = inhusr.EndPointSystemId,
                            EndPointSystemType = inhusr.EndPoint.EndPointSystemType.EndpointSystemTypeName,
                            EndpointSystemName = inhusr.EndPoint.DisplayName
                        };
                        presentationUserAccountList.Add(presentationUserAccount);
                    }
                }
            }
            return presentationUserAccountList;
        }

        //public static IList<PresentationUserAccount> GetUserAccountDetailByInhouseApplication(long endpointSystemId)
        //{
        //    IList<PresentationUserAccount> presentationUserAccountList = new List<PresentationUserAccount>();
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        IList<InhouseUserInfo> inhUserInfoList = InhouseUserInfo.GetActiveAccountsIncludingEndpoint(endpointSystemId);
        //        foreach (InhouseUserInfo inhusr in inhUserInfoList)
        //        {
        //            if (inhusr != null)
        //            {
        //                PresentationUserAccount UserAccount = new PresentationUserAccount
        //                {
        //                    EndpointUserId = inhusr.Id,
        //                    UserName = inhusr.AccountName,
        //                    FullName = inhusr.DisplayName,
        //                    AccountType = inhusr.AccountType,
        //                    LastLogonDate = inhusr.LastLoginDate,
        //                    EndpointSystemId = inhusr.EndPointSystemId,
        //                    EndPointSystemType = inhusr.EndPoint.EndPointSystemType.EndpointSystemTypeName,
        //                    EndpointSystemName = inhusr.EndPoint.DisplayName
        //                };
        //                presentationUserAccountList.Add(UserAccount);
        //            }
        //        }
        //    }
        //    return presentationUserAccountList;
        //}





        public static IList<PresentationUserAccount> GetSAPAccountDetail(long endpointUserId)
        {
            IList<PresentationUserAccount> presentationUserAccountList = new List<PresentationUserAccount>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                IList<SAPUserinfo> sapUserinfoList = SAPUserinfo.GetActiveAccounts(endpointUserId);
                foreach (SAPUserinfo sapusr in sapUserinfoList)
                {
                    if (sapusr != null)
                    {
                        PresentationUserAccount presentationUserAccount = new PresentationUserAccount
                        {
                            EndpointUserId = sapusr.Id,
                            UserName = sapusr.AccountName,
                            FullName = sapusr.Firstname + " " + sapusr.Lastname,
                            //AccountType = sapusr.AccountType,
                            LastLogonDate = sapusr.LastLoginTime,
                            EndpointSystemId = sapusr.EndPointSystemId,
                            EndPointSystemType = sapusr.EndPointSystem.EndPointSystemType.EndpointSystemTypeName,
                            EndpointSystemName = sapusr.EndPointSystem.DisplayName
                        };
                        presentationUserAccountList.Add(presentationUserAccount);
                    }
                }
            }
            return presentationUserAccountList;
        }


        public static IList<PresentationUserAccount> GetUserAccountDetailBySAPApplication(long endpointSystemId)
        {
            IList<PresentationUserAccount> presentationUserAccountList = new List<PresentationUserAccount>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                IList<SAPUserinfo> sapUserinfoList = SAPUserinfo.GetByEndPointSystemIdIsLockedIncludingEndpointSystem(endpointSystemId);
                foreach (SAPUserinfo sapusr in sapUserinfoList)
                {
                    if (sapusr != null)
                    {
                        PresentationUserAccount presentationUserAccount = new PresentationUserAccount
                        {
                            EndpointUserId = sapusr.Id,
                            UserName = sapusr.AccountName,
                            FullName = sapusr.Firstname + " " + sapusr.Lastname,
                            //AccountType = sapusr.AccountType,
                            LastLogonDate = sapusr.LastLoginTime,
                            EndpointSystemId = sapusr.EndPointSystemId,
                            EndPointSystemType = sapusr.EndPointSystem.EndPointSystemType.EndpointSystemTypeName,
                            EndpointSystemName = sapusr.EndPointSystem.DisplayName
                        };
                        presentationUserAccountList.Add(presentationUserAccount);
                    }
                }
            }
            return presentationUserAccountList;
        }

        public static List<AccountType> GetServiceAccountType(string value)
        {
            List<AccountType> accountTypes = new List<AccountType>();

            switch (value)
            {
                case "1000":
                    accountTypes.Add(AccountType.User);
                    break;
                case "0001":
                    accountTypes.Add(AccountType.Secondary);
                    break;
                case "0100":
                    accountTypes.Add(AccountType.Service);
                    break;
                case "0010":
                    accountTypes.Add(AccountType.Shared);
                    break;
                case "1001":
                    accountTypes.Add(AccountType.User);
                    accountTypes.Add(AccountType.Secondary);
                    break;
                case "1100":
                    accountTypes.Add(AccountType.User);
                    accountTypes.Add(AccountType.Service);
                    break;
                case "1010":
                    accountTypes.Add(AccountType.User);
                    accountTypes.Add(AccountType.Shared);
                    break;
                case "1011":
                    accountTypes.Add(AccountType.User);
                    accountTypes.Add(AccountType.Secondary);
                    accountTypes.Add(AccountType.Shared);
                    break;
                case "1110":
                    accountTypes.Add(AccountType.User);
                    accountTypes.Add(AccountType.Service);
                    accountTypes.Add(AccountType.Shared);
                    break;
                case "1111":
                    accountTypes.Add(AccountType.User);
                    accountTypes.Add(AccountType.Service);
                    accountTypes.Add(AccountType.Shared);
                    accountTypes.Add(AccountType.Secondary);
                    break;
                default:
                    accountTypes.Add(AccountType.User);
                    accountTypes.Add(AccountType.Secondary);
                    break;
            }
            return accountTypes;
        }




        //public static IList<PresentationUserAccount> GetSAPEPAccountDetail(long endpointUserId)
        //{
        //    IList<PresentationUserAccount> presentationUserAccountList = new List<PresentationUserAccount>();
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        IList<SAPEPUserInfo> sapepUserInfoList = SAPEPUserInfo.GetActiveAccountByDIUserId(endpointUserId);
        //        foreach (SAPEPUserInfo sapepusr in sapepUserInfoList)
        //        {
        //            if (sapepusr != null)
        //            {
        //                PresentationUserAccount presentationUserAccount = new PresentationUserAccount
        //                {
        //                    EndpointUserId = sapepusr.Id,
        //                    UserName = sapepusr.AccountName,
        //                    FullName = sapepusr.DisplayName,
        //                    LastLogonDate = sapepusr.LastModifiedDate,
        //                    EndpointSystemId = sapepusr.EndPointSystemId,
        //                    EndPointSystemType = sapepusr.EndPointSystem.EndPointSystemType.EndpointSystemTypeName,
        //                    EndpointSystemName = sapepusr.EndPointSystem.DisplayName
        //                };
        //                presentationUserAccountList.Add(presentationUserAccount);
        //            }
        //        }
        //    }
        //    return presentationUserAccountList;
        //}

        //public static IList<PresentationUserAccount> GetUserAccountDetailBySAPEPApplication(long endpointSystemId)
        //{
        //    IList<PresentationUserAccount> presentationUserAccountList = new List<PresentationUserAccount>();
        //    IList<SAPEPUserInfo> listSAPEPUserInfo = new List<SAPEPUserInfo>();
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        IList<SAPEPUserInfo> sapepUserInfoList = SAPEPUserInfo.GetUnlockedSAPEPUsersByEndPointSystemId(endpointSystemId);
        //        foreach (SAPEPUserInfo sapepusr in listSAPEPUserInfo)
        //        {
        //            if (sapepusr != null)
        //            {
        //                PresentationUserAccount presentationUserAccount = new PresentationUserAccount
        //                {
        //                    EndpointUserId = sapepusr.Id,
        //                    UserName = sapepusr.AccountName,
        //                    FullName = sapepusr.DisplayName,
        //                    AccountType = sapepusr.AccountType,
        //                    LastLogonDate = sapepusr.LastModifiedDate,
        //                    EndpointSystemId = sapepusr.EndPointSystemId,
        //                    EndPointSystemType = sapepusr.EndPointSystem.EndPointSystemType.EndpointSystemTypeName,
        //                    EndpointSystemName = sapepusr.EndPointSystem.DisplayName
        //                };
        //                presentationUserAccountList.Add(presentationUserAccount);
        //            }
        //        }
        //    }
        //    return presentationUserAccountList;
        //}

        //public static IList<PresentationUserAccount> GetUserAccountDetailByApplication(long endpointSystemId)
        //{
        //    IList<PresentationUserAccount> presentationUserAccountList = new List<PresentationUserAccount>();

        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        PresentationUserAccount.GetUserAccountDetailByADApplication(endpointSystemId).ToList().ForEach(lq =>
        //        {
        //            presentationUserAccountList.Add(lq);
        //        });
        //        PresentationUserAccount.GetUserAccountDetailByLotusNotesApplication(endpointSystemId).ToList().ForEach(lq =>
        //        {
        //            presentationUserAccountList.Add(lq);
        //        });
        //        PresentationUserAccount.GetUserAccountDetailByLinuxApplication(endpointSystemId).ToList().ForEach(lq =>
        //        {
        //            presentationUserAccountList.Add(lq);
        //        });
        //        PresentationUserAccount.GetUserAccountDetailByInhouseApplication(endpointSystemId).ToList().ForEach(lq =>
        //        {
        //            presentationUserAccountList.Add(lq);
        //        });
        //        PresentationUserAccount.GetUserAccountDetailByOEBSApplication(endpointSystemId).ToList().ForEach(lq =>
        //        {
        //            presentationUserAccountList.Add(lq);
        //        });

        //        PresentationUserAccount.GetUserAccountDetailByOUMApplication(endpointSystemId).ToList().ForEach(lq =>
        //        {
        //            presentationUserAccountList.Add(lq);
        //        });

        //        PresentationUserAccount.GetUserAccountDetailBySAPApplication(endpointSystemId).ToList().ForEach(lq =>
        //        {
        //            presentationUserAccountList.Add(lq);
        //        });


        //        PresentationUserAccount.GetUserAccountDetailBySAPEPApplication(endpointSystemId).ToList().ForEach(lq =>
        //        {
        //            presentationUserAccountList.Add(lq);

        //        });
        //        return presentationUserAccountList;
        //    }
        //}
    }
}
