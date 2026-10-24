using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Web.UsrControl
{
    public partial class UserSODConflicts : System.Web.UI.UserControl
    {
        private const string V_SODConflict = "V_SODConflict";
        private const string V_USERID = "V_USERID";
        private const string V_REQUESTID = "V_REQUESTID";
        private const string V_REQUESTEEID= "V_REQUESTEEID";
        private const string V_ENTITLEMENTCONFLICT = "V_ENTITLEMENTCONFLICT";
        private const string V_SHOWNEWCONFLICTALONE = "V_SHOWNEWCONFLICTALONE";
        private const string V_LOADFROMINTERIMSTORE = "V_LOADFROMINTERIMSTORE";
        private const string V_ISFROMACCESSREQUEST = "V_ISFROMACCESSREQUEST";
        private const string V_CANSHOWLINK = "V_CANSHOWLINK";
        private const string V_CANLOADACCESSREQITEMS = "V_CANLOADACCESSREQITEMS";
        bool isUserChanged; bool isRequestIdChanged;

        public List<UserSODComplianceReport> report = new List<UserSODComplianceReport>();
        public bool isFromAccessRequest
        {
            get
            {
                if (ViewState[V_ISFROMACCESSREQUEST] == null)
                    return false;
                return (bool)ViewState[V_ISFROMACCESSREQUEST];
            }
            set
            {
                ViewState[V_ISFROMACCESSREQUEST] = value;
            }
        }

        public bool canLoadAccessReqItems
        {
            get
            {
                if (ViewState[V_CANLOADACCESSREQITEMS] == null)
                    return false;
                return (bool)ViewState[V_CANLOADACCESSREQITEMS];
            }
            set
            {
                ViewState[V_CANLOADACCESSREQITEMS] = value;

            }
        }

        public bool isDenyRequest { get; set; }

        public bool canShowLink
        {
            get
            {
                if (ViewState[V_CANSHOWLINK] == null)
                    return false;
                return (bool)ViewState[V_CANSHOWLINK];
            }
            set
            {
                ViewState[V_CANSHOWLINK] = value;
            }
        }

        public List<UserSODComplianceReport> UserSODAndSAConflicts
        {
            get
            {
                if (LoadFromInterimstore || isRequestIdChanged || isUserChanged ||
                        (ViewState[V_SODConflict] == null && (UserId > 0 || RequestId > 0 )))
                {
                    UserSODAndSAConflicts = new List<UserSODComplianceReport>();
                    isRequestIdChanged = false; isUserChanged = false;
                    try
                    {
                        //store the conflict info for this user
                        UserSODComplianceRequest request = new UserSODComplianceRequest(); request.PolicyId = 0;
                        request.UserId = UserId;
                        request.RequesteeUserId = RequesteeId;
                        request.IsFromAccessRequest = LoadFromInterimstore;
                        request.AccessRequestId = RequestId;
                        var conflictRoles = DomainApiClient<UserSODComplianceReport>.GetByAction<List<UserSODComplianceReport>>(DomainApiAction.Complaince.CompliancePolicy.Policy.GetUserSODComplianceReport, null, request);
                        UserSODAndSAConflicts = conflictRoles;
                        report = conflictRoles;

                    }
                    catch (Exception ex)
                    {
                        UserSODAndSAConflicts = new List<UserSODComplianceReport>();
                    }
                }
                return (List<UserSODComplianceReport>)ViewState[V_SODConflict];
               
            }
            set
            {
                ViewState[V_SODConflict] = value;
            }
        }        
        public long UserId
        {
            get
            {
                if (ViewState[V_USERID] == null)
                    return 0;
                return (long)ViewState[V_USERID];

            }
            set
            {
                if (UserId != value)
                    isUserChanged = true;

                ViewState[V_USERID] = value;
            }
        }

        public long RequesteeId
        {
            get
            {
                if (ViewState[V_REQUESTEEID] == null)
                    return 0;
                return (long)ViewState[V_REQUESTEEID];
            }
            set
            {
                ViewState[V_REQUESTEEID] = value;
            }
        }
        public bool ShowNewConflictAlone
        {
            get
            {
                if (ViewState[V_SHOWNEWCONFLICTALONE] == null)
                    return false;
                return (bool)ViewState[V_SHOWNEWCONFLICTALONE];

            }
            set
            {
                ViewState[V_SHOWNEWCONFLICTALONE] = value;
            }
        }
        public bool LoadFromInterimstore
        {
            get
            {
                if (ViewState[V_LOADFROMINTERIMSTORE] == null)
                    return false;
                return (bool)ViewState[V_LOADFROMINTERIMSTORE];

            }
            set
            {
                ViewState[V_LOADFROMINTERIMSTORE] = value;
            }
        }
        public string EntitlementConflict
        {
            get
            {
                if (ViewState[V_ENTITLEMENTCONFLICT] == null)
                    return "";
                return (string)ViewState[V_ENTITLEMENTCONFLICT];

            }
            set
            {
                ViewState[V_ENTITLEMENTCONFLICT] = value;
            }
        }
        public long RequestId
        {
            get
            {
                if (ViewState[V_REQUESTID] == null)
                    return 0;
                return (long)ViewState[V_REQUESTID];

            }
            set
            {                
                if (RequestId != value)
                    isRequestIdChanged = true;
                ViewState[V_REQUESTID] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }



        private List<UserSODComplianceReport> getFunctions(string riskCode)
        {
            List<UserSODComplianceReport> list = new List<UserSODComplianceReport>();
            if (isFromAccessRequest)
            {
                if (UserSODAndSAConflicts == null) return new List<UserSODComplianceReport>();
              
                var dataSource = UserSODAndSAConflicts;

                if (!string.IsNullOrEmpty(EntitlementConflict))
                    dataSource = dataSource.Where(a => a.ConflictEntitlement1 == EntitlementConflict || a.ConflictEntitlement2 == EntitlementConflict).ToList();
                if (ShowNewConflictAlone)
                    dataSource = dataSource.Where(a => a.Type == "New").ToList();

                var distinctFns = (from r in dataSource
                                   where r.Risk == riskCode
                                   select new { Function1 = r.Function1Name, Function2 = r.Function2Name }).ToList().Distinct();



                foreach (var item in distinctFns)
                {
                    list.Add(dataSource.Where(t => t.Function1Name == item.Function1 && t.Function2Name == item.Function2).FirstOrDefault());
                }
            }
            else {
                if (!isFromAccessRequest)
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("userId", UserId.ToString());
                    List<UserSODComplianceReport> userSODReport = DomainApiClient<UserSODComplianceReport>.GetByAction<List<UserSODComplianceReport>>(DomainApiAction.SODFunction.UserSODComplianceReport.GetReportbyUserId, parameter);
                    var dbdataSource = userSODReport;
                    var dbdistinctFns = (from r in dbdataSource
                                         where r.Risk == riskCode
                                         select new { Function1 = r.Function1Name, Function2 = r.Function2Name }).ToList().Distinct();


                    foreach (var item in dbdistinctFns)
                    {
                        list.Add(dbdataSource.Where(t => t.Function1Name == item.Function1 && t.Function2Name == item.Function2).FirstOrDefault());
                    }
                }
            }
            return list;
        }
        public List<UserSODComplianceReport> GetRisks()
        {
            if (UserSODAndSAConflicts == null) return new List<UserSODComplianceReport>();

            var dataSource = UserSODAndSAConflicts;

            if (!string.IsNullOrEmpty(EntitlementConflict))
                dataSource = dataSource.Where(a => a.ConflictEntitlement1 == EntitlementConflict || a.ConflictEntitlement2 == EntitlementConflict).ToList();
            if (ShowNewConflictAlone)
                dataSource = dataSource.Where(a => a.Type == "New").ToList();

            var distinctRiskIds = dataSource.Select(a => new { a.Risk, a.isFromAccessRequest }).Distinct().ToList();

            List<UserSODComplianceReport> list = new List<UserSODComplianceReport>();

            foreach (var item in distinctRiskIds)
            {
                list.Add(dataSource.Where(t => t.Risk == item.Risk && t.isFromAccessRequest==item.isFromAccessRequest).FirstOrDefault());
            }

            return list;
        }
        private List<UserSODComplianceReport> getFunctionDetails(string function1, string function2)
        {
            List<UserSODComplianceReport> list = new List<UserSODComplianceReport>();
            if (isFromAccessRequest)
            {
                if (UserSODAndSAConflicts == null) return new List<UserSODComplianceReport>();
                var dataSource = UserSODAndSAConflicts;

                if (!string.IsNullOrEmpty(EntitlementConflict))
                    dataSource = dataSource.Where(a => a.ConflictEntitlement1 == EntitlementConflict || a.ConflictEntitlement2 == EntitlementConflict).ToList();
                if (ShowNewConflictAlone)
                    dataSource = dataSource.Where(a => a.Type == "New").ToList();

                var distinctFnDetails = (from r in dataSource
                                         where r.Function1Name == function1 && r.Function2Name == function2
                                         select new { entitlement1 = r.ConflictEntitlement1, entitlement2 = r.ConflictEntitlement2 }).ToList().Distinct();



                foreach (var item in distinctFnDetails)
                {
                    list.Add(dataSource.Where(t => t.ConflictEntitlement1 == item.entitlement1 && t.ConflictEntitlement2 == item.entitlement2).FirstOrDefault());
                }
            }
            else
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("userId", UserId.ToString());
                List<UserSODComplianceReport> userSODReport = DomainApiClient<UserSODComplianceReport>.GetByAction<List<UserSODComplianceReport>>(DomainApiAction.SODFunction.UserSODComplianceReport.GetReportbyUserId, parameter);
                var dbdataSource = userSODReport;
                var distinctFnDetails = (from r in dbdataSource
                                         where r.Function1Name == function1 && r.Function2Name == function2
                                         select new { entitlement1 = r.ConflictEntitlement1, entitlement2 = r.ConflictEntitlement2 }).ToList().Distinct();

                foreach (var item in distinctFnDetails)
                {
                    list.Add(dbdataSource.Where(t => t.ConflictEntitlement1 == item.entitlement1 && t.ConflictEntitlement2 == item.entitlement2).FirstOrDefault());
                }
            }
            return list;
        }


        protected void SODConflict_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            return;
            LoadData();
           
        }
        public void LoadData()
        {
            if (isFromAccessRequest)
            {
                var dataSource = GetRisks();
                if (!string.IsNullOrEmpty(EntitlementConflict))
                    dataSource = dataSource.Where(a => a.ConflictEntitlement1 == EntitlementConflict || a.ConflictEntitlement2 == EntitlementConflict).ToList();
                if (ShowNewConflictAlone)
                    dataSource = dataSource.Where(a => a.Type == "New").ToList();
                SODConflict.DataSource = dataSource;
            }
            else
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("userId", UserId.ToString());
                List<UserSODComplianceReport> userSODReport = DomainApiClient<UserSODComplianceReport>.GetByAction<List<UserSODComplianceReport>>(DomainApiAction.SODFunction.UserSODComplianceReport.GetReportbyUserId, parameter);
                
                var distinctRiskIds = userSODReport.Select(a => new { a.Risk, a.isFromAccessRequest }).Distinct().ToList();
                List<UserSODComplianceReport> lst = new List<UserSODComplianceReport>();
                List<UserSODComplianceReport> list = new List<UserSODComplianceReport>();
                foreach (var item in distinctRiskIds)
                {
                    list.Add(userSODReport.Where(t => t.Risk == item.Risk && t.isFromAccessRequest==item.isFromAccessRequest).FirstOrDefault());
                }
                if (canLoadAccessReqItems)
                    list = list.Where(x => x.isFromAccessRequest == true||x.isFromAccessRequest==false).ToList();
                else
                    list = list.Where(x => x.isFromAccessRequest == false).ToList();

                string[] entitlments = EntitlementConflict.Split(',');
                
                foreach(string entitlement in  entitlments)
                {
                    var item = list.Where(x => x.ConflictEntitlement1 == entitlement || x.ConflictEntitlement2 == entitlement).FirstOrDefault();
                    if(item!=null)
                        if(!(lst.Where(x=>x.Id==item.Id).Count()>0))
                    lst.Add(item);
                }
                if (!string.IsNullOrEmpty(EntitlementConflict))
                {
                   
                    SODConflict.DataSource = lst;
                    if (lst.Count > 0)
                        canShowLink = true;
                    else
                        canShowLink = false;
                    report = lst;
                }
                else
                {
                    SODConflict.DataSource = list;
                    if (list.Count > 0)
                        canShowLink = true;
                    else
                        canShowLink = false;
                    report = list;
                }
            }
        }

        public void BindConflicts()
        {
            SODConflict.Rebind();
        }

        protected void SODConflict_DetailTableDataBind(object sender, Telerik.Web.UI.GridDetailTableDataBindEventArgs e)
        {
            GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;

            switch (e.DetailTableView.Name)
            {
                case "Functions":
                    {
                        e.DetailTableView.DataSource = getFunctions(dataItem["Risk"].Text.ToString());
                        break;
                    }
                case "FunctionDetails":
                    {
                        e.DetailTableView.DataSource = getFunctionDetails(dataItem.GetDataKeyValue("Function1Name").ToString(), dataItem.GetDataKeyValue("Function2Name").ToString());
                        break;
                    }
            }
        }


       
    }
}