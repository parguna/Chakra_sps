<%@ Page Title="Attestation List" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.Admin.Attestation.List" %>

<%@ Register Src="~/Admin/Attestation/List/AttestationListUC.ascx" TagPrefix="uc1" TagName="AttestationListUC" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:AttestationListUC runat="server" ID="AttestationListUC" />
        </ContentTemplate>
    </asp:UpdatePanel> 
</asp:Content>