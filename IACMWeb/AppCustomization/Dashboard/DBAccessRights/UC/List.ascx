<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="List.ascx.cs" Inherits="DI.Chakra.Web.AppCustomization.Dashboard.DBAccessRights.UC.List" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
   
<div class="content-wrapper">

    <cc1:IACMRadGridControl ID="gvDBAccessRights" FilterMenu-CssClass="RadMenu_IACMFilterSkin" OnNeedDataSource="gvDBAccessRights_NeedDataSource"
        AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" runat="server" AllowFilteringByColumn="true" AllowPaging="true"
        AllowSorting="true" CellSpacing="0" GridLines="None" ClientSettings-EnablePostBackOnRowClick="true" OnSelectedIndexChanged="gvDBAccessRights_SelectedIndexChanged">
         <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available"
            EnableHeaderContextMenu="false" DataKeyNames="Id" TableLayout="Fixed"  Width="100%" >
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
                
                <telerik:GridBoundColumn DataField="Dashboard.DashboardName" FilterControlAltText="Filter Name " HeaderText="Name" SortExpression="DashboardName" UniqueName="DashboardName">
                </telerik:GridBoundColumn>
                 <telerik:GridBoundColumn DataField="Dashboard.DashboardCategory" FilterControlAltText="Filter Category" HeaderText="Category" SortExpression="DashboardCategory" UniqueName="DashboardCategory">
                </telerik:GridBoundColumn>
                 <telerik:GridBoundColumn DataField="Dashboard.DashBoardType" FilterControlAltText="Filter Type" HeaderText="Type" SortExpression="DashBoardType" UniqueName="DashBoardType">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Dashboard.TargetUrl" FilterControlAltText="Filter Target Url " HeaderText="Target Url" SortExpression="TargetUrl" UniqueName="TargetUrl">
                </telerik:GridBoundColumn>
                 <telerik:GridBoundColumn DataField="Dashboard.NavigateUrl" FilterControlAltText="Filter Navigate Url" HeaderText="Navigate Url" SortExpression="NavigateUrl" UniqueName="NavigateUrl">
                </telerik:GridBoundColumn>
                 <telerik:GridBoundColumn DataField="Role.RoleName" FilterControlAltText="Filter Role Name" HeaderText="Role Name" SortExpression="RoleName" UniqueName="RoleName">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </cc1:IACMRadGridControl>
</div>

<div class="buttons text-right nav-rectangle">
    <asp:Button runat="server" class="btn btn-default btn-sm" ID="btnCancel" Text="Cancel" UseSubmitBehavior="false" OnClick="btnCancel_Click" /></asp:Button>
</div>
