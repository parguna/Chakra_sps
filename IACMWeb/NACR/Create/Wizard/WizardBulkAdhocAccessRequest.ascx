<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WizardBulkAdhocAccessRequest.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.Wizard.WizardBulkAdhocAccessRequest" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/NACR/Create/BulkRequest/UC/BulkAdhocAccessRequestUC.ascx" TagPrefix="uc1" TagName="BulkAdhocAccessRequestUC" %>
<%@ Register Src="~/NACR/Create/BulkRequest/BulkAccessUC.ascx" TagPrefix="uc1" TagName="BulkAccessUC" %>

<style>
    .RadWizard {
        max-width: 99% !important;
    }

    ul.rwzBreadCrumb li:nth-child(-n+1) {
        width: 100px !important;
    }
</style>

<div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <asp:Panel runat="server">
                <telerik:RadWizard ID="RadWizard" runat="server" Localization-Finish="Submit" NavigationButtonsPosition="Top" OnClientButtonClicking="OnClientButtonClicking" DisplayCancelButton="true" DisplayProgressBar="false" OnCancelButtonClick="RadWizard_CancelButtonClick" OnFinishButtonClick="RadWizard_FinishButtonClick" OnPreviousButtonClick="ApplicationBind" OnNavigationBarButtonClick="ApplicationBind" OnNextButtonClick="ApplicationBind" ClientIDMode="Static">
                    <WizardSteps>
                        <telerik:RadWizardStep ID="RadWizardStep1" runat="server" Title="Request" SpriteCssClass="Step_One">
                            <uc1:BulkAdhocAccessRequestUC runat="server" ID="BulkAdhocAccessRequestUC" />
                        </telerik:RadWizardStep>
                        <telerik:RadWizardStep ID="RadWizardStep2" runat="server" Title="Application" SpriteCssClass="Step_Two">
                            <uc1:BulkAccessUC runat="server" ID="BulkAccessUC" />
                        </telerik:RadWizardStep>
                    </WizardSteps>
                </telerik:RadWizard>
            </asp:Panel>

        </ContentTemplate>
      <%--  <Triggers>
            <asp:AsyncPostBackTrigger ControlID="RadWizard"/>
        </Triggers>--%>
    </asp:UpdatePanel>
</div>
