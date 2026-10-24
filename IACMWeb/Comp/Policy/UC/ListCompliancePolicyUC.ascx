<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListCompliancePolicyUC.ascx.cs" Inherits="IACMWeb.Comp.Policy.UC.ListCompliancePolicyUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-wrapper">
    <cc1:IACMRadGridControl ID="grdCompanyPolicies" runat="server" OnInit="grdCompanyPolicies_Init" OnNeedDataSource="grdCompanyPolicies_NeedDataSource" OnSelectedIndexChanged="grdCompanyPolicies_SelectedIndexChanged">
        <MasterTableView DataKeyNames="Id">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="CompliancePolicyCode" HeaderText="Code" SortExpression="CompliancePolicyCode" UniqueName="CompliancePolicyCode" DataType="System.String" HeaderStyle-Width="30%" FilterControlWidth="85%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="CompliancePolicyDescription" HeaderText="Description" SortExpression="CompliancePolicyDescription" UniqueName="CompliancePolicyDescription" DataType="System.String" HeaderStyle-Width="30%" FilterControlWidth="85%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="CompliancePolicyType" HeaderText="Type" SortExpression="CompliancePolicyType" UniqueName="CompliancePolicyType" DataType="System.String" HeaderStyle-Width="30%" FilterControlWidth="85%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="IsActive" HeaderText="Is Active" AllowSorting="True" UniqueName="IsActive" AllowFiltering="true" HeaderStyle-Width="10%" FilterControlWidth="80%">
                </telerik:GridBoundColumn>

            </Columns>
        </MasterTableView>
    </cc1:IACMRadGridControl>

    <%--<div class="buttons text-right nav-rectangle">
        <asp:Button ID="btncancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="btncancel_Click" />
    </div>--%>

</div>