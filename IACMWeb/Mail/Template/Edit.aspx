<%@ Page Title="Email Template" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="DI.Chakra.Web.Mail.Template.Edit" %>

<%@ Register Src="~/Mail/Template/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:DetailUC runat="server" ID="DetailUC" UIMode="Edit"/>
</asp:Content>
