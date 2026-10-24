<%@ Page Title="Tasks" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.Prov.Task.List" %>
<%@ Register Src="~/UsrControl/Common/Task/TaskListUC.ascx" TagPrefix="uc1" TagName="TaskListUC" %>
<asp:Content ID="Content3" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <uc1:TaskListUC runat="server" ID="TaskListUC" IsAdminMode="Yes" />
            

    <div class="buttons text-right nav-rectangle">
        <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClientClick="Redirect()"></asp:Button>
    </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>