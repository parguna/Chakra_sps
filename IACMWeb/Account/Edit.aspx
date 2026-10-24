<%@ Page Title="Edit Account" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="DI.Chakra.Web.Account.Edit" %>

<%@ Register Src="~/Account/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>

<asp:Content ID="Content3" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:DetailUC runat="server" id="DetailUC" UIMode="Edit" />
</asp:Content>