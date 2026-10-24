<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConflictWizardUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.Conflict.Wizard.ConflictWizardUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Comp/Conflict/Common/ConflictDetailsUC.ascx" TagPrefix="uc1" TagName="ConflictDetailsUC" %>
<%@ Register Src="~/Comp/Conflict/Common/FucntionDetailsUC.ascx" TagPrefix="uc1" TagName="FucntionDetailsUC" %>

<telerik:RadWizard ID="ConflictRadWizard" runat="server" OnFinishButtonClick="ConflictRadWizard_FinishButtonClick" DisplayCancelButton="true"
     OnCancelButtonClick="ConflictRadWizard_CancelButtonClick" OnClientButtonClicking="OnClientButtonClicking" DisplayProgressBar="false">
    <WizardSteps>

        <telerik:RadWizardStep ID="RadWizardStep1" runat="server" Title="Conflict Details" SpriteCssClass="Step_One" ValidationGroup="personalInfo">
           <uc1:ConflictDetailsUC runat="server" id="conflictDetails" />
        </telerik:RadWizardStep>

        <telerik:RadWizardStep ID="RadWizardStep2" runat="server" Title="Function Details" SpriteCssClass="Step_Two">
          <uc1:FucntionDetailsUC runat="server" id="functionDetails" />
        </telerik:RadWizardStep>

    </WizardSteps>
</telerik:RadWizard>