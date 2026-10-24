<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocIdmsStep2UC_B.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC.AdhocIdmsStep2UC_B" %>

                                                                  

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>



<div class="form">
    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="col-xs-8">
                 

                <asp:Panel ID="pnlEndPointUserName" runat="server" CssClass="mBottom15" Visible="false">

                        <div class="form-group mBottom15">
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

                 <div class="form-group required mt-2">
                        <label class="col-xs-4 control-label">Entitlement Type</label>
                        <div class="col-xs-5">
                            <asp:DropDownList ID="ddlEntitlementType" AutoPostBack="true" runat="server" CssClass="form-control"  OnSelectedIndexChanged="ddlEntitlementType_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>

                <div id="divRole" runat="server" visible="false" class="form-group">
                    <label class="col-xs-4 control-label">IDMS Role</label>
                    <div class="col-xs-5">
                        <telerik:RadAutoCompleteBox ID="RolesAutoCompleteBox" CssClass="form-control" runat="server" EmptyMessage="Enter * to view full list"  OnEntryRemoved="AutoCompleteBox_EntryRemoved" OnEntryAdded="AutoCompleteBox_EntryAdded" TextSettings-SelectionMode="Single" InputType="Token">
                            <WebServiceSettings Method="GetIdmsRoles" Path="../../../../Services/AutoCompleteDataService.svc" />
                        </telerik:RadAutoCompleteBox>
                    </div>
                    <div class="col-xs-2">
                        <asp:Button ID="btnRoleAdd" CssClass="btn btn-default btn-sm" OnClick="btnAdd_Click" Text="Add Role" runat="server" />
                    </div>
                </div>

                <div id="divGroup" runat="server" visible="false" class="form-group">
                    <label class="col-xs-4 control-label">IDMS Group</label>
                    <div class="col-xs-5">
                        <telerik:RadAutoCompleteBox ID="GroupAutoCompleteBox" CssClass="form-control" runat="server" EmptyMessage="Enter * to view full list" OnEntryRemoved="AutoCompleteBox_EntryRemoved" OnEntryAdded="AutoCompleteBox_EntryAdded" TextSettings-SelectionMode="Single" InputType="Token">
                            <WebServiceSettings Method="GetIdmsGroups" Path="../../../../Services/AutoCompleteDataService.svc" />
                        </telerik:RadAutoCompleteBox>
                    </div>
                    <div class="col-xs-2">
                        <asp:Button ID="btnGroupAdd" CssClass="btn btn-default btn-sm" OnClick="btnAdd_Click"  Text="Add Group" runat="server" />
                    </div>
                </div> 

                <div class="form-group">
                    <label class="col-xs-4 control-label"></label>
                    <div class="col-xs-5">
                        <asp:Button ID="saveRequest" CssClass="btn btn-default btn-sm" OnClick="Save_Click" Text="Save" Visible="false" runat="server" />
                    </div>
                </div> 

                <table border="0" cellpadding="0" cellspacing="0" width="Auto" style="margin-left: 5px">
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
</div>
