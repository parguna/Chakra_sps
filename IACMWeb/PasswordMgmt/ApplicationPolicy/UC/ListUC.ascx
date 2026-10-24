<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.PasswordMgmt.ApplicationPolicy.UC.ListUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<cc1:IACMRadGridControl ID="grdViewAllRequest" AllowFilteringByColumn="true" FilterMenu-CssClass="RadMenu_IACMFilterSkin" ClientSettings-EnablePostBackOnRowClick="true" OnNeedDataSource="grdViewAllRequest_NeedDataSource" OnSelectedIndexChanged="grdViewAllRequest_SelectedIndexChanged" AllowPaging="True" AllowSorting="True" CellSpacing="0" runat="server">
    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" AlwaysVisible="true" />
    <ClientSettings>
        <Scrolling UseStaticHeaders="true" />
        <Selecting AllowRowSelect="true" />
    </ClientSettings>

    <GroupingSettings CaseSensitive="false" />
    <MasterTableView DataKeyNames="Id" AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="false" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn SortExpression="Name" UniqueName="Name" FilterControlAltText="Name" FilterControlWidth="73%"
                HeaderText="Policy Name" DataField="Name" HeaderStyle-Width="65%">
            </telerik:GridBoundColumn>
             <telerik:GridBoundColumn SortExpression="PasswordLength" UniqueName="PasswordLength" FilterControlAltText="PasswordLength" FilterControlWidth="78%"
                HeaderText="Password Length" DataField="PasswordLength" HeaderStyle-Width="40%">
            </telerik:GridBoundColumn>
             <telerik:GridBoundColumn SortExpression="PasswordMaximumLength" UniqueName="PasswordMaximumLength" FilterControlAltText="PasswordMaximumLength" FilterControlWidth="73%"
                HeaderText=" Maximum password length" DataField="PasswordMaximumLength" HeaderStyle-Width="80.5%">
            </telerik:GridBoundColumn>

        </Columns>
    </MasterTableView>
</cc1:IACMRadGridControl>
