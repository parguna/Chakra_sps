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
    public partial class NOrganisationHead : DomainModelBase<NOrganisationHead, EntityModel.NOrganisationHead>
    {
        public long OrganisationId { get; set; }

        public long? HeadId { get; set; }

        public long? CoordinatorId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public NOrganisation Organisation { get; set; }

        public User Head { get; set; }

        public User Coordinator { get; set; }

        public static User GetOrganisationLevelHead(long UserId, string Scope)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var user = User.GetById(UserId);
                if (user != null && user.NOrganisationId != null)
                {
                    var orgDetails = NOrganisationPresentation.GetOrgChart((long)user.NOrganisationId);
                    var scope = orgDetails.Where(a => a.OrganisationScope.Scope == Scope).FirstOrDefault();
                    if (scope != null)
                    {
                        var head = ORMap(new NOrganisationHeadRepository(dataAccessTransaction).Get(a => a.OrganisationId == scope.Id, true).FirstOrDefault());
                        if (head != null)
                        {
                            return head.Head;
                        }
                    }
                }
                return null;
            }
        }

        public static User GetOrganisationLevelCoordinator(long UserId, string Scope)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var user = User.GetById(UserId);
                if (user != null && user.NOrganisationId != null)
                {
                    var orgDetails = NOrganisationPresentation.GetOrgChart((long)user.NOrganisationId);
                    var scope = orgDetails.Where(a => a.OrganisationScope.Scope == Scope).FirstOrDefault();
                    if (scope != null)
                    {
                        var head = ORMap(new NOrganisationHeadRepository(dataAccessTransaction).Get(a => a.OrganisationId == scope.Id, true).FirstOrDefault());
                        if (head != null)
                        {
                            return head.Coordinator;
                        }
                    }
                }
                return null;
            }
        }

        public static long GetOrganisationHead(long OrgId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var orgHead = ORMap(new NOrganisationHeadRepository(dataAccessTransaction).Get(a => a.OrganisationId == OrgId, true).FirstOrDefault());

                if (orgHead != null)
                {
                    return orgHead.HeadId.HasValue ? orgHead.HeadId.Value : 0;
                }
                return 0;
            }
        }

        public static long GetByOrganisationIdAndHead(long OrgId, long head)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var orgHead = ORMap(new NOrganisationHeadRepository(dataAccessTransaction).Get(a => a.OrganisationId == OrgId && a.HeadId == head, true).FirstOrDefault());

                if (orgHead != null)
                {
                    return orgHead.HeadId.HasValue ? orgHead.HeadId.Value : 0;
                }
                return 0;
            }
        }

        public static User GetImmediateSupervisor(long UserId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var user = User.GetById(UserId);
                if (user != null && user.NOrganisationId != null)
                {
                    var orgDetails = NOrganisationPresentation.GetOrgChart((long)user.NOrganisationId);
                    orgDetails.Reverse();
                    if (orgDetails != null && orgDetails.Count > 0)
                    {
                        foreach (var orgDetail in orgDetails)
                        {
                            var head = ORMap(new NOrganisationHeadRepository(dataAccessTransaction).Get(a => a.OrganisationId == orgDetail.Id).FirstOrDefault());
                            if (head != null && head.Id != UserId)
                            {
                                return head.Head;
                            }
                        }
                    }
                }
                return null;
            }
        }

        public static User GetImmediateCoordinator(long UserId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var user = User.GetById(UserId);
                if (user != null && user.NOrganisationId != null)
                {
                    var orgDetails = NOrganisationPresentation.GetOrgChart((long)user.NOrganisationId);
                    orgDetails.Reverse();
                    if (orgDetails != null && orgDetails.Count > 0)
                    {
                        foreach (var orgDetail in orgDetails)
                        {
                            var head = ORMap(new NOrganisationHeadRepository(dataAccessTransaction).Get(a => a.OrganisationId == orgDetail.Id).FirstOrDefault());
                            if (head != null)
                            {
                                return head.Coordinator;
                            }
                        }
                    }
                }
                return null;
            }
        }

        public static Dictionary<long, string> GetOrganisationHead(string searchString)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return new NOrganisationHeadRepository(transaction).GetOrganisationHead(CheckNull.AsteriskToString(searchString));
            }
        }

        public static bool IsOrgHead(long userId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                bool ishead = new RepositoryBase<EntityModel.NOrganisationHead>(dataAccessTransaction).Any(u => u.HeadId == userId);

                if (ishead)
                {
                    long orgId = new RepositoryBase<EntityModel.NOrganisationHead>(dataAccessTransaction).Get(o => o.HeadId == userId).
                                    FirstOrDefault().OrganisationId;

                    bool hasUser = new RepositoryBase<EntityModel.User>(dataAccessTransaction).Any(u => u.NOrganisationId == orgId);
                    bool hasSubOrgs = new RepositoryBase<EntityModel.NOrganisation>(dataAccessTransaction).Any(u => u.ParentOrgId == orgId);

                    if (orgId > 0 && (hasUser || hasSubOrgs))
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
        }

        public static NOrganisationHead GetByOrganisationId(long organisationId, bool eagerLoadAllProperties = true)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.NOrganisationHead>(dataAccessTransaction).Get(orgHead => orgHead.OrganisationId == organisationId, eagerLoadAllProperties).FirstOrDefault());
            }
        }

        public static List<NOrganisationHead> GetAllByOrganisationCode(string organisationCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new NOrganisationHeadRepository(dataAccessTransaction).GetAllByOrgCode(organisationCode));
            }
        }
        public static NOrganisationHead GeByOrgCode(string organisationCode, long scopeId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new NOrganisationHeadRepository(dataAccessTransaction).GeByOrgCode(organisationCode, scopeId));
            }
        }
        
    }
}
