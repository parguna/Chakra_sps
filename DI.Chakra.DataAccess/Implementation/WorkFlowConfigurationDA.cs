using DI.Chakra.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DI.Chakra.Infrastructure;
using System.Data.Entity;
using DI.Chakra.Infrastructure.Enums;
using System.Text;
using DI.Chakra.Logger;

namespace DI.Chakra.DataAccess
{
    public class WorkFlowConfigurationDA
    {

        public void CreateWorkFlowConfiguration(WorkflowConfiguration wfConfig)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.WorkflowConfigurations.Add(wfConfig);
                    _context.SaveChanges();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var entry = _context.Entry(wfConfig);
                    if (entry.State == EntityState.Detached)
                    {
                        var set = _context.Set<WorkflowConfiguration>();
                        WorkflowConfiguration attachedTask = set.Find(wfConfig.Id);

                        if (attachedTask != null)
                        {
                            var attachedEntry = _context.Entry(attachedTask);
                            attachedEntry.CurrentValues.SetValues(wfConfig);
                        }
                    }
                    _context.SaveChanges();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.WorkflowConfigurationDetails.Add(wfConfigDetail);
                    _context.SaveChanges();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var entry = _context.Entry(wfConfigDetail);
                    if (entry.State == EntityState.Detached)
                    {
                        var set = _context.Set<WorkflowConfigurationDetail>();
                        WorkflowConfigurationDetail attachedTask = set.Find(wfConfigDetail.Id);

                        if (attachedTask != null)
                        {
                            var attachedEntry = _context.Entry(attachedTask);
                            attachedEntry.CurrentValues.SetValues(wfConfigDetail);
                        }
                    }
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public WorkflowConfiguration GetWorkFlowConfigurationById(long configId, bool deepLoad)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Configuration.ProxyCreationEnabled = false;

                    WorkflowConfiguration wfConfig = _context.WorkflowConfigurations.Include("Endpoint").FirstOrDefault(item => item.Id == configId);

                    if (deepLoad && wfConfig != null)
                    {
                        wfConfig.WorkflowConfigurationDetails = _context.WorkflowConfigurationDetails.Where(item => item.WorkflowConfigurationId == configId).
                            OrderBy(item => item.SequenceNumber).ToList();
                    }

