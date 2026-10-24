<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListConflictsUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.Conflict.UC.ListConflictsUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-wrapper">
    <cc1:IACMRadGridControl ID="RadConflicts" runat="server" OnNeedDataSource="RadConflicts_NeedDataSource" OnInit="RadConflicts_Init" OnSelectedIndexChanged="RadConflicts_SelectedIndexChanged">
        <MasterTableView  DataKeyNames="Id">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="SodConflictCode" HeaderText="Code" SortExpression="SodConflictCode" UniqueName="SodConflictCode" DataType="System.String" HeaderStyle-Width="25%" FilterControlWidth="83%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="SodConflictDescription" HeaderText="Description" SortExpression="SodConflictDescription" UniqueName="SodConflictDescription" DataType="System.String" HeaderStyle-Width="25%" FilterControlWidth="83%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="SodRiskCode.RiskCode" HeaderText="Risk Code" SortExpression="SodRiskCode.RiskCode" UniqueName="RiskCode" DataType="System.String" HeaderStyle-Width="20%" FilterControlWidth="78%">
                </telerik:GridBoundColumn>
                <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" DataField="LastGeneratedDate" FilterControlAltText="LastGeneratedDate" SortExpression="LastGeneratedDate" HeaderText="Last Generated Date" UniqueName="LastGeneratedDate" >
                </telerik:GridDateTimeColumn>
                <telerik:GridBoundColumn DataField="IsActive" HeaderText="Is Active" AllowSorting="True" UniqueName="IsActive" AllowFiltering="true" HeaderStyle-Width="10%" FilterControlWidth="80%">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
   </cc1:IACMRadGridControl>
    <%--<div class="buttons text-right nav-rectangle">
        <asp:Button ID="btncancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="btncancel_Click" />
    </div>--%>
</div>