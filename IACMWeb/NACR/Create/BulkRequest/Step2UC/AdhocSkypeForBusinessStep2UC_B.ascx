<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocSkypeForBusinessStep2UC_B.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC.AdhocSkypeForBusinessStep2UC_B" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<script type="text/javascript">
    function isNumberKey(evt) {
        var charCode = (evt.which) ? evt.which : evt.keyCode;
        if (charCode > 31 && (charCode < 48 || charCode > 57))
            return false;
        return true;
    }
    function OnClientRequesting(sender, args) {
        var context = args.get_context()
        context["AdhocActionType"] = "<%= AdhocActionType %>";
        context["endpointId"] = "<%= EndpointSystemId %>";
      }
</script>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
         <div class="row-fluid">
            <div class="form-group">
                <div class="col-xs-8">
                    <asp:Panel ID="pnlEndPointUserName" runat="server" Visible="false" CssClass="row">
                        <div class="form-group row">
                            <div class="col-xs-9">
                                <label class="control-label">Account Information: </label>
                            </div>
                        </div>
                        <div class="form-group row">
                            <div class="col-xs-12">
                                <telerik:RadGrid ID="AccountRepeater" runat="server" AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                                    GridLines="None" ShowFooter="false" MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Request Available">
                                    <GroupingSettings CaseSensitive="false" />
                                    <ClientSettings EnableRowHoverStyle="true">
                                        <Scrolling UseStaticHeaders="true" />
                                        <Selecting AllowRowSelect="true" />
                                    </ClientSettings>
                                    <MasterTableView AutoGenerateColumns="False" DataKeyNames="key,Value"  Width="100%">
                                        <Columns>
                                            <telerik:GridTemplateColumn HeaderStyle-Width="30%" HeaderText="User" ItemStyle-HorizontalAlign="Left">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblAcountName" runat="server" CssClass="required" Text='<%# Eval("key") %>' />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>

                                            <telerik:GridTemplateColumn HeaderStyle-Width="30%" HeaderText="Accounts" ItemStyle-HorizontalAlign="Left">
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
                    <div class="form-group row required" runat="server" visible="true" id="divEntitlementType">
                        <label class="col-xs-4 control-label">Type</label>
                        <div class="col-xs-8" style="padding-right:30px;">
                            <asp:DropDownList ID="ddlEntitlementType" OnSelectedIndexChanged="ddlEntitlementType_SelectedIndexChanged" AutoPostBack="true" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                    </div> 
                    <asp:Panel ID="pnlResponseGroupSelection" runat="server" Visible="false">
                       
                    <div class="form-group row required">
                        <label class="col-xs-4 control-label">Response Group</label>
                            <div class="col-xs-8" style="padding-right:30px;">
                                <telerik:RadAutoCompleteBox ID="ResponseGroupAutoCompleteBox" OnEntryRemoved="ResponseGroupAutoCompleteBox_EntryRemoved" OnEntryAdded="ResponseGroupAutoCompleteBox_EntryAdded"
                                    EmptyMessage="Enter * to view full list" runat="server" DropDownHeight="200px" DropDownWidth="300px" InputType="Token" OnClientRequesting="OnClientRequesting"
                                    CssClass="form-control" AutoPostBack="true">
                                    <TextSettings SelectionMode="Single" />
                                    <WebServiceSettings Method="GetResponseGroupForBulkRequestService" Path="../../../../Services/AutoCompleteDataService.svc" />
                                </telerik:RadAutoCompleteBox>
                            </div>
                           
                    </div>
                     <div class="form-group row" runat="server" id="divMask">
                        <label class="col-xs-4 control-label">Mask</label>
                        <div class="col-xs-8">
                            <asp:CheckBox ID="chkMask" runat="server" Checked="false" />
                        </div>
                    </div>
                    <div class="form-group row">
                            <label class="col-xs-4 control-label"></label>
                            <div class="col-xs-8">
                                <asp:Button ID="btnAddRequest" runat="server" Text="Add" Visible="false" CssClass="btn btn-default" OnClick="btnAddRequest_Click"></asp:Button>
                            </div>
                        </div>
                    </asp:Panel>
                    <div class="form-group row">
                        <div class="col-xs-12">
                            <asp:Panel ID="pnlResponseGroup" runat="server" Visible="false">
                                <telerik:RadGrid ID="ResponseGroupGrid" runat="server" Visible="false" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                                    GridLines="None" ShowFooter="false" OnItemCommand="ResponseGroupGrid_ItemCommand"
                                    MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Records Available" tablelayout="Fixed" CssClass="RadGridtextWrap">
                                    <GroupingSettings CaseSensitive="false" />
                                    <ClientSettings>
                                        <Scrolling AllowScroll="true" ScrollHeight="80px" UseStaticHeaders="true" SaveScrollPosition="True" />
                                    </ClientSettings>
                                    <MasterTableView AutoGenerateColumns="False" DataKeyNames="Id" TableLayout="Fixed"  Width="100%">
                                        <Columns>
                                            <telerik:GridBoundColumn DataField="Name" FilterControlAltText="Name" SortExpression="Name" HeaderText="Response Group Name" UniqueName="Name">
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn DataField="Mask" FilterControlAltText="Mask" SortExpression="Mask" HeaderText="Mask" UniqueName="Mask">
                                            </telerik:GridBoundColumn>
                                            <%--<telerik:GridCheckBoxColumn DataField="ADID" FilterControlAltText="ADID" HeaderText="ADID" UniqueName="ADID">
                                        </telerik:GridCheckBoxColumn--%>
                                            <telerik:GridButtonColumn ConfirmDialogType="RadWindow" ConfirmTitle="Delete" UniqueName="Delete" ConfirmText="Are you sure you want to delete?" CommandName="Delete" ButtonType="ImageButton" ItemStyle-CssClass="action-delete" ItemStyle-Width="1px" HeaderText="">
                                                <HeaderStyle Width="50px" />
                                            </telerik:GridButtonColumn>
                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>

