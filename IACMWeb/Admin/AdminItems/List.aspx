<%@ Page Title="Admin Mode" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.Admin.List" %>

<%@ Register Src="~/Admin/AdminItems/UC/ItemsUC.ascx" TagPrefix="uc1" TagName="ItemsUC" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:ItemsUC runat="server" ID="ItemsUC1" />

            <div class="buttons text-right nav-rectangle">
                <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClientClick="Redirect()"></asp:Button>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>