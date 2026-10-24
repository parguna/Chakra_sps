<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Admin.Role.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<script type="text/javascript">
    function RowSelecting(sender, args) {
        var UsingKey = args.getDataKeyValue("IsDefaultRole");
        window.parent.$("#Delete").find("input")
                        .attr('disabled', UsingKey == 'True' ? true : false);
    }
</script>
<cc1:IACMRadGridControl ID="RoleGrid" runat="server" OnNeedDataSource="RoleGrid_NeedDataSource" OnSelectedIndexChanged="RoleGrid_SelectedIndexChanged">
    <MasterTableView ShowHeadersWhenNoRecords="false" DataKeyNames="Id" ClientDataKeyNames="IsDefaultRole" Width="100%" TableLayout="fixed" NoMasterRecordsText="No Records Available" >
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn
                SortExpression="RoleName" UniqueName="RoleName" FilterControlAltText="Filter RoleName column" FilterControlWidth="87%"
                HeaderText="Role Name" DataField="RoleName" HeaderStyle-Width="35%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="RoleDescription" FilterControlAltText="Filter RoleDescription column" HeaderText="Role Description" SortExpression="RoleDescription" UniqueName="RoleDescription" HeaderStyle-Width="50%" FilterControlWidth="91%">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
            <telerik:GridTemplateColumn DataField="IsDefaultRole" AllowFiltering="false" HeaderText="Is Default Role" SortExpression="IsDefaultRole" UniqueName="IsDefaultRole" HeaderStyle-Width="15%" FilterControlWidth="50%">
                <ItemTemplate>
                    <asp:Label ID="lblIsDefault" runat="server" Text='<%#Convert.ToBoolean( Eval("IsDefaultRole")) == true ? "Yes" : "No" %>'></asp:Label>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
        </Columns>
    </MasterTableView>
</cc1:IACMRadGridControl>
<%--<div class="buttons text-right nav-rectangle">
    <asp:Button ID="btncancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="btncancel_Click" />
</div>--%>