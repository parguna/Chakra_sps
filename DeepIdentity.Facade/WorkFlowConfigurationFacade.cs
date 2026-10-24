using DeepIdentity.DataAccess;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using System;
using System.Collections.Generic;
using System.Data;

namespace DeepIdentity.Facade
{
    public class WorkFlowConfigurationFacade
    {
        WorkFlowConfigurationDA wfConfigDA = new WorkFlowConfigurationDA();

        public void CreateWorkFlowConfiguration(WorkflowConfiguration wfConfig)
        {
            try
            {
                wfConfigDA.CreateWorkFlowConfiguration(wfConfig);
            }
            catch
            {
                throw;
            }
        }

        public void UpdateWorkFlowConfiguration(WorkflowConfiguration wfConfig)
        {
            try
            {
                wfConfigDA.UpdateWorkFlowConfiguration(wfConfig);
            }
            catch
            {
                throw;
            }
        }

        public void CreateWorkFlowConfigurationDetail(WorkflowConfigurationDetail wfConfigDetail)
        {
            try
            {
                wfConfigDA.CreateWorkFlowConfigurationDetail(wfConfigDetail);
            }
            catch
            {
                throw;
            }
        }

        public void UpdateWorkFlowConfigurationDetail(WorkflowConfigurationDetail wfConfigDetail)
        {
            try
            {
                wfConfigDA.UpdateWorkFlowConfigurationDetail(wfConfigDetail);
            }
            catch
            {
                throw;
            }
        }

        public WorkflowConfiguration GetWorkFlowConfigurationById(short configId, bool deepLoad)
        {
            try
            {
                return wfConfigDA.GetWorkFlowConfigurationById(configId, deepLoad);
            }
            catch
            {
                throw;
            }
        }

        public List<WorkflowConfiguration> GetWorkFlowConfigurationsByName(string wfName)
        {
            try
            {
                return wfConfigDA.GetWorkFlowConfigurationsByName(wfName);
            }
            catch
            {
                throw;
            }
        }

        public WorkflowConfiguration GetWorkFlowConfigurationByAttestationId(int attestationId, bool deepLoad)
        {
            try
            {
                return wfConfigDA.GetWorkFlowConfigurationByAttestationId(attestationId, deepLoad);
            }
            catch
            {
                throw;
            }
        }

        public WorkflowConfiguration GetWorkFlowConfigurationByRunPolicyId(int runPolicyId, bool deepLoad)
        {
            try
            {
                return wfConfigDA.GetWorkFlowConfigurationByRunPolicyId(runPolicyId, deepLoad);
            }
            catch
            {
                throw;
            }
        }

        public IList<WorkflowConfiguration> GetWorkFlowConfigurationsByRequestType(RequestType requestType)
        {
            try
            {
                return wfConfigDA.GetWorkFlowConfigurationsByRequestType((byte)requestType);
            }
            catch
            {
                throw;
            }
        }

        public IList<WorkflowConfiguration> GetAllWorkflowConfigurations()
        {
            try
            {
                return wfConfigDA.GetAllWorkflowConfigurations();
            }

            catch
            {
                throw;
            }

        }

        public IList<WorkflowConfigurationDetail> GetWorkflowConfigurationDetailsByConfigurationId(short wfConfigID)
        {
            try
            {
                return wfConfigDA.GetWorkflowConfigurationDetailsByConfigurationId(wfConfigID);
            }

            catch
            {
                throw;
            }
        }

        public IList<WorkflowApproverGridData> GetApproverListForConfigurationDetailList(IList<WorkflowConfigurationDetail> _wfConfigDetailList)
        {
            try
            {
                return wfConfigDA.GetApproverListForConfigurationDetailList(_wfConfigDetailList);
            }
            catch
            {
                throw;
            }
        }

        public void DeleteWorkflowConfigurationDetailsByWfConfigDetailId(int WfConfigDetailId)
        {
            try
            {
                wfConfigDA.DeleteWorkflowConfigurationDetailsByWfConfigDetailId(WfConfigDetailId);
            }
            catch
            {

                throw;
            }
        }

        public void DeleteWorkflowConfigurationById(short WfConfigId)
        {
            try
            {
                wfConfigDA.DeleteWorkflowConfigurationById(WfConfigId);
            }
            catch
            {

                throw;
            }
        }

        public IList<WorkflowGridData> GetAllWorkflowConfigurationsGridData()
        {
            try
            {
                return wfConfigDA.GetAllWorkflowConfigurationsGridData();
            }
            catch
            {
                throw;
            }
        }

        public WorkflowConfiguration GetWorkFlowConfiguration(int endpointSystemId, byte userType, byte accountType, byte actionType)
        {
            try
            {
                var wfConfig = wfConfigDA.GetWorkFlowConfiguration(endpointSystemId, userType.ToString().PadLeft(3, '0'),
                            accountType.ToString().PadLeft(3, '0'), actionType.ToString().PadLeft(3, '0'));

                if (wfConfig != null)
                {
                    wfConfig = this.GetWorkFlowConfigurationById(wfConfig.WorkflowConfigurationId, true);
                }

                return wfConfig;
            }
            catch
            {
                throw;
            }
        }

    }
}
