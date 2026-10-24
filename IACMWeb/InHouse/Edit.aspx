<%@ Page Title="InHouse User Edit" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="DI.Chakra.Web.InHouse.Edit" %>

<%@ Register Src="~/InHouse/UC/Detail.ascx" TagPrefix="DetailUC" TagName="DetailUCControl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <DetailUC:DetailUCControl runat="server" ID="DetailUC" UIMode="Edit" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
