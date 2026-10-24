using AutoMapper;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{

    public class PresentationADSActiveAccountsWithInActiveUser
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public DateTime? TerminatedDate { get; set; }
        public bool UserIsActive { get; set; }
        public string EmailId { get; set; }
        public string OrganisationName { get; set; }

        public long EndPointSystemId { get; set; }

        public string sAMAccountName { get; set; }
        public bool AccountIsActive { get; set; }

        public string displayName { get; set; }

        public string department { get; set; }

        public DateTime? whenCreated { get; set; }

        public DateTime? lastLogon { get; set; }

        public DateTime? pwdLastSet { get; set; }

        public DateTime? accountExpires { get; set; }

        public bool IsAccountDisabled { get; set; }

        public bool IsAccountLocked { get; set; }

        public static PresentationADSActiveAccountsWithInActiveUser ORMap(EntityModel.Presentation.PresentationADSActiveAccountsWithInActiveUser presentationADSActiveAccountsWithInActiveUser)
        {
            if (presentationADSActiveAccountsWithInActiveUser == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationADSActiveAccountsWithInActiveUser, PresentationADSActiveAccountsWithInActiveUser>(presentationADSActiveAccountsWithInActiveUser);
        }

        public static List<PresentationADSActiveAccountsWithInActiveUser> ORMapList(IList<EntityModel.Presentation.PresentationADSActiveAccountsWithInActiveUser> presentationADSActiveAccountsWithInActiveUsers)
        {
            return presentationADSActiveAccountsWithInActiveUsers.Select(ORMap).ToList();
        }

        public static List<PresentationADSActiveAccountsWithInActiveUser> GetADSActiveAccountsWithInActiveUser(long endpointSystemId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationADSActiveAccountsWithInActiveUserRepository(transaction).GetADSActiveAccountsWithInActiveUser(endpointSystemId));
            }
        }
    }
}
