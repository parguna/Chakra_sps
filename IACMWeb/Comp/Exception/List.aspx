<%@ Page Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.Comp.Exception.List" %>

<%@ Register Src="~/Comp/Exception/UC/ListExceptionUC.ascx" TagPrefix="Ucl" TagName="Exception" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <Ucl:Exception ID="lstExceptionGrid" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
