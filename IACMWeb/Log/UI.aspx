<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="UI.aspx.cs" Inherits="DI.Chakra.Web.Log.UI" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:Panel ID="grdpanel" Visible="true" runat="server">
        <cc1:IACMRadGridControl ID="grdUILogs" runat="server" OnNeedDataSource="grdUser_NeedDataSource" OnItemDataBound="grdUILogs_ItemDataBound" PageSize="10" AllowCustomPaging="true">
            <MasterTableView EditMode="EditForms" Width="100%" AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available" DataKeyNames="UserId">
                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>
                <Columns>
                     <%--<telerik:GridTemplateColumn DataField="UserName" UniqueName="UserName" SortExpression="UserName" HeaderText="User Name" HeaderStyle-Width="13%" FilterControlWidth="65%">
                    <ItemTemplate>
                            <asp:Label ID="UserName" runat="server"  />
                    </ItemTemplate>
                    <ItemStyle CssClass="endpoint-system detail" HorizontalAlign="Left" />
                </telerik:GridTemplateColumn>--%>
                    <telerik:GridBoundColumn DataField="UserName" FilterControlAltText="UserName" HeaderText="User Name" SortExpression="UserName" UniqueName="UserName" HeaderStyle-Width="12%" FilterControlWidth="61%">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text="" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="IP" FilterControlAltText="IP" HeaderText="IP" SortExpression="IP" UniqueName="IP" HeaderStyle-Width="12%" FilterControlWidth="61%">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text="" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>

                    <telerik:GridBoundColumn DataField="Message" FilterControlAltText="Message" SortExpression="Message" HeaderText="MessageText" UniqueName="Message" HeaderStyle-Width="22%" FilterControlWidth="80%">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text="" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="ExceptionMessage" FilterControlAltText="ExceptionMessage" SortExpression="ExceptionMessage" HeaderText="Exception Message" UniqueName="ExceptionMessage" HeaderStyle-Width="20%" FilterControlWidth="78%">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text="" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>

                    <telerik:GridBoundColumn DataField="InnerExceptionMessage" FilterControlAltText="InnerExceptionMessage" SortExpression="InnerExceptionMessage" HeaderText="Inner Exception Message" UniqueName="InnerExceptionMessage" HeaderStyle-Width="20%" FilterControlWidth="78%">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text="" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>

                    <telerik:GridDateTimeColumn DataField="LogDate" FilterControlAltText="LogDate" SortExpression="LogDate" HeaderText="Log Date" UniqueName="LogDate" HeaderStyle-Width="12%" FilterControlWidth="82%"
                         EnableTimeIndependentFiltering="true" DataFormatString= "{0:dd-MM-yyyy hh:mm:ss tt}">
                        <ColumnValidationSettings>
                            <ModelErrorMessage Text="" />
                        </ColumnValidationSettings>
                    </telerik:GridDateTimeColumn>

                </Columns>
            </MasterTableView>
       </cc1:IACMRadGridControl>
                
    </asp:Panel>

    <div class="buttons text-right nav-rectangle">
        <asp:Button ID="btncancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="btnCancel_Click" />
         <asp:Button ID="btnRefresh" runat="server" CssClass="btn btn-default btn-sm" Text="Refresh" OnClick="btnRefresh_Click" />
    </div>

</asp:Content>
