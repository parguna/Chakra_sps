<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InfoUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.Task.InfoUC" %>
<%@ Register Src="~/UsrControl/Common/CurrentAccessUC.ascx" TagPrefix="uc1" TagName="CurrentAccessUC" %>
<%@ Register Src="~/UsrControl/Common/UserInfoUC.ascx" TagPrefix="uc1" TagName="UserInfoUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<style>
    .col-xs-6 {
        word-break: break-word !important;
        -ms-word-break: break-word !important;
    }

    .clear-padding{
        padding-left:0px !important;
    }
     #divModalbody {
    min-height: 95%;
    height: 570px !important;
    overflow-y: scroll;
}
     #RadWindowWrapper_ctl00_cplMain_RequestDetailUC_InfoUC_CurrentAccess {
    top: 10% !important;
}
</style>
<script>
    function showWindow() {
        var oWnd = $find("<%= CurrentAccess.ClientID %>");
        oWnd.show();
    }

    function closeWindow() {
        var oWnd = $find("<%= CurrentAccess.ClientID %>");
        oWnd.close();
    }
</script>
<asp:UpdatePanel ID="UpdatePanel2" runat="server">
    <contenttemplate>
        <div class="col-xs-12 mBottom15">
            <div class="col-xs-6">
                <div class="form-group row" runat="server" id="divTaskID">
                    <label class="col-xs-6">Task ID:</label>
                    <div class="col-xs-6">
                        <asp:Label ID="LabelTaskID" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                 <div class="form-group row">
                    <label class="col-xs-6">Task Name:</label>
                    <div class="col-xs-6">
                        <asp:Label ID="LabelTaskName" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                 <div class="form-group row" runat="server" id="divTaskStatus" visible="false">
                    <label class="col-xs-6">Task Status:</label>
                    <div class="col-xs-6">
                        <asp:Label ID="lblTaskStatus" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                  <div class="form-group row" runat="server" id="divTaskApprovedBy" visible="false">
                    <label class="col-xs-6">Task Approved By:</label>
                    <div class="col-xs-6">
                        <asp:Label ID="lblTaskApprovedBy" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group row">
                    <label class="col-xs-6">Request For:</label>
                    <div class="col-xs-6">
                        <uc1:UserInfoUC runat="server" ID="UserInfoUC" />
                    </div>
                </div>
                <div class="form-group row">
                    <label class="col-xs-6">Request Status:</label>
                    <div class="col-xs-6">
                        <asp:Label ID="LabelStatus" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group row">
                    <label class="col-xs-6">Request Date:</label>
                    <div class="col-xs-6">
                        <asp:Label ID="LabelRequestedDate" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group row">
                    <label class="col-xs-6">Request Time:</label>
                    <div class="col-xs-6">
                        <asp:Label ID="requesttimeLabel" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group row">
                    <label class="col-xs-6">Request By:</label>
                    <div class="col-xs-6">
                        <asp:Label ID="LabelRequestByName" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                 <div class="form-group row">
                    <label class="col-xs-6">Supervisor Name:</label>
                    <div class="col-xs-6">
                        <asp:Label ID="reqSupName" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="form-group row">
                    <label class="col-xs-12">
                        <asp:LinkButton ID="linkbuttonViewCurrentAccess" runat="server" Text="View Current Access" OnClick="linkbuttonViewCurrentAccess_Click"></asp:LinkButton>
                    </label>
                </div>
            </div>
            <div class="col-xs-6">
                 <div class="form-group row">
                    <label class="col-xs-6">Position:</label>
                    <div class="col-xs-6">
                        <asp:Label ID="reqDesignation" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                 <div class="form-group row">
                    <label class="col-xs-6">Company:</label>
                    <div class="col-xs-6">
                        <asp:Label ID="reqCompany" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                 <div class="form-group row">
                    <label class="col-xs-6">Department:</label>
                    <div class="col-xs-6">
                        <asp:Label ID="reqDepartment" runat="server" class="description"></asp:Label>
                    </div>
                </div>
               
               
               <%-- <div class="form-group row">
                    <label class="col-xs-6">Division:</label>
                    <div class="col-xs-6">
                        <asp:Label ID="reqDivision" runat="server" class="description"></asp:Label>
                    </div>
                </div>--%>
                <div class="form-group row">
                    <label class="col-xs-6">Branch:</label>
                    <div class="col-xs-6">
                        <asp:Label ID="reqBranch" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group row">
                    <label class="col-xs-6">Section:</label>
                    <div class="col-xs-6">
                        <asp:Label ID="reqSection" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group row">
                    <label class="col-xs-6">Unit:</label>
                    <div class="col-xs-6">
                        <asp:Label ID="reqUnit" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group row" runat="server" id="nonEmployeeCompanyDiv" visible="false">
                    <label class="col-xs-6">Non Employee Company:</label>
                    <div class="col-xs-6">
                        <asp:Label ID="lblNonEmployeeCompany" runat="server" class="description"></asp:Label>
                    </div>
                </div>
                <div class="form-group row" runat="server" id="divWithDraw" visible="false">
                    <label class="col-xs-6">Withdraw Reason:</label>
                    <div class="col-xs-6">
                        <asp:Label ID="lblWithDrawReason" runat="server" class="description label label-danger"></asp:Label>
                    </div>
                </div>
            </div>
        </div>
        <telerik:RadWindow runat="server" ID="CurrentAccess" Modal="true" Width="700px" Height="490px" Title="Current Access"
            RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h3 class="modal-title">Current Access</h3>
                        </div>
                        <div class="modal-body" id="divModalbody">
                            <uc1:CurrentAccessUC runat="server" ID="CurrentAccessUC" IsShowIcon="false" />
                        </div>
                        <div class="modal-footer">
                            <asp:Button runat="server" ID="btnCancel" class="btn btn-primary bordered-button" Text="OK" OnClick="btnCancel_Click" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>
    </contenttemplate>
</asp:UpdatePanel>
