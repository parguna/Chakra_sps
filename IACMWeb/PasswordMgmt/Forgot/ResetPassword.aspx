<%@ Page Title="" Language="C#" MasterPageFile="~/Master/FrontPage.master" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="DI.Chakra.Web.PasswordMgmt.Forgot.ResetPassword" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EnterprisePasswordPolicy.ascx" TagPrefix="uc1" TagName="EnterprisePasswordPolicy" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplLogin" runat="server">
 
    <telerik:RadWindowManager ID="radWinMgr" runat="server">
    </telerik:RadWindowManager>
    <telerik:RadWindow ID="DisplayPolicy" runat="server" VisibleStatusbar="false" Height="550px" Width="500px" RenderMode="Lightweight" Behaviors="Close"
        ShowContentDuringLoad="false" Modal="true" EnableViewState="true" VisibleTitlebar="false">
        <ContentTemplate>
            <uc1:EnterprisePasswordPolicy runat="server" ID="EnterprisePasswordPolicy" />
            <div class="modal-footer">
                 <asp:Button runat="server" ID="ok" CssClass="btn btn-primary transparent-button" Text="OK" UseSubmitBehavior="false" OnClick="ok_Click" />
            </div>
        </ContentTemplate>
    </telerik:RadWindow>
    <div class="left-inner-addon">
        <i class="icon icon-password"></i>
        <asp:TextBox ID="txtPassword" CssClass="form-control" runat="server" placeholder="Password" required="required" TextMode="Password" EnableViewState="false"></asp:TextBox>

    </div>
    <div class="left-inner-addon">
        <i class="icon icon-password"></i>
        <asp:TextBox ID="txtConfirmPassword" CssClass="form-control" runat="server" placeholder="Confirm Password" required="required" TextMode="Password" EnableViewState="false"></asp:TextBox>

    </div>
    <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-default btn-primary btn-block" Text="Submit"
        OnClick="btnSubmit_Click" />
    <div class="topBuffer">
    <asp:Button ID="DisplayPolicies" runat="server" CssClass="btn btn-default btn-sm" Text="View Password Policy" Visible="false" UseSubmitBehavior="false" OnClick="DisplayPolicies_Click" />
    </div>
    <div class="forget-password topBuffer">
         <a href="/Login.aspx">&#8592; Login</a>
    </div>   

    <div class="form-group topBuffer"><label class="control-label">Note:Change of Password is only for AD authentication systems in login page</label></div>
</asp:Content>
