<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WizardUC.ascx.cs" Inherits="DI.Chakra.Web.EndPoints.EndpointSystem.Wizard.WizardUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register Src="~/Admin/Endpoint/Common/EndpointInfoUC.ascx" TagPrefix="uc1" TagName="EndpointInfoUC" %>
<%@ register src="~/Admin/Endpoint/Common/EndpointConfigurationUC.ascx" tagprefix="uc1" tagname="EndpointConfigurationUC" %>
<%@ Register Src="~/Admin/Endpoint/Common/MappingDetailUC.ascx" TagPrefix="uc1" TagName="MappingDetailUC" %>

<style>
    .RadWizard {
        max-width: 99% !important;
    }

    ul.rwzBreadCrumb li:not(:last-child) {
        width: 100px !important;
    }
</style>

<telerik:RadWizard ID="Wizard" OnNavigationBarButtonClick="Navigation" OnNextButtonClick="Navigation" DisplayCancelButton="true" OnCancelButtonClick="Wizard_CancelButtonClick" DisplayProgressBar="false" runat="server" OnFinishButtonClick="Wizard_FinishButtonClick" OnClientButtonClicking="OnClientButtonClicking">
    <WizardSteps>
        <telerik:RadWizardStep ID="RadWizardStep1" runat="server"  Title="Endpoint Information" SpriteCssClass="Step_One">
            <uc1:EndpointInfoUC runat="server" ID="EndpointInfoUC" />
        </telerik:RadWizardStep>
        <telerik:RadWizardStep ID="RadWizardStep2" runat="server" Title="Credentials" SpriteCssClass="Step_Two">
            <uc1:endpointconfigurationuc runat="server" id="EndpointConfigurationUC" />
        </telerik:RadWizardStep>
        <telerik:RadWizardStep ID="RadWizardStep3" runat="server" Title="Attribute Mapping" SpriteCssClass="Step_Three">
            <uc1:MappingDetailUC runat="server" ID="MappingDetailUC" />
        </telerik:RadWizardStep>
    </WizardSteps>
</telerik:RadWizard>