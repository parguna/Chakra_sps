using DI.Chakra.Compliance;
//using DI.Chakra.Facade;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
//using DI.Chakra.Model;
//using DI.Chakra.Model.SAP;
//using DI.Chakra.Model.SOD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel; 
using DI.Chakra.Transaction;

namespace DI.Chakra.Services.ComplianceProcessor
{
    public class ComplianceService
    {
        private const string KEY_SERVICENAME = @"System";

        //private SchedulerFacade m_schedulerDA;
        //private SAPFacade sapFacade;
        //private SodFacade sodFacade;
        //private ProcessFacade processFacade;
        //private LoggerFacade LoggerFace;

        public ComplianceService()
        {
            //m_schedulerDA = new SchedulerFacade();
            //sodFacade = new SodFacade();
            //sapFacade = new SAPFacade();
            //processFacade = new ProcessFacade();
        }
        private void ChangeComplianceStatus(Process process, SchedulerStatus _status)
        {
           // var complainceProcess = ComplianceProcess.GetById(process.ProcessTypeKeyId,true);
            if (_status == SchedulerStatus.InProgress)
            {
                process.ProcessStartAt = DateTime.Now;
                process.ProcessEndAt = null;
                process.Status = "InProgress";
                //  discoveryProcess.Message = string.Empty;
            }

            else if (_status == SchedulerStatus.Failed)
            {
                process.ProcessEndAt = DateTime.Now;
                process.Status = "Failed";
                //SendEmail(process, discoveryProcess.ProcessName, endpoint.Owner.EmailId);
            }
            else if (_status == SchedulerStatus.Pending)
            {
                process.Status = "Pending";
            }
            else
            {
                process.ProcessEndAt = DateTime.Now;
                process.LastRunOn = DateTime.Now;
                process.Status = "Completed";
               
            }
            //processFacade.UpdateProcess(process);
            //complainceProcess.Save();
            process.Save(MasterConfigHelper.ServiceUser);
        }

