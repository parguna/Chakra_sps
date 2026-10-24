<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocInHouseStep2UC_B.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC.AdhocInHouseStep2UC_B" %>



<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/SecurityQuestionUC.ascx" TagPrefix="uc1" TagName="SecurityQuestionUC" %>

<div class="form">
    <div class="form-horizontal left clearfix">

        <div class="row-fluid">
            <div class="form-group">
                <div class="col-xs-8">
                    <asp:Panel ID="Accountpanel" runat="server" Visible="false">

                        <div class="form-group">
                            <div class="col-xs-3">
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
                    <asp:Panel ID="GroupPanelINH" runat="server">
                        <div class="form-group">
                            <p class="form-group col-xs-12 description">On selecting the entitlement, description will be shown on the right side if available.</p>
                        </div>
                        <div class="form-group required">
                            <label class="col-xs-3 control-label">Permission</label>
                            <div class="col-xs-4">
                                <telerik:RadAutoCompleteBox ID="RolesAutoCompleteBox" runat="server" AutoPostBack="true" EmptyMessage="Enter * to view full list" TextSettings-SelectionMode="Single"
                                    InputType="Token" CssClass="form-control" OnEntryAdded="RolesAutoCompleteBox_EntryAdded" OnEntryRemoved="RolesAutoCompleteBox_EntryRemoved">
                                    <WebServiceSettings Method="GetEntitlementInHouse" Path="../../../../Services/AutoCompleteDataService.svc" />
                                </telerik:RadAutoCompleteBox>
                            </div>
                            <div class="col-xs-5">
                                <asp:Label ID="lblPermissionDescription" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div class="form-group required" runat="server" id="divFunctions" visible="false">
                            <label class="col-xs-3 control-label">Function</label>
                            <div class="col-xs-4">
                                <%--<telerik:RadAutoCompleteBox ID="autoCompleteFunctions" runat="server" DropDownHeight="200px" EmptyMessage="Enter * to view full list" TextSettings-SelectionMode="Single" DropDownWidth="300px" InputType="Text" CssClass="wizlbl" Width="304px">
                                    <WebServiceSettings Method="GetFunctionsInhouse" Path="../Services/AutoCompleteDataService.svc" />
                                </telerik:RadAutoCompleteBox>--%>
                                <telerik:RadComboBox CheckBoxes="true" ID="ddlFunctions" AutoPostBack="true" OnItemChecked="ddlFunctions_ItemChecked" runat="server" Style="width: 100% !important;">
                                </telerik:RadComboBox>
                            </div>
                            <div class="col-xs-5">
                                <asp:Label ID="lblFunctionDescription" runat="server"></asp:Label>
                            </div>
                        </div>
                    </asp:Panel>
                       <div class="form-group">
                            <div class="col-xs-7">
                                <asp:Label runat="server" ID="lblNewAccountWarning" ForeColor="Red" Visible="true"></asp:Label>
                            </div>
                        </div>
                    <div class="form-group">
                        <label class="col-xs-3 control-label"></label>
                        <div class="col-xs-4">
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
                    <telerik:RadGrid ID="RequestGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                        GridLines="None" Width="75%" OnItemCommand="RequestGrid_ItemCommand" ShowFooter="false"
                        MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Request Available">
                        <GroupingSettings CaseSensitive="false" />
                        <MasterTableView AutoGenerateColumns="False" Width="100%">
                            <Columns>
                                <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Permission" ItemStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPermission" runat="server" Text='<%# ((KeyValuePair<string,string>)Container.DataItem).Key %>' />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Function" ItemStyle-HorizontalAlign="Left" UniqueName="Function">
                                    <ItemTemplate>
                                        <asp:Label ID="lblFunction" runat="server" Text='<%# ((KeyValuePair<string,string>)Container.DataItem).Value %>' />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="Delete">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="DeleteRoleButton" runat="server" CommandName="Delete" Text="Delete" ToolTip="Delete" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </td>
            </tr>
        </table>

    </div>
</div>
