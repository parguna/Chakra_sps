using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Transaction;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainModel
{
    public class PresentationADSUserEntitlement : Presentation.PresentationUserEntitlement
    {
        public override PresentationUserEntitlement GetEntitlement(IEndpointEntitlementInfo entitlement, string entitlementType)
        {
            ADSGroupInfo adsGroupInfo = (ADSGroupInfo)entitlement;
            PresentationADSUserEntitlement UserEntitlement = new PresentationADSUserEntitlement();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                UserEntitlement.EntitlementId = adsGroupInfo.Id;
                UserEntitlement.Category = Convert.ToString(EntitlementType.Group);
                UserEntitlement.Name = adsGroupInfo.name;
                UserEntitlement.Description = adsGroupInfo.description;
                UserEntitlement.UserDefinedDesc = adsGroupInfo.UserDefinedDesc;
                UserEntitlement.EndPointSystemType = EndpointType.ActiveDirectory;
                UserEntitlement.CanRequest = adsGroupInfo.CanRequest;
                UserEntitlement.EntitlementType = EntitlementType.Group;
                UserEntitlement.UserDefinedDisplayName = adsGroupInfo.UserDefinedDisplayName;
                UserEntitlement.Level1Approver = adsGroupInfo.Level1Approver;
                UserEntitlement.Level2Approver = adsGroupInfo.Level2Approver;
                UserEntitlement.IsLevel1Group = adsGroupInfo.IsLevel1Group;
                UserEntitlement.IsLevel2Group = adsGroupInfo.IsLevel2Group;

            }
            return UserEntitlement;
        }
        public override void UpdateEntitlement(PresentationUserEntitlement presentationEntitlement, IEndpointEntitlementInfo entitlement, string currentUser)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                if (presentationEntitlement.EntitlementType == EntitlementType.Group)
                {
                    ADSGroupInfo adsGroupInfo = (ADSGroupInfo)entitlement;
                    adsGroupInfo.UserDefinedDesc = presentationEntitlement.UserDefinedDesc;
                    adsGroupInfo.CanRequest = presentationEntitlement.CanRequest;

                    adsGroupInfo.UserDefinedDisplayName = presentationEntitlement.UserDefinedDisplayName;
                    adsGroupInfo.Level1Approver = presentationEntitlement.Level1Approver;
                    adsGroupInfo.Level2Approver = presentationEntitlement.Level2Approver;
                    adsGroupInfo.IsLevel1Group = presentationEntitlement.IsLevel1Group;
                    adsGroupInfo.IsLevel2Group = presentationEntitlement.IsLevel2Group;

                    adsGroupInfo.Save(dataAccessTransaction);
                    dataAccessTransaction.Commit();
                }
            }
        }
        public override List<PresentationUserEntitlement> GetEntitlementDetail(long endpointUserId)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            ADSUserInfo ADSUserInfo = new ADSUserInfo();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                ADSUserInfo = ADSUserInfo.GetByIdWithNavigation(endpointUserId);
                if (ADSUserInfo == null)
                    return presentationUserEntitlementList;
                PresentationADSUserEntitlement UserEntitlement = null;

                foreach (ADSGroupInfo grp in ADSUserInfo.ADSGroups)
                {
                    UserEntitlement = new PresentationADSUserEntitlement();
                    UserEntitlement.EntitlementId = grp.Id;
                    UserEntitlement.UserName = ADSUserInfo.AccountName;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Group);
                    UserEntitlement.EndPointSystemType = ADSUserInfo.EndPointSystem.EndPointSystemType.EndpointSystemTypeName;
                    UserEntitlement.EndpointSystemName = ADSUserInfo.EndPointSystem.DisplayName;
                    UserEntitlement.EntitlementType = EntitlementType.Group;
                    UserEntitlement.EndpointSystemId = grp.EndPointSystemId;
                    UserEntitlement.ParentEntitlementId = endpointUserId;
                    if (grp != null)
                    {
                        UserEntitlement.Name = grp.name;
                        UserEntitlement.Description = grp.description;
                        presentationUserEntitlementList.Add(UserEntitlement);
                    }
                }
            }
            return presentationUserEntitlementList;
        }
        public override List<PresentationUserEntitlement> GetEntitlementDetailByApplicationType(long endpointsystemId)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var Entitlement = ADSGroupInfo.GetByEndpointSystem(endpointsystemId).Where(a => a.CanRequest == true).ToList();
                PresentationADSUserEntitlement UserEntitlement = null;
                Entitlement.ForEach(lq =>
                {
                    UserEntitlement = new PresentationADSUserEntitlement();
                    UserEntitlement.EntitlementId = lq.Id;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Group);
                    UserEntitlement.Name = lq.name;
                    UserEntitlement.Description = lq.description;
                    UserEntitlement.CanRequest = lq.CanRequest;
                    UserEntitlement.UserDefinedDesc = lq.UserDefinedDesc;
                    UserEntitlement.EndpointSystemId = endpointsystemId;
                    UserEntitlement.EntitlementType = EntitlementType.Group;
                    UserEntitlement.UserDefinedDisplayName = lq.UserDefinedDisplayName;
                    UserEntitlement.Level1Approver = lq.Level1Approver;
                    UserEntitlement.Level2Approver = lq.Level2Approver;
                    UserEntitlement.IsLevel1Group = lq.IsLevel1Group;
                    UserEntitlement.IsLevel2Group = lq.IsLevel2Group;

                    presentationUserEntitlementList.Add(UserEntitlement);
                });
            }
            return presentationUserEntitlementList;
        }

        public override List<PresentationUserEntitlement> GetEntitlementDetailByApplicationTypeWithAllCanRequest(long endpointsystemId)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var Entitlement = ADSGroupInfo.GetByEndpointSystem(endpointsystemId).ToList();
                PresentationADSUserEntitlement UserEntitlement = null;
                Entitlement.ForEach(lq =>
                {
                    UserEntitlement = new PresentationADSUserEntitlement();
                    UserEntitlement.EntitlementId = lq.Id;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Group);
                    UserEntitlement.Name = lq.name;
                    UserEntitlement.Description = lq.description;
                    UserEntitlement.CanRequest = lq.CanRequest;
                    UserEntitlement.UserDefinedDesc = lq.UserDefinedDesc;
                    UserEntitlement.EndpointSystemId = endpointsystemId;
                    UserEntitlement.EntitlementType = EntitlementType.Group;
                    UserEntitlement.UserDefinedDisplayName = lq.UserDefinedDisplayName;
                    UserEntitlement.Level1Approver = lq.Level1Approver;
                    UserEntitlement.Level2Approver = lq.Level2Approver;
                    UserEntitlement.IsLevel1Group = lq.IsLevel1Group;
                    UserEntitlement.IsLevel2Group = lq.IsLevel2Group;

                    presentationUserEntitlementList.Add(UserEntitlement);
                });
            }
            return presentationUserEntitlementList;
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            presentationUserEntitlementList= GetChildGroupDetail(entitlementId);
            return presentationUserEntitlementList;
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetail(string entitlementCode, long endpointId)
        {
            return new List<PresentationUserEntitlement>();
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(long endpointId, string entityCode, EntitlementType fromValue, EntitlementType toValue)
        {
            return new List<PresentationUserEntitlement>();
        }

        public static void ChildGroupRecursive(long endpointSystemId, long parentGroupID, List<PresentationUserEntitlement> PresentationSubGroupInfoList)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var parent = ADSGroupInfo.GetById(parentGroupID);
                var NestedGroup = ADSNestedGroup.GetAll().Where(item => item.Supergroup == parentGroupID.ToString() && item.EndPointSystemId == endpointSystemId).ToList();
                if (NestedGroup.Count != 0)
                {
                    PresentationADSUserEntitlement UserEntitlement = null;
                    ADSGroupInfo adsGroupInfo = null;
                    for (int i = 0; i < NestedGroup.Count; i++)
                    {
                         UserEntitlement = new PresentationADSUserEntitlement();
                        adsGroupInfo= ADSGroupInfo.GetById(Convert.ToInt64(NestedGroup[i].Subgroup));
                        UserEntitlement.EntitlementId = parentGroupID;
                        UserEntitlement.ParentEntitlementId = NestedGroup[i].Id;
                        UserEntitlement.Name = adsGroupInfo.sAMAccountName;
                        UserEntitlement.EndPointSystemType = Infrastructure.Enums.EndpointType.ActiveDirectory;
                        PresentationSubGroupInfoList.Add(UserEntitlement);
                        ChildGroupRecursive(endpointSystemId, Convert.ToInt64(NestedGroup[i].Subgroup), PresentationSubGroupInfoList);
                    }
                }
            }
            return;
        }
        public static List<PresentationUserEntitlement> GetChildGroupDetail(long endpointUserId)
        {
            List<PresentationUserEntitlement> PresentationSubGroupInfoList = new List<PresentationUserEntitlement>();
            string parents = string.Empty;
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var parent = ADSGroupInfo.GetById(endpointUserId);
                if (parent == null)
                    return PresentationSubGroupInfoList;
                var NestedGroup = ADSNestedGroup.GetAll().Where(item => item.Supergroup == parent.Id.ToString() && item.EndPointSystemId == parent.EndPointSystemId).ToList();
                var subGroup = ADSNestedGroup.GetAll().Where(item => item.Subgroup == parent.Id.ToString() && item.EndPointSystemId == parent.EndPointSystemId).ToList();
                if (NestedGroup.Count != 0)
                {
                    PresentationADSUserEntitlement UserEntitlement = null;
                    ADSGroupInfo adsGroupInfo = null;
                    for (int i = 0; i < NestedGroup.Count; i++)
                    {
                        UserEntitlement = new PresentationADSUserEntitlement();
                        adsGroupInfo = ADSGroupInfo.GetById(Convert.ToInt64(NestedGroup[i].Subgroup));
                        UserEntitlement.EntitlementId =Convert.ToInt64(NestedGroup[i].Subgroup);
                        UserEntitlement.ParentEntitlementId = endpointUserId;
                        UserEntitlement.Name = adsGroupInfo.EntitlementName;
                        UserEntitlement.Description = adsGroupInfo.description;
                        UserEntitlement.Category = Infrastructure.Enums.EntitlementType.Member.ToString();
                        UserEntitlement.EntitlementType = EntitlementType.Member;
                        UserEntitlement.UserName = adsGroupInfo.sAMAccountName;
                        UserEntitlement.EndPointSystemType = Infrastructure.Enums.EndpointType.ActiveDirectory;
                        PresentationSubGroupInfoList.Add(UserEntitlement);                        
                    }
                }
                if (subGroup.Count != 0)
                {
                    PresentationADSUserEntitlement UserEntitlement = null;
                    ADSGroupInfo adsGroupInfo = null;
                    for (int i = 0; i < subGroup.Count; i++)
                    {
                        UserEntitlement = new PresentationADSUserEntitlement();
                        adsGroupInfo = ADSGroupInfo.GetById(Convert.ToInt64(subGroup[i].Supergroup));
                        UserEntitlement.EntitlementId = Convert.ToInt64(subGroup[i].Supergroup);
                        UserEntitlement.ParentEntitlementId = endpointUserId;
                        UserEntitlement.Name = adsGroupInfo.EntitlementName;
                        UserEntitlement.Description = adsGroupInfo.description;
                        UserEntitlement.Category = Infrastructure.Enums.EntitlementType.MemberOf.ToString();
                        UserEntitlement.EntitlementType = EntitlementType.MemberOf;
                        UserEntitlement.UserName = adsGroupInfo.sAMAccountName;
                        UserEntitlement.EndPointSystemType = Infrastructure.Enums.EndpointType.ActiveDirectory;
                        PresentationSubGroupInfoList.Add(UserEntitlement);
                    }
                }
            }           
            return PresentationSubGroupInfoList;
        }
        //public override Dictionary<long, string> GetEntitlementName(long endpointSystemId, string entitlementType)
        //{
        //    var getByEndpointSystem = this.GetEntitlementDetailByApplicationType(endpointSystemId);
        //    return getByEndpointSystem.Where(x=>x.Category == entitlementType)
        //                       .Select(item => new { Key = item.EntitlementId, Value = item.Name })
        //                       .Distinct()
        //                       .ToDictionary(item => item.Key, item => item.Value);
        //}
        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId, string entitlementType, string userAccountName)
        {
            return new List<PresentationUserEntitlement>();
        }

        public override List<PresentationUserEntitlement> GetAllEntitlementDetailByType(long endpointSystemId, EntitlementType entitlementType)
        {
            PresentationADSUserEntitlement UserEntitlement = null;
            switch (entitlementType)
            {
                case EntitlementType.Group:
                    List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
                    var adsGroups = ADSGroupInfo.GetAllByEndpointSystemId(endpointSystemId).Where(a => a.EntitlementType == entitlementType).ToList();
                    adsGroups.ForEach(lq =>
                    {
                        UserEntitlement = new PresentationADSUserEntitlement();
                        UserEntitlement.EntitlementId = lq.Id;
                        UserEntitlement.Category = Convert.ToString(EntitlementType.Group);
                        UserEntitlement.Name = lq.name;
                        UserEntitlement.Description = lq.description;
                        UserEntitlement.CanRequest = lq.CanRequest;
                        UserEntitlement.UserDefinedDesc = lq.UserDefinedDesc;
                        UserEntitlement.EndpointSystemId = endpointSystemId;
                        UserEntitlement.EntitlementType = EntitlementType.Group;
                        UserEntitlement.UserDefinedDisplayName = lq.UserDefinedDisplayName;
                        UserEntitlement.Level1Approver = lq.Level1Approver;
                        UserEntitlement.Level2Approver = lq.Level2Approver;
                        UserEntitlement.IsLevel1Group = lq.IsLevel1Group;
                        UserEntitlement.IsLevel2Group = lq.IsLevel2Group;

                        presentationUserEntitlementList.Add(UserEntitlement);
                    });
                    return presentationUserEntitlementList;
                default:
                    return new List<PresentationUserEntitlement>();
            }
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(long endpointId, long entityId, EntitlementType fromValue, EntitlementType toValue)
        {
            return new List<PresentationUserEntitlement>();
        }
    }
}
