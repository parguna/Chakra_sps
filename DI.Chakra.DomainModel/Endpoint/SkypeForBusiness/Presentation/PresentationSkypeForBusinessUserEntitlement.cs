using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;

namespace DI.Chakra.DomainModel
{
    public class PresentationSkypeForBusinessUserEntitlement : Presentation.PresentationUserEntitlement
    {
        public override List<PresentationUserEntitlement> GetAllEntitlementDetailByType(long endpointSystemId, EntitlementType entitlementType)
        {
            List<PresentationUserEntitlement> presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            var entilements = new List<EntitlementReconcilation>();
            switch (entitlementType)
            {
                case EntitlementType.ResponseGroup:
                    entilements = EntitlementReconcilation.GetAllEntitlementByType(endpointSystemId, (int)entitlementType).ToList();
                    entilements.ForEach(lq =>
                    {
                        PresentationSkypeForBusinessUserEntitlement UserEntitlement = new PresentationSkypeForBusinessUserEntitlement();
                        UserEntitlement.EntitlementId = lq.Id;
                        UserEntitlement.Category = Convert.ToString(EntitlementType.ResponseGroup);
                        UserEntitlement.Name = lq.Entitlement;
                        UserEntitlement.Description = lq.EntitlementDescription;
                        UserEntitlement.CanRequest = true;
                        UserEntitlement.UserDefinedDesc = lq.EntitlementDescription;
                        UserEntitlement.EndpointSystemId = endpointSystemId;
                        UserEntitlement.EntitlementType = EntitlementType.ResponseGroup;
                        presentationUserEntitlementList.Add(UserEntitlement);
                    });
                    break;
                case EntitlementType.DCC:
                case EntitlementType.DCCS:
                case EntitlementType.IDD:
                case EntitlementType.Default:
                    entilements = EntitlementReconcilation.GetAllEntitlementByType(endpointSystemId, (int)entitlementType).ToList();
                    entilements.ForEach(lq =>
                    {
                        PresentationSkypeForBusinessUserEntitlement UserEntitlement = new PresentationSkypeForBusinessUserEntitlement();
                        UserEntitlement.EntitlementId = lq.Id;
                        UserEntitlement.Category = Convert.ToString(entitlementType);
                        UserEntitlement.Name = lq.Entitlement;
                        UserEntitlement.Description = lq.EntitlementDescription;
                        UserEntitlement.CanRequest = true;
                        UserEntitlement.UserDefinedDesc = lq.EntitlementDescription;
                        UserEntitlement.EndpointSystemId = endpointSystemId;
                        UserEntitlement.EntitlementType = entitlementType;
                        presentationUserEntitlementList.Add(UserEntitlement);
                    });
                    break;
                default:
                    return new List<PresentationUserEntitlement>();
            }
            return presentationUserEntitlementList;
        }
        public override PresentationUserEntitlement GetEntitlement(IEndpointEntitlementInfo entitlement, string entitlementType)
        {
            var UserEntitlement = new PresentationSkypeForBusinessUserEntitlement();

            if (entitlementType == Infrastructure.Enums.EntitlementType.ResponseGroup.ToString())
            {
                var group = (SkypeForBusinessResponseGroupInfo)entitlement;
                UserEntitlement.EntitlementId = group.Id;
                UserEntitlement.Category = Convert.ToString(EntitlementType.ResponseGroup);
                UserEntitlement.Name = group.Name;
                UserEntitlement.Description = group.Name;
                UserEntitlement.UserDefinedDesc = group.Name;
                UserEntitlement.EndPointSystemType = EndpointType.MicrosoftTeams;
                UserEntitlement.EntitlementType = EntitlementType.ResponseGroup;
                UserEntitlement.CanRequest = true;
            }
            else if (entitlementType == Infrastructure.Enums.EntitlementType.DCC.ToString() ||
                entitlementType == Infrastructure.Enums.EntitlementType.DCCS.ToString() ||
                entitlementType == Infrastructure.Enums.EntitlementType.IDD.ToString() ||
                entitlementType == Infrastructure.Enums.EntitlementType.Default.ToString()
                )
            {
                var  sfbType = (EntitlementType) Enum.Parse(typeof(EntitlementType), entitlementType,true);
                var usrEntitlement = (SkypeForBusinessUserInfo)entitlement;
                UserEntitlement.EntitlementId = usrEntitlement.Id;
                UserEntitlement.Category = Convert.ToString(sfbType);
                UserEntitlement.Name = usrEntitlement.UserPrincipalName;
                UserEntitlement.Description = usrEntitlement.UserPrincipalName;
                UserEntitlement.UserDefinedDesc = usrEntitlement.UserPrincipalName;
                UserEntitlement.EndPointSystemType = EndpointType.MicrosoftTeams;
                UserEntitlement.EntitlementType = sfbType;
                UserEntitlement.CanRequest = true;
            }
            
            return UserEntitlement;
        }

        public override List<PresentationUserEntitlement> GetEntitlementDetail(long endpointUserId)
        {
            var presentationUserEntitlementList = new List<PresentationUserEntitlement>();
            var userInfo = SkypeForBusinessUserInfo.GetByEndpointUserId(endpointUserId);
            if(userInfo != null)
            {
                var entitlementRecons = EntitlementReconcilation.GetByEndpointUser(endpointUserId, userInfo.EndPointSystemId);
                foreach (var entitlementRecon in entitlementRecons)
                {
                    var entType = (EntitlementType)entitlementRecon.EntitlementType;
                    var UserEntitlement = new PresentationSkypeForBusinessUserEntitlement();
                    UserEntitlement.EntitlementId = entitlementRecon.EntitlementId;
                    UserEntitlement.UserName = userInfo.AccountName;
                    UserEntitlement.Category = Convert.ToString(entType);
                    UserEntitlement.EndPointSystemType = userInfo.EndPointSystem.EndPointSystemType.EndpointSystemTypeName;
                    UserEntitlement.EndpointSystemName = userInfo.EndPointSystem.DisplayName;
                    UserEntitlement.EndpointSystemId = userInfo.EndPointSystemId;
                    UserEntitlement.EntitlementType = entType;
                    UserEntitlement.Name = entitlementRecon.Entitlement;
                    UserEntitlement.Description = entitlementRecon.EntitlementDescription;
                    presentationUserEntitlementList.Add(UserEntitlement);
                }
            }
            
            return presentationUserEntitlementList;
        }

        public override List<PresentationUserEntitlement> GetEntitlementDetailByApplicationType(long endpointSystemId)
        {
            throw new NotImplementedException();
        }

        public override List<PresentationUserEntitlement> GetEntitlementDetailByApplicationTypeWithAllCanRequest(long endpointSystemId)
        {
            throw new NotImplementedException();
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId)
        {
            throw new NotImplementedException();
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetail(long entitlementId, string entitlementType, string userAccountName)
        {
            throw new NotImplementedException();
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetail(string entitlementCode, long endpointId)
        {
            throw new NotImplementedException();
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(long endpointId, string entityCode, EntitlementType fromValue, EntitlementType toValue)
        {
            throw new NotImplementedException();
        }

        public override List<PresentationUserEntitlement> GetEntitlementHierachyDetailByLevel(long endpointId, long entityId, EntitlementType fromValue, EntitlementType toValue)
        {
            throw new NotImplementedException();
        }

        public override void UpdateEntitlement(PresentationUserEntitlement presentationUserEntitlement, IEndpointEntitlementInfo entitlement, string currentUser)
        {
            throw new NotImplementedException();
        }
    }
}
