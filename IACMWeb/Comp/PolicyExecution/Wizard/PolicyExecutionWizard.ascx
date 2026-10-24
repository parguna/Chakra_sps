<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PolicyExecutionWizard.ascx.cs" Inherits="DI.Chakra.Web.Comp.PolicyExecution.Wizard.PolicyExecutionWizard" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Comp/PolicyExecution/Common/ComplianceExceptionsUC.ascx" TagPrefix="uc1" TagName="ComplianceExceptionsUC" %>
<%@ Register Src="~/Comp/PolicyExecution/Common/CompliancePolicyDetailUC.ascx" TagPrefix="uc1" TagName="CompliancePolicyDetailUC" %>
<%@ Register Src="~/Comp/PolicyExecution/Common/CompliancePolicyEndPointUC.ascx" TagPrefix="uc1" TagName="CompliancePolicyEndPointUC" %>

<telerik:RadWizard ID="PolicyExecutionRadWizard" runat="server" OnFinishButtonClick="PolicyExecutionRadWizard_FinishButtonClick" DisplayCancelButton="true"
     OnCancelButtonClick="PolicyExecutionRadWizard_CancelButtonClick"  OnClientButtonClicking="OnClientButtonClicking" DisplayProgressBar="false">
    <WizardSteps>

        <telerik:RadWizardStep ID="RadWizardStep1" runat="server" Title="Conflict Details" SpriteCssClass="Step_One" ValidationGroup="personalInfo">
            <uc1:CompliancePolicyDetailUC runat="server" ID="CompliancePolicyDetail" />
        </telerik:RadWizardStep>

        <telerik:RadWizardStep ID="RadWizardStep2" runat="server" Title="Function Details" SpriteCssClass="Step_Two">
            <uc1:CompliancePolicyEndPointUC runat="server" ID="CompliancePolicyEndPoint" />
        </telerik:RadWizardStep>

        <telerik:RadWizardStep ID="RadWizardStep3" runat="server" Title="Exception Details" SpriteCssClass="Step_Three">
            <uc1:ComplianceExceptionsUC runat="server" ID="ComplianceExceptions" />
        </telerik:RadWizardStep>

    </WizardSteps>
</telerik:RadWizard>