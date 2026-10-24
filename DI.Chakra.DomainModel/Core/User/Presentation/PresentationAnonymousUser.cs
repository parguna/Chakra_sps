using AutoMapper;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationAnonymousUser
    {
        public string UserName { get; set; }

        public string Password { get; set; }
        public Nullable<bool> FullTimeEmployee { get; set; }

        public Nullable<bool> IsActive { get; set; }

        public string AuthenticationMode { get; set; }

        public EmployeeType UserEmployeeType { get; set; }
        public static PresentationAnonymousUser GetUserByUserName(string userName)
        {

            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                PresentationAnonymousUser user = ORMap(new UserRepository(transaction).GetUserByUserName(userName));
                if (user != null)
                {
                    user.Password = RSAHelper.DecryptData(user.Password);
                    return user;
                }
                else
                    return null;
            }

        }

        public static PresentationAnonymousUser ORMap(EntityModel.Presentation.PresentationAnonymousUser presentationUser)
        {
            if (presentationUser == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationAnonymousUser, PresentationAnonymousUser>(presentationUser);
        }

        public static List<PresentationAnonymousUser> ORMapList(IList<EntityModel.Presentation.PresentationAnonymousUser> presentationUser)
        {
            return presentationUser.Select(ORMap).ToList();
        }
    }
}
