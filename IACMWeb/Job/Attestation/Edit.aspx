<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="DI.Chakra.Web.Job.Attestation.Edit" %>

<%@ Register Src="~/Job/Attestation/Wizard/SchedulerWizardUC.ascx" TagPrefix="uc1" TagName="SchedulerWizardUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:SchedulerWizardUC runat="server" ID="SchedulerWizardUC" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>