<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MitigationWized.ascx.cs" Inherits="DI.Chakra.Web.Comp.Mitigation.Wizard.MitigationWized" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Comp/Mitigation/Common/MitigationCodeUC.ascx" TagName="MitigCode" TagPrefix="MtgCde" %>
<%@ Register Src="~/Comp/Mitigation/Common/RiskCodeUC.ascx" TagName="MitigRskCode" TagPrefix="MitigCde" %>
<%@ Register Src="~/Comp/Mitigation/Common/OutputUC.ascx" TagName="MitigOutput" TagPrefix="MitigOpt" %>


<telerik:RadWizard ID="MitigationWizard" runat="server" DisplayCancelButton="true" OnFinishButtonClick="MitigationWizard_FinishButtonClick" 
    OnClientButtonClicking="OnClientButtonClicking" OnCancelButtonClick="MitigationWizard_CancelButtonClick" DisplayProgressBar="false">
    <WizardSteps>

        <telerik:RadWizardStep ID="MitigWizard1" runat="server" Title="Mitigation Code" SpriteCssClass="Step_One" ValidationGroup="PolicyNameGroup">
            <MtgCde:MitigCode ID="MitigatCode" runat="server" />
        </telerik:RadWizardStep>

        <telerik:RadWizardStep ID="MitigWizard2" runat="server" Title="Risk Code" SpriteCssClass="Step_Two" ValidationGroup="PolicyNameGroup">
            <MitigCde:MitigRskCode ID="MitigRiskCode" runat="server" />
        </telerik:RadWizardStep>

        <telerik:RadWizardStep ID="MitigWizard3" runat="server" Title="Output" SpriteCssClass="Step_Three" ValidationGroup="PolicyNameGroup">
            <MitigOpt:MitigOutput ID="MitigOutput" runat="server"/>
        </telerik:RadWizardStep>

    </WizardSteps>
</telerik:RadWizard>