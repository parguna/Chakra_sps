<%@ Page Language="C#" Title="My Tasks" AutoEventWireup="true" Inherits="DI.Chakra.Web.Task.List" MasterPageFile="~/Master/ProtoMast4.master" CodeBehind="List.aspx.cs" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/UsrControl/Common/Task/TaskListUC.ascx" TagPrefix="uc1" TagName="TaskListUC" %>


<asp:Content ID="Content3" ContentPlaceHolderID="cplMain" runat="server">
    <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left">
                <div class="row-fluid">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <uc1:TaskListUC runat="server" ID="TaskListUC" ShowGridFilter="false" IsAdminMode="No" />

                            <div class="buttons text-right nav-rectangle">
                                <asp:Button runat="server" ID="RefreshButton" CssClass="btn btn-default btn-sm" Text="Refresh" OnClick="RefreshButton_Click"></asp:Button>
                                <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClientClick="Redirect()"></asp:Button>
                            </div>

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>