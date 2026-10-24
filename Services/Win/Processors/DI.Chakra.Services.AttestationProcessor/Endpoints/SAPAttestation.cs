using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
//using DI.Chakra.Model;

using System;
using System.Collections.Generic;
using DI.Chakra.DomainModel;

namespace DI.Chakra.Services.AttestationProcessor
{
    public class SAPAttestation :EndpointAttestation
    {
        public override IList<AttestationReviewEndpointSystemL2Detail> GetRoleReviews( long userId , long reviewEndpointId )
        {
            liAttestationReviewEndpointSystemL2Detail = new List<AttestationReviewEndpointSystemL2Detail>();

            try
            {
                AttestationReviewEndpointSystemL2Detail reviewRoles = null;

                IList<SAPRoleInfo> listRoles = SAPRoleInfo.GetListByUserId(userId);

                if (listRoles == null)
                    return new List<AttestationReviewEndpointSystemL2Detail>();

                if (listRoles.Count <= 0)
                    return new List<AttestationReviewEndpointSystemL2Detail>();

                foreach (SAPRoleInfo sapuser in listRoles)
                {

                    reviewRoles = new AttestationReviewEndpointSystemL2Detail();
                    reviewRoles.ReviewEndpointId = reviewEndpointId;
                    reviewRoles.AccessName = sapuser.ROLE_CODE.Trim();
                    reviewRoles.AccessType = EntitlementType.Role;
                    reviewRoles.AccessDescription = sapuser.ROLE_DESC;
                    reviewRoles.Comments = string.Empty;
                    reviewRoles.Approve = true;
                    reviewRoles.PreviousAccess = Constants.APPROVE;
                    liAttestationReviewEndpointSystemL2Detail.Add(reviewRoles);

                }
                return liAttestationReviewEndpointSystemL2Detail;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<AttestationReviewEndpointSystemL3Detail> GetAuthorizationObjects( int userid , long reviewEndpointId )
        {
            IList<AttestationReviewEndpointSystemL3Detail> list = new List<AttestationReviewEndpointSystemL3Detail>();

            return list;
            //IList<SAPRoleToAuthMap>
        }

        public IList<AttestationReviewEndpointSystemL2Detail> GetSingleRoles( int userId , long reviewEndpointId )
        {
            IList<AttestationReviewEndpointSystemL2Detail> list = new List<AttestationReviewEndpointSystemL2Detail>();

            try
            {
                AttestationReviewEndpointSystemL2Detail reviewSingleRoles = null;
                IList<SAPRoleInfo> listRoles = SAPRoleInfo.GetListByRoleType(userId);

                if (listRoles == null)
                    return null;

                if (listRoles.Count <= 0)
                    return null;

                foreach (SAPRoleInfo sapuser in listRoles)
                {

                    reviewSingleRoles = new AttestationReviewEndpointSystemL2Detail();
                    reviewSingleRoles.ReviewEndpointId = reviewEndpointId;
                    reviewSingleRoles.AccessName = sapuser.ROLE_CODE.Trim();
                    reviewSingleRoles.AccessType = EntitlementType.Role;
                    reviewSingleRoles.Comments = string.Empty;
                    reviewSingleRoles.Approve = true;
                    reviewSingleRoles.PreviousAccess = Constants.APPROVE;
                    list.Add(reviewSingleRoles);

                }
                return list;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}