<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="DI.Chakra.Web.Prov.Request.Detail" %>

<%@ Register Src="~/Request/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:DetailUC runat="server" ID="RequestDetailUC" IsAdminMode="Yes" RedirectURL="~/Prov/Request/List.aspx" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>