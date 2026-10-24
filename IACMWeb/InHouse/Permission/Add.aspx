<%@ Page Language="C#" MasterPageFile="~/Master/ProtoMast4.master" Title="Inhouse Permission Add" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="DI.Chakra.Web.InHouse.Permission.Add" %>

<%@ Register Src="~/InHouse/Permission/UC/Detail.ascx" TagPrefix="uc1" TagName="Detail" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <uc1:Detail runat="server" ID="Detail" UIMode="Add" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
