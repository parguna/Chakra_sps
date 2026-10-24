using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SODPolicyReport : DomainModelBase<SODPolicyReport, EntityModel.SODPolicyReport>
    {
        public Nullable<long> PolicyExecutionDefinitionId { get; set; }

        public string UniqueRunCode { get; set; }

        public DateTime RunDate { get; set; }
        public Nullable<long> FunctionId1 { get; set; }

        public string Role1 { get; set; }

        public Nullable<long> FunctionItem1 { get; set; }

        public string FunctionType1 { get; set; }

        public Nullable<long> FunctionId2 { get; set; }

        public string Role2 { get; set; }

        public Nullable<long> FunctionItem2 { get; set; }

        public string FunctionType2 { get; set; }

        public string AuthorizationCode1 { get; set; }

        public string AuthorizationCode2 { get; set; }

        public string ProfileCode1 { get; set; }

        public string ProfileCode2 { get; set; }

        public string RoleCode1 { get; set; }

        public string RoleCode2 { get; set; }

        public Nullable<long> ConflictId { get; set; }

        public string ConflictCode { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }

        public string RunCode { get; set; }

        public bool PassedObjectLevelLeft { get; set; }

        public bool PassedObjectLevelRight { get; set; }

        public int TotalMatching { get; set; }

        public string MitigationCode { get; set; }

        public Nullable<long> SODRunPolicyId { get; set; }

        public long? ExceptionId { get; set; }

        public SODFunction Function1 { get; set; }

        public SODFunction Function2 { get; set; }

        public SODFunctionItem FunctionObjItem1 { get; set; }

        public SODFunctionItem FunctionObjItem2 { get; set; }

        public SODConflictFunctions Conflict { get; set; }

        public SODException exception { get; set; }

        public SODRunPolicy RunPolicy { get; set; }

        public SODPolicyExecutionDefinition PolicyDefinition { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public static void Save(List<SODPolicyReport> sODPolicyReportList, string uniqueRunCode, string currentUser)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                foreach (SODPolicyReport report in sODPolicyReportList)
                {
                    report.UniqueRunCode = uniqueRunCode;
                    report.PolicyDefinition = null;
                    report.Conflict = null;
                    report.exception = null;
                    report.Function1 = null;
                    report.Function2 = null;
                    report.FunctionObjItem1 = null;
                    report.FunctionObjItem2 = null;
                    report.Save(transaction);
                }
                transaction.Commit();
            }         
        }
        public static List<string> GetUniqueRunCodes()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new SODPolicyReportRepository(dataAccessTransaction).GetUniqueRunCodes();
            }
        }
        public static string GetObjectCode(long functionItem1)
        {
            SODFunctionItem item = SODFunctionItem.GetById(functionItem1, true);
            return item.SodRoleDesc;
        }

        public static string GetFieldValue(string Value, string authCode, string objectCode, string fieldCode)
        {
               string MatchString = string.Empty;
               SAPRoleToAuthMap roletoauthmap = SAPRoleToAuthMap.GetByAuthCode(authCode,objectCode,fieldCode);

                    if (roletoauthmap != null)
                    {
                        SAPFieldValueMapping fieldvalue = SAPFieldValueMapping.GetByFieldCode(authCode,objectCode,fieldCode);
                        MatchString = fieldvalue.FromValue;
                    }

                return MatchString;
       }
        public static List<SODPolicyReport> GetBySODRunPolicyIdIncludeSodFunctionItem(string UniqueRunCode)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SODPolicyReport>(tranaction)
                    .Get(rept => rept.UniqueRunCode == UniqueRunCode,
                    rept => rept.Function1,
                    rept => rept.Function2,
                    rept => rept.FunctionObjItem1,
                    rept => rept.FunctionObjItem1.EndpointType,
                    rept => rept.FunctionObjItem1.endpoint,
                    rept => rept.FunctionObjItem2,
                    rept => rept.FunctionObjItem2.EndpointType,
                    rept => rept.FunctionObjItem2.endpoint,
                    rept => rept.Conflict,
                    rept => rept.Conflict.SodRiskCode,
                    rept => rept.exception,
                    rept => rept.PolicyDefinition,
                    rept => rept.PolicyDefinition.CompliancePolicy).ToList());
            }
        }
        public static bool IsObjectValueEqual(string AuthCode, string ObjectCode, string FieldCode, string Value1, string Value2)
        {
                Value1 = Value1.TrimStart('0');
                Value2 = Value2.TrimStart('0');

                if (Value1[0] == '$')
                {
                    Value1 = GetFieldValue(Value1, AuthCode, ObjectCode, FieldCode);
                }

                if (Value2[0] == '$')
                {
                    Value2 = GetFieldValue(Value2, AuthCode, ObjectCode, FieldCode);
                }

                bool ReturnVal = true;

                if (Value1 != "*" && Value2 != "*")
                {
                    if (Value1 != Value2) ReturnVal = false;
                }


                return ReturnVal;
        }

        public static List<SODPolicyReport> CheckConflictAtObjectLevel(List<SODPolicyReport> sODPolicyReportList)
        {
                List<SODAuthorizationReport> lstLeftAuthorizations = new List<SODAuthorizationReport>();
                List<SODAuthorizationReport> lstRightAuthorizations = new List<SODAuthorizationReport>();

                foreach (SODPolicyReport report in sODPolicyReportList)
                {
                    if (report.FunctionType1 == "TCodes" && report.FunctionType2 == "TCodes" && report.UserName != string.Empty)
                    {
                        SODFunctionItem FunctionItem1 = SODFunctionItem.GetById(report.FunctionItem1.Value,true);
                        SODFunctionItem FunctionItem2 = SODFunctionItem.GetById(report.FunctionItem2.Value,true);

                        string ObjectCode1 = FunctionItem1.ObjectCode;
                        string FieldCode1 = FunctionItem1.FieldCode;
                        string FieldValue1 = FunctionItem1.FromValue;

                        string ObjectCode2 = FunctionItem2.ObjectCode;
                        string FieldCode2 = FunctionItem2.FieldCode;
                        string FieldValue2 = FunctionItem2.FromValue;

                        if (ObjectCode1 != string.Empty && ObjectCode2 != string.Empty)
                        {

                            List<string> SapAuthObjects1 = SAPAuthorizationToObject.GetByFromValue(FunctionItem1.SodRoleDesc, FunctionItem1.Endpointsystemid.Value);
                            List<string> SapAuthObjects2 = SAPAuthorizationToObject.GetByFromValue(FunctionItem2.SodRoleDesc, FunctionItem2.Endpointsystemid.Value);
                            if (SapAuthObjects1 == null || SapAuthObjects2 == null)
                            {
                                foreach (SODPolicyReport r in sODPolicyReportList)
                                {
                                    if (SODPolicyReport.GetObjectCode(r.FunctionItem1.Value) == ObjectCode1 || GetObjectCode(r.FunctionItem2.Value) == ObjectCode2)
                                    {
                                        sODPolicyReportList.Remove(r);
                                    }
                                }
                            }
                            else
                            {
                                bool FieldFound1 = false, FieldFound2 = false;

                                #region Left Matching

                                foreach (string AuthCode in SapAuthObjects1)
                                {

                                    List<SAPAuthorizationToObject> lstAuthorizeObj = SAPAuthorizationToObject.GetByObjectAuthCode((long)report.FunctionObjItem1.Endpointsystemid.Value, AuthCode).ToList(); //, ObjectCode2, FieldCode2);
                                    List<SAPAuthorizationToObject> MatchingObject = new List<SAPAuthorizationToObject>();
                                    bool ItemFieldMatch = false;

                                    if (lstAuthorizeObj != null)
                                    {

                                        foreach (SAPAuthorizationToObject AuthorizeObj in lstAuthorizeObj)
                                        {
                                            AuthorizeObj.FromValue = AuthorizeObj.FromValue.TrimStart('0');
                                            List<SODFunctionItem> items = report.Function1.SodFunctionItems.Where(s => s.ObjectCode == AuthorizeObj.ObjectCode && s.FieldCode == AuthorizeObj.FieldCode).ToList();
                                            foreach (SODFunctionItem item in items)
                                            {
                                                //if (item.FromValue == AuthorizeObj.FromValue)
                                                if (IsObjectValueEqual(AuthorizeObj.AuthorizationCode, AuthorizeObj.ObjectCode, AuthorizeObj.FieldCode, item.FromValue, AuthorizeObj.FromValue))
                                                {
                                                    MatchingObject.Add(AuthorizeObj);
                                                    if (report.FunctionObjItem1.ObjectCode == AuthorizeObj.ObjectCode &&
                                                        report.FunctionObjItem1.FieldCode == AuthorizeObj.FieldCode &&
                                                        report.FunctionObjItem1.FromValue == AuthorizeObj.FromValue)
                                                        ItemFieldMatch = true;
                                                }

                                            }



                                        }

                                        FieldFound1 = true;

                                        foreach (SODFunctionItem item in report.Function1.SodFunctionItems)
                                        {
                                            SAPAuthorizationToObject obj = SAPAuthorizationToObject.GetByObjectCode(item.ObjectCode, item.FieldCode);
                                            if (obj == null)
                                                FieldFound1 = false;
                                        }

                                        if (FieldFound1)
                                        {
                                            if (ItemFieldMatch)
                                            {
                                                report.AuthorizationCode1 = AuthCode;
                                                SODAuthorizationReport AuthReport = new SODAuthorizationReport();
                                                AuthReport.SODReportId = report.Id;
                                                AuthReport.LeftOrRight = 1;
                                                AuthReport.AuthorizationCode = AuthCode;
                                                lstLeftAuthorizations.Add(AuthReport);
                                            }
                                            report.PassedObjectLevelLeft = true;
                                        }
                                    }



                                }
                                #endregion

                                #region Right Matching
                                foreach (string AuthCode in SapAuthObjects2)
                                {
                                    List<SAPAuthorizationToObject> lstAuthorizeObj = SAPAuthorizationToObject.GetByObjectAuthCode((long)report.FunctionObjItem2.Endpointsystemid.Value, AuthCode).ToList(); //, ObjectCode2, FieldCode2);
                                    List<SAPAuthorizationToObject> MatchingObject = new List<SAPAuthorizationToObject>();
                                    bool ItemFieldMatch = false;

                                    if (lstAuthorizeObj != null)
                                    {

                                        foreach (SAPAuthorizationToObject AuthorizeObj in lstAuthorizeObj)
                                        {
                                            AuthorizeObj.FromValue = AuthorizeObj.FromValue.TrimStart('0');
                                            List<SODFunctionItem> items = report.Function2.SodFunctionItems.Where(s => s.ObjectCode == AuthorizeObj.ObjectCode && s.FieldCode == AuthorizeObj.FieldCode).ToList();
                                            foreach (SODFunctionItem item in items)
                                            {
                                                //if (item.FromValue == AuthorizeObj.FromValue)
                                                if (IsObjectValueEqual(AuthorizeObj.AuthorizationCode, AuthorizeObj.ObjectCode, AuthorizeObj.FieldCode, item.FromValue, AuthorizeObj.FromValue))
                                                {
                                                    MatchingObject.Add(AuthorizeObj);
                                                    if (report.FunctionObjItem2.ObjectCode == AuthorizeObj.ObjectCode &&
                                                        report.FunctionObjItem2.FieldCode == AuthorizeObj.FieldCode &&
                                                        report.FunctionObjItem2.FromValue == AuthorizeObj.FromValue)
                                                        ItemFieldMatch = true;
                                                }

                                            }



                                        }

                                        FieldFound2 = true;

                                        foreach (SODFunctionItem item in report.Function2.SodFunctionItems)
                                        {
                                            SAPAuthorizationToObject obj = SAPAuthorizationToObject.GetByObjectCode(item.ObjectCode, item.FieldCode);
                                            if (obj == null)
                                                FieldFound2 = false;
                                        }

                                        if (FieldFound2)
                                        {
                                            if (ItemFieldMatch)
                                            {
                                                report.AuthorizationCode2 = AuthCode;
                                                SODAuthorizationReport AuthReport = new SODAuthorizationReport();
                                                AuthReport.SODReportId = report.Id;
                                                AuthReport.LeftOrRight = 2;
                                                AuthReport.AuthorizationCode = AuthCode;
                                                lstRightAuthorizations.Add(AuthReport);


                                            }
                                            report.PassedObjectLevelRight = true;
                                        }
                                    }



                                }

                                #endregion

                                #region Removing Similiar Non Conflict Items

                                if (report.PassedObjectLevelLeft == false || report.PassedObjectLevelRight == false)
                                {
                                    sODPolicyReportList.Where(s => s.FunctionId1 == report.FunctionId1 &&
                                                                                            s.FunctionItem1 == report.FunctionItem1 &&
                                                                                            s.FunctionId2 == report.FunctionId2 &&
                                                                                            s.FunctionItem2 == report.FunctionItem2 &&
                                                                                            s.UserName != string.Empty).ToList()
                                                                                            .ForEach(s => s.UserName = string.Empty);
                                }
                                #endregion

                            }
                        }
                    }
                }

                foreach (SODPolicyReport report in sODPolicyReportList)
                {
                    if (report.PassedObjectLevelLeft == false || report.PassedObjectLevelRight == false)
                    {
                        if (report.PassedObjectLevelLeft == false)
                        {
                            List<SODPolicyReport> lstMatchingFieldCode = sODPolicyReportList.Where(s => s.Role1 == report.Role1 && s.Role2 == report.Role2 &&
                                                                                s.FunctionObjItem1.ObjectCode == report.FunctionObjItem1.ObjectCode &&
                                                                                s.FunctionObjItem1.FieldCode == report.FunctionObjItem1.FieldCode).ToList();

                            if (lstMatchingFieldCode != null && lstMatchingFieldCode.Count > 1)
                            {
                                foreach (SODPolicyReport SodMatchReport in lstMatchingFieldCode)
                                {
                                    if (!SodMatchReport.PassedObjectLevelLeft)
                                    {
                                        report.PassedObjectLevelLeft = true;
                                        break;
                                    }
                                }
                            }

                        }

                        if (report.PassedObjectLevelRight == false)
                        {
                            List<SODPolicyReport> lstMatchingFieldCode = sODPolicyReportList.Where(s => s.Role1 == report.Role1 && s.Role2 == report.Role2 &&
                                                                                s.FunctionObjItem2.ObjectCode == report.FunctionObjItem2.ObjectCode &&
                                                                                s.FunctionObjItem2.FieldCode == report.FunctionObjItem2.FieldCode).ToList();

                            if (lstMatchingFieldCode != null && lstMatchingFieldCode.Count > 1)
                            {
                                foreach (SODPolicyReport SodMatchReport in lstMatchingFieldCode)
                                {
                                    if (!SodMatchReport.PassedObjectLevelRight)
                                    {
                                        report.PassedObjectLevelRight = true;
                                        break;
                                    }
                                }
                            }

                        }

                        if (report.PassedObjectLevelLeft == false || report.PassedObjectLevelRight == false)
                        {
                            List<SODPolicyReport> lstMatchingReports = sODPolicyReportList.Where(s => s.FunctionId1 == report.FunctionId1 && s.FunctionId2 == report.FunctionId2).ToList();
                            foreach (SODPolicyReport MatchReport in lstMatchingReports)
                            {

                                MatchReport.UserName = string.Empty;

                            }
                        }
                    }
                }



                sODPolicyReportList = sODPolicyReportList.Where(s => s.UserName != string.Empty && s.AuthorizationCode1 != string.Empty && s.AuthorizationCode2 != string.Empty).ToList();

                List<SODPolicyReport> lstNewReport = new List<SODPolicyReport>();

                foreach (SODPolicyReport report1 in sODPolicyReportList)
                {
                    List<SODAuthorizationReport> lstMatchAuthLeft = lstLeftAuthorizations.Where(s => s.SODReportId == report1.Id).ToList();
                    List<SODAuthorizationReport> lstMatchAuthRight = lstRightAuthorizations.Where(s => s.SODReportId == report1.Id).ToList();

                    foreach (SODAuthorizationReport leftAuthReport in lstMatchAuthLeft)
                    {
                        foreach (SODAuthorizationReport rightAuthReport in lstMatchAuthRight)
                        {
                            SODPolicyReport reportnew = new SODPolicyReport();
                            reportnew.AuthorizationCode1 = leftAuthReport.AuthorizationCode;
                            reportnew.AuthorizationCode2 = rightAuthReport.AuthorizationCode;

                            lstNewReport.Add(reportnew);
                        }
                    }
                }

                foreach (SODPolicyReport report in lstNewReport)
                {
                    if (report.ProfileCode1 == null || report.ProfileCode1 == string.Empty)
                    {

                        List<SAPAuthorizationProfile> AuthProf1 = SAPAuthorizationProfile.GetByAuthcode(report.AuthorizationCode1).ToList();
                        SAPRoleToAuthMap AuthRole1 = SAPRoleToAuthMap.GetByAuthCode(report.AuthorizationCode1);

                        if (AuthProf1 != null)
                        {
                            report.ProfileCode1 = AuthProf1.Select(x=>x.ProfileCode).FirstOrDefault();

                            if (AuthRole1 != null)
                                report.RoleCode1 = AuthRole1.RoleName;

                            foreach (SODPolicyReport r1 in lstNewReport)
                            {
                                if (r1.AuthorizationCode1 == report.AuthorizationCode1)
                                {
                                    r1.ProfileCode1 = report.ProfileCode1;
                                    r1.RoleCode1 = report.RoleCode1;
                                }

                                if (r1.AuthorizationCode2 == report.AuthorizationCode1)
                                {
                                    r1.ProfileCode2 = report.ProfileCode1;
                                    r1.RoleCode2 = report.RoleCode1;
                                }
                            }
                        }
                    }

                    if (report.ProfileCode2 == null || report.ProfileCode2 == string.Empty)
                    {


                        List<SAPAuthorizationProfile> AuthProf2 = SAPAuthorizationProfile.GetByAuthcode(report.AuthorizationCode2).ToList();
                      
                        SAPRoleToAuthMap AuthRole2 = SAPRoleToAuthMap.GetByAuthCode(report.AuthorizationCode2);


                        if (AuthProf2 != null)
                        {
                            report.ProfileCode2 = AuthProf2.Select(x=>x.ProfileCode).FirstOrDefault();
                            if (AuthRole2 != null)
                                report.RoleCode2 = AuthRole2.RoleName;


                            foreach (SODPolicyReport r1 in lstNewReport)
                            {
                                if (r1.AuthorizationCode1 == report.AuthorizationCode2)
                                {
                                    r1.ProfileCode1 = report.ProfileCode2;
                                    r1.RoleCode1 = report.RoleCode2;
                                }

                                if (r1.AuthorizationCode2 == report.AuthorizationCode2)
                                {
                                    r1.ProfileCode2 = report.ProfileCode2;
                                    r1.RoleCode2 = report.RoleCode2;
                                }
                            }
                        }
                    }

                }
                return lstNewReport;
            }
    }
}
