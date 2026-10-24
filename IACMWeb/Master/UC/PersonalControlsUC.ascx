<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonalControlsUC.ascx.cs" Inherits="DI.Chakra.Web.Master.UC.PersonalControlsUC" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<style>
    .hiddencol {
        display: none;
    }
</style>

<telerik:RadWindow runat="server" ID="DashboardList" Modal="true" Width="500px" Height="100%" Title="Dashboard List"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title" id="H1">Dashboard List</h3>
                </div>
                <div class="modal-body">
                    <asp:ListBox runat="server" CssClass="DIListBox" ID="lbDashboard" Height="250px" Width="100%"></asp:ListBox>
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" CssClass="btn btn-primary transparent-button" data-dismiss="modal" ID="btnCancel" Text="Cancel" OnClick="btnCancel_Click" />
                    <asp:Button runat="server" CssClass="btn btn-primary bordered-button validation" ID="btnAdd" Text="Add" OnClick="btnSubmit_Click" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>

<a id="lnkConfig" class="grow" runat="server">
    <%--    <h5 class="quicklink-title">Home Control</h5>--%>
    <h5 class="quicklink-title" runat="server" id="DashboardTitle"></h5>
    <%--  <span class="glyphicon glyphicon-plus red"></span>  --%>
    <span id="DashboardId" runat="server" class="hiddencol"></span>
    <span id="Position" runat="server" class="hiddencol"></span>
    <span id="DashboardType" runat="server" class="hiddencol"></span>
</a>