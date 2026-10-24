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
    public class PresentationInhouseUserEntitlement : Presentation.PresentationUserEntitlement
    {
        public override List<PresentationUserEntitlement> GetAllEntitlementDetailByType(long endpointSystemId, EntitlementType entitlementType)
        {
            switch (entitlementType)
            {
                case EntitlementType.Permission:
                    List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
                    var adsGroups = InhousePermission.GetByEndpointSystem(endpointSystemId).Where(a => a.EntitlementType == entitlementType).ToList();
                    //adsGroups.ForEach(lq =>
                    //{
                    //    PresentationADSUserEntitlement UserEntitlement = new PresentationADSUserEntitlement();
                    //    UserEntitlement.EntitlementId = lq.Id;
                    //    UserEntitlement.Category = Convert.ToString(EntitlementType.Permission);
                    //    UserEntitlement.Name = lq.Name;
                    //    UserEntitlement.Description = lq.Description;
                    //    UserEntitlement.CanRequest = lq.CanRequest;
                    //    UserEntitlement.UserDefinedDesc = lq.UserDefinedDesc;
                    //    UserEntitlement.EndpointSystemId = endpointSystemId;
                    //    UserEntitlement.EntitlementType = EntitlementType.Permission;
                    //    UserEntitlement.UserDefinedDisplayName = lq.UserDefinedDisplayName;
                    //    UserEntitlement.Level1Approver = lq.Level1Approver;
                    //    UserEntitlement.Level2Approver = lq.Level2Approver;
                    //    UserEntitlement.IsLevel1Group = lq.IsLevel1Group;
                    //    UserEntitlement.IsLevel2Group = lq.IsLevel2Group;

                    //    presentationUserEntitlementList.Add(UserEntitlement);
                    //});
                    return presentationUserEntitlementList;
                default:
                    return new List<PresentationUserEntitlement>();
            }
        }

        public override List<PresentationUserEntitlement> GetEntitlementDetailByApplicationType(long endpointsystemId)
        {            
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                //var Entitlement = InhousePermission.GetByEndpointSystem(endpointsystemId).Where(a => a.CanRequest == true).ToList(); 
                //Entitlement.ForEach(lq =>
                //{
                //    PresentationInhouseUserEntitlement UserEntitlement = new PresentationInhouseUserEntitlement();
                //    UserEntitlement.EntitlementId = lq.Id;
                //    UserEntitlement.Category = Convert.ToString(EntitlementType.Permission);
                //    UserEntitlement.Name = lq.Name;
                //    UserEntitlement.Description = lq.Description;
                //    UserEntitlement.CanRequest = lq.CanRequest;
                //    UserEntitlement.UserDefinedDesc = lq.UserDefinedDesc;
                //    UserEntitlement.EndpointSystemId = endpointsystemId;
                //    UserEntitlement.EntitlementType = EntitlementType.Permission;
                //    UserEntitlement.UserDefinedDisplayName = lq.UserDefinedDisplayName;
                //    UserEntitlement.Level1Approver = lq.Level1Approver;
                //    UserEntitlement.Level2Approver = lq.Level2Approver;
                //    UserEntitlement.IsLevel1Group = lq.IsLevel1Group;
                //    UserEntitlement.IsLevel2Group = lq.IsLevel2Group;

                //    presentationUserEntitlementList.Add(UserEntitlement);
                //});
            }
            return presentationUserEntitlementList;
        }

        public override void UpdateEntitlement(PresentationUserEntitlement presentationEntitlement, IEndpointEntitlementInfo entitlement, string currentUser)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                InhousePermission inhousePermission = (InhousePermission)entitlement;
                //inhousePermission.UserDefinedDesc = presentationEntitlement.UserDefinedDesc;
                //inhousePermission.CanRequest = presentationEntitlement.CanRequest;

                //inhousePermission.UserDefinedDisplayName = presentationEntitlement.UserDefinedDisplayName;
                //inhousePermission.Level1Approver = presentationEntitlement.Level1Approver;
                //inhousePermission.Level2Approver = presentationEntitlement.Level2Approver;
                //inhousePermission.IsLevel1Group = presentationEntitlement.IsLevel1Group;
                //inhousePermission.IsLevel2Group = presentationEntitlement.IsLevel2Group;

                inhousePermission.Save(dataAccessTransaction);
                dataAccessTransaction.Commit();
            }
        }
        public override List<PresentationUserEntitlement> GetEntitlementDetail(long endpointUserId)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            InhouseUserInfo INHUserInfo = new InhouseUserInfo();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                INHUserInfo = InhouseUserInfo.GetByIdWithNavigation(endpointUserId);
                if (INHUserInfo == null)
                    return presentationUserEntitlementList;

                foreach (InhousePermission per in INHUserInfo.Permissions)
                {
                    PresentationInhouseUserEntitlement UserEntitlement = new PresentationInhouseUserEntitlement();
                    UserEntitlement.EntitlementId = INHUserInfo.Id;
                    UserEntitlement.UserName = INHUserInfo.AccountName;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Group);
                    UserEntitlement.EndPointSystemType = INHUserInfo.EndPoint.EndPointSystemType.EndpointSystemTypeName;
                    UserEntitlement.EndpointSystemName = INHUserInfo.EndPoint.DisplayName;
                    if (per != null)
                    {
                        UserEntitlement.Name = per.EntitlementName;
                        UserEntitlement.Description = per.Description;

                        //UserEntitlement.UserDefinedDisplayName = per.UserDefinedDisplayName;
                        //UserEntitlement.Level1Approver = per.Level1Approver;
                        //UserEntitlement.Level2Approver = per.Level2Approver;
                        //UserEntitlement.IsLevel1Group = per.IsLevel1Group;
                        //UserEntitlement.IsLevel2Group = per.IsLevel2Group;

                        presentationUserEntitlementList.Add(UserEntitlement);
                    }
                }
            }
            return presentationUserEntitlementList;
        }
        public override PresentationUserEntitlement GetEntitlement(IEndpointEntitlementInfo entitlement, string entitlementType)
        {
            InhousePermission inhousePermission = (InhousePermission)entitlement;
            PresentationInhouseUserEntitlement UserEntitlement = new PresentationInhouseUserEntitlement();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                UserEntitlement.EntitlementId = (inhousePermission != null) ? inhousePermission.Id : 0;
                UserEntitlement.Category = Convert.ToString(EntitlementType.Permission);
                UserEntitlement.Name = (inhousePermission != null) ? inhousePermission.Name : string.Empty;
                UserEntitlement.Description = (inhousePermission != null) ? inhousePermission.Description : string.Empty;
                //UserEntitlement.UserDefinedDesc = (inhousePermission != null) ? inhousePermission.UserDefinedDesc : string.Empty;
                //UserEntitlement.EndPointSystemType = EndpointType.InhouseSystems;
                //UserEntitlement.CanRequest = (inhousePermission != null) ? inhousePermission.CanRequest : false;

                //UserEntitlement.UserDefinedDisplayName = inhousePermission.UserDefinedDisplayName;
                //UserEntitlement.Level1Approver = inhousePermission.Level1Approver;
                //UserEntitlement.Level2Approver = inhousePermission.Level2Approver;
                //UserEntitlement.IsLevel1Group = inhousePermission.IsLevel1Group;
                //UserEntitlement.IsLevel2Group = inhousePermission.IsLevel2Group;
            }
            return UserEntitlement;
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId)
        {
            return new List<PresentationUserEntitlement>();
        }
        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetail(string entitlementCode, long endpointId)
        {
            return new List<PresentationUserEntitlement>();
        }
        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId, string entitlementType, string userAccountName)
        {
            return new List<PresentationUserEntitlement>();
        }
        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(long endpointId, string entityCode, EntitlementType fromValue, EntitlementType toValue)
        {
            if (fromValue == EntitlementType.Permission)
            {
                var permission = InhousePermission.GetByCode(entityCode);
                if (permission != null)
                {
                    return GetEntitlementHierachyDetailByLevel(endpointId, permission.Id, fromValue, toValue);
                }
            }

            return new List<PresentationUserEntitlement>();
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(long endpointId, long entityId, EntitlementType fromValue, EntitlementType toValue)
        {
            return new List<PresentationUserEntitlement>();
        }

        public override List<PresentationUserEntitlement> GetEntitlementDetailByApplicationTypeWithAllCanRequest(long endpointSystemId)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var Entitlement = InhousePermission.GetByEndpointSystem(endpointSystemId).ToList();
                Entitlement.ForEach(lq =>
                {
                    PresentationInhouseUserEntitlement UserEntitlement = new PresentationInhouseUserEntitlement();
                    UserEntitlement.EntitlementId = lq.Id;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Permission);
                    UserEntitlement.Name = lq.Name;
                    UserEntitlement.Description = lq.Description;
                    //UserEntitlement.CanRequest = lq.CanRequest;
                    //UserEntitlement.UserDefinedDesc = lq.UserDefinedDesc;
                    //UserEntitlement.EndpointSystemId = endpointSystemId;
                    //UserEntitlement.EntitlementType = EntitlementType.Permission;
                    //UserEntitlement.UserDefinedDisplayName = lq.UserDefinedDisplayName;
                    //UserEntitlement.Level1Approver = lq.Level1Approver;
                    //UserEntitlement.Level2Approver = lq.Level2Approver;
                    //UserEntitlement.IsLevel1Group = lq.IsLevel1Group;
                    //UserEntitlement.IsLevel2Group = lq.IsLevel2Group;

                    presentationUserEntitlementList.Add(UserEntitlement);
                });
            }
            return presentationUserEntitlementList;
        }
    }
}
