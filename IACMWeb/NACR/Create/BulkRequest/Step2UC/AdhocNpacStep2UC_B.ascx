<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocNpacStep2UC_B.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC.AdhocNpacStep2UC_B" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %> 


<div class="form">
    <div class="form-horizontal left clearfix">
        <asp:HiddenField ID="Mandatory1HiddenField" runat="server" />
        <asp:HiddenField ID="Mandatory2HiddenField" runat="server" />
        <div class="row-fluid">
            <div class="form-group">
                <div class="col-xs-6"> 

                    <asp:Panel ID="OtherControls" runat="server">
                        <div class="form-group required">
                            <label class="col-xs-3 control-label">Levels</label>
                            <div class="col-xs-8">
                                <telerik:RadComboBox ID="LevelRadComboBox" runat="server"  CssClass="selectpicker AccountsPageComoboWidth radbox100Width" data-width="100%" >
                                </telerik:RadComboBox>
                            </div>
                        </div> 
                    </asp:Panel>

                     <asp:Panel ID="Accountpanel" runat="server">
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

                    <div class="form-group">
                        <label class="col-xs-3 control-label"></label>
                        <div class="col-xs-8">
                             <asp:Button ID="SaveButton" CssClass="btn btn-default btn-sm" OnClick="Save_Click" Text="Save Request" runat="server" Visible="false"/>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
