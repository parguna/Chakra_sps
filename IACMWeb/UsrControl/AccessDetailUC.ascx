<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccessDetailUC.ascx.cs" Inherits="DI.Chakra.Web.OrgChart.UC.AccessDetailUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadGrid runat="server" ID="AccountDetail" AutoGenerateColumns="False" OnDetailTableDataBind="AccountDetail_DetailTableDataBind" 
    GroupPanelPosition="Top" OnNeedDataSource="AccountDetail_NeedDataSource" AllowPaging="true" AllowSorting="true" 
    FilterMenu-CssClass="RadMenu_IACMFilterSkin" >
    <ClientSettings>
        <Scrolling UseStaticHeaders="true" AllowScroll="true" />
    </ClientSettings>
    <GroupingSettings CaseSensitive="false" />
    <MasterTableView AutoGenerateColumns="False" ShowHeadersWhenNoRecords="false" CommandItemDisplay="None" EnableHeaderContextMenu="false" DataKeyNames="AcountReconcilationId,EndpointUserId,EndPointSystemType" NoMasterRecordsText="No Records Available"  Width="100%">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn SortExpression="Name" HeaderText="Name" HeaderButtonType="TextButton" DataField="UserName" HeaderStyle-Width="35%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="FullName" HeaderText="Display Name" HeaderButtonType="TextButton" DataField="FullName" HeaderStyle-Width="20%">
            </telerik:GridBoundColumn>
            <telerik:GridDateTimeColumn SortExpression="LastLogon" HeaderText="Last Logon" HeaderButtonType="TextButton" DataField="LastLogonDate" HeaderStyle-Width="17%">
            </telerik:GridDateTimeColumn>
            <telerik:GridBoundColumn UniqueName="EndpointSystemName" SortExpression="EndpointSystemName" HeaderText="Endpoint System Name" HeaderButtonType="TextButton" DataField="EndpointSystemName" HeaderStyle-Width="20%">
            </telerik:GridBoundColumn>
        </Columns>
        <DetailTables>
            <telerik:GridTableView DataKeyNames="EndpointUserId,EndPointSystemType,Name" Name="Entitlements" ShowHeadersWhenNoRecords="false" NoDetailRecordsText="No Records found">
                <DetailTables>
                    <telerik:GridTableView DataKeyNames="EndpointUserId" Name="Permissions" ShowHeadersWhenNoRecords="false" NoDetailRecordsText="No Records found">
                        <Columns>
                            <telerik:GridBoundColumn SortExpression="Name" HeaderText="Name" HeaderButtonType="TextButton" DataField="Name" HeaderStyle-Width="34%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn SortExpression="Description" HeaderText="Description" HeaderButtonType="TextButton" DataField="Description" HeaderStyle-Width="36%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn SortExpression="Category" HeaderText="Category" HeaderButtonType="TextButton" DataField="Category" HeaderStyle-Width="27.4%">
                            </telerik:GridBoundColumn>
                        </Columns>
                    </telerik:GridTableView>
                </DetailTables>
                <Columns>
                    <telerik:GridBoundColumn SortExpression="Name" HeaderText="Name" HeaderButtonType="TextButton" DataField="Name" HeaderStyle-Width="34%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="Description" HeaderText="Description" HeaderButtonType="TextButton" DataField="Description" HeaderStyle-Width="36%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="Category" HeaderText="Category" HeaderButtonType="TextButton" DataField="Category" HeaderStyle-Width="27.4%">
                    </telerik:GridBoundColumn>
                </Columns>
            </telerik:GridTableView>
        </DetailTables>
    </MasterTableView>
</telerik:RadGrid>