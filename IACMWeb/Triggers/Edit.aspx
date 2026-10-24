<%@ Page Title="Edit Trigger" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" Inherits="DI.Chakra.Web.Triggers.Edit" CodeBehind="Edit.aspx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/Triggers/UC/EditTriggerUC.ascx" TagPrefix="uc1" TagName="EditTriggerUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:EditTriggerUC runat="server" ID="EditTriggerControl"></uc1:EditTriggerUC>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

