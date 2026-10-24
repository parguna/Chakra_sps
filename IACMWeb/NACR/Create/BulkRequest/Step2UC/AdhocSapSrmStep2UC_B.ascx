<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocSapSrmStep2UC_B.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC.AdhocSapSrmStep2UC_B" %>



<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<script type="text/javascript">
    function OnClientEntrySRMRoleAddingHandler(sender, eventArgs) {



            if (sender.get_entries().get_count() > 0) {

                eventArgs.set_cancel(true);
            }
        }
</script>
<style>
    .fullwidth {
        width: 100% !important;
    }
</style>

<div class="form">

    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="col-xs-8">
                <asp:Panel ID="Accountpanel" runat="server">
                    <div class="form-group" runat="server" id="trAccount" visible="false">
                        <label class="col-xs-3 control-label">Account Information</label>
                        <div class="col-xs-6">
                            <telerik:RadGrid ID="AccountRepeater"  runat="server" AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" AllowPaging="false" AllowSorting="false" CellSpacing="0"
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

                                        <telerik:GridTemplateColumn HeaderStyle-Width="40%" HeaderText="Accounts" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <telerik:RadComboBox ID="ddlAccounts" runat="server" CssClass="selectpicker  AccountsPageComoboWidth radbox100Width" DataSource='<%# Eval("Value")%>' DataTextField="value" DataValueField="key"></telerik:RadComboBox>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>

                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                        </div>
                    </div>

                </asp:Panel>
                <asp:Panel ID="accountNamepanel" runat="server">
                    <div class="form-group" runat="server" id="trAccountnameinfo" visible="false">
                        <label class="col-xs-3 control-label">Account Name Information</label>
                        <div class="col-xs-6">
                            <telerik:RadGrid ID="accNameRepeater" runat="server" AllowAutomaticUpdates="false" Width="100%" EnableAjaxSkinRendering="true" AllowPaging="false" AllowSorting="false" CellSpacing="0"
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

                                        <telerik:GridTemplateColumn HeaderStyle-Width="40%" HeaderText="Accounts" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:DropDownList ID="dllFlinsAccount" runat="server" DataSource='<%# Eval("Value")%>' DataTextField="value" DataValueField="key" CssClass="selectpicker  AccountsPageComoboWidth radbox100Width"></asp:DropDownList>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>

                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>

    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="col-xs-8">

                <div id="trAccountName" runat="server" visible="false" class="form-group required">
                    <label class="col-xs-3 control-label">Account Name</label>
                    <div class="col-xs-6">
                        <asp:TextBox ID="AccontNameTxt" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>

                <div id="TRBuisnessRole" runat="server" class="form-group required">
                    <label class="col-xs-3 control-label">Buisness Role</label>
                    <div class="col-xs-6">
                        <telerik:RadAutoCompleteBox ID="BusinessRoleAutoCompleteBox" InputType="Token" EmptyMessage="Enter * to view full list" AutoPostBack="true" TextSettings-SelectionMode="Single" OnClientEntryAdding="OnClientEntrySRMRoleAddingHandler" OnEntryRemoved="BusinessRoleAutoCompleteBox_EntryRemoved" OnEntryAdded="BusinessRoleAutoCompleteBox_EntryAdded" runat="server" DropDownHeight="200px" DropDownWidth="300px" CssClass="form-control">
                            <TextSettings SelectionMode="Single" />
                            <WebServiceSettings Method="GetBulkSRMBusinesRoles" Path="../../../../Services/AutoCompleteDataService.svc" />
                        </telerik:RadAutoCompleteBox>
                    </div>
                </div>

            </div>
        </div>
    </div>

    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="col-xs-8">

                <div id="TRporg" runat="server" class="form-group required">
                    <label class="col-xs-3 control-label">Purchasing Org</label>
                    <div class="col-xs-6">
                        <asp:DropDownList ID="ddlPOrg" runat="server" OnSelectedIndexChanged="ddlPOrg_SelectedIndexChanged" CssClass="form-control selectpicker" AutoPostBack="true">
                        </asp:DropDownList>
                        <asp:CheckBox ID="chkboxRetainMainRole" runat="server" Text="Retain Main Role (Remove POrg & PGrp roles)" AutoPostBack="true" Checked="true" OnCheckedChanged="chkboxRetainMainRole_CheckedChanged" />
                    </div>
                </div>

                <div id="TRpgrp" runat="server" class="form-group required">
                    <label class="col-xs-3 control-label">Purchasing Group</label>
                    <div class="col-xs-6">
                       <telerik:RadComboBox ID="PGrpComboBox" runat="server" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" CssClass="fullwidth" DropDownWidth="325px" OnSelectedIndexChanged="PGrpComboBox_SelectedIndexChanged" AutoPostBack="true"></telerik:RadComboBox>
                    </div>
                </div>

                <div class="form-group">
                    <label class="col-xs-3 control-label"></label>
                    <div class="col-xs-6">
                        <asp:Button ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click" CssClass="btn btn-default btn-sm" />
                        <asp:Button ID="Button1" CssClass="btn btn-default btn-sm" OnClick="Save_Click" Text="Save Request" runat="server" Visible="false"/>
                    </div>
                </div>
            </div>
        </div>
        <div class="row-fluid">
            <div class="col-xs-8">
                <telerik:RadGrid ID="SRMObjGrid" runat="server" Visible="false" AllowPaging="false" AllowSorting="false" CellSpacing="0"
        GridLines="None" Width="100%" ShowFooter="false" OnItemCommand="SRMObjGrid_ItemCommand"
        MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Records Available" TableLayout="Fixed" CssClass="RadGridtextWrap" OnDataBound="SRMObjGrid_DataBound">
        <GroupingSettings CaseSensitive="false" />
        <ClientSettings>
            <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true" ScrollHeight="120px" FrozenColumnsCount="2"></Scrolling>
        </ClientSettings>
        <MasterTableView AutoGenerateColumns="False" DataKeyNames="Entitlement,PurchaseOrg,PurchaseGroup" TableLayout="Fixed"  Width="100%">
            <Columns>
                <telerik:GridBoundColumn DataField="Entitlement" FilterControlAltText="Entitlement" SortExpression="Entitlement" HeaderText="Entitlement" UniqueName="Entitlement">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="PurchaseOrg" FilterControlAltText="PurchaseOrg" HeaderText="Purchase Org" SortExpression="PurchaseOrg" UniqueName="PurchaseOrg">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="PurchaseGroup" FilterControlAltText="PurchaseGroup" SortExpression="PurchaseGroup" HeaderText="Purchase Group" UniqueName="PurchaseGroup">
                </telerik:GridBoundColumn>
                <telerik:GridCheckBoxColumn UniqueName="RetainMainRole" DataField="RetainMainRole" SortExpression="RetainMainRole" HeaderText="Retain Main Role">
                </telerik:GridCheckBoxColumn>
                <telerik:GridButtonColumn ConfirmDialogType="RadWindow" ConfirmTitle="Delete" UniqueName="Delete" ConfirmText="Are you sure you want to delete?" CommandName="Delete" ButtonType="ImageButton" ItemStyle-CssClass="action-delete" ItemStyle-Width="1px" HeaderText="">
                    <HeaderStyle Width="30px" />

                </telerik:GridButtonColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
            </div>
        </div>
    </div>

    

</div>
