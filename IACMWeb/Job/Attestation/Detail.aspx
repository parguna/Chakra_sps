<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="DI.Chakra.Web.Job.Attestation.Detail" %>
<%@ Register Src="~/Job/Attestation/Wizard/SchedulerWizardUC.ascx" TagPrefix="uc1" TagName="SchedulerWizardUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
      <uc1:SchedulerWizardUC runat="server" ID="SchedulerWizardUC" />
</asp:Content>
