using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DI.Chakra.Repository
{
    public class WorkflowConfigurationRepository : RepositoryBase<WorkflowConfiguration>
    {
        public WorkflowConfigurationRepository(DataAccessTransaction transaction) : base(transaction) { }


        public WorkflowConfiguration GetWorkFlowConfiguration(long requestTypeId, long endpointSystemId, string userType, string accountType, string actionType, DateTime requestDate,string entitlementType=null)
        {
            try
            {

                string query = @"declare @RequestType int = @P_RequestType
                                declare @EndpointSystemId int = @P_EndpointSystemId
                                declare @ApplicableUserType nvarchar(10) = @P_ApplicableUserType
                                declare @ApplicableAccountType nvarchar(10) = @P_ApplicableAccountType
                                declare @ApplicableActionType nvarchar(10) = @P_ApplicableActionType
                                declare @All nvarchar(10) = '000'
                                SELECT      *
                                FROM        DI_M_WorkflowConfiguration
                                WHERE       (RequestTypeId = @RequestType) AND (EndpointSystemId = @EndpointSystemId) AND (IsActive = 1) AND
                                            (ApplicableUserTypes LIKE CASE WHEN ApplicableUserTypes != @All THEN '%' + @ApplicableUserType + '%' ELSE ApplicableUserTypes END) AND
			                                (ApplicableAccountTypes LIKE CASE WHEN ApplicableAccountTypes != @All THEN '%' + @ApplicableAccountType + '%' ELSE ApplicableAccountTypes END) AND
			                                (ApplicableActionTypes LIKE CASE WHEN ApplicableActionTypes != @All THEN '%' + @ApplicableActionType + '%' ELSE ApplicableActionTypes END)";
                var queryParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@P_RequestType", requestTypeId),
                    new System.Data.SqlClient.SqlParameter("@P_EndpointSystemId", endpointSystemId),
                    new System.Data.SqlClient.SqlParameter("@P_ApplicableUserType", (object)userType ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@P_ApplicableAccountType", (object)accountType ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@P_ApplicableActionType", (object)actionType ?? DBNull.Value)
                };
                if (!string.IsNullOrWhiteSpace(entitlementType))
                {
                    query = query + " AND EntitlementType LIKE @P_EntitlementType";
                    queryParameters.Add(new System.Data.SqlClient.SqlParameter("@P_EntitlementType", "%" + entitlementType + "%"));
                }

                query = query + " order by case ApplicableActionTypes when @All then 1 else 0 end, case ApplicableAccountTypes when @All then 1 else 0 end, case ApplicableUserTypes when @All then 1 else 0 end ";
                DILog.ProvisioningLogger.WriteLog(null, endpointSystemId, LogLevelType.DebugStep, $"Checking work flow availablity : Endpoint system Id:{endpointSystemId},  UserType:{userType}, actiontype :{actionType}  is {query}");

                return  CurrentContext.Database.SqlQuery<WorkflowConfiguration>(query, queryParameters.ToArray()).FirstOrDefault(item => item.ValidFrom <= requestDate && item.ValidTo > requestDate);
            }
            catch
            {
                throw;
            }
        }

        public IList<PresentationWorkflowConfiguration> GetAll()
        {
            IList<PresentationWorkflowConfiguration> presentationWorkflowConfigurationList = new List<PresentationWorkflowConfiguration>();
            RequestType requestType;
            PresentationWorkflowConfiguration presentationWorkflowConfiguration;
            IList<WorkflowConfiguration> dbWorkflowConfigurationList = this.CurrentContext.WorkflowConfigurations.Include(work1 => work1.Endpoint).ToList();

            foreach (WorkflowConfiguration cnfg in dbWorkflowConfigurationList)
            {
                presentationWorkflowConfiguration = new PresentationWorkflowConfiguration();
                presentationWorkflowConfiguration.WorkflowConfigurationId = cnfg.Id;
                presentationWorkflowConfiguration.WorkflowName = cnfg.WorkflowName;
                presentationWorkflowConfiguration.Description = cnfg.Description;
                requestType = (RequestType)cnfg.RequestTypeId;
                presentationWorkflowConfiguration.RequestType = UtilityMethods.GetEnumAttribute<Description>(requestType);
                presentationWorkflowConfiguration.ApplicationType = cnfg.Endpoint == null ? string.Empty : cnfg.Endpoint.DisplayName;
                presentationWorkflowConfiguration.IsActive = cnfg.IsActive;
                presentationWorkflowConfiguration.CreatedBy = cnfg.CreatedBy;
                presentationWorkflowConfiguration.CreatedDate = cnfg.CreatedDate;
                presentationWorkflowConfiguration.LastModifiedBy = cnfg.LastModifiedBy;
                presentationWorkflowConfiguration.LastModifiedDate = cnfg.LastModifiedDate;

                presentationWorkflowConfigurationList.Add(presentationWorkflowConfiguration);
            }

            return presentationWorkflowConfigurationList;
        }

        public Dictionary<long, string> GetBySearchString(string searchString)
        {
            return CurrentContext.WorkflowConfigurations.Where(item => item.WorkflowName.Contains(searchString) &&
            item.RequestTypeId == (byte)RequestType.Provisioning)
                .Select(x => new { Key = x.Id, Value = x.WorkflowName })
                .Distinct().ToDictionary(o => o.Key, o => o.Value);
        }

    }
}

