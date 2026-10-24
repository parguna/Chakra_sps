<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="List.ascx.cs" Inherits="DI.Chakra.Web.ACL.TabMenu.UC.List" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-wrapper">
    <cc1:IACMRadGridControl ID="gvTabMenu" runat="server" OnNeedDataSource="gvTabMenu_NeedDataSource" OnSelectedIndexChanged="gvTabMenu_SelectedIndexChanged">
         <MasterTableView ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available" Width="100%" 
            DataKeyNames="Id" TableLayout="Fixed">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="UniqueName" FilterControlAltText="Filter Unique Name " HeaderText="Unique Name" SortExpression="UniqueName" UniqueName="UniqueName" HeaderStyle-Width="25%" FilterControlWidth="82%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="MenuName" FilterControlAltText="Filter MenuName" HeaderText="Menu Name" SortExpression="MenuName" UniqueName="MenuName" HeaderStyle-Width="25%" FilterControlWidth="82%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="ParentMenuName" FilterControlAltText="Filter Parent Menu" HeaderText="Parent TabMenu Id" SortExpression="ParentTabMenuId" UniqueName="ParentTabMenuId" HeaderStyle-Width="25%" FilterControlWidth="82%">
                </telerik:GridBoundColumn>
                 <telerik:GridBoundColumn DataField="PageName" FilterControlAltText="Filter Page Name" HeaderText="Page Name" SortExpression="Name" UniqueName="Name" HeaderStyle-Width="25%" FilterControlWidth="82%">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </cc1:IACMRadGridControl>

</div>