        public void Start(long _processId, Process process)
        {
            ComplianceProcess complianceProcess = null;         

            try
            {
                
                ChangeComplianceStatus(process, SchedulerStatus.InProgress);
                //complianceProcess = sodFacade.GetComplianceProcess(_processId);
                complianceProcess = ComplianceProcess.GetComplianceProcess(_processId);
                var policyExecutionDefinition = complianceProcess.PolicyExecutionDefinition;
                if (policyExecutionDefinition == null)
                {
                    DILog.ComplianceLogger.WriteLog(process.ProcessTypeKeyId, LogLevelType.Info, string.Format("PolicyExecutionDefinition not found for ProcessId - {0}", _processId.ToString()));
                    throw new EntityNotFoundException(string.Format("PolicyExecutionDefinition not found for ProcessId - {0}", _processId.ToString()));
                }
                var policy = policyExecutionDefinition.CompliancePolicy;
                if (policy == null)
                {
                    DILog.ComplianceLogger.WriteLog(process.ProcessTypeKeyId, LogLevelType.Info, string.Format("CompliancePolicy not found for ProcessId - {0}", _processId.ToString()));
                    throw new EntityNotFoundException(string.Format("CompliancePolicy not found for ProcessId - {0}", _processId.ToString()));
                }
                List<SODPolicyReport> ComplianceReport = new List<SODPolicyReport>();
                int counter = 0;
                if (policy.CompliancePolicyType != "SOD")
                {
                    foreach (SODConflictFunctions conflictfunction in policy.Conflicts)
                    {
                        //List<SodFunction> SodFunctions = sodFacade.GetSodFunctionFromConflictFunction(conflictfunction.SodConflictCode);
                        SODConflictFunctions conflictFunctions = SODConflictFunctions.GetSodFunctionFromConflictFunction(conflictfunction.SodConflictCode);
                        List<SODFunction> SodFunctions = conflictFunctions.Functions;
                        foreach (SODFunction function in SodFunctions)
                        {
                            List<SODPolicyReport> ReportLst = CheckUsersWithSensitiveAccess(function, policyExecutionDefinition.EndpointSelection, policyExecutionDefinition);
                            foreach (SODPolicyReport report in ReportLst)
                            {
                                // report.Id = counter++;
                                report.RunDate = DateTime.Now;
                                report.Conflict = conflictfunction;
                                report.ConflictId = conflictfunction.Id;
                                report.ConflictCode = conflictfunction.SodConflictCode;
                                ComplianceReport.Add(report);
                            }
                        }
                    }
                }
                else
                {
                    foreach (SODConflictFunctions conflictfunction in policy.Conflicts)
                    {
                        //List<SODConflict> lstConflicts = sodFacade.GetSodConflictList(conflictfunction.SodConflictCode);
                        List<SODConflict> lstConflicts = SODConflict.GetBySodConflictCodeIncludingSODRiskCode(conflictfunction.SodConflictCode);
                        if (lstConflicts.Count > 0)
                        {
                            foreach (SODConflict conflict in lstConflicts)
                            {
                                bool Runpolicy = true;
                                if (Runpolicy)
                                {
                                    List<SODPolicyReport> ReportLst = CheckConflictBetweenFunctions(conflict.function1, conflict.function2, policyExecutionDefinition.EndpointSelection, policyExecutionDefinition);
                                   
                                    foreach (SODPolicyReport report in ReportLst)
                                    {
                                        //report.Id = counter++;
                                        report.RunDate = DateTime.Now;
                                        report.Conflict = conflictfunction;
                                        
                                        report.ConflictCode = conflict.SODConflictCode;
                                        report.FunctionId1 = conflict.FunctionId1.Value;
                                        report.FunctionId2 = conflict.FunctionId2.Value;
                                        //report.FunctionObjItem1 = sodFacade.GetSodFunctionItem(report.FunctionItem1.Value);
                                        //report.FunctionObjItem2 = sodFacade.GetSodFunctionItem(report.FunctionItem2.Value);
                                        report.FunctionObjItem1 = SODFunctionItem.GetByIdWithNavigation(report.FunctionItem1.Value);
                                        report.FunctionObjItem2 = SODFunctionItem.GetByIdWithNavigation(report.FunctionItem2.Value);
                                        report.ConflictId = conflictfunction.Id;
                                        try
                                        {
                                            if (conflict.SodRiskId != null && conflict.SodRiskId.Mitigations.Count > 0)
                                                report.MitigationCode = conflict.SodRiskId.Mitigations[0].MitigationCode;
                                        }
                                        catch (System.Exception ex) { }
                                        ComplianceReport.Add(report);
                                    }
                                }
                            }
                        }
                        else
                        {
                            DILog.ComplianceLogger.WriteLog(process.ProcessTypeKeyId, LogLevelType.Info, string.Format("SOD Conflict pair not found for SodConflictCode - {0}", conflictfunction.SodConflictCode));
                            throw new Exception(string.Format("SOD Conflict pair not found for SodConflictCode - {0}", conflictfunction.SodConflictCode));
                        }
                    }
                }
                if (policyExecutionDefinition.RunAtObjectLevel.HasValue && policyExecutionDefinition.RunAtObjectLevel.Value)
                {
                    //ComplianceReport = sodFacade.CheckConflictAtObjectLevel(ComplianceReport);
                    ComplianceReport = SODPolicyReport.CheckConflictAtObjectLevel(ComplianceReport);

                }
                foreach (SODException exception in policyExecutionDefinition.Exceptions)
                {
                    var sodException = SODException.GetById(exception.Id, true);
                    ComplianceReport = ApplyException(ComplianceReport, sodException);
                }
                string UniqueRunId = string.Format("{0} {1}", policy.CompliancePolicyCode, DateTime.Now);
                //sodFacade.SaveReport(ComplianceReport, UniqueRunId);
                SODPolicyReport.Save(ComplianceReport, UniqueRunId, MasterConfigHelper.ServiceUser);

                 ChangeComplianceStatus(process, SchedulerStatus.Completed);
                ProcessHistory history = getHistory(process.Id, "Success", "");
                //processFacade.AddProcessHistory(history);
                history.Save(MasterConfigHelper.ServiceUser);

            }
            catch (Exception ex)
            {
                DILog.ComplianceLogger.WriteLog(process.ProcessTypeKeyId, LogLevelType.Info, "Process Failed"+ex.Message);
                process.ErrorMessage = ex.Message;
                ChangeComplianceStatus(process, SchedulerStatus.Failed);
                // I need to write a code for saving error message and status in to ProcessHistroy table.
                ProcessHistory history = getHistory(_processId, "Failure", ex.Message);
                //processFacade.AddProcessHistory(history);
                history.Save(MasterConfigHelper.ServiceUser);
               // DILog.DiscoveryLogger.WriteLog(null, LogLevelType.Error, "ErrorTraced", ex, DiscoveryObjectType.Process, 0);
                System.Diagnostics.EventLog _log = new System.Diagnostics.EventLog();
                System.Diagnostics.EventLogTraceListener _listener = new System.Diagnostics.EventLogTraceListener();
                _log.Source = "DI-Team";
                _log.Log = "V5 Dev Service";
                _listener.EventLog = _log;
                System.Diagnostics.Trace.Listeners.Add(_listener);
                System.Diagnostics.Trace.Write(ex);
                //throw ex;
            }
        }
        private ProcessHistory getHistory(long ProcessId, string Status, string ErrMessage)
        {
            ProcessHistory history = new ProcessHistory();
            history.ProcessType = ProcessType.Discovery;
            history.ProcessId = ProcessId;
            history.Status = Status;
            history.ErrorMessage = ErrMessage;
            return history;

        }
        private List<SODPolicyReport> ApplyException(List<SODPolicyReport> lstReport, SODException exception)
        {
            List<SODPolicyReport> ExceptionMatching = null;

            foreach (SODExceptionItem sodexItem in exception.ExceptionItems)
            {
                var exItem = SODExceptionItem.GetById(sodexItem.Id, true);
                if (exItem.user != null)
                {
                    if (exItem.functionitem1 != null && exItem.functionitem2 != null)
                    {
                        ExceptionMatching = lstReport.Where(s => s.UserName == exItem.user.DisplayName && s.FunctionId1 == exItem.FunctionId1
                                                            && s.FunctionItem1 == exItem.FunctionItemId1 && s.FunctionItem2 == exItem.FunctionItemId2
                                                            && s.FunctionId2 == exItem.FunctionId2).ToList();
                    }
                    else if (exItem.functionitem1 != null && exItem.functionitem2 == null)
                    {
                        ExceptionMatching = lstReport.Where(s => s.UserName == exItem.user.DisplayName && s.FunctionId1 == exItem.FunctionId1
                                                            && s.FunctionItem1 == exItem.FunctionItemId1).ToList();
                    }
                    else if (exItem.function1 != null && exItem.function2 != null)
                    {
                        ExceptionMatching = lstReport.Where(s => s.UserName == exItem.user.DisplayName && s.FunctionId1 == exItem.FunctionId1 && s.FunctionId2 == exItem.FunctionId2).ToList();
                    }
                    else if (exItem.function1 != null && exItem.function2 == null)
                    {
                        ExceptionMatching = lstReport.Where(s => s.UserName == exItem.user.DisplayName && s.FunctionId1 == exItem.FunctionId1).ToList();

                    }
                    else if (exItem.function1 == null && exItem.function2 == null)
                    {
                        ExceptionMatching = lstReport.Where(s => s.UserName == exItem.user.DisplayName).ToList();

                    }
                }
                else
                {
                    if (exItem.functionitem1 != null && exItem.functionitem2 != null)
                    {
                        ExceptionMatching = lstReport.Where(s => s.FunctionId1 == exItem.FunctionId1
                                                            && s.FunctionItem1 == exItem.FunctionItemId1 && s.FunctionItem2 == exItem.FunctionItemId2
                                                            && s.FunctionId2 == exItem.FunctionId2).ToList();
                    }
                    if (exItem.functionitem1 != null && exItem.functionitem2 == null)
                    {
                        ExceptionMatching = lstReport.Where(s => (s.FunctionId1 == exItem.FunctionId1
                                                            && s.FunctionItem1 == exItem.FunctionItemId1) ||
                                                                (s.FunctionId2 == exItem.FunctionId1
                                                            && s.FunctionItem2 == exItem.FunctionItemId1)

                                                            ).ToList();
                    }
                    else if (exItem.function1 != null && exItem.function2 != null)
                    {
                        ExceptionMatching = lstReport.Where(s => s.FunctionId1 == exItem.FunctionId1 && s.FunctionId2 == exItem.FunctionId2).ToList();
                    }
                    else if (exItem.function1 != null && exItem.function2 == null)
                    {
                        ExceptionMatching = lstReport.Where(s => s.FunctionId1 == exItem.FunctionId1 || s.FunctionId2 == exItem.FunctionId1).ToList();

                    }

                }
            }

            foreach (SODPolicyReport report in lstReport)
            {
                if (ExceptionMatching.Contains(report))
                {
                    report.ExceptionId = exception.Id;
                }


            }
            return lstReport;
        }
        private List<SODPolicyReport> CheckConflictBetweenFunctions(SODFunction function1, SODFunction function2, List<SODEndPointPolicySelection> endpointSelections,SODPolicyExecutionDefinition definition)
        {
            List<SODPolicyReport> lstreport = new List<SODPolicyReport>();
            IList<string> uniqueUsersInFunction1 = new List<string>();
            IList<string> uniqueUsersInFunction2 = new List<string>();

            List<UsersInRole> UsersinRoleFunction1 = GetAllUsersInsideFunction(function1, ref uniqueUsersInFunction1, endpointSelections);
            if (UsersinRoleFunction1.Count == 0) return lstreport;
            List<UsersInRole> UsersinRoleFunction2 = GetAllUsersInsideFunction(function2, ref uniqueUsersInFunction2, endpointSelections);

            List<string> ProblemUsers = uniqueUsersInFunction1.Intersect(uniqueUsersInFunction2).ToList();

            var listUsersinRoleFunction1 = new List<UsersInRole>();
            var listUsersinRoleFunction2 = new List<UsersInRole>();

            foreach (string problemUser in ProblemUsers)
            {
                listUsersinRoleFunction1 = (from u in UsersinRoleFunction1
                                            from x in u.Users
                                            where x == problemUser
                                            select u).ToList();

                listUsersinRoleFunction2 = (from u in UsersinRoleFunction2
                                            from x in u.Users
                                            where x == problemUser
                                            select u).ToList();


                SODPolicyReport report = null;
                foreach (var usrFn1 in listUsersinRoleFunction1)
                {

                    foreach (var usrFn2 in listUsersinRoleFunction2)
                    {

                        report = new SODPolicyReport();
                        report.Function1 = function1;
                        report.Role1 = usrFn1.RoleName;
                        report.Function2 = function2;
                        report.Role2 = usrFn2.RoleName;
                        report.UserName = problemUser;
                        report.FunctionId1 = usrFn1.FunctionId;
                        report.FunctionItem1 = (int)usrFn1.FunctionItemId;
                        report.FunctionType1 = usrFn1.FunctionType;
                        report.FunctionId2 = usrFn2.FunctionId;
                        report.FunctionItem2 = (int)usrFn2.FunctionItemId;
                        report.FunctionType2 = usrFn2.FunctionType;
                        report.PolicyDefinition = definition;
                        report.PolicyExecutionDefinitionId = definition.Id;

                        lstreport.Add(report);
                    }
                }
            }
            return lstreport;

        }
        private List<SODPolicyReport> CheckUsersWithSensitiveAccess(SODFunction function1, List<SODEndPointPolicySelection> endPointSelections, SODPolicyExecutionDefinition policyExecutionDefinition)
        {

            IList<string> uniqueUsersInFunction1 = new List<string>();

            List<UsersInRole> UsersinRoleFunction1 = GetAllUsersInsideFunction(function1, ref uniqueUsersInFunction1, endPointSelections);


            var listUsersinRoleFunction1 = new List<UsersInRole>();
            List<SODPolicyReport> lstreport = new List<SODPolicyReport>();

            SODPolicyReport report = null;
            foreach (var usrFn1 in UsersinRoleFunction1)
            {
                if (usrFn1.Users != null)
                {
                    foreach (string user in usrFn1.Users)
                    {
                        report = new SODPolicyReport();
                        report.Function1 = function1;
                        report.Role1 = usrFn1.RoleName;
                        report.UserName = user;
                        report.FunctionId1 = usrFn1.FunctionId;
                        report.FunctionItem1 = (int)usrFn1.FunctionItemId;
                        report.FunctionType1 = usrFn1.FunctionType;
                        report.PolicyDefinition = policyExecutionDefinition;
                        report.PolicyExecutionDefinitionId = policyExecutionDefinition.Id;
                        lstreport.Add(report);
                    }
                }

            }

            return lstreport;

        }
        private List<UsersInRole> GetAllUsersInsideFunction(SODFunction function, ref IList<string> UniqueUsersinsideFunction, List<SODEndPointPolicySelection> endPointSelections)
        {

            IList<string> uniqueUsers = new List<string>();
            IList<int> DIuniqueUsers = new List<int>();

            List<UsersInRole> UsersInAllRoles = new List<UsersInRole>();

            foreach (SODFunctionItem functionitem in function.SodFunctionItems)
            {

                UsersInRole u = GetAllUsersInsideFunctionItem(functionitem, ref uniqueUsers, endPointSelections);

                u.FunctionItemId = functionitem.Id;
                u.FunctionId = function.Id;
                u.FunctionType = functionitem.SodFunctionType;
                UsersInAllRoles.Add(u);
                UniqueUsersinsideFunction = UniqueUsersinsideFunction.Union(uniqueUsers).ToList();

            }

            return UsersInAllRoles;
        }
        private UsersInRole GetAllUsersInsideFunctionItem(SODFunctionItem fItem, ref IList<string> UniqueUsers, List<SODEndPointPolicySelection> endPointSelections)
        {

            UsersInRole u = new UsersInRole();
            //UserFacade usrDa = new UserFacade();

            List<string> UsersInSingleFunctionItem = new List<string>();
            List<SODEndPointPolicySelection> Matched = endPointSelections.Where(s => s.LevelName == fItem.SodFunctionType).ToList();
            if ((Matched != null && Matched.Count > 0))
            {
                if (fItem.EndpointType.EndpointSystemTypeName == Infrastructure.Enums.EndpointType.ActiveDirectory)
                {
                    //ActiveDirectoryFacade activeFacade = new ActiveDirectoryFacade();
                    //List<ADSUserInfo> usrList1 = activeFacade.GetAllUsersByGroup(fItem.SodRoleId, fItem.Endpointsystemid.Value);
                    List<ADSUserInfo> usrList1 = ADSUserInfo.GetByCnAndEndpointId(fItem.SodRoleId, fItem.Endpointsystemid.Value);

                    u.RoleName = fItem.SodRoleDesc;


                    foreach (ADSUserInfo userinfo in usrList1)
                    {
                        AccountReconcilation useracc = AccountReconcilation.GetByUser(userinfo.EndPointSystemId, userinfo.Id);
                        if (useracc!=null? (useracc.UserId != null):false)
                        {
                            //User usr = usrDa.GetUser(userinfo.DIUserId.Value);
                            User usr = User.GetById(useracc.UserId.Value);
                            UsersInSingleFunctionItem.Add(usr.DisplayName.Trim());
                            UniqueUsers.Add(usr.DisplayName.Trim());
                        }
                    }

                    u.Users = UsersInSingleFunctionItem;
                }
                else if (fItem.EndpointType.EndpointSystemTypeName == Infrastructure.Enums.EndpointType.SAP)
                {
                    u.RoleName = fItem.SodRoleDesc;

                    if (fItem.SodFunctionType == "Role")
                    {
                        // Get all users linked to the role
                        //List<SAPUserinfo> sapUsrList = sapFacade.GetAllUsersInARole(fItem.SodRoleDesc, fItem.Endpointsystemid.Value);
                        List<SAPUserinfo> sapUsrList = SAPUserinfo.GetByRoleCode(fItem.SodRoleDesc, fItem.Endpointsystemid.Value);
                        if (sapUsrList != null)
                        {
                            foreach (SAPUserinfo usrrole in sapUsrList)
                            {
                                AccountReconcilation useracc = AccountReconcilation.GetByUser(usrrole.Id, usrrole.EndPointSystemId);
                                if (useracc.UserId != null)
                                {
                                    //User usr = usrDa.GetUser(usrrole.DIUserId.Value);
                                    User usr = User.GetById(useracc.UserId.Value);
                                    if (usr != null)
                                    {
                                        UniqueUsers.Add(usr.DisplayName.Trim());
                                        UsersInSingleFunctionItem.Add(usr.DisplayName.Trim());
                                    }
                                }
                            }
                        }

                    }
                    else if (fItem.SodFunctionType == "TCodes")
                    {
                        // get all users linked to the tcode
                        IList<SAPUserinfo> sapUsrList = SAPUserinfo.GetAllUsersInATCode(fItem.SodRoleDesc, fItem.Endpointsystemid.Value);
                        foreach (SAPUserinfo usrrole in sapUsrList)
                        {
                            UsersInSingleFunctionItem.Add(usrrole.UserLogonName.Trim());
                            UniqueUsers.Add(usrrole.UserLogonName.Trim());
                        }

                    }
                    else if (fItem.SodFunctionType == "Profiles")
                    {
                        //List<SAPUserinfo> sapUsrList = sapFacade.GetAllUsersInProfile(fItem.SodRoleDesc, fItem.Endpointsystemid.Value);
                        IList<SAPUserinfo> sapUsrList = SAPUserinfo.GetByProfileCodeWithNavigation(fItem.SodRoleDesc, fItem.Endpointsystemid.Value);
                        foreach (SAPUserinfo usrrole in sapUsrList)
                        {
                            UsersInSingleFunctionItem.Add(usrrole.UserLogonName.Trim());
                            UniqueUsers.Add(usrrole.UserLogonName.Trim());
                        }
                    }


                    u.Users = UsersInSingleFunctionItem;
                }
            }

            return u;
        }
    }
}
