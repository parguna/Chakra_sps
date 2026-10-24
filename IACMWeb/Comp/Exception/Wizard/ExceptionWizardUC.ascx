<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExceptionWizardUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.Exception.Wizard.ExceptionWizardUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register Src="~/Comp/Exception/Common/ExceptionDetailsUC.ascx" TagName="ExceptionDeatils" TagPrefix="ExD" %>
<%@ Register Src="~/Comp/Exception/Common/UserDetailsUC.ascx" TagName="UserDetails" TagPrefix="UsD" %>

<telerik:RadWizard ID="ExceptionWizard" runat="server" OnFinishButtonClick="ExceptionWizard_FinishButtonClick" DisplayCancelButton="true"
     OnCancelButtonClick="ExceptionRadWizard_CancelButtonClick" OnClientButtonClicking="OnClientButtonClicking" DisplayProgressBar="false">
    <WizardSteps>

        <telerik:RadWizardStep ID="RadWizardStep1" runat="server" Title="Exception Details" SpriteCssClass="Step_One" ValidationGroup="PolicyNameGroup"> 
            <ExD:ExceptionDeatils runat="server"  ID="Exceptiondetails"/>
        </telerik:RadWizardStep>

        <telerik:RadWizardStep ID="RadWizardStep2" runat="server" Title="User Details" SpriteCssClass="Step_Two">
            <UsD:UserDetails runat="server" ID="UserDetails" />
        </telerik:RadWizardStep>

    </WizardSteps>
</telerik:RadWizard>