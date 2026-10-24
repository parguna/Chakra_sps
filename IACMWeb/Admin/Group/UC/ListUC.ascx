<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Admin.Group.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<script type="text/javascript">
    function RowSelecting(sender, args) {
        var UsingKey = args.getDataKeyValue("IsDefaultGroup");
        window.parent.$("#Delete").find("input")
                        .attr('disabled', UsingKey == 'True' ? true : false);
    }
</script>
<cc1:IACMRadGridControl ID="GroupRadGrid" runat="server" OnNeedDataSource="GroupRadGrid_NeedDataSource" OnSelectedIndexChanged="GroupRadGrid_SelectedIndexChanged">
    <MasterTableView ShowHeadersWhenNoRecords="true"  NoMasterRecordsText="No Records Available" Width="100%"
        DataKeyNames="Id" ClientDataKeyNames="IsDefaultGroup">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn
                SortExpression="GroupName" UniqueName="GroupName" FilterControlAltText="Filter GroupName column" FilterControlWidth="83%"
                HeaderText="Group Name" DataField="GroupName" HeaderStyle-Width="25%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="GroupDescription" FilterControlAltText="Filter GroupDescription column" HeaderText="Description" SortExpression="GroupDescription" UniqueName="GroupDescription" HeaderStyle-Width="50%" FilterControlWidth="92%">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text="" />
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
            <telerik:GridTemplateColumn DataField="IsDefaultGroup" AllowFiltering="false" HeaderText="Is Default" SortExpression="IsDefaultGroup" UniqueName="IsDefaultGroup" HeaderStyle-Width="15%">
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%#Convert.ToBoolean( Eval("IsDefaultGroup")) == true ? "Yes" : "No" %>'></asp:Label>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
        </Columns>
    </MasterTableView>
</cc1:IACMRadGridControl>
<%--<div class="buttons text-right nav-rectangle">
    <asp:Button ID="btncancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="btncancel_Click" />
</div>--%>