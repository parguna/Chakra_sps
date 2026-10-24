using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class PresentationOrganisationRepository : RepositoryBase<PresentationOrganisation>
    {
        public PresentationOrganisationRepository(DataAccessTransaction transaction) : base(transaction) { }


        public IList<PresentationOrganisation> GetAllOrganizationInfo()
        {
            return (from orgHead in CurrentContext.NOrganisationHeads
                    join org in CurrentContext.NOrganisations
                    on orgHead.OrganisationId equals org.Id
                    join user in CurrentContext.User
                    on orgHead.HeadId equals user.Id
                    select new PresentationOrganisation
                    {
                        DisplayName = user.DisplayName,
                        OrganisationName = org.Name,
                        OrgUnitPrefix = org.Code,
                        ParentOrganisation = org.ParentOrgCode,
                        Scope = org.OrganisationScope.Scope
                    }).ToList();
        }
    }
}
