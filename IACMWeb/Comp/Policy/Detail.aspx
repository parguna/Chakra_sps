<%@ Page Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="IACMWeb.Comp.Policy.Detail" %>

<%@ Register Src="~/Comp/Policy/Wizard/CompliancePolicyWizardUC.ascx" TagPrefix="uc1" TagName="CompliancePolicyWizardUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:CompliancePolicyWizardUC runat="server" ID="CompliancePolicyWizard"  UIMode="View" />
</asp:Content>