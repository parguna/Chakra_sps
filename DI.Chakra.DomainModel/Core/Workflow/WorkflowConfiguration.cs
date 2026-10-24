using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainModel
{
    public class WorkflowConfiguration : DomainModelBase<WorkflowConfiguration, EntityModel.WorkflowConfiguration>
    {
        public string WorkflowName { get; set; }

        public string Description { get; set; }

        public long RequestTypeId { get; set; }

        public long? EndpointSystemId { get; set; }

        public string ApplicableUserTypes { get; set; }

        public string ApplicableAccountTypes { get; set; }

        public string ApplicableActionTypes { get; set; }

        public bool IsActive { get; set; }

        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public List<WorkflowConfigurationDetail> WorkflowConfigurationDetails { get; set; }

        public EndPoint Endpoint { get; set; }

        public RequestTypes RequestTypes { get; set; }
        public string EntitlementType { get; set; }

        public static WorkflowConfiguration GetByIdIncludeEndpoint(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.WorkflowConfiguration>(tranaction)
                       .Get(workflow => workflow.Id == id, workflow => workflow.Endpoint).FirstOrDefault());
            }
        }

        public static WorkflowConfiguration GetWorkflowConfigurationDetailById(long id)
        {
            //using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                WorkflowConfiguration wfConfig = GetByIdIncludeEndpoint(id);
                if (wfConfig != null)
                {
                    List<WorkflowConfigurationDetail> wcd = WorkflowConfigurationDetail.GetByWorkflowConfigurationId(id);
                    wfConfig.WorkflowConfigurationDetails = wcd;
                }
                return wfConfig;
            }
        }

        public static List<WorkflowConfiguration> GetByWorkflowName(string workflowName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.WorkflowConfiguration>(dataAccessTransaction)
                       .Get(work => work.WorkflowName == workflowName).ToList());
            }
        }

        public static List<WorkflowConfiguration> GetByRequestTypeId(byte requestTypeId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.WorkflowConfiguration>(dataAccessTransaction)
                       .Get(work => work.RequestTypeId == (byte)requestTypeId && work.IsActive).OrderBy(wf => wf.WorkflowName).ToList());
            }
        }

        public static WorkflowConfiguration GetByRequestType(byte requestTypeId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.WorkflowConfiguration>(dataAccessTransaction)
                       .Get(work => work.RequestTypeId == (byte)requestTypeId && work.IsActive).FirstOrDefault());
            }
        }

        public static WorkflowConfiguration GetByEndpointSystemId(long requestTypeId, long endpointSystemId, string userType, string accountType, string actionType, DateTime requestDate, string entitlementType=null)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new WorkflowConfigurationRepository(dataAccessTransaction)
                       .GetWorkFlowConfiguration(requestTypeId, endpointSystemId, userType.PadLeft(3, '0'), accountType.PadLeft(3, '0'), actionType.PadLeft(3, '0'), requestDate.Date, entitlementType));
            }
        }
        
        public static Dictionary<long, string> GetBySearchString(string searchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new WorkflowConfigurationRepository(dataAccessTransaction).GetBySearchString(CheckNull.AsteriskToString(searchString));
            }

        }

        public static List<WorkflowConfiguration> GetByIncludeEndpoint()
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.WorkflowConfiguration>(dataAccessTransaction)
                       .Get(x => 1 == 1, includeProperties: u => u.Endpoint).ToList());
            }
                
        }

        public static bool DoesExists(WorkflowConfiguration workflow)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                bool isForSFB = false;
                if (workflow.EndpointSystemId.HasValue)
                {
                    var endPoint = EndPoint.GetById(workflow.EndpointSystemId.Value);
                    if(endPoint != null)
                    {
                        if(endPoint.ApplicationType  == ApplicationType.MicrosoftTeams)
                        {
                            isForSFB = true;
                        }
                    }
                }
                if (isForSFB)
                {
                    if (workflow.Id == 0)
                        return new RepositoryBase<EntityModel.WorkflowConfiguration>(tranaction)
                               .Any(wf => wf.RequestTypeId == workflow.RequestTypeId && wf.ApplicableAccountTypes == workflow.ApplicableAccountTypes
                               && wf.ApplicableActionTypes == workflow.ApplicableActionTypes && wf.ApplicableUserTypes == workflow.ApplicableUserTypes && wf.EndpointSystemId == workflow.EndpointSystemId
                                && wf.ValidFrom == workflow.ValidFrom && wf.EntitlementType == workflow.EntitlementType
                               );
                    else
                        return new RepositoryBase<EntityModel.WorkflowConfiguration>(tranaction)
                           .Any(wf => wf.Id != workflow.Id && wf.RequestTypeId == workflow.RequestTypeId && wf.ApplicableAccountTypes == workflow.ApplicableAccountTypes
                           && wf.ApplicableActionTypes == workflow.ApplicableActionTypes && wf.ApplicableUserTypes == workflow.ApplicableUserTypes && wf.EndpointSystemId == workflow.EndpointSystemId
                           && wf.EntitlementType == workflow.EntitlementType
                           );
                }
                else
                {
                    if (workflow.Id == 0)
                        return new RepositoryBase<EntityModel.WorkflowConfiguration>(tranaction)
                               .Any(wf => wf.RequestTypeId == workflow.RequestTypeId && wf.ApplicableAccountTypes == workflow.ApplicableAccountTypes
                               && wf.ApplicableActionTypes == workflow.ApplicableActionTypes && wf.ApplicableUserTypes == workflow.ApplicableUserTypes && wf.EndpointSystemId == workflow.EndpointSystemId
                                && wf.ValidFrom == workflow.ValidFrom
                               );
                    else
                        return new RepositoryBase<EntityModel.WorkflowConfiguration>(tranaction)
                           .Any(wf => wf.Id != workflow.Id && wf.RequestTypeId == workflow.RequestTypeId && wf.ApplicableAccountTypes == workflow.ApplicableAccountTypes
                           && wf.ApplicableActionTypes == workflow.ApplicableActionTypes && wf.ApplicableUserTypes == workflow.ApplicableUserTypes && wf.EndpointSystemId == workflow.EndpointSystemId);
                }
                
            }

        }

    }
}
