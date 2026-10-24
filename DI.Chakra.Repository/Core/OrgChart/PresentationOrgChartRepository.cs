using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Repository
{
    public class PresentationOrgChartRepository : RepositoryBase<PresentationOrgChart>
    {
        public PresentationOrgChartRepository(DataAccessTransaction transaction) : base(transaction) { }

        public static long counter = 0;
        public List<PresentationOrgChart> GetOrgMembers(long orgId, long parentId, long userId)
        {
            List<PresentationOrgChart> orgMembers =
                                                (
                                                from usr in CurrentContext.User
                                                join org in CurrentContext.NOrganisations
                                                on usr.NOrganisationId equals org.Id
                                                join des in CurrentContext.NDesignations
                                                on usr.NBusinessRoleId equals des.Id into destemp
                                                from dt in destemp.DefaultIfEmpty()
                                                join ui in CurrentContext.UserImage
                                                on usr.Id equals ui.UserId into imgtemp
                                                from it in imgtemp.DefaultIfEmpty()
                                                where usr.NOrganisationId == orgId && usr.Id != userId
                                                select new PresentationOrgChart
                                                {
                                                    UserId = usr.Id,
                                                    Name = usr.DisplayName,
                                                    Designation = dt.Description,
                                                    OrgId = org.Id,
                                                    ParentId = parentId,
                                                    NodeType = OrgChartNodeType.NormalUserNode,
                                                    OrgHeadCaption = string.Empty,
                                                    Image = it.Image
                                                }).ToList();
            orgMembers.All(x => { counter = counter + 1; x.Id = counter; return true; });
            List<PresentationOrgChart> orgs = GetOrganisations(orgId, parentId);

            orgMembers = orgMembers.Union(orgs).ToList();
            return orgMembers;
        }

        public List<PresentationOrgChart> GetOrganisations(long orgId, long parentId)
        {
            List<PresentationOrgChart> memberOrgs = (from c in CurrentContext.NOrganisations
                                                     join o in CurrentContext.NOrganisationHeads
                                                     on c.Id equals o.OrganisationId into sr
                                                     from x in sr.DefaultIfEmpty()
                                                     join u in CurrentContext.User
                                                     on x.HeadId equals u.Id into temp
                                                     from t in temp.DefaultIfEmpty()
                                                     join des in CurrentContext.NDesignations
                                                     on t.NBusinessRoleId equals des.Id into destemp
                                                     from dt in destemp.DefaultIfEmpty()
                                                     join ui in CurrentContext.UserImage
                                                     on t.Id equals ui.UserId into imgtemp
                                                     from it in imgtemp.DefaultIfEmpty()
                                                     where c.ParentOrgId == orgId
                                                     select new PresentationOrgChart
                                                     {
                                                         UserId = t.Id > 0 ? t.Id : 0,
                                                         Name = t.DisplayName == null ? "Head Not Available" : t.DisplayName,
                                                         Designation = dt.Description,
                                                         OrgId = c.Id,
                                                         ParentId = parentId,
                                                         NodeType = OrgChartNodeType.OrgHeadNode,
                                                         OrgHeadCaption = c.Name,
                                                         Image = it.Image
                                                     }).ToList();
            memberOrgs.All(x => { counter = counter + 1; x.Id = counter; return true; });
            return memberOrgs;
        }

        public PresentationOrgChart GetFirstLevelHeadNode(long userId, long parentOrgId)
        {
            PresentationOrgChart headNode =
                                                (from usr in CurrentContext.User
                                                 join oh in CurrentContext.NOrganisationHeads
                                                 on usr.Id equals oh.HeadId
                                                 join org in CurrentContext.NOrganisations
                                                 on oh.OrganisationId equals org.Id
                                                 join des in CurrentContext.NDesignations
                                                 on usr.NBusinessRoleId equals des.Id into destemp
                                                 from dt in destemp.DefaultIfEmpty()
                                                 join ui in CurrentContext.UserImage
                                                 on usr.Id equals ui.UserId into imgtemp
                                                 from it in imgtemp.DefaultIfEmpty()
                                                 where oh.HeadId == userId && org.Id == parentOrgId
                                                 select new PresentationOrgChart
                                                 {
                                                     Id = counter + 1,
                                                     UserId = usr.Id,
                                                     Name = usr.DisplayName,
                                                     Designation = dt.Description,
                                                     OrgId = org.Id,
                                                     ParentId = null,
                                                     NodeType = OrgChartNodeType.OrgHeadNode,
                                                     OrgHeadCaption = org.Name,
                                                     Image = it.Image
                                                 }).FirstOrDefault();
            counter = counter + 1;
            return headNode;
        }
    }
}
