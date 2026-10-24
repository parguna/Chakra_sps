<%@ Page Title="Add Trigger" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" Inherits="DI.Chakra.Web.Triggers.Add" CodeBehind="Add.aspx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/Triggers/UC/AddTriggerUC.ascx" TagPrefix="uc1" TagName="AddTriggerUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:AddTriggerUC runat="server" ID="AddTriggerControl"></uc1:AddTriggerUC>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>