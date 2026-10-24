using DI.Chakra.Infrastructure;
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
    public partial class NOrganisationScope : DomainModelBase<NOrganisationScope, EntityModel.NOrganisationScope>
    {
        public string Scope { get; set; }
        public string Description { get; set; }

        public int Level { get; set; }

        public bool IsValidForApproval { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }


        public static List<NOrganisationScope> GetByIsActive()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.NOrganisationScope>(dataAccessTransaction)
                       .Get(org => org.IsActive == true).ToList());
            }
        }
        public static List<NOrganisationScope> GetAllOrganisationScope()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<NOrganisationScope> orgList = new List<NOrganisationScope>();
                orgList = NOrganisationScope.GetByIsActive();
                NOrganisationScope organisation;
                long i = orgList.Max(item => item.Id) + 1;
                foreach (var attScope in Enum.GetValues(typeof(AttestationScopeEnum)))
                {
                    //if (((AttestationScopeEnum)Enum.Parse(typeof(AttestationScopeEnum), attScope.ToString(), true) != AttestationScopeEnum.None)&&
                    //   ((AttestationScopeEnum)Enum.Parse(typeof(AttestationScopeEnum), attScope.ToString(), true) != AttestationScopeEnum.OrganisationUnit))
                    var aType = ((AttestationScopeEnum)Enum.Parse(typeof(AttestationScopeEnum), attScope.ToString(), true));
                    if (aType != AttestationScopeEnum.None && aType != AttestationScopeEnum.Company 
                        && aType != AttestationScopeEnum.Department && aType != AttestationScopeEnum.Section
                        && aType != AttestationScopeEnum.Unit && aType != AttestationScopeEnum.Branch
                        )
                    {

                        if (!orgList.Any(a => a.Scope.ToUpper() == attScope.ToString().ToUpper()))
                        {
                            organisation = new NOrganisationScope();
                            organisation.Id = i;
                            organisation.Scope = Convert.ToString((AttestationScopeEnum)Enum.Parse(typeof(AttestationScopeEnum), attScope.ToString(), true));
                            orgList.Add(organisation);
                            i++;
                        }
                    }
                }
                return orgList;
            }
        }

        public static NOrganisationScope GetOrganisationScope(string Scope)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.NOrganisationScope>(dataAccessTransaction)
                     .Get(org => org.Scope.ToLower() == Scope.ToLower()).FirstOrDefault());
            }
        }
    }
}

