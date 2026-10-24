<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.SODPolicyConfig.UC.ListUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<cc1:IACMRadGridControl ID="gvRisklevel" runat="server" OnSelectedIndexChanged="gvRisklevel_SelectedIndexChanged" OnNeedDataSource="gvRisklevel_NeedDataSource"
    OnItemDataBound ="gvRisklevel_ItemDataBound">
    <MasterTableView  ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available"
         DataKeyNames="Id" TableLayout="Fixed">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn DataField="SODRiskLevel.RiskLevelCode" FilterControlAltText="SODRiskLevel.RiskLevelCode" SortExpression="SODRiskLevel.RiskLevelCode" HeaderText="RiskLevel" UniqueName="SODRiskLevel.RiskLevelCode" HeaderStyle-Width="25%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Operator" FilterControlAltText="Operator" SortExpression="Operator" HeaderText="Operator" UniqueName="Operator" HeaderStyle-Width="25%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="RiskScoreOperator" FilterControlAltText="RiskScoreOperator" SortExpression="RiskScoreOperator" HeaderText="RiskScoreOperator" UniqueName="RiskScoreOperator" HeaderStyle-Width="25%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Riskscore" FilterControlAltText="RiskScore" SortExpression="Riskscore" HeaderText="RiskScore" UniqueName="RiskScore" HeaderStyle-Width="20%" FilterControlWidth="77%">
            </telerik:GridBoundColumn>
            <%--<telerik:GridBoundColumn DataField="Allowed" FilterControlAltText="Allowed" SortExpression="Allowed" HeaderText="Allowed" UniqueName="Allowed" HeaderStyle-Width="15%" FilterControlWidth="68%">
            </telerik:GridBoundColumn>     --%>     
        </Columns>
    </MasterTableView>
</cc1:IACMRadGridControl>