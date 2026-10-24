<%@ Page Title="Trigger List" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.Triggers.List" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Triggers/UC/ListUC.ascx" TagPrefix="uc" TagName="TriggerUC" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cplMain" runat="server">
   <uc:TriggerUC runat="server" ID="ucTrigger"></uc:TriggerUC>
</asp:Content>
