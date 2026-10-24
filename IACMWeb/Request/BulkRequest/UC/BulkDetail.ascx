<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BulkDetail.ascx.cs" Inherits="DI.Chakra.Web.Request.BulkRequest.UC.BulkDetail" %>



<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Request/BulkRequest/UC/BulkGenericRequestDetails.ascx" TagPrefix="uc1" TagName="BulkGenericRequestDetails" %>

<script>
   <%-- function ShowApproverChain() {
        $find("<%= ApproverChain.ClientID %>").show();
        return false;
    }--%>

    function showWindowWithdraw() {
        var oWnd = $find("<%= BulkWithDrawnModalPopupExtender.ClientID %>");
        oWnd.show();
    }

    function closeWindowWithdraw() {
        var oWnd = $find("<%= BulkWithDrawnModalPopupExtender.ClientID %>");
        oWnd.close();
    }

    function showWindowUsers() {
        var oWnd = $find("<%= radWindowUsers.ClientID %>");
        oWnd.show();
    }

    function closeWindowUsers() {
        var oWnd = $find("<%= radWindowUsers.ClientID %>");
        oWnd.close();
    }
</script>

<asp:UpdatePanel ID="updSearch" runat="server" ChildrenAsTriggers="true">
    <ContentTemplate>
        <div class="form">
            <div class="form-horizontal">
                <div class="table-header left clearfix">
                    <%--<img style="float: left; max-width: 50px;" alt="My Bulk Request" src="../../../Images/action-buttons/request.jpg">--%>
                    <h4>Bulk Request</h4>
                </div>

                <div class="row-fluid">
                    <div class="col-xs-6">

                        <div class="form-group">
                            <label class="col-xs-4">Request Type</label>
                            <div class="col-xs-7">
                                <asp:Label ID="lblRequestName" runat="server" class="description"></asp:Label>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-xs-4">Request Date</label>
                            <div class="col-xs-7">
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
                                <asp:Label ID="lblRequestBy" runat="server" class="description"></asp:Label>
                            </div>
                        </div>

                    </div>

                </div>

                <div id="trentitle" runat="server" visible="false" class="row-fluid">

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


                <div class="row-fluid">

                    <div class="col-xs-6">
                        <div class="form-group">

                            <label class="col-xs-4">Status</label>
                            <div class="col-xs-8">
                                <asp:Label ID="lblStatus" runat="server" class="description">Waiting For Manager Approval (Hui Wai)</asp:Label>
                            </div>

                        </div>
                    </div>
                </div>


                <uc1:BulkGenericRequestDetails runat="server" ID="BulkGenericRequestDetails" />




                <div class="table-header mTop15">
                    <h4>Approval</h4>
                </div>
                <div class="col-xs-12">
                    <div class="row" style="margin-bottom: 10px;">
                        <div class="col-xs-12">

                            <telerik:RadGrid ID="grdApproverList" runat="server" AllowPaging="false" AllowSorting="false" RetainExpandStateOnRebind="true" EnableHierarchyExpandAll="true"  OnPreRender="grdApproverList_PreRender"
                                CellSpacing="0" GridLines="None" Width="100%" OnDetailTableDataBind="grdApprover_DetailTableDataBind"  OnItemCommand="grdApproverList_ItemCommand">
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
                                <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" NoMasterRecordsText="No previous approvals" DataKeyNames="ApprovalState,Action,IsGroupApproval,ApproverName"  Width="100%">
                                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="true" ShowExportToExcelButton="false" ShowExportToPdfButton="false" ShowExportToCsvButton="false"></CommandItemSettings>
                                    <DetailTables>
                                        <telerik:GridTableView AutoGenerateColumns="false" EnableHierarchyExpandAll="true" ShowHeader="true" Width="100%"  HierarchyDefaultExpanded="false" 
                                            runat="server" Name="Entitlements" NoDetailRecordsText="No records found." AllowPaging="false" AllowFilteringByColumn="false">
                                            <Columns>
                                                <telerik:GridBoundColumn DataField="TaskOwner" FilterControlAltText="TaskOwner" SortExpression="TaskOwner" HeaderText="Approver Name" UniqueName="TaskOwner" HeaderStyle-Width="15%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="Status" FilterControlAltText="Status" SortExpression="Status" HeaderText="Status" UniqueName="Status" HeaderStyle-Width="5%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="ActionDate" FilterControlAltText="ActionDate" SortExpression="ActionDate" HeaderText="ActionDate" UniqueName="ActionDate" HeaderStyle-Width="13%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="Comments" FilterControlAltText="Comments" SortExpression="Comments" HeaderText="Comments" UniqueName="Comments" HeaderStyle-Width="25%">
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
                                                <telerik:GridBoundColumn DataField="DelegatedFrom" FilterControlAltText="DelegatedFrom" SortExpression="DelegatedFrom" HeaderText="Delegated From" UniqueName="DelegatedFrom" HeaderStyle-Width="10%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="DelegatedBy" FilterControlAltText="DelegatedBy" SortExpression="DelegatedBy" HeaderText="Delegated By" UniqueName="DelegatedBy" HeaderStyle-Width="10%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="DelegationComments" FilterControlAltText="DelegationComments" SortExpression="DelegationComments" HeaderText="Delegation Comments" UniqueName="DelegationComments" HeaderStyle-Width="22%">
                                                </telerik:GridBoundColumn>
                                            </Columns>
                                        </telerik:GridTableView>
                                    </DetailTables>
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="ApproverName" FilterControlAltText="ApproverName" SortExpression="ApproverName" HeaderText="Approver Name" UniqueName="ApproverName">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridTemplateColumn DataField="ApproverType" FilterControlAltText="ApproverType" SortExpression="ApproverType" HeaderText="Approver Type" UniqueName="ApproverType">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkGroupInfo" runat="server" ForeColor='<%# System.Drawing.ColorTranslator.FromHtml(Eval("ForeColor").ToString())%>' Text='<%#Eval("ApproverType") != null && Eval("ApproverType").ToString() == "Group - NPAC Approver"?"Group - NPAC/NMAC/PROTIS/ENIS Approver":Eval("ApproverType") %>'
                                                    CommandArgument='<%#Eval("IsGroup").ToString() + "|" + Eval("ApproverType").ToString()%>' OnCommand="lnkGroupInfo_Command" />
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


                <div class="form-group">
                    <div id="withdrawtd" runat="server" style="float: right !important; padding: 10px;">
                        <asp:Button ID="WithDrawnButton" runat="server" CssClass="btn btn-default btn-sm" Text="Withdraw" OnClick="WithDrawnButton_Click" />
                    </div>
                    <div id="tdbtn" runat="server" style="float: right !important; padding: 10px;">
                        <asp:Button runat="server" Text="Back" CssClass="btn btn-default btn-sm" OnClick="BackButton_Click" />
                    </div>
                    <div id="tdback" visible="false" style="float: right !important; padding: 10px;" runat="server">
                        <asp:Button runat="server" Text="Back" CssClass="btn btn-default btn-sm" OnClick="btnback_Click" />
                    </div>
                    <div style="float: right !important; padding: 10px;" runat="server">
                        <asp:Button ID="RefreshButton" CssClass="btn btn-default btn-sm" runat="server" Text="Refresh" OnClick="RefreshButton_Click" />
                    </div>
                </div>


            </div>
        </div>


    </ContentTemplate>
