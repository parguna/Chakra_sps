<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocADStep2UC_B.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC.AdhocADStep2UC_B" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>



<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>

        <div class="form">
            <div class="form-horizontal left clearfix">
                <div class="row-fluid">
                    <div class="col-xs-8">

                        

                        <asp:Panel ID="Panel1" runat="server">
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
                        <div class="row">
                            <div class="col-xs-12">
                                &nbsp;
                            </div>
                        </div>
                        <asp:Panel ID="GroupPanelAD" runat="server">
                            <div id="tdGroupAdd" runat="server" visible="false" class="form-group">
                                <div class="col-xs-4">
                                    <asp:Label ID="lblComments" runat="server" CssClass="control-label">Comments for Group Add/Remove</asp:Label> 
                                </div>
                                <div class="col-xs-7">
                                    <asp:TextBox ID="txtRequesterComments" runat="server" CssClass="form-control" TextMode="MultiLine" Height="100px">
                                    </asp:TextBox>
                                </div>
                            </div>
                        </asp:Panel>
                        <div class="form-group" style="margin-top: 15px; float: right;">
                            <asp:Button ID="Button2" runat="server" CssClass="btn btn-primary" Text="Submit Request" Visible="false" OnClick="btnSave_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
