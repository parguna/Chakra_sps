<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.Request.UC.DetailUC" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/UsrControl/Common/CommonTaskInfoUC.ascx" TagPrefix="uc1" TagName="CommonTaskInfoUC" %>
<%@ Register Src="~/UsrControl/Common/ApprverChain.ascx" TagPrefix="uc1" TagName="ApprverChain" %>
<%@ Register Src="~/UsrControl/Common/Task/InfoUC.ascx" TagPrefix="uc2" TagName="InfoUC" %>
<%@ Register Src="~/UsrControl/Common/ViewNotification.ascx" TagPrefix="uc1" TagName="ViewNotificationUC" %>
<%@ Register Src="~/UsrControl/Common/ViewAttachmentUC.ascx" TagPrefix="uc1" TagName="ViewAttachmentUC" %>
<%@ Register Src="~/UsrControl/Common/GenericRequestDetails.ascx" TagPrefix="uc1" TagName="GenericRequestDetails" %>


<style>
    .disableClass {
        cursor: not-allowed;
    }

    .employeeRequestDetails {
        position: relative;
    }

    .disableEmployeeDiv {
        position: absolute;
        width: 100%;
        height: 100%;
        background-color: white;
        z-index: 2999;
        opacity: 0.1;
        top: 0;
    }

    .WordWrapTask {
       
        word-wrap: break-word !important;
         
    }
    .navbar {
        z-index: 99999988 !important;
    }
