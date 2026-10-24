using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;

namespace DI.Chakra.Repository
{
    public class NOrganisationHeadRepository : RepositoryBase<NOrganisationHead>
    {
        public NOrganisationHeadRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        public Dictionary<long, string> GetOrganisationHead(string searchString)
        {
            var uList = (from org in CurrentContext.NOrganisations
                         join orgHead in CurrentContext.NOrganisationHeads on org.Id equals orgHead.OrganisationId
                         join usr in CurrentContext.User on orgHead.HeadId equals usr.Id
                         where usr.IsActive == true && usr.FullTimeEmployee == true && usr.DisplayName.Contains(searchString)
                         select new {  UserId = usr.Id, DisplayName=usr.DisplayName + " [" + usr.UserName + "]" }).Distinct();
            return uList.ToDictionary(c => c.UserId, c => c.DisplayName);

        }


        public List<NOrganisationHead> GetAllByOrgCode(string OrgCode)
        {
            var uList = (from orgHead in CurrentContext.NOrganisationHeads //.Include("OrganisationId").Include("HeadId")
                         where orgHead.Organisation.Code == OrgCode
                         select orgHead); 

            return uList.ToList();

        }
        public NOrganisationHead GeByOrgCode(string OrgCode,long scopeId)
        {
            var org = CurrentContext.NOrganisations.Where(i=> i.Code == OrgCode && i.ScopeId == scopeId);
            return(from orgHead in CurrentContext.NOrganisationHeads
                         join organisation in org on orgHead.OrganisationId equals organisation.Id
                         select orgHead).FirstOrDefault();
        }

    }
}
