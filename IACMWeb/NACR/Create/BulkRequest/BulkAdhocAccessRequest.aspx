<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="BulkAdhocAccessRequest.aspx.cs" Inherits="DI.Chakra.Web.NACR.Create.BulkRequest.BulkAdhocAccessRequest" %>

<%--<%@ Register Src="~/NACR/Create/BulkRequest/UC/BulkAdhocAccessRequestUC.ascx" TagPrefix="uc1" TagName="BulkAdhocAccessRequestUC" %>--%>
<%@ Register Src="~/NACR/Create/Wizard/WizardBulkAdhocAccessRequest.ascx" TagPrefix="uc1" TagName="WizardBulkAdhocAccessRequest" %>






<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:WizardBulkAdhocAccessRequest runat="server" id="WizardBulkAdhocAccessRequest" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
