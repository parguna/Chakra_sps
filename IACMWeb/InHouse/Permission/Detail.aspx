<%@ Page MasterPageFile="~/Master/ProtoMast4.master" Title="Inhouse Permission Detail" Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="DI.Chakra.Web.InHouse.Permission.Detail" %>
<%@ Register Src="~/InHouse/Permission/UC/Detail.ascx" TagPrefix="uc1" TagName="Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:Detail runat="server" id="PermissionDetail" UIMode="View" />
</asp:Content>
