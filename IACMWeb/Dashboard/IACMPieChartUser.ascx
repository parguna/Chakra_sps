<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="IACMPieChartUser.ascx.cs" Inherits="DI.Chakra.Web.Dashboard.IACMPieChartUser" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Dashboard/DashBoardHeader.ascx" TagPrefix="uc1" TagName="DashBoardHeaderUC" %>

<script type="text/javascript">
    function resizePieChart() {
        $find("<%=AccessRequestSourceChart.ClientID%>").get_kendoWidget().resize();
    }

    if (window.addEventListener) {    // most non-IE browsers and IE9
        window.addEventListener("resize", resizePieChart, false);
    } else if (window.attachEvent) {  // Internet Explorer 5 or above
        window.attachEvent("onresize", resizePieChart);
    }
</script>

<uc1:DashBoardHeaderUC runat="server" ID="DashBoardHeaderUCUC" />
<div class="dashboard-box dashboard-content chart panel panel-default">
<div class="form-group">
        <div class="col-xs-7">
            <asp:Label ID="lblYear" Text="" runat="server"></asp:Label>            
        </div>
    </div>
    <telerik:RadHtmlChart runat="server" ID="AccessRequestSourceChart" Width="100%" Height="100%" Transitions="true">

        <ChartTitle Text="Access Request Sources">

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

<telerik:RadAjaxLoadingPanel ID="LoadingPanel1" Height="77px" Width="113px" runat="server">
</telerik:RadAjaxLoadingPanel>
