<%@ Page Title="" Language="C#" MasterPageFile="~/Master/FrontPage.master" AutoEventWireup="true" CodeBehind="Otp.aspx.cs" Inherits="DI.Chakra.Web.PasswordMgmt.Forgot.Otp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplLogin" runat="server">
        <script type="text/javascript">
            function Redirect() {
                window.location = '/Login.aspx';
            }
       </script>
    <div>
        <h2 class="di-title inline">Please enter the OTP recieved on your Mobile/E-mail.</h2>
    </div>

    <div class="left-inner-addon">
        <i class="icon icon-user"></i>
        <asp:TextBox ID="txtOTP" CssClass="form-control" runat="server" TextMode="Password"></asp:TextBox>
    </div>

    <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-default btn-primary btn-block" Text="Submit"
        OnClick="btnSubmit_Click" />

    <div class="forget-password topBuffer">
         <a href="/Login.aspx">&#8592; Login</a>
    </div>
</asp:Content>
