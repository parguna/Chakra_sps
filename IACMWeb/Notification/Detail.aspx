<%@ Page Title="Notification Detail" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="DI.Chakra.Web.Notification.Detail" %>

<%@ Register Src="~/Notification/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:DetailUC runat="server" id="DetailUC" />
</asp:Content>
