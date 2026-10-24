<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.DepartmentRequest.UC.DetailUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register Src="~/UsrControl/Common/CommonTaskInfoUC.ascx" TagPrefix="uc1" TagName="CommonTaskInfoUC" %>
<%@ Register Src="~/UsrControl/Common/ApprverChain.ascx" TagPrefix="uc1" TagName="ApprverChain" %>
<%@ Register Src="~/UsrControl/Common/Task/InfoUC.ascx" TagPrefix="uc2" TagName="InfoUC" %>
<%@ Register Src="~/UsrControl/Common/ViewNotification.ascx" TagPrefix="uc1" TagName="ViewNotificationUC" %>
<%@ Register Src="~/UsrControl/Common/ViewAttachmentUC.ascx" TagPrefix="uc1" TagName="ViewAttachmentUC" %>

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
        z-index: 99999987;
        opacity: 0.1;
        top: 0;
    }

    .RadWindow_IACMSkin {
        z-index: 99999988 !important;
    }

    .navbar {
        z-index: 99999988 !important;
    }
</style>
<script>
    function ShowApproverChain() {
        $find("<%= ApproverChain.ClientID %>").show();
        return false;
    }

    function ShowModalPopup() {
        $find("<%= modalPopup.ClientID %>").show();
        return false;
    }

    function HideModalPopup() {
        document.getElementById('<%= ReasonField.ClientID %>').value = "";
        $find("<%= modalPopup.ClientID %>").close();
        return false;
    }
    $(document).ready(function () {
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
                                <uc1:CommonTaskInfoUC runat="server" ID="CommonTaskInfoUC" canLoadAccessReqItems="true" />
                                <div runat="server" id="exceptionDetails" visible="false">
                                    <div class="table-header mTop15">
                                        <h4>Exception Details</h4>
                                    </div>
                                    <div class="col-xs-12">
                                        <asp:Label ID="lblExceptionDetails" runat="server" ></asp:Label>
                                    </div>
                                    <br />
                                </div>
                                <div class="table-header mTop15">
                                    <h4><%= T("App.Request.Approval") %></h4>
                                </div>
                                <div class="col-xs-12">
                                    <div class="row" style="margin-bottom: 10px;">
                                        <div class="col-xs-12">
                                            <telerik:RadGrid ID="grdApprover" runat="server" AllowSorting="false" AllowPaging="true"
                                                CellSpacing="0" GridLines="None">
                                                <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" />

                                                <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="false">
                                                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                                                    <Columns>
                                                        <telerik:GridBoundColumn DataField="ApproverName" FilterControlAltText="ApproverName" SortExpression="ApproverName" HeaderText="Approver Name" UniqueName="ApproverName" HeaderStyle-Width="40%">
                                                        </telerik:GridBoundColumn>

                                                        <telerik:GridTemplateColumn HeaderText="Authorisation Type" SortExpression="ApproverType" UniqueName="ApproverType" HeaderStyle-Width="10%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblApprover" runat="server" Text='<%#Eval("ApproverType") != null && Eval("ApproverType").ToString() == "Group - NPAC Approver"?"Group - NPAC/NMAC/PROTIS/ENIS Approver":Eval("ApproverType") %>' />
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridBoundColumn DataField="Action" FilterControlAltText="Status" SortExpression="Status" HeaderText="Status" UniqueName="Status" HeaderStyle-Width="5%">
                                                        </telerik:GridBoundColumn>
                                                        <telerik:GridBoundColumn DataField="ActionDate" FilterControlAltText="ActionDate" SortExpression="ActionDate" HeaderText="Action Date" UniqueName="ActionDate" HeaderStyle-Width="10%">
                                                        </telerik:GridBoundColumn>
                                                        <telerik:GridBoundColumn DataField="Comments" FilterControlAltText="Comments" SortExpression="Comments" HeaderText="Comments" UniqueName="Comments" HeaderStyle-Width="35%">
                                                        </telerik:GridBoundColumn>
                                                    </Columns>
                                                </MasterTableView>
                                            </telerik:RadGrid>

                                            <telerik:RadGrid ID="grdApproverList" runat="server" AllowSorting="false" AllowPaging="true"
                                                CellSpacing="0" GridLines="None">
                                                <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" />

                                                <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="false">
                                                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                                                    <Columns>
                                                        <telerik:GridBoundColumn DataField="ApproverName" FilterControlAltText="ApproverName" SortExpression="ApproverName" HeaderText="Approver Name" UniqueName="ApproverName" HeaderStyle-Width="18%">
                                                        </telerik:GridBoundColumn>

                                                        <telerik:GridBoundColumn DataField="ApproverRole" FilterControlAltText="ApproverRole" SortExpression="ApproverRole" HeaderText="Authorisation Type" UniqueName="ApproverRole" HeaderStyle-Width="27%">
                                                        </telerik:GridBoundColumn>
                                                        <telerik:GridBoundColumn DataField="Level" FilterControlAltText="Level" SortExpression="Level" HeaderText="Level" UniqueName="Level" HeaderStyle-Width="5%">
                                                        </telerik:GridBoundColumn>
                                                        <telerik:GridBoundColumn DataField="Status" FilterControlAltText="Status" SortExpression="Status" HeaderText="Status" UniqueName="Status" HeaderStyle-Width="10%">
                                                        </telerik:GridBoundColumn>
                                                        <telerik:GridBoundColumn DataField="ApprovedOn" FilterControlAltText="ApprovedOn" SortExpression="ApprovedOn" HeaderText="On" UniqueName="ApprovedOn" HeaderStyle-Width="18%" DataFormatString="{0:dd-MM-yyyy hh:mm tt}">
                                                        </telerik:GridBoundColumn>
                                                        <telerik:GridBoundColumn DataField="Remarks" FilterControlAltText="Remarks" SortExpression="Remarks" HeaderText="Remarks" UniqueName="Remarks" HeaderStyle-Width="22%">
                                                        </telerik:GridBoundColumn>
                                                    </Columns>
                                                </MasterTableView>
                                            </telerik:RadGrid>
                                        </div>
                                    </div>
                                </div> 

                                <div class="table-header mTop15">
                                    <h4><%= T("App.Request.MoreDetails") %></h4>
                                </div>
                                <div class="col-xs-12">
                                    <%--<asp:LinkButton runat="server" OnClientClick="return ShowApproverChain();"><%= T("App.Request.ViewApprovalChain") %></asp:LinkButton>
                                    <br />
                                    <br />--%>
                                    <asp:LinkButton ID="lnkViewNotification" Visible="false" runat="server" OnClick="lnkViewNotification_Click"><%= T("App.Common.ViewNotification") %></asp:LinkButton>
                                    <%--<br />
                                    <br />--%>
                                    <asp:LinkButton ID="lnkViewAttachment" runat="server" OnClick="lnkViewAttachment_Click"  visible ="false"></asp:LinkButton>
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
            <asp:Button ID="WithDrawnButton" Visible="false" runat="server" class="btn btn-default btn-sm" Text="Withdraw" OnClientClick="return ShowModalPopup();"></asp:Button>
            <asp:Button runat="server" class="btn btn-default btn-sm" Text="Cancel" OnClick="Back_Click"></asp:Button>
        </div>

        <telerik:RadWindow runat="server" ID="ApproverChain" Modal="true" Height="100%" Width="850px" Title="Approval Chain"
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
                            <asp:Button runat="server" ID="btnCancel" class="btn btn-primary bordered-button" Text="OK" />
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

        <telerik:RadWindow ID="modalPopup" runat="server" Width="450px" Height="180px" Modal="true" OffsetElementID="main" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title" runat="server" id="H1">Withdraw Reason</h3>
                </div>
                <div class="modal-body">
                    <div class="form-horizontal left clearfix">
                        <div class="row-fluid">
                            <div class="col-xs-12">
                                <div class="form-group">
                                    <asp:Label ID="Label2" AssociatedControlID="ReasonField" runat="server" Text="Reason" class="col-xs-3 control-label"></asp:Label>
                                    <div class="col-xs-9">
                                        <asp:TextBox ID="ReasonField" TextMode="MultiLine" class="form-control" Rows="3" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" class="btn btn-primary transparent-button" ID="Button1" Text="Cancel" data-dismiss="modal" OnClientClick="return HideModalPopup();"></asp:Button>
                    <asp:Button ID="btnSubmit" CssClass="add btn btn-primary bordered-button" runat="server"
                        Text="Submit" OnClick="btnSubmit_Click"></asp:Button>
                </div>
            </ContentTemplate>
        </telerik:RadWindow>
    </ContentTemplate>
</asp:UpdatePanel>
