using DI.Chakra.DataAccess;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model;
using System;
using System.Collections.Generic;
using System.Data;

namespace DI.Chakra.Facade
{
    public class WorkFlowConfigurationFacade
    {
        WorkFlowConfigurationDA wfConfigDA = new WorkFlowConfigurationDA();

        //GetWorkFlowConfigurationByAttestationId
        //public void CreateWorkFlowConfiguration(WorkflowConfiguration wfConfig)
        //{
        //    try
        //    {
        //       // wfConfigDA.CreateWorkFlowConfiguration(wfConfig);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}


        //public void UpdateWorkFlowConfiguration(WorkflowConfiguration wfConfig)
        //{
        //    try
        //    {
        //        wfConfigDA.UpdateWorkFlowConfiguration(wfConfig);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //Default Save
        //public void CreateWorkFlowConfigurationDetail(WorkflowConfigurationDetail wfConfigDetail)
        //{
        //    try
        //    {
        //        wfConfigDA.CreateWorkFlowConfigurationDetail(wfConfigDetail);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //Default Save
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

        //GetWorkflowConfigurationDetailById
        public WorkflowConfiguration GetWorkFlowConfigurationById(long configId, bool deepLoad)
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

        //GetByWorkflowName
        //public List<WorkflowConfiguration> GetWorkFlowConfigurationsByName(string wfName)
        //{
        //    try
        //    {
        //       // return wfConfigDA.GetWorkFlowConfigurationsByName(wfName);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}


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

        //public WorkflowConfiguration GetWorkFlowConfigurationByRunPolicyId(int runPolicyId, bool deepLoad)
        //{
        //    try
        //    {
        //       // return wfConfigDA.GetWorkFlowConfigurationByRunPolicyId(runPolicyId, deepLoad);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //GetByRequestTypeId
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
        
        //presentation - GetApproversList
        //public IList<WorkflowApproverGridData> GetApproverListForConfigurationDetailList(IList<WorkflowConfigurationDetail> _wfConfigDetailList)
        //{
        //    try
        //    {
        //        return wfConfigDA.GetApproverListForConfigurationDetailList(_wfConfigDetailList);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //WorkflowConfigurationDetails.DefaultDeleteBy ID
        public void DeleteWorkflowConfigurationDetailsByWfConfigDetailId(long WfConfigDetailId)
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
        ////  Usemethod
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

        //public IList<WorkflowGridData> GetAllWorkflowConfigurationsGridData()
        //{
        //    try
        //    {
        //        return wfConfigDA.GetAllWorkflowConfigurationsGridData();
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public WorkflowConfiguration GetWorkFlowConfiguration(long endpointSystemId, byte userType, byte accountType, byte actionType)
        {
            try
            {
                var wfConfig = wfConfigDA.GetWorkFlowConfiguration(endpointSystemId, userType.ToString().PadLeft(3, '0'),
                            accountType.ToString().PadLeft(3, '0'), actionType.ToString().PadLeft(3, '0'));

                if (wfConfig != null)
                {
                    wfConfig = this.GetWorkFlowConfigurationById(wfConfig.Id, true);
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
