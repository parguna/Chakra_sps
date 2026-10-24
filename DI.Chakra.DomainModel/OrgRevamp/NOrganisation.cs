using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public partial class NOrganisation : DomainModelBase<NOrganisation, EntityModel.NOrganisation>
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ParentOrgCode { get; set; }

        public Nullable<long> ParentOrgId { get; set; }

        public Nullable<long> ScopeId { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<DateTime> CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public virtual NOrganisationScope OrganisationScope { get; set; }

        public int Level;

        public override string ToString()
        {
            return this.Name;
        }

        public bool HasChildren
        {
            get
            {
                if (NOrganisation.GetByParentOrgId(this.Id).Count > 0)
                    return true;
                return false;
            }
        }

        public static NOrganisation GetByOrgCode(string code)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new NOrganisationRepository(dataAccessTransaction).Get(org => org.Code == code, true).FirstOrDefault());
            }
        }

        public static NOrganisation GetByOrgCodeAndScopeId(string code, long scopeId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new NOrganisationRepository(dataAccessTransaction).Get(org => org.Code == code && org.ScopeId == scopeId, true).FirstOrDefault());
            }
        }

        public static NOrganisation GetByCompanyCode(string code)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.NOrganisation>(dataAccessTransaction)
                       .Get(org => org.Code == code && org.ScopeId == 1).FirstOrDefault());
            }

        }

        public static long GetTopmostByHeadId(long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new NOrganisationRepository(dataAccessTransaction).GetTopmostByHeadId(userId);
            }
        }

        public static List<NOrganisation> GetByParentOrgId(long? parentOrgId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new NOrganisationRepository(dataAccessTransaction).Get(org => org.ParentOrgId == parentOrgId).ToList());
            }
        }

        public static Dictionary<long, string> GetOrganisations(string searchString)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new NOrganisationRepository(dataAccessTransaction).GetOrganisations(CheckNull.AsteriskToString(searchString));
            }
        }

        public static Dictionary<long, string> GetNOrganisationByScope(string searchString, long scopeId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new NOrganisationRepository(dataAccessTransaction).GetNOrganisationByScope(CheckNull.AsteriskToString(searchString), scopeId);
            }
        }

        public static bool IsUserOrgHead(long userID)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ((new RepositoryBase<EntityModel.NOrganisationHead>(dataAccessTransaction)
                    .Get(org => org.HeadId == userID).FirstOrDefault()) != null) ? true : false;
            }
        }

        public static List<string> GetChildOrgs(string parentOrg)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<string> orgList = new List<string>();

                orgList.Add(parentOrg);
                var childs = NOrganisation.GetByChildOrganisationForParentOrgCode(parentOrg);

                if (childs != null && childs.Count != 0)
                {
                    foreach (NOrganisation org in childs)
                        orgList.Add(org.Code);
                }

                return orgList;

            }
        }

        public static List<NOrganisation> GetByChildOrganisationForParentOrgCode(string parentOrganisation)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.NOrganisation>(dataAccessTransaction)
                       .Get(org => org.ParentOrgCode == parentOrganisation).ToList());
            }
        }

        public static List<NOrganisation> GetOrganisationWithLevel()
        {
            List<NOrganisation> finalList = new List<NOrganisation>();
            var parentList = (from org in NOrganisation.GetByParentOrgId(null)
                              select new NOrganisation
                              {
                                  Level = 1,
                                  Id = org.Id,
                                  Code = org.Code,
                                  Name = org.Name,
                                  ParentOrgId = org.ParentOrgId,
                                  ParentOrgCode = org.ParentOrgCode,
                                  IsActive = org.IsActive,
                                  Description = org.Description,
                                  ScopeId = org.ScopeId,
                                  OrganisationScope = org.OrganisationScope,
                                  LastModifiedBy = org.LastModifiedBy,
                                  LastModifiedDate = org.LastModifiedDate,
                                  CreatedBy = org.CreatedBy,
                                  CreatedDate = org.CreatedDate
                              }).ToList();
            finalList.AddRange(parentList);
            foreach (var parent in parentList)
            {
                finalList = finalList.Concat(RecursiveLoad(parent)).ToList();
                finalList = finalList.Distinct().ToList();
            }

            return finalList;
        }


        public static List<NOrganisation> GetCompaniesByFilter(DateTime? FromDate, DateTime? ToDate, bool? Internal)
        {
            List<NOrganisation> list = new List<NOrganisation>();

            NOrganisationScope companyScope = NOrganisationScope.GetOrganisationScope("Company");
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (FromDate.HasValue && ToDate.HasValue && Internal.HasValue)
                {
                    list = ORMapList(new RepositoryBase<EntityModel.NOrganisation>(dataAccessTransaction).Get(org =>
                     org.CreatedDate >= FromDate && org.CreatedDate <= ToDate && org.ScopeId == companyScope.Id
                    ).ToList());
                }

                else if (FromDate.HasValue && ToDate.HasValue)
                {
                    list = ORMapList(new RepositoryBase<EntityModel.NOrganisation>(dataAccessTransaction).Get(org =>
                     org.CreatedDate >= FromDate && org.CreatedDate <= ToDate && org.ScopeId == companyScope.Id
                   ).ToList());

                }
                else if (Internal.HasValue)
                {
                    list = ORMapList(new RepositoryBase<EntityModel.NOrganisation>(dataAccessTransaction).Get(org =>
                        org.ScopeId == companyScope.Id
                  ).ToList());
                }
                else
                {
                    list = ORMapList(new RepositoryBase<EntityModel.NOrganisation>(dataAccessTransaction).GetAll().Where(m => m.ScopeId == companyScope.Id).ToList());
                }
            }
            return list;

        }

        public static List<NOrganisation> GetOrgCompanies()
        {

            NOrganisationScope companyScope = NOrganisationScope.GetOrganisationScope("Company");
            if (companyScope != null)
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMapList(new RepositoryBase<EntityModel.NOrganisation>(dataAccessTransaction).Get(m => m.ScopeId == companyScope.Id).ToList());
                }
            }
            else
                return new List<NOrganisation>();
        }


        static List<NOrganisation> RecursiveLoad(NOrganisation parent)
        {
            var childForTheParent = (from org in NOrganisation.GetByParentOrgId(parent.Id)
                                     select new NOrganisation
                                     {
                                         Level = parent.Level + 1,
                                         Id = org.Id,
                                         Code = org.Code,
                                         Name = org.Name,
                                         ParentOrgId = org.ParentOrgId,
                                         ParentOrgCode = org.ParentOrgCode,
                                         IsActive = org.IsActive,
                                         Description = org.Description,
                                         ScopeId = org.ScopeId,
                                         OrganisationScope = org.OrganisationScope,
                                         LastModifiedBy = org.LastModifiedBy,
                                         LastModifiedDate = org.LastModifiedDate,
                                         CreatedBy = org.CreatedBy,
                                         CreatedDate = org.CreatedDate
                                     }).ToList();

            List<NOrganisation> final = new List<NOrganisation>();
            final.AddRange(childForTheParent);
            foreach (var child in childForTheParent)
            {
                final.AddRange(RecursiveLoad(child));
            }

            return final;
        }
        public static Dictionary<long, string> GetBusinessUnits(string searchString)
        {
            Dictionary<long, string> businessUnits = new Dictionary<long, string>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                NOrganisationScope businessUnitScope = NOrganisationScope.GetOrganisationScope("BusinessUnit");
                businessUnits = new NOrganisationRepository(dataAccessTransaction).GetOrganisationsByScopeId(CheckNull.AsteriskToString(searchString), businessUnitScope.Id);
            }
            return businessUnits;
        }

        public static Dictionary<string,string> GetOrganisationsByName(string name)
        { 
            Dictionary<string, string> departMents = new Dictionary<string, string>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                departMents = new NOrganisationRepository(dataAccessTransaction).GetOrganisationsByName(name);
            }
            return departMents;
        }
        public static Dictionary<string, string> GetOrganisations(string searchString, string scopeID, string ParentCode)
        {
            searchString = CheckNull.AsteriskToString(searchString);
            Dictionary<string, string> departMents = new Dictionary<string, string>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                NOrganisationScope departMentScope = NOrganisationScope.GetOrganisationScope(scopeID);
                 
                departMents = new NOrganisationRepository(dataAccessTransaction).GetOrganisationCodesByScopeId(CheckNull.AsteriskToString(searchString), departMentScope.Id, ParentCode);
            }
            return departMents;
        }
        public static Dictionary<long, string> GetDepartMents(string searchString)
        {
            searchString = CheckNull.AsteriskToString(searchString);
            Dictionary<long, string> departMents = new Dictionary<long, string>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    departMents = new RepositoryBase<EntityModel.PersonnelArea>(dataAccessTransaction).Get(item => item.Text.ToLower().Contains(searchString.ToLower()) && item.Text != "All")
                   .Select(x => new { Key = x.Id, Value = x.Code + " - " + x.Text })
                       .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
                else
                {
                    departMents = new RepositoryBase<EntityModel.PersonnelArea>(dataAccessTransaction).Get(item => item.Text != "All")
                   .Select(x => new { Key = x.Id, Value = x.Code + " - " + x.Text })
                       .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }

                //NOrganisationScope departMentScope = NOrganisationScope.GetOrganisationScope("Department");
                //departMents = new NOrganisationRepository(dataAccessTransaction).GetOrganisationsByScopeId(CheckNull.AsteriskToString(searchString), departMentScope.Id);
            }
            return departMents;
        }
        public static Dictionary<long, string> GetCompanies(string searchString)
        {
            Dictionary<long, string> companies = new Dictionary<long, string>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                searchString = searchString?.Replace("*", string.Empty);
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    companies = new RepositoryBase<EntityModel.Company>(dataAccessTransaction).Get(item => item.Text.ToLower().Contains(searchString.ToLower()) && item.Text != "All").Select(x => new { Key = x.Id, Value = x.Code + " - " + x.Text })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
                else
                {
                    companies = new RepositoryBase<EntityModel.Company>(dataAccessTransaction).Get(item => item.Text != "All").Select(x => new { Key = x.Id, Value = x.Code + " - " + x.Text })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }

                //NOrganisationScope companyScope = NOrganisationScope.GetOrganisationScope("Company");
                //companies = new NOrganisationRepository(dataAccessTransaction).GetOrganisationsByScopeId(CheckNull.AsteriskToString(searchString), companyScope.Id);
            }
            return companies;
        }
        public static Dictionary<long, string> GetDivisionsByDeptID(string searchString, string departmentId)
        {
            try
            {
                searchString = CheckNull.AsteriskToString(searchString);
                var deptIds = new List<int>();
                if (!string.IsNullOrWhiteSpace(departmentId))
                {
                    deptIds = departmentId.Split(',').Select(Int32.Parse).ToList();
                }
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    Dictionary<long, string> dic = new Dictionary<long, string>();
                    if (deptIds != null && deptIds.Count > 0)
                    {
                        foreach (int DepID in deptIds)
                        {
                            if (!string.IsNullOrWhiteSpace(searchString))
                            {
                                Dictionary<long, string> div = new RepositoryBase<EntityModel.PersonnelSubArea>(dataAccessTransaction).Get(item => item.Text.ToLower().Contains(searchString.ToLower()) && item.Id == DepID && item.Text != "All")
                            .Select(x => new { Key = x.Id, Value = x.Code + " - " + x.Text })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                                foreach (KeyValuePair<long, string> division in div)
                                {
                                    dic.Add(division.Key, division.Value);
                                }
                            }
                            else
                            {
                                Dictionary<long, string> div = new RepositoryBase<EntityModel.PersonnelSubArea>(dataAccessTransaction).Get(item => item.Id == DepID && item.Text != "All")
                            .Select(x => new { Key = x.Id, Value = x.Code + " - " + x.Text })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                                foreach (KeyValuePair<long, string> division in div)
                                {
                                    dic.Add(division.Key, division.Value);
                                }
                            }
                        }
                        return dic;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(searchString))
                        {
                            return new RepositoryBase<EntityModel.PersonnelSubArea>(dataAccessTransaction).Get(item => item.Text.ToLower().Contains(searchString.ToLower()) && item.Text != "All")
                        .Select(x => new { Key = x.Id, Value = x.Code + " - " + x.Text })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                        }
                        else
                        {
                            return new RepositoryBase<EntityModel.PersonnelSubArea>(dataAccessTransaction).Get(item => item.Text != "All")
                       .Select(x => new { Key = x.Id, Value = x.Code + " - " + x.Text })
                       .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        public static Dictionary<string, string> GetDisticntSectionFromUsers(string searchString)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    searchString = CheckNull.AsteriskToString(searchString);
                    if (!string.IsNullOrWhiteSpace(searchString))
                    {
                        return new RepositoryBase<EntityModel.NOrganisation>(dataAccessTransaction).Get(item => item.Name.ToLower().Contains(searchString.ToLower()))
                        .Select(x => new { Key = x.Code, Value = x.Code + " - " + x.Name })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                    }
                    else
                    {
                        return new RepositoryBase<EntityModel.NOrganisation>(dataAccessTransaction).GetAll()
                        .Select(x => new { Key = x.Code, Value = x.Code + " - " + x.Name })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        public static Dictionary<string, string> GetCostCenterCodeAndTextByCompanyID(string searchString, string companyId)
        {
            try
            {
                var companyIds = new List<int>();
                if (!string.IsNullOrWhiteSpace(companyId))
                {
                    companyIds = companyId.Split(',').Select(Int32.Parse).ToList();
                }
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    if (companyIds != null && companyIds.Count > 0)
                    {
                        foreach (int CompID in companyIds)
                        {
                            Dictionary<string, string> res = new RepositoryBase<EntityModel.NCostCenter>(dataAccessTransaction).Get(item => item.Description.ToLower().Contains(searchString.ToLower()) && item.CompanyId == CompID && item.Description != "All")
                            .Select(x => new { Key = x.Code, Value = x.Code + " - " + x.Description })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                            foreach (KeyValuePair<string, string> item in res)
                            {
                                dic.Add(item.Key, item.Value);
                            }
                        }
                        return dic;
                    }
                    else
                    {
                        var dictionary = new Dictionary<string, string>();
                        var res = new RepositoryBase<EntityModel.NCostCenter>(dataAccessTransaction).Get(item => item.Description.ToLower().Contains(searchString.ToLower()) && item.Description != "All")
                        .Select(x => new { Key = x.Code, Value = x.Code + " - " + x.Description });
                        foreach (var item in res)
                        {
                            if (dictionary.ContainsKey(item.Key))
                                continue;

                            dictionary.Add(item.Key, item.Value);
                        }
                        return dictionary;
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        public static Dictionary<long, string> GetInternalCompanies(string searchString)
        {
            Dictionary<long, string> companies = new Dictionary<long, string>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                NOrganisationScope companyScope = NOrganisationScope.GetOrganisationScope("Company");
                companies = new NOrganisationRepository(dataAccessTransaction).GetInternalCompany(CheckNull.AsteriskToString(searchString), companyScope.Id);
            }
            return companies;
        }

        public static Dictionary<long, string> GetExternalCompanies(string searchString)
        {
            Dictionary<long, string> companies = new Dictionary<long, string>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                NOrganisationScope companyScope = NOrganisationScope.GetOrganisationScope("Company");
                companies = new NOrganisationRepository(dataAccessTransaction).GetExternalCompany(CheckNull.AsteriskToString(searchString), companyScope.Id);
            }
            return companies;
        }

        public static Dictionary<long, string> GetLocations(string searchString)
        {
            Dictionary<long, string> locations = new Dictionary<long, string>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                NOrganisationScope locationScope = NOrganisationScope.GetOrganisationScope("Location");
                locations = new NOrganisationRepository(dataAccessTransaction).GetOrganisationsByScopeId(CheckNull.AsteriskToString(searchString), locationScope.Id);
            }
            return locations;
        }

        public static Dictionary<long, string> GetCostCenters(string searchString)
        {
            Dictionary<long, string> costCenters = new Dictionary<long, string>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                NOrganisationScope costCenterScope = NOrganisationScope.GetOrganisationScope("CostCenter");
                costCenters = new NOrganisationRepository(dataAccessTransaction).GetOrganisationsByScopeId(CheckNull.AsteriskToString(searchString), costCenterScope.Id);
            }
            return costCenters;
        }
        public static Dictionary<long, string> GetOrganisationScopeUsers(string searchString, string OrganisationScopeId)
        {
            Dictionary<long, string> scopeUsers = new Dictionary<long, string>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                int i;
                if ((OrganisationScopeId != "0") && int.TryParse(OrganisationScopeId, out i))
                {
                    scopeUsers = new NOrganisationRepository(dataAccessTransaction).GetOrganisationsByScopeId(CheckNull.AsteriskToString(searchString), Convert.ToUInt32(OrganisationScopeId));
                }
                else
                {
                    if (OrganisationScopeId == AttestationScopeEnum.User.ToString())
                    {
                        scopeUsers = User.GetBySearchString(CheckNull.AsteriskToString(searchString), true);
                    }
                    else if (OrganisationScopeId == AttestationScopeEnum.RO.ToString())
                    {
                        //Below is for actual V5
                        //scopeUsers = NOrganisationHead.GetOrganisationHead(CheckNull.AsteriskToString(searchString));
                        //Below is for SP
                        scopeUsers = User.GetAllActiveRO(CheckNull.AsteriskToString(searchString));
                    }
                    else if (OrganisationScopeId == AttestationScopeEnum.IACMGroup.ToString())
                    {
                        scopeUsers = Group.GetAllGroups(searchString);
                    }
                }
            }

            return scopeUsers;
        }

        public static List<NOrganisation> GetByScopeId(long scopeId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new NOrganisationRepository(dataAccessTransaction).Get(org => org.ScopeId == scopeId).ToList());
            }
        }
        public static NOrganisation GetNOrganisationByScopeId(string Code, long scopeId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new NOrganisationRepository(dataAccessTransaction).Get(org => org.ScopeId == scopeId && org.Code == Code).FirstOrDefault());
            }
        }
        public static NOrganisation GetNOrganisationIdByScopeId(long Id, long scopeId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new NOrganisationRepository(dataAccessTransaction).Get(org => org.ScopeId == scopeId && org.Id == Id).FirstOrDefault());
            }
        }
        public static List<NOrganisation> GetParentOrgs()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new NOrganisationRepository(dataAccessTransaction).Get(org => org.ParentOrgId == null && org.ScopeId == (long)NOrganisationScopeValue.Department).ToList());
            }
        }


        public static bool isExists(string companyCode, long scopeId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {

                return new RepositoryBase<EntityModel.NOrganisation>(dataAccessTransaction)
                   .Any(rl => rl.Code == companyCode && rl.ScopeId == scopeId);
            }
        }
        public static void FlushHeadInformation()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new NOrganisationRepository(dataAccessTransaction).FlushHeadInformation();
            }
        }

        public static void UpdateParentOrgId()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new NOrganisationRepository(dataAccessTransaction).UpdateParentOrgId();
            }
        }
        public static List<NOrganisation> GetAllOrganisations()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new NOrganisationRepository(dataAccessTransaction).GetAllOrganisations());
            }

        }
        public static NOrganisation GetNOrganisationByName(string name)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new NOrganisationRepository(dataAccessTransaction).Get(org => org.Name == name).FirstOrDefault());
            }
        }
        public static NOrganisation GetNOrganisationByCode(string code)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.NOrganisation>(dataAccessTransaction).Get(org => org.Code == code && org.IsActive).FirstOrDefault());

            }
        }
    }
}
