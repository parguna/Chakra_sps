<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccessRequestColumnChartByUser.ascx.cs" Inherits="DI.Chakra.Web.Dashboard.AccessRequestColumnChartByUser" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Dashboard/DashBoardHeader.ascx" TagPrefix="uc1" TagName="DashBoardHeaderUC" %>

<script type="text/javascript">
    function resizeAccessRequestSourceChart() {
        $find("<%=AccessRequestSourceColumnChart.ClientID%>").get_kendoWidget().resize();
    }

    if (window.addEventListener) {    // most non-IE browsers and IE9
        window.addEventListener("resize", resizeAccessRequestSourceChart, false);
    } else if (window.attachEvent) {  // Internet Explorer 5 or above
        window.attachEvent("onresize", resizeAccessRequestSourceChart);
    }
</script>

<uc1:DashBoardHeaderUC runat="server" ID="DashBoardHeaderUC" />
<div class="dashboard-box dashboard-content chart panel panel-default">
    <div class="form-group">
        <div class="col-xs-7">
            <asp:Label ID="lblYear" Text="Select Year" runat="server"></asp:Label>
            <asp:DropDownList ID="drpYear" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpYear_SelectedIndexChanged" CssClass="selectbox"></asp:DropDownList>
        </div>
    </div>
    <telerik:RadHtmlChart ID="AccessRequestSourceColumnChart" runat="server" Width="100%" Height="100%">

        <ChartTitle Text="Access Request By Month">
        </ChartTitle>
        <Legend>
            <Appearance Position="Bottom"></Appearance>
        </Legend>
        <PlotArea>
            <YAxis>
                <TitleAppearance Text="No. of Requests"></TitleAppearance>
            </YAxis>

            <XAxis DataLabelsField="MonthName"></XAxis>

            <Series>

                <telerik:ColumnSeries Name="AdhocRequest" DataFieldY="AdhocRequest" Gap="2" Spacing="1" Stacked="true">

                    <TooltipsAppearance DataFormatString="{0} AdhocRequest Request" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#26A9E0"></Appearance>
                </telerik:ColumnSeries>

                <telerik:ColumnSeries Name="OnBoarding" DataFieldY="OnBoarding">

                    <TooltipsAppearance DataFormatString="{0} OnBoarding Request" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#AEB536"></Appearance>
                </telerik:ColumnSeries>

                <telerik:ColumnSeries Name="OffBoarding" DataFieldY="OffBoarding">

                    <TooltipsAppearance DataFormatString="{0} OffBoarding Request" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#FE8442"></Appearance>
                </telerik:ColumnSeries>

                <telerik:ColumnSeries Name="OUChange" DataFieldY="OUChange">

                    <TooltipsAppearance DataFormatString="{0} OUChange Request" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#8BC53F"></Appearance>
                </telerik:ColumnSeries>

                <telerik:ColumnSeries Name="ReOrganisation" DataFieldY="ReOrganisation">

                    <TooltipsAppearance DataFormatString="{0} ReOrganisation Request" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#61B045"></Appearance>
                </telerik:ColumnSeries>

                <telerik:ColumnSeries Name="Employee Extension" DataFieldY="EmployeeExtension">

                    <TooltipsAppearance DataFormatString="{0} Employee Extension Request" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#0E6251"></Appearance>
                </telerik:ColumnSeries>
                <telerik:ColumnSeries Name="Attestation Remediation" DataFieldY="AttestationRemediation">

                    <TooltipsAppearance DataFormatString="{0} Attestation Remediation Request" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#1A5276"></Appearance>
                </telerik:ColumnSeries>
                <telerik:ColumnSeries Name="Employee Creation" DataFieldY="EmployeeCreation">

                    <TooltipsAppearance DataFormatString="{0} Employee Creation Request" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#F4D03F"></Appearance>
                </telerik:ColumnSeries>
                <telerik:ColumnSeries Name="Re-Hire" DataFieldY="Rehire">

                    <TooltipsAppearance DataFormatString="{0} Re-Hire Request" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#F39C12"></Appearance>
                </telerik:ColumnSeries>
                <telerik:ColumnSeries Name="Promotion" DataFieldY="Promotion">

                    <TooltipsAppearance DataFormatString="{0} Promotion Request" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#C39BD3"></Appearance>
                </telerik:ColumnSeries>

                <telerik:ColumnSeries Name="ExtendedRequest" DataFieldY="ExtendedRequest">

                    <TooltipsAppearance DataFormatString="{0} ExtendedRequest Request" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#0E6251"></Appearance>
                </telerik:ColumnSeries>
                <telerik:ColumnSeries Name="GamingApplicationRemoval" DataFieldY="GamingApplicationRemoval">

                    <TooltipsAppearance DataFormatString="{0} GamingApplicationRemoval Request" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#922B21"></Appearance>
                </telerik:ColumnSeries>
                <telerik:ColumnSeries Name="Off-BoardingDay2" DataFieldY="OffBoardingDay2">

                    <TooltipsAppearance DataFormatString="{0} Off-BoardingDay2 Request" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#EDBB99"></Appearance>
                </telerik:ColumnSeries>
                <telerik:ColumnSeries Name="Off-BoardingDay3" DataFieldY="OffBoardingDay3">

                    <TooltipsAppearance DataFormatString="{0} Off-BoardingDay3 Request" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#F6DDCC"></Appearance>
                </telerik:ColumnSeries>
                <telerik:ColumnSeries Name="Demotion" DataFieldY="Demotion">

                    <TooltipsAppearance DataFormatString="{0} Demotion Request" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#7F8C8D"></Appearance>
                </telerik:ColumnSeries>




            </Series>
            <XAxis AxisCrossingValue="0" Color="black" MajorTickType="Outside" MinorTickType="Outside"
                Reversed="false">

                <LabelsAppearance RotationAngle="0" Skip="0" Step="1"></LabelsAppearance>

                <TitleAppearance Position="Center" RotationAngle="0" Text="Months">
                </TitleAppearance>

            </XAxis>
        </PlotArea>

    </telerik:RadHtmlChart>
</div>