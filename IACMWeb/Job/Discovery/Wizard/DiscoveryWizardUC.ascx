<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DiscoveryWizardUC.ascx.cs" Inherits="DI.Chakra.Web.Job.Discovery.Wizard.DiscoveryWizardUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register Src="~/UsrControl/Scheduler/SchedulerUC.ascx" TagPrefix="uc1" TagName="SchedulerUC" %>
<%@ Register Src="~/Job/Discovery/Common/DiscoveryInfoUC.ascx" TagPrefix="uc1" TagName="DiscoveryInfoUC" %>

<telerik:RadWizard ID="DiscoveryRadWizard" runat="server" OnFinishButtonClick="DiscoveryRadWizard_FinishButtonClick" DisplayCancelButton="true"
    OnCancelButtonClick="DiscoveryRadWizard_CancelButtonClick" OnClientButtonClicking="OnClientButtonClicking" DisplayProgressBar="false">
    <WizardSteps>
        <telerik:RadWizardStep ID="RadWizardStep1" runat="server" Title="Process" SpriteCssClass="Step_One" ValidationGroup="personalInfo">

            <uc1:DiscoveryInfoUC runat="server" ID="DiscoveryInfoUC" />

        </telerik:RadWizardStep>
        <telerik:RadWizardStep ID="RadWizardStep2" runat="server" Title="Scheduler" SpriteCssClass="Step_Two">

            <uc1:SchedulerUC runat="server" ID="SchedulerUC" />
        </telerik:RadWizardStep>
    </WizardSteps>
</telerik:RadWizard>