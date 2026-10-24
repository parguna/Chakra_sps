<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaskAssigneeUC.ascx.cs" Inherits="DI.Chakra.Web.Dashboard.Attestation.TaskAssigneeUC" %>
<%@ Register Src="~/Dashboard/Attestation/AttestationTypeUC.ascx" TagPrefix="uc1" TagName="AttestationTypeUC" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/Dashboard/DashBoardHeader.ascx" TagPrefix="uc1" TagName="DashBoardHeaderUC" %>
<%@ Register Src="~/Dashboard/Attestation/NoDataUC.ascx" TagPrefix="uc1" TagName="NoDataUC" %>

<uc1:DashBoardHeaderUC runat="server" ID="DashBoardHeaderUC" />
<link rel="stylesheet" href="Content/Dashboard.css" type="text/css" />


<div id ="divChartControl" runat="server" class="dashboard-box dashboard-content chart panel panel-default">
    <h3>
        <span>Task Assignee Info </span>
    </h3>
    <div>
        <uc1:AttestationTypeUC runat="server" ID="AttestationTypeUC" />
    </div>
    <asp:Panel ID="pnlAssignee" runat="server" Height="400" Width="800"></asp:Panel>
</div>
<div id="divNoData" runat="server">
    <uc1:NoDataUC runat="server" ID="NoDataUC" />
</div>