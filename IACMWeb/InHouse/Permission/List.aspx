<%@ Page Title="InHouse Permission" MasterPageFile="~/Master/ProtoMast4.master" Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.InHouse.Permission.List" %>

<%@ Register Src="~/InHouse/Permission/UC/List.ascx" TagPrefix="uc1" TagName="List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:List runat="server" ID="PermissionList" />

            <div class="buttons text-right nav-rectangle">
                <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClientClick="Redirect()"></asp:Button>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
