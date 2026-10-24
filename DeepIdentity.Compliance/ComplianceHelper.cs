using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Compliance
{
    public class UsersInRole
    {

        public string RoleName { get; set; }
        public string FunctionType { get; set; }
        public long FunctionId { get; set; }
        public long FunctionItemId { get; set; }

        public IList<string> Users { get; set; }
        public IList<int> DIUserId { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1}", RoleName, Users.Count.ToString());
        }
    }
    /*
    public class ComplianceHelper
    {
        private SodFacade sodFacade = new SodFacade();

            public void ExecuteThePolicy(SODRunPolicy _runPolicy)
            {
                string ComplianceCode = _runPolicy.compliancepolicy.CompliancePolicyCode;
                CompliancePolicy policy = sodFacade.GetCompliancePolicy(ComplianceCode);

                //List<CompliancePolicy> CompliancePolicies = 
                List<SODReport> ComplianceReport = new List<SODReport>();
                int counter = 0;

                if (policy.CompliancePolicyType != "SOD")
                {
                    foreach (SODConflictFunction conflictfunction in policy.Conflicts)
                    {
                        List<SodFunction> SodFunctions = sodFacade.GetSodFunctionFromConflictFunction(conflictfunction.SodConflictCode);
                        foreach (SodFunction function in SodFunctions)
                        {
                            List<SODReport> ReportLst = CheckUsersWithSensitiveAccess(_runPolicy, function);
                            foreach (SODReport report in ReportLst)
                            {
                                report.Id = counter++;
                                report.ConflictCode = conflictfunction.SodConflictCode;
                                ComplianceReport.Add(report);
                            }
                        }
                    }
                }
                else
                {
                    foreach (SODConflictFunction conflictfunction in policy.Conflicts)
                    {
                        List<SODConflict> lstConflicts = sodFacade.GetSodConflictList(conflictfunction.SodConflictCode);
                        foreach (SODConflict conflict in lstConflicts)
                        {
                            bool Runpolicy = true;

                            Runpolicy = IsRiskLevelSelected(conflict);


                            if (Runpolicy)
                            {
                                List<SODReport> ReportLst = CheckConflictBetweenFunctions(_runPolicy, conflict.function1, conflict.function2);
                                foreach (SODReport report in ReportLst)
                                {
                                    report.Id = counter++;

                                    report.Conflict = conflictfunction;
                                    report.ConflictCode = conflict.SODConflictCode;
                                    report.FunctionId1 = conflict.FunctionId1.Value;
                                    report.FunctionId2 = conflict.FunctionId2.Value;
                                    report.FunctionObjItem1 = sodFacade.GetSodFunctionItem(report.FunctionItem1.Value);
                                    report.FunctionObjItem2 = sodFacade.GetSodFunctionItem(report.FunctionItem2.Value);
                                    report.ConflictId = conflictfunction.Id;

                                    if (conflict.SodRiskId.Mitigations.Count > 0)
                                        report.MitigationCode = conflict.SodRiskId.Mitigations[0].MitigationCode;

                                    ComplianceReport.Add(report);
                                }

                            }
                        }
                    }
                }

                if (_runPolicy.ObjectChecked)
                {
                    // ComplianceReport = sodFacade.CheckConflictAtObjectLevel(ComplianceReport);

                }

                //foreach (SODException exception in _runPolicy.Exceptions)
                //{

                //    ComplianceReport = ApplyException(ComplianceReport, exception);
                //}

                SODRunPolicy RunPolicy = new SODRunPolicy();

                RunPolicy.RunDate = DateTime.Now;


                RunPolicy.UniqueRunCode = _runPolicy.UniqueRunCode;
                RunPolicy.CompliancePolicyId = policy.Id;
                RunPolicy.UniqueRunCode = _runPolicy.UniqueRunCode;

                if (RunPolicy.UniqueRunCode.Trim() == string.Empty)
                {
                    RunPolicy.UniqueRunCode = string.Format("{0} {1}", policy.CompliancePolicyCode, DateTime.Now);
                }


                RunPolicy.RiskLevelId = _runPolicy.RiskLevelId;
                RunPolicy.RiskScoreFrom = _runPolicy.RiskScoreFrom;
                RunPolicy.RiskScoreTo = _runPolicy.RiskScoreTo;

                RunPolicy.RunAtObjectLevel = _runPolicy.RunAtObjectLevel;
                RunPolicy.EndpointSelection = _runPolicy.EndpointSelection;

                //RunPolicy.Exceptions = _runPolicy.Exceptions;




                long UniqueRunId = sodFacade.SaveRunPolicy(RunPolicy);

                foreach (SODReport report in ComplianceReport)
                {
                    report.RunPolicy = null;
                    report.Conflict = null;
                    report.exception = null;
                    report.Function1 = null;
                    report.Function2 = null;
                    report.FunctionObjItem1 = null;
                    report.FunctionObjItem2 = null;
                }


                //sodFacade.SaveReport(ComplianceReport, UniqueRunId);
            }

            private  bool IsRiskLevelSelected(SODConflict conflict)
        {
            return true;
        }

            private List<SODReport> CheckConflictBetweenFunctions(SODRunPolicy RunPolicy, SodFunction function1, SodFunction function2)
            {

                IList<string> uniqueUsersInFunction1 = new List<string>();
                IList<string> uniqueUsersInFunction2 = new List<string>();

                List<UsersInRole> UsersinRoleFunction1 = GetAllUsersInsideFunction(RunPolicy, function1, ref uniqueUsersInFunction1);
                List<UsersInRole> UsersinRoleFunction2 = GetAllUsersInsideFunction(RunPolicy, function2, ref uniqueUsersInFunction2);

                List<string> ProblemUsers = uniqueUsersInFunction1.Intersect(uniqueUsersInFunction2).ToList();

                var listUsersinRoleFunction1 = new List<UsersInRole>();
                var listUsersinRoleFunction2 = new List<UsersInRole>();
                List<SODReport> lstreport = new List<SODReport>();


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


                    SODReport report = null;
                    foreach (var usrFn1 in listUsersinRoleFunction1)
                    {

                        foreach (var usrFn2 in listUsersinRoleFunction2)
                        {

                            report = new SODReport();
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


                            lstreport.Add(report);
                        }
                    }






                }

                return lstreport;
            }

            private List<SODReport> ApplyException(List<SODReport> lstReport, SODException exception)
        {
            List<SODReport> ExceptionMatching = null;

            foreach (SodExceptionItem exItem in exception.ExceptionItems)
            {
                if (exItem.user != null)
                {
                    if (exItem.functionitem1 != null && exItem.functionitem2 != null)
                    {
                        ExceptionMatching = lstReport.Where(s => s.UserName == exItem.user.UserName && s.FunctionId1 == exItem.FunctionId1
                                                            && s.FunctionItem1 == exItem.FunctionItemId1 && s.FunctionItem2 == exItem.FunctionItemId2
                                                            && s.FunctionId2 == exItem.FunctionId2).ToList();
                    }
                    if (exItem.functionitem1 != null && exItem.functionitem2 == null)
                    {
                        ExceptionMatching = lstReport.Where(s => s.UserName == exItem.user.UserName && s.FunctionId1 == exItem.FunctionId1
                                                            && s.FunctionItem1 == exItem.FunctionItemId1).ToList();
                    }
                    else if (exItem.function1 != null && exItem.function2 != null)
                    {
                        ExceptionMatching = lstReport.Where(s => s.UserName == exItem.user.UserName && s.FunctionId1 == exItem.FunctionId1 && s.FunctionId2 == exItem.FunctionId2).ToList();
                    }
                    else if (exItem.function1 != null && exItem.function2 == null)
                    {
                        ExceptionMatching = lstReport.Where(s => s.UserName == exItem.user.UserName && s.FunctionId1 == exItem.FunctionId1).ToList();

                    }
                    else if (exItem.function1 == null && exItem.function2 == null)
                    {
                        ExceptionMatching = lstReport.Where(s => s.UserName == exItem.user.UserName).ToList();

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

            foreach (SODReport report in lstReport)
            {
                if (ExceptionMatching.Contains(report))
                {
                    report.ExceptionId = exception.Id;
                }


            }

            return lstReport;


        }



            private List<SODReport> CheckUsersWithSensitiveAccess(SODRunPolicy RunPolicy, SodFunction function1)
            {

                IList<string> uniqueUsersInFunction1 = new List<string>();

                List<UsersInRole> UsersinRoleFunction1 = GetAllUsersInsideFunction(RunPolicy, function1, ref uniqueUsersInFunction1);


                var listUsersinRoleFunction1 = new List<UsersInRole>();
                List<SODReport> lstreport = new List<SODReport>();

                SODReport report = null;
                foreach (var usrFn1 in UsersinRoleFunction1)
                {
                    if (usrFn1.Users != null)
                    {
                        foreach (string user in usrFn1.Users)
                        {
                            report = new SODReport();
                            report.Function1 = function1;
                            report.Role1 = usrFn1.RoleName;
                            report.UserName = user;
                            report.FunctionId1 = usrFn1.FunctionId;
                            report.FunctionItem1 = (int)usrFn1.FunctionItemId;
                            report.FunctionType1 = usrFn1.FunctionType;


                            lstreport.Add(report);
                        }
                    }

                }

                return lstreport;



            }


            private UsersInRole GetAllUsersInsideFunctionItem(SODRunPolicy RunPolicy, SodFunctionItem fItem, ref IList<string> UniqueUsers)
        {

            UsersInRole u = new UsersInRole();
            UserFacade usrDa = new UserFacade();

            List<string> UsersInSingleFunctionItem = new List<string>();
            List<string> Matched = RunPolicy.gblAttributes.Where(s => s == fItem.SodFunctionType).ToList();
            if ((Matched != null && Matched.Count > 0) || RunPolicy.ObjectChecked)
            {
                if (fItem.EndpointType.Id  == (int) EndpointType.ActiveDirectory)
                {
                    //int GroupId = Convert.ToInt16(fItem.SodRoleId);
                    List<ADSUserInfo> usrList1 = new ActiveDirectoryFacade().GetAllUsersByGroup(fItem.SodRoleId, fItem.Endpointsystemid.Value);

                    u.RoleName = fItem.SodRoleDesc;


                    foreach (ADSUserInfo userinfo in usrList1)
                    {
                        //UsersInSingleFunctionItem.Add(userinfo.displayName);
                        //UniqueUsers.Add(userinfo.displayName);
                        if (userinfo.DIUserId != null)
                        {
                            User usr = usrDa.GetUser(userinfo.DIUserId.Value);
                            UsersInSingleFunctionItem.Add(usr.DisplayName.Trim());
                            UniqueUsers.Add(usr.DisplayName.Trim());
                        }
                    }

                    u.Users = UsersInSingleFunctionItem;
                }

                else if (fItem.EndpointType.Id == (int)EndpointType.SAP)
                {
                    u.RoleName = fItem.SodRoleDesc;

                    if (fItem.SodFunctionType == "Role")
                    {
                        // Get all users linked to the role
                        List<SAPUserinfo> sapUsrList = new SAPFacade().GetAllUsersInARole(fItem.SodRoleDesc, fItem.Endpointsystemid.Value);
                        if (sapUsrList != null)
                        {
                            foreach (SAPUserinfo usrrole in sapUsrList)
                            {

                                if (usrrole.DIUserId != null)
                                {
                                    User usr = usrDa.GetUser(usrrole.DIUserId.Value);
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
                            //IList<SAPUserinfo> sapUsrList = new SAPFacade().GetAllUsersInATCode(fItem.SodRoleDesc, fItem.Endpointsystemid.Value);
                            IList<DomainModel.SAPUserinfo> sapUsrList = DomainModel.SAPUserinfo.GetAllUsersInATCode(fItem.SodRoleDesc, fItem.Endpointsystemid.Value);

                        foreach (DomainModel.SAPUserinfo usrrole in sapUsrList)
                        {
                            UsersInSingleFunctionItem.Add(usrrole.UserLogonName.Trim());
                            UniqueUsers.Add(usrrole.UserLogonName.Trim());
                        }


                    }
                    else if (fItem.SodFunctionType == "Profiles")
                    {
                        List<SAPUserinfo> sapUsrList = new SAPFacade().GetAllUsersInProfile(fItem.SodRoleDesc, fItem.Endpointsystemid.Value);
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
            //return UsersInSingleFunctionItem;
        }

        private List<UsersInRole> GetAllUsersInsideFunction(SODRunPolicy RunPolicy, SodFunction function, ref IList<string> UniqueUsersinsideFunction)
        {

            IList<string> uniqueUsers = new List<string>();
            IList<int> DIuniqueUsers = new List<int>();

            List<UsersInRole> UsersInAllRoles = new List<UsersInRole>();

            foreach (SodFunctionItem functionitem in function.SodFunctionItems)
            {

                UsersInRole u =  GetAllUsersInsideFunctionItem(RunPolicy, functionitem, ref uniqueUsers);

                u.FunctionItemId = functionitem.Id;
                u.FunctionId = function.Id;
                u.FunctionType = functionitem.SodFunctionType;


                UsersInAllRoles.Add(u);
                UniqueUsersinsideFunction = UniqueUsersinsideFunction.Union(uniqueUsers).ToList();


            }

            return UsersInAllRoles;
        }


        private List<UsersInRole> GetAllUsersInsideFunctionForSensitiveAccess(SODRunPolicy RunPolicy, SodFunction function, ref IList<string> UniqueUsersinsideFunction)
        {

            IList<string> uniqueUsers = new List<string>();
            IList<int> DIuniqueUsers = new List<int>();

            List<UsersInRole> UsersInAllRoles = new List<UsersInRole>();

            foreach (SodFunctionItem functionitem in function.SodFunctionItems)
            {

                UsersInRole u = GetAllUsersInsideFunctionItemForSensitiveAccess(RunPolicy, functionitem, ref uniqueUsers);

                u.FunctionItemId = functionitem.Id;
                u.FunctionId = function.Id;
                u.FunctionType = functionitem.SodFunctionType;


                UsersInAllRoles.Add(u);
                UniqueUsersinsideFunction = UniqueUsersinsideFunction.Union(uniqueUsers).ToList();



            }

            return UsersInAllRoles;
        }

        private UsersInRole GetAllUsersInsideFunctionItemForSensitiveAccess(SODRunPolicy RunPolicy, SodFunctionItem fItem, ref IList<string> UniqueUsers)
        {

            UsersInRole u = new UsersInRole();
            UserFacade usrDa = new UserFacade();

            List<string> UsersInSingleFunctionItem = new List<string>();
            List<string> Matched = RunPolicy.gblAttributes.Where(s => s == fItem.SodFunctionType).ToList();
            if ((Matched != null && Matched.Count > 0) || RunPolicy.ObjectChecked)
            {
                if (fItem.EndpointType.Id == (int)EndpointType.ActiveDirectory)
                {
                    //int GroupId = Convert.ToInt16(fItem.SodRoleId);
                    List<ADSUserInfo> usrList1 = new ActiveDirectoryFacade().GetAllUsersByGroup(fItem.SodRoleId, fItem.Endpointsystemid.Value);

                    u.RoleName = fItem.SodRoleDesc;


                    foreach (ADSUserInfo userinfo in usrList1)
                    {
                        if (userinfo.DIUserId != null)
                        {
                            User usr = usrDa.GetUser(userinfo.DIUserId.Value);
                            UsersInSingleFunctionItem.Add(usr.DisplayName.Trim());
                            UniqueUsers.Add(usr.DisplayName.Trim());
                        }
                    }

                    u.Users = UsersInSingleFunctionItem;
                }
                else if (fItem.EndpointType.Id == (int)EndpointType.LotusNotes)
                {
                    IList<LotusUserInfo> usrList1 = new LotusNotesFacade().GetLotusNotesUsersByGroupName(fItem.SodRoleId, fItem.Endpointsystemid.Value);

                    u.RoleName = fItem.SodRoleDesc;


                    foreach (LotusUserInfo userinfo in usrList1)
                    {
                        if (userinfo.DIUserId != null)
                        {
                            User usr = usrDa.GetUser(userinfo.DIUserId.Value);
                            UsersInSingleFunctionItem.Add(usr.DisplayName.Trim());
                            UniqueUsers.Add(usr.DisplayName.Trim());
                        }
                    }

                    u.Users = UsersInSingleFunctionItem;
                }

                u.Users = UsersInSingleFunctionItem;

            }
            else if (fItem.EndpointType.Id == (int)EndpointType.SAP)
            {
                u.RoleName = fItem.SodRoleDesc;

                if (fItem.SodFunctionType == EntitlementType.Role.ToString())
                {
                    // Get all users linked to the role
                    List<SAPUserinfo> sapUsrList = new SAPFacade().GetAllUsersInARole(fItem.SodRoleDesc, fItem.Endpointsystemid.Value);
                    if (sapUsrList != null)
                    {
                        foreach (SAPUserinfo usrrole in sapUsrList)
                        {

                            if (usrrole.DIUserId != null)
                            {
                                User usr = usrDa.GetUser(usrrole.DIUserId.Value);
                                if (usr != null)
                                {
                                    UniqueUsers.Add(usr.DisplayName.Trim());
                                    UsersInSingleFunctionItem.Add(usr.DisplayName.Trim());
                                }
                            }
                        }
                    }

                }
                else if (fItem.SodFunctionType == EntitlementType.TCodes.ToString())
                {

                        // get all users linked to the tcode
                        //IList<SAPUserinfo> sapUsrList = new List<SAPUserinfo>();
                        IList <DomainModel.SAPUserinfo> sapUsrList = new List<DomainModel.SAPUserinfo>();
                    if (fItem.ObjectCode == "")
                    {
                        sapUsrList =DomainModel.SAPUserinfo.GetAllUsersInATCode(fItem.SodRoleDesc, fItem.Endpointsystemid.Value);
                    }
                    else
                    {
                            sapUsrList = DomainModel.SAPUserinfo.GetAllUsersInObject(fItem.SodRoleDesc, fItem.ObjectCode, fItem.FieldCode, Convert.ToString(fItem.Endpointsystemid.Value));
                             //   new SAPFacade().GetAllUsersInObject(fItem.SodRoleDesc, fItem.ObjectCode, fItem.FieldCode, Convert.ToString(fItem.Endpointsystemid.Value));
                    }
                    foreach (DomainModel.SAPUserinfo usrrole in sapUsrList)
                    {
                        UsersInSingleFunctionItem.Add(usrrole.UserLogonName.Trim());
                        UniqueUsers.Add(usrrole.UserLogonName.Trim());
                    }


                }
                else if (fItem.SodFunctionType == EntitlementType.Profile.ToString())
                {
                    List<SAPUserinfo> sapUsrList = new SAPFacade().GetAllUsersInProfile(fItem.SodRoleDesc, fItem.Endpointsystemid.Value);
                    foreach (SAPUserinfo usrrole in sapUsrList)
                    {
                        UsersInSingleFunctionItem.Add(usrrole.UserLogonName.Trim());
                        UniqueUsers.Add(usrrole.UserLogonName.Trim());
                    }
                }


                u.Users = UsersInSingleFunctionItem;
            }


            return u;
        }
    }
    */
}
