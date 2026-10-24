<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ComplianceWizardUC.ascx.cs" Inherits="DI.Chakra.Web.Job.Compliance.Wizard.ComplianceWizardUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register Src="~/UsrControl/Scheduler/SchedulerUC.ascx" TagPrefix="uc1" TagName="SchedulerUC" %>
<%@ Register Src="~/Job/Compliance/Common/ComplianceinfoUC.ascx" TagPrefix="uc1" TagName="ComplianceinfoUC" %>



<telerik:RadWizard ID="ComplianceRadWizard" runat="server" OnFinishButtonClick="ComplianceRadWizard_FinishButtonClick" DisplayCancelButton="true"
     OnCancelButtonClick="ComplianceRadWizard_CancelButtonClick" OnClientButtonClicking="OnClientButtonClicking" DisplayProgressBar="false">
    <WizardSteps>
        <telerik:RadWizardStep ID="RadWizardStep1" runat="server" Title="Select Objects" SpriteCssClass="Step_One" ValidationGroup="personalInfo">

            <uc1:ComplianceinfoUC runat="server" ID="ComplianceinfoUC" />

        </telerik:RadWizardStep>
        <telerik:RadWizardStep ID="RadWizardStep2" runat="server" Title="Scheduler" SpriteCssClass="Step_Two">

            <uc1:SchedulerUC runat="server" ID="SchedulerUC" />
        </telerik:RadWizardStep>
    </WizardSteps>
</telerik:RadWizard>