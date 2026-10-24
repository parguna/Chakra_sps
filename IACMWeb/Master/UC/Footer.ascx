<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Footer.ascx.cs" Inherits="DI.Chakra.Web.Master.UC.Footer" %>
<footer>
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="col-xs-6 text-left">
                <%--<img class="di-logo white inline" alt="Deep Identity" src="~/Images/deep-identity-logo-white.png" runat="server">--%>
                <h2 class="di-title inline"><%--Deep Identity--%> &nbsp;&nbsp;</h2>
                <p><%--Copyright © 2015 Deep Identity. All rights reserved.--%>&nbsp;&nbsp;</p>
            </div>
             <div class="last-login col-xs-6 text-right pull-down">
               <%--Version   <asp:Label ID="lblVersion" CssClass="user-firstname" runat="server"></asp:Label>--%>
                 &nbsp;&nbsp;
            </div>
            <div class="last-login col-xs-6 text-right pull-down">
                <p>Last login:  <asp:Label ID="lblLastLogin" CssClass="user-firstname" runat="server"></asp:Label></p>
            </div>
            

        </div>
    </div>
</footer>