</asp:UpdatePanel>

<telerik:RadWindow runat="server" ID="BulkWithDrawnModalPopupExtender" Modal="true" Width="500px" Height="100%" Title=""
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title">Withdraw Reason</h3>
                </div>
                <div class="modal-body">
                    <div class="form-horizontal left clearfix">
                        <div>
                            <label class="col-xs-4">Reason<span style="color: red;">*</span></label>
                            <div class="col-xs-8">
                                <asp:TextBox ID="ReasonField" Height="50" TextMode="MultiLine" Width="282px" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rr" Display="Dynamic" ValidationGroup="submit" runat="server" ControlToValidate="ReasonField" CssClass="text-danger" ErrorMessage="Reason is required."></asp:RequiredFieldValidator>

                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" class="btn btn-primary transparent-button" ID="btnSubmit" Text="Submit" OnClick="btnSubmit_Click" CausesValidation="false"></asp:Button>
                    <asp:Button ID="cancelpopup" CssClass="add btn btn-primary bordered-button" runat="server" CausesValidation="true" UseSubmitBehavior="true"
                        Text="Cancel" OnClick="cancelpopup_Click"></asp:Button>

                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>

<telerik:RadWindow runat="server" ID="radWindowUsers" Modal="true" Height="300px" Width="50%" Title="Users"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel5" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title">Users</h3>
                </div>
                <div class="modal-body">
                    <asp:Panel ID="pnlUsers" CssClass="ModalWindow" runat="server">
                        <telerik:RadGrid runat="server" ID="grdUsers" GridLines="None" AllowAutomaticUpdates="false" Width="100%" EnableAjaxSkinRendering="true" AllowPaging="False" CellSpacing="0">
                            <ClientSettings>
                                <Scrolling AllowScroll="True" UseStaticHeaders="True"></Scrolling>
                            </ClientSettings>
                            <PagerStyle Mode="NumericPages" />
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="false"  Width="100%">
                                <Columns>
                                    <telerik:GridBoundColumn DataField="value" FilterControlAltText="value" SortExpression="value" HeaderText="Display name" UniqueName="value">
                                    </telerik:GridBoundColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </asp:Panel>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnCloseUserInfo" CssClass="btn btn-default btn-sm" runat="server" Text="Close" OnClick="btnCloseUserInfo_Click" />
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnCloseUserInfo" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>

    </ContentTemplate>

</telerik:RadWindow>

