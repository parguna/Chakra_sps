<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="SubmitFailedRequests.aspx.cs" Inherits="DI.Chakra.Web.Prov.SubmitFailedRequests" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxCtrl" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Src="~/Prov/UC/SubmitRequest.ascx" TagPrefix="uc1" TagName="SubmitRequest" %>



<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">

    <uc1:SubmitRequest runat="server" id="SubmitRequest" IncludeAdhocAccessRequest="false" />


</asp:Content>
