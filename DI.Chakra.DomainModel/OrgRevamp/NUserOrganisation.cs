using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public partial class NUserOrganisation : DomainModelBase<NUserOrganisation, EntityModel.NUserOrganisation>
    {
        public long UserId { get; set; }

        public long OrganisationId { get; set; }

        public long TypeId { get; set; }

        public string Category { get; set; }

        public Nullable<System.DateTime> ValidFrom { get; set; }

        public Nullable<System.DateTime> ValidTo { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public User User { get; set; }

        public NOrganisation Organisation { get; set; }        

        public NScopeFor ScopeFor { get; set; }

        

        public static List<NUserOrganisation> GetUserOrganisation(long UserId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var userOrgDetail = new RepositoryBase<EntityModel.NUserOrganisation>(dataAccessTransaction)
                       .Get(item => item.UserId == UserId);

                if (userOrgDetail != null)
                    return ORMapList(userOrgDetail.ToList());
                else
                    return null;
            }
        }

        public static Dictionary<long, string> GetOrganisationUsers(string searchString) 
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return new UserRepository(transaction).GetActiveAndNonTerminaters(CheckNull.AsteriskToString(searchString));
            }
        }
    }
}
