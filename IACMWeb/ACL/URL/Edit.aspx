<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" CodeBehind="Edit.aspx.cs" Inherits="DI.Chakra.Web.ACL.URL.Edit" %>
<%@ Register Src="~/ACL/URL/UC/Detail.ascx" TagPrefix="uc1" TagName="Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:Detail runat="server" id="Detail" UIMode="Edit" />
</asp:Content>