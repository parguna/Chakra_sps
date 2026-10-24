<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocSAPHANAStep2UC_B.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC.AdhocSAPHANAStep2UC_B" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/SecurityQuestionUC.ascx" TagPrefix="uc1" TagName="SecurityQuestionUC" %>

<script type="text/javascript">
    function validation() {

    }
    function OnClientEntryAdded(sender, eventArgs) {
        $telerik.$(eventArgs.get_entry().get_token()).find('a').attr("title", "");
    }

    function OnClientRequesting(sender, args) {
        var context = args.get_context()
        context["EndpointSystemId"] = document.getElementById("<%= HdnfieldEndpointSystemId.ClientID %>").value;
    }
</script>

<div class="form">
    <div class="form-horizontal left clearfix">
        <asp:HiddenField ID="Mandatory1HiddenField" runat="server" />
        <asp:HiddenField ID="Mandatory2HiddenField" runat="server" />
        <asp:HiddenField ID="HdnfieldEndpointSystemId" runat="server" />

        <asp:Panel ID="Accountpanel" runat="server" Visible="false">

            <div class="form-group">
                <div class="form-group">
                    <div class="col-xs-4">
                        <label class="control-label">Account Information: </label>
                    </div>
                </div>

                <div class="form-group col-xs-7">
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
                                        <telerik:RadComboBox ID="ddlAccounts" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlAccounts_SelectedIndexChanged" CssClass="selectpicker  AccountsPageComoboWidth radbox100Width" DataSource='<%# Eval("Value")%>' DataTextField="value" DataValueField="key"></telerik:RadComboBox>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>

                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </div>
        </asp:Panel>


        <asp:Panel ID="GroupPanelSAPMSSL" runat="server">



            <asp:Panel ID="OtherControl" runat="server">
                <asp:Panel ID="FullPositionPanel" runat="server">

                    <div class="row-fluid">
                        <div class="form-group">
                            <div class="col-xs-6">

                                <%-- <div class="form-group">
                                    <label class="col-xs-3 control-label"></label>
                                    <div class="col-xs-8">
                                        <label class="control-label">Either Position or Role is mandatory.</label>
                                    </div>
                                </div>--%>

                                <div id="Position" runat="server" class="form-group">
                                    <label class="col-xs-3 control-label" runat="server" id="lblPosition">Position</label>
                                    <div class="col-xs-8">
                                        <telerik:RadAutoCompleteBox ID="PositionAutocomplete" runat="server" CssClass="form-control" OnClientRequesting="OnClientRequesting" OnEntryAdded="PositionAutocomplete_EntryAdded" 
                                            DropDownHeight="200px" DropDownWidth="300px" EmptyMessage="Enter * to view full list" InputType="Token" TextSettings-SelectionMode="Single">
                                            <WebServiceSettings Path="../../../../Services/AutoCompleteDataService.svc" Method="GetAllPositionSAPHANA" /><%-- changed from GetAllPositionEBS to  GetAllPositionSAPHANA--%>
                                            <TextSettings SelectionMode="Single" />
                                        </telerik:RadAutoCompleteBox>
                                    </div>
                                    <div class="col-xs-1" style="padding-left: 0px !important;">
                                        <asp:LinkButton CssClass="icon icon-info-sign icon-entitlement-info" ID="PositionLinkButton" runat="server" OnClick="ViewDeatilsLinkButton_Click">
                                        </asp:LinkButton>
                                    </div>
                                </div>

                                <asp:Panel ID="AssignPostionPanel" runat="server" Enabled="false">
                                    <div class="form-group">
                                        <label class="col-xs-3 control-label"></label>
                                        <div class="col-xs-8">
                                            <asp:CheckBox ID="AssignPositionCheckBox" Visible="false" runat="server" Checked="true" />
                                            <asp:Label ID="AssignPositionLable" runat="server" Text=""></asp:Label>
                                        </div>
                                    </div>
                                </asp:Panel>

                            </div>

                            <div class="col-xs-6">

                                <div class="form-group">
                                    <label class="col-xs-3 control-label"></label>
                                    <div class="col-xs-8">
                                        <label class="control-label"></label>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <asp:Panel ID="PostionAddRemovePanel" CssClass="col-xs-12" runat="server" Visible="false">
                                        <div class="form-group">
                                            <asp:RadioButton ID="PositionAddRadioButton" runat="server" GroupName="PositionAddRemove" Checked="true" AutoPostBack="true" OnCheckedChanged="PositionAddRadioButton_CheckedChanged" />
                                            <span class="lbl">Add</span>
                                            <asp:RadioButton ID="PositionReaalocateRadioButton" runat="server" GroupName="PositionAddRemove" AutoPostBack="true" OnCheckedChanged="PositionAddRadioButton_CheckedChanged" />
                                            <span class="lbl">Re-allocate</span>&nbsp;<asp:Label runat="server" ID="lblReallocationMsg" Text="Note: All previous positions and roles will be removed for Reallocation request" ForeColor="Red" Visible ="false"></asp:Label>
                                        </div>
                                    </asp:Panel>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <div class="row-fluid">
                    <div class="form-group">
                        <div class="col-xs-6">
                            <div id="Role" runat="server" class="form-group">
                                <label class="col-xs-3 control-label">Role</label>
                                <div class="col-xs-8">
                                    <telerik:RadAutoCompleteBox ID="RoleAutoCompleteBox" OnClientRequesting="OnClientRequesting" runat="server" CssClass="form-control" OnEntryAdded="RoleAutoCompleteBox_EntryAdded" DropDownHeight="200px" DropDownWidth="300px" EmptyMessage="Enter * to view full list" InputType="Token">
                                        <WebServiceSettings Path="../../../../Services/AutoCompleteDataService.svc" Method="GetAllRolesSAPHANA" />
                                        <TextSettings SelectionMode="Single" />
                                    </telerik:RadAutoCompleteBox>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>


            </asp:Panel>

        </asp:Panel>
        <div class="row-fluid">
            <div class="form-group">
                <div class="col-xs-6">
                    <label class="col-xs-3 control-label"></label>
                    <div class="col-xs-8 form-group ">
                        <asp:Button ID="AddRequestButton" CssClass="btn btn-default btn-sm" OnClick="AddRequestButton_Click" Text="Add" runat="server" />
                        <asp:Button ID="Button1" CssClass="btn btn-default btn-sm" OnClick="Save_Click" Text="Save Request" runat="server" Visible="false"/>
                    </div>
                </div>
            </div>
        </div>

    </div>
