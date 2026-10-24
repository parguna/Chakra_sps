
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccessUC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.AccessUC" %>
<%@ Register Src="~/UsrControl/Common/CurrentAccessUC.ascx" TagPrefix="uc1" TagName="CurrentAccessUC" %>
<%@ Register Src="~/NACR/Create/App/CartInfo.ascx" TagPrefix="uc1" TagName="CartInfo" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/NACR/Create/App/RequestItemList.ascx" TagPrefix="uc1" TagName="RequestItemList" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/AdhocADStep2UC.ascx" TagPrefix="uc1" TagName="AdhocADStep2UC" %>
<%@ Register Src="~/NACR/Create/Common/AdhocAccessUC.ascx" TagPrefix="uc1" TagName="AdhocAccessUC" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/SecurityQuestionUC.ascx" TagPrefix="uc1" TagName="SecurityQuestionUC" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/AdhocSapFlinsStep2UC.ascx" TagPrefix="uc1" TagName="AdhocSapFlinsStep2UC" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/AdhocSapLocalStep2UC.ascx" TagPrefix="uc1" TagName="AdhocSapLocalStep2UC" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/AdhocSapFlinsBIStep2UC.ascx" TagPrefix="uc1" TagName="AdhocSapFlinsBIStep2UC" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/AdhocSapPiStep2UC.ascx" TagPrefix="uc1" TagName="AdhocSapPiStep2UC" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/AdhocSapGrcStep2UC.ascx" TagPrefix="uc1" TagName="AdhocSapGrcStep2UC" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/AdhocSapMsslStep2UC.ascx" TagPrefix="uc1" TagName="AdhocSapMsslStep2UC" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/AdhocNpacStep2UC.ascx" TagPrefix="uc1" TagName="AdhocNpacStep2UC" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/AdhocGemsStep2UC.ascx" TagPrefix="uc1" TagName="AdhocGemsStep2UC" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/AdhocSapEbsStep2UC.ascx" TagPrefix="uc1" TagName="AdhocSapEbsStep2UC" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/AdhocOffice365Step2UC.ascx" TagPrefix="uc1" TagName="AdhocOffice365Step2UC" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/AdhocInHouseStep2UC.ascx" TagPrefix="uc1" TagName="AdhocInHouseStep2UC" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/AdhocSapEpCeStep2UC.ascx" TagPrefix="uc1" TagName="AdhocSapEpCeStep2UC" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/AdhocOneCardStep2UC.ascx" TagPrefix="uc1" TagName="AdhocOneCardStep2UC" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/AdhocSapSrmStep2UC.ascx" TagPrefix="uc1" TagName="AdhocSapSrmStep2UC" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/AdhocSsidsStep2UC.ascx" TagPrefix="uc1" TagName="AdhocSsidsStep2UC" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/AdhocIdmsStep2UC.ascx" TagPrefix="uc1" TagName="AdhocIdmsStep2UC" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/AdhocVPNStep2UC.ascx" TagPrefix="uc1" TagName="AdhocVPNStep2UC" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/AdhocSkypeForBusinessStep2UC.ascx" TagPrefix="uc1" TagName="AdhocSkypeForBusinessStep2UC" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/AdhocNovaStep2UC.ascx" TagPrefix="uc1" TagName="AdhocNovaStep2UC" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/AdhocSAPHANAStep2UC.ascx" TagPrefix="uc1" TagName="AdhocSAPHANAStep2UC" %>

<style>
    #RadWindowWrapper_ctl00_cplMain_WizardCartUC_AccessUC_CurrentAccess{
        top:20% !important;
    }
    #ctl00_cplMain_WizardCartUC_AccessUC_CurrentAccess_C{
        max-height:550px !important;
    }
     #RadWindowWrapper_ctl00_cplMain_WizardCartUC_AccessUC_CurrentAccess{
        top:6% !important;
    }
      div#divModalbody {
    min-height: 95%;
    height: 550px !important;
    overflow-y: scroll;
}
</style>
<script>
      function showWindow() {
        var oWnd = $find("<%= CurrentAccess.ClientID %>");
        oWnd.show();
    }

    function closeWindow() {
        var oWnd = $find("<%= CurrentAccess.ClientID %>");
        oWnd.close();
    }
</script>

