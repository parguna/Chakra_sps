<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocSapEbsStep2UC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.Step2UC.AdhocSapEbsStep2UC" %>



<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/SecurityQuestionUC.ascx" TagPrefix="uc1" TagName="SecurityQuestionUC" %>
<%@ Register Src="~/NACR/Create/App/Special/SpecialAccountWarning.ascx" TagPrefix="uc1" TagName="SpecialAccountWarning" %>




<script type="text/javascript">
    function validation() {

    }
    function OnClientEntryAdded(sender, eventArgs) {
        $telerik.$(eventArgs.get_entry().get_token()).find('a').attr("title", "");
    }

    function RequestingFromBusinessRole(sender, eventArgs) {
        var submoduleID = '';
        var modules = document.getElementById("ctl00$cplMain$WizardCartUC$AccessUC$AdhocSapFlinsStep2UC$ModuleDropDownList");
        var subModules = document.getElementById("ctl00$cplMain$WizardCartUC$AccessUC$AdhocSapFlinsStep2UC$SubModuleDropDownList");
        if (modules != null && modules.value != "-1" && subModules != null && subModules.value == '-1') {
            alert('Please Select Submodule and try again');

        }
        else {
            if (subModules != null && subModules.value != '-1') {
                // Enter into the loop indicate the module id and submodule id is selected
                submoduleID = subModules[subModules.selectedIndex].innerHTML;
            }
            else {
                // Enter into the loop indicate the module id and submodule id is not selected
                submoduleID = '-1';
            }
            jQuery.ajax({
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: "{'subModuleID': '" + submoduleID + "'}",
                dataType: 'json',
                url: 'AdhocSAPFLINS.aspx/SetModuleIDSubModule',
                success: function (result) {
                }
            });
        }
    }


</script>

<div class="form">
    <div class="form-horizontal left clearfix">
        <asp:HiddenField ID="Mandatory1HiddenField" runat="server" />
        <asp:HiddenField ID="Mandatory2HiddenField" runat="server" />


        <asp:Panel ID="SharedPanelSAPMSSL" runat="server" Visible="false">
            <div class="row-fluid">
                <div class="form-group">
                    <div class="col-xs-6">
                        <div class="form-group required">
                            <label class="col-xs-3 control-label">SAP UserType</label>
                            <div class="col-xs-8">
                                <asp:DropDownList ID="SAPUserTypeDropDown" CssClass="form-control selectpicker" runat="server">
                                </asp:DropDownList>

                            </div>
                        </div>

                        <div class="form-group required">
                            <label class="col-xs-3 control-label">Account name</label>
                            <div class="col-xs-8">
                                <telerik:RadTextBox ID="txtAcntName" MaxLength="12" runat="server" CssClass="form-control">
                                </telerik:RadTextBox>
                            </div>
                            <div class="col-xs-3"></div>
                            <div class="col-xs-8">
                                <uc1:specialaccountwarning runat="server" id="SpecialAccountWarning" />
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-3 control-label">Description</label>
                            <div class="col-xs-8">
                                <telerik:RadTextBox ID="txtDiscription" TextMode="MultiLine" runat="server" CssClass="form-control" MaxLength="40">
                                </telerik:RadTextBox>

                            </div>
                        </div>

                        <div class="form-group" id="trSecurityQstns" runat="server" visible="false">
                            <div class="col-xs-3">
                            </div>
                            <div class="col-xs-8">
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                    <ContentTemplate>
                                        <asp:Panel ID="Panel2" runat="server">
                                            <uc1:SecurityQuestionUC runat="server" ID="SecurityQuestionUC1" OnSecurityQuestionPopUp="SecurityQuestionUC_SecurityQuestionPopUp" OnSecurityResponseSaveClicked="SecurityQuestion_SecurityResponseSaveClicked" LinkText="Service account compliance check" />
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </asp:Panel>


        <asp:Panel ID="GroupPanelSAPMSSL" runat="server">
            <div class="row-fluid">
                <div class="form-group">
                    <div class="col-xs-6">
                        <asp:Panel ID="trEndPointUser" runat="server" Visible="false">
                            <div class="form-group required">
                                <label class="col-xs-3 control-label">EndPoint User </label>
                                <div class="col-xs-8">
                                    <asp:DropDownList ID="EndPointUserDropDownList" runat="server" CssClass="form-control selectpicker" AutoPostBack="true" OnSelectedIndexChanged="EndPointUserDropDownList_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </asp:Panel>

                    </div>
                </div>
            </div>


            <asp:Panel ID="OtherControl" runat="server">
                <asp:Panel ID="FullPositionPanel" runat="server">

                    <div class="row-fluid">
                        <div class="form-group">
                            <div class="col-xs-6">

                                <div id="Position" runat="server" class="form-group">
                                    <label class="col-xs-3 control-label required" runat="server" id="lblPosition">Position</label>
                                    <div class="col-xs-8">
                                        <telerik:RadAutoCompleteBox ID="PositionAutocomplete" runat="server" CssClass="form-control" OnEntryAdded="PositionAutocomplete_EntryAdded" DropDownHeight="200px" DropDownWidth="300px" EmptyMessage="Enter * to view full list" InputType="Token">
                                            <WebServiceSettings Path="../../../../Services/AutoCompleteDataService.svc" Method="" />
                                            <TextSettings SelectionMode="Single" />
                                        </telerik:RadAutoCompleteBox>
                                    </div>
                                    <div class="col-xs-1" style="padding-left: 0px !important;">
                                        &nbsp;&nbsp;<asp:LinkButton  CssClass="icon icon-info-sign icon-entitlement-info" ID="PositionLinkButton" runat="server" OnClick="ViewDeatilsLinkButton_Click">
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
                                            <asp:RadioButton ID="PositionAddRadioButton" runat="server" GroupName="PositionAddRemove" AutoPostBack="true" OnCheckedChanged="PositionAddRadioButton_CheckedChanged" Checked="true" />
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
                                <label class="col-xs-3 control-label" runat="server" id="lblRole">Role</label>
                                <div class="col-xs-8">
                                    <telerik:RadAutoCompleteBox ID="RoleAutoCompleteBox" runat="server" CssClass="form-control" OnEntryAdded="RoleAutoCompleteBox_EntryAdded" DropDownHeight="200px" DropDownWidth="300px" EmptyMessage="Enter * to view full list" InputType="Token">
                                        <WebServiceSettings Path="../../../../Services/AutoCompleteDataService.svc" Method="" />
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
                    <div class="form-group">
                        <label class="col-xs-3 control-label"></label>
                        <div class="col-xs-8">
                            <asp:Button ID="AddRequestButton" CssClass="btn btn-default btn-sm" OnClick="AddRequestButton_Click" Text="Add Role" runat="server" />

                            <asp:Button ID="Button1" CssClass="btn btn-default btn-sm" OnClick="AddToCart" Text="Save Request" runat="server" Visible="false"/>
                        </div>
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
                    <asp:Button ID="btnCancel" CssClass="uiboxgray" runat="server" Text="Close" OnClick="btnCancel_Click" />
                </div>

            </ContentTemplate>

        </asp:UpdatePanel>

    </ContentTemplate>

</telerik:RadWindow>
