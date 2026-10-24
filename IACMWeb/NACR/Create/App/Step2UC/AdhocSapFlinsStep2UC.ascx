<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocSapFlinsStep2UC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.Step2UC.AdhocSapFlinsStep2UC" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxCtrl" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/SecurityQuestionUC.ascx" TagPrefix="uc1" TagName="SecurityQuestionUC" %>
<%@ Register Src="~/NACR/Create/App/Special/SpecialAccountWarning.ascx" TagPrefix="uc1" TagName="SpecialAccountWarning" %>

<style>
    .clear-padding {
        padding-right: 0px;
    }

    .margin-top {
        margin-top: 10px;
    }
</style>


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

<style>
    .companystyle {
        width: 100% !important;
    }

    .companystyleForCheckBox {
        width: 40%;
        margin-right: -25px;
        float: right;
    }

    .fullwidth {
        width: 100% !important;
    }

    .RadComboBox_IACMSkin .rcbInput {
        padding: 2px 12px !important;
    }
</style>

<div class="form">
    <div class="form-horizontal left clearfix">
        <asp:HiddenField ID="Mandatory1HiddenField" runat="server" />
        <asp:HiddenField ID="Mandatory2HiddenField" runat="server" />
        <div class="row-fluid">
            <div class="form-group">
                <div class="col-xs-6">
                    <asp:Panel ID="SharedPanelHRMSFLINS" runat="server" Visible="false">

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

