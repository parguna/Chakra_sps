<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttestationOverallStatus.ascx.cs" Inherits="DI.Chakra.Web.Dashboard.Attestation.AttestationOverallStatus" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Dashboard/Attestation/AttestationTypeUC.ascx" TagPrefix="uc1" TagName="AttestationTypeUC" %>
<%@ Register Src="~/Dashboard/DashBoardHeader.ascx" TagPrefix="uc1" TagName="DashBoardHeaderUC" %>
<uc1:DashBoardHeaderUC runat="server" ID="DashBoardHeaderUC" />

<div class="dashboard-box dashboard-content chart panel panel-default">  

<script type="text/javascript">
    function resizePieChart() {
        $find("<%=AttestationcompletedChart.ClientID%>").get_kendoWidget().resize();
    }

    if (window.addEventListener) {    // most non-IE browsers and IE9
        window.addEventListener("resize", resizePieChart, false);
    } else if (window.attachEvent) {  // Internet Explorer 5 or above
        window.attachEvent("onresize", resizePieChart);
    }
</script>
    <uc1:AttestationTypeUC runat="server" ID="AttestationTypeUC" />
 <telerik:RadHtmlChart runat="server" ID="AttestationcompletedChart" Width="50%" Height="50%" Transitions="true">

        <ChartTitle Text=" ">

            <Appearance Align="Center" Position="Top">
            </Appearance>

        </ChartTitle>

        <Legend>

            <Appearance Position="Bottom" Visible="true">
            </Appearance>

        </Legend>

        <PlotArea>

            <Series>

                <telerik:PieSeries StartAngle="90" DataFieldY="Y" Name="Name">

                    <LabelsAppearance Position="OutsideEnd">
                    </LabelsAppearance>

                    <TooltipsAppearance Color="White"></TooltipsAppearance>

                </telerik:PieSeries>

            </Series>
            <XAxis DataLabelsField="Name">
                <LabelsAppearance RotationAngle="75" />
                <TitleAppearance Text="Name" />
            </XAxis>

        </PlotArea>

    </telerik:RadHtmlChart>


    </div>