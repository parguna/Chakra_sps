using AutoMapper;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    [NotMapped]
    public class PresentationNRulesetForApplication : NRulesetForApplication
    {
        public string RuleCode { get; set; }

        public string EndpointSystemName { get; set; }


        public static List<PresentationNRulesetForApplication> GetByRuleset(long rulesetId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
              return ORMapList(new PresentationNRulesetForApplicationRepository(dataAccessTransaction).GetByRuleEndpointandRuleset(rulesetId));
            }
        }

        public static PresentationNRulesetForApplication ORMap(EntityModel.Presentation.PresentationNRulesetForApplication presentationNRulesetForApplicationEntityModel)
        {
            if (presentationNRulesetForApplicationEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationNRulesetForApplication, PresentationNRulesetForApplication>(presentationNRulesetForApplicationEntityModel);
        }

        public static List<PresentationNRulesetForApplication> ORMapList(IList<EntityModel.Presentation.PresentationNRulesetForApplication> presentationNRulesetForProvisioningEntityModel)
        {
            return presentationNRulesetForProvisioningEntityModel.Select(ORMap).ToList();
        }

    }
}
