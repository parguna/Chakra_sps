using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;

namespace DI.Chakra.Repository
{
    public class NOrganisationRepository : RepositoryBase<NOrganisation>
    {
        public NOrganisationRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        public void FlushHeadInformation()
        {
            string query1 = "UPDATE DI_M_NOrganisation set ParentOrgId = NULL, ParentOrgCode = NULL";
            string query2 = "UPDATE DI_M_NOrganisationHead set HeadId = NULL, CoordinatorId = NULL";

            this.CurrentContext.Database.ExecuteSqlCommand(query1);
            this.CurrentContext.Database.ExecuteSqlCommand(query2);
        }

        public void UpdateParentOrgId()
        {
            string query1 = "UPDATE org SET org.ParentOrgId = org1.Id FROM DI_M_NOrganisation org JOIN DI_M_NOrganisation org1 ON org.ParentOrgCode = org1.Code";

            this.CurrentContext.Database.ExecuteSqlCommand(query1);
        }

        public Dictionary<long, string> GetOrganisations(string searchString)
        {
            if (CurrentContext.NOrganisationRelationships.Count() > 0)
            {
                return CurrentContext.NOrganisations.Where(item => (item.Name.Contains(searchString) || item.Code.Contains(searchString)))
                    .Select(x => new { Key = x.Id, Value = x.Code + " - " + (!string.IsNullOrEmpty(x.Description) ? x.Description : x.Name) }).Take(1000)
                    .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
            }
            else
            {
                return GetNOrganisationByScope(searchString, (long)Infrastructure.Enums.NOrganisationScopeValue.Department);
            }
        }

        public List<NOrganisationPresentation> GetOrgValue(long OrgId)
        {
            List<NOrganisationPresentation> orgChart = new List<NOrganisationPresentation>();

            var curOrgLevel = CurrentContext.NOrganisations.Include("OrganisationScope").Where(item => item.Id == OrgId).FirstOrDefault();
            orgChart.Add(new NOrganisationPresentation
            {
                Id = curOrgLevel.Id,
                Code = curOrgLevel.Code,
                Name = curOrgLevel.Name,
                Description = curOrgLevel.Description,
                ParentOrgCode = curOrgLevel.ParentOrgCode,
                ParentOrgId = curOrgLevel.ParentOrgId,
                ScopeId = curOrgLevel.ScopeId.HasValue ? (long)curOrgLevel.ScopeId : 0,
                IsActive = curOrgLevel.IsActive,
                OrganisationScope = curOrgLevel.OrganisationScope,
                OrganisationRelationship = CurrentContext.NOrganisationRelationships.Where(a => a.OrganisationId == curOrgLevel.Id).ToList(),
            });

            return orgChart.OrderBy(a => null == a.OrganisationScope ? 1 : a.OrganisationScope.Level).ToList();
        }

