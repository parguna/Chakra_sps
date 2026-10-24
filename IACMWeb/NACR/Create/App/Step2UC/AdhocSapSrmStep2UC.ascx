<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocSapSrmStep2UC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.Step2UC.AdhocSapSrmStep2UC" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/SecurityQuestionUC.ascx" TagPrefix="uc1" TagName="SecurityQuestionUC" %>
<%@ Register Src="~/NACR/Create/App/Special/SpecialAccountWarning.ascx" TagPrefix="uc1" TagName="SpecialAccountWarning" %>

<style>
    .fullwidth {
        width: 100% !important;
    }
</style>
<script type="text/javascript">
    function OnClientRequesting(sender, args) {
        var context = args.get_context()
        context["SRMUserName"] = document.getElementById("<%= hdnUserName.ClientID%>").value;
        context["SRMUserId"] = document.getElementById("<%= hdnSRMUserId.ClientID%>").value;
        context["SRMActionType"] = document.getElementById("<%= hdnHSRMActionType.ClientID%>").value;
        context["SRMAccountType"] = document.getElementById("<%= hdnSRMAccountType.ClientID%>").value;
        context["EndpointId"] = document.getElementById("<%= hdnEndpointId.ClientID%>").value;
    }
    function OnClientEntrySRMAccessRoleAddingHandler(sender, eventArgs) {



        if (sender.get_entries().get_count() > 0) {

            eventArgs.set_cancel(true);
        }
    }
