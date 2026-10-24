<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocOffice365Step2UC_B.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC.AdhocOffice365Step2UC_B" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<style>
    #ctl00_cplMain_WizardBulkAdhocAccessRequest_BulkAccessUC_AdhocOffice365Step2UC_B_ServicePlanComboBox{
         width:100% !important;
    }
</style>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>

        <div class="form-horizontal left clearfix">

            <div class="row-fluid">
                <div class="form-group">
                    <div class="col-xs-8">
                        <asp:Panel ID="trEndPointUser" runat="server" Visible="false">

                            <div class="form-group">
                                <div class="col-xs-4">
                                <div class="col-xs-8">
                                    <label class="control-label">Account Information: </label>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-xs-8">
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
                            </div>
                        </div>
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>

        <div class="form-horizontal left clearfix">
            <div class="row-fluid">
                <div class="form-group">
                    <div class="col-xs-6">

                        <asp:Panel ID="UserAccPanel" runat="server" Visible="false">

                            <div id="TRExternalEmail" class="form-group" runat="server">

                                <label id="lblAllowExternalEmail" runat="server" class="col-xs-3 control-label"></label>
                                <div class="col-xs-8">
                                    <asp:CheckBox ID="chkboxExtenalEmail" runat="server" />
                                </div>
                            </div>

                        </asp:Panel>

                    </div>
                </div>
            </div>




            <asp:Panel runat="server" ID="LicensePanel" Visible="false">
                <div class="row-fluid">
                    <div class="form-group">
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-xs-3 control-label">License</label>
                                <div class="col-xs-8 form-group" style="padding-left:30px">
                                    <telerik:RadAutoCompleteBox ID="LicenseAutoCompleteBox" CssClass="form-control" OnEntryRemoved="LicenseAutoCompleteBox_EntryRemoved" OnEntryAdded="LicenseAutoCompleteBox_EntryAdded" InputType="Token" EmptyMessage="Enter * to view full list" runat="server" DropDownHeight="200px" AutoPostBack="true">
                                        <TextSettings SelectionMode="Single" />
                                        <WebServiceSettings Method="GetAdhocLicenseInfo"
                                            Path="../../../../Services/AutoCompleteDataService.svc" />
                                    </telerik:RadAutoCompleteBox>
                                    <asp:CheckBox ID="chkboxRemoveLicense" runat="server" Text="Remove License" Visible="false" AutoPostBack="true" OnCheckedChanged="chkboxRemoveLicense_CheckedChanged" Style="vertical-align: super;" />
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="col-xs-3 control-label">Service Plans</label>
                                <div class="col-xs-8 form-group" style="padding-left:30px">
                                     <table style="width:100%">
                                <tr>
                                    <td style="width:85%">
                                        <telerik:RadComboBox ID="ServicePlanComboBox" runat="server" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" DropDownHeight="200px"  AutoPostBack="true"></telerik:RadComboBox>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnAdd" runat="server" Text="Add" Enabled="false" CssClass="btn btn-default btn-sm"  OnClick="btnAdd_Click" />
                                    </td>
                                 </tr>
                                </table>            
                                </div>
                            </div>

                        </div>
                    </div>
                                          
                </div>
                <div class="row-fluid">
                    <div class="form-group">
                        <div class="col-xs-12">

                    <telerik:RadGrid ID="ServicePlanGrid" runat="server" Visible="false" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                        GridLines="None" Width="100%" ShowFooter="false" OnItemCommand="ServicePlanGrid_ItemCommand"
                        MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Records Available" TableLayout="Fixed">
                        <GroupingSettings CaseSensitive="false" />
                        <MasterTableView AutoGenerateColumns="False" DataKeyNames="AccountSkuId" TableLayout="Fixed" Width="100%">
                            <Columns>
                                <telerik:GridBoundColumn DataField="AccountSkuId" FilterControlAltText="License" SortExpression="License" HeaderText="License" UniqueName="License" HeaderStyle-Width="50%">
                                </telerik:GridBoundColumn>

                                <telerik:GridTemplateColumn DataField="IsRemoveLicense" FilterControlAltText="IsRemoveLicense" HeaderText="Is Remove License" SortExpression="IsRemoveLicense" UniqueName="IsRemoveLicense" HeaderStyle-Width="20%">
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" Text='<%#(Convert.ToBoolean(Eval("IsRemoveLicense"))?"Yes":"No")%>'></asp:Label>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="ServiceName" FilterControlAltText="ServiceName" SortExpression="ServiceName" HeaderText="Service Plans" UniqueName="ServiceName" HeaderStyle-Width="20%">
                                </telerik:GridBoundColumn>
                                <telerik:GridButtonColumn ConfirmDialogType="RadWindow" ConfirmTitle="Delete" UniqueName="Delete" ConfirmText="Are you sure you want to delete?" CommandName="Delete" ButtonType="ImageButton" ItemStyle-CssClass="action-delete" ItemStyle-Width="1px" HeaderText="" HeaderStyle-Width="10%">
                                    <HeaderStyle Width="30px" />

                                </telerik:GridButtonColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>  
                    </div>
                  </div>
                  </div>                  
            </asp:Panel>

            <div class="form-horizontal left clearfix">
                <div class="row-fluid">
                    <div class="form-group">
                        <div class="col-xs-6">

                            <div class="form-group">
                                <label class="col-xs-3 control-label"></label>
                                <div class="col-xs-8">
                                    <asp:Button ID="Button1" CssClass="btn btn-default btn-sm" OnClick="Save_Click" Text="Save Request" runat="server" Visible="false"/>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
    </ContentTemplate>
</asp:UpdatePanel>
