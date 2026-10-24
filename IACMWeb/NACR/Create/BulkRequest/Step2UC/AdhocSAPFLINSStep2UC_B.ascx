<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocSapFlinsStep2UC_B.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC.AdhocSapFlinsStep2UC_B" %>



<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxCtrl" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/SecurityQuestionUC.ascx" TagPrefix="uc1" TagName="SecurityQuestionUC" %>



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
        margin-left: 15px;
        width: 100%;
    }

    .companystyleForCheckBox {
        width: 40%;
        margin-right: -25px;
        float: right;
    }

    .fullwidth {
        width: 100% !important;
    }
    #ctl00_cplMain_WizardBulkAdhocAccessRequest_BulkAccessUC_AdhocSapFlinsStep2UC_B_CompanyAutoCompleteBox{
        width:100% !important;
    }
</style>

<div class="form">
    <div class="form-horizontal left clearfix">
        <asp:HiddenField ID="Mandatory1HiddenField" runat="server" />
        <asp:HiddenField ID="Mandatory2HiddenField" runat="server" />
        <div class="row-fluid">
            <div class="form-group">
                <div class="col-xs-8">
                    <asp:Panel ID="trEndPointUser" runat="server" Visible="false">

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
                </div>
            </div>
        </div>
    </div>

    <div class="form-horizontal left clearfix">
        <asp:Panel ID="OtherDetailPanel" runat="server" Style="margin-top: -10px;">
            <div class="row-fluid">
                <div class="form-group">
                    <div class="col-xs-6">
                    </div>
                    <div class="col-xs-6">
                        <div class="form-group" id="buttontr" runat="server" visible="false">
                            <div>
                                <asp:Button ID="Button2" CssClass="btn  btn-default btn-sm " OnClick="Save_Click" Text="Save" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row-fluid">
                <div class="form-group">
                    <div id="ControlDiv1"  runat="server"  class="col-xs-6">

                        <div id="Module" class="form-group">
                            <label class="col-xs-3 control-label">Module</label>
                            <div class="col-xs-8">
                                <asp:DropDownList ID="ModuleDropDownList" runat="server" AutoPostBack="true" CssClass="form-control selectpicker" OnSelectedIndexChanged="ModuleDropDownList_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div id="BusinessRoleAsterix" class="form-group" runat="server">
                            <label class="col-xs-3 control-label">Business Role</label>
                            <div class="col-xs-8">
                                <telerik:RadAutoCompleteBox ID="BusinessRoleAutoCompleteBox" runat="server" EmptyMessage="Enter * to view full list" TextSettings-SelectionMode="Single"
                                    InputType="Token" CssClass="form-control" Rows="4" required="required"  OnEntryRemoved="BusinessRoleAutoCompleteBox_EntryRemoved" OnEntryAdded="BusinessRoleAutoCompleteBox_EntryAdded" OnClientEntryAdded="OnClientEntryAdded" EnableAjaxSkinRendering="true" RegisterWithScriptManager="true" OnClientRequesting="RequestingFromBusinessRole">

                                    <WebServiceSettings Path="../../../../Services/AutoCompleteDataService.svc" Method="GetBulkBusinessRole" />
                                    <TextSettings SelectionMode="Single" />
                                </telerik:RadAutoCompleteBox>
                            </div>
                        </div>

                        <div class="form-group" runat="server" id="divCompany">
                            <label class="col-xs-3 control-label">Company</label>
                            <div class="form-group col-xs-8">
                                <telerik:RadComboBox CheckBoxes="true" ID="CompanyAutoCompleteBox" AutoPostBack="true" CssClass="form-control selectpicker companystyle" OnSelectedIndexChanged="CompanyAutoCompleteBox_SelectedIndexChanged" runat="server" Enabled="false">
                                </telerik:RadComboBox>
                                <asp:CheckBox ID="RemoveCompanyCheckBox" CssClass="companystyleForCheckBox" runat="server" Visible="false" Checked="false" Text="Retain Company Roles" />
                            </div>
                        </div>

                        <div class="form-group" style="margin-top: -25px;" runat="server" id="divCostCenter">
                            <label class="col-xs-3 control-label">Cost Center</label>
                            <div class="col-xs-8">
                                <telerik:RadComboBox CheckBoxes="true" ID="CostCenterAutoCompleteBox" CssClass="form-control selectpicker fullwidth" runat="server" Enabled="false">
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

                    <div id="ControlDiv" runat="server" class="col-xs-6">

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

                        <div class="form-group">

                            <asp:Button ID="DeleteAccountAdd" CssClass="btn btn-default btn-sm" OnClick="AddButton_Click" Text="Add Request" runat="server" />

                            <asp:Button ID="Button1" CssClass="btn btn-default btn-sm" OnClick="Save_Click" Text="Save" runat="server" Visible="false" />

                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>

    <table border="0" cellpadding="0" cellspacing="0" width="99%" style="margin-left: 5px">
        <tr>
            <td>
                <telerik:RadGrid ID="RequestGrid" runat="server" AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                    GridLines="None" Width="100%" OnNeedDataSource="RequestGrid_NeedDataSource" OnItemCommand="RequestGrid_ItemCommand" ShowFooter="false"
                    MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Request Available">
                    <GroupingSettings CaseSensitive="false" />
                    <ClientSettings>
                        <Scrolling AllowScroll="true" UseStaticHeaders="true" />
                    </ClientSettings>
                    <MasterTableView AutoGenerateColumns="False" DataKeyNames="Entitlement,Company" Width="100%" TableLayout="Fixed">
                        <ItemStyle   Wrap="True" /> 
                        <Columns>
                             <telerik:GridButtonColumn ConfirmDialogType="RadWindow" ConfirmTitle="Delete" UniqueName="Delete" ConfirmText="Are you sure you want to delete?" CommandName="Delete" ButtonType="ImageButton" ItemStyle-CssClass="action-delete" ItemStyle-Width="1px" HeaderText="">
                                <HeaderStyle Width="60px" />
                            </telerik:GridButtonColumn>
                            <telerik:GridTemplateColumn HeaderStyle-Width="300px" HeaderText="Technical role" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:LinkButton ID="ViewTechnicalRolesLinkButton" CommandName="ViewTechicalRole" Text="View Technical Roles" runat="server"></asp:LinkButton>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                           
                            <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Business Role" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:Label ID="EntitlementLabel" runat="server" Text='<%#Eval("Entitlement") %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Company" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:Label ID="CompanyLabel" runat="server" Text='<%#Eval("Company") %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="CostCenter" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:Label ID="CostCenterLabel" runat="server" Text='<%#Eval("CostCenter") %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="PurchaseOrg" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:Label ID="PurchaseOrgLabel" runat="server" Text='<%#Eval("PurchaseOrg") %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="PurchaseGroup" ItemStyle-HorizontalAlign="Left">
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

<telerik:RadWindow runat="server" ID="ViewTechnicalSourceWindow" Modal="true" Height="100%" Width="700px" Title="Techinical Roles"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title">Techinical Roles</h3>
                </div>
                <div class="modal-body">
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
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnCancel" CssClass="uiboxgray" runat="server" Text="Close" OnClick="btnCancel_Click" />
                </div>
            </ContentTemplate>

        </asp:UpdatePanel>

    </ContentTemplate>

</telerik:RadWindow>