        public List<NOrganisationPresentation> GetOrgCharByCode(string OrgChartCode)
        {
            List<NOrganisationPresentation> orgChart = new List<NOrganisationPresentation>();

            var curOrgLevel = CurrentContext.NOrganisations.Include("OrganisationScope").Where(item => item.Code == OrgChartCode).FirstOrDefault();
            orgChart.Add(new NOrganisationPresentation
            {
                Id = curOrgLevel.Id,
                Code = curOrgLevel.Code,
                Name = curOrgLevel.Name,
                Description = curOrgLevel.Description,
                ParentOrgCode = curOrgLevel.ParentOrgCode,
                ParentOrgId = curOrgLevel.ParentOrgId,
                ScopeId = curOrgLevel.ScopeId.HasValue ? (long)curOrgLevel.ScopeId : 0,
                IsActive = curOrgLevel.IsActive,
                OrganisationScope = curOrgLevel.OrganisationScope,
                OrganisationRelationship = CurrentContext.NOrganisationRelationships.Where(a => a.OrganisationId == curOrgLevel.Id).ToList(),
            });

            if (curOrgLevel != null)
            {
                string parentCode = curOrgLevel.ParentOrgCode;

                while (!string.IsNullOrWhiteSpace(parentCode))
                {
                    curOrgLevel = CurrentContext.NOrganisations.Include("OrganisationScope").Where(item => item.Code == parentCode).FirstOrDefault();
                    if (curOrgLevel != null)
                    {
                        parentCode = curOrgLevel.ParentOrgCode;
                        orgChart.Add(new NOrganisationPresentation
                        {
                            Id = curOrgLevel.Id,
                            Code = curOrgLevel.Code,
                            Name = curOrgLevel.Name,
                            Description = curOrgLevel.Description,
                            ParentOrgCode = curOrgLevel.ParentOrgCode,
                            ParentOrgId = curOrgLevel.ParentOrgId,
                            ScopeId = curOrgLevel.ScopeId.HasValue ? (long)curOrgLevel.ScopeId : 0,
                            IsActive = curOrgLevel.IsActive,
                            OrganisationScope = curOrgLevel.OrganisationScope,
                            OrganisationRelationship = CurrentContext.NOrganisationRelationships.Where(a => a.OrganisationId == curOrgLevel.Id).ToList(),
                        });
                    }
                    else
                    {//To avoid indefinite looping
                        parentCode = null;
                    }
                }
            }
            return orgChart.OrderBy(a => null == a.OrganisationScope ? 1 : a.OrganisationScope.Level).ToList();
        }

        public List<NOrganisationPresentation> GetOrgChart(long OrgChartId)
        {
            List<NOrganisationPresentation> orgChart = new List<NOrganisationPresentation>();

            var curOrgLevel = CurrentContext.NOrganisations.Include("OrganisationScope").Where(item => item.Id == OrgChartId).FirstOrDefault();
            orgChart.Add(new NOrganisationPresentation
            {
                Id = curOrgLevel.Id,
                Code = curOrgLevel.Code,
                Name = curOrgLevel.Name,
                Description = curOrgLevel.Description,
                ParentOrgCode = curOrgLevel.ParentOrgCode,
                ParentOrgId = curOrgLevel.ParentOrgId,
                ScopeId = curOrgLevel.ScopeId.HasValue ? (long)curOrgLevel.ScopeId : 0,
                IsActive = curOrgLevel.IsActive,
                OrganisationScope = curOrgLevel.OrganisationScope,
                OrganisationRelationship = CurrentContext.NOrganisationRelationships.Where(a => a.OrganisationId == curOrgLevel.Id).ToList(),
            });

            if (curOrgLevel != null)
            {
                long? parentId = curOrgLevel.ParentOrgId;

                while (parentId != null && parentId > 0)
                {
                    curOrgLevel = CurrentContext.NOrganisations.Include("OrganisationScope").Where(item => item.Id == parentId).FirstOrDefault();
                    if (curOrgLevel != null)
                    {
                        parentId = curOrgLevel.ParentOrgId;
                        orgChart.Add(new NOrganisationPresentation
                        {
                            Id = curOrgLevel.Id,
                            Code = curOrgLevel.Code,
                            Name = curOrgLevel.Name,
                            Description = curOrgLevel.Description,
                            ParentOrgCode = curOrgLevel.ParentOrgCode,
                            ParentOrgId = curOrgLevel.ParentOrgId,
                            ScopeId = curOrgLevel.ScopeId.HasValue ? (long)curOrgLevel.ScopeId : 0,
                            IsActive = curOrgLevel.IsActive,
                            OrganisationScope = curOrgLevel.OrganisationScope,
                            OrganisationRelationship = CurrentContext.NOrganisationRelationships.Where(a => a.OrganisationId == curOrgLevel.Id).ToList(),
                        });
                    }
                    else
                    {
                        //To avoid indefinite looping
                        parentId = null;
                    }
                }
            }
            return orgChart.OrderBy(a => null == a.OrganisationScope ? 1 : a.OrganisationScope.Level).ToList();
        }
        public Dictionary<string, string> GetOrganisationCodesByScopeId(string searchString, long scopeId)
        {
            return CurrentContext.NOrganisations.Where(item => item.ScopeId == scopeId && item.ScopeId != null && item.ScopeId >= 0 && (item.Name.Contains(searchString) || item.Code.Contains(searchString)))
                .Select(x => new { Key = x.Code, Value = $"{(string.IsNullOrEmpty(x.Description) ? x.Name : x.Description)}-{x.Code}" })
                .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
        }

