<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" MasterPageFile="~/Master/ProtoMast4.master" Inherits="DI.Chakra.Web.AppCustomization.Dashboard.DefaultRoleDB.List" %>

<%@ Register Src="~/AppCustomization/Dashboard/DefaultRoleDB/UC/List.ascx" TagPrefix="uc1" TagName="Lst" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:Lst runat="server" ID="List" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
