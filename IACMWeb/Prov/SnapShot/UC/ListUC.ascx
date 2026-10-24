<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Prov.SnapShot.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="UserSearchUC" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
 <style>
    .HoverPopup
        {
            background-color: #2077BD;
            color: White;
            padding: 5px;
            display:none;
        }.list-group{
		background-color: #2077BD;
		}
		.list-group-item{
			background-color: #2077BD;
		}
</style>
<div class="content-wrapper">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-12">
                    <div class="form-group">
                        <label class="col-xs-1 control-label">User Name</label>
                        <div class="col-xs-4">
                            <uc1:UserSearchUC runat="server" ID="UserSearchUC" />
                        </div>
                        <div class="col-xs-1">
                            <asp:Button ID="btnReprt" runat="server" CssClass="btn btn-default btn-sm" Text="View" OnClick="btnReprt_Click" />
                        </div>
						<div class="col-xs-2">
                            <asp:CheckBox ID="chkOlder" runat="server" Checked="false" Text="Show old Snapshot" AutoPostBack="true" OnCheckedChanged="chkOlder_CheckedChanged"/>
                        </div>
                        <div class="col-xs-4">
                            <asp:Button ID="btnUndo" runat="server" CssClass="btn btn-default btn-sm" Text="Undo Transfer/Offboard" OnClick="btnUndo_Click" />
                            &nbsp;&nbsp; <asp:LinkButton  CssClass="icon icon-info-sign icon-entitlement-info" ID="ReqInfo" runat="server"></asp:LinkButton>
                            <asp:HoverMenuExtender ID="hoverDisplayName" runat="server" PopupControlID="pnlDescription" PopupPosition="Center" TargetControlID="ReqInfo"></asp:HoverMenuExtender>
                            <asp:Panel ID="pnlDescription" runat="server" CssClass="HoverPopup">
                                <ul class="list-group">
                                    <li class="list-group-item">1. Undo can be done with in 30 days and Employee must be active before initiate Undo</li>
                                    <li class="list-group-item">2. To see more than 30 days snapshop please click <b>Show old Snapshot</b></li>
                                </ul>
                            </asp:Panel>
                        </div>
                        
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<asp:Panel runat="server" ID="GrdPanel" Visible="false">
    <cc1:IACMRadGridControl ID="grdsnapshot" runat="server" OnNeedDataSource="grdsnapshot_NeedDataSource" OnPreRender="grdsnapshot_PreRender">
        <MasterTableView ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available" Width="100%">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridDateTimeColumn DataField="CreatedDate" FilterControlAltText="CreatedDate" SortExpression="CreatedDate" HeaderText="Created Date" UniqueName="CreatedDate" FilterControlWidth="50%">
                </telerik:GridDateTimeColumn>
                <telerik:GridBoundColumn DataField="EmployeeEventType" SortExpression="EmployeeEventType" HeaderText="Employeee Event" UniqueName="EmployeeEventType" FilterControlWidth="50%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="UserName" FilterControlAltText="UserName" SortExpression="UserName" HeaderText="User Name" UniqueName="UserName" FilterControlWidth="50%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="EmployeeNo" FilterControlAltText="EmployeeNo" SortExpression="EmployeeNo" HeaderText="Employee Id" UniqueName="EmployeeNo" FilterControlWidth="50%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="EmailID" FilterControlAltText="EmailID" SortExpression="EmailID" HeaderText="EmailID" UniqueName="EmailID" FilterControlWidth="50%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="EndpointSystemType" FilterControlAltText="EndpointSystemType" SortExpression="EndpointSystemType" HeaderText="EndpointSystem Type" UniqueName="EndpointSystemType">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="EndpointsystemName" FilterControlAltText="EndpointsystemName" SortExpression="EndpointsystemName" HeaderText="Endpoint System Name" UniqueName="EndpointsystemName" FilterControlWidth="50%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="AccountName" FilterControlAltText="AccountName" SortExpression="AccountName" HeaderText="Account Name" UniqueName="AccountName" FilterControlWidth="50%">
                </telerik:GridBoundColumn>
                <%--<telerik:GridBoundColumn DataField="AccountType" FilterControlAltText="AccountType" SortExpression="AccountType" HeaderText="Account Type" UniqueName="AccountType" FilterControlWidth="50%">
                </telerik:GridBoundColumn>--%>
                <telerik:GridBoundColumn DataField="Entitlements" FilterControlAltText="Entitlement" SortExpression="Entitlement" HeaderText="Entitlement" UniqueName="Entitlement" FilterControlWidth="50%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="EntitlementType" FilterControlAltText="EntitlementType" SortExpression="EntitlementType" HeaderText="Entitlement Type" UniqueName="EntitlementType" FilterControlWidth="50%">
                </telerik:GridBoundColumn>

                
            </Columns>
        </MasterTableView>
    </cc1:IACMRadGridControl>
</asp:Panel>

<div class="buttons text-right nav-rectangle">
    <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClientClick="Redirect()"></asp:Button>
</div>
