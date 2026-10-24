<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="List.ascx.cs" Inherits="DI.Chakra.Web.AppCustomization.QuickLink.DefaultRoleQL.UC.List" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<div class="content-wrapper">

    <cc1:IACMRadGridControl ID="gvDefaultRoleQL" FilterMenu-CssClass="RadMenu_IACMFilterSkin" OnNeedDataSource="gvDefaultRoleQL_NeedDataSource"
        AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" runat="server" AllowFilteringByColumn="true" AllowPaging="true"
        AllowSorting="true" CellSpacing="0" GridLines="None" ClientSettings-EnablePostBackOnRowClick="true" OnSelectedIndexChanged="gvDefaultRoleQL_SelectedIndexChanged">
               
         <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available"
            EnableHeaderContextMenu="false" DataKeyNames="Id" TableLayout="Fixed">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
                
                <telerik:GridBoundColumn DataField="Name" FilterControlAltText="Filter Name " HeaderText="Name" SortExpression="Name" UniqueName="Name" FilterControlWidth="83%">
                </telerik:GridBoundColumn>
                 <telerik:GridBoundColumn DataField="Category" FilterControlAltText="Filter Category" HeaderText="Category" SortExpression="Category" UniqueName="Category" FilterControlWidth="83%">
                </telerik:GridBoundColumn>
                 <telerik:GridBoundColumn DataField="RoleName" FilterControlAltText="Filter Role Name" HeaderText="Role Name" SortExpression="RoleName" UniqueName="RoleName" FilterControlWidth="83%">
                </telerik:GridBoundColumn>
                  <telerik:GridBoundColumn DataField="Position" FilterControlAltText="Filter Position" HeaderText="Position" SortExpression="Position" UniqueName="Position" FilterControlWidth="83%">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </cc1:IACMRadGridControl>
</div>

<div class="buttons text-right nav-rectangle">
    <asp:Button runat="server" class="btn btn-default btn-sm" ID="btnCancel" Text="Cancel" UseSubmitBehavior="false" OnClick="btnCancel_Click"></asp:Button>
</div>

