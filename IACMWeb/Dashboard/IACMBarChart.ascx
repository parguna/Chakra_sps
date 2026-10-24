<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="IACMBarChart.ascx.cs" Inherits="DI.Chakra.Web.Dashboard.IACMBarChart" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<script type="text/javascript">
    function resizeBarChart() {
        $find("<%=BarChart.ClientID%>").get_kendoWidget().resize();
			}

			if (window.addEventListener) {    // most non-IE browsers and IE9
			    window.addEventListener("resize", resizeBarChart, false);
			} else if (window.attachEvent) {  // Internet Explorer 5 or above
			    window.attachEvent("onresize", resizeBarChart);
			}
</script>
<div class="dashboard-box dashboard-content chart panel panel-default">
<telerik:RadHtmlChart runat="server" ID="BarChart" Width="100%" Height="100%" Transitions="true" Skin="Silk">

    <PlotArea>

        <Series>

            <telerik:BarSeries Name="Quarter 1" Stacked="false" Gap="1.5" Spacing="0.4">

                <Appearance>

                    <FillStyle BackgroundColor="#c5d291"></FillStyle>

                </Appearance>

                <LabelsAppearance DataFormatString="${0}" Position="Center">
                </LabelsAppearance>

                <TooltipsAppearance BackgroundColor="#c5d291" DataFormatString="${0}" Color="White"></TooltipsAppearance>

                <SeriesItems>

                    <telerik:CategorySeriesItem Y="315000"></telerik:CategorySeriesItem>

                    <telerik:CategorySeriesItem Y="495000"></telerik:CategorySeriesItem>

                    <telerik:CategorySeriesItem Y="690000"></telerik:CategorySeriesItem>

                </SeriesItems>

            </telerik:BarSeries>

            <telerik:BarSeries Name="Quarter 2">

                <Appearance>

                    <FillStyle BackgroundColor="#92b622"></FillStyle>

                </Appearance>

                <LabelsAppearance DataFormatString="${0}" Position="Center"></LabelsAppearance>

                <TooltipsAppearance BackgroundColor="#92b622" DataFormatString="${0}" Color="White"></TooltipsAppearance>

                <SeriesItems>

                    <telerik:CategorySeriesItem Y="360000"></telerik:CategorySeriesItem>

                    <telerik:CategorySeriesItem Y="540000"></telerik:CategorySeriesItem>

                    <telerik:CategorySeriesItem Y="735000"></telerik:CategorySeriesItem>

                </SeriesItems>

            </telerik:BarSeries>

            <telerik:BarSeries Name="Quarter 3">

                <Appearance>

                    <FillStyle BackgroundColor="#729021"></FillStyle>

                </Appearance>

                <LabelsAppearance DataFormatString="${0}" Position="Center"></LabelsAppearance>

                <TooltipsAppearance BackgroundColor="#729021" DataFormatString="${0}" Color="White"></TooltipsAppearance>

                <SeriesItems>

                    <telerik:CategorySeriesItem Y="405000"></telerik:CategorySeriesItem>

                    <telerik:CategorySeriesItem Y="600000"></telerik:CategorySeriesItem>

                    <telerik:CategorySeriesItem Y="780000"></telerik:CategorySeriesItem>

                </SeriesItems>

            </telerik:BarSeries>

        </Series>

        <Appearance>

            <FillStyle BackgroundColor="Transparent"></FillStyle>

        </Appearance>

        <XAxis AxisCrossingValue="0" Color="black" MajorTickType="Outside" MinorTickType="Outside"
            Reversed="false">

            <Items>

                <telerik:AxisItem LabelText="2003"></telerik:AxisItem>

                <telerik:AxisItem LabelText="2004"></telerik:AxisItem>

                <telerik:AxisItem LabelText="2005"></telerik:AxisItem>

            </Items>

            <LabelsAppearance DataFormatString="Year {0}" RotationAngle="0" Skip="0" Step="1"></LabelsAppearance>

            <TitleAppearance Position="Center" RotationAngle="0" Text="Years"></TitleAppearance>

        </XAxis>

        <YAxis AxisCrossingValue="0" Color="black" MajorTickSize="1" MajorTickType="Outside"
            MinorTickType="None" Reversed="false">

            <LabelsAppearance DataFormatString="${0}" RotationAngle="0" Skip="0" Step="1"></LabelsAppearance>

            <TitleAppearance Position="Center" RotationAngle="0" Text="Sum"></TitleAppearance>

        </YAxis>

    </PlotArea>

    <Appearance>

        <FillStyle BackgroundColor="Transparent"></FillStyle>

    </Appearance>

    <ChartTitle Text="Revenue">

        <Appearance Align="Center" BackgroundColor="Transparent" Position="Top"></Appearance>

    </ChartTitle>

    <Legend>

        <Appearance BackgroundColor="Transparent" Position="Bottom"></Appearance>

    </Legend>

</telerik:RadHtmlChart>
</div>