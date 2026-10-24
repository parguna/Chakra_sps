<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" EnableEventValidation="false" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="DI.Chakra.Web.Job.Discovery.Edit" %>

<%@ Register Src="~/Job/Discovery/Wizard/DiscoveryWizardUC.ascx" TagPrefix="uc1" TagName="DiscoveryWizardUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">

            <uc1:DiscoveryWizardUC runat="server" ID="DiscoveryWizardUC" />

</asp:Content>