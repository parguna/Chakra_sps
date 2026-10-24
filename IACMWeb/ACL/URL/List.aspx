<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.ACL.URL.List" %>

<%@ Register Src="~/ACL/URL/UC/List.ascx" TagPrefix="uc1" TagName="List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:List runat="server" ID="list" />

            <div class="buttons text-right nav-rectangle">
                <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClientClick="Redirect()"></asp:Button>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>