<div class="form">
    <div class="form-horizontal clearfix">
        <div class="col-md-12">
            <div style="float:right;padding-bottom:5px;">
            <asp:LinkButton ID="ViewCurrentAccess" OnClick="ViewCurrentAccess_Click" runat="server" Text="View Current Access"></asp:LinkButton>
            </div>
        </div>
    </div>
    <div class="form-horizontal left clearfix">
        <div class="col-md-12">
            <asp:Panel runat="server" ID="pnlStep2Controls">
                <uc1:AdhocADStep2UC runat="server" ID="AdhocADStep2UC" Visible="false" OnToSaveAccessRequest="SaveAccessRequest" />
                <uc1:AdhocSapFlinsStep2UC runat="server" ID="AdhocSapFlinsStep2UC" Visible="false" OnToSaveAccessRequest="SaveAccessRequest" /> 
                <uc1:AdhocSapLocalStep2UC runat="server" id="AdhocSapLocalStep2UC"  Visible="false" OnToSaveAccessRequest="SaveAccessRequest"  />
                <uc1:AdhocSapFlinsBIStep2UC runat="server" id="AdhocSapFlinsBIStep2UC"  Visible="false" OnToSaveAccessRequest="SaveAccessRequest" />
                <uc1:AdhocSapPiStep2UC runat="server" id="AdhocSapPiStep2UC"  Visible="false"   OnToSaveAccessRequest="SaveAccessRequest" />
                <uc1:AdhocSapGrcStep2UC runat="server" id="AdhocSapGrcStep2UC"   Visible="false"   OnToSaveAccessRequest="SaveAccessRequest"  />
                <uc1:AdhocSapMsslStep2UC runat="server" id="AdhocSapMsslStep2UC"  Visible="false"  OnToSaveAccessRequest="SaveAccessRequest"   />
                <uc1:AdhocNpacStep2UC runat="server" id="AdhocNpacStep2UC"  Visible="false"   OnToSaveAccessRequest="SaveAccessRequest"    /> 
                <uc1:AdhocGemsStep2UC runat="server" id="AdhocGemsStep2UC"  Visible="false"    OnToSaveAccessRequest="SaveAccessRequest"    />
                <uc1:AdhocSapEbsStep2UC runat="server" id="AdhocSapEbsStep2UC"  Visible="false"   OnToSaveAccessRequest="SaveAccessRequest"     />
                <uc1:AdhocOffice365Step2UC runat="server" id="AdhocOffice365Step2UC"   Visible="false"   OnToSaveAccessRequest="SaveAccessRequest"      />
                <uc1:AdhocInHouseStep2UC runat="server" ID="AdhocInHouseStep2UC" Visible="false" OnToSaveAccessRequest="SaveAccessRequest" />
                <uc1:AdhocSapEpCeStep2UC runat="server" id="AdhocSapEpCeStep2UC"  Visible="false" OnToSaveAccessRequest="SaveAccessRequest" />
                <uc1:AdhocOneCardStep2UC runat="server" id="AdhocOneCardStep2UC"   Visible="false" OnToSaveAccessRequest="SaveAccessRequest"  />
                <uc1:AdhocSapSrmStep2UC runat="server" id="AdhocSapSrmStep2UC"     Visible="false" OnToSaveAccessRequest="SaveAccessRequest"  />
                <uc1:AdhocSsidsStep2UC runat="server" ID="AdhocSsidsStep2UC"     Visible="false" OnToSaveAccessRequest="SaveAccessRequest"   />
                <uc1:AdhocIdmsStep2UC runat="server" id="AdhocIdmsStep2UC"      Visible="false" OnToSaveAccessRequest="SaveAccessRequest"  />
                <uc1:AdhocVPNStep2UC runat="server" ID="AdhocVPNStep2UC"     Visible="false" OnToSaveAccessRequest="SaveAccessRequest"   />
                <uc1:AdhocSkypeForBusinessStep2UC runat="server" ID="AdhocSkypeForBusinessStep2UC" Visible="false" OnToSaveAccessRequest="SaveAccessRequest"/>
                <uc1:AdhocNovaStep2UC runat="server" ID="AdhocNovaStep2UC"     Visible="false" OnToSaveAccessRequest="SaveAccessRequest"   />
                <uc1:AdhocSAPHANAStep2UC runat="server" ID="AdhocSAPHANAStep2UC"     Visible="false" OnToSaveAccessRequest="SaveAccessRequest"   />
             </asp:Panel>
        </div>
    </div>

</div>

<telerik:RadWindow runat="server" ID="CurrentAccess" Modal="true" Height="100%" Title="Current Access"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title">Current Access</h3>
                </div>
                <div class="modal-body" id="divModalbody">
                    <div class="row-fluid">
                        <uc1:CurrentAccessUC runat="server" ID="CurrentAccessUC" IsShowIcon="false" />
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnCancel" class="btn btn-primary bordered-button" Text="OK" OnClick="btnCancel_Click" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>
