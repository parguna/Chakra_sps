<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccessListUC.ascx.cs" Inherits="DI.Chakra.Web.Account.UC.AccessListUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<telerik:RadGrid ID="AccessList" runat="server" OnNeedDataSource="AccessList_NeedDataSource" AllowPaging="True" AllowSorting="True" CellSpacing="0"
    AllowFilteringByColumn="true" FilterMenu-CssClass="RadMenu_IACMFilterSkin">
    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" AlwaysVisible="true" />
    <ClientSettings>
        <Scrolling UseStaticHeaders="true" />
    </ClientSettings>
    <GroupingSettings CaseSensitive="false" />
    <MasterTableView AutoGenerateColumns="False" Width="100%" CommandItemDisplay="None" EnableHeaderContextMenu="false" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available" DataKeyNames="EndpointUserId">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn SortExpression="Name" HeaderText="Name" HeaderButtonType="TextButton" DataField="Name" HeaderStyle-Width="33.33%" FilterControlWidth="86.7%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="Description" HeaderText="Description" HeaderButtonType="TextButton" DataField="Description" HeaderStyle-Width="33.33%" FilterControlWidth="86.7%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="Category" HeaderText="Category" HeaderButtonType="TextButton" DataField="Category" HeaderStyle-Width="33.33%" FilterControlWidth="86.7%">
            </telerik:GridBoundColumn>
        </Columns>
    </MasterTableView>
</telerik:RadGrid>
