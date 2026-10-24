
    using DI.Chakra.EntityModel.Presentation;
    using DI.Chakra.Infrastructure.Enums;
    using DI.Chakra.Transaction;
    using System.Collections.Generic;
    using System.Linq;

namespace DI.Chakra.Repository
{
    public class PresentationNRulesetForApplicationRepository : RepositoryBase<PresentationNRulesetForApplication>
    {
        public PresentationNRulesetForApplicationRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<PresentationNRulesetForApplication> GetByRuleEndpointandRuleset(long rulesetId)
        {
            List<PresentationNRulesetForApplication> lstRuleSetforApp = new List<PresentationNRulesetForApplication>();
          
                lstRuleSetforApp = (from rp in CurrentContext.NRulesetForApplications
                                    join r in CurrentContext.NRulesets on rp.RuleId equals r.RuleId
                                    join ep in CurrentContext.EndPoint on rp.EndpointSystemId equals ep.Id
                                    select new PresentationNRulesetForApplication
                                    {
                                        Id = rp.Id,
                                        RuleCode = r.Code,
                                        EndpointSystemName = ep.DisplayName,
                                        RuleId = rp.RuleId,
                                        EndpointSystemId = rp.EndpointSystemId,
                                        AccountType = rp.AccountType,
                                        CreatedBy = rp.CreatedBy,
                                        CreatedDate = rp.CreatedDate
                                    }).Distinct().ToList();
          
            if(rulesetId > 0 )
            {
                lstRuleSetforApp = lstRuleSetforApp.Where(x => x.RuleId == rulesetId).ToList();
            }
            return lstRuleSetforApp;
        }
    }

}

       
