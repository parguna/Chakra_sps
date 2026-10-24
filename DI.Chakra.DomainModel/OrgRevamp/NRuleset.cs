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
    public partial class NRuleset : DomainModelBase<NRuleset, EntityModel.NRuleset>
    {
        public long RuleId { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public long TypeId { get; set; }

        public string Value { get; set; }
        public string ValueDescription { get; set; }
        public string Operator { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }


        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public NRulesetType RuleSetType { get; set; }

        public List<NRuleset> SelectedRuleSets { get; set; }

        public static List<NRulesetPresentation> GetMatchingRulesets(long UserId)
        {
            return getMatchingRulesetCodes(UserId, null);


        }

        public static List<NRulesetPresentation> GetMatchingRulesets(User user)
        {
            return getMatchingRulesetCodes(user, null);

        }
        
        public static List<NRulesetPresentation> GetMatchingRulesets(long UserId, EmployeeEventTrigger eventTrigger)
        {
            return getMatchingRulesetCodes(UserId, eventTrigger);

        }

        public static List<NRulesetPresentation> GetMatchingRulesets(User user, EmployeeEventTrigger eventTrigger)
        {
            return getMatchingRulesetCodes(user, eventTrigger);

        }

       
        public static Dictionary<long, string> GetAllRulesetstring(string searchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new NRulesetRepository(dataAccessTransaction).GetAllRulesetstring(CheckNull.AsteriskToString(searchString));
            }
        }

        public static Dictionary<long, string> GetAllRuleset(string scopeType, string scopeValue)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return (new RepositoryBase<EntityModel.NRuleset>(dataAccessTransaction).Get(a => a.RuleSetType.RulesetType == scopeType && a.Value == scopeValue)
                    .Select(x => new { Key = x.RuleId, Value = x.Code + " - " + x.RuleSetType.Description })
                    .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value));
            }
        }
        public static NRuleset GetRuleset(string rulesetCode, long ruleId, long TypeId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.NRuleset>(dataAccessTransaction).Get(a => a.Code == rulesetCode && a.RuleId == ruleId && a.TypeId == TypeId).FirstOrDefault());
            }
        }

       
        public static List<NRuleset> GetUniqueRulesets()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new NRulesetRepository(dataAccessTransaction).GetUniqueRuleSets());
            }
        }

        public static List<NRuleset> GetUserUniqueRulesets(long userId)
        {
            var matchinRuleSet = GetMatchingRulesets(userId);
            var ruleIds = matchinRuleSet.Select(a => a.RuleId).Distinct().ToList();
            List<NRuleset> rule = new List<NRuleset>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                rule = ORMapList(new NRulesetRepository(dataAccessTransaction).GetUniqueRuleSetsByIds(ruleIds));
            }

            return rule;
        }

        public static bool DoesCodeExists(string Code)
        {            
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return (new RepositoryBase<EntityModel.NRuleset>(dataAccessTransaction).Any(m => m.Code == Code));
            }
        }
        public static int DeleteRuleSetsByRuleId(long ruleId)
        {
            List<NRuleset> rulesets = NRuleset.GetAll().Where(m => m.RuleId == ruleId).ToList();
            NRuleset.DeleteList(rulesets);
            // If no exception occurs, delete has been successfull and return 1
            return 1;
        }
        public static List<NRuleset> GetByRuleId(long ruleId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.NRuleset>(dataAccessTransaction).Get(a => a.RuleId == ruleId).ToList());
            }
        }

        public static List<NRuleset> GetByRuleIds(List<long> ruleIds)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.NRuleset>(dataAccessTransaction).Get(a => ruleIds.Contains(a.RuleId)).ToList());
            }            
        }

        public static NRuleset GetRuleCode(string ruleset)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.NRuleset>(dataAccessTransaction).Get(ep => ep.Code == ruleset).FirstOrDefault());
            }
        }
        public static NRuleset GetRuleSet(string value, long TypeId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.NRuleset>(dataAccessTransaction).Get(ep => ep.Value == value && ep.TypeId == TypeId).FirstOrDefault());
            }
        }
        public static NRuleset GetRuleSet(string ruleset)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.NRuleset>(dataAccessTransaction).Get(ep => ep.Code == ruleset).FirstOrDefault());
            }
        }
        public static bool IsValid(long ruleId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return (new RepositoryBase<EntityModel.NRuleset>(dataAccessTransaction).Any(r => r.RuleId == ruleId));
            }
        }

        //public static NRuleset GetByRuleId(long ruleId)
        public static Dictionary<string, string> GetRulesByRuleID(long ruleId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new NRulesetRepository(dataAccessTransaction).getRulesByRuleID(ruleId);

            }
        }

        public static NRuleset GetSingleRulesetByRuleId(long ruleId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.NRuleset>(dataAccessTransaction).Get(a => a.RuleId == ruleId).FirstOrDefault());
            }
        }

        public static List<NRuleset> GetRulesetByRuleId(long ruleId)
        {
           
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new NRulesetRepository(dataAccessTransaction).getNRulesByRuleID(ruleId)).ToList();
            }
        }

        public static NRuleset GetRuleSetByCodeAndRuleId(long ruleId,string Code)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.NRuleset>(dataAccessTransaction).Get(a => a.RuleId == ruleId && a.Code == Code).FirstOrDefault());
            }
        }

        public static void DeleteRuleSets(long ruleId)
        {
            var rulesets = NRuleset.GetAll().Where(a => a.RuleId == ruleId).ToList();
            if(rulesets.Count > 0)
               NRuleset.DeleteList(rulesets);

        }

        public static long GetMaxRuleId()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new NRulesetRepository(dataAccessTransaction).GetMaxRuleId();
            }
        }

        public override long Save(string currentUser)
        {
            bool isNew = this.IsNew;
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                //  base.Save(transaction);

                if (!isNew && this.SelectedRuleSets != null)
                {

                    List<NRuleset> dbRuleSets = ORMapList(new RepositoryBase<EntityModel.NRuleset>(transaction)
                        .Get(g => g.RuleId == this.Id).ToList());

                    foreach (NRuleset ruleSet in dbRuleSets)
                    {
                        if (!this.SelectedRuleSets.Any(m => m.RuleId == ruleSet.RuleId && m.Code == ruleSet.Code 
                        && m.TypeId == ruleSet.TypeId && m.Value == ruleSet.Value))
                            NRuleset.Delete(transaction, ruleSet.Id);
                    }

                    this.SelectedRuleSets.ForEach(rule => { rule.RuleId = this.Id; rule.Description = this.Description; rule.Save(transaction); });

                }
                else
                    this.SelectedRuleSets.ForEach(rule => rule.Save(transaction));
                //DomainModelBase<NRuleset, EntityModel.NRuleset>.SaveList(transaction, this.SelectedRuleSets);

                transaction.Commit();
            }
            return 0;
        }

        static List<NRulesetPresentation> getMatchingRulesetCodes(long UserId, EmployeeEventTrigger eventTrigger)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var user = User.GetById(UserId);

                return getMatchingRulesetCodes(user, eventTrigger);
            }
        }

        static List<NRulesetPresentation> getMatchingRulesetCodes(User user, EmployeeEventTrigger eventTrigger)
        {
            try
            {
                List<NRulesetPresentation> validRuleset = new List<NRulesetPresentation>();
                List<string> inValidRuleset = new List<string>();

                //if (user != null && user.NOrganisationId.HasValue)
                {                    
                    List<NOrganisationPresentation> orgDetails = new List<NOrganisationPresentation>();
                    List<NDesignation> userDesignation = new List<NDesignation>();
                    if (user != null && user.NBusinessRoleId.HasValue)
                    {
                        userDesignation = NDesignation.GetAll().Where(a => a.Id == user.NBusinessRoleId).ToList();
                    }

                    if (!string.IsNullOrWhiteSpace(user.OrganisationCode))
                    {
                        orgDetails = NOrganisationPresentation.GetOrgChart(user.OrganisationCode);
                        orgDetails.Reverse();
                    }
                   
                    var getAllRuleSetType = NRulesetType.GetAll();
                    var getAllRuleSet = NRuleset.GetAll();

                    foreach (var rulesetType in getAllRuleSetType)
                    {
                        switch (rulesetType.RulesetType)
                        {
                            case "Department":
                                if (orgDetails != null)
                                    foreach (var orgDetail in orgDetails)
                                    {
                                        var baseLevelOrgRulesets = getAllRuleSet.Where(a => a.TypeId == rulesetType.Id && (a.Value == orgDetail.Code || a.Value == "*") && a.Operator == "Equals").ToList();
                                        if (baseLevelOrgRulesets != null && baseLevelOrgRulesets.Count > 0)
                                        {
                                            getMatchingRulesetCodesBasedOnRulesetType(
                                                validRuleset, inValidRuleset,
                                                baseLevelOrgRulesets,
                                                user, orgDetails, userDesignation,
                                                eventTrigger
                                                );
                                        }
                                    }
                                break;
                            case "Organisation":
                                if (orgDetails != null)
                                    foreach (var orgDetail in orgDetails)
                                    {
                                        var baseLevelOrgRulesets = getAllRuleSet.Where(a => a.TypeId == rulesetType.Id && (a.Value == orgDetail.Code || a.Value == "*") && a.Operator == "Equals").ToList();
                                        if (baseLevelOrgRulesets != null && baseLevelOrgRulesets.Count > 0)
                                        {
                                            getMatchingRulesetCodesBasedOnRulesetType(
                                                validRuleset, inValidRuleset,
                                                baseLevelOrgRulesets,
                                                user, orgDetails, userDesignation,
                                                eventTrigger
                                                );
                                        }
                                    }
                                break;
                            case "Designation":
                                if (userDesignation != null)
                                    foreach (var designation in userDesignation)
                                    {
                                        var baseLevelDesignationRulesets = getAllRuleSet.Where(a => a.TypeId == rulesetType.Id && (a.Value == designation.Code || a.Value == "*") && a.Operator == "Equals").ToList();
                                        if (baseLevelDesignationRulesets != null && baseLevelDesignationRulesets.Count > 0)
                                        {
                                            getMatchingRulesetCodesBasedOnRulesetType(
                                                validRuleset, inValidRuleset,
                                                baseLevelDesignationRulesets,
                                                user, orgDetails, userDesignation,
                                                eventTrigger
                                                );
                                        }
                                    }
                                break;
                           
                    
                          
                            
                            case "EmployeeType":
                                var baseLevelEmployeeTypeRulesets = getAllRuleSet.Where(a => a.TypeId == rulesetType.Id && (a.Value == user.EmployeeType.ToString() || a.Value == "*") && a.Operator == "Equals").ToList();
                                if (baseLevelEmployeeTypeRulesets != null && baseLevelEmployeeTypeRulesets.Count > 0)
                                {
                                    getMatchingRulesetCodesBasedOnRulesetType(
                                        validRuleset, inValidRuleset,
                                        baseLevelEmployeeTypeRulesets,
                                        user, orgDetails, userDesignation, 
                                                eventTrigger
                                        );
                                }
                                break;
                            case "EventTrigger":
                                if (eventTrigger != null)
                                    {
                                    var baseLevelEventRulesets = getAllRuleSet.Where(a => a.TypeId == rulesetType.Id && (a.Value == eventTrigger.EventType.ToString() || a.Value == "*") && a.Operator == "Equals").ToList();
                                    if (baseLevelEventRulesets != null && baseLevelEventRulesets.Count > 0)
                                    {
                                            getMatchingRulesetCodesBasedOnRulesetType(
                                                validRuleset, inValidRuleset,
                                            baseLevelEventRulesets,
                                                user, orgDetails, userDesignation, 
                                                eventTrigger
                                                );
                                        }
                                    }
                                break;
                           
                            default:
                                break;
                        }
                    }
                }
                return validRuleset;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        static void getMatchingRulesetCodesBasedOnRulesetType(List<NRulesetPresentation> validRuleset, List<string> inValidRuleset,
           List<NRuleset> baseLevelOrgRulesets,
           User user, 
           List<NOrganisationPresentation> orgDetails, 
           List<NDesignation> userDesignation, 
           EmployeeEventTrigger eventTrigger)
        {
            foreach (var baseLevelOrgRuleset in baseLevelOrgRulesets)
            {
                if (inValidRuleset.Contains(baseLevelOrgRuleset.Code)) continue;

                List<NRuleset> rulesets = new List<NRuleset>();
                using (var dataAccessTransaction = new DataAccessTransaction())
                {
                    rulesets = ORMapList(new RepositoryBase<EntityModel.NRuleset>(dataAccessTransaction).Get(a => a.Code == baseLevelOrgRuleset.Code, a => a.RuleSetType).ToList());
                }

                if (rulesets != null)
                {
                    foreach (var ruleset in rulesets)
                    {
                        if (inValidRuleset.Contains(ruleset.Code)) continue;

                        switch (ruleset.RuleSetType.RulesetType)
                        {
                            case "Organisation":
                                if (orgDetails.Any(a => a.Code == ruleset.Value) || orgDetails.Any(b => b.OrganisationRelationship.Any(c => c.Value == ruleset.Value) || ruleset.Value == "*"))
                                {
                                    var org = orgDetails.Where(a => a.Code == ruleset.Value).ToList();
                                    var rank = (org == null || org.Count == 0 ? 1 : org.FirstOrDefault().ScopeId) * ruleset.RuleSetType.Score;
                                    if (ruleset.Value == "*")
                                        rank = 0;
                                    if (!validRuleset.Any(a => a.Code == ruleset.Code))
                                        validRuleset.Add(new NRulesetPresentation
                                        {
                                            Code = ruleset.Code,
                                            RuleId = ruleset.RuleId,
                                            OrgRank = rank * ruleset.RuleSetType.Score,
                                        });
                                    else
                                    {
                                        var existingRuleset = validRuleset.Where(a => a.Code == ruleset.Code).First();
                                        existingRuleset.OrgRank = Math.Max(rank, validRuleset.Where(a => a.Code == ruleset.Code).First().OrgRank);
                                    }
                                }
                                else
                                {
                                    var itemToRemove = validRuleset.Where(a => a.Code == baseLevelOrgRuleset.Code);
                                    if (itemToRemove.Any())
                                        validRuleset.Remove(itemToRemove.First());
                                    if (!inValidRuleset.Contains(ruleset.Code))
                                        inValidRuleset.Add(ruleset.Code);
                                }
                                break;
                            case "Department":
                                if (orgDetails.Any(a => a.Code == ruleset.Value) || orgDetails.Any(b => b.OrganisationRelationship.Any(c => c.Value == ruleset.Value) || ruleset.Value == "*"))
                                {
                                    var org = orgDetails.Where(a => a.Code == ruleset.Value).ToList();
                                    var rank = (org == null || org.Count == 0 ? 1 : org.FirstOrDefault().ScopeId) * ruleset.RuleSetType.Score;
                                    if (ruleset.Value == "*")
                                        rank = 0;
                                    if (!validRuleset.Any(a => a.Code == ruleset.Code))
                                        validRuleset.Add(new NRulesetPresentation
                                        {
                                            Code = ruleset.Code,
                                            RuleId = ruleset.RuleId,
                                            OrgRank = rank * ruleset.RuleSetType.Score,
                                        });
                                    else
                                    {
                                        var existingRuleset = validRuleset.Where(a => a.Code == ruleset.Code).First();
                                        existingRuleset.OrgRank = Math.Max(rank, validRuleset.Where(a => a.Code == ruleset.Code).First().OrgRank);
                                    }
                                }
                                else
                                {
                                    var itemToRemove = validRuleset.Where(a => a.Code == baseLevelOrgRuleset.Code);
                                    if (itemToRemove.Any())
                                        validRuleset.Remove(itemToRemove.First());
                                    if (!inValidRuleset.Contains(ruleset.Code))
                                        inValidRuleset.Add(ruleset.Code);
                                }
                                break;
                            case "Designation":
                                if (userDesignation.Any(a => a.Code == ruleset.Value) || ruleset.Value == "*")
                                {
                                    var rank = 1;
                                    if (ruleset.Value == "*")
                                        rank = 0;
                                    if (!validRuleset.Any(a => a.Code == ruleset.Code))
                                        validRuleset.Add(new NRulesetPresentation
                                        {
                                            Code = ruleset.Code,
                                            RuleId = ruleset.RuleId,
                                            DesignationRank = rank * ruleset.RuleSetType.Score,
                                        });
                                    else
                                    {
                                        var existingRuleset = validRuleset.Where(a => a.Code == ruleset.Code).First();
                                        existingRuleset.DesignationRank = Math.Max(rank * ruleset.RuleSetType.Score, existingRuleset.DesignationRank);
                                    }
                                }
                                else
                                {
                                    var itemToRemove = validRuleset.Where(a => a.Code == baseLevelOrgRuleset.Code);
                                    if (itemToRemove.Any())
                                        validRuleset.Remove(itemToRemove.First());
                                    if (!inValidRuleset.Contains(ruleset.Code))
                                        inValidRuleset.Add(ruleset.Code);
                                }
                                break;
                           
                            case "EmployeeType":
                                if (user.EmployeeType.ToString() == ruleset.Value || ruleset.Value == "*")
                                {
                                    var rank = 1;
                                    if (ruleset.Value == "*")
                                        rank = 0;
                                    if (!validRuleset.Any(a => a.Code == ruleset.Code))
                                        validRuleset.Add(new NRulesetPresentation
                                        {
                                            Code = ruleset.Code,
                                            RuleId = ruleset.RuleId,
                                            EmployeeTypeRank = rank * ruleset.RuleSetType.Score,
                                        });
                                    else
                                    {
                                        var existingRuleset = validRuleset.Where(a => a.Code == ruleset.Code).First();
                                        existingRuleset.EmployeeTypeRank = Math.Max(rank * ruleset.RuleSetType.Score, existingRuleset.EmployeeTypeRank);
                                    }
                                }
                                else
                                {
                                    var itemToRemove = validRuleset.Where(a => a.Code == baseLevelOrgRuleset.Code);
                                    if (itemToRemove.Any())
                                        validRuleset.Remove(itemToRemove.First());
                                    if (!inValidRuleset.Contains(ruleset.Code))
                                        inValidRuleset.Add(ruleset.Code);
                                }
                                break;
                            case "EventTrigger":
                                if ((eventTrigger != null && eventTrigger.EventType.ToString() == ruleset.Value) || ruleset.Value == "*")
                                {
                                    var rank = 1;
                                    if (ruleset.Value == "*")
                                        rank = 0;
                                    if (!validRuleset.Any(a => a.Code == ruleset.Code))
                                        validRuleset.Add(new NRulesetPresentation
                                        {
                                            Code = ruleset.Code,
                                            RuleId = ruleset.RuleId,
                                            EventTriggerRank = rank * ruleset.RuleSetType.Score,
                                        });
                                    else
                                    {
                                        var existingRuleset = validRuleset.Where(a => a.Code == ruleset.Code).First();
                                        existingRuleset.EventTriggerRank = Math.Max(rank * ruleset.RuleSetType.Score, existingRuleset.EventTriggerRank);
                                    }
                                }
                                else
                                {
                                    var itemToRemove = validRuleset.Where(a => a.Code == baseLevelOrgRuleset.Code);
                                    if (itemToRemove.Any())
                                        validRuleset.Remove(itemToRemove.First());
                                    if (!inValidRuleset.Contains(ruleset.Code))
                                        inValidRuleset.Add(ruleset.Code);
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}
