<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrganisationUC.ascx.cs" Inherits="DI.Chakra.Web.Report.Provisioning.OrganisationUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<telerik:RadGrid runat="server" ID="OrgDetail" AutoGenerateColumns="False"  GroupPanelPosition="Top"
    OnNeedDataSource="OrgDetail_NeedDataSource" AllowPaging="true" FilterMenu-CssClass="RadMenu_IACMFilterSkin" AllowFilteringByColumn ="true">
       <PagerStyle Mode="NextPrevAndNumeric" PageSizes="10" Height="100%" AlwaysVisible="true" />
    <ClientSettings>
        <Scrolling UseStaticHeaders="true" />
    </ClientSettings>
    <GroupingSettings CaseSensitive="false" />
    <MasterTableView AutoGenerateColumns="False" ShowHeadersWhenNoRecords="false"   Width="100%"
        CommandItemDisplay="None" EnableHeaderContextMenu="false"
        NoMasterRecordsText="No Records Available"  >
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
           <telerik:GridBoundColumn SortExpression="OrgUnitPrefix" HeaderText="Org UnitPrefix" HeaderButtonType="TextButton" DataField="OrgUnitPrefix" HeaderStyle-Width="20%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="OrganisationName" HeaderText="Organisation Name" HeaderButtonType="TextButton" DataField="OrganisationName" HeaderStyle-Width="17%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn UniqueName="ParentOrganisation" SortExpression="ParentOrganisation" HeaderText="Parent Organisation" HeaderButtonType="TextButton" DataField="ParentOrganisation" HeaderStyle-Width="20%">
            </telerik:GridBoundColumn>
           <telerik:GridBoundColumn UniqueName="Scope" SortExpression="Scope" HeaderText="Scope" HeaderButtonType="TextButton" DataField="Scope" HeaderStyle-Width="20%">
            </telerik:GridBoundColumn>
             <telerik:GridBoundColumn UniqueName="DisplayName" SortExpression="DisplayName" HeaderText="Head" HeaderButtonType="TextButton" DataField="DisplayName" HeaderStyle-Width="20%">
            </telerik:GridBoundColumn>
        </Columns>
       
    </MasterTableView>
</telerik:RadGrid>