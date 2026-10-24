<%@ Page Title="" Language="C#" MasterPageFile="~/Master/FrontPage.master" AutoEventWireup="true" CodeBehind="SecurityQuestion.aspx.cs" Inherits="DI.Chakra.Web.PasswordMgmt.Forgot.SecurityQuestion" %>

<%@ Register src="UC/SecurityQA.ascx" tagname="SecurityQA" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cplLogin" runat="server">
    <div>
        <uc2:SecurityQA ID="SecurityQA" runat="server" />
    </div>
</asp:Content>
