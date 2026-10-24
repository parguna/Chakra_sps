using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Repository
{
    public class NRulesetRepository : RepositoryBase<NRuleset>
    {
        public NRulesetRepository(DataAccessTransaction transaction) : base(transaction) { }

        public long GetMaxRuleId()
        {
            long ruleId = 0;
            NRuleset ruleset = CurrentContext.NRulesets.OrderByDescending(i => i.RuleId).FirstOrDefault();
            if (ruleset != null)
            {
                ruleId = ruleset.RuleId;
            }
            return ruleId + 1;
        }
        public List<NRuleset> GetUniqueRuleSets()
        {
            var distinctIds = CurrentContext.NRulesets.Select(m => m.RuleId).Distinct().ToList();
            var norg = CurrentContext.NOrganisations.Where(a => a.IsActive).ToList();
            var departments = CurrentContext.NDesignations.Where(a => a.IsActive).ToList();
            var cc = CurrentContext.NCostCenters.ToList();
            var ruleSets = new List<NRuleset>();

            foreach (var id in distinctIds)
            {
                var ruleset = CurrentContext.NRulesets.Where(m => m.RuleId == id).FirstOrDefault();
                string value = "";
                string delimiter = "";
                List<NRuleset> sets = CurrentContext.NRulesets.Where(m => m.RuleId == id).ToList();
                foreach (NRuleset set in sets)
                {
                    var type = (from rType in CurrentContext.NRulesetTypes where rType.Id == set.TypeId select rType.RulesetType).FirstOrDefault().ToString();
                    switch (type)
                    {
                        case "Organisation":
                            {
                                string info = "";
                                var norgValue = norg.Where(a => a.Code == set.Value).ToList();
                                if (norgValue != null && norgValue.Count > 0)
                                    info = string.Format("({0})", norgValue.First().Description);
                                value = value + delimiter + "OrganisationUnit = " + set.Value + info;
                                break;
                            }
                        case "Designation":
                            {
                                string info = "";
                                var depart = departments.Where(a => a.Code == set.Value).ToList();
                                if (depart != null && depart.Count > 0)
                                    info = string.Format("({0})", depart.First().Description);
                                value = value + delimiter + "Designation = " + set.Value + info;
                                break;
                            }
                        case "EmployeeType":
                            {
                                value = value + delimiter + "EmployeeType = " + set.Value;
                                break;
                            }
                        case "EventTrigger":
                            {
                                value = value + delimiter + "EventTrigger = " + set.Value;
                                break;
                            }
                      
                        case "Department":
                            {
                                string info = "";
                                var norgValue = norg.Where(a => a.Code == set.Value).ToList();
                                if (norgValue != null && norgValue.Count > 0)
                                    info = string.Format("({0})", norgValue.First().Description);
                                value = value + delimiter + "Department = " + set.Value + info;
                                break;
                            }
                        case "Company":
                            {
                                string info = "";
                                var norgValue = norg.Where(a => a.Code == set.Value).ToList();
                                if (norgValue != null && norgValue.Count > 0)
                                    info = string.Format("({0})", norgValue.First().Description);
                                value = value + delimiter + "Company = " + set.Value + info;
                                break;
                            }
                        case "BusinessUnit":
                            {
                                string info = "";
                                var norgValue = norg.Where(a => a.Code == set.Value).ToList();
                                if (norgValue != null && norgValue.Count > 0)
                                    info = string.Format("({0})", norgValue.First().Description);
                                value = value + delimiter + "BusinessUnit = " + set.Value + info;
                                break;
                            }
                        case "CostCenter":
                            {
                                string info = "";
                                var c = cc.Where(a => a.Code == set.Value).ToList();
                                if (c != null && c.Count > 0)
                                    info = string.Format("({0})", c.First().Description);
                                value = value + delimiter + "CostCenter = " + set.Value + info;
                                break;
                            }
                        case "Location":
                            {
                                string info = "";
                                var norgValue = norg.Where(a => a.Code == set.Value).ToList();
                                if (norgValue != null && norgValue.Count > 0)
                                    info = string.Format("({0})", norgValue.First().Description);
                                value = value + delimiter + "Location = " + set.Value + info;
                                break;
                            }
                        default:
                            break;
                    }
                    delimiter = ",";
                }
                ruleset.Value = value;
                ruleSets.Add(ruleset);
            }
            return ruleSets;
        }

        public List<NRuleset> GetUniqueRuleSetsByIds(List<long> distinctIds)
        {
            var norg = CurrentContext.NOrganisations.Where(a => a.IsActive).ToList();
            var departments = CurrentContext.NDesignations.Where(a => a.IsActive).ToList();
            var cc = CurrentContext.NCostCenters.ToList();
            var ruleSets = new List<NRuleset>();

            foreach (var id in distinctIds)
            {
                var ruleset = CurrentContext.NRulesets.Where(m => m.RuleId == id).FirstOrDefault();
                string value = "";
                string delimiter = "";
                List<NRuleset> sets = CurrentContext.NRulesets.Where(m => m.RuleId == id).ToList();
                foreach (NRuleset set in sets)
                {
                    var type = (from rType in CurrentContext.NRulesetTypes where rType.Id == set.TypeId select rType.RulesetType).FirstOrDefault().ToString();
                    switch (type)
                    {
                        case "Organisation":
                            {
                                string info = "";
                                var norgValue = norg.Where(a => a.Code == set.Value).ToList();
                                if (norgValue != null && norgValue.Count > 0)
                                    info = string.Format("({0})", norgValue.First().Description);
                                value = value + delimiter + "OrganisationUnit = " + set.Value + info;
                                break;
                            }
                        case "Designation":
                            {
                                string info = "";
                                var depart = departments.Where(a => a.Code == set.Value).ToList();
                                if (depart != null && depart.Count > 0)
                                    info = string.Format("({0})", depart.First().Description);
                                value = value + delimiter + "Designation = " + set.Value + info;
                                break;
                            }
                        case "EmployeeType":
                            {
                                value = value + delimiter + "EmployeeType = " + set.Value;
                                break;
                            }
                        case "EventTrigger":
                            {
                                value = value + delimiter + "EventTrigger = " + set.Value;
                                break;
                            }
                       
                        case "Department":
                            {
                                string info = "";
                                var norgValue = norg.Where(a => a.Code == set.Value).ToList();
                                if (norgValue != null && norgValue.Count > 0)
                                    info = string.Format("({0})", norgValue.First().Description);
                                value = value + delimiter + "Department = " + set.Value + info;
                                break;
                            }
                        case "Company":
                            {
                                string info = "";
                                var norgValue = norg.Where(a => a.Code == set.Value).ToList();
                                if (norgValue != null && norgValue.Count > 0)
                                    info = string.Format("({0})", norgValue.First().Description);
                                value = value + delimiter + "Company = " + set.Value + info;
                                break;
                            }
                        case "BusinessUnit":
                            {
                                string info = "";
                                var norgValue = norg.Where(a => a.Code == set.Value).ToList();
                                if (norgValue != null && norgValue.Count > 0)
                                    info = string.Format("({0})", norgValue.First().Description);
                                value = value + delimiter + "BusinessUnit = " + set.Value + info;
                                break;
                            }
                        case "CostCenter":
                            {
                                string info = "";
                                var c = cc.Where(a => a.Code == set.Value).ToList();
                                if (c != null && c.Count > 0)
                                    info = string.Format("({0})", c.First().Description);
                                value = value + delimiter + "CostCenter = " + set.Value + info;
                                break;
                            }
                        case "Location":
                            {
                                string info = "";
                                var norgValue = norg.Where(a => a.Code == set.Value).ToList();
                                if (norgValue != null && norgValue.Count > 0)
                                    info = string.Format("({0})", norgValue.First().Description);
                                value = value + delimiter + "Location = " + set.Value + info;
                                break;
                            }
                        default:
                            break;
                    }
                    delimiter = ",";
                }
                ruleset.Value = value;
                ruleSets.Add(ruleset);
            }
            return ruleSets;
        }
        public Dictionary<string, string> getRulesByRuleID(long ruleId)
        {
            Dictionary<string, string> dicRules = new Dictionary<string, string>();

            List<NRuleset> rules = (from rs in CurrentContext.NRulesets where rs.RuleId == ruleId select rs).ToList();
            var norg = CurrentContext.NOrganisations.Where(a => a.IsActive).ToList();
            var departments = CurrentContext.NDesignations.Where(a => a.IsActive).ToList();
            var cc = CurrentContext.NCostCenters.ToList();

            foreach (NRuleset r in rules)
            {
                var type = (from rType in CurrentContext.NRulesetTypes where rType.Id == r.TypeId select rType.RulesetType).FirstOrDefault().ToString();
                switch (type)
                {
                    case "Organisation":
                        {
                            var Organisations = norg.Where(c => c.Code == r.Value).FirstOrDefault();
                            string strOrganisation = "";
                            if (Organisations != null)
                                strOrganisation = string.Format("{0}({1})", Organisations.Code, Organisations.Description);
                            dicRules.Add(type, strOrganisation);
                            break;
                        }
                    case "Designation":
                        {
                            var Designations = departments.Where(c => c.Code == r.Value).FirstOrDefault();
                            string strDesignation = "";
                            if (Designations != null)
                                strDesignation = string.Format("{0}({1})", Designations.Code, Designations.Description);
                            dicRules.Add(type, strDesignation);
                            break;
                        }
                    case "EmployeeType":
                        {
                            dicRules.Add(type, r.Value.ToString());
                            break;
                        }
                    case "EventTrigger":
                        {
                            dicRules.Add(type, Enum.Parse(typeof(EmployeeEvents), r.Value).ToString());
                            break;
                        }
                   
                    case "Department":
                        {
                            var Organisations = norg.Where(c => c.Code == r.Value).FirstOrDefault();
                            string strOrganisation = "";
                            if (Organisations != null)
                                strOrganisation = string.Format("{0}({1})", Organisations.Code, Organisations.Description);
                            dicRules.Add(type, strOrganisation);
                            break;
                        }
                    case "Company":
                        {
                            var Organisations = norg.Where(c => c.Code == r.Value).FirstOrDefault();
                            string strOrganisation = "";
                            if (Organisations != null)
                                strOrganisation = string.Format("{0}({1})", Organisations.Code, Organisations.Description);
                            dicRules.Add(type, strOrganisation);
                            break;
                        }
                    case "BusinessUnit":
                        {
                            var Organisations = norg.Where(c => c.Code == r.Value).FirstOrDefault();
                            string strOrganisation = "";
                            if (Organisations != null)
                                strOrganisation = string.Format("{0}({1})", Organisations.Code, Organisations.Description);
                            dicRules.Add(type, strOrganisation);
                            break;
                        }
                    case "CostCenter":
                        {
                            var Organisations = cc.Where(c => c.Code == r.Value).FirstOrDefault();
                            string strOrganisation = "";
                            if (Organisations != null)
                                strOrganisation = string.Format("{0}({1})", Organisations.Code, Organisations.Description);
                            dicRules.Add(type, strOrganisation);
                            break;
                        }
                    case "Location":
                        {
                            var Organisations = norg.Where(c => c.Code == r.Value).FirstOrDefault();
                            string strOrganisation = "";
                            if (Organisations != null)
                                strOrganisation = string.Format("{0}({1})", Organisations.Code, Organisations.Description);
                            dicRules.Add(type, strOrganisation);
                            break;
                        }
                    default:
                        break;
                }


            }
            return dicRules;
        }

        public List<NRuleset> getNRulesByRuleID(long ruleId)
        {
            List<NRuleset> rules = (from rs in CurrentContext.NRulesets.Include("RuleSetType") where rs.RuleId == ruleId select rs).ToList();
            var norg = CurrentContext.NOrganisations.Where(a => a.IsActive).ToList();
            var departments = CurrentContext.NDesignations.Where(a => a.IsActive).ToList();
            var cc = CurrentContext.NCostCenters.ToList();

            foreach (NRuleset r in rules)
            {
                if (!string.IsNullOrEmpty(r.Value) && r.Value == "*")
                {
                    r.ValueDescription = r.Value;
                    continue;
                }

                var type = (from rType in CurrentContext.NRulesetTypes where rType.Id == r.TypeId select rType.RulesetType).FirstOrDefault().ToString();
                switch (type)
                {
                    case "Organisation":
                        {
                            var Organisations = norg.Where(c => c.Code == r.Value).FirstOrDefault();
                            string strOrganisation = "";
                            if (Organisations != null)
                                strOrganisation = string.Format("{0}({1})", Organisations.Code, Organisations.Description);
                            r.ValueDescription = strOrganisation;
                            break;
                        }
                    case "Designation":
                        {
                            var Designations = departments.Where(c => c.Code == r.Value).FirstOrDefault();
                            string strDesignation = "";
                            if (Designations != null)
                                strDesignation = string.Format("{0}({1})", Designations.Code, Designations.Description);
                            r.ValueDescription = strDesignation;
                            break;
                        }
                    case "EmployeeType":
                        {
                            r.ValueDescription = r.Value.ToString();
                            break;
                        }
                    case "EventTrigger":
                        {
                            r.ValueDescription = (r.Value).ToString();
                            break;
                        }
                   
                    case "Department":
                        {
                            var Organisations = norg.Where(c => c.Code == r.Value).FirstOrDefault();
                            string strOrganisation = "";
                            if (Organisations != null)
                                strOrganisation = string.Format("{0}({1})", Organisations.Code, Organisations.Description);
                            r.ValueDescription = strOrganisation;
                            break;
                        }
                    case "Company":
                        {
                            var Organisations = norg.Where(c => c.Code == r.Value).FirstOrDefault();
                            string strOrganisation = "";
                            if (Organisations != null)
                                strOrganisation = string.Format("{0}({1})", Organisations.Code, Organisations.Description);
                            r.ValueDescription = strOrganisation;
                            break;
                        }
                    case "BusinessUnit":
                        {
                            var Organisations = norg.Where(c => c.Code == r.Value).FirstOrDefault();
                            string strOrganisation = "";
                            if (Organisations != null)
                                strOrganisation = string.Format("{0}({1})", Organisations.Code, Organisations.Description);
                            r.ValueDescription = strOrganisation;
                            break;
                        }
                    case "CostCenter":
                        {
                            var Organisations = cc.Where(c => c.Code == r.Value).FirstOrDefault();
                            string strOrganisation = "";
                            if (Organisations != null)
                                strOrganisation = string.Format("{0}({1})", Organisations.Code, Organisations.Description);
                            r.ValueDescription = strOrganisation;
                            break;
                        }
                    case "Location":
                        {
                            var Organisations = norg.Where(c => c.Code == r.Value).FirstOrDefault();
                            string strOrganisation = "";
                            if (Organisations != null)
                                strOrganisation = string.Format("{0}({1})", Organisations.Code, Organisations.Description);
                            r.ValueDescription = strOrganisation;
                            break;
                        }
                    default:
                        break;
                }


            }
            return rules;
        }

        public Dictionary<long, string> GetAllRulesetstring(string searchString)
        {
            return (from rule in CurrentContext.NRulesets
                    where rule.Code.ToString().Contains(searchString)
                    select new { rules = rule.RuleId, Value = rule.Code }).Distinct()
                         .ToDictionary(c => c.rules, c => c.Value);
        }
    }
}
