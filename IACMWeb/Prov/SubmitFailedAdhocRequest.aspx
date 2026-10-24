<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="SubmitFailedAdhocRequest.aspx.cs" Inherits="DI.Chakra.Web.Prov.SubmitFailedAdhocRequest" %>

<%@ Register Src="~/Prov/UC/SubmitRequest.ascx" TagPrefix="uc1" TagName="SubmitRequest" %>



<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:SubmitRequest runat="server" id="SubmitRequest" IncludeAdhocAccessRequest="true" />

</asp:Content>
