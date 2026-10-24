<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListFunctionUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.Function.UC.ListFunctionUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-wrapper">   
    <cc1:IACMRadGridControl ID="RadFunctions" runat="server"  OnNeedDataSource="RadFunctions_NeedDataSource" OnSelectedIndexChanged ="RadFunctions_SelectedIndexChanged" OnInit="RadFunctions_Init">
        <MasterTableView DataKeyNames="Id">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="SODFunctionCode" HeaderText="Code" SortExpression="SODFunctionCode" UniqueName="SODFunctionCode" DataType="System.String" HeaderStyle-Width="45%" FilterControlWidth="90.5%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="SodFunctionDescription" HeaderText="Description" SortExpression="SodFunctionDescription" UniqueName="SodFunctionDescription" DataType="System.String" HeaderStyle-Width="45%" FilterControlWidth="90.5%">
                </telerik:GridBoundColumn>
                  <telerik:GridBoundColumn DataField="IsActive" HeaderText="Is Active" AllowSorting="True" UniqueName="IsActive" AllowFiltering="true" DataType="System.Boolean" HeaderStyle-Width="10%" FilterControlWidth="80%">
                </telerik:GridBoundColumn>
               
            </Columns>
        </MasterTableView>
   </cc1:IACMRadGridControl>
    <div class="buttons text-right nav-rectangle">
        <asp:Button ID="btncancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="btncancel_Click" />
    </div>
</div>