        public Dictionary<string, string> GetOrganisationCodesByScopeId(string searchString, long scopeId, string ParentCode)
        {
            var query = CurrentContext.NOrganisations.Where(item => item.ScopeId == scopeId && item.ScopeId != null && item.ScopeId >= 0 && (item.Name.Contains(searchString) || item.Code.Contains(searchString)));

            if (!string.IsNullOrWhiteSpace(ParentCode))
            {
                query = query.Where(i => i.ParentOrgCode == ParentCode);
            }       


            Dictionary<string, string> ret = query.Select(x => new { Key = x.Code, Value = x.Name+"("+x.Code+")" })
                .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
            return ret;
        }
        public Dictionary<long, string> GetOrganisationsByScopeId(string searchString, long scopeId)
        {
            return CurrentContext.NOrganisations.Where(item => item.ScopeId == scopeId && item.ScopeId != null && item.ScopeId >= 0 && (item.Name.Contains(searchString) || item.Code.Contains(searchString)))
                .Select(x => new { Key = x.Id, Value = x.Code + " - " + (string.IsNullOrEmpty(x.Description) ? x.Name : x.Description) })
                .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
        }


        public Dictionary<string, string> GetOrganisationsByName(string name)
        {
            var query = CurrentContext.NOrganisations.Where(item =>   (item.Name.Contains(name) || item.Name.StartsWith(name))); 

            Dictionary<string, string> ret = query.Select(x => new { Key = x.Code, Value = x.Name + "(" + x.Code + ")" })
                .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
            return ret;
        }

        public Dictionary<long, string> GetInternalCompany(string searchString, long scopeId)
        {
            return CurrentContext.NOrganisations.Where(item => (item.Name.Contains(searchString) || item.Code.Contains(searchString)) && item.ScopeId == scopeId && item.ScopeId != null && item.ScopeId >= 0)
                .Select(x => new { Key = x.Id, Value = x.Code + " - " + (string.IsNullOrEmpty(x.Description) ? x.Name : x.Description) })
                .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
        }

        public Dictionary<long, string> GetExternalCompany(string searchString, long scopeId)
        {
            return CurrentContext.NOrganisations.Where(item => (item.Name.Contains(searchString) || item.Code.Contains(searchString)) && item.ScopeId == scopeId && item.ScopeId != null && item.ScopeId >= 0)
                .Select(x => new { Key = x.Id, Value = x.Code + " - " + (string.IsNullOrEmpty(x.Description) ? x.Name : x.Description) })
                .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
        }

        public Dictionary<long, string> GetNOrganisationByScope(string searchString, long scopeId)
        {
            return CurrentContext.NOrganisations.Where(item => (item.Name.Contains(searchString) || item.Code.Contains(searchString)) && item.ScopeId == scopeId && item.ScopeId != null && item.ScopeId >= 0)
                .Select(x => new { Key = x.Id, Value = x.Code + " - " + (string.IsNullOrEmpty(x.Description) ? x.Name : x.Description) })
                .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
        }

        public long GetTopmostByHeadId(long userId)
        {
            return (from oh in CurrentContext.NOrganisationHeads
                    join org in CurrentContext.NOrganisations
                    on oh.OrganisationId equals org.Id
                    join scp in CurrentContext.NOrganisationScopes
                    on org.ScopeId equals scp.Id
                    where oh.HeadId == userId
                    orderby scp.Level
                    select org.Id).FirstOrDefault();
        }
        public List<NOrganisation> GetAllOrganisations()
        {
            var orgs = CurrentContext.NOrganisations.Include("OrganisationScope").ToList();
            return orgs;
        }
    }
}
