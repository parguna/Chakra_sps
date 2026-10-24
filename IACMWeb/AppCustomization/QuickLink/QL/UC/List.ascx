<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="List.ascx.cs" Inherits="DI.Chakra.Web.AppCustomization.QuickLink.QL.UC.List" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<div class="content-wrapper">
    <cc1:IACMRadGridControl ID="gvQuicklink" runat="server" Width="100%"  OnNeedDataSource="gvQuicklink_NeedDataSource" OnSelectedIndexChanged="gvQuicklink_SelectedIndexChanged">
        <MasterTableView  NoMasterRecordsText="No Records Available" Width="100%"
            DataKeyNames="Id" TableLayout="Auto">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="Name" FilterControlAltText="Filter Name" HeaderText="Name" SortExpression="Name" UniqueName="Name" HeaderStyle-Width="100%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Category" FilterControlAltText="Filter Category" HeaderText="Category" SortExpression="Category" UniqueName="Category"  HeaderStyle-Width="100%">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
</cc1:IACMRadGridControl>
</div>