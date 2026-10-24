<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompliancePolicyWizardUC.ascx.cs" Inherits="IACMWeb.Comp.Policy.Wizard.CompliancePolicyWizardUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Comp/Policy/Common/ComplianceDetailsUC.ascx" TagPrefix="uc1" TagName="ComplianceDetailsUC" %>
<%@ Register Src="~/Comp/Policy/Common/ConflictDetailsUC.ascx" TagPrefix="uc1" TagName="ConflictDetailsUC" %>



<telerik:RadWizard ID="ComplianceRadWizard" runat="server" OnFinishButtonClick="ComplianceRadWizard_FinishButtonClick" DisplayCancelButton="true"
     OnCancelButtonClick="CompliancePolicyRadWizard_CancelButtonClick" OnClientButtonClicking="OnClientButtonClicking" DisplayProgressBar="false">
    <WizardSteps>

        <telerik:RadWizardStep ID="RadWizardStep1" runat="server" Title="Conflict Details" SpriteCssClass="Step_One" ValidationGroup="personalInfo">
            <uc1:ComplianceDetailsUC runat="server" id="ComplianceDetails" />
        </telerik:RadWizardStep>

        <telerik:RadWizardStep ID="RadWizardStep2" runat="server" Title="Function Details" SpriteCssClass="Step_Two">
            <uc1:ConflictDetailsUC runat="server" ID="ConflictDetails" />
        </telerik:RadWizardStep>

    </WizardSteps>
</telerik:RadWizard>