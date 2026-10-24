<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DashBoardHeader.ascx.cs" Inherits="DI.Chakra.Web.Dashboard.DashBoardHeader" %>

<div class="dashboard-wrapper">
    <h3 runat ="server" id ="dashboardCaption"></h3>
    <div class="dashboard-box dashboard-header panel panel-default">
        <div class="panel-heading">
            <span class="dashboard-icon" runat ="server" id ="imageSpan">
                <%--<span><img src="../Images/compliance-icon.png" /></span>--%>
                <span>
                    <asp:Image ID="imgHeader" runat="server" ImageUrl="../Images/compliance-icon.png" /></span>
            </span>
            <span class="dashboard-icon" runat ="server" id ="rowCountSpan" style="display:block;">
                <span class="dashboard-count" id ="dashBoardRowCount" runat ="server"></span>
            </span>
            <h4 class="dashboard-title inline lblPageTitle" style="font-weight:bold" runat ="server" id ="dashBoardTitle"></h4>
            <span class="inline pull-right"><a href ="" title="More" class="go" id ="navigateUrl" runat ="server"></a></span>
        </div>
        
    </div>
</div>