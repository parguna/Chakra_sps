<%@ Page Title="Trigger Detail" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" Inherits="DI.Chakra.Web.Triggers.Detail" CodeBehind="Detail.aspx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/Triggers/UC/ViewTriggerUC.ascx" TagPrefix="uc1" TagName="ViewTriggerUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:ViewTriggerUC runat="server" ID="ViewTriggerControl"></uc1:ViewTriggerUC>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