                    return wfConfig;
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.WorkflowConfigurations.Where(item => item.WorkflowName == wfName).ToList();
                }
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
                long configId = new AttestationDA().GetAttestation(attestationId, false).WorkflowConfigurationId;
                return GetWorkFlowConfigurationById(configId, deepLoad);
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
                long configId = new SodDA().GetSODRunPolicyById(runPolicyId, false).WorkflowConfigurationId.Value;
                return GetWorkFlowConfigurationById(configId, deepLoad);
            }
            catch
            {
                throw;
            }
        }

        public IList<WorkflowConfiguration> GetWorkFlowConfigurationsByRequestType(byte requestTypeId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.WorkflowConfigurations.Where(item => item.RequestTypeId == requestTypeId && item.IsActive).ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.WorkflowConfigurations.ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured While Getting All Workflow Configurations", ex);
                throw ex;
            }
        }

        public IList<WorkflowConfigurationDetail> GetWorkflowConfigurationDetailsByConfigurationId(short wfConfigID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.WorkflowConfigurationDetails.Where(wfc => wfc.WorkflowConfigurationId == wfConfigID).ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured While Getting WorkflowConfigurationDetails", ex);
                throw ex;
            }
        }

        public IList<WorkflowApproverGridData> GetApproverListForConfigurationDetailList(IList<WorkflowConfigurationDetail> _wfConfigDetailList)
        {
            IList<WorkflowApproverGridData> approverList = new List<WorkflowApproverGridData>();
            WorkflowApproverGridData approver;
            UserDA userDA = new UserDA();
            GroupDA groupDA = new GroupDA();
            MasterDA masterDA = new MasterDA();

            try
            {
                foreach (var listItem in _wfConfigDetailList)
                {
                    approver = new WorkflowApproverGridData();

                    approver.WorkflowConfigurationDetailId = listItem.Id;

                    approver.ApprovalUserType = UtilityMethods.GetEnumAttribute<Description>(listItem.ApprovalUserType); // listItem.ApprovalUserType.ToString();

                    if (listItem.ApprovalUserType == ApprovalUserType.User)
                    {
                        User usr = userDA.GetUser(listItem.Approver);
                        if (usr != null)
                            approver.Approver = usr.KnownAs;
                    }
                    else if (listItem.ApprovalUserType == ApprovalUserType.Role || listItem.ApprovalUserType == ApprovalUserType.UseApplicationConfiguration)
                    {
                        //here its temporary commented Need to implement in DomainLayer
                        //IList<RoleTypes> roleTypes = masterDA.GetRoleTypes();
                        //approver.Approver = roleTypes.First(roltyp => roltyp.Id == listItem.Approver).RoleTypeDesc;
                    }
                    else if (listItem.ApprovalUserType == ApprovalUserType.Group)
                    {
                        Group grp = groupDA.GetGroupById(listItem.Approver);
                        if (grp != null)
                            approver.Approver = grp.GroupName;
                    }

                    approver.ResponseTimeInDays = listItem.ResponseTimeInDays;

                    if (listItem.IsReminderRequired)
                    {
                        approver.IsReminderRequired = string.Format("{0} (Every {1} days)", "Yes", listItem.ReminderIntervalInDays.Value);
                    }
                    else
                    {
                        approver.IsReminderRequired = "No";
                    }

                    if (listItem.IsEscalationRequired)
                    {
                        approver.IsEscalationRequired = string.Format("{0} (After {1} days)", "Yes", listItem.DaysToWaitAfterEscalation.Value);
                    }
                    else
                    {
                        approver.IsEscalationRequired = "No";
                    }

                    approver.SequenceNumber = listItem.SequenceNumber;

                    approver.AdditionalInfo = listItem.IsActionRequired ? "Action Task" : "Approval Task";

                    if (listItem.ApprovalUserType == ApprovalUserType.Group)
                    {
                        approver.AdditionalInfo += ", " + (listItem.IsMockGroup ? "Pseudo Group" : "Real Group");
                        approver.AdditionalInfo += ", " + (listItem.RequiresApprovalFromAll ? "Requires approval from all persons from this group" : "Requires approval from anyone person from this group");
                    }

                    if (listItem.ApprovalUserType == ApprovalUserType.UseApplicationConfiguration)
                    {
                        approver.AdditionalInfo += ", " + (listItem.RequiresApprovalFromAll ? "Requires approval from all persons from this level" : "Requires approval from anyone person from this level");
                    }

                    approverList.Add(approver);
                }
                return approverList.OrderBy(item => item.SequenceNumber).ToList();
            }
            catch
            {
                throw;
            }
        }

        public void DeleteWorkflowConfigurationDetailsByWfConfigDetailId(long WfConfigDetailId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    WorkflowConfigurationDetail delApprover = _context.WorkflowConfigurationDetails.Where(mt => mt.Id == WfConfigDetailId).First();
                    if (delApprover != null)
                    {
                        _context.WorkflowConfigurationDetails.Remove(delApprover);
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Deleting the Approver", ex);
                throw ex;
            }
        }

        public void DeleteWorkflowConfigurationById(long WfConfigId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    WorkflowConfiguration wfConfig = _context.WorkflowConfigurations.FirstOrDefault(mt => mt.Id == WfConfigId);
                    if (wfConfig != null)
                    {
                        _context.WorkflowConfigurations.Remove(wfConfig);
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Deleting the Workflow", ex);
                throw ex;
            }
        }

        public IList<WorkflowGridData> GetAllWorkflowConfigurationsGridData()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    IList<WorkflowGridData> _wfGridDataList = new List<WorkflowGridData>();
                    WorkflowGridData _wfCnfg;
                    RequestType rqstType;
                    IList<WorkflowConfiguration> _wfconfigList = _context.WorkflowConfigurations.Include("Endpoint").ToList();
                    foreach (WorkflowConfiguration cnfg in _wfconfigList)
                    {
                        _wfCnfg = new WorkflowGridData();
                        _wfCnfg.WorkflowConfigurationId = cnfg.Id;
                        _wfCnfg.WorkflowName = cnfg.WorkflowName;
                        _wfCnfg.Description = cnfg.Description;
                        rqstType = (RequestType)cnfg.RequestTypeId;
                        _wfCnfg.RequestType = UtilityMethods.GetEnumAttribute<Description>(rqstType);
                        _wfCnfg.ApplicationType = cnfg.Endpoint == null ? string.Empty : cnfg.Endpoint.DisplayName;
                        _wfCnfg.IsActive = cnfg.IsActive;
                        _wfCnfg.CreatedBy = cnfg.CreatedBy;
                        _wfCnfg.CreatedDate = cnfg.CreatedDate;
                        _wfCnfg.LastModifiedBy = cnfg.LastModifiedBy;
                        _wfCnfg.LastModifiedDate = cnfg.LastModifiedDate;

                        _wfGridDataList.Add(_wfCnfg);
                    }
                    return _wfGridDataList;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured While Getting All Workflow Configurations", ex);
                throw ex;
            }
        }

        public WorkflowConfiguration GetWorkFlowConfiguration(long endpointSystemId, string userType, string accountType, string actionType)
        {
            try
            {

                //DECLARE @wfId SMALLINT; DECLARE @userTypeWFs TABLE (Id SMALLINT); DECLARE @accountTypeWFs TABLE (Id SMALLINT); DECLARE @actionTypeWFs TABLE (Id SMALLINT);

                //IF EXISTS(SELECT WorkflowConfigurationId FROM [dbo].[DI_M_WorkflowConfiguration] WHERE ApplicableUserTypes LIKE '%' + @userType + '%' AND EndpointSystemId = @EndpointSystemId)
                //BEGIN INSERT INTO @userTypeWFs SELECT WorkflowConfigurationId FROM [DI_M_WorkflowConfiguration] WHERE ApplicableUserTypes LIKE '%' + @userType + '%' AND EndpointSystemId = @EndpointSystemId END
                //ELSE BEGIN INSERT INTO @userTypeWFs SELECT WorkflowConfigurationId FROM [DI_M_WorkflowConfiguration] WHERE ApplicableUserTypes = '000' AND EndpointSystemId = @EndpointSystemId END

                //IF EXISTS(SELECT WorkflowConfigurationId FROM [DI_M_WorkflowConfiguration] WHERE ApplicableAccountTypes LIKE '%' + @accountType + '%' AND EndpointSystemId = @EndpointSystemId)
                //BEGIN INSERT INTO @accountTypeWFs SELECT WorkflowConfigurationId FROM [DI_M_WorkflowConfiguration] WHERE ApplicableAccountTypes LIKE '%' + @accountType + '%' AND EndpointSystemId = @EndpointSystemId END
                //ELSE BEGIN INSERT INTO @accountTypeWFs SELECT WorkflowConfigurationId FROM [DI_M_WorkflowConfiguration] WHERE ApplicableAccountTypes = '000' AND EndpointSystemId = @EndpointSystemId END

                //IF EXISTS(SELECT WorkflowConfigurationId FROM [DI_M_WorkflowConfiguration] WHERE ApplicableActionTypes LIKE '%' + @actionType + '%' AND EndpointSystemId = @EndpointSystemId)
                //BEGIN INSERT INTO @actionTypeWFs SELECT WorkflowConfigurationId FROM [DI_M_WorkflowConfiguration] WHERE ApplicableActionTypes LIKE '%' + @actionType + '%' AND EndpointSystemId = @EndpointSystemId END
                //ELSE BEGIN INSERT INTO @actionTypeWFs SELECT WorkflowConfigurationId FROM [DI_M_WorkflowConfiguration] WHERE ApplicableActionTypes = '000' AND EndpointSystemId = @EndpointSystemId END

                //SELECT @wfId = (SELECT Id FROM @userTypeWFs INTERSECT SELECT Id FROM @accountTypeWFs INTERSECT SELECT Id FROM @actionTypeWFs)

                //SELECT * FROM [DI_M_WorkflowConfiguration] WHERE WorkflowConfigurationId = @wfId


                StringBuilder queryBuilder = new StringBuilder();

                queryBuilder.Append("DECLARE @wfId SMALLINT; DECLARE @userTypeWFs TABLE (Id SMALLINT); DECLARE @accountTypeWFs TABLE (Id SMALLINT); DECLARE @actionTypeWFs TABLE (Id SMALLINT); ");

                queryBuilder.AppendFormat("IF EXISTS(SELECT Id FROM [dbo].[DI_M_WorkflowConfiguration] WHERE ApplicableUserTypes LIKE '%{0}%' AND EndpointSystemId = {1}) ", userType, endpointSystemId);
                queryBuilder.AppendFormat("BEGIN INSERT INTO @userTypeWFs SELECT Id FROM [DI_M_WorkflowConfiguration] WHERE ApplicableUserTypes LIKE '%{0}%' AND EndpointSystemId = {1} END ", userType, endpointSystemId);
                queryBuilder.AppendFormat("ELSE BEGIN INSERT INTO @userTypeWFs SELECT Id FROM [DI_M_WorkflowConfiguration] WHERE ApplicableUserTypes = '000' AND EndpointSystemId = {0} END ", endpointSystemId);

                queryBuilder.AppendFormat("IF EXISTS(SELECT Id FROM [dbo].[DI_M_WorkflowConfiguration] WHERE ApplicableAccountTypes LIKE '%{0}%' AND EndpointSystemId = {1}) ", accountType, endpointSystemId);
                queryBuilder.AppendFormat("BEGIN INSERT INTO @accountTypeWFs SELECT Id FROM [DI_M_WorkflowConfiguration] WHERE ApplicableAccountTypes LIKE '%{0}%' AND EndpointSystemId = {1} END ", accountType, endpointSystemId);
                queryBuilder.AppendFormat("ELSE BEGIN INSERT INTO @accountTypeWFs SELECT Id FROM [DI_M_WorkflowConfiguration] WHERE ApplicableAccountTypes = '000' AND EndpointSystemId = {0} END ", endpointSystemId);

                queryBuilder.AppendFormat("IF EXISTS(SELECT Id FROM [dbo].[DI_M_WorkflowConfiguration] WHERE ApplicableActionTypes LIKE '%{0}%' AND EndpointSystemId = {1}) ", actionType, endpointSystemId);
                queryBuilder.AppendFormat("BEGIN INSERT INTO @actionTypeWFs SELECT Id FROM [DI_M_WorkflowConfiguration] WHERE ApplicableActionTypes LIKE '%{0}%' AND EndpointSystemId = {1} END ", actionType, endpointSystemId);
                queryBuilder.AppendFormat("ELSE BEGIN INSERT INTO @actionTypeWFs SELECT Id FROM [DI_M_WorkflowConfiguration] WHERE ApplicableActionTypes = '000' AND EndpointSystemId = {0} END ", endpointSystemId);

                queryBuilder.Append("SELECT @wfId = (SELECT Id FROM @userTypeWFs INTERSECT SELECT Id FROM @accountTypeWFs INTERSECT SELECT Id FROM @actionTypeWFs) ");
                queryBuilder.Append("SELECT * FROM [DI_M_WorkflowConfiguration] WHERE Id = @wfId");

                string query = queryBuilder.ToString();
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Database.SqlQuery<WorkflowConfiguration>(query).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }

    }
}
