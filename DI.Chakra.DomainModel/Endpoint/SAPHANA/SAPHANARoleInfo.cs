using AutoMapper;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DI.Chakra.DomainModel
{
    public class SAPHANARoleInfo : DomainModelBase<SAPHANARoleInfo, EntityModel.SAPHANARoleInfo>, IEndpointEntitlementInfo
    {
        

        public long EndPointSystemId { get; set; }

        public string ROLE_CODE { get; set; }

        public string ROLE_DESC { get; set; }

        public string TargetSysId { get; set; }

        public string RoleType { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public List<SAPHANAUserinfo> SAPUserinfo { get; set; }

        public int? Version { get; set; }
        public bool? CanRequest { get; set; }
        public string UserDefinedDesc { get; set; }
        public long SAP_ROLEID
        { get; private set; }

        public string EntitlementName
        {
            get { return ROLE_CODE; }
        }

        public string EntitlementDesc
        {
            get { return ROLE_DESC; }
        }

        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Role; }
        }


        public string Reserved1
        {
            get
            {
                return string.Empty;
            }
        }

        public string Reserved2
        {
            get
            {
                return string.Empty;
            }
        }

        public string Reserved3
        {
            get
            {
                return string.Empty;
            }
        }

        public string Reserved4
        {
            get
            {
                return string.Empty;
            }
        }

        public string Reserved5
        {
            get
            {
                return string.Empty;
            }
        }


        public string Reserved6
        {
            get
            {
                return string.Empty;
            }
        }

        public string Reserved7
        {
            get
            {
                return string.Empty;
            }
        }

        public string Reserved8
        {
            get
            {
                return string.Empty;
            }
        }

        public string Reserved9
        {
            get
            {
                return string.Empty;
            }
        }

        public string Reserved10
        {
            get
            {
                return string.Empty;
            }
        }

        public static SAPHANARoleInfo GetByRoleCode(string roleCode, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANARoleInfo>(dataAccessTransaction).Get(obj => obj.ROLE_CODE == roleCode && obj.EndPointSystemId == endPointSystemId).FirstOrDefault());
            }
        }

        public static List<SAPHANARoleInfo> GetByEndpointSystem(long endPointSysemId, string entitlement)
        {
            using (var transaction = new DataAccessTransaction())
            {
                entitlement = entitlement.Replace(".*", "*").Replace("*", ".*");
                var rx = new Regex(entitlement, RegexOptions.IgnoreCase);
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleInfo>(transaction)
                    .Get(ent => ent.EndPointSystemId == endPointSysemId).Where(x => rx.IsMatch(x.ROLE_CODE)).ToList());
            }

        }
        public static List<SAPHANARoleInfo> GetByRoleCodesAndEndpointId(long endPointSystemId, string[] roleCodes)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleInfo>(dataAccessTransaction).Get(obj => obj.EndPointSystemId == endPointSystemId && roleCodes.Contains(obj.ROLE_CODE)).ToList());
            }
        }
        public static SAPHANARoleInfo GetByRoleCode(string roleCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANARoleInfo>(dataAccessTransaction).Get(obj => obj.ROLE_CODE == roleCode).FirstOrDefault());
            }
        }

        public static List<SAPHANARoleInfo> GetByEndpointSystemId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleInfo>(dataAccessTransaction).Get(obj => obj.EndPointSystemId == endPointSystemId).ToList());
            }
        }
        public static List<SAPHANARoleInfo> GetRolesWithEmptyRoleType(long endPointSystemId, string RoleType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleInfo>(dataAccessTransaction).Get(obj => obj.EndPointSystemId == endPointSystemId && string.IsNullOrEmpty(obj.RoleType)).ToList());
            }
        }

        public static SAPHANARoleInfo GetSAPUserInfoByRoleCodeIncludingSAPUserInfo(string roleCode, long endPointSystemId)
        {

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANARoleInfo>(dataAccessTransaction).Get(obj => obj.ROLE_CODE == roleCode && obj.EndPointSystemId == endPointSystemId, obj => obj.SAPUserinfo).FirstOrDefault());
            }

        }

        public static List<SAPHANARoleInfo> GetByModifiedDate(long endPointSystemId, DateTime? modifiedDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleInfo>(dataAccessTransaction).Get(obj => obj.EndPointSystemId == endPointSystemId && obj.LastModifiedDate < modifiedDate).ToList());
            }
        }

        // deleted items based on last modified or created date (where last modified is null)
        public static List<SAPHANARoleInfo> GetDeletedByModifiedDate(long endPointSystemId, DateTime? modifiedDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleInfo>(dataAccessTransaction).Get(obj => obj.EndPointSystemId == endPointSystemId && 
                ((obj.LastModifiedDate == null && obj.CreatedDate < modifiedDate) || obj.LastModifiedDate < modifiedDate)).ToList());
            }
        }

        public static List<SAPHANARoleInfo> GetByEndpointId(long endPointSystemId, string roleCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleInfo>(dataAccessTransaction).Get(obj => obj.EndPointSystemId == endPointSystemId && obj.ROLE_CODE == roleCode).ToList());
            }
        }

        public static List<SAPHANARoleInfo> GetByRoleId(long roleId, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleInfo>(dataAccessTransaction).Get(obj => obj.EndPointSystemId == endPointSystemId && obj.Id == roleId).ToList());
            }
        }

        public static List<SAPHANARoleInfo> GetByRoleId(long roleId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleInfo>(dataAccessTransaction).Get(obj => obj.Id == roleId).ToList());
            }
        }

        public static void SaveRoles(IList<SAPHANARoleInfo> list, long endpointSystemId, string currentUser)
        {

            //using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            //{
            SAPHANARoleInfo sapexistingRole = null;

                foreach (SAPHANARoleInfo role in list)
                {

                    sapexistingRole = SAPHANARoleInfo.GetByRoleCode(role.ROLE_CODE, endpointSystemId);

                    if (sapexistingRole == null)
                    {
                        role.CanRequest = true;
                        role.Save(currentUser);

                    }
                    else
                    {
                        role.Id = sapexistingRole.Id;
                        role.CanRequest = sapexistingRole.CanRequest;
                        role.UserDefinedDesc = sapexistingRole.UserDefinedDesc;
                        role.CreatedBy = sapexistingRole.CreatedBy;
                        role.CreatedDate = sapexistingRole.CreatedDate;

                        role.Save(currentUser);
                        //_context.SAPRoleInfos.Attach(sapexistingRole);
                        //_context.Entry(sapexistingRole).State = EntityState.Modified;
                        //_context.SaveChanges();

                    }
                //}

                //dataAccessTransaction.Commit();

            }
        }

        public static void SaveRoles(IList<SAPHANARoleInfo> list, string currentUser, Dictionary<string, SAPHANARoleInfo> existingRoles)
        {
            SAPHANARoleInfo sapexistingRole = null;

            foreach (SAPHANARoleInfo role in list)
            {
                if (existingRoles.ContainsKey(role.ROLE_CODE))
                {
                    sapexistingRole = existingRoles[role.ROLE_CODE];
                }
                else
                {
                    sapexistingRole = null;
                }

                if (sapexistingRole == null)
                {
                    role.CanRequest = true;
                    role.Save(currentUser);
                    existingRoles.Add(role.ROLE_CODE, role);
                }
                else
                {
                    role.Id = sapexistingRole.Id;
                    role.CanRequest = sapexistingRole.CanRequest;
                    role.UserDefinedDesc = sapexistingRole.UserDefinedDesc;
                    role.CreatedBy = sapexistingRole.CreatedBy;
                    role.CreatedDate = sapexistingRole.CreatedDate;

                    role.Save(currentUser);
                    //_context.SAPRoleInfos.Attach(sapexistingRole);
                    //_context.Entry(sapexistingRole).State = EntityState.Modified;
                    //_context.SaveChanges();

                }
            }
        }

        public IList<SAPHANARoleInfo> GetDeletedSapRoles(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPHANARoleInfo> listDeletedSapRoles = new List<SAPHANARoleInfo>();
                objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemId(endpointSystemId).FirstOrDefault();

                if (objDiscoveryProcess != null)
                {
                    Process process = Process.GetByProcessTypeKeyId(objDiscoveryProcess.Id, Infrastructure.Enums.ProcessType.Discovery);
                    if (process != null)
                    {
                        listDeletedSapRoles = SAPHANARoleInfo.GetByModifiedDate(endpointSystemId, process.ProcessStartAt);
                    }
                }

                return listDeletedSapRoles;

            }
        }

        public bool IsSAPRoleValid(string value, long EndpointSystemId)
        {

            var entitlements = value.Split(',');
            var isExists = false;

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                foreach (var entitlement in entitlements)
                {
                    if (!string.IsNullOrEmpty(entitlement))
                    {
                        isExists = DoesExist(entitlement);
                        if (!isExists)
                        {
                            break;
                        }
                    }
                }
            }
            return isExists;
        }

        public static bool DoesExist(string value, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPHANARoleInfo>(dataAccessTransaction).Any(info => info.ROLE_CODE == value && info.EndPointSystemId == endpointSystemId);
            }
        }
        public static bool DoesExist(string value)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.SAPHANARoleInfo>(dataAccessTransaction).Any(info => info.ROLE_CODE == value);
            }
        }
        public static bool IsSAPRoleValid(long endpointSystemId, string value)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPHANARoleInfo>(dataAccessTransaction).Any(grp => grp.ROLE_CODE == value && grp.EndPointSystemId == endpointSystemId);
            }
        }
        public static void UpdateInfo(DataAccessTransaction dataAccessTransaction, long endpointSystemId, long userId)
        {
            new SAPHANARoleInfoRepository(dataAccessTransaction).UpdateInfo(endpointSystemId, userId);
        }
        public bool AddSAPUserIntoRoles(SAPUserinfo sapUsr)
        {
            bool isDataSaved = false;
            if (!(sapUsr != null && sapUsr.Roles != null))
                return isDataSaved;
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var newSapUser = DomainModel.SAPHANAUserinfo.GetById(sapUsr.Id);

                if (newSapUser != null)
                {
                    foreach (string strRole in sapUsr.Roles)
                    {
                        SAPHANARoleInfo roleinfo = DomainModel.SAPHANARoleInfo.GetByRoleCode(strRole, sapUsr.EndPointSystemId);

                        if (roleinfo != null)
                        {
                            SAPHANARoleInfo existingrole = newSapUser.SAPRoleInfo.FirstOrDefault(role => role.ROLE_CODE == roleinfo.ROLE_CODE);
                            if (existingrole == null)
                            {
                                newSapUser.SAPRoleInfo.Add(roleinfo);
                            }
                        }
                    }
                }
                newSapUser.Save(dataAccessTransaction);

                UpdateInfo(dataAccessTransaction, sapUsr.EndPointSystemId, sapUsr.Id);

                dataAccessTransaction.Commit();
                isDataSaved = true;
            }
            return isDataSaved;
        }

        public static void UpdateRolesAsSingleIfNull(long endpointSystemId, string currentUser)
        {
            var ItemstobeUpdate = GetRolesWithEmptyRoleType(endpointSystemId, "");
            ItemstobeUpdate.ForEach(l => { l.RoleType = Constants.SINGLE; l.Save(currentUser); });
        }

        public static List<SAPHANARoleInfo> GetListByUserId(long userId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new SAPHANARoleInfoRepository(transaction).GetListByUserId(userId));
            }
        }


        public static List<DI.Chakra.EntityModel.Presentation.PresentationSAPHANAUsersToRolesReport> GetUserRoles(long userId, long endpointSystemID)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new SAPHANARoleInfoRepository(transaction).GetUserRoles(userId, endpointSystemID);
            }
        }

        public static List<SAPHANARoleInfo> GetListByRoleType(long userId)
        {
            List<SAPHANARoleInfo> roleInfo = new List<SAPHANARoleInfo>();
            var item = DomainModel.SAPHANAUserinfo.GetById(userId);
            return (item == null || item.SAPRoleInfo == null) ? new List<SAPHANARoleInfo>() : item.SAPRoleInfo;
        }

        public static IList<SAPHANARoleInfo> GetSAPRoles(long endpointId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                SAPHANARoleInfo sapRole = null;
                IList<SAPHANARoleInfo> sapRoleList = ORMapList(new SAPHANARoleInfoRepository(transaction).
                    GetByEndPointSystemId(endpointId));

                foreach (var saprole in sapRoleList)
                {
                    sapRole = new SAPHANARoleInfo();
                    sapRole.Id = CheckNull.AgainstInt(saprole.SAP_ROLEID);
                    sapRole.ROLE_CODE = CheckNull.AgainstString(saprole.ROLE_CODE);
                    sapRole.ROLE_DESC = CheckNull.AgainstString(saprole.ROLE_DESC);
                    sapRoleList.Add(sapRole);
                }
                return sapRoleList;
            }
        }
        public static Dictionary<long, string> GetSAPRoleBySearchString(long endpointId,string searchString)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new SAPHANARoleInfoRepository(transaction).
                    GetSAPRoleBySearchString(endpointId, CheckNull.AsteriskToString(searchString));
            }
        }

        public static Dictionary<long, string> GetBusinessRoles(long endpointId,long endpointUserId, string module = null, string subModule = null)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new SAPHANARoleInfoRepository(transaction).GetBusinessRoles(endpointId,endpointUserId, module, subModule);
            }
        }
        public static List<SAPHANARoleInfo> GetSAPRolesByDIUserId(long DIUserId)
        {
            List<SAPHANARoleInfo> sapgroupsList = new List<SAPHANARoleInfo>();

            //using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {


                SAPHANAUserinfo sapuser = DomainModel.SAPHANAUserinfo.GetByDIUserIdIncludeSAPRoleInfo(DIUserId);
                if (sapuser != null)
                {
                    sapgroupsList = sapuser.SAPRoleInfo.ToList();
                }
                return sapgroupsList;
            }
        }

        public static List<SAPHANARoleInfo> GetSAPRolesByDIUserId(long diUserId, AccountType accountType)
        {
            List<SAPHANARoleInfo> sapgroupsList = new List<SAPHANARoleInfo>();
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {

                SAPHANAUserinfo sapuser = null;
                sapuser = DomainModel.SAPHANAUserinfo.GetByAccountType(diUserId, accountType);
                if (sapuser != null)
                {
                    sapgroupsList = sapuser.SAPRoleInfo.ToList();
                }
                return sapgroupsList;
            }
        }

        public static List<SAPHANARoleInfo> GetByEndpointSystem(long endPointSysemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleInfo>(transaction)
                    .Get(entitlement => entitlement.EndPointSystemId == endPointSysemId).ToList());
            }

        }


        public static List<DomainModel.Presentation.SapRoleToAuthorizationReportData> GetSapRolesToAuthorizationData(string RoleCode, int EndpointSystemId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                var res = new SAPHANARoleInfoRepository(transaction).GetSapRolesToAuthorizationData(RoleCode, EndpointSystemId);
                if (res != null)
                    return res.Select(p => Mapper.Map<DomainModel.Presentation.SapRoleToAuthorizationReportData>(p)).ToList();
                return null;
            }
        }
        public static List<PresentationBusinessRoleDetail> GetBusinessRoles(long endpointId,long endpointUserId, ApplicationType applicationType)
        {
            List<PresentationBusinessRoleDetail> presentationRoles = new List<PresentationBusinessRoleDetail>();
            List<EntityModel.Presentation.PresentationBusinessRoleDetail> bRoles = new List<EntityModel.Presentation.PresentationBusinessRoleDetail>();
            using (var tran = new DataAccessTransaction(false))
            {
                bRoles = new SAPHANARoleInfoRepository(tran).GetSAPBusinessRole(endpointId, endpointUserId, applicationType).ToList();
            }
            
            foreach (var bRole in bRoles)
            {
                presentationRoles.Add(new PresentationBusinessRoleDetail
                {
                    BusinessRole = bRole.BusinessRole,
                    Company = bRole.Company,
                    Description = bRole.Description,
                    EndpointUserId = endpointUserId,
                    Module = bRole.Module,
                    Remarks = bRole.Remarks,
                    SubModule = bRole.SubModule,
                    ValidToDate = bRole.ValidToDate
                });
            }
            
            return presentationRoles;
        }
        public static PresentationBusinessRoleDetail GetBusinessRole(long endpointId, long endpointUserId, ApplicationType applicationType, string businessRole)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                PresentationBusinessRoleDetail presentationRoles = new PresentationBusinessRoleDetail();
                var bRole = new SAPHANARoleInfoRepository(tran).GetSAPBusinessRole(endpointId, endpointUserId, applicationType, businessRole);
                presentationRoles = new PresentationBusinessRoleDetail()
                {
                    BusinessRole = bRole.BusinessRole,
                    Company = bRole.Company,
                    Description = bRole.Description,
                    EndpointUserId = endpointUserId,
                    Module = bRole.Module,
                    Remarks = bRole.Remarks,
                    SubModule = bRole.SubModule,
                    ValidToDate = bRole.ValidToDate
                };
                return presentationRoles;
            }
        }
        public static List<SAPHANARoleInfo> GetListByUserName(string userName, string applicationtypes)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new SAPHANARoleInfoRepository(transaction).GetListByUserName(userName, applicationtypes));
            }
        }
    }
}
