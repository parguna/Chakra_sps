<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.ProvRS.DefaultApplication.UC.ListUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<cc1:IACMRadGridControl ID="RuleSetApplicationGrid" runat="server" OnNeedDataSource="NRuleSetApplicationGrid_NeedDataSource" OnSelectedIndexChanged="NRuleSetApplicationGrid_SelectedIndexChanged">
    <MasterTableView DataKeyNames="Id" TableLayout="Fixed"  Width="100%">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn DataField="RuleCode" FilterControlAltText="Ruleset" HeaderText="Rule" SortExpression="RuleCode" UniqueName="RuleCode" FilterControlWidth="56%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="EndpointSystemName" HeaderText="Endpoint" FilterControlAltText="DisplayName" SortExpression="Endpoint.DisplayName" UniqueName="EndpointSystemName" FilterControlWidth="56%">
            </telerik:GridBoundColumn>
            <telerik:GridDateTimeColumn DataField="CreatedDate" HeaderText="Created Date" SortExpression="CreatedDate" UniqueName="CreatedDate" FilterControlWidth="90%">
            </telerik:GridDateTimeColumn>
            <telerik:GridBoundColumn DataField="CreatedBy" HeaderText="Created By" SortExpression="CreatedBy" UniqueName="CreatedBy" FilterControlWidth="70%">
            </telerik:GridBoundColumn>
        </Columns>
    </MasterTableView>
</cc1:IACMRadGridControl>


