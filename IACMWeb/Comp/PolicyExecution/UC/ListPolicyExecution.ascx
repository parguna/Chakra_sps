<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListPolicyExecution.ascx.cs" Inherits="DI.Chakra.Web.Comp.PolicyExecution.UC.ListPolicyExecution" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-wrapper">
    <cc1:IACMRadGridControl ID="RadPolicyExecution" runat="server" OnInit="RadPolicyExecution_Init" OnNeedDataSource="RadPolicyExecution_NeedDataSource" OnSelectedIndexChanged="RadPolicyExecution_SelectedIndexChanged">
        <MasterTableView  DataKeyNames="Id">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="UniqueRunCode" HeaderText="Unique Run Code" SortExpression="UniqueRunCode" UniqueName="UniqueRunCode" DataType="System.String" HeaderStyle-Width="33%" FilterControlWidth="86.5%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="CompliancePolicy.CompliancePolicyCode" HeaderText="Compliance Policy Code" SortExpression="CompliancePolicy.CompliancePolicyCode" UniqueName="CompliancePolicyCode" DataType="System.String" HeaderStyle-Width="33%" FilterControlWidth="86.5%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="OutputType" HeaderText="Output Type" SortExpression="OutputType" UniqueName="OutputType" DataType="System.String" HeaderStyle-Width="33%" FilterControlWidth="86.5%">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </cc1:IACMRadGridControl>

    <%--<div class="buttons text-right nav-rectangle">
        <asp:Button ID="btncancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="btncancel_Click" />
    </div>--%>

</div>