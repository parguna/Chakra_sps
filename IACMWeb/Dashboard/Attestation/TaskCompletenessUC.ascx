<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaskCompletenessUC.ascx.cs" Inherits="DI.Chakra.Web.Dashboard.Attestation.TaskCompletenessUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Dashboard/Attestation/AttestationTypeUC.ascx" TagPrefix="uc1" TagName="AttestationTypeUC" %>
<%@ Register Src="~/Dashboard/DashBoardHeader.ascx" TagPrefix="uc1" TagName="DashBoardHeaderUC" %>

<script type="text/javascript">
    function resizePieChart() {
        $find("<%=ragTaskCompleteness.ClientID%>").get_kendoWidget().resize();
    }

    if (window.addEventListener) {    // most non-IE browsers and IE9
        window.addEventListener("resize", resizePieChart, false);
    } else if (window.attachEvent) {  // Internet Explorer 5 or above
        window.attachEvent("onresize", resizePieChart);
    }
</script>

<uc1:DashBoardHeaderUC runat="server" ID="DashBoardHeaderUC" />

<div class="dashboard-box dashboard-content chart panel panel-default">

    <uc1:AttestationTypeUC runat="server" ID="AttestationTypeUC" />

    <telerik:RadRadialGauge runat="server" ID="ragTaskCompleteness" Width="100%" Height="100%" Transitions="true">

        <Pointer Color="#FE8442">
            <Cap Color="#FE8442" Size="0.05" />
        </Pointer>

    </telerik:RadRadialGauge>
</div>
