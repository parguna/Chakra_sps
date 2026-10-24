<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NFooter.ascx.cs" Inherits="DI.Chakra.Web.Master.UC.NFooter" %>
<footer>
    <div class="container-fluid">
        <div style="display: inline-block;float:right;">
            <span class="text-muted" style="color:#fff !important;font-weight:500 !important;">Last login:<asp:Label ID="lblLastLogin" CssClass="user-firstname" runat="server"></asp:Label>
            </span>
        </div>

    </div>
    <style>
        footer {
            display:block;
            width: 100% !important;
            height: 15px !important;
            background: none repeat scroll 0% 0% #2077BD;
            color:#fff;
            font-weight:lighter;
        }
    </style>
</footer>
