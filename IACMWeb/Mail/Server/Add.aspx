<%@ Page Title="Email Server" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="DI.Chakra.Web.Mail.Server.Add" %>
<%@ Register Src="~/Mail/Server/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:DetailUC runat="server" ID="DetailUC" UIMode="Add"/>
</asp:Content>
