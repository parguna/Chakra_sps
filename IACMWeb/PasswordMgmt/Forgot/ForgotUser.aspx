<%@ Page Title="" Language="C#" MasterPageFile="~/Master/FrontPage.master" AutoEventWireup="true" CodeBehind="ForgotUser.aspx.cs" Inherits="DI.Chakra.Web.PasswordMgmt.Forgot.ForgotUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplLogin" runat="server">
    <div class="left-inner-addon">
        <i class="icon icon-user"></i>
        <asp:TextBox ID="txtUserName" CssClass="form-control" runat="server" placeholder="Username" required="required" EnableViewState="false"></asp:TextBox>
          <i class="icon icon-user"></i>
        <asp:TextBox ID="txtNRIC" CssClass="form-control" runat="server" placeholder="NRIC"  EnableViewState="false" Visible="false"></asp:TextBox>
          <i class="icon icon-user"></i>
        <asp:TextBox ID="txtDOB" CssClass="form-control" runat="server" placeholder="D.O.B"  EnableViewState="false" Visible="false"></asp:TextBox>
    </div>

    <asp:Button ID="btnForgot" runat="server" CssClass="btn btn-default btn-primary btn-block" Text="Forgot Password"
        OnClick="btnForgot_Click" />

    <div class="forget-password topBuffer">
         <a href="/Login.aspx">&#8592; Login</a>
    </div>
</asp:Content>
