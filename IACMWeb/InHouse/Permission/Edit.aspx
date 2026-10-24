<%@ Page Language="C#" AutoEventWireup="true" Title="Inhouse Permission Edit" MasterPageFile="~/Master/ProtoMast4.master" CodeBehind="Edit.aspx.cs" Inherits="DI.Chakra.Web.InHouse.Permission.Edit" %>
<%@ Register Src="~/InHouse/Permission/UC/Detail.ascx" TagPrefix="uc1" TagName="Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:Detail runat="server" id="Detail" UIMode="Edit"/>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
