<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocGemsStep2UC_B.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC.AdhocGemsStep2UC_B" %>



<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/SecurityQuestionUC.ascx" TagPrefix="uc1" TagName="SecurityQuestionUC" %>

<div class="form">
    <div class="form-horizontal left clearfix">

        <div class="row-fluid">
            <div class="form-group">
                <div class="col-xs-6">
                    <asp:Panel ID="Accountpanel" runat="server" Visible="false">

                        <div class="form-group">
                            <div class="col-xs-4">
                                <label class="control-label">Account Information: </label>
                            </div>
                        </div>

                        <telerik:RadGrid ID="AccountRepeater" runat="server" AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                            GridLines="None" ShowFooter="false" MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Request Available">
                            <GroupingSettings CaseSensitive="false" />
                            <ClientSettings EnableRowHoverStyle="true">
                                <Scrolling UseStaticHeaders="true" />
                                <Selecting AllowRowSelect="true" />
                            </ClientSettings>
                            <MasterTableView AutoGenerateColumns="False" DataKeyNames="key,Value" Width="100%">
                                <Columns>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="60%" HeaderText="User" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAcountName" runat="server" CssClass="required" Text='<%# Eval("key") %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                    <telerik:GridTemplateColumn HeaderStyle-Width="40%" HeaderText="Accounts" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <telerik:RadComboBox ID="ddlAccounts" runat="server" CssClass="selectpicker  AccountsPageComoboWidth radbox100Width" DataSource='<%# Eval("Value")%>' DataTextField="value" DataValueField="key"></telerik:RadComboBox>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                        <br />
                    </asp:Panel>
                    <asp:Panel ID="GroupPanelGEMS" runat="server">

                        <div id="RoleAstreix" runat="server" class="form-group required">
                            <label class="col-xs-3 control-label required">Roles</label>
                            <div class="col-xs-8 form-group">
                                <telerik:RadAutoCompleteBox ID="RolesAutoCompleteBox" CssClass="form-control" runat="server" DropDownHeight="200px" EmptyMessage="Enter * to view full list" TextSettings-SelectionMode="Single" DropDownWidth="300px" InputType="Token" OnClientEntryAdding="OnClientEntryGemsAccessRoleAddingHandler">
                                    <WebServiceSettings Method="GetEntitlementGEMS" Path="../../../../Services/AutoCompleteDataService.svc" />
                                </telerik:RadAutoCompleteBox>
                            </div>
                        </div>

                    </asp:Panel>



                    <div class="form-group">
                        <label class="col-xs-3 control-label"></label>
                        <div class="col-xs-8">
                            <asp:Button ID="AddRequestButton" CssClass="btn btn-default btn-sm" OnClick="AddRequestButton_Click" Text="Add" runat="server" />

                            <asp:Button ID="Button1" CssClass="btn btn-default btn-sm" OnClick="Save_Click" Text="Save Request" runat="server" Visible="false"/>
                        </div>
                    </div>

                </div>
            </div>
        </div>

        <table border="0" cellpadding="0" cellspacing="0" width="99%" style="margin-left: 5px">
            <tr id="trGrdid" runat="server">
                <td>
                    <telerik:RadGrid ID="RequestGrid" runat="server" AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                        GridLines="None" Width="1200px" OnItemCommand="RequestGrid_ItemCommand" ShowFooter="false"
                        MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Request Available">
                        <GroupingSettings CaseSensitive="false" />
                        <MasterTableView AutoGenerateColumns="False" DataKeyNames="" Width="100%">
                            <Columns>
                                <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Role" ItemStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:Label ID="EntitlementLabel" runat="server" Text="<%# Container.DataItem %>" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridButtonColumn ConfirmDialogType="RadWindow" ConfirmTitle="Delete" UniqueName="Delete" ConfirmText="Are you sure you want to delete?" CommandName="Delete" ButtonType="ImageButton" ItemStyle-CssClass="action-delete" ItemStyle-Width="1px" HeaderText="">
                                    <HeaderStyle Width="30px" />

                                </telerik:GridButtonColumn>
                                <%--<telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="Delete">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="DeleteRoleButton" runat="server" CommandName="Delete" Text="Delete" ToolTip="Delete" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>--%>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </td>
            </tr>
        </table>

    </div>
</div>
<script type="text/javascript">
    function OnClientEntryGemsAccessRoleAddingHandler(sender, eventArgs) {
            if (sender.get_entries().get_count() > 0) {

                eventArgs.set_cancel(true);
            }
        }
</script>