<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="DI.Chakra.Web.ACL.Page.Detail" %>

<%@ Register Src="~/ACL/Page/UC/Detail.ascx" TagPrefix="uc1" TagName="Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:Detail runat="server" id="Detail1" UIMode="View"/>
</asp:Content>
