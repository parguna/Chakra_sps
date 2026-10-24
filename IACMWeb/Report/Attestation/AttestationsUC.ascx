<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttestationsUC.ascx.cs" Inherits="DI.Chakra.Web.Report.Attestation.AttestationsUC" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<cc1:IACMRadGridControl runat="server" ID="AttesationDetail" AutoGenerateColumns="False" OnDetailTableDataBind="AttesationDetail_DetailTableDataBind" GroupPanelPosition="Top"
    OnNeedDataSource="AttesationDetail_NeedDataSource" AllowPaging="true" FilterMenu-CssClass="RadMenu_IACMFilterSkin" AllowFilteringByColumn ="true">
       <PagerStyle Mode="NextPrevAndNumeric" PageSizes="10" Height="100%" AlwaysVisible="true" />
    <ClientSettings>
        <Scrolling UseStaticHeaders="true" />
    </ClientSettings>
    <GroupingSettings CaseSensitive="false" />
    <MasterTableView AutoGenerateColumns="False" ShowHeadersWhenNoRecords="false" 
        CommandItemDisplay="None" EnableHeaderContextMenu="false" DataKeyNames="Id" 
        NoMasterRecordsText="No Records Available"  >
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn SortExpression="AttestationName" HeaderText="Attestation Name" HeaderButtonType="TextButton" DataField="AttestationName" HeaderStyle-Width="35%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="AttestationDescription" HeaderText="Attestation Description" HeaderButtonType="TextButton" DataField="AttestationDescription" HeaderStyle-Width="20%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="RequiresAttestationRemediation" HeaderText="Requires AttestationRemediation" HeaderButtonType="TextButton" DataField="RequiresAttestationRemediation" HeaderStyle-Width="17%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn UniqueName="WorkFlow" SortExpression="WorkFlow" HeaderText="WorkFlow" HeaderButtonType="TextButton" DataField="WorkFlow" HeaderStyle-Width="14%">
            </telerik:GridBoundColumn>
           <telerik:GridDateTimeColumn UniqueName="CreatedDate" SortExpression="CreatedDate" HeaderText="Created Date" HeaderButtonType="TextButton" DataField="CreatedDate" HeaderStyle-Width="20%">
            </telerik:GridDateTimeColumn>
        </Columns>
        <DetailTables>
            
            <telerik:GridTableView DataKeyNames="Id" Name="AttestationEndPoints" ShowHeadersWhenNoRecords="false" 
                NoDetailRecordsText="No Records found">
                <Columns>
                    <telerik:GridBoundColumn SortExpression="TargetSystem" HeaderText="Target System" HeaderButtonType="TextButton" DataField="TargetSystem" HeaderStyle-Width="34%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="ApplicationName" HeaderText="Application Name" HeaderButtonType="TextButton" DataField="ApplicationName" HeaderStyle-Width="36%">
                    </telerik:GridBoundColumn>
                   
                </Columns>
            </telerik:GridTableView>
            
        </DetailTables>
    </MasterTableView>
</cc1:IACMRadGridControl>