using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using DI.Chakra.DomainModel;

namespace DI.Chakra.Services.AttestationProcessor
{
    public class OracleAttestation :EndpointAttestation
    {
        public IList<AttestationReviewEndpointSystemL2Detail> GetRoleReviews( int userId , long reviewEndpointId )
        {
            IList<AttestationReviewEndpointSystemL2Detail> list = new List<AttestationReviewEndpointSystemL2Detail>();

            try
            {
                AttestationReviewEndpointSystemL2Detail reviewRole = null;

                IList<OracleUMRoleInfo> listRoles = OracleUMRoleInfo.GetListByUserId(userId);

                if (listRoles == null)
                    return null;

                if (listRoles.Count <= 0)
                    return null;

                foreach (OracleUMRoleInfo oraUser in listRoles)
                {
                    reviewRole = new AttestationReviewEndpointSystemL2Detail();
                    reviewRole.ReviewEndpointId = reviewEndpointId;
                    reviewRole.AccessName = oraUser.RoleName;
                    reviewRole.Comments = string.Empty;
                    reviewRole.Approve = true;
                    reviewRole.PreviousAccess = Constants.APPROVE;
                    list.Add(reviewRole);
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
