<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BulkTaskDetail.ascx.cs" Inherits="DI.Chakra.Web.Task.BulkTask.UC.BulkTaskDetail" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/Task/InfoUC.ascx" TagPrefix="uc1" TagName="InfoUC" %>
<%@ Register Src="~/UsrControl/Common/CommonTaskInfoUC.ascx" TagPrefix="uc1" TagName="CommonTaskInfoUC" %>
<%@ Register Src="~/UsrControl/Common/ApprverChain.ascx" TagPrefix="uc1" TagName="ApprverChain" %>
<%@ Register Src="~/UsrControl/Common/Task/Applications/InhouseTaskInfoUC.ascx" TagPrefix="uc1" TagName="InhouseTaskInfoUC" %>
<%@ Register Src="~/UsrControl/Common/Task/Applications/InhouseMockTaskInfoUC.ascx" TagPrefix="uc1" TagName="InhouseMockTaskInfoUC" %>
<%@ Register Src="~/UsrControl/Common/SendNotification.ascx" TagPrefix="uc1" TagName="SendNotificationUC" %>
<%@ Register Src="~/UsrControl/Common/ViewNotification.ascx" TagPrefix="uc1" TagName="ViewNotificationUC" %>
<%@ Register Src="~/UsrControl/Common/ViewAttachmentUC.ascx" TagPrefix="uc1" TagName="ViewAttachmentUC" %>
<%@ Register Src="~/UsrControl/Common/NoteInfoUC.ascx" TagPrefix="uc2" TagName="NoteInfoUC" %>
<%@ Register Src="~/Request/BulkRequest/UC/BulkGenericRequestDetails.ascx" TagPrefix="uc1" TagName="BulkGenericRequestDetails" %>
<%@ Register Src="~/UsrControl/Common/UserInfoUC.ascx" TagPrefix="uc1" TagName="UserInfoUC" %>
<%@ Register Src="~/UsrControl/Common/ViewCurrentAccessUC.ascx" TagPrefix="uc1" TagName="ViewCurrentAccessUC" %>
<%@ Register Src="~/UsrControl/Common/ConfirmBox.ascx" TagPrefix="uc1" TagName="ConfirmMessage" %>
<%@ Register Src="~/UsrControl/Common/CurrentAccessUC.ascx" TagPrefix="uc1" TagName="CurrentAccessUC" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<style>
    .modal-body .form-group {
        margin-bottom: 0.3em !important;
    }

    .desc-textwrap {
        word-wrap: break-word;
    }

    .RadComboBoxFullWidth {
        width: 100% !important;
    }

    .repeater-border {
        margin: 15px 0 !important;
        border-bottom: 1px solid #ddd;
    }

    .clear-padding {
        padding-left: 0px !important;
    }
</style>

<script>
    function goBack() {
        window.history.back(); 
    }
function showWindow() {
        var oWnd = $find("<%= RadWinViewCurrentAccess.ClientID %>");
        oWnd.show();
    }

    function closeWindow() {
        var oWnd = $find("<%= RadWinViewCurrentAccess.ClientID %>");
        oWnd.close();
    }
     function OnClientRequestingForSAPCE(sender, args) {
        var context = args.get_context()

         context["endpointSystemId"] = document.getElementById("<%= hdnEndpointSystemId.ClientID %>").value;
         context["endpointUserId"] = document.getElementById("<%= hdnEndpointUserId.ClientID %>").value;
         context["actionType"]= document.getElementById("<%= hdnActionType.ClientID %>").value;
    }
