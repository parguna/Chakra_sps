<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListMitigationUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.Mitigation.UC.ListMitigationUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<div class="content-wrapper">
    <cc1:IACMRadGridControl ID="RadMitigation" runat="server" OnNeedDataSource="RadMitigation_NeedDataSource" OnInit="RadMitigation_Init" OnSelectedIndexChanged="RadMitigation_SelectedIndexChanged">
        <MasterTableView  DataKeyNames="Id">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="false" ShowExportToCsvButton="false" />
            <Columns>
                <telerik:GridBoundColumn DataField="MitigationCode" HeaderText="Code" SortExpression="MitigationCode" UniqueName="MitigationCode" DataType="System.String" HeaderStyle-Width="23%" FilterControlWidth="80%"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="MitigationDescription" HeaderText="Description" SortExpression="MitigationDescription" UniqueName="MitigationDescription" DataType="System.String" HeaderStyle-Width="23%" FilterControlWidth="80%"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="IsActive" HeaderText="Active" SortExpression="IsActive" UniqueName="Active" DataType="System.Boolean" HeaderStyle-Width="8%" FilterControlWidth="79%"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="DocumentName" HeaderText="Document" SortExpression="DocumentName" UniqueName="Document" DataType="System.String" HeaderStyle-Width="23%" FilterControlWidth="80%"></telerik:GridBoundColumn>
                <telerik:GridDateTimeColumn DataField="DocumentUploadedDate" HeaderText="Document Date" SortExpression="DocumentUploadedDate"  UniqueName="DocumentDate" HeaderStyle-Width="23%" FilterControlWidth="80%" FilterControlAltText="DocumentUploadedDate">
                    <ColumnValidationSettings>
                        <ModelErrorMessage Text="" />
                    </ColumnValidationSettings>
                </telerik:GridDateTimeColumn>
            </Columns>
        </MasterTableView>
   </cc1:IACMRadGridControl>

</div>