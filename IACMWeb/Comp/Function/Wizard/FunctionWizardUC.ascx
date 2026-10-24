<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FunctionWizardUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.Function.Wizard.FunctionWizardUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Comp/Function/Common/FunctionDetailsUC.ascx" TagPrefix="uc1" TagName="FunctionDetailsUC" %>
<%@ Register Src="~/Comp/Function/Common/EndPointSystemUC.ascx" TagPrefix="uc1" TagName="EndPointSystemUC" %>

<telerik:RadWizard ID="FunctionRadWizard" runat="server" OnFinishButtonClick="FunctionRadWizard_FinishButtonClick" DisplayCancelButton="true"
     OnCancelButtonClick="FunctionRadWizard_CancelButtonClick" OnClientButtonClicking="OnClientButtonClicking" DisplayProgressBar="false">
    <wizardsteps>

        <telerik:RadWizardStep ID="RadWizardStep1" runat="server" Title="Conflict Details" SpriteCssClass="Step_One" ValidationGroup="personalInfo">
            <uc1:FunctionDetailsUC runat="server" ID="FunctionDetailsUC" />
        </telerik:RadWizardStep>

        <telerik:RadWizardStep ID="RadWizardStep2" runat="server" Title="Function Details" SpriteCssClass="Step_Two">
            <uc1:EndPointSystemUC runat="server" ID="EndPointSystemUC" />
        </telerik:RadWizardStep>

    </wizardsteps>
</telerik:RadWizard>