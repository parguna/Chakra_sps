using System;
using Telerik.Web.UI;
using DI.Chakra.Dto;
using System.Collections.Generic;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System.Linq;
using DI.Chakra.Logger;
using Application;

namespace DI.Chakra.Web.EndPoints.EndpointSystem.List
{
    public partial class EndpointListUC : UIBaseUC
    {
        private static IList<EndPoint> list = new List<EndPoint>();
        #region Method

        private void BindEndPointSystems()
        {

            if (EndpointUC.SelectedApplication == null)
            {
                list = DomainApiClient<Dto.EndPoint>.GetByAction<List<EndPoint>>(DomainApiAction.Endpoint.EndPoint.GetAllIncludingEndPointSystemType, null);
            }
            else
            {
                int EndPointSystemTypeId = Convert.ToInt32(EndpointUC.SelectedApplication.Id);

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endPointSystemTypeId", EndPointSystemTypeId.ToString());
                parameter.Add("hostName", null);
                list = DomainApiClient<EndPoint>.GetByAction<List<EndPoint>>(DomainApiAction.Endpoint.EndPoint.GetByEndpointTypeIncludingOwner, parameter);
            }
            this.ListOfEndpointRadGrid.DataSource = list;
        }

        private void Edit()
        {
            if (!ContextObjectId.HasValue)
                return;
        }

        private void delete()
        {
            if (!ContextObjectId.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item.");
                return;
            }
            try
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("key", "IsWOG");
                parameter.Add("value", Boolean.TrueString);
                EndPoint wogEp = DomainApiClient<Dto.EndPoint>.GetByAction<List<EndPoint>>(DomainApiAction.Endpoint.EndPoint.GetByEndpointConfiguration, parameter).FirstOrDefault();
                if (wogEp != null)
                {
                    if (wogEp.Id == this.ContextObjectId.Value)
                    {
                        parameter.Clear();
                        parameter.Add("key", "IsOneWayTrust");
                        parameter.Add("value", Boolean.TrueString);
                        List<EndPoint> oneWayTrustEps = DomainApiClient<Dto.EndPoint>.GetByAction<List<EndPoint>>(DomainApiAction.Endpoint.EndPoint.GetByEndpointConfiguration, parameter);
                        if (oneWayTrustEps.Count > 0)
                        {
                            MasterPage.ShowAlert(AlertType.Warning, "Delete the dependant endpoints before deleting WOD AD.");
                            return;
                        }
                    }
                }
                parameter.Clear();

                var usr = new SessionStatusImpl().User;
                EndPoint ep = DomainApiClient<EndPoint>.GetById(this.ContextObjectId.Value);
                parameter.Add("id", this.ContextObjectId.Value.ToString());
                bool isDeleted = DomainApiClient<EndPoint>.GetByAction<bool>(DomainApiAction.Endpoint.EndPoint.Delete, parameter); 
                ListOfEndpointRadGrid.Rebind();
                if (isDeleted)
                {
                    MasterPage.ShowAlert(AlertType.Success, "Endpoint Deleted Successfully");
                    DILog.UILogger.WriteLog(usr.Id, usr.DisplayName, LogLevelType.Info, $"Endpoint Deleted: Endpoint named {ep.DisplayName} deleted Successfully",null);

                }
                else
                    MasterPage.ShowAlert(AlertType.Error, "Endpoint Deleted Failed.");
                this.ContextObjectId = null;
            }
            catch (Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, string.Format("{0}{1}{2}", "Endpoint Deleted Failed.", System.Environment.NewLine, ex.Message));
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {

            this.MasterPage.ActionClicked += ActionClicked;
            if (!IsPostBack)
            {
                EndpointUC.Initialize();
            }
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            var usr = new SessionStatusImpl().User;
            if (e.ActionType == ACLAction.Edit)
            {
                DILog.UILogger.WriteLog(usr.Id, usr.DisplayName, LogLevelType.Info, "IDMS Endpoint - Edit",null);
                Edit();
            }

            if (e.ActionType == ACLAction.Detail)
            {
                Detail();
            }
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(usr.Id, usr.DisplayName, LogLevelType.Info, "IDMS Endpoint - Delete",null);
                delete();
            }
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                ListOfEndpointRadGrid.ExportSettings.FileName = "Endpoint";
                ListOfEndpointRadGrid.Export(ListOfEndpointRadGrid, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                ListOfEndpointRadGrid.Export<IList<EndPoint>>(list, e.ActionType, "Endpoint");
            }
        }

        private void Detail()
        {
            if (!ContextObjectId.HasValue)
                return;
        }

        protected void ListOfEndpointRadGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindEndPointSystems();
        }

        protected void EndpointTypeChanged(EndPointSystemType EndPointSystemType)
        {
            ListOfEndpointRadGrid.Rebind();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }

        protected void ListOfEndpointRadGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = ListOfEndpointRadGrid.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
            AdHocAccessRequest adhoc = new AdHocAccessRequest();
            UtilityMethods.GetEnumAttribute<AccountType>(adhoc.AccountType);
        }

        #endregion

        protected void btnTestConnection_Click(object sender, EventArgs e)
        {
            if (!ContextObjectId.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item.");
                return;
            }

            try
            {
                var list = new List<ServiceParameter>
                {
                new ServiceParameter{ Name = "endpointSystemId", Value = this.ContextObjectId.Value.ToString() }
                   };

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endpointSystemId", this.ContextObjectId.Value.ToString());

                var result = 
                DomainServiceClient.GetByAction<bool>
                   ("EndPointService", "IsEndpointConnected", parameter);

                if (result) 
                    MasterPage.ShowAlert(AlertType.Success, string.Format("{0}", "Connected Successfully"));
                else
                    MasterPage.ShowAlert(AlertType.Error, string.Format("{0}", "Connection Failed"));
            }
            catch (Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, string.Format("{0}{1}{2}", "Exception occured.", System.Environment.NewLine, ex.Message));
            }
        }
    }
}