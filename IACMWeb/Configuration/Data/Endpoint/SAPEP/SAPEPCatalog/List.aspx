<%@ Page Title="SAPEP Catalog" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.Configuration.Data.Endpoint.SAPEP.SAPEPCatalog.List" %>

<%@ Register Src="~/Configuration/Data/Endpoint/SAPEP/SAPEPCatalog/UC/ListUC.ascx" TagPrefix="ListUC" TagName="ListUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <ListUC:ListUC ID="ListUC" runat="server" />

            <div class="buttons text-right nav-rectangle">
                <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClientClick="Redirect()"></asp:Button>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>