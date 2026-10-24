<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EndpointListUC.ascx.cs" Inherits="DI.Chakra.Web.EndPoints.EndpointSystem.List.EndpointListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="uc1" TagName="EndpointUC" %>

<div class="content-wrapper">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-sm-6">
                    <uc1:EndpointUC runat="server" ID="EndpointUC" DisplayControl="ApplicationType" OnApplicationChanged="EndpointTypeChanged" />
                </div>
                <div class="col-sm-6">
                    <asp:Button ID="btnTestConnection" runat="server" CssClass="btn btn-default btn-sm" Text="Test Connection" OnClick="btnTestConnection_Click" />
                </div>
            </div>
        </div>
    </div>
</div>

<div class="content-wrapper">
    <cc1:IACMRadGridControl ID="ListOfEndpointRadGrid" runat="server" OnNeedDataSource="ListOfEndpointRadGrid_NeedDataSource" OnSelectedIndexChanged="ListOfEndpointRadGrid_SelectedIndexChanged">
        <MasterTableView NoMasterRecordsText="No Records Available" DataKeyNames="Id" TableLayout="Fixed" Width="100%">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn AllowFiltering="true" SortExpression="Id" UniqueName="Id" HeaderText="Id" DataField="Id" FilterControlAltText="Filter Id column" FilterControlWidth="80%" HeaderStyle-Width="30%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn AllowFiltering="true" SortExpression="DisplayName" UniqueName="DisplayName" HeaderText="Name" DataField="DisplayName" FilterControlAltText="Filter DisplayName column" FilterControlWidth="80%" HeaderStyle-Width="30%">
                </telerik:GridBoundColumn>
                <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" DataField="CreatedDate" HeaderText="Created Date" SortExpression="CreatedDate" UniqueName="CreatedDate" FilterControlAltText="Filter CreatedDate column" FilterControlWidth="80%" HeaderStyle-Width="30%">
                </telerik:GridDateTimeColumn>
                <telerik:GridBoundColumn AllowFiltering="true" DataField="Owner.DisplayName" HeaderText="Owner" SortExpression="Owner.DisplayName" UniqueName="Owner.DisplayName" FilterControlAltText="Filter Owner.DisplayName column" FilterControlWidth="80%" HeaderStyle-Width="30%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn AllowFiltering="true" DataField="IsActive" HeaderText="Is Active" SortExpression="IsActive" UniqueName="IsActive" FilterControlAltText="IsActive" FilterControlWidth="80%" HeaderStyle-Width="30%">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </cc1:IACMRadGridControl>

    <div class="buttons text-right nav-rectangle">
        <asp:Button ID="btncancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="btnCancel_Click" />
    </div>
    <%--<div class="buttons text-right nav-rectangle">
        <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClientClick="Redirect()"></asp:Button>
    </div>--%>
</div>
