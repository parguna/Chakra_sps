<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocSapFlinsBIStep2UC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.Step2UC.AdhocSapFlinsBIStep2UC" %>


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
        <div class="row-fluid">
            <div class="form-group">
                <div class="col-xs-6">

                    <asp:Panel ID="SharedPanelFLINSBI" runat="server" Visible="false">

                        <div class="form-group required">
                            <label class="col-xs-3 control-label">SAP UserType</label>
                            <div class="col-xs-8">
                                <asp:DropDownList ID="SAPUserTypeDropDown" CssClass="form-control selectpicker" OnSelectedIndexChanged="sapuserselectionchanged" runat="server">
                                </asp:DropDownList>

                            </div>
                        </div>

                        <div class="form-group required">
                            <label class="col-xs-3 control-label">Account Name</label>
                            <div class="col-xs-8">
                                <telerik:RadTextBox ID="txtAcntName" MaxLength="12" runat="server" CssClass="form-control" OnTextChanged="txtAcntName_TextChanged">
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

                        <%--<div id="trSecurityQstns" runat="server" visible="false">
                            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                <ContentTemplate>
                                    <asp:Panel ID="Panel2" runat="server">
                                        <uc1:SecurityQuestionUC runat="server" ID="SecurityQuestionUC1" OnSecurityQuestionPopUp="SecurityQuestionUC_SecurityQuestionPopUp" OnSecurityResponseSaveClicked="SecurityQuestion_SecurityResponseSaveClicked" LinkText="Service account compliance check" />
                                    </asp:Panel>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>--%>
                    </asp:Panel>


                    <asp:Panel ID="GroupPanelFLINSBI" runat="server">

                        <asp:Panel ID="trEndPointUser" runat="server" Visible="false">

                            <div class="form-group required">
                                <label class="col-xs-3 control-label">EndPoint User </label>
                                <div class="col-xs-8">
                                    <asp:DropDownList ID="EndPointUserDropDownList" runat="server" CssClass="form-control selectpicker" AutoPostBack="true" OnSelectedIndexChanged="EndPointUserDropDownList_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>
                            </div>

                        </asp:Panel>

                        <asp:Panel ID="OtherControls" runat="server">

                            <div id="Module" class="form-group">
                                <label class="col-xs-3 control-label">Module</label>
                                <div class="col-xs-8">
                                    <asp:DropDownList ID="ModuleDropDownList" runat="server" AutoPostBack="true" CssClass="form-control selectpicker" OnSelectedIndexChanged="ddlModule_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="col-xs-3 control-label">Sub Modules</label>
                                <div class="col-xs-8">
                                    <asp:DropDownList ID="SubModuleDropDownList" runat="server" AutoPostBack="true" CssClass="form-control selectpicker" OnSelectedIndexChanged="ddlSubModule_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div id="BusinessRoleAsterix" class="form-group" runat="server">
                                <label class="col-xs-3 control-label">Business Role</label>
                                <div class="col-xs-8">
                                    <telerik:RadAutoCompleteBox ID="BusinessRoleAutoCompleteBox" runat="server" EmptyMessage="Enter * to view full list" TextSettings-SelectionMode="Single"
                                        InputType="Token" CssClass="form-control" Rows="4" required="required"  EnableAjaxSkinRendering="true" RegisterWithScriptManager="true" OnEntryAdded="BusinessRoleAutoCompleteBox_EntryAdded" >
                                        <%--OnClientRequesting="RequestingFromBusinessRole"--%>
                                        <WebServiceSettings Path="../../../../Services/AutoCompleteDataService.svc" Method="GetBusinessRoleFromFlinsBiRole" />
                                        <TextSettings SelectionMode="Single" />
                                    </telerik:RadAutoCompleteBox>
                                </div>
                            </div>

                        </asp:Panel>

                    </asp:Panel>

                    <asp:Panel ID="SharedPanelFLINSBI1" runat="server" Visible="false">

                        <div id="trSecurityQstns" runat="server" visible="false" class="form-group">
                            <label class="col-xs-3 control-label"></label>
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

                    </asp:Panel>

                    <div class="form-group">
                        <label class="col-xs-3 control-label"></label>
                        <div class="col-xs-8">
                            <asp:Button ID="DeleteAccountAdd" CssClass="btn btn-default btn-sm" OnClick="AddButton_Click" Text="Add" runat="server" />

                            <asp:Button ID="Button1" CssClass="btn btn-default btn-sm" OnClick="AddToCart" Text="Save Request" runat="server" Visible="false"/>
                        </div>
                    </div>

                </div>
            </div>


            <table border="0" cellpadding="0" cellspacing="0" width="99%" style="margin-left: 5px">
                <tr>
                    <td>
                        <telerik:RadGrid ID="RequestGrid" runat="server" AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                            GridLines="None"  OnNeedDataSource="RequestGrid_NeedDataSource" OnItemDataBound="RequestGrid_ItemDataBound" OnItemCommand="RequestGrid_ItemCommand" ShowFooter="false"
                            MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Request Available">
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="False" DataKeyNames="" Width="100%">
                                <Columns>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="45%" HeaderText="Business Role" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="EntitlementLabel" runat="server" Text="<%# Container.DataItem %>" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="45%" HeaderText="Technical role" ItemStyle-HorizontalAlign="Left">
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
                    </td>
                </tr>
            </table>
        </div>
    </div>
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


