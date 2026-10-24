<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocSapEpCeStep2UC_B.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC.AdhocSapEpCeStep2UC_B" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<div class="form">

    <div class="form-horizontal left clearfix">
        <asp:HiddenField ID="HiddenField1" runat="server" />
        <asp:HiddenField ID="HiddenField2" runat="server" />
        <div class="row-fluid">
            <div class="form-group">
                <div class="col-xs-6">
                    <asp:Panel ID="Panel1" runat="server" Visible="false">

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

                    </asp:Panel>

                    <asp:Panel ID="GroupPanelPLRELearn" runat="server">
                        <br />
                        <asp:Panel ID="OtherControls" runat="server">

                            <div class="form-group required">
                                <label class="col-xs-4 control-label">Request Info</label>
                                <div class="col-xs-7">

                                    <asp:TextBox ID="txtRequestDetails" TextMode="MultiLine" runat="server" CssClass="form-control">
                                    </asp:TextBox>
                                </div>
                            </div>


                            <div class="form-group required"  runat="server" visible="false">
                                <label class="col-xs-4 control-label">Entitlement Type</label>
                                <div class="col-xs-7">
                                    <asp:DropDownList ID="ddlEntitlementType" AutoPostBack="true" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlEntitlementType_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div id="divRole" runat="server" visible="false" class="form-group">
                                <label class="col-xs-4 control-label">Role</label>
                                <div class="col-xs-5">
                                    <telerik:RadAutoCompleteBox ID="RolesAutoCompleteBox" CssClass="form-control" runat="server" EmptyMessage="Enter * to view full list" OnEntryRemoved="AutoCompleteBox_EntryRemoved" OnEntryAdded="AutoCompleteBox_EntryAdded" TextSettings-SelectionMode="Single" InputType="Token">
                                        <WebServiceSettings Method="GetEpCeRoles" Path="../../../../Services/AutoCompleteDataService.svc" />
                                    </telerik:RadAutoCompleteBox>
                                </div>
                                <div class="col-xs-2">
                                    <asp:Button ID="btnRoleAdd" CssClass="btn btn-default btn-sm" OnClick="btnAdd_Click" Text="Add Role" runat="server" />
                                </div>
                            </div>

                            <div id="divGroup" runat="server" visible="false" class="form-group">
                                <label class="col-xs-4 control-label">Group</label>
                                <div class="col-xs-5">
                                    <telerik:RadAutoCompleteBox ID="GroupAutoCompleteBox" CssClass="form-control" runat="server" EmptyMessage="Enter * to view full list" OnEntryRemoved="AutoCompleteBox_EntryRemoved" OnEntryAdded="AutoCompleteBox_EntryAdded" TextSettings-SelectionMode="Single" InputType="Token">
                                        <WebServiceSettings Method="GetEpCeGroups" Path="../../../../Services/AutoCompleteDataService.svc" />
                                    </telerik:RadAutoCompleteBox>
                                </div>
                                <div class="col-xs-2">
                                    <asp:Button ID="btnGroupAdd" CssClass="btn btn-default btn-sm" OnClick="btnAdd_Click" Text="Add Group" runat="server" />
                                </div>
                            </div>


                        </asp:Panel>

                        <div class="form-group">
                            <label class="col-xs-4 control-label"></label>
                            <div class="col-xs-7">
                                <asp:Button ID="Button1" CssClass="btn btn-default btn-sm" OnClick="Save_Click" Text="Save Request" runat="server" Visible="false"/>
                            </div>
                        </div>
                    </asp:Panel>

                </div>
            </div>
        </div>

        <div class="col-xs-8">
            <table border="0" cellpadding="0" cellspacing="0" width="Auto" style="margin-left: 5px">
                <tr id="trGrdid" runat="server">
                    <td>
                        <telerik:RadGrid ID="RequestGrid" runat="server" AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                            GridLines="None" Width="1200px" OnItemCommand="RequestGrid_ItemCommand" ShowFooter="false"
                            MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Request Available">
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="False" DataKeyNames="" Width="100%">
                                <Columns>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="50px" HeaderText="Role" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="EntitlementType" runat="server" Text='<%#Eval("EntitlementType") %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Role" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="EntitlementLabel" runat="server" Text='<%#Eval("Entitlement") %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridButtonColumn ConfirmDialogType="RadWindow" ConfirmTitle="Delete" UniqueName="Delete" ConfirmText="Are you sure you want to delete?" CommandName="Delete" ButtonType="ImageButton" ItemStyle-CssClass="action-delete" ItemStyle-Width="1px" HeaderText="">
                                        <HeaderStyle Width="30px" />

                                    </telerik:GridButtonColumn>

                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </td>
                </tr>
            </table>
        </div>

    </div>

</div>
