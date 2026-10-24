<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SchedulerWizardUC.ascx.cs" Inherits="DI.Chakra.Web.Job.Attestation.Wizard.SchedulerWizardUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Job/Attestation/Common/JobInfo.ascx" TagPrefix="uc1" TagName="JobInfo" %>
<%@ Register Src="~/UsrControl/Scheduler/SchedulerUC.ascx" TagPrefix="uc1" TagName="SchedulerUC" %>

<telerik:RadWizard ID="AttSchRadWizard" runat="server" OnFinishButtonClick="AttSchRadWizard_OnFinishButtonClick" DisplayCancelButton="true"
    OnCancelButtonClick="SchedulerRadWizard_CancelButtonClick" OnClientButtonClicking="OnClientButtonClicking" DisplayProgressBar="false">
    <WizardSteps>
        <telerik:RadWizardStep ID="RadWizardStep1" runat="server" Title="Process" SpriteCssClass="Step_One" ValidationGroup="personalInfo">

            <uc1:JobInfo runat="server" ID="JobInfo" />
        </telerik:RadWizardStep>
        <telerik:RadWizardStep ID="RadWizardStep2" runat="server" Title="Scheduler" SpriteCssClass="Step_Two">
            <uc1:SchedulerUC runat="server" ID="SchedulerUC" />
        </telerik:RadWizardStep>
    </WizardSteps>
</telerik:RadWizard>