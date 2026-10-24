<%@ Page Title="Create Attestation Process" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="DI.Chakra.Web.Admin.Attestation.Add" %>

<%@ Register Src="~/Admin/Attestation/Wizard/AttestationWizardUC.ascx" TagPrefix="uc1" TagName="AttestationWizardUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlADS" runat="server" Visible="true">
                <uc1:AttestationWizardUC runat="server" ID="AttestationWizardUC" UIMode="Add" />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>