using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Services;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AutoCompleteDataService : IAutoCompleteDataService
    {
        SessionStatusImpl sessionStatus = new SessionStatusImpl();

        public AutoCompleteBoxData GetAllReportMenu(Dictionary<string, object> context)
        {
            try
            {
                List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
                string[] EntryValues = null;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("searchString", context["Text"].ToString());
                Dictionary<long, string> data = DomainApiClient<Dto.TabMenu>.GetByAction<Dictionary<long, string>>(DomainApiAction.ACL.TabMenu.GetAllReportMenu, parameter);

                foreach (KeyValuePair<long, string> item in data)
                {
                    AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                    childNode.Text = item.Value;
                    childNode.Value = item.Key.ToString();
                    result.Add(childNode);
                }

                AutoCompleteBoxData res = new AutoCompleteBoxData();
                if (EntryValues != null)
                {
                    res.Items = (result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray());
                }
                else
                {
                    res.Items = result.OrderBy(m => m.Text).ToArray();
                }
                return res;
            }
            catch
            {
                throw;
            }
        }

        #region SRM

        public AutoCompleteBoxData GetSRMBusinesRoles(Dictionary<string, object> context)
        {
            var SRMUserName = context["SRMUserName"].ToString();
            var userid = context["SRMUserId"].ToString();
            var accounType = Convert.ToInt16(context["SRMAccountType"].ToString());
            var adhocActionType = Convert.ToInt32(context["SRMActionType"].ToString());
            var EndpointId = context["EndpointId"].ToString();

            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            Dictionary<string, string> data = DomainApiClient<Dto.SAPSRMMainRoleMatrix>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.SRM.GetSAPSRMRoles,
                new Dictionary<string, string> { { "adhocActiontype", adhocActionType.ToString() }, { "userId", userid}, { "searchString", searchString },
                 { "accountType", accounType.ToString() },{ "srmUserName", SRMUserName },{ "EndpointId", EndpointId }
                });

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<string, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = Convert.ToString(item.Key);
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;
        }

        public AutoCompleteBoxData GetBulkSRMBusinesRoles(Dictionary<string, object> context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            Dictionary<string, string> data = DomainApiClient<Dto.SAPSRMMainRoleMatrix>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.SRM.GetAllSAPSRMRoles, new Dictionary<string, string> { { "searchString", searchString } }
                );

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<string, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = Convert.ToString(item.Key);
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;
        }

        #endregion SRM

        #region O365

        public AutoCompleteBoxData GetAdhocLicenseInfo(Dictionary<string, object> context)
        {
            Dictionary<string, string> data = new System.Collections.Generic.Dictionary<string, string>();
            if (HttpContext.Current.Session["AdhocActionType"] != null)
            {
                Office365LicenseConfigDto dto = new Office365LicenseConfigDto();
                var adhocactiontype = (AdHocActionType)Enum.Parse(typeof(AdHocActionType), HttpContext.Current.Session["AdhocActionType"].ToString(), true);
                List<ServicePlanDetail> ExistinglicenseInfo = new System.Collections.Generic.List<ServicePlanDetail>();
                if (HttpContext.Current.Session["ExistingServicePlanDetail"] != null)
                {
                    dto.ExistingServicePlanDetail = (List<ServicePlanDetail>)(Convert.ChangeType(HttpContext.Current.Session["ExistingServicePlanDetail"], typeof(List<ServicePlanDetail>)));
                }
                List<AdHocLicenseInfo> licenseInfo = new System.Collections.Generic.List<AdHocLicenseInfo>();
                if (HttpContext.Current.Session["AdHocLicenseInfo"] != null)
                {

                    dto.AdHocLicenseInfo = (List<AdHocLicenseInfo>)(Convert.ChangeType(HttpContext.Current.Session["AdHocLicenseInfo"], typeof(List<AdHocLicenseInfo>)));
                }
                string searchString = context["Text"].ToString().Replace("*", string.Empty);

                data = DomainApiClient<Dto.Office365LicenseConfig>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.O365.GetAdhocOffice365LicenseInfos,
                                                    new Dictionary<string, string>{ { "SearchString", searchString },
                                                                                    {"adhocActionType",adhocactiontype.ToString()}
                                                                                  },
                                                    dto);
            }

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<string, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = Convert.ToString(item.Key);
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;
        }


        public AutoCompleteBoxData GetLicenseInfoForBulkReq(Dictionary<string, object> context)
        {
            Dictionary<string, string> data = new System.Collections.Generic.Dictionary<string, string>();


            Office365LicenseConfigDto dto = new Office365LicenseConfigDto();

            dto.AdHocLicenseInfo = new System.Collections.Generic.List<AdHocLicenseInfo>();
            if (HttpContext.Current.Session["SelectedLicenseInfo"] != null)
            {

                dto.AdHocLicenseInfo = (List<AdHocLicenseInfo>)(Convert.ChangeType(HttpContext.Current.Session["SelectedLicenseInfo"], typeof(List<AdHocLicenseInfo>)));
            }

            string searchString = context["Text"].ToString().Replace("*", string.Empty);

            data = DomainApiClient<Dto.Office365LicenseConfig>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.O365.GetLicenseInfoForBulkReq,
                                                   new Dictionary<string, string> { { "SearchString", searchString } },
                                                   dto);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<string, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = Convert.ToString(item.Key);
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;
        }


        public AutoCompleteBoxData GetO365Users(Dictionary<string, object> context)
        {
            Dictionary<string, string> data = new System.Collections.Generic.Dictionary<string, string>();

            if (HttpContext.Current.Session["EndpointSystemId"] != null)
            {
                var endpointId = (long)HttpContext.Current.Session["EndpointSystemId"];
                string searchString = context["Text"].ToString().Replace("*", string.Empty);
                data = DomainApiClient<Dto.Office365LicenseConfig>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.O365.GetO365Users,
                                                  new Dictionary<string, string> { { "searchString", searchString }, { "endpointsystemId", endpointId.ToString() } });

            }
            List<AutoCompleteBoxItemData> result = new System.Collections.Generic.List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<string, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = Convert.ToString(item.Key);
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;
        }

        #endregion

        #region EBS

        public AutoCompleteBoxData GetAllPositionEBS(Dictionary<string, object> context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            ApplicationType ApplicationType = (ApplicationType)Convert.ToByte(HttpContext.Current.Session["ApplicationType"]);

            string prefix = Convert.ToString(HttpContext.Current.Session["SelectedPrefix"]);
            string employeeType = Convert.ToString(HttpContext.Current.Session["employeeType"]);

            if (string.IsNullOrWhiteSpace(employeeType))
                employeeType = EmployeeType.Permanent.ToString();

            var list = DomainApiClient<Dto.SAPEBSXLPositionsMatrix>.GetByAction<IList<Dto.SAPEBSXLPositionsMatrix>>(DomainApiAction.Matrix.SAP_EBS.BindPositionsEBS,
                 new Dictionary<string, string> { { "Entitlement", ((int)ApplicationType).ToString() }, { "prefix", prefix }, { "employeeType", employeeType } }
                );

            Dictionary<long, string> data = list.Where(lq => string.IsNullOrEmpty(searchString) ? true : lq.PositionText.ToLower().Contains(searchString.ToLower()))
                             .Select(x => new { Key = x.Id, Value = x.PositionText })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();

            foreach (KeyValuePair<long, string> item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;
        }

        public AutoCompleteBoxData GetExistingPositionEBS(Dictionary<string, object> context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            ApplicationType ApplicationType = (ApplicationType)Convert.ToByte(HttpContext.Current.Session["ApplicationType"]);

            long endpointUserId = Convert.ToInt32(HttpContext.Current.Session["EndpointUserId"]);
            string AccountName = HttpContext.Current.Session["UserAccountName"] == null ? string.Empty : Convert.ToString(HttpContext.Current.Session["UserAccountName"]);
            SAPUserinfo sapUser = DomainApiClient<Dto.SAPUserinfo>.GetByAction<Dto.SAPUserinfo>(DomainApiAction.Endpoint.SAP.SAPUserinfo.GetSAPUserByUserId,
                new Dictionary<string, string> { { "userId", endpointUserId.ToString() }, { "UserAccountName", AccountName } });
            //EndPoint Endpoint = new EndPointFacade().GetEndPointSystemByKey(sapUser.EndPointSystemId);

            if (sapUser == null) return new AutoCompleteBoxData();
            var position = DomainApiClient<Dto.SAPUserinfo>.GetByAction<Dictionary<long, string>>(DomainApiAction.Endpoint.SAP.SAPUserinfo.GetPositionTextByUser,
                 new Dictionary<string, string> {{ "endpointUserId", endpointUserId.ToString() },
                                                        { "endpointSystemId", sapUser.EndPointSystemId.ToString() },
                                                         { "applicationType", ((int)ApplicationType).ToString() }});

            Dictionary<long, string> data = position.Where(lq => string.IsNullOrEmpty(searchString) ? true : lq.Value.ToLower().Contains(searchString.ToLower())).Select(x => new { Key = x.Key, Value = x.Value })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();

            foreach (KeyValuePair<long, string> item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }

            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();

            return res;
        }

        public AutoCompleteBoxData GetAllRolesEBS(Dictionary<string, object> context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            ApplicationType ApplicationType = (ApplicationType)Convert.ToByte(HttpContext.Current.Session["ApplicationType"]);
            Dictionary<long, string> data = DomainApiClient<Dto.SAPEBSXLRoleMatrix>.GetByAction<IList<Dto.SAPEBSXLRoleMatrix>>(DomainApiAction.Matrix.SAP_EBS.BindRolesEBS,
                    new Dictionary<string, string> { { "ApplicationName", ((int)ApplicationType).ToString() } })
                    .Where(lq => string.IsNullOrEmpty(searchString) ? true : lq.RoleName.ToLower().Contains(searchString.ToLower()))
                     .Select(x => new { Key = x.Id, Value = x.RoleName })
                     .Distinct().ToDictionary(o => o.Key, o => o.Value);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();

            foreach (KeyValuePair<long, string> item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }

            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();

            return res;
        }

        public AutoCompleteBoxData GetExistingRolesEBS(Dictionary<string, object> context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            ApplicationType ApplicationType = (ApplicationType)Convert.ToByte(HttpContext.Current.Session["ApplicationType"]);
            int endpointUserId = Convert.ToInt32(HttpContext.Current.Session["EndpointUserId"]);

            var param = new Dictionary<string, string> { { "endpointId", endpointUserId.ToString() }, { "_applicationType", Convert.ToString((int)ApplicationType) } };

            Dictionary<long, string> data = DomainApiClient<SAPRoleInfo>.GetByAction<Dictionary<long, string>>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.GetBusinessRoles, param);



            //var roles = new RoleMatrixFacade().GetBusinessRoles(endpointUserId, ApplicationType).Where(lq => string.IsNullOrEmpty(searchString) ? true : lq.Value.ToLower().Contains(searchString.ToLower()));
            //Dictionary<int, string> data = roles.Select(x => new { Key = x.Key, Value = x.Value })
            //                .Distinct().ToDictionary(o => o.Key, o => o.Value);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();

            foreach (KeyValuePair<long, string> item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }

            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();

            return res;
        }

        #endregion

        public AutoCompleteBoxData GetUsers(Dictionary<string, object> context)
        {
            try
            {
                List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
                string[] EntryValues = null;
                string hiddenVal = string.Empty;

                if (!context.ContainsKey("hidden"))
                    hiddenVal = "True";
                else
                    hiddenVal = context["hidden"].ToString();

                bool val = Convert.ToBoolean(hiddenVal);
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("searchString", context["Text"].ToString());
                parameter.Add("hidden", val.ToString());
                Dictionary<long, string> data = new Dictionary<long, string>();
                if (context.ContainsKey("ConsiderJoindDate") && context["ConsiderJoindDate"] != null && context["ConsiderJoindDate"].ToString() == "true")
                {
                    parameter.Add("considerJoinDate", bool.TrueString);
                    data = DomainApiClient<Dto.User>.GetByAction<Dictionary<long, string>>
                       (DomainApiAction.User.GetBySearchStringAndConsiderJoinDate, parameter);
                }
                else
                {
                    data = DomainApiClient<Dto.User>.GetByAction<Dictionary<long, string>>
                       (DomainApiAction.User.GetBySearchString, parameter);
                }

                foreach (KeyValuePair<long, string> item in data)
                {
                    AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                    childNode.Text = item.Value;
                    childNode.Value = item.Key.ToString();
                    result.Add(childNode);
                }

                AutoCompleteBoxData res = new AutoCompleteBoxData();
                if (EntryValues != null)
                {
                    res.Items = (result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray());
                }
                else
                {
                    res.Items = result.OrderBy(m => m.Text).ToArray();
                }
                return res;
            }
            catch
            {
                throw;
            }
        }
        
        public AutoCompleteBoxData GetROUsers(Dictionary<string, object> context)
        {
            try
            {
                List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("searchString", context["Text"].ToString());
                Dictionary<long, string> data = new Dictionary<long, string>();
                data = DomainApiClient<Dto.User>.GetByAction<Dictionary<long, string>>
                   (DomainApiAction.User.GetROUsersBySearchString, parameter);

                foreach (KeyValuePair<long, string> item in data)
                {
                    AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                    childNode.Text = item.Value;
                    childNode.Value = item.Key.ToString();
                    result.Add(childNode);
                }

                AutoCompleteBoxData res = new AutoCompleteBoxData();
                res.Items = result.OrderBy(m => m.Text).ToArray();
                return res;
            }
            catch
            {
                throw;
            }
        }
        public AutoCompleteBoxData GetEntitlementGEMS(Dictionary<string, object> context)
        {
            int EndpointUserId = 0; int actionType = 0;
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            if (HttpContext.Current.Session["EndpointUserId"] != null)
            {
                EndpointUserId = Convert.ToInt32(HttpContext.Current.Session["EndpointUserId"]);
            }
            if (HttpContext.Current.Session["AdhocActionType"] != null)
            {
                actionType = Convert.ToInt32(HttpContext.Current.Session["AdhocActionType"]);
            }

            Dictionary<long, string> data;
            if (EndpointUserId == 0)
            {
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    data = DomainApiClient<Dto.GEMSRoleMatrix>.GetByAction<List<Dto.GEMSRoleMatrix>>(DomainApiAction.Matrix.GEMS.BindRolesGEMS, null).Where(p => p.BusinessRoleName.Contains(searchString,StringComparison.InvariantCultureIgnoreCase)).Select(x => new { Key = x.Id, Value = x.BusinessRoleName })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
                else
                {
                    data = DomainApiClient<Dto.GEMSRoleMatrix>.GetByAction<List<Dto.GEMSRoleMatrix>>(DomainApiAction.Matrix.GEMS.BindRolesGEMS, null).Select(x => new { Key = x.Id, Value = x.BusinessRoleName })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
            }
            else
            {
                var param = new Dictionary<string, string> { { "endpointId", EndpointUserId.ToString() },
                    { "_applicationType", Convert.ToString((int)ApplicationType.GEMS) }
                   ,{"_actionType", actionType.ToString()  },
                    { "searchstring",searchString} };

                data = DomainApiClient<GEMSRoleMatrix>.GetByAction<Dictionary<long, string>>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.GetBusinessRoles, param);
            }
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;
        }
        public AutoCompleteBoxData GetAllPositionMSSL(Dictionary<string, object> context)
        {
            IList<Dto.SAPXLMSSLPositionsMatrix> position = new List<Dto.SAPXLMSSLPositionsMatrix>();
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            ApplicationType ApplicationType = (ApplicationType)Convert.ToByte(HttpContext.Current.Session["ApplicationType"]);
            string prefix = Convert.ToString(HttpContext.Current.Session["SelectedPrefix"]);
            string employeeType = Convert.ToString(HttpContext.Current.Session["employeeType"]);
            if (string.IsNullOrWhiteSpace(employeeType))
                employeeType = EmployeeType.Permanent.ToString();

            var list = DomainApiClient<Dto.SAPXLMSSLPositionsMatrix>.GetByAction<IList<Dto.SAPXLMSSLPositionsMatrix>>(DomainApiAction.Matrix.SAP_MSSL.BindPositions,
                 new Dictionary<string, string> { { "Entitlement", ((int)ApplicationType).ToString() }, { "prefix", prefix }, { "employeeType", employeeType } }
                );
            if (list != null)
                position = list.Where(lq => string.IsNullOrEmpty(searchString) ? true : lq.PositionText.ToLower().Contains(searchString.ToLower())).Select(lq => lq).ToList();
            Dictionary<long, string> data = position.Select(x => new { Key = x.Id, Value = x.PositionText })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();

            foreach (KeyValuePair<long, string> item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;
        }

        public AutoCompleteBoxData GetExistingPositionMSSL(Dictionary<string, object> context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            ApplicationType ApplicationType = (ApplicationType)Convert.ToByte(HttpContext.Current.Session["ApplicationType"]);
            long endpointUserId = Convert.ToInt32(HttpContext.Current.Session["EndpointUserId"]);
            string AccountName = HttpContext.Current.Session["UserAccountName"] == null ? string.Empty : Convert.ToString(HttpContext.Current.Session["UserAccountName"]);
            SAPUserinfo sapUser = DomainApiClient<Dto.SAPUserinfo>.GetByAction<Dto.SAPUserinfo>(DomainApiAction.Endpoint.SAP.SAPUserinfo.GetSAPUserByUserId,
                new Dictionary<string, string> { { "userId", endpointUserId.ToString() }, { "UserAccountName", AccountName } });
            //EndPoint Endpoint = new EndPointFacade().GetEndPointSystemByKey(sapUser.EndPointSystemId);

            if (sapUser == null) return new AutoCompleteBoxData();
            var position = DomainApiClient<Dto.SAPUserinfo>.GetByAction<Dictionary<long, string>>(DomainApiAction.Endpoint.SAP.SAPUserinfo.GetPositionTextByUser,
                 new Dictionary<string, string> {{ "endpointUserId", endpointUserId.ToString() },
                                                        { "endpointSystemId", sapUser.EndPointSystemId.ToString() },
                                                         { "applicationType", ((int)ApplicationType).ToString() }});

            Dictionary<long, string> data = position.Where(lq => string.IsNullOrEmpty(searchString) ? true : lq.Value.ToLower().Contains(searchString.ToLower())).Select(x => new { Key = x.Key, Value = x.Value })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();

            foreach (KeyValuePair<long, string> item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }

            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();

            return res;
        }

        public AutoCompleteBoxData GetExistingRolesMSSL(Dictionary<string, object> context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            ApplicationType ApplicationType = (ApplicationType)Convert.ToByte(HttpContext.Current.Session["ApplicationType"]);
            int endpointUserId = Convert.ToInt32(HttpContext.Current.Session["EndpointUserId"]);

            var param = new Dictionary<string, string>() { { "endpointId", endpointUserId.ToString() }, { "_applicationType", ((int)ApplicationType).ToString() } };
            Dictionary<long, string> datadict = DomainApiClient<SAPRoleInfo>.GetByAction<Dictionary<long, string>>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.GetBusinessRoles, param);

            var roles = datadict.Where(lq => string.IsNullOrEmpty(searchString) ? true : lq.Value.ToLower().Contains(searchString.ToLower()));
            Dictionary<long, string> data = roles.Select(x => new { Key = x.Key, Value = x.Value }).Distinct().ToDictionary(o => o.Key, o => o.Value);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();

            foreach (KeyValuePair<long, string> item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }

            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();

            return res;
        }

        public AutoCompleteBoxData GetAllRolesMSSL(Dictionary<string, object> context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            ApplicationType ApplicationType = (ApplicationType)Convert.ToByte(HttpContext.Current.Session["ApplicationType"]);
            IList<Dto.SAPXLMSSLRolesMatrix> roles = DomainApiClient<Dto.SAPXLMSSLRolesMatrix>.GetByAction<List<Dto.SAPXLMSSLRolesMatrix>>(DomainApiAction.Matrix.SAP_MSSL.BindRoles, new Dictionary<string, string> { { "ApplicationName", ((int)ApplicationType).ToString() } });
            if (roles == null)
                roles = new List<Dto.SAPXLMSSLRolesMatrix>();

            Dictionary<long, string> data = roles.Where(lq => string.IsNullOrEmpty(searchString) ? true : lq.RoleName.ToLower().Contains(searchString.ToLower())).Select(x => new { Key = x.Id, Value = x.RoleName })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();

            foreach (KeyValuePair<long, string> item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }

            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();

            return res;
        }
        public AutoCompleteBoxData GetBusinessRoleFromSAPGRCRole(RadAutoCompleteContext context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            string subModule = Convert.ToString(HttpContext.Current.Session["SubModule"]);
            string Module = Convert.ToString(HttpContext.Current.Session["Module"]);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();

            int EndpointUserId = 0; int actionType = 0;
            if (HttpContext.Current.Session["EndpointUserId"] != null)
            {
                EndpointUserId = Convert.ToInt32(HttpContext.Current.Session["EndpointUserId"]);
            }

            if (HttpContext.Current.Session["AdhocActionType"] != null)
            {
                actionType = Convert.ToInt32(HttpContext.Current.Session["AdhocActionType"]);
            }
            Dictionary<long, string> data = new Dictionary<long, string>();
            if (EndpointUserId != 0)
            {
                //added filter criteria for sap grc role for amendmend add/remove.
                var param = new Dictionary<string, string>() {
                    { "endpointId", EndpointUserId.ToString() },
                    { "_applicationType", ((int)ApplicationType.SAP_GRC).ToString() },
                    {"_actionType", actionType.ToString()  },
                    { "searchString", searchString }, { "subModule", subModule }, { "Module", Module }
                };
                data = DomainApiClient<SAPGRCRole>.GetByAction<Dictionary<long, string>>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.GetBusinessRoles, param);
            }
            else
            {
                data = DomainApiClient<SAPGRCRole>.GetByAction<Dictionary<long, string>>(DomainApiAction.Matrix.SAP_GRC.GetBusinessRoleFromGRCRole,
                    new Dictionary<string, string> { { "searchString", searchString }, { "subModule", subModule }, { "Module", Module } });
            }
            foreach (KeyValuePair<long, string> item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }

            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();

            return res;
        }


        public AutoCompleteBoxData GetBusinessRoleFromSAPPIRole(RadAutoCompleteContext context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            string subModule = Convert.ToString(HttpContext.Current.Session["SubModule"]);
            string Module = Convert.ToString(HttpContext.Current.Session["Module"]);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();

            int EndpointUserId = 0;
            if (HttpContext.Current.Session["EndpointUserId"] != null)
            {
                EndpointUserId = Convert.ToInt32(HttpContext.Current.Session["EndpointUserId"]);
            }
            Dictionary<long, string> data = new Dictionary<long, string>();
            if (EndpointUserId != 0)
            {
                var param = new Dictionary<string, string>() { { "endpointId", EndpointUserId.ToString() }, { "_applicationType", ((int)ApplicationType.SAP_PI).ToString() } };
                data = DomainApiClient<SAPRoleInfo>.GetByAction<Dictionary<long, string>>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.GetBusinessRoles, param);
            }
            else
            {
                data = DomainApiClient<SAPPIRole>.GetByAction<Dictionary<long, string>>(DomainApiAction.Matrix.SAP_PI.GetBusinessRoleFromPIRole,
                    new Dictionary<string, string> { { "searchString", searchString }, { "subModule", subModule }, { "Module", Module } });
            }
            foreach (KeyValuePair<long, string> item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }

            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();

            return res;
        }
        public AutoCompleteBoxData GetBusinessRoleFromFlinsBiRole(RadAutoCompleteContext context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            string subModule = Convert.ToString(HttpContext.Current.Session["SubModule"]);
            string Module = Convert.ToString(HttpContext.Current.Session["Module"]);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();

            int EndpointUserId = 0; int actionType = 0;
            if (HttpContext.Current.Session["EndpointUserId"] != null)
            {
                EndpointUserId = Convert.ToInt32(HttpContext.Current.Session["EndpointUserId"]);
            }

            if (HttpContext.Current.Session["AdhocActionType"] != null)
            {
                actionType = Convert.ToInt32(HttpContext.Current.Session["AdhocActionType"]);
            }

            Dictionary<long, string> data = new Dictionary<long, string>();
            if (EndpointUserId != 0)
            {
                //string searchString, long endpointUserId, long actionType
                var param = new Dictionary<string, string>() { { "searchString", searchString }, { "endpointUserId", EndpointUserId.ToString() },                                                            { "actionType", actionType.ToString() },
                                                                { "subModule", subModule },
                                                                { "Module",Module} };
                data = DomainApiClient<SAPBIRoleCategory>.GetByAction<Dictionary<long, string>>(DomainApiAction.Matrix.SAP_FLINS_BI.GetBusinessRoles, param);
            }
            else
            {

                var param = new Dictionary<string, string>() {  { "searchString", searchString },
                                                                { "subModule", subModule },
                                                                { "Module",Module} };
                data = DomainApiClient<SAPBIRoleCategory>.GetByAction<Dictionary<long, string>>(DomainApiAction.Matrix.SAP_FLINS_BI.GetBusinessRoleFromFlinsBIRole, param);

            }
            foreach (KeyValuePair<long, string> item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }

            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();

            return res;
        }

        public AutoCompleteBoxData GetBusinessRole(RadAutoCompleteContext context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            string subModule = Convert.ToString(HttpContext.Current.Session["SubModule"]);
            string Module = Convert.ToString(HttpContext.Current.Session["Module"]);
            AdHocAccessRequest request = (AdHocAccessRequest)HttpContext.Current.Session["AdhocrequestObj"];
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            int EndpointUserId = 0;
            if (HttpContext.Current.Session["EndpointUserId"] != null)
            {
                EndpointUserId = Convert.ToInt32(HttpContext.Current.Session["EndpointUserId"]);
            }
            Dictionary<long, string> data = new Dictionary<long, string>();
            if (EndpointUserId == 0)
            {
                var param = new Dictionary<string, string> { { "searchString", searchString }, { "module", Module }, { "subModuleID", subModule }, { "UserID", request.RequesteeUserId.ToString() } };
                data = DomainApiClient<SAPHRFLINSRoleMatrix>.GetByAction<Dictionary<long, string>>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.GetBusinessRoles, param);
            }
            else
            {
                var param = new Dictionary<string, string> { { "endpointId", EndpointUserId.ToString() }, { "_applicationType", Convert.ToString((int)ApplicationType.HRMS_FLINS) }, { "module", Module }, { "subModule", subModule } };

                data = DomainApiClient<SAPRoleInfo>.GetByAction<Dictionary<long, string>>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.GetBusinessRoles, param);
            }
            if (data != null && data.Count > 0 && !string.IsNullOrWhiteSpace(searchString))
            {
                data = data.Where(i => i.Value != null && i.Value.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)).ToDictionary(i => i.Key, j => j.Value);
            }
            foreach (KeyValuePair<long, string> item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            if (result.Count <= 0)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = "No Records";
                childNode.Value = "-1";
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();

            return res;
        }

        public AutoCompleteBoxData GetUsersForBulkRequest(Dictionary<string, object> context)
        {
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            bool IsFulltimeEmployee = true;
            string RosEmpId = "";
            if (context["IsFulltimeEmployee"] != null && !string.IsNullOrWhiteSpace(context["IsFulltimeEmployee"].ToString()) && !context["IsFulltimeEmployee"].ToString().Equals("null", StringComparison.InvariantCultureIgnoreCase))
            {
                IsFulltimeEmployee = context["IsFulltimeEmployee"].ToString() == "False" ? false : true;
            }
            if (context["RosEmpid"] != null && !string.IsNullOrWhiteSpace(context["RosEmpid"].ToString()))
            {
                RosEmpId = Convert.ToString(context["RosEmpid"]);
            }

            string searchString = context["Text"].ToString().Replace("*", string.Empty);

            Dictionary<string, string> param = new Dictionary<string, string>
                {
                    {"searchString",searchString },
                    {"IsFulltimeEmployee",IsFulltimeEmployee? "true": "false" },
                    {"RosEmpId",RosEmpId }
                };

            Dictionary<long, string> data = DomainApiClient<Dto.User>.GetByAction<Dictionary<long, string>>(DomainApiAction.User.GetUsersForBulkRequest, param);
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }

            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;

        }
        //public AutoCompleteBoxData GetRules(Dictionary<string, object> context)
        //{
        //    try
        //    {
        //        List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
        //        string[] EntryValues = null;

        //        Dictionary<string, string> parameter = new Dictionary<string, string>();
        //        parameter.Add("searchString", context["Text"].ToString());
        //        //Dictionary<long, string> data = DomainApiClient<Dto.NRuleset>.GetByAction<Dictionary<long, string>>
        //        //    ("Testmethod", parameter);

        //        Dictionary<long, string> data = new Dictionary<long, string>();
        //        data.Add(1, "dsf");

        //        foreach (KeyValuePair<long, string> item in data)
        //        {
        //            AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
        //            childNode.Text = item.Value;
        //            childNode.Value = item.Key.ToString();
        //            result.Add(childNode);
        //        }

        //        AutoCompleteBoxData res = new AutoCompleteBoxData();
        //        if (EntryValues != null)
        //        {
        //            res.Items = (result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray());
        //        }
        //        else
        //        {
        //            res.Items = result.OrderBy(m => m.Text).ToArray();
        //        }
        //        return res;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        
        public AutoCompleteBoxData GetADGroupsByCanRequest(Dictionary<string, object> context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            int EndpointSystemID = HttpContext.Current.Session["EndpointSystemIdValue"] != null ? Convert.ToInt32(HttpContext.Current.Session["EndpointSystemIdValue"]) : 0;
            int accessRequestId = context["AccessRequestId"] != null ? Convert.ToInt32(context["AccessRequestId"]) : 0;
            bool isBulkRequest = context["IsBulkRequest"] != null ? (context["IsBulkRequest"].ToString() == "1" ? true:false):false ;
            var param = new Dictionary<string, string> { { "searchString", searchString }, { "EndpointId", EndpointSystemID.ToString() }, { "AccessRequestId", accessRequestId.ToString() },
                { "isBulkRequest", isBulkRequest.ToString() }
            };


            Dictionary<long, string> data = DomainApiClient<Dto.ADSGroupInfo>.GetByAction<Dictionary<long, string>>(DomainApiAction.Endpoint.ActiveDirectory.ADSGroupInfo.GetADSGroupBySearchString, param);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();

            foreach (KeyValuePair<long, string> item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }

            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();

            return res;
        }
        public AutoCompleteBoxData GetRules(Dictionary<string, object> context)
        {
            try
            {
                List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
                string[] EntryValues = null;

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("searchString", context["Text"].ToString());
                Dictionary<long, string> data = DomainApiClient<Dto.NRuleset>.GetByAction<Dictionary<long, string>>
                    (DomainApiAction.Ruleset.NRuleset.GetAllRuleset, parameter);

                foreach (KeyValuePair<long, string> item in data)
                {
                    AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                    childNode.Text = item.Value;
                    childNode.Value = item.Key.ToString();
                    result.Add(childNode);
                }

                AutoCompleteBoxData res = new AutoCompleteBoxData();
                if (EntryValues != null)
                {
                    res.Items = (result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray());
                }
                else
                {
                    res.Items = result.OrderBy(m => m.Text).ToArray();
                }
                return res;
            }
            catch
            {
                throw;
            }
        }
        public AutoCompleteBoxData GetEntitlementName(Dictionary<string, object> context)
        {
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            try
            {
                string hiddenVal = context["hidden"].ToString();
                string[] values = hiddenVal.Split(',');
                EndpointType endpointType = (EndpointType)Enum.Parse(typeof(EndpointType), values[0]);
                long endpointSystemId = Convert.ToInt64(values[1]);
                EntitlementType entitlementType = (EntitlementType)Enum.Parse(typeof(EntitlementType), values[2]);

                List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
                string[] EntryValues = null;

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("applicationType", endpointType.ToString());
                parameter.Add("endpointSystemId", endpointSystemId.ToString());
                parameter.Add("entitlementType", entitlementType.ToString());
                parameter.Add("searchString", context["Text"].ToString());
                Dictionary<long, string> entitlementName = DomainApiClient<Dto.Presentation.PresentationUserEntitlement>.GetByAction<Dictionary<long, string>>(DomainApiAction.Endpoint.Presentation.PresentationUserEntitlement.GetEntitlementName, parameter);

                foreach (KeyValuePair<long, string> item in entitlementName)
                {
                    AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                    childNode.Text = item.Value;
                    childNode.Value = item.Key.ToString();
                    result.Add(childNode);
                }

                if (EntryValues != null)
                {
                    res.Items = (result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray());
                }
                else
                {
                    res.Items = result.OrderBy(m => m.Text).ToArray();
                }
                return res;
            }
            catch
            {
                return res;
            }
        }
        public AutoCompleteBoxData GetSapFields(Dictionary<string, object> context)
        {
            string ObjectCode = "";
            string[] Items = context["Text"].ToString().Split(',');
            if (Items.Count() >= 2)
            {
                ObjectCode = Items[1].ToString();
            }
            string[] EntryValues = null;
            long EndpointId = Convert.ToInt64(HttpContext.Current.Session["ComplianceEndpointId"]);

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endPointId", EndpointId.ToString());
            parameter.Add("objCode", ObjectCode);
            List<string> data = DomainApiClient<Dto.SAPObjectInfo>.GetByAction<List<string>>
                (DomainApiAction.Endpoint.SAP.SAPObjectInfo.GetSapFields, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (string value in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = value;
                childNode.Value = value;
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }
        public AutoCompleteBoxData GetRuleSetValues(Dictionary<string, object> context)
        {
            string[] EntryValues = null;
            string ruleSetType = Convert.ToString(HttpContext.Current.Session["RuleSetType"]);
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = new Dictionary<long, string>();
            if (ruleSetType.Equals("Department"))
            {
                data = DomainApiClient<Dto.NOrganisation>.GetByAction<Dictionary<long, string>>
               ("GetNOrganisations", parameter);
            }
            else if (ruleSetType.Equals("Designation"))
            {
                data = DomainApiClient<Dto.NDesignation>.GetByAction<Dictionary<long, string>>
                ("GetNDesignations", parameter);
            }

            else if (ruleSetType.Equals("Company"))
            {
                parameter.Add("scopeId", ((long)Infrastructure.Enums.NOrganisationScopeValue.Company).ToString());
                data = DomainApiClient<Dto.NOrganisation>.GetByAction<Dictionary<long, string>>
                ("GetNOrganisationByScope", parameter);
            }
            else if (ruleSetType.Equals("BusinessUnit"))
            {
                parameter.Add("scopeId", ((long)Infrastructure.Enums.NOrganisationScopeValue.BusinessUnit).ToString());
                data = DomainApiClient<Dto.NOrganisation>.GetByAction<Dictionary<long, string>>
                ("GetNOrganisationByScope", parameter);
            }
            else if (ruleSetType.Equals("CostCenter"))
            {
                data = DomainApiClient<Dto.NCostCenter>.GetByAction<Dictionary<long, string>>
                ("GetCostCenters", parameter);
            }
            else if (ruleSetType.Equals("Location"))
            {
                parameter.Add("scopeId", ((long)Infrastructure.Enums.NOrganisationScopeValue.Location).ToString());
                data = DomainApiClient<Dto.NOrganisation>.GetByAction<Dictionary<long, string>>
                ("GetNOrganisationByScope", parameter);
            }
            else if (ruleSetType.Equals("EmployeeType"))
            {
                data = DI.Chakra.Infrastructure.UtilityMethods.GetAll<EmployeeType>();
                data = data.Where(m => m.Value != "None").ToDictionary(n => n.Key, n => n.Value);
            }
            else if (ruleSetType.Equals("EventTrigger"))
            {
                data = DI.Chakra.Infrastructure.UtilityMethods.GetAll<EmployeeEvents>();
                data = data.Where(m => m.Value != "None").ToDictionary(n => n.Key, n => n.Value);
            }
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }
        public AutoCompleteBoxData GetSAPAuthorizationObjects(Dictionary<string, object> context)
        {
            string[] EntryValues = null;
            long EndpointId = Convert.ToInt64(HttpContext.Current.Session["ComplianceEndpointId"]);

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endPointId", EndpointId.ToString());
            parameter.Add("objCode", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.SAPObjectInfo>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Endpoint.SAP.SAPObjectInfo.GetDictionaryByObjCode, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }
        public AutoCompleteBoxData GetADUserGroups(Dictionary<string, object> context)
        {
            string[] EntryValues = null;

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.ADSGroupInfo>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Endpoint.ActiveDirectory.ADSGroupInfo.GetAllByName, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }
        public AutoCompleteBoxData GetFunctions(Dictionary<string, object> context)
        {
            string[] EntryValues = null;

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.SODFunction>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.SODFunction.Function.GetBySearchString, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }
        public AutoCompleteBoxData GetAllSodExceptions(Dictionary<string, object> context)
        {
            string[] EntryValues = null;

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.SODException>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.SODException.Exception.GetBySearchString, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }
        public AutoCompleteBoxData GetAllGroups(Dictionary<string, object> context)
        {
            string[] EntryValues = null;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.Group>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Group.GetAllGroups, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }
        public AutoCompleteBoxData GetAllRiskLevel(Dictionary<string, object> context)
        {
            string[] EntryValues = null;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.SODRiskLevel>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Risks.SODRiskLevel.GetBySearchString, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }
        public AutoCompleteBoxData GetCompliancePolicy(Dictionary<string, object> context)
        {
            string[] EntryValues = null;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.CompliancePolicy>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Complaince.CompliancePolicy.Policy.GetBySearchString, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }
        [WebMethod(EnableSession = true)]
        public AutoCompleteBoxData GetSODConflictFunctions(Dictionary<string, object> context)
        {
            string filterString = HttpContext.Current.Session["CompliancType"] != null ?
                HttpContext.Current.Session["CompliancType"].ToString() : "";
            string[] EntryValues = null;

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            parameter.Add("filterString", filterString);
            Dictionary<long, string> data = DomainApiClient<Dto.SODConflictFunctions>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.SODFunction.SODConflictFunction.GetBySodConflictCode, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }
        public AutoCompleteBoxData GetSODPolicyDefinitions(Dictionary<string, object> context)
        {
            string[] EntryValues = null;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.SODPolicyExecutionDefinition>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.SODPolicyExecutionDefinition.ExecutionDefinition.GetBySearchString, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }
        public AutoCompleteBoxData GetEndPoint(Dictionary<string, object> context)
        {
            string[] EntryValues = null;
            Dictionary<long, string> data;

            if (HttpContext.Current.Session["EndPointId"].ToString() == "ALL")
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("searchString", context["Text"].ToString());
                data = DomainApiClient<Dto.EndPoint>.GetByAction<Dictionary<long, string>>
                    (DomainApiAction.Endpoint.EndPoint.GetByDescriptionSearchString, parameter);
            }
            else
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("searchString", context["Text"].ToString());
                data = DomainApiClient<Dto.EndPoint>.GetByAction<Dictionary<long, string>>
                    (DomainApiAction.Endpoint.EndPoint.GetByDescriptionSearchString, parameter);
            }
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }

        public AutoCompleteBoxData GetAllEndPoint(Dictionary<string, object> context)
        {
            try
            {
                List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
                string[] EntryValues = null;
                string hiddenVal = string.Empty;

                if (!context.ContainsKey("hidden"))
                    hiddenVal = "True";
                else
                    hiddenVal = context["hidden"].ToString();

                bool val = Convert.ToBoolean(hiddenVal);
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("descriptionSearchString", context["Text"].ToString());
                Dictionary<long, string> data= DomainApiClient<Dto.EndPoint>.GetByAction<Dictionary<long, string>>
                    ("GetByEndpointDescSearchString", parameter);

                foreach (KeyValuePair<long, string> item in data)
                {
                    AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                    childNode.Text = item.Value;
                    childNode.Value = item.Key.ToString();
                    result.Add(childNode);
                }

                AutoCompleteBoxData res = new AutoCompleteBoxData();
                if (EntryValues != null)
                {
                    res.Items = (result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray());
                }
                else
                {
                    res.Items = result.OrderBy(m => m.Text).ToArray();
                }
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public AutoCompleteBoxData GetNOrganisations(Dictionary<string, object> context)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.NOrganisation>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Org.NOrganisation.GetNOrganisations, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }
        public AutoCompleteBoxData GetDepartments(Dictionary<string, object> context)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.NOrganisation>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Org.NOrganisation.GetNOrganisations, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }
        public AutoCompleteBoxData GetNEmployeeTypes(Dictionary<string, object> context)
        {
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (EmployeeType item in Enum.GetValues(typeof(EmployeeType)))
            {
                var description = UtilityMethods.GetEnumAttribute<Description>(item);
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = description;
                childNode.Value = description;
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }
        public AutoCompleteBoxData GetNEmployeeEventTypes(Dictionary<string, object> context)
        {
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (EmployeeEvents item in Enum.GetValues(typeof(EmployeeEvents)))
            {
                var description = UtilityMethods.GetEnumAttribute<Description>(item);
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = description;
                childNode.Value = description;
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }
        public AutoCompleteBoxData GetNRulesetTypes(Dictionary<string, object> context)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.NRulesetType>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Ruleset.NRulesetType.GetAllRulesetType, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }
        public AutoCompleteBoxData GetNDesignations(Dictionary<string, object> context)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.NDesignation>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Ruleset.Designation.GetNDesignations, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }

        public AutoCompleteBoxData GetNRulesets(Dictionary<string, object> context)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.NRuleset>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Ruleset.NRuleset.GetAllRuleset, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }
        //public AutoCompleteBoxData GetProvisioningFields(Dictionary<string, object> context)
        //{
        //    Dictionary<string, string> parameter = new Dictionary<string, string>();
        //    parameter.Add("searchString", context["Text"].ToString());

        //    if(context.Where(a => a.Key == "Filter").Any())
        //    {
        //        var customFilter = context["Filter"].ToString();
        //        foreach(var filter in customFilter.Split(';'))
        //        {
        //            var keyValuePair = filter.Split('=');
        //            if (keyValuePair.Length == 2)
        //            {
        //                parameter.Add(keyValuePair[0], keyValuePair[1]);
        //            }
        //        }
        //    }

        //    Dictionary<long, string> data = DomainApiClient<Dto.NProvisioningField>.GetByAction<Dictionary<long, string>>
        //        (DomainApiAction.Ruleset.NProvisioningField.GetAllProvisioningFields, parameter);

        //    List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
        //    foreach (KeyValuePair<long, string> item in data)
        //    {
        //        AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
        //        childNode.Text = item.Value;
        //        childNode.Value = item.Key.ToString();
        //        result.Add(childNode);
        //    }
        //    AutoCompleteBoxData res = new AutoCompleteBoxData();
        //    res.Items = result.OrderBy(m => m.Text).ToArray();
        //    return res;
        //}

        public AutoCompleteBoxData GetProvisioningWokflows(Dictionary<string, object> context)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.WorkflowConfiguration>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Workflow.WorkflowConfiguration.GetBySearchString, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }
        public AutoCompleteBoxData GetApplicableApplications(Dictionary<string, object> context)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("displayNameSearchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.EndPoint>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Endpoint.EndPoint.GetApplicableApplications, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }
        public AutoCompleteBoxData GetPasswordPolicyApplications(Dictionary<string, object> context)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("displayNameSearchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.EndPoint>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Endpoint.EndPoint.GetPasswordPolicyApplications, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }
        public AutoCompleteBoxData GetActiveAndNonTerminatedUsers(Dictionary<string, object> context)
        {
            try
            {
                List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
                string TaskRequestedBy = string.Empty;
                var LstRequestFor = new List<string>();
                //if (HttpContext.Current.Session["TaskRequestedBy"] != null)
                //{
                //    TaskRequestedBy = HttpContext.Current.Session["TaskRequestedBy"].ToString();
                //}

                if (HttpContext.Current.Session["LstRequestedFor"] != null)
                {
                    LstRequestFor = (List<string>)HttpContext.Current.Session["LstRequestedFor"];
                }

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("searchString", context["Text"].ToString());
                Dictionary<long, string> data = DomainApiClient<Dto.User>.GetByAction<Dictionary<long, string>>
                    (DomainApiAction.User.GetActiveAndNonTerminaters, parameter);

                AutoCompleteBoxItemData childNode;
                foreach (KeyValuePair<long, string> item in data)
                {
                    if (!LstRequestFor.Any(p => item.Value.StartsWith(p, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        childNode = new AutoCompleteBoxItemData();
                        childNode.Text = item.Value;
                        childNode.Value = item.Key.ToString();
                        result.Add(childNode);
                    }
                }
                AutoCompleteBoxData res = new AutoCompleteBoxData();
                //if (TaskRequestedBy != null)
                //{
                //    res.Items = (result.Where(c => !TaskRequestedBy.Contains(c.Text)).OrderBy(m => m.Text).ToArray());
                //}
                //else
                //{
                //    res.Items = result.OrderBy(m => m.Text).ToArray();
                //}
                res.Items = result.OrderBy(m => m.Text).ToArray();
                return res;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public AutoCompleteBoxData GetAllGroupsByEP(Dictionary<string, object> context)
        {
            long EndpointTypeId = sessionStatus.ProvisioningFormEndPointTypeId;
            long EndpointId = sessionStatus.ProvisioningFormEndPointId;

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endpointSystemTypeId", EndpointTypeId.ToString());
            parameter.Add("endPointID", EndpointId.ToString());
            parameter.Add("searchString", context["Text"].ToString());
            IList<Group> groups = DomainApiClient<Dto.Group>.GetByAction<IList<Dto.Group>>
                (DomainApiAction.Group.GetAllGroupsByEndpointID, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (Group item in groups)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.GroupName;
                childNode.Value = item.Id.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }
        public AutoCompleteBoxData GetAllSAPRolesByEP(Dictionary<string, object> context)
        {
            EndPoint endPoint = new EndPoint();
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            long EndpointTypeId = sessionStatus.ProvisioningFormEndPointTypeId;
            long EndpointId = sessionStatus.ProvisioningFormEndPointId;
            endPoint.Id = EndpointId;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endpointId", EndpointId.ToString());

            IList<SAPRoleInfo> allSapRoles = DomainApiClient<Dto.SAPRoleInfo>.GetByAction<IList<Dto.SAPRoleInfo>>
                (DomainApiAction.Endpoint.SAP.SAPRoleinfo.GetSAPRoles, parameter);

            Dictionary<long, string> data = allSapRoles.Where(item => item.ROLE_CODE.Contains(searchString))
                           .Select(x => new { Key = x.Id, Value = x.ROLE_CODE })
                           .Distinct().ToDictionary(o => o.Key, o => o.Value);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }
        public AutoCompleteBoxData GetADOrganisationUnits(Dictionary<string, object> context)
        {
            long EndpointSystemdID = HttpContext.Current.Session["EndpointSystemID"] != null ?
                Convert.ToInt64(HttpContext.Current.Session["EndpointSystemID"]) : sessionStatus != null ?
                Convert.ToInt64(sessionStatus.RequestFormEPId) : 0;

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            parameter.Add("endpointsystemid", EndpointSystemdID.ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.ADSOuInfo>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Endpoint.ActiveDirectory.ADSOuInfo.GetBySearchString, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }
        public AutoCompleteBoxData GetLNSOrganisationUnits(Dictionary<string, object> context)
        {
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }

        public AutoCompleteBoxData GetInhouseSystemPermissionsByEndPointID(Dictionary<string, object> context)
        {
            long EndpointId = 0;

            if (HttpContext.Current.Session["InhouseSystemEndpointId"] != null)
            {
                EndpointId = (long)Convert.ChangeType(HttpContext.Current.Session["InhouseSystemEndpointId"], typeof(long));
            }
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EndpointId == 0)
                return res;

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endPointId", EndpointId.ToString());
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> inhPer = DomainApiClient<Dto.InhousePermission>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Endpoint.InhouseSystems.InhousePermission.GetByEndPointSystemId, parameter);

            foreach (KeyValuePair<long, string> item in inhPer)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }
        public AutoCompleteBoxData GetInhouseSystemCodeByEndPointID(Dictionary<string, object> context)
        {
            long EndpointId = 0;

            if (HttpContext.Current.Session["InhouseSystemEndpointId"] != null)
            {
                EndpointId = (long)Convert.ChangeType(HttpContext.Current.Session["InhouseSystemEndpointId"], typeof(long));
            }
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EndpointId == 0)
                return res;

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", EndpointId.ToString());
            parameter.Add("applicationCode", context["Text"].ToString());
            Dictionary<long, string> inhPer = DomainApiClient<Dto.EndPoint>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Endpoint.EndPoint.GetApplicationCodeById, parameter);

            foreach (KeyValuePair<long, string> item in inhPer)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }
        public AutoCompleteBoxData getReferenceUser(Dictionary<string, object> context)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("prefix", context["Text"].ToString());
            Dictionary<string, string> ajaxDataCollection = DomainApiClient<Dto.User>.GetByAction<Dictionary<string, string>>
                (DomainApiAction.User.GetReferenceUser, parameter);

            List<AutoCompleteBoxItemData> olistAutocomplete = new List<AutoCompleteBoxItemData>();
            foreach (var items in ajaxDataCollection)
            {
                AutoCompleteBoxItemData Children = new AutoCompleteBoxItemData();
                Children.Text = items.Value.ToString();
                Children.Value = items.Key.ToString();
                olistAutocomplete.Add(Children);
            }
            AutoCompleteBoxData result = new AutoCompleteBoxData();
            result.Items = olistAutocomplete.OrderBy(m => m.Text).ToArray();
            return result;
        }
        public AutoCompleteBoxData getReferencePermanentUser(Dictionary<string, object> context)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("prefix", context["Text"].ToString());
            Dictionary<string, string> ajaxDataCollection = DomainApiClient<Dto.User>.GetByAction<Dictionary<string, string>>
                (DomainApiAction.User.GetReferenceUserOnlyPermanent, parameter);

            List<AutoCompleteBoxItemData> olistAutocomplete = new List<AutoCompleteBoxItemData>();
            foreach (var items in ajaxDataCollection)
            {
                AutoCompleteBoxItemData Children = new AutoCompleteBoxItemData();
                Children.Text = items.Value.ToString();
                Children.Value = items.Key.ToString();
                olistAutocomplete.Add(Children);
            }
            AutoCompleteBoxData result = new AutoCompleteBoxData();
            result.Items = olistAutocomplete.OrderBy(m => m.Text).ToArray();
            return result;
        }
        
        public AutoCompleteBoxData GetReportingManager(Dictionary<string, object> context)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.User>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.User.GetReportingManager, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }
        public AutoCompleteBoxData GetPosition(Dictionary<string, object> context)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("prefix", context["Text"].ToString());
            Dictionary<string, string> data = DomainApiClient<Dto.User>.GetByAction<Dictionary<string, string>>
                (DomainApiAction.User.GetPosition, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (var items in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = items.Value;
                childNode.Value = items.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }
        public AutoCompleteBoxData setCostCenters(Dictionary<string, object> context)
        {
            long companyId = 0;

            if (HttpContext.Current.Session["CompanyId"] != null)
            {
                companyId = (long)Convert.ChangeType(HttpContext.Current.Session["CompanyId"], typeof(long));
            }

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            parameter.Add("companyId", companyId.ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.User>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.User.GetCostCenters, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }
        public AutoCompleteBoxData setOrganisatins(Dictionary<string, object> context)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<string, string> data = DomainApiClient<Dto.User>.GetByAction<Dictionary<string, string>>
                (DomainApiAction.User.GetOrganisationsWithPrefix, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<string, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }
        public AutoCompleteBoxData GetNonReportingUsersOfThisManagerID(Dictionary<string, object> context)
        {
            try
            {
                List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
                Dictionary<string, string> parameter = new Dictionary<string, string>();

                if (new SessionStatusImpl().User.Id > 0)
                {
                    long managerid = Convert.ToInt64(new SessionStatusImpl().User.Id);
                    parameter.Add("managerId", managerid.ToString());
                    parameter.Add("searchString", context["Text"].ToString());
                    Dictionary<long, string> data = DomainApiClient<Dto.User>.GetByAction<Dictionary<long, string>>
                        (DomainApiAction.User.GetNonReportingUsersOfManagerID, parameter);

                    foreach (KeyValuePair<long, string> item in data)
                    {
                        AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                        childNode.Text = item.Value;
                        childNode.Value = item.Key.ToString();
                        result.Add(childNode);
                    }
                }
                AutoCompleteBoxData res = new AutoCompleteBoxData();
                res.Items = result.OrderBy(m => m.Text).ToArray();
                return res;
            }
            catch
            {
                throw;
            }
        }


        public AutoCompleteBoxData GetUsersByGroup(Dictionary<string, object> context)
        {
            string[] EntryValues = null;
            string groupName = string.Empty;

            if (HttpContext.Current.Session["GroupName"] != null)
            {
                groupName = (string)Convert.ChangeType(HttpContext.Current.Session["GroupName"], typeof(string));
            }
            var delimiter = new char[] { ',' };

            string requestfor = "0";
            List<string> userIds = new List<string>();
            if (HttpContext.Current.Session["Requestfor"] != null)
            {
                requestfor = (string)Convert.ChangeType(HttpContext.Current.Session["Requestfor"], typeof(string));
            }
            if (!string.IsNullOrWhiteSpace(requestfor))
            {
                userIds = requestfor.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            parameter.Add("searchString", searchString);
            parameter.Add("groupName", groupName);

            Dictionary<long, string> data = DomainApiClient<Dto.User>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.User.GetUsersByGroup, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                if (item.Key != new SessionStatusImpl().User.Id)
                {
                    AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                    childNode.Text = item.Value;
                    childNode.Value = item.Key.ToString();
                    if (!userIds.Exists(i => i.Equals(childNode.Value)))
                        result.Add(childNode);
                }
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }
        public AutoCompleteBoxData GetAllRolesByEP(Dictionary<string, object> context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            long EndpointTypeId = sessionStatus.ProvisioningFormEndPointTypeId;
            long EndpointId = sessionStatus.ProvisioningFormEndPointId;

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endpointSystemId", EndpointTypeId.ToString());
            parameter.Add("endPointId", EndpointId.ToString());
            IList<DI.Chakra.Dto.Role> allRoleList = DomainApiClient<Dto.Role>.GetByAction<IList<Dto.Role>>
                (DomainApiAction.ACL.Role.GetAllRolesByEndpointID, parameter);

            Dictionary<long, string> data = allRoleList.Where(item => item.RoleName.Contains(searchString))
                           .Select(x => new
                           {
                               Key = x.Id
                           ,
                               Value = x.RoleName
                           })
                           .Distinct().ToDictionary(o => o.Key, o => o.Value);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();

            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }
        public AutoCompleteBoxData GetReportingUsersOfThisManagerIDandAdmin(Dictionary<string, object> context)
        {
            try
            {
                string searchString = context["Text"].ToString().Replace("*", string.Empty);
                List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
                if (new SessionStatusImpl().User.Id > 0)
                {
                    long managerid = Convert.ToInt64(new SessionStatusImpl().User.Id);
                    Dto.Presentation.PresentationReportingEmployees presentationReportingEmployees = new Dto.Presentation.PresentationReportingEmployees();

                    presentationReportingEmployees.UserID = managerid;
                    presentationReportingEmployees.SearchString = searchString;
                    if (HttpContext.Current.Session["CompanyID"] != null)
                    {
                        presentationReportingEmployees.CompanyId = (List<long>)(HttpContext.Current.Session["CompanyID"]);
                    }
                    if (HttpContext.Current.Session["DepartmentID"] != null)
                    {
                        presentationReportingEmployees.DepartmentId = (List<long>)(HttpContext.Current.Session["DepartmentID"]);
                    }
                    if (HttpContext.Current.Session["DivisionId"] != null)
                    {
                        presentationReportingEmployees.DivisionId = (List<long>)(HttpContext.Current.Session["DivisionId"]);
                    }
                    if (HttpContext.Current.Session["OrganisationId"] != null)
                    {
                        presentationReportingEmployees.OrganisationId = (List<long>)(HttpContext.Current.Session["OrganisationId"]);
                    }
                    if (HttpContext.Current.Session["SectionId"] != null)
                    {
                        presentationReportingEmployees.SectionCode = (List<string>)(HttpContext.Current.Session["SectionId"]);
                    }

                    Dictionary<long, string> data = DomainApiClient<Dto.User>.GetByAction<Dictionary<long, string>>
                        (DomainApiAction.User.GetReportingEmployees, null, presentationReportingEmployees);

                    foreach (KeyValuePair<long, string> item in data)
                    {
                        AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                        childNode.Text = item.Value;
                        childNode.Value = item.Key.ToString();
                        result.Add(childNode);
                    }
                }
                AutoCompleteBoxData res = new AutoCompleteBoxData();
                res.Items = result.OrderBy(m => m.Text).ToArray();
                return res;
            }
            catch
            {
                throw;
            }
        }
        public AutoCompleteBoxData GetPortalGroups(Dictionary<string, object> context)
        {
            long PortalEndpointID = 0;
            //if (HttpContext.Current.Session["PortalEndpointID"] != null)
            //{
            //    PortalEndpointID = (long)Convert.ChangeType(HttpContext.Current.Session["PortalEndpointID"], typeof(long));
            //}
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("EndpointSystemId", context["endpointSystemId"].ToString());
            parameter.Add("searchString", context["Text"].ToString());
            parameter.Add("endpointUserId", context["endpointUserId"].ToString());
            parameter.Add("actionType", context["actionType"].ToString());
            Dictionary<string, string> data = DomainApiClient<Dto.SAPEPGroupInfo>.GetByAction<Dictionary<string, string>>
                (DomainApiAction.Endpoint.SAPEP.SAPEPGroupInfo.GetPortalGroupsByActionType, parameter);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();

            foreach (KeyValuePair<string, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }
        public AutoCompleteBoxData GetPortalRoles(Dictionary<string, object> context)
        {
            //long PortalEndpointID = 0;
            //if (HttpContext.Current.Session["PortalEndpointID"] != null)
            //{
            //    PortalEndpointID = (long)Convert.ChangeType(HttpContext.Current.Session["PortalEndpointID"], typeof(long));
            //}

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("EndpointSystemId", context["endpointSystemId"].ToString());
            parameter.Add("searchString", context["Text"].ToString());
            parameter.Add("endpointUserId", context["endpointUserId"].ToString());
            parameter.Add("actionType", context["actionType"].ToString());
            Dictionary<string, string> data = DomainApiClient<Dto.SAPEPRoleInfo>.GetByAction<Dictionary<string, string>>
                (DomainApiAction.Endpoint.SAPEP.SAPEPRoleInfo.GetPortalRolesByActionType, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<string, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }
        public AutoCompleteBoxData GetAllSAPEntitlements(Dictionary<string, object> context)
        {
            long EndpointId = Convert.ToInt64(HttpContext.Current.Session["ComplianceEndpointId"]);
            string EntitlementType = Convert.ToString(HttpContext.Current.Session["EntitlementType"]);
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endpointId", EndpointId.ToString());
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = new Dictionary<long, string>();

            if (EntitlementType == "Role")
            {
                data = DomainApiClient<Dto.SAPRoleInfo>.GetByAction<Dictionary<long, string>>
                    (DomainApiAction.Endpoint.SAP.SAPRoleinfo.GetSAPRoleBySearchString, parameter);

                //data = allSapRoles.Where(item => item.ROLE_CODE.Contains(searchString))
                //               .Select(x => new { Key = x.Id, Value = x.ROLE_CODE })
                //               .Distinct().ToDictionary(o => o.Key, o => o.Value);
            }
            else if (EntitlementType == "Profiles")
            {
                data = DomainApiClient<Dto.SAPProfileInfo>.GetByAction<Dictionary<long, string>>
                    (DomainApiAction.Endpoint.SAP.SAPProfileInfo.GetSAPProfileBySearchString, parameter);

                //data = allSapProfiles.Where(item => item.PROFILE_CODE.Contains(searchString))
                //               .Select(x => new { Key = x.Id, Value = x.PROFILE_CODE })
                //               .Distinct().ToDictionary(o => o.Key, o => o.Value);
            }
            else if (EntitlementType == "T-Code")
            {
                data = DomainApiClient<Dto.SAPTCodeInfo>.GetByAction<Dictionary<long, string>>
                    (DomainApiAction.Endpoint.SAP.SAPTCodeInfo.GetSAPTCodeBySearchString, parameter);

                //data = allSapTCodes.Where(item => item.TCODE_Code.StartsWith(searchString))
                //               .Select(x => new { Key = x.Id, Value = x.TCODE_Code }).Take(1000)
                //               .Distinct().ToDictionary(o => o.Key, o => o.Value);
            }
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }

        public AutoCompleteBoxData GetAllOrgUnits(Dictionary<string, object> context)
        {
            string[] EntryValues = null;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.NOrganisation>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Org.Organisation.GetNOrganisations, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }

        public AutoCompleteBoxData GetEndPointSystemTypes(Dictionary<string, object> context)
        {
            try
            {
                string searchString = context["Text"].ToString().Replace("*", string.Empty);
                Dictionary<long, string> data;

                data = DomainApiClient<Dto.EndPointSystemType>.GetByAction<Dictionary<long, string>>
                    (DomainApiAction.Endpoint.EndPointSystemType.GetEndpointSystemType, null);
                List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();

                foreach (KeyValuePair<long, string> item in data)
                {
                    AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                    childNode.Text = item.Value;
                    childNode.Value = item.Key.ToString();
                    result.Add(childNode);
                }
                AutoCompleteBoxData res = new AutoCompleteBoxData();
                res.Items = result.OrderBy(m => m.Text).ToArray();
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public AutoCompleteBoxData GetAllEndPointsBySelecetedSystemTypes(Dictionary<string, object> context)
        {
            try
            {
                List<long> EndpointTypeIds = sessionStatus.AttestationEndPointTypeIds;

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endpointSystemTypeIdList", EndpointTypeIds.ToString());
                parameter.Add("displayNameSearchString", context["Text"].ToString());
                Dictionary<long, string> data = DomainApiClient<Dto.EndPoint>.
                GetByAction<Dictionary<long, string>>(DomainApiAction.Endpoint.EndPoint.GetByEndpointSystemTypeIds, parameter);

                List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
                foreach (KeyValuePair<long, string> item in data)
                {
                    AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                    childNode.Text = item.Value;
                    childNode.Value = item.Key.ToString();
                    result.Add(childNode);
                }
                AutoCompleteBoxData res = new AutoCompleteBoxData();
                res.Items = result.OrderBy(m => m.Text).ToArray();
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public AutoCompleteBoxData GetAllAttestationEndPointsBySelecetedSystemTypes(Dictionary<string, object> context)
        {
            try
            {
                List<long> EndpointTypeIds = sessionStatus.AttestationEndPointTypeIds;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("displayNameSearchString", context["Text"].ToString());
                Dictionary<long, string> data = DomainApiClient<Dto.EndPoint>.
                GetByAction<Dictionary<long, string>>(DomainApiAction.Endpoint.EndPoint.GetAttestationByEndpointSystemTypeIds, parameter, EndpointTypeIds);

                List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
                foreach (KeyValuePair<long, string> item in data)
                {
                    AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                    childNode.Text = item.Value;
                    childNode.Value = item.Key.ToString();
                    result.Add(childNode);
                }
                AutoCompleteBoxData res = new AutoCompleteBoxData();
                res.Items = result.OrderBy(m => m.Text).ToArray();
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public AutoCompleteBoxData GetUserObjectNameList(Dictionary<string, object> context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            Type ti = typeof(Dto.IEndpointUserInfo);
            var objectType = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                              from type in asm.GetTypes()
                              where type.IsClass && ti.IsAssignableFrom(type)
                              select type).ToList();
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            int count = 1;
            objectType.ForEach(lq =>
                {
                    AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                    childNode.Text = lq.FullName.Replace("DI.Chakra.Dto.", "").Replace("Presentation.", "").Trim();
                    childNode.Value = Convert.ToString(count);
                    if (string.IsNullOrEmpty(searchString))
                        result.Add(childNode);
                    else if (childNode.Text.ToLower().Contains(searchString.ToLower()))
                    {
                        result.Add(childNode);
                    }
                    count++;
                });
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }

        public AutoCompleteBoxData GetAllRiskScore(Dictionary<string, object> context)
        {
            string[] EntryValues = null;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.SODRiskScore>.
            GetByAction<Dictionary<long, string>>(DomainApiAction.Risks.SODRiskScore.GetBySearchString, parameter);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();

            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }
        public AutoCompleteBoxData GetFunctionItems(Dictionary<string, object> context)
        {
            string[] FunctionCode;
            string searchString = string.Empty;
            FunctionCode = new string[] { };
            string[] Items = context["Text"].ToString().Split(',');
            if (Items.Count() >= 2)
            {
                searchString = Items[0].ToString();
                FunctionCode = Items[1].ToString().Replace(";", string.Empty).Trim().Split('-');
            }
            string[] EntryValues = null;

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("inputParams", FunctionCode[0].ToString());
            parameter.Add("searchString", Items[0].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.SODFunctionItem>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.SODFunction.SODFunctionItem.GetBySearchString, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();

            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }
        public AutoCompleteBoxData GetPageURLs(Dictionary<string, object> context)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("url", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.PageUrl>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.ACL.PageUrl.GetByUrl, parameter);

            List<AutoCompleteBoxItemData> olistAutocomplete = new List<AutoCompleteBoxItemData>();
            foreach (var items in data)
            {
                AutoCompleteBoxItemData Children = new AutoCompleteBoxItemData();
                Children.Text = items.Value.ToString();
                Children.Value = items.Key.ToString();

                olistAutocomplete.Add(Children);
            }
            AutoCompleteBoxData result = new AutoCompleteBoxData();
            result.Items = olistAutocomplete.OrderBy(m => m.Text).ToArray();
            return result;
        }
        public AutoCompleteBoxData GetPages(Dictionary<string, object> context)
        {

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("name", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.WebPage>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.ACL.WebPage.GetByName, parameter);

            List<AutoCompleteBoxItemData> olistAutocomplete = new List<AutoCompleteBoxItemData>();
            foreach (var items in data)
            {
                AutoCompleteBoxItemData Children = new AutoCompleteBoxItemData();
                Children.Text = items.Value.ToString();
                Children.Value = items.Key.ToString();

                olistAutocomplete.Add(Children);
            }
            AutoCompleteBoxData result = new AutoCompleteBoxData();
            result.Items = olistAutocomplete.OrderBy(m => m.Text).ToArray();
            return result;
        }
        public AutoCompleteBoxData GetMenus(Dictionary<string, object> context)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("menuName", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.TabMenu>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.ACL.TabMenu.GetBySearchString, parameter);

            List<AutoCompleteBoxItemData> olistAutocomplete = new List<AutoCompleteBoxItemData>();
            foreach (var items in data)
            {
                AutoCompleteBoxItemData Children = new AutoCompleteBoxItemData();
                Children.Text = items.Value.ToString();
                Children.Value = items.Key.ToString();

                olistAutocomplete.Add(Children);
            }
            AutoCompleteBoxData result = new AutoCompleteBoxData();
            result.Items = olistAutocomplete.OrderBy(m => m.Text).ToArray();
            return result;
        }
        public AutoCompleteBoxData GetAllRiskCodes(Dictionary<string, object> context)
        {
            string[] EntryValues = null;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.SODRiskCode>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Risks.SODRiskCode.GetBySearchString, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }
        public AutoCompleteBoxData GetPasswordPolicies(Dictionary<string, object> context)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("displayNameSearchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.PasswordPolicy>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Endpoint.EndPoint.GetPasswordPolicies, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }

        public AutoCompleteBoxData GetDepartMents(Dictionary<string, object> context)
       {
            string[] EntryValues = null;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            parameter.Add("scopeID", "10-Dept");

            if (HttpContext.Current.Session["CompanyCode"] != null)
            {
                var CompanyCode = (string)(HttpContext.Current.Session["CompanyCode"]);
                parameter.Add("ParentCode", CompanyCode);
            }
            else
            {
                parameter.Add("ParentCode", null);
            } 
            
            Dictionary<string, string> data = DomainApiClient<Dto.NOrganisation>.GetByAction<Dictionary<string, string>>
                (DomainApiAction.Org.NOrganisation.GetOrganisations, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<string, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }

        public AutoCompleteBoxData GetSectionsByBranchCode(Dictionary<string, object> context)
        {
            string[] EntryValues = null;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            parameter.Add("scopeID", "40-Section");
            if (HttpContext.Current.Session["BranchCode"] != null)
            {
                var BranchCode = (string)(HttpContext.Current.Session["BranchCode"]);
                parameter.Add("ParentCode", BranchCode);
            }
            else
            {
                parameter.Add("ParentCode", null);
            }
            Dictionary<string, string> data = DomainApiClient<Dto.NOrganisation>.GetByAction<Dictionary<string, string>>
                (DomainApiAction.Org.NOrganisation.GetOrganisations, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<string, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }

        public AutoCompleteBoxData GetUnitsBySectionCode(Dictionary<string, object> context)
        {
            string[] EntryValues = null;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            parameter.Add("scopeID", "50-Unit");
            if (HttpContext.Current.Session["SectionCode"] != null)
            {
                var section = (string)(HttpContext.Current.Session["SectionCode"]);
                parameter.Add("ParentCode", section);
            }
            else
            {
                parameter.Add("ParentCode", null);
            }
            Dictionary<string, string> data = DomainApiClient<Dto.NOrganisation>.GetByAction<Dictionary<string, string>>
                (DomainApiAction.Org.NOrganisation.GetOrganisations, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<string, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }

        //GetSectionsByBranchCode
        public AutoCompleteBoxData GetDivisionsByDeptID(Dictionary<string, object> context)
        {
            string[] EntryValues = null;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            parameter.Add("scopeID", "30-Branch");
             
            if (HttpContext.Current.Session["DepartmentCode"] != null)
            {
                var departmentCode = (string)(HttpContext.Current.Session["DepartmentCode"]);
                parameter.Add("ParentCode", departmentCode);
            }
            else
            {
                parameter.Add("ParentCode", null);
            }

            Dictionary<string, string> data = DomainApiClient<Dto.NOrganisation>.GetByAction<Dictionary<string, string>>
                (DomainApiAction.Org.NOrganisation.GetOrganisations, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<string, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }

        public AutoCompleteBoxData GetDisticntSectionFromUsers(Dictionary<string, object> context)
        {
            string[] EntryValues = null;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<string, string> data = DomainApiClient<Dto.NOrganisation>.GetByAction<Dictionary<string, string>>
                (DomainApiAction.Org.NOrganisation.GetDisticntSectionFromUsers, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<string, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }

        public AutoCompleteBoxData GetCostCenterCodeAndTextByCompanyID(Dictionary<string, object> context)
        {
            string[] EntryValues = null;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());

            List<int> companyId = null;

            if (HttpContext.Current.Session["CompanyID"] != null)
            {
                companyId = (List<int>)(HttpContext.Current.Session["CompanyID"]);
            }

            parameter.Add("companyId", string.Join(",", companyId ?? new List<int>()));

            Dictionary<string, string> data = DomainApiClient<Dto.NOrganisation>.GetByAction<Dictionary<string, string>>
                (DomainApiAction.Org.NOrganisation.GetCostCenterCodeAndTextByCompanyID, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<string, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }
        public AutoCompleteBoxData GetBusinessUnits(Dictionary<string, object> context)
        {
            string[] EntryValues = null;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.NOrganisation>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Org.NOrganisation.GetBusinessUnits, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }
        public AutoCompleteBoxData GetCostCenters(Dictionary<string, object> context)
        {
            string[] EntryValues = null;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.NCostCenter>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Ruleset.CostCenter.GetNCostCenters, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }


        public AutoCompleteBoxData GetBulkBusinessRole(RadAutoCompleteContext context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            string subModule = Convert.ToString(HttpContext.Current.Session["SubModule"]);
            string Module = Convert.ToString(HttpContext.Current.Session["Module"]);
            BulkRequest request = (BulkRequest)HttpContext.Current.Session["BulkAdhocrequestObj"];
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            int EndpointUserId = 0;
            if (HttpContext.Current.Session["EndpointUserId"] != null)
            {
                EndpointUserId = Convert.ToInt32(HttpContext.Current.Session["EndpointUserId"]);
            }
            Dictionary<long, string> data = new Dictionary<long, string>();
            if (EndpointUserId == 0)
            {
                var param = new Dictionary<string, string> { { "searchString", searchString }, { "module", Module }, { "subModuleID", subModule }, { "UserID", request.BulkRequestDetails[0].UserId.ToString() } };
                data = DomainApiClient<SAPHRFLINSRoleMatrix>.GetByAction<Dictionary<long, string>>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.GetBusinessRoles, param);

            }
            else
            {
                var param = new Dictionary<string, string> { { "endpointId", EndpointUserId.ToString() }, { "_applicationType", Convert.ToString((int)ApplicationType.HRMS_FLINS) } };

                data = DomainApiClient<SAPRoleInfo>.GetByAction<Dictionary<long, string>>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.GetBusinessRoles, param);

            }
            foreach (KeyValuePair<long, string> item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            if (result.Count <= 0)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = "No Records";
                childNode.Value = "-1";
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();

            return res;
        }

        public AutoCompleteBoxData GetCompanies(Dictionary<string, object> context)
        {
            string[] EntryValues = null;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.Company>.GetByAction<Dictionary<long, string>>
                ("User", DomainApiAction.Org.NOrganisation.GetCompanies, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }

        public AutoCompleteBoxData GetInternalCompanies(Dictionary<string, object> context)
        {
            string[] EntryValues = null;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.NOrganisation>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Org.NOrganisation.GetInternalCompanies, parameter);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }

        public AutoCompleteBoxData GetExternalCompanies(Dictionary<string, object> context)
        {
            string[] EntryValues = null;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.NOrganisation>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Org.NOrganisation.GetExternalCompanies, parameter);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }

        public AutoCompleteBoxData GetLocations(Dictionary<string, object> context)
        {
            string[] EntryValues = null;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", context["Text"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.NOrganisation>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Org.NOrganisation.GetLocations, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            if (EntryValues != null)
            {
                res.Items = result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray();
            }
            else
            {
                res.Items = result.OrderBy(m => m.Text).ToArray();
            }
            return res;
        }

        public AutoCompleteBoxData GetOrganisationScopeUsers(Dictionary<string, object> context)
        {
            try
            {
                List<AutoCompleteBoxItemData> dataResult = new List<AutoCompleteBoxItemData>();
                string[] EntryValues = null;
                string organisationScopeId = string.Empty;
                if (HttpContext.Current.Session["OrganisationScopeId"] != null)
                    organisationScopeId = (string)Convert.ChangeType(HttpContext.Current.Session["OrganisationScopeId"], typeof(string));

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("searchString", context["Text"].ToString());
                parameter.Add("OrganisationScopeId", organisationScopeId.ToString());
                Dictionary<long, string> data = DomainApiClient<Dto.NOrganisation>.GetByAction<Dictionary<long, string>>
                    (DomainApiAction.Org.NOrganisation.GetOrganisationScopeUsers, parameter);

                foreach (KeyValuePair<long, string> item in data)
                {
                    AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                    childNode.Text = item.Value;
                    childNode.Value = item.Key.ToString();
                    dataResult.Add(childNode);
                }
                AutoCompleteBoxData res = new AutoCompleteBoxData();
                if (EntryValues != null)
                {
                    dataResult = dataResult.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToList();
                }
                else
                {
                    dataResult = dataResult.OrderBy(m => m.Text).ToList();
                }
                List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
                if (dataResult.Count > 0)
                {
                    AutoCompleteBoxItemData childFirstNode = new AutoCompleteBoxItemData();
                    childFirstNode.Text = "ALL";
                    childFirstNode.Value = "ALL";
                    result.Add(childFirstNode);
                    res.Items = result.Union(dataResult).ToArray();
                }
                return res;
            }
            catch
            {
                throw;
            }
        }

        public AutoCompleteBoxData GetUserPropertiesList(Dictionary<string, object> context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            Type ti = typeof(Dto.User);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            int count = 1;
            PropertyInfo[] properties = typeof(Dto.User).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo info in properties)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                if ((info.Name == "NOrganisation") || (info.Name == "NBusinessRole"))
                {
                    if (info.Name == "NOrganisation")
                    {
                        PropertyInfo[] propertiesVirtual = typeof(Dto.NOrganisation).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        foreach (PropertyInfo infoOrg in propertiesVirtual)
                        {
                            AutoCompleteBoxItemData childNodeOrg = new AutoCompleteBoxItemData();
                            childNodeOrg.Text = info.Name + "." + infoOrg.Name.Trim();
                            childNodeOrg.Value = Convert.ToString(count);
                            result.Add(childNodeOrg);
                            count++;
                        }
                    }
                    if (info.Name == "NBusinessRole")
                    {
                        PropertyInfo[] propertiesVirtual = typeof(Dto.NDesignation).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        foreach (PropertyInfo infoDesig in propertiesVirtual)
                        {
                            AutoCompleteBoxItemData childNodeDesig = new AutoCompleteBoxItemData();
                            childNodeDesig.Text = info.Name + "." + infoDesig.Name.Trim();
                            childNodeDesig.Value = Convert.ToString(count);
                            result.Add(childNodeDesig);
                            count++;
                        }
                    }
                }
                else
                {
                    childNode.Text = info.Name.Replace("DI.Chakra.Dto.", "").Replace("Presentation.", "").Trim();
                    childNode.Value = Convert.ToString(count);
                    result.Add(childNode);
                }
                count++;
            }

            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.OrderBy(m => m.Text).ToArray();
            return res;
        }

        public AutoCompleteBoxData GetFullTimeUsers(Dictionary<string, object> context)
        {
            try
            {
                List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
                string[] EntryValues = null;

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("searchString", context["Text"].ToString());
                Dictionary<long, string> data = DomainApiClient<Dto.User>.GetByAction<Dictionary<long, string>>
                    (DomainApiAction.User.GetFullTimeUsers, parameter);

                foreach (KeyValuePair<long, string> item in data)
                {
                    AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                    childNode.Text = item.Value;
                    childNode.Value = item.Key.ToString();
                    result.Add(childNode);
                }

                AutoCompleteBoxData res = new AutoCompleteBoxData();
                if (EntryValues != null)
                {
                    res.Items = (result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray());
                }
                else
                {
                    res.Items = result.OrderBy(m => m.Text).ToArray();
                }
                return res;
            }
            catch
            {
                throw;
            }
        }

        public AutoCompleteBoxData GetVendorUsers(Dictionary<string, object> context)
        {
            try
            {
                List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
                string[] EntryValues = null;

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("searchString", context["Text"].ToString());
                Dictionary<long, string> data = DomainApiClient<Dto.User>.GetByAction<Dictionary<long, string>>
                    (DomainApiAction.User.GetVendorUsers, parameter);

                foreach (KeyValuePair<long, string> item in data)
                {
                    AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                    childNode.Text = item.Value;
                    childNode.Value = item.Key.ToString();
                    result.Add(childNode);
                }

                AutoCompleteBoxData res = new AutoCompleteBoxData();
                if (EntryValues != null)
                {
                    res.Items = (result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray());
                }
                else
                {
                    res.Items = result.OrderBy(m => m.Text).ToArray();
                }
                return res;
            }
            catch
            {
                throw;
            }
        }
        public AutoCompleteBoxData GetUsersForSupervisor(Dictionary<string, object> context)
        {
            try
            {
                List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
                string[] EntryValues = null;

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("searchString", context["Text"].ToString());
                Dictionary<long, string> data = DomainApiClient<Dto.User>.GetByAction<Dictionary<long, string>>
                    (DomainApiAction.User.GetVendorUsers, parameter);

                foreach (KeyValuePair<long, string> item in data)
                {
                    AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                    childNode.Text = item.Value;
                    childNode.Value = item.Key.ToString();
                    result.Add(childNode);
                }

                AutoCompleteBoxData res = new AutoCompleteBoxData();
                if (EntryValues != null)
                {
                    res.Items = (result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray());
                }
                else
                {
                    res.Items = result.OrderBy(m => m.Text).ToArray();
                }
                return res;
            }
            catch
            {
                throw;
            }
        }

        public AutoCompleteBoxData GetEntitlementInHouse(Dictionary<string, object> context)
        {

            int EndpointSystemId = 0;
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            if (HttpContext.Current.Session["endpointSystemId"] != null)
            {
                EndpointSystemId = Convert.ToInt32(HttpContext.Current.Session["endpointSystemId"]);
            }
            Dictionary<long, string> data = new Dictionary<long, string>();
            if (EndpointSystemId != 0)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endpointId", EndpointSystemId.ToString());
                parameter.Add("searchString", searchString);
                data = DomainApiClient<InhousePermission>.
                    GetByAction<Dictionary<long, string>>(DomainApiAction.Endpoint.Presentation.INH.GetByEndPointSystemId, parameter);
            }
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (var item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;
        }

        public AutoCompleteBoxData GetEntitlementInHouseExisting(Dictionary<string, object> context)
        {
            int EndpointSystemId = 0;
            int endpointUserId = 0;
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            if (HttpContext.Current.Session["endpointSystemId"] != null)
            {
                EndpointSystemId = Convert.ToInt32(HttpContext.Current.Session["endpointSystemId"]);
            }

            if (HttpContext.Current.Session["endpointUserId"] != null)
            {
                endpointUserId = Convert.ToInt32(HttpContext.Current.Session["endpointUserId"]);
            }
            Dictionary<long, string> data = new Dictionary<long, string>();
            AutoCompleteBoxData res = new AutoCompleteBoxData();

            if (EndpointSystemId == 0 || endpointUserId == 0)
                return res;

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endPointSystemId", EndpointSystemId.ToString());
            parameter.Add("searchString", searchString);
            parameter.Add("endpointUserId", endpointUserId.ToString());
            data = DomainApiClient<InhousePermission>.
                GetByAction<Dictionary<long, string>>(DomainApiAction.Endpoint.Presentation.INH.GetByEndPointSystemIdExisting, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (var item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            res.Items = result.ToArray();
            return res;
        }

        public AutoCompleteBoxData GetAllPermissionsExceptExisting(Dictionary<string, object> context)
        {
            int EndpointSystemId = 0;
            int endpointUserId = 0;
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            if (HttpContext.Current.Session["endpointSystemId"] != null)
            {
                EndpointSystemId = Convert.ToInt32(HttpContext.Current.Session["endpointSystemId"]);
            }

            if (HttpContext.Current.Session["EndpointUserId"] != null)
            {
                endpointUserId = Convert.ToInt32(HttpContext.Current.Session["EndpointUserId"]);
            }
            Dictionary<long, string> data = new Dictionary<long, string>();
            AutoCompleteBoxData res = new AutoCompleteBoxData();

            if (EndpointSystemId == 0 || endpointUserId == 0)
                return res;

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endPointSystemId", EndpointSystemId.ToString());
            parameter.Add("searchString", searchString);
            parameter.Add("endpointUserId", endpointUserId.ToString());
            data = DomainApiClient<InhousePermission>.
                GetByAction<Dictionary<long, string>>(DomainApiAction.Endpoint.Presentation.INH.GetAllPermissionsExceptExisting, parameter);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (var item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            res.Items = result.ToArray();
            return res;
        }

        public AutoCompleteBoxData GetFunctionsInhouse(Dictionary<string, object> context)
        {

            int EndpointSystemId = 0;
            string permissionName = string.Empty;
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            if (HttpContext.Current.Session["endpointSystemId"] != null)
            {
                EndpointSystemId = Convert.ToInt32(HttpContext.Current.Session["endpointSystemId"]);
            }
            if (HttpContext.Current.Session["PermissionName"] != null)
            {
                permissionName = Convert.ToString(HttpContext.Current.Session["PermissionName"]);
            }
            Dictionary<long, string> data = new Dictionary<long, string>();
            if (EndpointSystemId != 0)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endpointId", EndpointSystemId.ToString());
                parameter.Add("permission", permissionName);
                parameter.Add("searchString", searchString);
                data = DomainApiClient<InhousePermission>.
                    GetByAction<Dictionary<long, string>>(DomainApiAction.Endpoint.Presentation.INH.GetByEndPointSystemId, parameter);
            }
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (var item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;
        }

        public AutoCompleteBoxData GetIdmsGroups(Dictionary<string, object> context)
        {
            int EndpointSystemId = 0;
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            AdHocActionType actionType = (AdHocActionType)HttpContext.Current.Session["Action"];
            if (HttpContext.Current.Session["EndpointUserId"] != null)
            {
                EndpointSystemId = Convert.ToInt32(HttpContext.Current.Session["EndpointUserId"]);
            }
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("Id", EndpointSystemId.ToString());
            parameter.Add("actionType", Convert.ToString((int)actionType));
            parameter.Add("searchString", searchString);

            Dictionary<string, string> data = new Dictionary<string, string>();
            data = DomainApiClient<Dto.User>.GetByAction<Dictionary<string, string>>(DomainApiAction.User.GetGroupsById, parameter);
            foreach (var item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key;
                result.Add(childNode);
            }

            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;
        }


        public AutoCompleteBoxData GetIdmsRoles(Dictionary<string, object> context)
        {
            int EndpointSystemId = 0;
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            AdHocActionType actionType = (AdHocActionType)HttpContext.Current.Session["Action"];
            if (HttpContext.Current.Session["EndpointUserId"] != null)
            {
                EndpointSystemId = Convert.ToInt32(HttpContext.Current.Session["EndpointUserId"]);
            }
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("Id", EndpointSystemId.ToString());
            parameter.Add("actionType", Convert.ToString((int)actionType));
            parameter.Add("searchString", searchString);

            Dictionary<string, string> data = new Dictionary<string, string>();
            data = DomainApiClient<Dto.User>.GetByAction<Dictionary<string, string>>(DomainApiAction.User.GetRolesById, parameter);
            foreach (var item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key;
                result.Add(childNode);
            }

            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;
        }

        public AutoCompleteBoxData GetEpCeRoles(Dictionary<string, object> context)
        {
            int EndpointUserId = 0;
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            AdHocActionType actionType = (AdHocActionType)HttpContext.Current.Session["Action"];
            string endpointsystemId = Convert.ToString((long)HttpContext.Current.Session["EndPointSystemId"]);
            if (HttpContext.Current.Session["EndpointUserId"] != null)
            {
                EndpointUserId = Convert.ToInt32(HttpContext.Current.Session["EndpointUserId"]);
            }
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("Id", EndpointUserId.ToString());
            parameter.Add("actionType", Convert.ToString((int)actionType));
            parameter.Add("searchString", searchString);
            parameter.Add("endpointsystemId", endpointsystemId);

            Dictionary<string, string> data = new Dictionary<string, string>();
            data = DomainApiClient<Dto.SAPEPRoleInfo>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.SAPEP.SAPEPRoleInfo.GetRolesById, parameter);
            foreach (var item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key;
                result.Add(childNode);
            }

            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;
        }

        public AutoCompleteBoxData GetEpCeGroups(Dictionary<string, object> context)
        {
            int EndpointUserId = 0;
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            AdHocActionType actionType = (AdHocActionType)HttpContext.Current.Session["Action"];
            string endpointsystemId = Convert.ToString((long)HttpContext.Current.Session["EndPointSystemId"]);
            if (HttpContext.Current.Session["EndpointUserId"] != null)
            {
                EndpointUserId = Convert.ToInt32(HttpContext.Current.Session["EndpointUserId"]);
            }
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("Id", EndpointUserId.ToString());
            parameter.Add("actionType", Convert.ToString((int)actionType));
            parameter.Add("searchString", searchString);
            parameter.Add("endpointsystemId", endpointsystemId);

            Dictionary<string, string> data = new Dictionary<string, string>();
            data = DomainApiClient<Dto.User>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.SAPEP.SAPEPGroupInfo.GetGroupsById, parameter);
            foreach (var item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key;
                result.Add(childNode);
            }

            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;
        }
        public AutoCompleteBoxData GetVPNApplications(Dictionary<string, object> context)
        {
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            bool IsFulltimeEmployee = true;
            var temp = context["IsFulltimeEmployee"].ToString();
            if (!string.IsNullOrWhiteSpace(temp))
            {
                IsFulltimeEmployee = Convert.ToBoolean(temp);
            }
            var AdhocActionType = context["AdhocActionType"].ToString();
            var vpnType = context["vpnType"].ToString();
            var endPointUserId = Convert.ToString(context["endPointUserId"]);
            var endpointId = Convert.ToString(context["endpointId"]);
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", searchString);
            parameter.Add("isFullTimeEmployee", IsFulltimeEmployee ? "true" : "false");
            parameter.Add("adhocActionType", AdhocActionType);
            parameter.Add("endPointUserId", endPointUserId);
            parameter.Add("endpointId", endpointId);
            parameter.Add("vpnType", vpnType);
            Dictionary<long, string> data = new Dictionary<long, string>();
            data = DomainApiClient<Dto.VPNMatrix>.
                GetByAction<Dictionary<long, string>>(DomainApiAction.Matrix.VPNMatrix.GetApplicationProjectMatrix, parameter);
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;
        }
        public AutoCompleteBoxData GetVPNApplicationsForBulkRequest(Dictionary<string, object> context)
        {
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            var vpnType = context["vpnType"].ToString();
            var endpointId = Convert.ToString(context["endpointId"]);
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", searchString);
            parameter.Add("vpnType", vpnType);
            Dictionary<long, string> data = new Dictionary<long, string>();
            data = DomainApiClient<Dto.VPNMatrix>.
                GetByAction<Dictionary<long, string>>(DomainApiAction.Matrix.VPNMatrix.GetApplicationForBulkRequest, parameter);
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;
        }

        public AutoCompleteBoxData GetAllTables(Dictionary<string, object> context)
        {
            try
            {
                List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
                string[] EntryValues = null;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("searchString", context["Text"].ToString());

                List<string> data = DomainApiClient<Dto.GEMSRoleMatrix>.GetByAction<List<string>>(DomainApiAction.Matrix.GEMS.GetAllTables, parameter);

                foreach (var item in data)
                {
                    AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                    childNode.Text = item;
                    childNode.Value = item;
                    result.Add(childNode);
                }

                AutoCompleteBoxData res = new AutoCompleteBoxData();
                if (EntryValues != null)
                {
                    res.Items = (result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray());
                }
                else
                {
                    res.Items = result.OrderBy(m => m.Text).ToArray();
                }
                return res;
            }
            catch
            {
                throw;
            }
        }
        public AutoCompleteBoxData GetSkypeForBusinessResponseGroup(Dictionary<string, object> context)
        {
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            var AdhocActionType = context["AdhocActionType"].ToString();
            var endPointUserId = Convert.ToString(context["endPointUserId"]);
            var endpointId = Convert.ToString(context["endpointId"]);
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", searchString);
            parameter.Add("adhocActionType", AdhocActionType);
            parameter.Add("endPointUserId", endPointUserId);
            parameter.Add("endpointId", endpointId);
            Dictionary<long, string> data = new Dictionary<long, string>();
            data = DomainApiClient<Dto.SkypeForBusinessResponseGroupInfo>.
                GetByAction<Dictionary<long, string>>(DomainApiAction.Endpoint.SkypeForBusiness.GetResponseGroup, parameter);
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;
        }
        public AutoCompleteBoxData GetResponseGroupForBulkRequestService(Dictionary<string, object> context)
        {
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            var AdhocActionType = context["AdhocActionType"].ToString();
            var endpointId = Convert.ToString(context["endpointId"]);
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", searchString);
            parameter.Add("adhocActionType", AdhocActionType);
            parameter.Add("endpointId", endpointId);
            Dictionary<long, string> data = new Dictionary<long, string>();
            data = DomainApiClient<Dto.SkypeForBusinessResponseGroupInfo>.
                GetByAction<Dictionary<long, string>>(DomainApiAction.Endpoint.SkypeForBusiness.GetResponseGroupForBulkRequest, parameter);
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;
        }
        AutoCompleteBoxData GetAutoCompleteBoxData(Dictionary<string, string> data)
        {
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            foreach (KeyValuePair<string, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }

            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;
        }
        public AutoCompleteBoxData GetOrgCompanies(Dictionary<string, object> context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            var param = new Dictionary<string, string>();
            param.Add("searchString", searchString);
            var data = DomainApiClient<Dto.User>.GetByAction<Dictionary<string, string>>(DomainApiAction.User.GetOrgCompanies, param);
            return GetAutoCompleteBoxData(data);
        }

        public AutoCompleteBoxData GetOrgDepartments(Dictionary<string, object> context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            var CompanyNameFilter = string.Empty;
            if (HttpContext.Current.Session["CompanyName"] != null)
            {
                CompanyNameFilter = Convert.ToString(HttpContext.Current.Session["CompanyName"]);
            }
            var param = new Dictionary<string, string>();
            param.Add("searchString", searchString);
            param.Add("CompanyNameFilter", CompanyNameFilter);
            var data = DomainApiClient<Dto.User>.GetByAction<Dictionary<string, string>>(DomainApiAction.User.GetOrgDepartments, param);
            return GetAutoCompleteBoxData(data);
        }

        public AutoCompleteBoxData GetOrgDivisions(Dictionary<string, object> context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            var DepartmentNameFilter = string.Empty;
            var CompanyNameFilter = string.Empty;
            if (HttpContext.Current.Session["DepartmentName"] != null)
            {
                DepartmentNameFilter = Convert.ToString(HttpContext.Current.Session["DepartmentName"]);
            }
            if (HttpContext.Current.Session["CompanyName"] != null)
            {
                CompanyNameFilter = Convert.ToString(HttpContext.Current.Session["CompanyName"]);
            }
            var param = new Dictionary<string, string>();
            param.Add("searchString", searchString);
            param.Add("CompanyNameFilter", CompanyNameFilter);
            param.Add("DepartNameFilter", DepartmentNameFilter);
            var data = DomainApiClient<Dto.User>.GetByAction<Dictionary<string, string>>(DomainApiAction.User.GetOrgDivisions, param);
            return GetAutoCompleteBoxData(data);
        }

        public AutoCompleteBoxData GetOrgBranches(Dictionary<string, object> context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);

            var DepartmentNameFilter = string.Empty;
            var CompanyNameFilter = string.Empty;
            var DivisionNameFilter = string.Empty;

            if (HttpContext.Current.Session["CompanyName"] != null)
            {
                CompanyNameFilter = Convert.ToString(HttpContext.Current.Session["CompanyName"]);
            }
            if (HttpContext.Current.Session["DepartmentName"] != null)
            {
                DepartmentNameFilter = Convert.ToString(HttpContext.Current.Session["DepartmentName"]);
            }
            if (HttpContext.Current.Session["DivisionName"] != null)
            {
                DivisionNameFilter = Convert.ToString(HttpContext.Current.Session["DivisionName"]);
            }

            var param = new Dictionary<string, string>();
            param.Add("searchString", searchString);
            param.Add("CompanyNameFilter", CompanyNameFilter);
            param.Add("DepartNameFilter", DepartmentNameFilter);
             
            var data = DomainApiClient<Dto.User>.GetByAction<Dictionary<string, string>>(DomainApiAction.User.GetOrgBranches, param);
            return GetAutoCompleteBoxData(data);
        }

        public AutoCompleteBoxData GetOrgSections(Dictionary<string, object> context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);

            var CompanyNameFilter = string.Empty;
            var DepartmentNameFilter = string.Empty;
            var DivisionNameFilter = string.Empty;
            var BranchNameFilter = string.Empty;

            if (HttpContext.Current.Session["CompanyName"] != null)
            {
                CompanyNameFilter = Convert.ToString(HttpContext.Current.Session["CompanyName"]);
            }
            if (HttpContext.Current.Session["DepartmentName"] != null)
            {
                DepartmentNameFilter = Convert.ToString(HttpContext.Current.Session["DepartmentName"]);
            }
            
            if (HttpContext.Current.Session["BranchName"] != null)
            {
                BranchNameFilter = Convert.ToString(HttpContext.Current.Session["BranchName"]);
            }

            var param = new Dictionary<string, string>();
            param.Add("searchString", searchString);
            param.Add("CompanyNameFilter", CompanyNameFilter);
            param.Add("DepartNameFilter", DepartmentNameFilter);
           
            param.Add("BranchNameFilter", BranchNameFilter);
            var data = DomainApiClient<Dto.User>.GetByAction<Dictionary<string, string>>(DomainApiAction.User.GetOrgSections, param);
            return GetAutoCompleteBoxData(data);
        }

        public AutoCompleteBoxData GetOrgUnits(Dictionary<string, object> context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);

            string CompanyNameFilter = string.Empty;
            string DepartmentNameFilter = string.Empty;
             
            string BranchNameFilter = string.Empty;
            string SectionNameFilter = string.Empty;
            if (HttpContext.Current.Session["CompanyName"] != null)
            {
                CompanyNameFilter = Convert.ToString(HttpContext.Current.Session["CompanyName"]);
            }
            if (HttpContext.Current.Session["DepartmentName"] != null)
            {
                DepartmentNameFilter = Convert.ToString(HttpContext.Current.Session["DepartmentName"]);
            }
             
            if (HttpContext.Current.Session["BranchName"] != null)
            {
                BranchNameFilter = Convert.ToString(HttpContext.Current.Session["BranchName"]);
            }
            if (HttpContext.Current.Session["SectionName"] != null)
            {
                SectionNameFilter = Convert.ToString(HttpContext.Current.Session["SectionName"]);
            }

            var param = new Dictionary<string, string>();
            param.Add("searchString", searchString);
            param.Add("CompanyNameFilter", CompanyNameFilter);
            param.Add("DepartNameFilter", DepartmentNameFilter);
            
            param.Add("BranchNameFilter", BranchNameFilter);
            param.Add("SectioNameFilter", SectionNameFilter);
            var data = DomainApiClient<Dto.User>.GetByAction<Dictionary<string, string>>(DomainApiAction.User.GetOrgUnits, param);
            return GetAutoCompleteBoxData(data);
        }
        public AutoCompleteBoxData GetReportingUsersOfThisManagerIDandAdmin_New(Dictionary<string, object> context)
        {
            try
            {
                string searchString = context["Text"].ToString().Replace("*", string.Empty);
                List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
                long managerid = Convert.ToInt32(new SessionStatusImpl().User.Id);

                string strCompany = string.Empty;
                string strDepartment = string.Empty;
                string strDivision = string.Empty;
                string strBranch = string.Empty;
                string strSection = string.Empty;
                string strUnit = string.Empty;

                string CompanyNameFilter = string.Empty;
                string DepartmentNameFilter = string.Empty;
               
                string BranchNameFilter = string.Empty;
                string SectionNameFilter = string.Empty;
                string UnitNameFilter = string.Empty;
                if (HttpContext.Current.Session["CompanyName"] != null)
                {
                    CompanyNameFilter = Convert.ToString(HttpContext.Current.Session["CompanyName"]);
                }
                if (HttpContext.Current.Session["DepartmentName"] != null)
                {
                    DepartmentNameFilter = Convert.ToString(HttpContext.Current.Session["DepartmentName"]);
                }
                 
                if (HttpContext.Current.Session["BranchName"] != null)
                {
                    BranchNameFilter = Convert.ToString(HttpContext.Current.Session["BranchName"]);
                }
                if (HttpContext.Current.Session["SectionName"] != null)
                {
                    SectionNameFilter = Convert.ToString(HttpContext.Current.Session["SectionName"]);
                }
                if (HttpContext.Current.Session["UnitName"] != null)
                {
                    UnitNameFilter = Convert.ToString(HttpContext.Current.Session["UnitName"]);
                }

                var param = new Dictionary<string, string>();
                param.Add("searchString", searchString);
                param.Add("CompanyNameFilter", CompanyNameFilter);
                param.Add("DepartNameFilter", DepartmentNameFilter);
                 
                param.Add("BranchNameFilter", BranchNameFilter);
                param.Add("GetOrgUnits", SectionNameFilter);
                param.Add("SectioNameFilter", SectionNameFilter);
                param.Add("UnitNameFilter", UnitNameFilter);
                param.Add("userId", managerid.ToString());
                var data = DomainApiClient<Dto.User>.GetByAction<Dictionary<string, string>>(DomainApiAction.User.GetReportingUsersOfThisManagerIDandAdmin_New, param);
                return GetAutoCompleteBoxData(data);
            }
            catch
            {
                throw;
            }
        }
        public AutoCompleteBoxData GetAllEmployeesUnderRO(Dictionary<string, object> context)
        {
            try
            {
                string searchString = context["Text"].ToString().Replace("*", string.Empty);
                List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
                var roEmployeeId = Convert.ToString(new SessionStatusImpl().User.EmployeeId);

                var param = new Dictionary<string, string>();
                param.Add("roEmployeeId", roEmployeeId);
                param.Add("searchString", searchString);
                var data = DomainApiClient<Dto.User>.GetByAction<Dictionary<string, string>>(DomainApiAction.User.GetAllEmployeesUnderRO, param);
                return GetAutoCompleteBoxData(data);
            }
            catch
            {
                throw;
            }
        }

        public AutoCompleteBoxData GetSkypeForBusinessReservedNumber(Dictionary<string, object> context)
        {
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            string endpointId = context["endpointId"].ToString().Replace("*", string.Empty);
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("search", searchString);
            parameter.Add("endpointId", endpointId);
            Dictionary<long, string> data = new Dictionary<long, string>();
            data = DomainApiClient<Dto.SkypeForBusinessUnAssignedNumberMatrix>.
                GetByAction<Dictionary<long, string>>(DomainApiAction.Endpoint.SkypeForBusiness.GetAllReservedNumbers, parameter);
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;
        }

        public AutoCompleteBoxData GetNovaJobByMatrixType(Dictionary<string, object> context)
        {
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("searchString", searchString);
            parameter.Add("AdhocActionType", context["AdhocActionType"].ToString());
            var type = (AdHocActionType)Enum.Parse(typeof(AdHocActionType), context["AdhocActionType"].ToString());
            parameter.Add("userId", context["userId"].ToString());
            parameter.Add("endpointId", context["endpointId"].ToString());
            parameter.Add("matrixType", context["matrixType"].ToString());
            parameter.Add("SAPSectionCode", context["SAPSection"].ToString().Trim());
            parameter.Add("DepartmentCode", context["DepartmentCode"].ToString());
            Dictionary<long, string> data = new Dictionary<long, string>();
            data = DomainApiClient<Dto.SAPEamNovaUserInfo>.
                GetByAction<Dictionary<long, string>>(DomainApiAction.Matrix.NOVA.GetNovaJobByMatrixType, parameter);
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;
        }

        public AutoCompleteBoxData GetGroupMembersbyUserId(Dictionary<string, object> context)
        {
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            //string[] EntryValues = null;// context["EntryValues"] as string[];
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            var param = new Dictionary<string, string>();
            param.Add("searchString", searchString);
            param.Add("UserId", new SessionStatusImpl().User.Id.ToString());
            var data = DomainApiClient<Dto.User>.
                GetByAction<Dictionary<long, string>>(DomainApiAction.User.GetGroupMembersbyUserId, param);
            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            //Dictionary<int, string> data = new UserFacade().GetGroupMembersbyUserId(searchString, new SessionStatusImpl().User.UserId);
            //foreach (KeyValuePair<int, string> item in data)
            //{

            //    AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
            //    childNode.Text = item.Value;
            //    childNode.Value = item.Key.ToString();
            //    result.Add(childNode);
            //}
            res.Items = result.ToArray();
            return res;
        }

        public AutoCompleteBoxData GetUsersByGroupId(Dictionary<string, object> context)
        {
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
            AutoCompleteBoxData res = new AutoCompleteBoxData();

            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            string groupId = context["GroupId"].ToString();

            var param = new Dictionary<string, string>();
            param.Add("searchString", searchString);
            param.Add("groupId", groupId.ToString());
            var data = DomainApiClient<Dto.User>.
                GetByAction<Dictionary<long, string>>(DomainApiAction.User.GetUsersByGroupId, param);

            foreach (KeyValuePair<long, string> item in data)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            res.Items = result.ToArray();
            return res;
        }
        public AutoCompleteBoxData GetUsersForCustomReport(Dictionary<string, object> context)
        {
            try
            {
                List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
                string[] EntryValues = null;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("searchString", context["Text"].ToString());
                Dictionary<long, string> data = new Dictionary<long, string>();
                    data = DomainApiClient<Dto.User>.GetByAction<Dictionary<long, string>>
                       (DomainApiAction.User.GetCustomReportUserBySearchString, parameter);

                foreach (KeyValuePair<long, string> item in data)
                {
                    AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                    childNode.Text = item.Value;
                    childNode.Value = item.Key.ToString();
                    result.Add(childNode);
                }

                AutoCompleteBoxData res = new AutoCompleteBoxData();
                if (EntryValues != null)
                {
                    res.Items = (result.Where(c => !EntryValues.Contains(c.Text)).OrderBy(m => m.Text).ToArray());
                }
                else
                {
                    res.Items = result.OrderBy(m => m.Text).ToArray();
                }
                return res;
            }
            catch
            {
                throw;
            }
        }
        #region SAPHANA

        public AutoCompleteBoxData GetAllPositionSAPHANA(Dictionary<string, object> context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            var endpointSystemId = Convert.ToInt64(context["EndpointSystemId"].ToString());

            string prefix = Convert.ToString(HttpContext.Current.Session["SelectedPrefix"]);
            string employeeType = Convert.ToString(HttpContext.Current.Session["employeeType"]);

            if (string.IsNullOrWhiteSpace(employeeType))
                employeeType = EmployeeType.Permanent.ToString();

            var list = DomainApiClient<Dto.SAPHANAXLPositionsMatrix>.GetByAction<IList<Dto.SAPHANAXLPositionsMatrix>>(DomainApiAction.Matrix.SAPHANA.BindPositionsSAPHANA,
                 new Dictionary<string, string> { { "endpointSystemId",endpointSystemId.ToString() }, { "prefix", prefix }, { "employeeType", employeeType } }
                );

            Dictionary<long, string> data = list.Where(lq => string.IsNullOrEmpty(searchString) ? true : lq.PositionText.ToLower().Contains(searchString.ToLower()))
                             .Select(x => new { Key = x.Id, Value = x.PositionText })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);

            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();

            foreach (KeyValuePair<long, string> item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
            return res;
        }

        public AutoCompleteBoxData GetExistingPositionSAPHANA(Dictionary<string, object> context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            var endpointSystemId = Convert.ToInt64(context["EndpointSystemId"].ToString());

            long endpointUserId = Convert.ToInt32(context["EndpointUserId"].ToString());
            string AccountName = HttpContext.Current.Session["UserAccountName"] == null ? string.Empty : Convert.ToString(HttpContext.Current.Session["UserAccountName"]);
            var sapUser = DomainApiClient<Dto.SAPHANAUserinfo>.GetByAction<Dto.SAPHANAUserinfo>(DomainApiAction.Endpoint.SAPHANA.SAPHANAUserinfo.GetSAPUserByUserId,
                new Dictionary<string, string> { { "userId", endpointUserId.ToString() }, { "UserAccountName", AccountName } });

            if (sapUser == null) return new AutoCompleteBoxData();
            var position = DomainApiClient<Dto.SAPHANAUserinfo>.GetByAction<Dictionary<long, string>>(DomainApiAction.Endpoint.SAPHANA.SAPHANAUserinfo.GetPositionTextByUser,
                 new Dictionary<string, string> {{ "endpointUserId", endpointUserId.ToString() },
                                                        { "endpointSystemId", sapUser.EndPointSystemId.ToString() },
                                                  });

            Dictionary<long, string> data = position.Where(lq => string.IsNullOrEmpty(searchString) ? true : lq.Value.ToLower().Contains(searchString.ToLower())).Select(x => new { Key = x.Key, Value = x.Value })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();

            foreach (KeyValuePair<long, string> item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }

            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();

            return res;
        }

        public AutoCompleteBoxData GetAllRolesSAPHANA(Dictionary<string, object> context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            var endpointSystemId = Convert.ToInt64(context["EndpointSystemId"].ToString());
            Dictionary<long, string> data = DomainApiClient<Dto.SAPHANAXLRolesMatrix>.GetByAction<IList<Dto.SAPHANAXLRolesMatrix>>(DomainApiAction.Matrix.SAPHANA.BindRolesSAPHANA,
                    new Dictionary<string, string> { { "endpointSystemId", endpointSystemId.ToString() } })
                    .Where(lq => string.IsNullOrEmpty(searchString) ? true : lq.RoleName.ToLower().Contains(searchString.ToLower()))
                     .Select(x => new { Key = x.Id, Value = x.RoleName })
                     .Distinct().ToDictionary(o => o.Key, o => o.Value);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();

            foreach (KeyValuePair<long, string> item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }

            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();

            return res;
        }

        public AutoCompleteBoxData GetExistingRolesSAPHANA(Dictionary<string, object> context)
        {
            string searchString = context["Text"].ToString().Replace("*", string.Empty);
            var endpointSystemId = Convert.ToInt64(context["EndpointSystemId"].ToString());
            int endpointUserId = Convert.ToInt32(context["EndpointUserId"].ToString());
            var ep = DomainApiClient<EndPoint>.GetById(endpointSystemId);
            var param = new Dictionary<string, string> { { "endpointUserId", endpointUserId.ToString() }, { "endpointId", Convert.ToString(endpointSystemId) } };

            Dictionary<long, string> data = DomainApiClient<SAPHANARoleInfo>.GetByAction<Dictionary<long, string>>(DomainApiAction.Matrix.SAPHANA.GetBusinessRoles, param);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();

            foreach (KeyValuePair<long, string> item in data.OrderBy(key => key.Value))
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = item.Value;
                childNode.Value = item.Key.ToString();
                result.Add(childNode);
            }

            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();

            return res;
        }

        #endregion
    }
}