</div>

<div id="trGrdid" runat="server" class="col-xs-10">
    <telerik:RadGrid ID="RequestGrid" runat="server" AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" AllowPaging="false" AllowSorting="false" CellSpacing="0"
        GridLines="None" Width="100%" OnItemDataBound="RequestGrid_ItemDataBound" OnItemCommand="RequestGrid_ItemCommand" ShowFooter="false"
        MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Request Available">
        <GroupingSettings CaseSensitive="false" />
        <MasterTableView AutoGenerateColumns="False" DataKeyNames="" Width="100%">
            <Columns>
                <telerik:GridTemplateColumn HeaderText="Role" ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                        <asp:Label ID="EntitlementLabel" runat="server" Text="<%# Container.DataItem %>" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Technical role" ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                        <asp:Label ID="TechnicalRoleLabel" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridButtonColumn ConfirmDialogType="RadWindow" ConfirmTitle="Delete" UniqueName="Delete" ConfirmText="Are you sure you want to delete?" CommandName="Delete" ButtonType="ImageButton" ItemStyle-CssClass="action-delete" ItemStyle-Width="1px" HeaderText="">
                    <HeaderStyle Width="60px" />

                </telerik:GridButtonColumn>
                <%-- <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="Delete">
                                <ItemTemplate>
                                    <asp:LinkButton ID="DeleteRoleButton" runat="server" CommandName="Delete" Text="Delete" ToolTip="Delete" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>--%>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</div>

<telerik:RadWindow runat="server" ID="ViewTechnicalSourceWindow" Modal="true" Height="100%" Width="700px" Title="Techinical Roles"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title">Technical Roles</h3>
                </div>
                <div class="modal-body">
                    <asp:Panel ID="panelViewTechnicalSource" CssClass="ModalWindow" runat="server">

                        <telerik:RadGrid runat="server" ID="ViewTechnicalSource" GridLines="None" AllowAutomaticUpdates="false" Width="100%" EnableAjaxSkinRendering="true" AllowPaging="False" CellSpacing="0">
                            <ClientSettings>
                                <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true" FrozenColumnsCount="2"></Scrolling>
                            </ClientSettings>
                            <PagerStyle Mode="NumericPages" />
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="true" EditFormSettings-PopUpSettings-ScrollBars="Vertical" Width="100%">
                                <Columns>
                                    <telerik:GridBoundColumn DataField="Value" HeaderText="Technical Role" HeaderStyle-Width="10%" FilterControlWidth="94%">
                                    </telerik:GridBoundColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </asp:Panel>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnCancel" CssClass="btn btn-default btn-sm" runat="server" Text="Close" OnClick="btnCancel_Click" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>
