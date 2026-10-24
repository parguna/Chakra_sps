<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="DI.Chakra.Web.Admin.Endpoint.Detail" %>

<%@ Register Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ register src="~/Admin/Endpoint/Wizard/WizardUC.ascx" tagprefix="uc1" tagname="WizardUC" %>

<asp:content id="Content1" contentplaceholderid="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:wizarduc runat="server" id="WizardUC" UIMode="View" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:content>
