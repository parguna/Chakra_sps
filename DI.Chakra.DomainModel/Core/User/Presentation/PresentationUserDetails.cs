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
    public class PresentationUserDetails
    {
        public bool FullTimeEmployee { get; set; }
        public string EmployeeId { get; set; }
        public long Id { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public DateTime? JoinDate { get; set; }
        public string TelephoneNumber { get; set; }
        public static List<PresentationUserDetails> GetPresentationUsersToUpdateEmailIdInHR(bool takeOnlyNeedTelephoneUpdateToHR=false, bool takeOnlyNeedEmailUpdateToHR=false,bool takeUpdateFLINSLoginIdInHR=false)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new UserRepository(transaction).GetPresentationUsersToUpdateEmailIdInHR(takeOnlyNeedTelephoneUpdateToHR, takeOnlyNeedEmailUpdateToHR, takeUpdateFLINSLoginIdInHR));
                //return ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(user => user.NeedEmailUpdateToHR).ToList());
            }
        }
        public static PresentationUserDetails ORMap(EntityModel.Presentation.PresentationUserDetails presentationADSUsersEntityModel)
        {
            if (presentationADSUsersEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationUserDetails, PresentationUserDetails>(presentationADSUsersEntityModel);
        }

        public static List<PresentationUserDetails> ORMapList(IList<EntityModel.Presentation.PresentationUserDetails> presentationADSUsersEntityModels)
        {
            return presentationADSUsersEntityModels.Select(ORMap).ToList();
        }
    }
}