</script>
<div class="form">
    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="col-xs-6">

                <div id="trEndPointUser" runat="server" class="form-group required">
                    <label class="col-xs-3 control-label">EndPoint User Name</label>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlEndpointUser" OnSelectedIndexChanged="ddlEndpointUser_SelectedIndexChanged" CssClass="form-control" AutoPostBack="true" runat="server">
                        </asp:DropDownList>
                    </div>
                </div>

                <asp:HiddenField runat="server" ID="hdnUserName" />
                <asp:HiddenField runat="server" ID="hdnSRMAccountType" />
                <asp:HiddenField runat="server" ID="hdnHSRMActionType" />
                <asp:HiddenField runat="server" ID="hdnSRMUserId" />
                <asp:HiddenField runat="server" ID="hdnEndpointId" />
                <div id="trflinsUsers" runat="server" visible="false" class="form-group required">
                    <label class="col-xs-3 control-label">User Name</label>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlFlinsUsers" AutoPostBack="true" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlFlinsUsers_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>

                </div>

                <asp:Panel ID="SharedPanelFLINSBI" runat="server" Visible="false">

                    <div class="form-group required">
                        <label class="col-xs-3 control-label">SAP UserType</label>
                        <div class="col-xs-8">
                            <asp:DropDownList ID="SAPUserTypeDropDown" CssClass="form-control selectpicker" runat="server">
                            </asp:DropDownList>

                        </div>
                    </div>

                    <div class="form-group required">
                        <label class="col-xs-3 control-label">Account Name</label>
                        <div class="col-xs-8">
                            <asp:TextBox ID="AccontNameTxt" MaxLength="12" runat="server" CssClass="form-control"></asp:TextBox>
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
                </asp:Panel>


                <div id="TRBuisnessRole" runat="server" class="form-group required">
                    <label class="col-xs-3 control-label">Business Role</label>
                    <div class="col-xs-8">
                        <telerik:RadAutoCompleteBox ID="BusinessRoleAutoCompleteBox" OnClientRequesting="OnClientRequesting" OnClientEntryAdding="OnClientEntrySRMAccessRoleAddingHandler" InputType="Token" EmptyMessage="Enter * to view full list" AutoPostBack="true" OnEntryRemoved="BusinessRoleAutoCompleteBox_EntryRemoved" OnEntryAdded="BusinessRoleAutoCompleteBox_EntryAdded" runat="server" DropDownHeight="200px" DropDownWidth="300px" CssClass="form-control">
                            <TextSettings SelectionMode="Single" />
                            <WebServiceSettings Method="GetSRMBusinesRoles" Path="../../../../Services/AutoCompleteDataService.svc" />
                        </telerik:RadAutoCompleteBox>
                    </div>
                </div>

            </div>
            <div class="col-xs-6">
                <div class="form-group" runat="server" id="divMessage" visible="false">
                    <div class="row">
                        <div class="col-xs-12" style="color: blue">
                            The same account name should exist in SAP FLINS and Singaporepower.Local AD.
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="col-xs-6">

                <div id="Div1" runat="server" class="form-group">
                    <label class="col-xs-3 control-label"></label>
                    <div class="col-xs-8">
                        <asp:CheckBox ID="chkboxRetainMainRole" runat="server" Text="Retain Main Role (Remove POrg & PGrp roles)" AutoPostBack="true" Checked="true" OnCheckedChanged="chkboxRetainMainRole_CheckedChanged" />
                    </div>
                </div>

                <div id="TRporg" runat="server" class="form-group required">
                    <label class="col-xs-3 control-label">Purchasing Org</label>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlPOrg" runat="server" OnSelectedIndexChanged="ddlPOrg_SelectedIndexChanged" CssClass="form-control selectpicker" AutoPostBack="true">
                        </asp:DropDownList>
                        <%--<asp:CheckBox ID="chkboxRetainMainRole" runat="server" Text="Retain Main Role (Remove POrg & PGrp roles)" AutoPostBack="true" Checked="true" OnCheckedChanged="chkboxRetainMainRole_CheckedChanged" />--%>
                    </div>
                </div>

                <div id="TRpgrp" runat="server" class="form-group required">
                    <label class="col-xs-3 control-label">Purchasing Group</label>
                    <div class="col-xs-8">
                        <telerik:RadComboBox ID="PGrpComboBox" runat="server" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" Style="width: 100% !important;" OnSelectedIndexChanged="PGrpComboBox_SelectedIndexChanged" AutoPostBack="true"></telerik:RadComboBox>
                    </div>
                </div>
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
                <div class="form-group">
                    <label class="col-xs-3 control-label"></label>
                    <div class="col-xs-8">
                        <%-- <asp:Button ID="AddRequestButton" CssClass="btn btn-default btn-sm" OnClick="AddRequestButton_Click" Text="Add" runat="server" />--%>
                        <asp:Button ID="btnAdd" runat="server" Text="Add" Enabled="false" OnClick="btnAdd_Click" CssClass="btn btn-default btn-sm" />
                        <asp:Button ID="Button1" CssClass="btn btn-default btn-sm" OnClick="AddToCart" Text="Save Request" runat="server" Visible="false" />
                    </div>
                </div>

            </div>
        </div>
    </div>

    <telerik:RadGrid ID="SRMObjGrid" runat="server" Visible="false" AllowPaging="false" AllowSorting="false" CellSpacing="0"
        GridLines="None" Width="100%" ShowFooter="false" OnItemCommand="SRMObjGrid_ItemCommand" OnItemDataBound="SRMObjGrid_ItemDataBound"
        MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Records Available" TableLayout="Fixed" CssClass="RadGridtextWrap" OnDataBound="SRMObjGrid_DataBound">
        <GroupingSettings CaseSensitive="false" />
        <ClientSettings>
            <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true" ScrollHeight="120px" FrozenColumnsCount="2"></Scrolling>
        </ClientSettings>
        <MasterTableView AutoGenerateColumns="False" DataKeyNames="Entitlement,PurchaseOrg,PurchaseGroup,RetainMainRole" TableLayout="Fixed" Width="100%">
            <Columns>
                <telerik:GridBoundColumn DataField="Entitlement" FilterControlAltText="Entitlement" SortExpression="Entitlement" HeaderText="Entitlement" UniqueName="Entitlement">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="PurchaseOrg" FilterControlAltText="PurchaseOrg" HeaderText="Purchase Org" SortExpression="PurchaseOrg" UniqueName="PurchaseOrg">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="PurchaseGroup" FilterControlAltText="PurchaseGroup" SortExpression="PurchaseGroup" HeaderText="Purchase Group" UniqueName="PurchaseGroup">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn UniqueName="RetainMainRole" DataField="RetainMainRole" SortExpression="RetainMainRole" HeaderText="Retain Main Role" Display="false" Visible="false" HeaderStyle-Width="25%">
                    <ItemTemplate>
                        <%# Convert.ToBoolean(Eval("RetainMainRole")) ? "Yes" : "No" %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <%-- <telerik:GridCheckBoxColumn UniqueName="RetainMainRole" DataField="RetainMainRole" SortExpression="RetainMainRole" HeaderText="Retain Main Role">
                </telerik:GridCheckBoxColumn>--%>
                <telerik:GridButtonColumn ConfirmDialogType="RadWindow" ConfirmTitle="Delete" UniqueName="Delete" ConfirmText="Are you sure you want to delete?" CommandName="Delete" ButtonType="ImageButton" ItemStyle-CssClass="action-delete" ItemStyle-Width="1px" HeaderText="">
                    <HeaderStyle Width="30px" />
                </telerik:GridButtonColumn>
                <telerik:GridButtonColumn ButtonType="ImageButton" CommandName="Details" Text="Details" HeaderText="Details"
                    UniqueName="Details" Exportable="false" Visible="false" HeaderStyle-Width="80px" ImageUrl="~/Images/action-buttons/detail.png">
                    <ItemStyle HorizontalAlign="Center" />
                </telerik:GridButtonColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
    <telerik:RadWindow runat="server" ID="detailWindow" Modal="true" Title="Detail" Width="1000px" Height="200%"
        RenderMode="Lightweight" Behaviors="Close" DestroyOnClose="true" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
        <ContentTemplate>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="modal-header">
                        <h3 class="modal-title">Details:</h3>
                    </div>

                    <div class="modal-body">
                        <div class="row-fluid">
                            <asp:Label ID="lblInformation" runat="server"></asp:Label>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button runat="server" ID="Button2" class="btn btn-primary bordered-button" Text="Close" OnClick="detail_OK_Click" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
</div>
<script>
    function showSRMWindow() {
        var oWnd = $find("<%= detailWindow.ClientID %>");
        oWnd.show();
    }

    function closeSRMWindow() {
        var oWnd = $find("<%= detailWindow.ClientID %>");
        oWnd.close();
    }
</script>
