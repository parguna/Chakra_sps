<%@ Page Title="" MasterPageFile="~/Master/ProtoMast4.master" Language="C#" AutoEventWireup="true" CodeBehind="PagePermission.aspx.cs" Inherits="DI.Chakra.Web.ACL.Page.PagePermission" %>
<%@ Register Src="~/ACL/Page/UC/PagePermission.ascx" TagPrefix="uc1" TagName="PagePermission" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:PagePermission runat="server" id="PagePermission" />
</asp:Content>



