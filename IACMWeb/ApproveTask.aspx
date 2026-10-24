<%@ Page Language="C#" Title="Approve Task" MasterPageFile="~/Master/FrontPage.master" AutoEventWireup="true" CodeBehind="ApproveTask.aspx.cs" Inherits="DI.Chakra.Web.ApproveTask" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplLogin" runat="server">
    <style>
        .success {
            font-weight: bold;
            color: green;
        }

        .fail {
            font-weight: bold;
            color: red;
        }
    </style>

    <div class="login-form-wrapper">
        <div style="text-align:center">
            <span id="processing" runat="server">Please wait while your task is being Approved...</span>
            <br />
            <br />
            <br />
            <span class="" id="taskStatus" runat="server"></span>
        </div>
    </div>
</asp:Content>
