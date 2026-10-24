<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Master/ProtoMast4.master" CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.AppCustomization.QuickLink.QLAccessRights.List" %>

<%@ Register Src="~/AppCustomization/QuickLink/QLAccessRights/UC/List.ascx" TagPrefix="uc1" TagName="Lst" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:Lst runat="server" ID="List" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
