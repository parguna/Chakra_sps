<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttestationWizardUC.ascx.cs" Inherits="DI.Chakra.Web.Attestation.Wizard.AttestationWizardUC" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Admin/Attestation/Common/AttestationInfo.ascx" TagPrefix="uc1" TagName="AttestationInfo" %>
<%@ Register Src="~/Admin/Attestation/Common/AttestationScopeInfo.ascx" TagPrefix="uc1" TagName="AttestationScopeInfo" %>
<%@ Register Src="~/UsrControl/Common/WorkflowInfo.ascx" TagPrefix="uc1" TagName="WorkflowInfo" %>
<%@ Register Src="~/UsrControl/Scheduler/SchedulerUC.ascx" TagPrefix="uc1" TagName="SchedulerInfo" %>

<div class="content-wrapper" id="ContentWrapper" runat="server">
    <div class="form">
        <telerik:RadWizard ID="ProvRoleWizard" runat="server" DisplayCancelButton="true" OnNavigationBarButtonClick="ProvRoleWizard_NextButtonClick" 
            OnNextButtonClick="ProvRoleWizard_NextButtonClick" OnCancelButtonClick="ProvRoleWizard_CancelButtonClick"
            DisplayProgressBar="false" OnFinishButtonClick="ProvRoleWizard_FinishButtonClick"  OnClientButtonClicking="OnClientButtonClicking">
            <WizardSteps>
                <telerik:RadWizardStep ID="RadWizardAttestationStep1" runat="server" Title="Basic"  SpriteCssClass="Step_One">
                    <uc1:AttestationInfo runat="server" ID="AttestationInfo" />
                </telerik:RadWizardStep>
                <telerik:RadWizardStep ID="RadWizardAttestationScopeStep2" runat="server" Title="Scope" SpriteCssClass="Step_Two">
                    <uc1:AttestationScopeInfo runat="server" ID="AttestationScopeInfo" />
                </telerik:RadWizardStep>
                <telerik:RadWizardStep ID="RadWizardWorkflowStep3" runat="server" Title="Workflow" SpriteCssClass="Step_Three">
                    <uc1:WorkflowInfo runat="server" ID="WorkflowInfo" />
                </telerik:RadWizardStep>         
            </WizardSteps>
        </telerik:RadWizard>
    </div>
</div>

