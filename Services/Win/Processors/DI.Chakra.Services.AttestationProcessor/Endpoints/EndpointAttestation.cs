using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.DomainModel;

namespace DI.Chakra.Services.AttestationProcessor
{
    public class EndpointAttestation
    {
        public List<AttestationReviewEndpointSystemL2Detail> liAttestationReviewEndpointSystemL2Detail { get; set; }


        public virtual IList<AttestationReviewEndpointSystem> GetEndpointReviews(long userId)
        {

            IList<AttestationReviewEndpointSystem> retlist = new List<AttestationReviewEndpointSystem>();

            //ReconcilationFacade reconFacade = new ReconcilationFacade();
            try
            {

                //var listAccntRecon = reconFacade.GetAccountReconcilations(userId);
                // get entries in account reconciliation for this user id which are not locked
                var listAccntRecon = AccountReconcilation.GetByUser(userId);

                // create a list of AttestationReviewEndpointSystem based on the above data
                retlist = (from acnt in listAccntRecon
                           select new AttestationReviewEndpointSystem()
                           {
                               EndpointSystemId = acnt.EndpointSystemId,
                               EndpointUserId = acnt.EndpointUserId,
                               EndpointUserName = acnt.AccountName,
                               HostName = acnt.EndPoint.DisplayName,
                               EndpointSystemTypeId = acnt.EndPoint.EndpointSystemTypeId.Value,
                               EndpointSystemTypeName = acnt.EndPoint.EndPointSystemType.DisplayName,
                               Comments = string.Empty,
                               Approve = true,
                               PreviousAccess = Constants.APPROVE,
                               AccountType = acnt.AccountType,
                               IsActive = !acnt.IsLocked,

                           }).ToList();



            }
            catch (Exception)
            {
                throw;
            }
            return retlist;
        }

        public virtual IList<AttestationReviewEndpointSystemL2Detail> GetRoleReviews(long endpointUserId, long reviewEndpointId)
        {

            liAttestationReviewEndpointSystemL2Detail = new List<AttestationReviewEndpointSystemL2Detail>();

            try
            {
                // get user info from corresponding user table based on endpointtype id
                IEndpointUserInfo usrInfo = AccountReconcilation.GetEndpointUserInfo(reviewEndpointId,endpointUserId);

                if (usrInfo == null)
                    return new List<AttestationReviewEndpointSystemL2Detail>();

                IList<AttestationReviewEndpointSystemL2Detail> list = new List<AttestationReviewEndpointSystemL2Detail>();


                if (usrInfo.Entitlements == null)
                {
                    return new List<AttestationReviewEndpointSystemL2Detail>();
                }

                foreach (IEndpointEntitlementInfo ent in usrInfo.Entitlements)
                {
                    var reviewGroup = getL2Review(ent);
                    liAttestationReviewEndpointSystemL2Detail.Add(reviewGroup);
                }

                if (liAttestationReviewEndpointSystemL2Detail.Count > 0)
                    liAttestationReviewEndpointSystemL2Detail.ToList().ForEach(f => f.CreatedDate = DateTime.Now);

                return liAttestationReviewEndpointSystemL2Detail;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private AttestationReviewEndpointSystemL2Detail getL2Review(IEndpointEntitlementInfo entitlementInfo)
        {
            AttestationReviewEndpointSystemL2Detail reviewGroup = null;

            try
            {
                reviewGroup = new AttestationReviewEndpointSystemL2Detail();
                reviewGroup.ReviewEndpointId = entitlementInfo.EndPointSystemId;
                reviewGroup.ReviewEntitlementId = entitlementInfo.Id;
                reviewGroup.Comments = string.Empty;
                reviewGroup.AccessName = entitlementInfo.EntitlementName;
                reviewGroup.AccessType = entitlementInfo.EntitlementType;
                reviewGroup.AccessDescription = entitlementInfo.EntitlementDesc;
                reviewGroup.Comments = string.Empty;
                reviewGroup.Approve = true;
                reviewGroup.PreviousAccess = Constants.APPROVE;
            }
            catch (Exception)
            {

                throw;
            }
            return reviewGroup;
        }
        public static EndpointAttestation GetAttestationServiceByEndpointSystemTypeId(long EndpointSystemTypeId)
        {
            EndpointAttestation _att = null;

            try
            {
                switch (EndpointSystemTypeId)
                {


                    case (int)EndpointType.SAP:
                        _att = new SAPAttestation();
                        break;
                    case (int)EndpointType.Linux:
                        _att = new LinuxAttestation();
                        break;

                    default:
                        _att = new EndpointAttestation();
                        break;
                }
            }
            catch (Exception)
            {

                throw;
            }

            return _att;
        }


    }
}
