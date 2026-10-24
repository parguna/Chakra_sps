using AutoMapper;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationADUserDetails
    {
        public long Id { get; set; }
        public long EndpointSystemId { get; set; }

        public string AccountName { get; set; }

        public long? WogUserId { get; set; }
        public string physicalDeliveryOfficeName { get; set; }

        public bool NeedO365License { get; set; }
        public static PresentationADUserDetails ORMap(EntityModel.Presentation.PresentationADUserDetails presentationADSUsersEntityModel)
        {
            if (presentationADSUsersEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationADUserDetails, PresentationADUserDetails>(presentationADSUsersEntityModel);
        }

        public static List<PresentationADUserDetails> ORMapList(IList<EntityModel.Presentation.PresentationADUserDetails> presentationADSUsersEntityModels)
        {
            return presentationADSUsersEntityModels.Select(ORMap).ToList();
        }
        public static List<PresentationADUserDetails> GetADUsersForLicenseAssignment(long endpointId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new ADSUserInfoRepository(transaction).GetADUsersForLicenseAssignment(endpointId).ToList());
            }

        }
    }
}
