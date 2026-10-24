<%@ Page Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="DI.Chakra.Web.Job.Compliance.Detail" %>

<%@ Register Src="~/Job/Compliance/Wizard/ComplianceWizardUC.ascx" TagPrefix="uc1" TagName="ComplianceWizardUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:ComplianceWizardUC runat="server" ID="ComplianceWizardUC" />
</asp:Content>