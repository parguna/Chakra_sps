<%@ Page Title="Email Server List" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.Mail.Server.List" %>

<%@ Register Src="~/Mail/Server/UC/ListUC.ascx" TagPrefix="uc1" TagName="ListUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:ListUC runat="server" ID="ListUC" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
