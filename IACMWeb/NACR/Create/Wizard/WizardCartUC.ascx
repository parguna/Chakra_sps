<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WizardCartUC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.Wizard.WizardCartUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/NACR/Create/Common/CartItemUC.ascx" TagPrefix="uc1" TagName="CartItemUC" %>
<%@ Register Src="~/NACR/Create/App/AccessUC.ascx" TagPrefix="uc1" TagName="AccessUC" %>
<%@ Register Src="~/NACR/Create/Common/AdhocAccessUC.ascx" TagPrefix="uc1" TagName="AdhocAccessUC" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/AdhocADStep2UC.ascx" TagPrefix="uc1" TagName="AdhocADStep2UC" %>
<%@ Register Src="~/NACR/Create/App/CartInfo.ascx" TagPrefix="uc1" TagName="CartInfo" %>

<script>
    //function BackgroundOnDoubeClick() {
    //    var divLoadImage = document.getElementById('divLoadImg');
    //    divLoadImage.style.display = 'none';
    //    var divBackground = document.getElementById('divBackground');
    //    divBackground.style.display = 'none';
    //    var objMan = Sys.WebForms.PageRequestManager.getInstance();
    //    if (objMan.get_isInAsyncPostBack())
    //        objMan.abortPostBack();
    //}
</script>

<style>
    .RadWizard {
        max-width: 99% !important;
    }

    ul.rwzBreadCrumb li:nth-child(-n+2) {
        width: 100px !important;
    }

    /*#divLoadImg {
        position: absolute;
        left: 50%;
        top: calc(50% - 45px);
        transform: translateX(-50%);
        visibility: visible;
        vertical-align: middle;
        z-index: 10000000001;
        border: 1px solid #2077bd;
        background-color: white;
        border-bottom-left-radius: 5px;
        border-bottom-right-radius: 5px;
        border-top-left-radius: 5px;
        border-top-right-radius: 5px;
    }*/
</style>

<div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
           <%-- <asp:UpdateProgress ID="UpdateProgressBar" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="1000">
                <ProgressTemplate>
                    <div id="divBackground" ondblclick="BackgroundOnDoubeClick()"
                        style="position: fixed; top: 0px; left: 0px; z-index: 100000001; height: 100%; width: 100%; background-color: black; opacity: 0.4; filter: alpha(opacity=40)">
                    </div>
                    <div id="divLoadImg" align="center" valign="middle" runat="server">
                        <asp:Image ID="Image1" runat="server" ImageUrl="../Images/Common/loading.gif" Style="padding: 5px" border="0" Width="40" />
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>--%>
            <asp:Panel runat="server">

                <div class="form-horizontal left clearfix">
                    <%--<div class="search-result-header">
                <h4 class="inline">Search Results</h4>--%>
                    <uc1:CartInfo runat="server" ID="CartInfo" EditCartItemClicked="GotoStep3_Clicked" />
                </div>

                <telerik:RadWizard ID="RadWizard" runat="server" Localization-Finish="Submit" OnClientButtonClicking="OnClientButtonClicking" DisplayCancelButton="true" DisplayProgressBar="false" OnCancelButtonClick="RadWizard_CancelButtonClick" OnFinishButtonClick="RadWizard_FinishButtonClick" OnPreviousButtonClick="ApplicationBind" OnNavigationBarButtonClick="RadWizard_NavigationBarButtonClick" OnNextButtonClick="ApplicationBind" ClientIDMode="Static">
                    <WizardSteps>
                        <telerik:RadWizardStep ID="RadWizardStep1" runat="server" Title="Request" SpriteCssClass="Step_One">
                            <uc1:AdhocAccessUC runat="server" ID="AdhocAccessUC" />
                        </telerik:RadWizardStep>
                        <telerik:RadWizardStep ID="RadWizardStep2" runat="server" Title="Application" SpriteCssClass="Step_Two">
                            <uc1:AccessUC runat="server" ID="AccessUC" OnSaveInterimStore="RefreshCart" />
                        </telerik:RadWizardStep>
                        <telerik:RadWizardStep ID="RadWizardStep3" runat="server" Title="Edit Cart" SpriteCssClass="Step_Three">
                            <uc1:CartItemUC runat="server" ID="CartItemUC" />
                        </telerik:RadWizardStep>
                    </WizardSteps>
                </telerik:RadWizard>

            </asp:Panel>

        </ContentTemplate>
      <%--  <Triggers>
            <asp:AsyncPostBackTrigger ControlID="RadWizard" />
        </Triggers>--%>
    </asp:UpdatePanel>
</div>
