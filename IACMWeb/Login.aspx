<%@ Page Title="Identity & Data Governance Suite Login" Language="C#" MasterPageFile="~/Master/FrontPage.master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="DI.Chakra.Web.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplLogin" runat="server">
    <style>
        .divLoadImg {
            position: absolute;
            left: 50%;
            top: calc(50% - 45px);
            transform: translateX(-50%) !important;
            visibility: visible;
            vertical-align: middle;
            z-index: 10000000001;
            border: 1px solid #2077bd;
            background-color: white;
            border-bottom-left-radius: 5px;
            border-bottom-right-radius: 5px;
            border-top-left-radius: 5px;
            border-top-right-radius: 5px;
        }
    </style>
    <asp:UpdatePanel ID="updatePanel2" runat="server" UpdateMode="Conditional"
        ChildrenAsTriggers="true">
        <contenttemplate>
            <asp:UpdateProgress ID="UpdateProgressBar" runat="server" AssociatedUpdatePanelID="updatePanel2" DisplayAfter="10">
                <ProgressTemplate>
                    <div id="divBackground" ondblclick="BackgroundOnDoubeClick()"
                        style="position: fixed; top: 0px; left: 0px; z-index: 100000001; height: 100%; width: 100%; background-color: black; opacity: 0.4; filter: alpha(opacity=40)">
                    </div>
                    <div id="divLoadImg" class="divLoadImg" align="center" valign="middle" runat="server">
                        <asp:Image ID="Image1" runat="server" ImageUrl="../Images/Common/loading.gif" Style="padding: 5px" border="0" Width="40" />
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <div class="login-form-wrapper">
                <div class="left-inner-addon">
                    <i class="icon icon-user"></i>
                    <asp:TextBox ID="txtUserName" CssClass="form-control" runat="server" placeholder="Username" required="required" EnableViewState="false"></asp:TextBox>
                </div>
                <div class="left-inner-addon">
                    <i class="icon icon-password"></i>

                    <asp:TextBox ID="txtPassword" CssClass="form-control" oncopy="return false" onpaste="return false" runat="server"
                        TextMode="Password" placeholder="Password" required="required" data-required-msg="Password is required"></asp:TextBox>

                </div>
                <asp:Button ID="btnLogin" runat="server" CssClass="btn btn-default btn-primary btn-block" Text="Login"
                    OnClick="btnLogin_Click" />
                <div style=" text-align: center;">
                <p class="loginnotes" style="color:red;margin-top:8px !important;margin-bottom:2px !important;font-weight:bolder;float:left;white-space: nowrap; ">
                    <asp:Label runat="server" ID="lblHeadingNotes" Text="Application access request for PRODUCTION environment only"></asp:Label></p>
                 <p class="loginnotes" style="color:red;margin-top:2px !important;margin-bottom:2px !important;float:left;white-space: nowrap; ">
                     <asp:Label runat="server" ID="lblSubHeadingNotes" Text="For QA/DEV access request, please raise through ServiceNow"></asp:Label>
                 </p>
                </div>
            </div>
             <div class="login-form-wrapper" style="">
                 
            </div>
        </contenttemplate>
        <triggers>
            <asp:AsyncPostBackTrigger ControlID="btnLogin" EventName="Click" />
        </triggers>
    </asp:UpdatePanel>
</asp:Content>
