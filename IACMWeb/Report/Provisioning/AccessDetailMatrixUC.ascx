<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccessDetailMatrixUC.ascx.cs" Inherits="DI.Chakra.Web.Report.Provisioning.AccessDetailMatrixUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadGrid runat="server" ID="AccountDetail" AutoGenerateColumns="False" OnDetailTableDataBind="AccountDetail_DetailTableDataBind" GroupPanelPosition="Top"
    OnNeedDataSource="AccountDetail_NeedDataSource" AllowPaging="true" FilterMenu-CssClass="RadMenu_IACMFilterSkin" AllowFilteringByColumn ="true">
       <PagerStyle Mode="NextPrevAndNumeric" PageSizes="10" Height="100%" AlwaysVisible="true" />
    <ClientSettings>
        <Scrolling UseStaticHeaders="true" />
    </ClientSettings>
    <GroupingSettings CaseSensitive="false" />
    <MasterTableView AutoGenerateColumns="False" ShowHeadersWhenNoRecords="false"   Width="100%"
        CommandItemDisplay="None" EnableHeaderContextMenu="false" DataKeyNames="AcountReconcilationId,EndpointUserId,EndPointSystemType" 
        NoMasterRecordsText="No Records Available"  >
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn SortExpression="Name" HeaderText="Name" HeaderButtonType="TextButton" DataField="UserName" HeaderStyle-Width="35%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="FullName" HeaderText="Display Name" HeaderButtonType="TextButton" DataField="FullName" HeaderStyle-Width="20%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="LastLogon" HeaderText="Last Logon" HeaderButtonType="TextButton" DataField="LastLogonDate" HeaderStyle-Width="17%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn UniqueName="EndpointSystemName" SortExpression="EndpointSystemName" HeaderText="Endpoint System Name" HeaderButtonType="TextButton" DataField="EndpointSystemName" HeaderStyle-Width="20%">
            </telerik:GridBoundColumn>
           
        </Columns>
        <DetailTables>
            <telerik:GridTableView DataKeyNames="EndpointUserId" Name="Entitlements" ShowHeadersWhenNoRecords="false" NoDetailRecordsText="No Records found">
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