<%--                        <div class="form-group">
                            <div class="col-xs-11">
                                <asp:Label ID="lblInfo" runat="server" ForeColor="Red"></asp:Label>
                            </div>
                        </div>--%>

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

                    </asp:Panel>

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
    </div>

    <div class="form-horizontal left clearfix">
        <asp:Panel ID="OtherDetailPanel" runat="server" Style="margin-top: -10px;">
            <div class="row-fluid">
                <div class="form-group">
                    <div class="col-xs-6">
                        <div id="Module" class="form-group">
                            <label class="col-xs-3 control-label">Module</label>
                            <div class="col-xs-8">
                                <asp:DropDownList ID="ModuleDropDownList" runat="server" AutoPostBack="true" CssClass="form-control selectpicker" OnSelectedIndexChanged="ModuleDropDownList_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div id="BusinessRoletr" class="form-group" runat="server">
                            <label class="col-xs-3 control-label">Business Role</label>
                            <div class="col-xs-8">
                                <telerik:RadAutoCompleteBox ID="BusinessRoleAutoCompleteBox" runat="server" EmptyMessage="Enter * to view full list" TextSettings-SelectionMode="Single"
                                    InputType="Token" CssClass="form-control" Rows="4" required="required" OnEntryRemoved="BusinessRoleAutoCompleteBox_EntryRemoved" OnEntryAdded="BusinessRoleAutoCompleteBox_EntryAdded" OnClientEntryAdded="OnClientEntryAdded" EnableAjaxSkinRendering="true" RegisterWithScriptManager="true" OnClientRequesting="RequestingFromBusinessRole">
                                    <WebServiceSettings Path="../../../../Services/AutoCompleteDataService.svc" Method="GetBusinessRole" />
                                    <TextSettings SelectionMode="Single" />
                                </telerik:RadAutoCompleteBox>
                            </div>
                        </div>

                        <div class="form-group" runat="server" id="divCompany">
                            <label class="col-xs-3 control-label">Company</label>
                            <div class="col-xs-8 clear-padding">
                                <div class="row">
                                    <div class="col-xs-12 clear-padding">
                                        <telerik:RadComboBox CheckBoxes="true" ID="CompanyAutoCompleteBox" AutoPostBack="true" CssClass="form-control selectpicker fullwidth" OnSelectedIndexChanged="CompanyAutoCompleteBox_SelectedIndexChanged" runat="server" Enabled="false">
                                        </telerik:RadComboBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-xs-12">
                                        <asp:CheckBox ID="RemoveCompanyCheckBox" runat="server" CssClass="margin-top" Visible="false" Checked="false" Text="Retain Business Role(s)" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="form-group" runat="server" id="divCostCenter">
                            <label class="col-xs-3 control-label">Cost Center</label>
                            <div class="col-xs-8">
                                <telerik:RadComboBox Sort="Ascending" CheckBoxes="true" ID="CostCenterAutoCompleteBox" CssClass="form-control selectpicker fullwidth" runat="server" Enabled="false">
                                </telerik:RadComboBox>
                            </div>
                        </div>

                        <div class="form-group" runat="server" id="divPlant">
                            <label class="col-xs-3 control-label">Plant</label>
                            <div class="col-xs-8">
                                <telerik:RadComboBox CheckBoxes="true" ID="PlanetAutoCompleteBox" CssClass="form-control selectpicker fullwidth" runat="server" Enabled="false" />
                            </div>
                        </div>

                        <div class="form-group" runat="server" id="divMRPController">
                            <label class="col-xs-3 control-label">MRP Controller</label>
                            <div class="col-xs-8">
                                <telerik:RadComboBox CheckBoxes="true" ID="MRPControllerAutoCompleteBox" CssClass="form-control selectpicker fullwidth" runat="server" Enabled="false" />
                            </div>
                        </div>

                    </div>

                    <div class="col-xs-6">

                        <div class="form-group">
                            <label class="col-xs-3 control-label">Sub Modules</label>
                            <div class="col-xs-8">
                                <asp:DropDownList ID="SubModuleDropDownList" runat="server" AutoPostBack="true" CssClass="form-control selectpicker" OnSelectedIndexChanged="SubModuleDropDownList_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-3 control-label">Remarks</label>
                            <div class="col-xs-8">
                                <asp:Label ID="RemarksLabel" runat="server" CssClass="wizlbl fullwidth" />
                            </div>
                        </div>

                        <div class="form-group" style="padding-top: 17px;" runat="server" id="divPurchasingOrg">
                            <label class="col-xs-3 control-label">Purchasing Org</label>
                            <div class="col-xs-8">
                                <telerik:RadComboBox CheckBoxes="true" ID="PurchaseOrgAutoCompleteBox" CssClass="form-control fullwidth" runat="server" Enabled="false" />
                            </div>
                        </div>

                        <div class="form-group" runat="server" id="divPgroup">
                            <label class="col-xs-3 control-label">Purchasing Group</label>
                            <div class="col-xs-8">
                                <telerik:RadComboBox CheckBoxes="true" ID="PurchaseGroupAutoCompleteBox" runat="server" CssClass="form-control fullwidth" Enabled="false">
                                </telerik:RadComboBox>
                            </div>
                        </div>

                        <div class="form-group" runat="server" id="divStoreLocation">
                            <label class="col-xs-3 control-label">Store Locations</label>
                            <div class="col-xs-8">
                                <telerik:RadComboBox CheckBoxes="true" ID="StoreLocationsAutoCompleteBox" runat="server" Enabled="false" CssClass="form-control fullwidth" />
                            </div>
                        </div>


                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>

    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="form-group">
                <div class="col-xs-6">
                    <div class="col-xs-3"></div>
                    <div class="col-xs-8">
                        <asp:Button ID="DeleteAccountAdd" CssClass="btn btn-default btn-sm" OnClick="AddButton_Click" Text="Add" runat="server" />
                        <asp:Button ID="Button1" CssClass="btn btn-default btn-sm" OnClick="AddToCart" Text="Save Request" runat="server" Visible="false" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <table border="0" cellpadding="0" cellspacing="0" width="99%" style="margin-left: 5px">
        <tr>
            <td>
                <telerik:RadGrid ID="RequestGrid" runat="server" AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                    GridLines="None" OnNeedDataSource="RequestGrid_NeedDataSource" OnItemCommand="RequestGrid_ItemCommand" ShowFooter="false"
                    MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Request Available">
                    <GroupingSettings CaseSensitive="false" />
                    <ClientSettings>
                        <Scrolling AllowScroll="true" UseStaticHeaders="true" />
                    </ClientSettings>
                    <MasterTableView AutoGenerateColumns="False" DataKeyNames="Entitlement,Company" TableLayout="Fixed" Width="100%">
                        <Columns>
                            <telerik:GridButtonColumn ConfirmDialogType="RadWindow" ConfirmTitle="Delete" UniqueName="Delete" ConfirmText="Are you sure you want to delete?" CommandName="Delete" ButtonType="ImageButton" ItemStyle-CssClass="action-delete" ItemStyle-Width="1px" HeaderText="">
                                <HeaderStyle Width="60px" />
                            </telerik:GridButtonColumn>
                            <telerik:GridTemplateColumn HeaderStyle-Width="250px" HeaderText="Technical role" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:LinkButton ID="ViewTechnicalRolesLinkButton" CommandName="ViewTechicalRole" Text="View Technical Roles" runat="server"></asp:LinkButton>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Business Role" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:Label ID="EntitlementLabel" runat="server" Text='<%#Eval("Entitlement") %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderStyle-Width="290px" HeaderText="Company" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:Label ID="CompanyLabel" runat="server" Text='<%#Eval("Company") %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderStyle-Width="300px" HeaderText="CostCenter" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:Label ID="CostCenterLabel" runat="server" Text='<%#Eval("CostCenter") %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderStyle-Width="300px" HeaderText="PurchaseOrg" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:Label ID="PurchaseOrgLabel" runat="server" Text='<%#Eval("PurchaseOrg") %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderStyle-Width="300px" HeaderText="PurchaseGroup" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:Label ID="PurchaseGroupLabel" runat="server" Text='<%#Eval("PurchaseGroup") %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Plant" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:Label ID="PlantLabel" runat="server" Text='<%#Eval("Plant") %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Storage Location" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:Label ID="StorageLocationLabel" runat="server" Text='<%#Eval("StorageLocation") %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="MRP Controller" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:Label ID="MRPControllerLabel" runat="server" Text='<%#Eval("MRPController") %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderStyle-Width="150px" HeaderText="Retain Roles" UniqueName="RetainCompanyRoles" ItemStyle-HorizontalAlign="Left" Display="false">
                                <ItemTemplate>
                                    <%# Convert.ToBoolean(Eval("RetainCompanyRoles"))? "Yes":"No" %>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>


                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </td>
        </tr>
    </table>



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
