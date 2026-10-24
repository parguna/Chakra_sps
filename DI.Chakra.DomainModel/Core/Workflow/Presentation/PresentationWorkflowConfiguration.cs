using AutoMapper;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationWorkflowConfiguration : WorkflowConfiguration
    {
        public long WorkflowConfigurationId { get; set; }

        public string RequestType { get; set; }

        public string ApplicationType { get; set; }

        public new static List<PresentationWorkflowConfiguration> GetAll() 
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new WorkflowConfigurationRepository(dataAccessTransaction).GetAll());
            }
        }

        public static PresentationWorkflowConfiguration ORMap(EntityModel.Presentation.PresentationWorkflowConfiguration presentationWorkflowConfigurationEntityModel)
        {
            if (presentationWorkflowConfigurationEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationWorkflowConfiguration, PresentationWorkflowConfiguration>(presentationWorkflowConfigurationEntityModel);
        }

        public static List<PresentationWorkflowConfiguration> ORMapList(IList<EntityModel.Presentation.PresentationWorkflowConfiguration> presentationWorkflowConfigurationEntityModel)
        {
            return presentationWorkflowConfigurationEntityModel.Select(ORMap).ToList();
        }
    }
}
