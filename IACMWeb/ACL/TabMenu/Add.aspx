<%@ Page Title="TabMenu" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="DI.Chakra.Web.ACL.TabMenu.Add" %>

<%@ Register Src="~/ACL/TabMenu/UC/Detail.ascx" TagPrefix="uc1" TagName="Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:Detail runat="server" id="Detail" UIMode="Add" />
</asp:Content>
