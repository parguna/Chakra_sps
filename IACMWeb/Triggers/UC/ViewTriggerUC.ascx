<%@ Control Language="C#" AutoEventWireup="true" Inherits="DI.Chakra.Web.Triggers.UC.ViewTriggerUC" CodeBehind="ViewTriggerUC.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/Triggers/UC/ViewUserDetailUC.ascx" TagPrefix="uc1" TagName="UserDetailUC" %>

<script>
    function HideModalPopup() {
        $find("<%= UserDetailWindow.ClientID %>").close();
        window.setTimeout(function () {
            $("#UpdateProgressBar").hide();
        }, 1000);
        return false;
    }
</script>

<style>
    .RadWindow_NewAndReply
{
    width:80% !important;
    left:15% !important;
}
</style>

<div class="content-wrapper">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <br />

                <div class="col-xs-6">
                    <div class="form-group">
                        <label class="col-xs-5">User Name</label>
                        <div class="col-xs-7">
                            <asp:Label ID="UserNameLabel" runat="server" CssClass="form-control"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-5">Event Type</label>
                        <div class="col-xs-7">
                            <asp:Label ID="EventTypeLabel" runat="server" CssClass="form-control"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-5">Effective Date</label>
                        <div class="col-xs-7">
                            <asp:Label ID="EffectiveDateLabel" runat="server" CssClass="form-control">
                            </asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-5">Created Date</label>
                        <div class="col-xs-7">
                            <asp:Label ID="CreatedDateLabel" runat="server" CssClass="form-control"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-5">Created By</label>
                        <div class="col-xs-7">
                            <asp:Label ID="CreatedByLabel" runat="server" CssClass="form-control"></asp:Label>
                        </div>
                    </div>
                </div>

                <div class="col-xs-6">
                    <div class="form-group">
                        <label class="col-xs-5">Request Created</label>
                        <div class="col-xs-7">
                            <asp:Label ID="RequestCreatedLabel" runat="server" CssClass="form-control"> </asp:Label>
                        </div>
                    </div>
                   <%-- <div class="form-group">
                        <label class="col-xs-5">Rejoining User</label>
                        <div class="col-xs-7">
                            <asp:Label ID="RejoiningUserLabel" runat="server" CssClass="form-control"></asp:Label>
                        </div>
                    </div>--%>
                    <div class="form-group">
                        <label class="col-xs-5">Admin Check Required</label>
                        <div class="col-xs-7">
                            <asp:Label ID="AdminCheckRequiredLabel" runat="server" CssClass="form-control"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-5 control-label">Original Effective Date</label>
                        <div class="col-xs-7">
                             <asp:Label ID="orgEffectiveDate" runat="server" CssClass="form-control"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-5">Active</label>
                        <div class="col-xs-7">
                            <asp:Label ID="ActiveLabel" runat="server" CssClass="form-control"></asp:Label>
                        </div>
                    </div>
                </div>

                <br />
                <br />

                <div class="col-xs-12">
                    <div class="form-group">
                        <div class="col-xs-8">
                        </div>
                        <div class="buttons text-right nav-rectangle col-xs-4">
                            <asp:Button ID="ShowUserDetailButton" runat="server" CssClass="btn btn-default btn-sm" Text="Show Snapshot Detail" OnClick="ShowUserDetailButton_Click" />
                            <asp:Button ID="Button2" runat="server" CssClass="btn btn-default btn-sm cancel" Text="Cancel" PostBackUrl="~/Triggers/List.aspx" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<telerik:RadWindow runat="server" ID="UserDetailWindow"  CssClass="RadWindow_NewAndReply" Modal="true"  MinWidth="80%"  Title="User Detail"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <uc1:UserDetailUC ID="UserDetailControl" OnOnDetailCancel="UserDetailControl_OnDetailCancel" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>

