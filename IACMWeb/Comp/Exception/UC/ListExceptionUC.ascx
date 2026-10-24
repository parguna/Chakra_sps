<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListExceptionUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.Exception.UC.ListExceptionUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-wrapper">
    <cc1:IACMRadGridControl ID="RadException" runat="server" OnNeedDataSource="RadException_NeedDataSource" OnSelectedIndexChanged="RadException_SelectedIndexChanged" OnInit="RadException_Init">
        <MasterTableView  NoMasterRecordsText="No Records Available" DataKeyNames="Id">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="false" ShowExportToCsvButton="false" />
            <Columns>
                <telerik:GridBoundColumn DataField="ExceptionCode" HeaderText="Code" SortExpression="ExceptionCode" UniqueName="ExceptionCode"  DataType="System.String" HeaderStyle-Width="50%" FilterControlWidth="91.5%">
                </telerik:GridBoundColumn>  
                <telerik:GridBoundColumn DataField="ExceptionDesc" HeaderText="Description" SortExpression="ExceptionDesc" UniqueName="ExceptionDesc"  DataType="System.String" HeaderStyle-Width="50%" FilterControlWidth="91.5%">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
  </cc1:IACMRadGridControl>

      <div class="buttons text-right nav-rectangle">
        <asp:Button ID="btncancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="btncancel_Click" />
    </div>
</div>