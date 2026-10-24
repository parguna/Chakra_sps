using AutoMapper;
using DI.Chakra.Infrastructure;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationUserAccessDetail
    {
        public long Id { get; set; }

        public string userName { get; set; }

        public string EndpointSystemName { get; set; }

        public string EndpointHostName { get; set; }

        public long EndpointSystemID { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public string AccountType { get; set; }

        public string Type { get; set; }

        public long ParentId { get; set; }

        public Nullable<DateTime> LastLogonDate { get; set; }

        public string Institution { get; set; }

        public string Description { get; set; }

        public static PresentationUserAccessDetail ORMap(EntityModel.Presentation.PresentationUserAccessDetail presentationUsrAccDetailEntityModel)
        {
            if (presentationUsrAccDetailEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationUserAccessDetail, PresentationUserAccessDetail>(presentationUsrAccDetailEntityModel);
        }

        public static List<PresentationUserAccessDetail> ORMapList(IList<EntityModel.Presentation.PresentationUserAccessDetail> presentationUsrAccDetailEntityModels)
        {
            return presentationUsrAccDetailEntityModels.Select(ORMap).ToList();
        }

        public static List<PresentationUserAccessDetail> GetUserAccessDetail(long userId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                List<PresentationUserAccessDetail> usrAccessDetailList = new List<PresentationUserAccessDetail>();
                PresentationUserAccessDetail usrAccessDetail = new PresentationUserAccessDetail();

                List<ADSUserInfo> listADSUserInfo = new List<ADSUserInfo>();
                List<SAPUserinfo> listSAPUserInfo = new List<SAPUserinfo>();
                List<SAPEPUserInfo> listSAPEPUserInfo = new List<SAPEPUserInfo>();
                List<InhouseUserInfo> listINHUserInfo = new List<InhouseUserInfo>();
                List<User> listIDASUserInfo = new List<User>();

                User usr = User.GetById(userId);

                if (usr != null)
                {
                    if (usr.Id != 1)
                    {
                        listADSUserInfo = ADSUserInfo.GetUnlockedAccountDetails(usr.Id);
                        listSAPUserInfo = SAPUserinfo.GetActiveAccounts(usr.Id);
                        listSAPEPUserInfo = SAPEPUserInfo.GetActiveAccountByDIUserId(usr.Id);
                        listINHUserInfo = InhouseUserInfo.GetUnlockedAccountsByDIUserId(usr.Id);
                        listIDASUserInfo = User.GetUnlockedAccountsByDIUserId(usr.Id);

                        usrAccessDetailList = GetADSUserAccessDetail(listADSUserInfo, usrAccessDetail, usrAccessDetailList, usr);
                        usrAccessDetailList = GetSAPUserAccessDetail(listSAPUserInfo, usrAccessDetail, usrAccessDetailList, usr);
                        usrAccessDetailList = GetInhouseUserAccessDetail(listINHUserInfo, usrAccessDetail, usrAccessDetailList, usr);
                        usrAccessDetailList = GetSAPEPUserAccessDetail(listSAPEPUserInfo, usrAccessDetail, usrAccessDetailList, usr);
                        usrAccessDetailList = GetIDASUserAccessDetail(listIDASUserInfo, usrAccessDetail, usrAccessDetailList, usr);

                    }
                }
                return usrAccessDetailList;
            }
        }

        private static List<PresentationUserAccessDetail> GetIDASUserAccessDetail(List<User> listIDASUserInfo, PresentationUserAccessDetail usrAccessDetail, List<PresentationUserAccessDetail> usrAccessDetailList, User usr)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                foreach (User idasuser in listIDASUserInfo)
                {
                    if (idasuser != null)
                    {
                        var Endpoint = EndPoint.GetByIdIncludingEndPointSystemType(idasuser.EndPointSystemId);
                        usrAccessDetail = new PresentationUserAccessDetail();
                        usrAccessDetail.ParentId = usr.Id;
                        usrAccessDetail.Id = usrAccessDetailList.Count + 1; // adusr.UserId;
                        usrAccessDetail.userName = usr.UserName;
                        usrAccessDetail.EndpointHostName = Endpoint.DisplayName;
                        usrAccessDetail.EndpointSystemName = Endpoint.EndPointSystemType.EndpointSystemTypeName.ToString();
                        usrAccessDetail.Name = idasuser.UserName;
                        usrAccessDetail.EndpointSystemID = idasuser.EndPointSystemId;
                        usrAccessDetail.FullName = usr.DisplayName;
                      //  usrAccessDetail.LastLogonDate = idasuser.lastLogon;
                        usrAccessDetail.Type = "User"; //Constants.SECURITY_GROUP; // "Groups";
                        usrAccessDetail.userName = usr.UserName;
                        usrAccessDetail.AccountType = AccountReconcilation.GetByDIUserId(usr.Id, usrAccessDetail.EndpointSystemID) == null ? DI.Chakra.Infrastructure.Enums.AccountType.None.ToString() : AccountReconcilation.GetByDIUserId(usr.Id, usrAccessDetail.EndpointSystemID).AccountType.ToString();
                        usrAccessDetailList.Add(usrAccessDetail);

                        var idasRoles = (from idasRole in idasuser.Roles
                                         select new PresentationUserAccessDetail
                                         {
                                             ParentId = usrAccessDetail.Id,
                                             Name = idasRole.RoleName, // adsGrp.description
                                             Description = idasRole.RoleDescription,
                                             Type = "Role"
                                         }).ToList();

                        foreach (var idasRole in idasRoles)
                        {
                            usrAccessDetail = new PresentationUserAccessDetail();
                            usrAccessDetail.EndpointSystemID = idasuser.EndPointSystemId;
                            usrAccessDetail.ParentId = idasRole.ParentId;
                            usrAccessDetail.Id = usrAccessDetailList.Count + 1;
                            usrAccessDetail.Name = idasRole.Name;
                            usrAccessDetail.Description = idasRole.Description;
                            usrAccessDetail.Type = idasRole.Type;
                            usrAccessDetail.AccountType = AccountReconcilation.GetByDIUserId(usr.Id, usrAccessDetail.EndpointSystemID) == null ? DI.Chakra.Infrastructure.Enums.AccountType.None.ToString() : AccountReconcilation.GetByDIUserId(usr.Id, usrAccessDetail.EndpointSystemID).AccountType.ToString();
                            usrAccessDetailList.Add(usrAccessDetail);
                        }
                    }
                }
                return usrAccessDetailList;
            }
        }
        private static List<PresentationUserAccessDetail> GetADSUserAccessDetail(List<ADSUserInfo> listADSUserInfo, PresentationUserAccessDetail usrAccessDetail, List<PresentationUserAccessDetail> usrAccessDetailList, User usr)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                foreach (ADSUserInfo adusr in listADSUserInfo)
                {
                    if (adusr != null)
                    {
                        var Endpoint = EndPoint.GetByIdIncludingEndPointSystemType(adusr.EndPointSystemId);
                        usrAccessDetail = new PresentationUserAccessDetail();
                        usrAccessDetail.ParentId = usr.Id;
                        usrAccessDetail.Id = usrAccessDetailList.Count + 1; // adusr.UserId;
                        usrAccessDetail.userName = usr.UserName;
                        usrAccessDetail.EndpointHostName = Endpoint.DisplayName;
                        usrAccessDetail.EndpointSystemName = Endpoint.EndPointSystemType.EndpointSystemTypeName.ToString();
                        usrAccessDetail.Name = adusr.sAMAccountName;
                        usrAccessDetail.EndpointSystemID = adusr.EndPointSystemId;
                        usrAccessDetail.FullName = usr.DisplayName;
                        usrAccessDetail.LastLogonDate = adusr.lastLogon;
                        usrAccessDetail.Type = "User"; //Constants.SECURITY_GROUP; // "Groups";
                        usrAccessDetail.userName = usr.UserName;
                        usrAccessDetail.AccountType = AccountReconcilation.GetByDIUserId(usr.Id, usrAccessDetail.EndpointSystemID) == null ? DI.Chakra.Infrastructure.Enums.AccountType.None.ToString() : AccountReconcilation.GetByDIUserId(usr.Id, usrAccessDetail.EndpointSystemID).AccountType.ToString();
                        usrAccessDetailList.Add(usrAccessDetail);

                        var adGrpList = (from adsGrp in adusr.ADSGroups
                                         select new PresentationUserAccessDetail
                                         {
                                             ParentId = usrAccessDetail.Id,
                                             Name = adsGrp.cn, // adsGrp.description
                                             Description = adsGrp.description,
                                             Type = "Group"
                                         }).ToList();

                        foreach (var adgrp in adGrpList)
                        {
                            usrAccessDetail = new PresentationUserAccessDetail();
                            usrAccessDetail.EndpointSystemID = adusr.EndPointSystemId;
                            usrAccessDetail.ParentId = adgrp.ParentId;
                            usrAccessDetail.Id = usrAccessDetailList.Count + 1;
                            usrAccessDetail.Name = adgrp.Name;
                            usrAccessDetail.Description = adgrp.Description;
                            usrAccessDetail.Type = adgrp.Type;
                            usrAccessDetail.AccountType = AccountReconcilation.GetByDIUserId(usr.Id, usrAccessDetail.EndpointSystemID) == null ? DI.Chakra.Infrastructure.Enums.AccountType.None.ToString() : AccountReconcilation.GetByDIUserId(usr.Id, usrAccessDetail.EndpointSystemID).AccountType.ToString();
                            usrAccessDetailList.Add(usrAccessDetail);
                        }
                    }
                }
                return usrAccessDetailList;
            }
        }

        private static List<PresentationUserAccessDetail> GetSAPUserAccessDetail(List<SAPUserinfo> listSAPUserInfo, PresentationUserAccessDetail usrAccessDetail, List<PresentationUserAccessDetail> usrAccessDetailList, User usr)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                foreach (SAPUserinfo sapusr in listSAPUserInfo)
                {
                    if (sapusr != null)
                    {
                        var Endpoint = EndPoint.GetByIdIncludingEndPointSystemType(sapusr.EndPointSystemId);
                        usrAccessDetail = new PresentationUserAccessDetail();
                        usrAccessDetail.ParentId = usr.Id;
                        usrAccessDetail.Id = usrAccessDetailList.Count + 1; // sapusr.UserId;
                        usrAccessDetail.userName = usr.UserName;
                        usrAccessDetail.EndpointHostName = Endpoint.DisplayName;
                        usrAccessDetail.EndpointSystemName = Endpoint.EndPointSystemType.EndpointSystemTypeName.ToString();
                        usrAccessDetail.Name = sapusr.UserLogonName;
                        usrAccessDetail.FullName = usr.DisplayName;
                        usrAccessDetail.EndpointSystemID = sapusr.EndPointSystemId;
                        usrAccessDetail.LastLogonDate = sapusr.LastLoginTime;
                        usrAccessDetail.userName = usr.UserName;
                        usrAccessDetail.Type = "User"; // "Roles";
                        usrAccessDetail.AccountType = AccountReconcilation.GetByDIUserId(usr.Id, usrAccessDetail.EndpointSystemID) == null ? DI.Chakra.Infrastructure.Enums.AccountType.None.ToString() : AccountReconcilation.GetByDIUserId(usr.Id, usrAccessDetail.EndpointSystemID).AccountType.ToString();
                        usrAccessDetailList.Add(usrAccessDetail);
                        long pId = usrAccessDetail.Id;
                        var sapRoleList = (from sapRole in sapusr.SAPRoleInfo
                                           select new PresentationUserAccessDetail
                                           {
                                               ParentId = pId,
                                               Name = sapRole.ROLE_CODE,
                                               Description = sapRole.ROLE_DESC,
                                               Type = "Role",
                                               EndpointSystemID = sapusr.EndPointSystemId
                                           }).ToList();

                        //IList<SAPUserRoleTCodeAuth> UserRoleTCodeAuth = new SAPConnectorDA().GetSapUserRoleTCodeAuthReport(sapusr.UserId, sapusr.EndPointSystemId, true);

                        foreach (var saprole in sapRoleList)
                        {
                            usrAccessDetail = new PresentationUserAccessDetail();
                            usrAccessDetail.ParentId = saprole.ParentId;
                            usrAccessDetail.Id = usrAccessDetailList.Count + 1;
                            usrAccessDetail.Name = saprole.Name;
                            usrAccessDetail.Description = saprole.Description;
                            usrAccessDetail.Type = saprole.Type;
                            usrAccessDetail.EndpointSystemID = sapusr.EndPointSystemId;
                            usrAccessDetail.userName = usr.UserName;
                            usrAccessDetail.AccountType = AccountReconcilation.GetByDIUserId(usr.Id, usrAccessDetail.EndpointSystemID) == null ? DI.Chakra.Infrastructure.Enums.AccountType.None.ToString() : AccountReconcilation.GetByDIUserId(usr.Id, usrAccessDetail.EndpointSystemID).AccountType.ToString();
                            usrAccessDetailList.Add(usrAccessDetail);
                        }
                    }
                }
                return usrAccessDetailList;
            }
        }

        private static List<PresentationUserAccessDetail> GetInhouseUserAccessDetail(List<InhouseUserInfo> listINHUserInfo, PresentationUserAccessDetail usrAccessDetail, List<PresentationUserAccessDetail> usrAccessDetailList, User usr)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                foreach (InhouseUserInfo inhusr in listINHUserInfo)
                {
                    if (inhusr != null)
                    {
                        var Endpoint = EndPoint.GetByIdIncludingEndPointSystemType(inhusr.EndPointSystemId);
                        usrAccessDetail = new PresentationUserAccessDetail();
                        usrAccessDetail.Id = usrAccessDetailList.Count + 1;
                        usrAccessDetail.EndpointHostName = Endpoint.DisplayName;
                        usrAccessDetail.EndpointSystemName = Endpoint.EndPointSystemType.EndpointSystemTypeName.ToString();
                        usrAccessDetail.EndpointSystemID = inhusr.EndPointSystemId;
                        usrAccessDetail.Name = inhusr.UserName;
                        usrAccessDetail.FullName = usr.DisplayName;
                        usrAccessDetail.LastLogonDate = inhusr.LastLoginDate;
                        usrAccessDetail.Type = "User";
                        usrAccessDetail.userName = usr.UserName;
                        usrAccessDetail.AccountType = AccountReconcilation.GetByDIUserId(usr.Id, usrAccessDetail.EndpointSystemID) == null ? DI.Chakra.Infrastructure.Enums.AccountType.None.ToString() : AccountReconcilation.GetByDIUserId(usr.Id, usrAccessDetail.EndpointSystemID).AccountType.ToString();
                        usrAccessDetailList.Add(usrAccessDetail);

                        //var inhPermissionList = ORMapList(new InhouseUserInfoRepository(dataAccessTransaction).GetPermissions());

                        foreach (var perm in inhusr.Permissions)
                        {
                            usrAccessDetail = new PresentationUserAccessDetail();
                            usrAccessDetail.ParentId = perm.Id;
                            usrAccessDetail.Id = usrAccessDetailList.Count + 1;
                            usrAccessDetail.Name = perm.Name;
                            usrAccessDetail.Description = perm.Description;
                            usrAccessDetail.Type = "Permission";
                            usrAccessDetail.EndpointSystemID = inhusr.EndPointSystemId;
                            usrAccessDetail.AccountType = AccountReconcilation.GetByDIUserId(usr.Id, usrAccessDetail.EndpointSystemID) == null ? DI.Chakra.Infrastructure.Enums.AccountType.None.ToString() : AccountReconcilation.GetByDIUserId(usr.Id, usrAccessDetail.EndpointSystemID).AccountType.ToString();
                            usrAccessDetailList.Add(usrAccessDetail);
                        }
                    }
                }
                return usrAccessDetailList;
            }
        }

        private static List<PresentationUserAccessDetail> GetSAPEPUserAccessDetail(List<SAPEPUserInfo> listSAPEPUserInfo, PresentationUserAccessDetail usrAccessDetail, List<PresentationUserAccessDetail> usrAccessDetailList, User usr)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                foreach (SAPEPUserInfo sapepusr in listSAPEPUserInfo)
                {
                    if (sapepusr != null)
                    {
                        var Endpoint = EndPoint.GetByIdIncludingEndPointSystemType(sapepusr.EndPointSystemId);
                        usrAccessDetail = new PresentationUserAccessDetail();
                        usrAccessDetail.Id = usrAccessDetailList.Count + 1;
                        usrAccessDetail.EndpointHostName = Endpoint.DisplayName;
                        usrAccessDetail.EndpointSystemName = Endpoint.EndPointSystemType.EndpointSystemTypeName.ToString();
                        usrAccessDetail.EndpointSystemID = sapepusr.EndPointSystemId;
                        usrAccessDetail.Name = sapepusr.UserName;
                        usrAccessDetail.FullName = usr.DisplayName;
                        usrAccessDetail.Type = "User";
                        usrAccessDetail.userName = usr.UserName;
                        usrAccessDetail.AccountType = AccountReconcilation.GetByDIUserId(usr.Id, usrAccessDetail.EndpointSystemID) == null ? DI.Chakra.Infrastructure.Enums.AccountType.None.ToString() : AccountReconcilation.GetByDIUserId(usr.Id, usrAccessDetail.EndpointSystemID).AccountType.ToString();
                        usrAccessDetailList.Add(usrAccessDetail);

                        var sapepRoleList = (from sapepRole in sapepusr.SAPEPRoles
                                             select new PresentationUserAccessDetail
                                             {
                                                 ParentId = usrAccessDetail.Id,
                                                 Name = sapepRole.RoleName,
                                                 Type = "Role",
                                                 Description = sapepRole.RoleDescription,
                                                 EndpointSystemID = sapepusr.EndPointSystemId
                                             }
                                                      ).ToList();
                        foreach (var sapeprole in sapepRoleList)
                        {
                            usrAccessDetail = new PresentationUserAccessDetail();
                            usrAccessDetail.ParentId = sapeprole.ParentId;
                            usrAccessDetail.Id = usrAccessDetailList.Count + 1;
                            usrAccessDetail.Name = sapeprole.Name;
                            usrAccessDetail.Description = sapeprole.Description;
                            usrAccessDetail.Type = sapeprole.Type;
                            usrAccessDetail.EndpointSystemID = sapepusr.EndPointSystemId;
                            usrAccessDetail.AccountType = AccountReconcilation.GetByDIUserId(usr.Id, usrAccessDetail.EndpointSystemID) == null ? DI.Chakra.Infrastructure.Enums.AccountType.None.ToString() : AccountReconcilation.GetByDIUserId(usr.Id, usrAccessDetail.EndpointSystemID).AccountType.ToString();
                            usrAccessDetailList.Add(usrAccessDetail);
                        }
                    }
                }
                return usrAccessDetailList;
            }
        }


    }
}