</style>
<script>
   <%-- function ShowApproverChain() {
        $find("<%= ApproverChain.ClientID %>").show();
        return false;
    }--%>

    function showWindowWithdraw() {
        var oWnd = $find("<%= modalPopup.ClientID %>");
        oWnd.show();
    }

    function closeWindowWithdraw() {
        var oWnd = $find("<%= modalPopup.ClientID %>");
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

  <%--  function ValidateReason() {
        alert('1');
        alert(document.getElementById('<%= lblMessage.ClientID %>'));
        alert(document.getElementById('<%= ReasonField.ClientID %>').innerText);
        if (document.getElementById('<%= ReasonField.ClientID %>').innerText == "") {
            alert('3');
            document.getElementById('<%= lblMessage.ClientID %>').style.display = 'block';
            document.getElementById('<%= lblMessage.ClientID %>').style.Visible = true;
            alert($("#UpdateProgressBar"));
            $("#UpdateProgressBar").hide();
            alert('5');
            return false;
        }
        alert('5');
        return true;
    }--%>

    $(document).ready(function () {
        //alert('0');
        $(".employeeRequestDetails").append('<div class="disableEmployeeDiv">&nbsp;</div>');
    });

    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(RunThisAfterEachAsyncPostback);
    function RunThisAfterEachAsyncPostback() {
        $(".employeeRequestDetails").append('<div class="disableEmployeeDiv">&nbsp;</div>');
    }

</script>

<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <div class="content-wrapper">
            <div class="form">
                <div class="form-horizontal left">
                    <div class="row-fluid">
                        <div class="col-md-12">
                            <div class="form-group">
                                <div class="table-header mTop15">
                                    <h4><%= T("App.Request.TaskDetails") %></h4>
                                </div>
                                <uc2:InfoUC runat="server" ID="InfoUC" />
                                <uc1:GenericRequestDetails runat="server" ID="GenericRequestDetails" />
                                <%--<uc1:CommonTaskInfoUC runat="server" ID="CommonTaskInfoUC" canLoadAccessReqItems="true" />--%>
                                <div runat="server" id="exceptionDetails" visible="false">
                                    <div class="table-header mTop15">
                                        <h4>Exception Details</h4>
                                    </div>
                                    <div class="col-xs-12">
                                        <asp:Label ID="lblExceptionDetails" runat="server"></asp:Label>
                                    </div>
                                    <br />
                                </div>
                                <div class="table-header mTop15" id="idHeadingApprovals" runat="server">
                                    <h4><%= T("App.Request.Approval") %></h4>
                                </div>
                                <div class="col-xs-12" id="gridApprovals" runat="server">
                                    <div class="row" style="margin-bottom: 10px;">
                                        <div class="col-xs-12">

                                            <telerik:RadGrid ID="grdApproverList" runat="server" RetainExpandStateOnRebind="true" EnableHierarchyExpandAll="true" AllowPaging="false" AllowSorting="false" OnPreRender="grdApproverList_PreRender"
                                                CellSpacing="0" GridLines="None" Width="100%" OnDetailTableDataBind="grdApprover_DetailTableDataBind" OnItemCommand="grdApproverList_ItemCommand">
                                                <PagerStyle Mode="NextPrevAndNumeric" PageSizes="20" />
                                                <ClientSettings>
                                                    <Resizing EnableRealTimeResize="true" />
                                                </ClientSettings>
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
                                                <MasterTableView AutoGenerateColumns="False" TableLayout="Fixed" CommandItemDisplay="None" NoMasterRecordsText="No previous approvals" DataKeyNames="ApprovalState,ApproverType,Action,IsGroupApproval,ApproverName"  Width="100%">
                                                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="false" ShowExportToCsvButton="false" ></CommandItemSettings>
                                                    <DetailTables>
                                                        <telerik:GridTableView AutoGenerateColumns="false" EnableHierarchyExpandAll="true" ShowHeader="true" HierarchyDefaultExpanded="false" 
                                                            runat="server" Name="Entitlements" NoDetailRecordsText="No records found." AllowPaging="false" AllowFilteringByColumn="false" Width="100%">
                                                            <Columns>
                                                                <telerik:GridBoundColumn DataField="TaskOwner" FilterControlAltText="TaskOwner" SortExpression="TaskOwner" HeaderText="Approver Name" UniqueName="TaskOwner"  HeaderStyle-Width="15%">
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn DataField="Status" FilterControlAltText="Status" SortExpression="Status" ItemStyle-CssClass="WordWrapTask"  HeaderText="Status" UniqueName="Status"  HeaderStyle-Width="5%">
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn DataField="ActionDate" FilterControlAltText="ActionDate" SortExpression="ActionDate" HeaderText="ActionDate" UniqueName="ActionDate"  HeaderStyle-Width="13%">
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn DataField="Comments" FilterControlAltText="Comments" SortExpression="Comments" ItemStyle-CssClass="WordWrapTask"  HeaderText="Comments" UniqueName="Comments"  HeaderStyle-Width="25%">
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
                                                               <%--  <telerik:GridBoundColumn DataField="Remarks" FilterControlAltText="Remarks" SortExpression="Remarks" HeaderText="Remarks" UniqueName="Remarks" HeaderStyle-Width="25%"  ItemStyle-CssClass="ApproverCommonWidth">
                                                                 </telerik:GridBoundColumn>--%>
                                                                <telerik:GridBoundColumn DataField="DelegatedFrom" FilterControlAltText="DelegatedFrom" SortExpression="DelegatedFrom" ItemStyle-CssClass="WordWrapTask"  HeaderText="Delegated From" UniqueName="DelegatedFrom"  HeaderStyle-Width="10%">
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn DataField="DelegatedBy" FilterControlAltText="DelegatedBy" SortExpression="DelegatedBy" ItemStyle-CssClass="WordWrapTask"  HeaderText="Delegated By" UniqueName="DelegatedBy"  HeaderStyle-Width="10%">
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn DataField="DelegationComments" FilterControlAltText="DelegationComments" ItemStyle-CssClass="WordWrapTask"  SortExpression="DelegationComments" HeaderText="Delegation Comments" UniqueName="DelegationComments"  HeaderStyle-Width="22%">
                                                                </telerik:GridBoundColumn>
                                                            </Columns>
                                                        </telerik:GridTableView>
                                                    </DetailTables>
                                                    <Columns>
                                                        <telerik:GridBoundColumn DataField="ApproverName" FilterControlAltText="ApproverName" SortExpression="ApproverName" HeaderText="ApproverName" UniqueName="ApproverName">
                                                        </telerik:GridBoundColumn>
                                                        <telerik:GridTemplateColumn DataField="ApproverType" FilterControlAltText="ApproverType" SortExpression="ApproverType" HeaderText="Approver Type" UniqueName="ApproverType">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkGroupInfo" runat="server" ForeColor='<%# System.Drawing.ColorTranslator.FromHtml(Eval("ForeColor").ToString())%>' Text='<%#Eval("ApproverType") != null && Eval("ApproverType").ToString() == "Group - NPAC Approver"?"Group - NPAC/NMAC/PROTIS/ENIS Approver":Eval("ApproverType") %>'
                                                                    CommandArgument='<%#Eval("IsGroup").ToString() + "|" + Eval("ApproverType").ToString() %>' OnCommand="lnkGroupInfo_Command" />
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

                                <%-- <div id="divExecutionerHeader" runat="server" class="table-header clear">
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
                                                        <telerik:GridBoundColumn DataField="ApproverName" FilterControlAltText="ApproverName" SortExpression="ApproverName" HeaderText="ApproverName" UniqueName="ApproverName">
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



                                <div class="table-header mTop15">
                                    <h4><%= T("App.Request.MoreDetails") %></h4>
                                </div>
                                <div class="col-xs-12">
                                    <asp:LinkButton ID="lnkViewNotification" runat="server" Visible="false" OnClick="lnkViewNotification_Click"><%= T("App.Common.ViewNotification") %></asp:LinkButton>
                                    <%-- <br />
                                    <br />--%>
                                    <asp:LinkButton ID="lnkViewAttachment" runat="server" OnClick="lnkViewAttachment_Click" Visible="false"></asp:LinkButton>
                                    <br />
                                    <br />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="buttons text-right nav-rectangle">
            <asp:Button runat="server" ID="btnRefresh" CssClass="btn btn-default btn-sm" Text="Refresh" OnClick="Refresh_Click"></asp:Button>
            <asp:Button ID="WithDrawnButton" runat="server" class="btn btn-default btn-sm" Text="Withdraw" OnClick="WithDrawnButton_Click"></asp:Button>
            <asp:Button runat="server" class="btn btn-default btn-sm" Text="Cancel" OnClick="Back_Click"></asp:Button>
        </div>

        <telerik:RadWindow runat="server" ID="ApproverChain" Modal="true" Height="100%" Width="900px" Title="Approval Chain"
            RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" ReloadOnShow="true" ShowContentDuringLoad="false" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h3 class="modal-title"><%--<%= T("App.Request.ApproverDetail") %>--%> Approval Chain </h3>
                        </div>
                        <div class="modal-body">
                            <uc1:ApprverChain runat="server" ID="ApprverChain" />
                        </div>
                        <div class="modal-footer">
                            <asp:Button runat="server" OnClick="btnCancel_Click" ID="btnCancel" class="btn btn-primary bordered-button" Text="OK" />
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
                            <asp:Button runat="server" ID="btnOk" OnClick="btnOk_Click" class="btn btn-primary bordered-button" Text="OK" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>

        <telerik:RadWindow runat="server" ID="RadWinViewAttachment" Modal="true" Width="700px" Height="100%"
            RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
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

        <telerik:RadWindow ID="modalPopup" runat="server" Width="450px" Height="180px" Modal="true" OffsetElementID="main" RenderMode="Lightweight" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h3 class="modal-title" runat="server" id="H1">Withdraw Reason</h3>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal left clearfix">
                                <div class="row-fluid">
                                    <div class="col-xs-12">
                                        <div class="form-group">
                                            <asp:Label ID="Label2" AssociatedControlID="ReasonField" runat="server" Text="Reason" class="col-xs-3 control-label required"></asp:Label>
                                            <div class="col-xs-9">
                                                <asp:TextBox ID="ReasonField" TextMode="MultiLine" class="form-control" ValidationGroup="submit" Rows="3" runat="server" CausesValidation="true"></asp:TextBox>
                                                <%--<asp:Label ID="lblMessage" runat="server" Text="Reason is required." class="control-label text-danger" Style="display: none"></asp:Label>--%>
                                                <asp:RequiredFieldValidator ID="rr" Display="Dynamic" ValidationGroup="submit" runat="server" ControlToValidate="ReasonField" CssClass="text-danger" ErrorMessage="Reason is required."></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button runat="server" class="btn btn-primary transparent-button" ID="Button1" Text="Cancel" OnClick="Button1_Click" CausesValidation="false"></asp:Button>
                            <asp:Button ID="btnSubmit" CssClass="add btn btn-primary bordered-button" runat="server" CausesValidation="true" UseSubmitBehavior="true"
                                Text="Submit" OnClick="btnSubmit_Click"></asp:Button>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnSubmit" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>
    </ContentTemplate>
</asp:UpdatePanel>
