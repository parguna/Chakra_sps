<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="DI.Chakra.Web.Job.Attestation.Add" %>

<%@ Register Src="~/Job/Attestation/Wizard/SchedulerWizardUC.ascx" TagPrefix="uc1" TagName="SchedulerWizardUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:SchedulerWizardUC runat="server" id="SchedulerWizardUC" />
</asp:Content>
