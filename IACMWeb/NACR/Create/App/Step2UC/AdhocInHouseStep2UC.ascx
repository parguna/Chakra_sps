<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocInHouseStep2UC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.Step2UC.AdhocInHouseStep2UC" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxCtrl" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/SecurityQuestionUC.ascx" TagPrefix="uc1" TagName="SecurityQuestionUC" %>
<%@ Register Src="~/UsrControl/Common/GenericAutoCompleteBox.ascx" TagPrefix="uc1" TagName="GenericAutoCompleteBox" %>

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

    .bootstrap-select {
        width: 100% !important;
    }
</style>

<div class="form">
    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="col-xs-12">
                <asp:Panel ID="SharedPanelInhouse" runat="server" Visible="false">
                    <div class="form-group required">
                        <label class="col-xs-3 control-label">Account Name</label>
                        <div class="col-xs-4">
                            <asp:TextBox ID="txtAcntName" runat="server" MaxLength="20" class="col-xs-12">
                            </asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group required">
                        <label class="col-xs-3 control-label">Display name</label>
                        <div class="col-xs-4">

                            <asp:TextBox ID="txtDisplayName" runat="server" MaxLength="60" class="col-xs-12">
                            </asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-3 control-label">Description</label>
                        <div class="col-xs-4">
                            <asp:TextBox ID="txtDiscription" runat="server" MaxLength="255" class="col-xs-12">
                            </asp:TextBox>
                        </div>
                    </div>
                </asp:Panel>

                <div class="form-group">
                    <div class="col-xs-3"></div>
                    <asp:Panel ID="pnlSecurityQstns" CssClass="col-xs-4" runat="server">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <asp:Panel ID="Panel2" runat="server">
                                    <uc1:SecurityQuestionUC runat="server" ID="SecurityQuestionUC" OnSecurityQuestionPopUp="SecurityQuestionUC_SecurityQuestionPopUp" OnSecurityResponseSaveClicked="SecurityQuestion_SecurityResponseSaveClicked" LinkText="Service account compliance check" />
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="form-horizontal left clearfix">
    <div class="row-fluid">
        <div class="col-xs-12">
            <asp:Panel ID="GroupPanel" runat="server">
                <div class="form-group required" id="divEndpointUser" runat="server">
                    <label class="col-xs-3 control-label">Endpoint User Name</label>
                    <div class="col-xs-4">
                        <asp:DropDownList ID="ddlEPUserName" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlEPUserName_SelectedIndexChanged" Width="100%">
                        </asp:DropDownList>
                    </div>

                </div>
                <asp:Panel ID="OtherControl" runat="server">
                    <div class="form-group">
                        <p class="form-group col-xs-7 description">On selecting the entitlement, description will be shown on the right side if available.</p>
                    </div>
                    <div class="form-group required">
                        <label class="col-xs-3 control-label">Permission</label>
                        <div class="col-xs-4">
                            <telerik:RadAutoCompleteBox ID="RolesAutoCompleteBox" runat="server" AutoPostBack="true" EmptyMessage="Enter * to view full list" TextSettings-SelectionMode="Single" 
                                InputType="Token" CssClass="form-control" OnEntryAdded="RolesAutoCompleteBox_EntryAdded" OnEntryRemoved="RolesAutoCompleteBox_EntryRemoved">
                                <WebServiceSettings Method="GetEntitlementInHouse" Path="../Services/AutoCompleteDataService.svc" />
                            </telerik:RadAutoCompleteBox>
                        </div>
                        <div class="col-xs-5">
                            <asp:Label ID="lblPermissionDescription" runat="server"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group required" runat="server" id="divFunctions" visible="false">
                        <label class="col-xs-3 control-label">Function</label>
                        <div class="col-xs-4">
                            <%--<telerik:RadAutoCompleteBox ID="autoCompleteFunctions" runat="server" DropDownHeight="200px" EmptyMessage="Enter * to view full list" TextSettings-SelectionMode="Single" DropDownWidth="300px" InputType="Text" CssClass="wizlbl" Width="304px">
                                    <WebServiceSettings Method="GetFunctionsInhouse" Path="../Services/AutoCompleteDataService.svc" />
                                </telerik:RadAutoCompleteBox>--%>
                            <telerik:RadComboBox CheckBoxes="true" ID="ddlFunctions" AutoPostBack="true" OnItemChecked="ddlFunctions_ItemChecked" runat="server" CssClass="col-xs-12" Style="width: 100%! important">
                            </telerik:RadComboBox>
                        </div>
                        <div class="col-xs-5">
                            <asp:Label ID="lblFunctionDescription" runat="server"></asp:Label>
                        </div>
                    </div>
                    <br />
                    <div class="form-group">
                         <div class=" col-xs-7" id="divGrid" runat="server" style="padding-left:0px;">
                        <telerik:RadGrid ID="RequestGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                            GridLines="None" Width="100%" OnItemCommand="RequestGrid_ItemCommand" ShowFooter="false"
                            MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Request Available">
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="False"  Width="100%">
                                <Columns>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Permission" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPermission" runat="server" Text='<%# ((KeyValuePair<string,string>)Container.DataItem).Key %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Function" ItemStyle-HorizontalAlign="Left" UniqueName="Function">
                                        <ItemTemplate>
                                            <asp:Label ID="lblFunction" runat="server" Text='<%# ((KeyValuePair<string,string>)Container.DataItem).Value %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="Delete">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="DeleteRoleButton" runat="server" CommandName="Delete" Text="Delete" ToolTip="Delete" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </div>
                    </div>
                   
                </asp:Panel>
            </asp:Panel>
        </div>
    </div>
</div>
<div class="form-horizontal left clearfix">
    <div class="row-fluid">
        <div class="col-xs-12">
            <div class="form-group">
                <div class="col-xs-3"></div>
                <div class="col-xs-4">
                    <asp:Button ID="Button1" CssClass="btn btn-default btn-sm" OnClick="AddRequestButton_Click" Text="Add" runat="server" />
                    <asp:Button ID="Button3" CssClass="btn btn-default btn-sm" OnClick="AddToCart" Text="Save Request" runat="server" Visible="false"/>
                </div>
            </div>
        </div>
    </div>
</div>
