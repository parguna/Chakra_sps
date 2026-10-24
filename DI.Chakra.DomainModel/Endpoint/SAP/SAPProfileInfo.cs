using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
     public class SAPProfileInfo:DomainModelBase<SAPProfileInfo,EntityModel.SAPProfileInfo>, IEndpointEntitlementInfo
    {
         
        

        public long EndPointSystemId { get; set; }

        public string PROFILE_CODE { get; set; }

        public string PROFILE_DESC { get; set; }

        public string TargetSysId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int? Version { get; set; }

        public IList<SAPUserinfo> SAPUserinfos { get; set; }

        public string EntitlementName
        {
            get { return PROFILE_CODE; }
        }

        public string EntitlementDesc
        {
            get { return PROFILE_DESC; }
        }
        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Profile; }
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
        public static IList<SAPProfileInfo> GetByEndpointSystemId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPProfileInfo>(dataAccessTransaction).Get(profileInfo => profileInfo.EndPointSystemId == endPointSystemId).ToList());
            }
        }
        public static Dictionary<long, string> GetSAPProfileBySearchString(long endpointId, string searchString)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return new SAPProfileInfoRepository(transaction).
                    GetSAPProfileBySearchString(endpointId, CheckNull.AsteriskToString(searchString));
            }
        }
        public static SAPProfileInfo GetByProfileCode(string profileCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPProfileInfo>(dataAccessTransaction).Get(profileInfo => profileInfo.PROFILE_CODE == profileCode).FirstOrDefault());
            }
        }

        public static SAPProfileInfo GetByProfileCode( string profileCode, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPProfileInfo>(dataAccessTransaction).Get(profile => profile.PROFILE_CODE == profileCode && profile.EndPointSystemId == endPointSystemId).FirstOrDefault());
            }
        }

        public static IList<SAPProfileInfo> GetByModifiedDate(long endPointSystemId, DateTime? modifiedDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPProfileInfo>(dataAccessTransaction).Get(profileInfo => profileInfo.EndPointSystemId == endPointSystemId 
                // if last modified date is null, check created date
                && ((profileInfo.LastModifiedDate == null && profileInfo.CreatedDate < modifiedDate) ||  (profileInfo.LastModifiedDate != null && profileInfo.LastModifiedDate < modifiedDate))).ToList());
            }
        }

        public static IList<SAPProfileInfo> GetByProfileCodeIncludingSAPUserInfo(long endPointSystemId, string profileCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPProfileInfo>(dataAccessTransaction).Get(profileInfo => profileInfo.PROFILE_CODE == profileCode && profileInfo.EndPointSystemId == endPointSystemId, profileInfo => profileInfo.SAPUserinfos).ToList());
            }
        }

        public void GetSAPUserGroupRoleProfile(int UserId, out IList<SAPGroupInfo> groupList, out IList<SAPCompositeRoleInfo> sapCompRoleList, out IList<SAPProfileInfo> profileList, out IList<SAPSingleRoleInfo> sapsingleRoleList, out IList<SAPRoleInfo> sapRoleList)
        {

            string tmpValue, tmpType;
            groupList = new List<SAPGroupInfo>();
            sapCompRoleList = new List<SAPCompositeRoleInfo>();
            profileList = new List<SAPProfileInfo>();
            sapsingleRoleList = new List<SAPSingleRoleInfo>();
            sapRoleList = new List<SAPRoleInfo>();
            IList<SAPSingleRoleInfo> tempSapsingleRoleList = new List<SAPSingleRoleInfo>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                SAPUserinfo aSAPUser = SAPUserinfo.GetById(UserId);
                if (aSAPUser != null)
                {
                    //---Group infos
                    groupList = aSAPUser.SAPGroupInfos.ToList();

                    //---Composite role info
                    //    sapCompRoleList = aSAPUser.SAPCompositeRoleInfos.ToList();



                    sapRoleList = aSAPUser.SAPRoleInfo.ToList();

                    foreach (SAPRoleInfo SapRole in sapRoleList)
                    {
                        if (SapRole.RoleType == "SINGLE")
                        {
                            SAPSingleRoleInfo SingleRole = SAPSingleRoleInfo.GetBySingleRoleCode(SapRole.ROLE_CODE);                             

                            if (SingleRole != null)
                                sapsingleRoleList.Add(SingleRole);
                        }
                        else
                        {
                            SAPCompositeRoleInfo CompositeRole = SAPCompositeRoleInfo.GetByRoleCode(SapRole.ROLE_CODE);
                           
                            if (CompositeRole != null)
                                sapCompRoleList.Add(CompositeRole);
                        }

                    }

                    // Commented out to implement composite and single role
                    ////---singlerole  info from composite info
                    //foreach (SAPCompositeRoleInfo compRoleItem in sapCompRoleList)
                    //{
                    //    tempSapsingleRoleList = compRoleItem.SAPSingleRoleInfos.ToList();
                    //    foreach (SAPSingleRoleInfo singleRoleItem in tempSapsingleRoleList)
                    //    {
                    //        if (sapsingleRoleList.Contains(singleRoleItem) == false)
                    //            sapsingleRoleList.Add(singleRoleItem);
                    //    }
                    //}

                    ////---single role info from user
                    ////tempSapsingleRoleList = aSAPUser.SAPSingleRoleInfos.ToList();
                    ////foreach (SAPSingleRoleInfo singleRoleItem in tempSapsingleRoleList)
                    ////{
                    ////    if (sapsingleRoleList.Contains(singleRoleItem) == false)
                    ////        sapsingleRoleList.Add(singleRoleItem);
                    ////}

                    //---Profile
                    //List<SapUserProfile> lstUserProfiles = oIACMDBContext.SapUserProfile.Where(user => user.UserName == aSAPUser.UserLogonName).ToList();
                    //foreach (SapUserProfile UserProfile in lstUserProfiles)
                    //{
                    //    SAPProfileInfo SapUserProfile = oIACMDBContext.SAPProfileInfos.FirstOrDefault(profile => profile.PROFILE_CODE == UserProfile.ProfileName);
                    //    if (SapUserProfile != null)
                    //        profileList.Add(SapUserProfile);
                    //}

                    profileList = aSAPUser.SAPProfileInfos.ToList();

                }
            }

        }

        public static IList<SAPProfileInfo> GetDeletedProfiles(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPProfileInfo> listDeletedProfiles = new List<SAPProfileInfo>();
                objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemId(endPointSystemId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = Process.GetByProcessTypeKeyId(objDiscoveryProcess.Id, Infrastructure.Enums.ProcessType.Discovery);
                    if (process != null)
                    {
                        listDeletedProfiles = SAPProfileInfo.GetByModifiedDate(endPointSystemId, process.ProcessStartAt);
                    }
                }

                return listDeletedProfiles;
            }
        }

        public static void SaveProfiles(IList<SAPProfileInfo> list, long endpointSystemId, string currentUser)
        {
            //using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                IList<SAPProfileInfo> listExistingSAPProfileInfos = new List<SAPProfileInfo>();
                SAPProfileInfo sapexistingSAPProfileInfo = null;
                listExistingSAPProfileInfos = SAPProfileInfo.GetByEndpointSystemId(endpointSystemId);
                foreach (SAPProfileInfo profile in list)
                {
                    sapexistingSAPProfileInfo = listExistingSAPProfileInfos.Where(t => t.PROFILE_CODE == profile.PROFILE_CODE).FirstOrDefault();
                    if (sapexistingSAPProfileInfo == null)
                    {
                        profile.Save(currentUser);
                    }
                    else
                    {
                        profile.Id = sapexistingSAPProfileInfo.Id;
                        //profile.PROFILE_CODE = sapexistingSAPProfileInfo.PROFILE_CODE;
                        profile.Save(currentUser);
                    }
                }

                //dataAccessTransaction.Commit();
            }
        }
        public static SAPProfileInfo GetByProfileCodeWithNavigation(string profileCode, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPProfileInfo>(dataAccessTransaction).Get(profileInfo => profileInfo.PROFILE_CODE == profileCode && profileInfo.EndPointSystemId == endPointSystemId, profileInfo => profileInfo.SAPUserinfos).FirstOrDefault());
            }
        }


    }
}
