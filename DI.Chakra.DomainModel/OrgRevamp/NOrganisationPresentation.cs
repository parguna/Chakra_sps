using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class NOrganisationPresentation : DomainModelBase<NOrganisationPresentation, EntityModel.NOrganisationPresentation>
    {
        public int OrgId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ParentOrgCode { get; set; }

        public Nullable<long> ParentOrgId { get; set; }

        public long ScopeId { get; set; }
        
        public bool IsActive { get; set; }
        
        public virtual NOrganisationScope OrganisationScope { get; set; }

        public List<NOrganisationRelationship> OrganisationRelationship { get; set; }

        public static List<NOrganisationPresentation> GetOrgChart(string OrgCode)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var orgDetail = new RepositoryBase<EntityModel.NOrganisation>(dataAccessTransaction)
                       .Get(item => item.Code == OrgCode).FirstOrDefault();

                if (orgDetail != null)
                    return ORMapList(new NOrganisationRepository(dataAccessTransaction).GetOrgCharByCode(orgDetail.Code));
                else
                    return null;
            }
        }

        public static List<NOrganisationPresentation> GetOrgChart(long OrgId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new NOrganisationRepository(dataAccessTransaction).GetOrgChart(OrgId));
            }
        }

        public static List<NOrganisationPresentation> GetOrgValue(long OrgId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new NOrganisationRepository(dataAccessTransaction).GetOrgValue(OrgId));
            }
        }

        public static string GetOrgLevels(long UserOrgId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List < NOrganisationPresentation> orgs= GetOrgChart(UserOrgId).OrderByDescending(a => a.OrganisationScope.Level).ToList();
                string orgLevelText = string.Empty;
                foreach(NOrganisationPresentation org in orgs)
                {
                    string orgScopeText = org.OrganisationScope !=null ? org.OrganisationScope.Scope :"";
                    orgLevelText = orgLevelText + ";" + string.Format("{0} : {1} - {2}", orgScopeText,org.Code,org.Name);
                }
                return orgLevelText;
            }
        }

        public static List<NOrganisationPresentation> GetUserOrgChart(long UserId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var usr = User.GetById(UserId);
                if (usr != null && usr.NOrganisationId.HasValue)
                    return GetOrgChart((long)usr.NOrganisationId);
                else
                    return new List<NOrganisationPresentation>();
            }
        }
    }
}