</script>
<asp:UpdatePanel ID="updSearch" runat="server" ChildrenAsTriggers="true">
    <ContentTemplate>
        <div class="form">
            <div class="form-horizontal left  clearfix">
                <div class="row-fluid">
                    <div class="table-header left">
                        <h4>Bulk Request</h4>
                    </div>
                </div>
                <div class="row-fluid col-xs-12">
                    <div class="col-xs-6">

                        <div class="form-group">
                            <label class="col-xs-4">Request Type</label>
                            <div class="col-xs-8">
                                <asp:Label ID="lblRequestName" runat="server" class="description"></asp:Label>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-4">Request Date</label>
                            <div class="col-xs-8">
                                <asp:Label ID="lblRequestedDate" runat="server" class="description"></asp:Label>
                            </div>
                        </div>

                    </div>

                    <div class="col-xs-6">

                        <div class="form-group">
                            <label class="col-xs-4">Requested Employee Type</label>
                            <div class="col-xs-8">
                                <asp:Label ID="lblRequestFor" runat="server" class="description"></asp:Label>
                            </div>
                        </div>


                        <div class="form-group">
                            <label class="col-xs-4">Request By</label>
                            <div class="col-xs-8">
                                <asp:Label ID="lblRequesteeName" runat="server" class="description"></asp:Label>
                            </div>
                        </div>

                    </div>

                </div>

                <div id="trentitle" runat="server" visible="false" class="row-fluid col-xs-12">

                    <div class="col-xs-6">

                        <div class="form-group">
                            <label class="col-xs-4">Group</label>
                            <div class="col-xs-8">
                                <asp:Label ID="lblgroupinfo" runat="server" class="description"></asp:Label>
                            </div>
                        </div>
                    </div>

                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-xs-4">Role</label>
                            <div class="col-xs-8">
                                <asp:Label ID="lbroleinfo" runat="server" class="description"></asp:Label>
                            </div>
                        </div>
                    </div>

                </div>


             <%--   <div class="row-fluid col-xs-12">

                    <div class="col-xs-6">
                        <div class="form-group">

                            <label class="col-xs-4">Status</label>
                            <div class="col-xs-8">
                                <asp:Label ID="lblStatus" runat="server" class="description">Waiting For Manager Approval (Hui Wai)</asp:Label>
                            </div>

                        </div>
                    </div>
                </div>--%>




                <uc1:BulkGenericRequestDetails runat="server" ID="BulkGenericRequestDetails" />
                 <asp:HiddenField runat="server"  ID="hdnActionType"/>
                <asp:HiddenField runat="server" ID="hdnEndpointUserId" />
                <asp:HiddenField runat="server"  ID="hdnEndpointSystemId"/>

                <div class="table-header mTop15">
                    <h4>Approval</h4>
                </div>
                <div class="col-xs-12">
                    <div class="row" style="margin-bottom: 10px;">
                        <div class="col-xs-12">

                            <telerik:RadGrid ID="grdApproverList" runat="server" AllowPaging="false" AllowSorting="false"
                                CellSpacing="0" GridLines="None" Width="100%" OnDetailTableDataBind="grdApprover_DetailTableDataBind">
                                <PagerStyle Mode="NextPrevAndNumeric" PageSizes="20" />
                                <ExportSettings
                                    HideStructureColumns="true"
                                    IgnorePaging="true" ExportOnlyData="true"
                                    Excel-Format="ExcelML"
                                    OpenInNewWindow="true">
                                    <Pdf PageHeight="210mm" AllowPrinting="true" PageWidth="297mm" DefaultFontFamily="Arial Unicode MS" PageTopMargin="45mm"
                                        BorderStyle="Medium" BorderColor="#666666" BorderType="AllBorders">
                                    </Pdf>
                                </ExportSettings>
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" NoMasterRecordsText="No previous approvals" DataKeyNames="ApprovalState"  Width="100%">
                                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="true" ShowExportToExcelButton="false" ShowExportToPdfButton="false" ShowExportToCsvButton="false"></CommandItemSettings>
                                    <DetailTables>
                                        <telerik:GridTableView AutoGenerateColumns="false" EnableHierarchyExpandAll="true" ShowHeader="true" Width="100%"
                                            runat="server" Name="Entitlements" NoDetailRecordsText="No records found." AllowPaging="false" AllowFilteringByColumn="false">
                                            <Columns>
                                                <telerik:GridBoundColumn DataField="TaskOwner" FilterControlAltText="TaskOwner" SortExpression="TaskOwner" HeaderText="Approver Name" UniqueName="TaskOwner"  HeaderStyle-Width="15%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="Status" FilterControlAltText="Status" SortExpression="Status" HeaderText="Status" UniqueName="Status"  HeaderStyle-Width="5%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="ActionDate" FilterControlAltText="ActionDate" SortExpression="ActionDate" HeaderText="ActionDate" UniqueName="ActionDate"  HeaderStyle-Width="13%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="Comments" FilterControlAltText="Comments" SortExpression="Comments" HeaderText="Comments" UniqueName="Comments"  HeaderStyle-Width="25%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridTemplateColumn  DataField="Remarks" FilterControlAltText="Remarks" SortExpression="Remarks" HeaderText="System Remarks" UniqueName="Remarks" HeaderStyle-Width="13%"  ItemStyle-CssClass="ApproverCommonWidth">
                                                                    <ItemTemplate>
                                                                <asp:Label runat="server" ID="lblDisplayName" Text="View Remarks" Font-Bold="true" ForeColor="#0066ff" Width="100%" Visible='<%# !string.IsNullOrWhiteSpace ( Convert.ToString( Eval("Remarks")) )  %>'></asp:Label>
                                                                <asp:HoverMenuExtender ID="hoverDisplayName" runat="server" PopupControlID="pnlDescription" PopupPosition="Center" TargetControlID="lblDisplayName"></asp:HoverMenuExtender>
                                                                <asp:Panel ID="pnlDescription" runat="server" CssClass="HoverPopupMsslEBS">
                                                                    <%# Eval("Remarks") %>
                                                                </asp:Panel>
                                                            </ItemTemplate>
                                                                  
                                                                </telerik:GridTemplateColumn>
                                                <telerik:GridBoundColumn DataField="DelegatedFrom" FilterControlAltText="DelegatedFrom" SortExpression="DelegatedFrom" HeaderText="Delegated From" UniqueName="DelegatedFrom"  HeaderStyle-Width="10%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="DelegatedBy" FilterControlAltText="DelegatedBy" SortExpression="DelegatedBy" HeaderText="Delegated By" UniqueName="DelegatedBy"  HeaderStyle-Width="10%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="DelegationComments" FilterControlAltText="DelegationComments" SortExpression="DelegationComments" HeaderText="Delegation Comments" UniqueName="DelegationComments"  HeaderStyle-Width="22%">
                                                </telerik:GridBoundColumn>
                                            </Columns>
                                        </telerik:GridTableView>
                                    </DetailTables>
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="ApproverName" FilterControlAltText="ApproverName" SortExpression="ApproverName" HeaderText="Approver Name" UniqueName="ApproverName">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridTemplateColumn HeaderText="Approver Role" SortExpression="ApproverType" UniqueName="ApproverType">
                                            <ItemTemplate>
                                                <asp:Label ID="lblApprover" runat="server" Text='<%#Eval("ApproverType") != null && Eval("ApproverType").ToString() == "Group - NPAC Approver"?"Group - NPAC/NMAC/PROTIS/ENIS Approver":Eval("ApproverType") %>' />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn DataField="Type" FilterControlAltText="Type" SortExpression="Type" HeaderText="Type" UniqueName="Type">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Status" FilterControlAltText="Status" SortExpression="Status" HeaderText="Status" UniqueName="Status">
                                        </telerik:GridBoundColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                        </div>
                    </div>
                </div>

                <%--<div id="divExecutionerHeader" runat="server" class="table-header clear">
                    <h4><%= T("App.Common.Executioner") %></h4>
                </div>

                <div id="divExecutionerGrid" runat="server" class="col-xs-12">
                    <div class="row" style="margin-bottom: 10px;">
                        <div class="col-xs-12">
                            <telerik:RadGrid ID="grdExecutioner" runat="server" AllowPaging="false" AllowSorting="false"
                                CellSpacing="0" GridLines="None" Width="100%" OnDetailTableDataBind="grdApprover_DetailTableDataBind">
                                <PagerStyle Mode="NextPrevAndNumeric" PageSizes="20" />
                                <ExportSettings
                                    HideStructureColumns="true"
                                    IgnorePaging="true" ExportOnlyData="true"
                                    Excel-Format="ExcelML"
                                    OpenInNewWindow="true">
                                    <Pdf PageHeight="210mm" AllowPrinting="true" PageWidth="297mm" DefaultFontFamily="Arial Unicode MS" PageTopMargin="45mm"
                                        BorderStyle="Medium" BorderColor="#666666" BorderType="AllBorders">
                                    </Pdf>
                                </ExportSettings>
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" NoMasterRecordsText="No previous approvals" DataKeyNames="ApprovalState">
                                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="true" ShowExportToExcelButton="false" ShowExportToPdfButton="false" ShowExportToCsvButton="false"></CommandItemSettings>
                                    <DetailTables>
                                        <telerik:GridTableView AutoGenerateColumns="false" EnableHierarchyExpandAll="true" ShowHeader="true" Width="100%"
                                            runat="server" Name="Entitlements" NoDetailRecordsText="No records found." AllowPaging="false" AllowFilteringByColumn="false">
                                            <Columns>
                                                <telerik:GridBoundColumn DataField="TaskOwner" FilterControlAltText="TaskOwner" SortExpression="TaskOwner" HeaderText="Approver Name" UniqueName="TaskOwner">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="Status" FilterControlAltText="Status" SortExpression="Status" HeaderText="Status" UniqueName="Status">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="ActionDate" FilterControlAltText="ActionDate" SortExpression="ActionDate" HeaderText="ActionDate" UniqueName="ActionDate">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="Comments" FilterControlAltText="Comments" SortExpression="Comments" HeaderText="Comments" UniqueName="Comments">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="DelegatedFrom" FilterControlAltText="DelegatedFrom" SortExpression="DelegatedFrom" HeaderText="Delegated From" UniqueName="DelegatedFrom">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="DelegatedBy" FilterControlAltText="DelegatedBy" SortExpression="DelegatedBy" HeaderText="Delegated By" UniqueName="DelegatedBy">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="DelegationComments" FilterControlAltText="DelegationComments" SortExpression="DelegationComments" HeaderText="Delegation Comments" UniqueName="DelegationComments">
                                                </telerik:GridBoundColumn>
                                            </Columns>
                                        </telerik:GridTableView>
                                    </DetailTables>
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="ApproverName" FilterControlAltText="ApproverName" SortExpression="ApproverName" HeaderText="Approver Name" UniqueName="ApproverName">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridTemplateColumn HeaderText="Approver Role" SortExpression="ApproverType" UniqueName="ApproverType">
                                            <ItemTemplate>
                                                <asp:Label ID="lblApprover" runat="server" Text='<%#Eval("ApproverType") != null && Eval("ApproverType").ToString() == "Group - NPAC Approver"?"Group - NPAC/NMAC/PROTIS/ENIS Approver":Eval("ApproverType") %>' />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn DataField="Status" FilterControlAltText="Status" SortExpression="Status" HeaderText="Status" UniqueName="Status">
                                        </telerik:GridBoundColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                        </div>
                    </div>
                </div>--%>


                <asp:Panel ID="MainPanel" runat="server" Visible="false">


                    <div class="table-header mTop15">
                        <h4 id="MainPanelHeaderSpan" runat="server">Prefix Submission</h4>
                    </div>


                    <asp:Panel ID="PostionPrefixPanel" runat="server" Visible="false">
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-xs-4">Position Prefix</label>
                                <div class="col-xs-7">
                                    <asp:TextBox ID="txtPrefixValue" MaxLength="4" runat="server" CssClass="form-control" Width="345px"></asp:TextBox><br />

                                </div>
                            </div>
                        </div>


                    </asp:Panel>

                    <asp:Panel ID="SAPEPEntitlementPanel" runat="server" Visible="true">

                        <div class="row col-xs-12 5Bottom15">
                            <div class="col-xs-6">
                                <div id="GroupEntilementtr" runat="server" class="form-group">
                                    <label class="col-xs-3">Group</label>
                                    <div class="col-xs-6">
                                        <telerik:RadComboBox CheckBoxes="true" ID="ExistingGroupAutoCompleteBox"  CssClass="form-control RadComboBoxFullWidth" DropDownAutoWidth="Enabled" runat="server" Visible="false" AutoPostBack="true">
                                        </telerik:RadComboBox>
                                        <telerik:RadAutoCompleteBox ID="GroupsAutoCompleteBox" HighlightFirstMatch="false"  OnClientRequesting="OnClientRequestingForSAPCE"  CssClass="form-control" runat="server" Visible="false" EmptyMessage="Enter * to view full list" DropDownHeight="500px" DropDownWidth="500px" InputType="Token">
                                            <TextSettings SelectionMode="Single" />
                                            <WebServiceSettings Method="GetPortalGroups" Path="../../Services/AutoCompleteDataService.svc" />
                                        </telerik:RadAutoCompleteBox>
                                    </div>
                                    <div class="col-xs-2">
                                        <asp:Button ID="btnGrpAdd" runat="server" CssClass="btn btn-default" OnClick="btnGrpAdd_Click" Text="Add" />
                                    </div>
                                </div>
                            </div>

                            <div class="col-xs-6">
                                <div id="RoleEntilementtr" runat="server" class="form-group">
                                    <label class="col-xs-3">Role</label>
                                    <div class="col-xs-6">
                                        <telerik:RadComboBox CheckBoxes="true" ID="ExistingRoleAutoCompleteBox" CssClass="form-control RadComboBoxFullWidth" DropDownAutoWidth="Enabled" runat="server" Visible="false" AutoPostBack="true">
                                        </telerik:RadComboBox>
                                        <telerik:RadAutoCompleteBox ID="RolesAutoCompleteBox" HighlightFirstMatch="false"  CssClass="form-control" OnClientRequesting="OnClientRequestingForSAPCE" runat="server" Visible="false" EmptyMessage="Enter * to view full list" DropDownHeight="200px" DropDownWidth="500px" InputType="Token">
                                            <TextSettings SelectionMode="Single" />
                                            <WebServiceSettings Method="GetPortalRoles" Path="../../Services/AutoCompleteDataService.svc" />
                                        </telerik:RadAutoCompleteBox>
                                    </div>
                                    <div class="col-xs-2">
                                        <asp:Button ID="btnroleAdd" runat="server" CssClass="btn btn-default" OnClick="btnroleAdd_Click" Text="Add" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row col-xs-12 5Bottom15">
                            <div id="trGroupGrid" runat="server" visible="false" class="col-xs-6 form-group">
                                <telerik:RadGrid ID="GroupGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0" GridLines="None">
                                    <MasterTableView AutoGenerateColumns="False" DataKeyNames="Key"  Width="100%">
                                        <Columns>
                                            <telerik:GridBoundColumn DataField="Value" FilterControlAltText="Value" HeaderStyle-Width="300px" HeaderText="Group" SortExpression="Value">
                                            </telerik:GridBoundColumn>
                                            <telerik:GridTemplateColumn HeaderStyle-Width="60px">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="DeleteGroupButton" runat="server" CommandArgument='<%#Eval("Key") %>' OnClick="DeleteGroupButton_Click" Text="Delete" ToolTip="Edit" />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>
                            </div>

                            <div id="trRoleGrid" runat="server" class="col-xs-6 form-group">
                                <telerik:RadGrid ID="RoleGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0" GridLines="None">
                                    <MasterTableView AutoGenerateColumns="False" DataKeyNames="Key"  Width="100%">
                                        <Columns>
                                            <telerik:GridBoundColumn DataField="Value" FilterControlAltText="Value" HeaderStyle-Width="300px" HeaderText="Role" SortExpression="Value">
                                            </telerik:GridBoundColumn>
                                            <telerik:GridTemplateColumn HeaderStyle-Width="60px">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="DeleteRoleButton" runat="server" CommandArgument='<%#Eval("Key") %>' OnClick="DeleteRoleButton_Click" Text="Delete" ToolTip="Edit" />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>
                            </div>

                        </div>



                        <%-- <div class="row 5Bottom15">
                    <div class="col-xs-6">
                        <div id="GroupEntilementtr" runat="server" class="form-group">
                            <label class="col-xs-3">Group</label>
                            <div class="col-xs-9">
                                <div class="col-xs-8">
                                    <telerik:radcombobox checkboxes="true" id="ExistingGroupAutoCompleteBox" runat="server" visible="false" autopostback="true">
                                </telerik:radcombobox>
                                    <telerik:radautocompletebox id="GroupsAutoCompleteBox" cssclass="form-control" runat="server" visible="false" emptymessage="Enter * to view full list" inputtype="Token">
                                    <WebServiceSettings Method="GetPortalGroups" Path="../../Services/AutoCompleteDataService.svc" />
                                </telerik:radautocompletebox>
                                </div>
                                <div class="col-xs-4">
                                    <asp:Button ID="btnGrpAdd" runat="server" CssClass="btn btn-default" OnClick="btnGrpAdd_Click" Text="Add" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-6">
                        <div id="trGroupGrid" runat="server" class="form-group" visible="false">
                            <telerik:radgrid id="GroupGrid" runat="server" allowpaging="false" allowsorting="false" cellspacing="0" gridlines="None" width="300px">
                                <MasterTableView AutoGenerateColumns="False" DataKeyNames="Key">
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="Value" FilterControlAltText="Value" HeaderStyle-Width="300px" HeaderText="Group" SortExpression="Value">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="70px">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="DeleteGroupButton" runat="server" CommandArgument='<%#Eval("Key") %>' OnClick="DeleteGroupButton_Click" Text="Delete" ToolTip="Edit" />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:radgrid>
                        </div>

                    </div>

                </div>
                </br>
                <div class="row 5Bottom15">
                    <div class="col-xs-6">
                        <div id="RoleEntilementtr" runat="server" class="form-group">
                            <label class="col-xs-3">Role</label>
                            <div class="col-xs-9">
                                <div class="col-xs-8">
                                    <telerik:radcombobox checkboxes="true" id="ExistingRoleAutoCompleteBox" runat="server" visible="false" autopostback="true">
                                </telerik:radcombobox>
                                    <telerik:radautocompletebox id="RolesAutoCompleteBox" cssclass="form-control" runat="server" visible="false" emptymessage="Enter * to view full list" inputtype="Token">
                                    <WebServiceSettings Method="GetPortalRoles" Path="../../Services/AutoCompleteDataService.svc" />
                                </telerik:radautocompletebox>
                                </div>
                                <div class="col-xs-4">
                                    <asp:Button ID="btnroleAdd" runat="server" CssClass="btn btn-default" OnClick="btnroleAdd_Click" Text="Add" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-xs-6">
                        <div id="trRoleGrid" runat="server" class="form-group" visible="false">
                            <telerik:radgrid id="RoleGrid" runat="server" allowpaging="false" allowsorting="false" cellspacing="0" gridlines="None" width="300px">
                                <MasterTableView AutoGenerateColumns="False" DataKeyNames="Key">
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="Value" FilterControlAltText="Value" HeaderStyle-Width="300px" HeaderText="Role" SortExpression="Value">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="70px">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="DeleteRoleButton" runat="server" CommandArgument='<%#Eval("Key") %>' OnClick="DeleteRoleButton_Click" Text="Delete" ToolTip="Edit" />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:radgrid>
                        </div>
                    </div>
                </div>--%>
                    </asp:Panel>

                </asp:Panel>

                <div class="table-header clear">
                    <h4>Additional Task Information</h4>
                </div>


                <div class="col-xs-12 mBottom15">

                    <div id="ExceptionRemarkstr" runat="server" class="form-group" visible="false">
                        <label class="col-xs-4" style="width: auto; min-width: 175px;">Exception Remarks</label>
                        <div class="col-xs-12">
                            <asp:Label ID="ExceptionRemarksValueLabel" runat="server" CssClass="description"></asp:Label>
                        </div>
                    </div>

                    <div id="AdditionalInformationtr" runat="server" class="form-group" visible="false">
                        <label class="col-xs-4" style="width: auto; min-width: 175px;">Additional Task Information</label>
                        <div class="col-xs-12">
                            <asp:Label ID="MSSLAdditionalInfoValueLabel" runat="server" ForeColor="#627F97" CssClass="description"></asp:Label>
                        </div>
                    </div>

                    <div id="ConflictsRemarktr" runat="server" class="form-group" visible="false">
                        <label class="col-xs-4" style="width: auto; min-width: 175px;">Conflict Remarks</label>
                        <div class="col-xs-8">
                            <asp:Label ID="ConflictsRemarkLabel" runat="server" ForeColor="#627F97" CssClass="description"></asp:Label>
                            <asp:Image ID="ConflictsRemarkImage" runat="server" Width="20px" Height="20px" src="../../Images/warning32.png" alt="" />
                        </div>
                    </div>


                    <div id="RemarksValuetr" runat="server" class="form-group" visible="false">
                        <label class="col-xs-4" style="width: auto; min-width: 175px;">Remarks</label>
                        <div class="col-xs-8">
                            <asp:Label ID="RemarksValueLabel" runat="server" CssClass="description"></asp:Label>
                        </div>
                    </div>
                    <div id="RoleNameValuetr" runat="server" class="form-group" visible="false">
                        <label class="col-xs-4" style="width: auto; min-width: 175px;">RoleName</label>
                        <div class="col-xs-8">
                            <asp:Label ID="RoleNameValueLabel" runat="server" CssClass="description"></asp:Label>
                        </div>
                    </div>

                    <div id="TCodeValueLabeltr" runat="server" class="form-group" visible="false">
                        <label class="col-xs-4" style="width: auto; min-width: 175px;">TCode</label>
                        <div class="col-xs-8">
                            <asp:Label ID="TCodeValueLabel" runat="server" CssClass="description"></asp:Label>
                        </div>
                    </div>

                    <div id="DelegatedBytr" runat="server" class="form-group" visible="false">
                        <label class="col-xs-4" style="width: auto; min-width: 175px;">Delegated By</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblDelegatedBy" CssClass="description" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                    <div id="DelegatedFromtr" runat="server" class="form-group" visible="false">
                        <label class="col-xs-4" style="width: auto; min-width: 175px;">Delegated From</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblDelegatedFrom" CssClass="description" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                    <div id="DelegationCommentstr" runat="server" class="form-group" visible="false">
                        <label class="col-xs-4" style="width: auto; min-width: 175px;">Delegation Comments</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblDelegationComments" CssClass="description" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                    <div id="Tr3" runat="server" class="form-group" visible="false">
                        <label class="col-xs-4" style="width: auto; min-width: 175px;">Updated Termination Date</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblcuurentTerminationDate" CssClass="description" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                    <div id="Tr4" runat="server" class="form-group" visible="false">
                        <label class="col-xs-4" style="width: auto; min-width: 175px;">Updated Termination Date</label>
                        <div class="col-xs-8">
                            <asp:Label ID="lblModifiedTerminationDate" CssClass="description" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                </div>

                <div class="form-group col-xs-12 mBottom15" id="divInhouseUserDetails" runat="server" visible="false">
                    <div class="table-header clear">
                        <h4><%= T("App.Common.MoreDetails") %></h4>
                    </div>

                    <div class="form-group col-xs-12">
                        <asp:Repeater ID="rptInhUserInfo" runat="server" OnItemDataBound="rptInhUserInfo_ItemDataBound" OnItemCreated="rptInhUserInfo_ItemCreated">
                            <ItemTemplate>
                                <div class="form-group repeater-border">
                                    <div class="col-xs-3">
                                        <div class="col-xs-12 clear-padding">
                                            <uc1:UserInfoUC runat="server" ID="UserInfoUC" UserId='<%# DataBinder.Eval(Container.DataItem, "UserId") %>' />
                                        </div>
                                        <div class="col-xs-12 clear-padding">
                                            &nbsp;
                                        </div>
                                        <div class="col-xs-12 clear-padding">
                                            <%--<uc1:ViewCurrentAccessUC runat="server" ID="ViewCurrentAccessUC"  />--%>
                                            <asp:LinkButton ID="lnkViewCurrentAccess"  Text="View Current Access" runat="server" OnCommand="lnkViewCurrentAccess_Command"  CommandName="show" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "UserId") %>'></asp:LinkButton>
                                        </div>
                                    </div>
                                    <div class="col-xs-6">
                                        <div class="col-xs-12">
                                            <div class="col-xs-6">
                                                <asp:Label ID="lblUserInfo" runat="server" Text="Username"></asp:Label><span style="color:red">*</span>
                                                <asp:HiddenField ID="hdnUserId" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "UserId") %>'></asp:HiddenField>
                                            </div>
                                            <div class="col-xs-6">
                                                <asp:TextBox ID="txtUsername" runat="server" MaxLength="20" Width="100%"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-xs-12">
                                            &nbsp;
                                        </div>
                                        <div class="col-xs-12">
                                            <div class="col-xs-6">
                                                <asp:Label ID="lblPassword" runat="server" Text="Password"></asp:Label><span style="color:red">*</span>
                                            </div>
                                            <div class="col-xs-6">
                                                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" MaxLength="100" Width="100%"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-xs-12">
                                            &nbsp;
                                        </div>
                                    </div>
                                    <div class="col-xs-3">
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <div class="row-fluid" id="divEntitlements" runat="server" visible="false">
                    <div class="col-xs-12 form-group">
                        <div class="col-xs-12 table-header clear">
                            <h4 class="form-group">Entitlement Detail</h4>
                        </div>
                    </div>
                    <div class="col-xs-12">
                        <div class="col-xs-6">
                          <%--  <div class="form-group">
                                <asp:Label ID="Label1" AssociatedControlID="RolesAutoCompleteBox" runat="server" CssClass="col-xs-6 control-label"><%= T("App.Common.INHPermission") %></asp:Label>
                                <div class="col-xs-6">
                                    <telerik:RadAutoCompleteBox ID="INHRoleAutoCompleteBox" runat="server" AutoPostBack="true" EmptyMessage="Enter * to view full list" TextSettings-SelectionMode="Single"
                                        InputType="Token" CssClass="form-control" OnEntryAdded="INHRoleAutoCompleteBox_EntryAdded" OnEntryRemoved="INHRoleAutoCompleteBox_EntryRemoved">
                                        <WebServiceSettings Method="GetEntitlementInHouse" Path="~/Services/AutoCompleteDataService.svc" />
                                    </telerik:RadAutoCompleteBox>
                                </div>
                            </div>
                            <div class="form-group" runat="server" id="divFunctions" visible="false">
                                <asp:Label ID="Label2" AssociatedControlID="ddlFunctions" runat="server" CssClass="col-xs-6 control-label">Functions</asp:Label>
                                <div class="col-xs-6">
                                    <telerik:RadComboBox CheckBoxes="true" ID="ddlFunctions" runat="server" CssClass="col-xs-12" Style="width: 100%! important">
                                    </telerik:RadComboBox>
                                </div>
                            </div>
                            <div class="form-group" id="addPermissionButtonDiv" runat="server">
                                <div class="col-xs-6">
                                </div>
                                <div class="col-xs-6">
                                    <asp:Button ID="AddPermissionButton" ValidationGroup="AddPermission" runat="server" Text="Add" OnClick="AddPermissionButton_Click" CssClass="btn btn-default btn-sm" />
                                </div>
                            </div>--%>
                            <div class="form-group" id="divGrid" runat="server" visible="false">
                                <telerik:RadGrid ID="RequestGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                                    GridLines="None" Width="100%" OnItemCommand="RequestGrid_ItemCommand" ShowFooter="false"
                                    MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Request Available">
                                    <GroupingSettings CaseSensitive="false" />
                                    <MasterTableView AutoGenerateColumns="False" Width="100%">
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
                                           <%-- <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="Delete">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="DeleteRoleButton" runat="server" CommandName="Delete" Text="Delete" ToolTip="Delete" />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>--%>
                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>
                            </div>
                        </div>
                    </div>
                </div>


                <div class="table-header clear">
                    <h4><%= T("App.Common.MoreDetails") %></h4>
                </div>

                <div class="col-xs-12">
                    <div class="col-xs-8">
                        <div class="form-group">
                            <label class="col-xs-2"><%= T("App.Common.ApprovalComments") %></label>
                            <div class="col-xs-6">
                                <asp:TextBox ID="txtComments" runat="server" CssClass="wizlbl" MaxLength="255" Width="100%"
                                    TextMode="MultiLine" Rows="7"></asp:TextBox>
                            </div>
                            <div class="col-xs-4">
                                <%--<asp:LinkButton runat="server" ID="lnkApproverChain" OnClick="lnkApproverChain_Click" ><%= T("App.Common.ViewApprovalChain") %></asp:LinkButton>
                        <br />
                        <br />--%>
                                <asp:LinkButton ID="lnkViewNotification" runat="server" Visible="false" OnClick="lnkViewNotification_Click"><%= T("App.Common.ViewNotification") %></asp:LinkButton>
                                <%--<br />
                        <br />--%>
                                <asp:LinkButton ID="lnkViewAttachment" runat="server" OnClick="lnkViewAttachment_Click" Visible="false"></asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="buttons text-right nav-rectangle">
                    <asp:Button ID="btnAttach" CssClass="btn btn-default btn-sm" runat="server" Text="Attach" Visible="false" OnClick="btnAttach_Click" />
                    <%--<asp:Button ID="btnSendNotification" CssClass="btn btn-default btn-sm" runat="server" Text="Send Notification" OnClick="btnSendNotification_Click" />--%>
                    <asp:Button ID="btnSubmit" CssClass="btn btn-default btn-sm" runat="server" Text="Submit" OnClick="SubmitButton_Click" />
                    <asp:Button ID="btnApprove" CssClass="btn btn-default btn-sm green" runat="server" Text="Approve" OnClick="btnApprove_Click" />
                    <asp:Button ID="btnReject" CssClass="btn btn-default btn-sm red" runat="server" Text="Reject" OnClick="btnReject_Click" />
                    <asp:Button ID="BackButton" CssClass="btn btn-default btn-sm" runat="server" Text="Back" OnClientClick="window.history.back();"  />
                     <asp:Button ID="RefreshButton" CssClass="btn btn-default btn-sm" runat="server" Text="Refresh" OnClick="RefreshButton_Click" />
                </div>




            </div>
        </div>




        <telerik:RadWindow runat="server" ID="RadWinSendNotification" Modal="true" Width="600px" Height="100%"
            RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h3 class="modal-title"><%= T("App.Common.SendNotification") %></h3>
                        </div>
                        <div class="modal-body">
                            <div class="row-fluid">
                                <uc1:SendNotificationUC runat="server" ID="UCSendNotification" />
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button runat="server" ID="btnSend" class="btn btn-primary bordered-button" Text="Send" OnClick="btnSend_Click" />
                            <asp:Button runat="server" ID="btnCancelSend" class="btn btn-primary bordered-button" Text="Cancel" OnClick="btnCancelSend_Click" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>

        <telerik:RadWindow runat="server" ID="RadWinViewNotification" Modal="true" Width="900px" Height="100%"
            RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h3 class="modal-title"><%= T("App.Common.ViewNotification") %></h3>
                        </div>
                        <div class="modal-body">
                            <div class="row-fluid">
                                <uc1:ViewNotificationUC runat="server" ID="ViewNotification" />
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button runat="server" ID="btnOk" class="btn btn-primary bordered-button" OnClick="btnOk_Click" Text="OK" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>

        <telerik:RadWindow runat="server" ID="RadWinViewAttachment" Modal="true" Width="500px" Height="600px"
            RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h3 class="modal-title"><%= T("App.Common.ViewAttachment") %></h3>
                        </div>
                        <div class="modal-body">
                            <div class="row-fluid">
                                <uc1:ViewAttachmentUC runat="server" ID="ViewAttachmentUC" />
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button runat="server" ID="btnViewOk" class="btn btn-primary bordered-button" Text="OK" OnClick="btnViewOk_Click" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>

        <telerik:RadWindow runat="server" ID="RadWinAttachFile" Modal="true" Width="500px" Height="100%"
            RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h3 class="modal-title"><%=T("App.Common.AttachFile") %></h3>
                        </div>
                        <div class="modal-body">
                            <div class="row-fluid">
                                <div class="form clearfix">
                                    <div class="form-horizontal left">
                                        <div class="row-fluid">
                                            <div class="col-xs-12">
                                                <div class="form-group">
                                                    <asp:Label ID="lblAttachFile" AssociatedControlID="AsyncUploadAttach" runat="server" Text="Attach File" CssClass="col-xs-3"></asp:Label>
                                                    <div class="col-xs-9">
                                                        <telerik:RadAsyncUpload RenderMode="Lightweight" runat="server" ID="AsyncUploadAttach"
                                                            HideFileInput="true" AllowedFileExtensions=".jpeg,.jpg,.png,.doc,.docx,.xls,.xlsx,.csv,.pdf,.txt,.zip"
                                                            OnClientValidationFailed="OnClientValidationFailed" MaxFileSize="10485760"
                                                            Width="100%" EnableInlineProgress="false" />
                                                        <span class="allowed-attachments">Allowed File Formats <span class="allowed-attachments-list">(<%= String.Join( ",", AsyncUploadAttach.AllowedFileExtensions ) %>)</span></span>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button runat="server" ID="btnSave" class="btn btn-primary bordered-button" Text="Save" OnClick="btnSave_Click" />
                            <asp:Button runat="server" ID="btnCancelAdd" class="btn btn-primary bordered-button" Text="Cancel" OnClick="btnCancelAdd_Click" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>
         <telerik:RadWindow runat="server" ID="RadWinViewCurrentAccess" Modal="true" Width="900px" Height="100%"
            RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanelViewAccess" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h3 class="modal-title">Current Access</h3>
                        </div>
                        <div class="modal-body">
                            <div class="row-fluid">
                                <uc1:CurrentAccessUC ID="ucCurrentAccess" runat="server" />
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button runat="server" ID="btnViewAccess" class="btn btn-primary bordered-button" Text="OK" OnClick="btnViewAccess_Click" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>

    </ContentTemplate>
    
</asp:UpdatePanel>

<asp:UpdatePanel ID="ConfirmMessageUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <uc1:ConfirmMessage ID="ConfirmMessage1" runat="server" />
    </ContentTemplate>
</asp:UpdatePanel>
