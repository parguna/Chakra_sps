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
     public class SAPHANAProfileInfo : DomainModelBase<SAPHANAProfileInfo, EntityModel.SAPHANAProfileInfo>, IEndpointEntitlementInfo
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

        public IList<SAPHANAUserinfo> SAPUserinfos { get; set; }

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
        public static IList<SAPHANAProfileInfo> GetByEndpointSystemId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAProfileInfo>(dataAccessTransaction).Get(profileInfo => profileInfo.EndPointSystemId == endPointSystemId).ToList());
            }
        }
        public static Dictionary<long, string> GetSAPProfileBySearchString(long endpointId, string searchString)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return new SAPHANAProfileInfoRepository(transaction).
                    GetSAPProfileBySearchString(endpointId, CheckNull.AsteriskToString(searchString));
            }
        }
        public static SAPHANAProfileInfo GetByProfileCode(string profileCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANAProfileInfo>(dataAccessTransaction).Get(profileInfo => profileInfo.PROFILE_CODE == profileCode).FirstOrDefault());
            }
        }

        public static SAPHANAProfileInfo GetByProfileCode( string profileCode, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANAProfileInfo>(dataAccessTransaction).Get(profile => profile.PROFILE_CODE == profileCode && profile.EndPointSystemId == endPointSystemId).FirstOrDefault());
            }
        }

        public static IList<SAPHANAProfileInfo> GetByModifiedDate(long endPointSystemId, DateTime? modifiedDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAProfileInfo>(dataAccessTransaction).Get(profileInfo => profileInfo.EndPointSystemId == endPointSystemId 
                // if last modified date is null, check created date
                && ((profileInfo.LastModifiedDate == null && profileInfo.CreatedDate < modifiedDate) ||  (profileInfo.LastModifiedDate != null && profileInfo.LastModifiedDate < modifiedDate))).ToList());
            }
        }

        public static IList<SAPHANAProfileInfo> GetByProfileCodeIncludingSAPUserInfo(long endPointSystemId, string profileCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAProfileInfo>(dataAccessTransaction).Get(profileInfo => profileInfo.PROFILE_CODE == profileCode && profileInfo.EndPointSystemId == endPointSystemId, profileInfo => profileInfo.SAPUserinfos).ToList());
            }
        }

        public void GetSAPUserGroupRoleProfile(int UserId, out IList<SAPHANAGroupInfo> groupList, out IList<SAPHANACompositeRoleInfo> sapCompRoleList, out IList<SAPHANAProfileInfo> profileList, out IList<SAPHANASingleRoleInfo> sapsingleRoleList, out IList<SAPHANARoleInfo> sapRoleList)
        {

            string tmpValue, tmpType;
            groupList = new List<SAPHANAGroupInfo>();
            sapCompRoleList = new List<SAPHANACompositeRoleInfo>();
            profileList = new List<SAPHANAProfileInfo>();
            sapsingleRoleList = new List<SAPHANASingleRoleInfo>();
            sapRoleList = new List<SAPHANARoleInfo>();
            IList<SAPHANASingleRoleInfo> tempSapsingleRoleList = new List<SAPHANASingleRoleInfo>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                SAPHANAUserinfo aSAPUser = SAPHANAUserinfo.GetById(UserId);
                if (aSAPUser != null)
                {
                    //---Group infos
                    groupList = aSAPUser.SAPGroupInfos.ToList();

                    //---Composite role info
                    //    sapCompRoleList = aSAPUser.SAPCompositeRoleInfos.ToList();



                    sapRoleList = aSAPUser.SAPRoleInfo.ToList();

                    foreach (SAPHANARoleInfo SapRole in sapRoleList)
                    {
                        if (SapRole.RoleType == "SINGLE")
                        {
                            SAPHANASingleRoleInfo SingleRole = SAPHANASingleRoleInfo.GetBySingleRoleCode(SapRole.ROLE_CODE);                             

                            if (SingleRole != null)
                                sapsingleRoleList.Add(SingleRole);
                        }
                        else
                        {
                            SAPHANACompositeRoleInfo CompositeRole = SAPHANACompositeRoleInfo.GetByRoleCode(SapRole.ROLE_CODE);
                           
                            if (CompositeRole != null)
                                sapCompRoleList.Add(CompositeRole);
                        }

                    }

                    

                    profileList = aSAPUser.SAPProfileInfos.ToList();

                }
            }

        }

        public static IList<SAPHANAProfileInfo> GetDeletedProfiles(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPHANAProfileInfo> listDeletedProfiles = new List<SAPHANAProfileInfo>();
                objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemId(endPointSystemId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = Process.GetByProcessTypeKeyId(objDiscoveryProcess.Id, Infrastructure.Enums.ProcessType.Discovery);
                    if (process != null)
                    {
                        listDeletedProfiles = SAPHANAProfileInfo.GetByModifiedDate(endPointSystemId, process.ProcessStartAt);
                    }
                }

                return listDeletedProfiles;
            }
        }

        public static void SaveProfiles(IList<SAPHANAProfileInfo> list, long endpointSystemId, string currentUser)
        {
            //using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                IList<SAPHANAProfileInfo> listExistingSAPProfileInfos = new List<SAPHANAProfileInfo>();
                SAPHANAProfileInfo sapexistingSAPProfileInfo = null;
                listExistingSAPProfileInfos = SAPHANAProfileInfo.GetByEndpointSystemId(endpointSystemId);
                foreach (SAPHANAProfileInfo profile in list)
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
        public static SAPHANAProfileInfo GetByProfileCodeWithNavigation(string profileCode, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANAProfileInfo>(dataAccessTransaction).Get(profileInfo => profileInfo.PROFILE_CODE == profileCode && profileInfo.EndPointSystemId == endPointSystemId, profileInfo => profileInfo.SAPUserinfos).FirstOrDefault());
            }
        }


    }
}
