<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Master/ProtoMast4.master"  CodeBehind="Detail.aspx.cs" Inherits="DI.Chakra.Web.Admin.Attestation.Detail" %>

<%@ Register Src="~/Admin/Attestation/Wizard/AttestationWizardUC.ascx" TagPrefix="uc1" TagName="AttestationWizardUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlADS" runat="server" Visible="true">
                <uc1:AttestationWizardUC runat="server" id="AttestationWizardUC" UIMode="View" />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

