<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="DI.Chakra.Web.Job.Discovery.Add" %>

<%@ Register Src="~/Job/Discovery/Wizard/DiscoveryWizardUC.ascx" TagPrefix="uc1" TagName="DiscoveryWizardUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:DiscoveryWizardUC runat="server" ID="DiscoveryWizardUC" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>