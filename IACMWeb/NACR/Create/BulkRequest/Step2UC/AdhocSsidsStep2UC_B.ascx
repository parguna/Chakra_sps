<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocSsidsStep2UC_B.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC.AdhocSsidsStep2UC_B" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<style>
    .rgSelectedRow input {
        color: grey !important;
    }
</style>

<div class="form">
    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="col-xs-6">

                <asp:Panel ID="pnlEndPointUserName" runat="server" Visible="false">

                    <div class="form-group">
                        <div class="col-xs-4">
                            <label class="control-label">Account Information: </label>
                        </div>
                    </div>
                     <div class="form-group">
                          <div class="col-xs-12">
                    <cc1:IACMRadGridControl ID="AccountRepeater" runat="server" AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                        GridLines="None" ShowFooter="false" MasterTableView-EnableNoRecordsTemplate="true" DisableRowSelect="true" MasterTableView-NoDetailRecordsText="No Request Available">
                        <GroupingSettings CaseSensitive="false" />
                        <ClientSettings Selecting-AllowRowSelect="true" EnablePostBackOnRowClick="true">
                        </ClientSettings>
                        <MasterTableView AutoGenerateColumns="False" DataKeyNames="key,Value"  Width="100%">
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
                    </cc1:IACMRadGridControl>   
                          </div>  
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlSMS" runat="server" Visible="false">
                    <div class="form-group ">
                        <div class="col-xs-4">
                        <label class="control-label">Hand Phone Number&nbsp;<span style="color:red">*</span></label>
                            </div>
                    </div>

                    <div class="form-group">
                          <div class="col-xs-12">
                        <telerik:RadGrid ID="RadGridSMS" runat="server" AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                            GridLines="None" ShowFooter="false" MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Request Available">
                            <GroupingSettings CaseSensitive="false" />
                            <ClientSettings EnableRowHoverStyle="true">
                                <Scrolling UseStaticHeaders="true" />
                                <Selecting AllowRowSelect="true" />
                            </ClientSettings>
                            <MasterTableView AutoGenerateColumns="False" DataKeyNames="key,Value"  Width="100%">
                                <Columns>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="60%" HeaderText="User" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAcountName" runat="server" CssClass="required" Text='<%# Eval("key") %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                    <telerik:GridTemplateColumn HeaderStyle-Width="40%" HeaderText="Phone Number" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtSMS" runat="server" CssClass="required" Text='<%# Bind("Value") %>' MaxLength="8"/>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                       </div> 
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlGUI" runat="server" Visible="false">  
                    <div class="form-group">
                        <div class="col-xs-4">
                            <label class="control-label">Role&nbsp;<span style="color:red">*</span></label>
                        </div>
                        <div class="col-xs-8">
                            <asp:DropDownList ID="ddlRole" CssClass="form-control selectpicker" runat="server">
                            </asp:DropDownList>
                        </div>
                    </div>
                </asp:Panel>

                <div class="form-group">
                    <label class="col-xs-4 control-label"></label>
                    <div class="col-xs-7">
                        <asp:Button ID="Button1" CssClass="btn btn-default btn-sm" OnClick="Save_Click" Text="Save Request" runat="server" Visible="false"/>
                    </div>
                </div>

            </div>
        </div>
    </div>
</div>
