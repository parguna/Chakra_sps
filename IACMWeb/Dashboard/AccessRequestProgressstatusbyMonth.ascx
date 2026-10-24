<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccessRequestProgressstatusbyMonth.ascx.cs" Inherits="DI.Chakra.Web.Dashboard.AccessRequestProgressstatusbyMonth" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Dashboard/DashBoardHeader.ascx" TagPrefix="uc1" TagName="DashBoardHeader" %>

<script type="text/javascript">
    function resizeAccessRequestProgressChart() {
        $find("<%=AccessRequestByProgressStatus.ClientID%>").get_kendoWidget().resize();
    }

    if (window.addEventListener) {    // most non-IE browsers and IE9
        window.addEventListener("resize", resizeAccessRequestProgressChart, false);
    } else if (window.attachEvent) {  // Internet Explorer 5 or above
        window.attachEvent("onresize", resizeAccessRequestProgressChart);
    }
</script>

<uc1:DashBoardHeader runat="server" ID="DashBoardHeaderUC" />
<div class="dashboard-box dashboard-content chart panel panel-default">

<div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left">
                <div class="row-fluid">
                    <div class="col-xs-12">
                        <div class="form-group">
                            <div class="col-xs-6">
                                <asp:Label ID="lblYear" Text="Select Year" runat="server" CssClass="col-xs-5 control-label" AssociatedControlID="drpYear"></asp:Label>
                                <div class="col-xs-7">
                                    <asp:DropDownList ID="drpYear" runat="server" AutoPostBack="true" CssClass="form-control selectpicker" OnSelectedIndexChanged="drpYear_SelectedIndexChanged" data-width="100%"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-xs-6">
                                <asp:Label ID="lblMonth" Text="Select Month" runat="server" CssClass="col-xs-5 control-label" AssociatedControlID="drpMonth"></asp:Label>
                                <div class="col-xs-7">
                                    <asp:DropDownList ID="drpMonth" runat="server" AutoPostBack="true" CssClass="form-control selectpicker" OnSelectedIndexChanged="drpMonth_SelectedIndexChanged" data-width="100%"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <telerik:RadHtmlChart ID="AccessRequestByProgressStatus" runat="server" Width="100%" Height="100%">

        <ChartTitle Text="Access Request Progress Status By Month">
        </ChartTitle>
        <Legend>
            <Appearance Position="Bottom"></Appearance>
        </Legend>
        <PlotArea>
            <YAxis>
                <TitleAppearance Text="No. of Requests"></TitleAppearance>
            </YAxis>

            <XAxis DataLabelsField="ProgressStatus"></XAxis>

            <Series>

                <telerik:ColumnSeries Name="Waiting for Approval" DataFieldY="WaitingforApproval" Gap="2" Spacing="1">

                    <TooltipsAppearance DataFormatString="{0} Waiting for Approval" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#D6DE23"></Appearance>
                </telerik:ColumnSeries>                

                <telerik:ColumnSeries Name="Ready to Execute" DataFieldY="ReadytoExecute">

                    <TooltipsAppearance DataFormatString="{0} Ready to Execute" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#AEB536"></Appearance>
                </telerik:ColumnSeries>

                <telerik:ColumnSeries Name="Executing" DataFieldY="Executing">

                    <TooltipsAppearance DataFormatString="{0} Executing" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#8BC53F"></Appearance>
                </telerik:ColumnSeries>

                <telerik:ColumnSeries Name="Completed" DataFieldY="Completed">

                    <TooltipsAppearance DataFormatString="{0} Completed" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#61B045"></Appearance>
                </telerik:ColumnSeries>

                <telerik:ColumnSeries Name="Withdrawn" DataFieldY="Withdrawn">

                    <TooltipsAppearance DataFormatString="{0} Withdrawn" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#26A9E0"></Appearance>
                </telerik:ColumnSeries>

                <telerik:ColumnSeries Name="Request Rejected" DataFieldY="RequestRejected">

                    <TooltipsAppearance DataFormatString="{0} Request Rejected" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#CC0066"></Appearance>
                </telerik:ColumnSeries>

                <telerik:ColumnSeries Name="Execution Failed" DataFieldY="ExecutionFailed">

                    <TooltipsAppearance DataFormatString="{0} Execution Failed" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#B0051C"></Appearance>
                </telerik:ColumnSeries>

                <telerik:ColumnSeries Name="Validation Failed" DataFieldY="ValidationFailed">

                    <TooltipsAppearance DataFormatString="{0} Validation Failed" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#D72C43"></Appearance>
                </telerik:ColumnSeries>
                
                <telerik:ColumnSeries Name="ADID Construction Failed" DataFieldY="ADIDConstructionFailed">

                    <TooltipsAppearance DataFormatString="{0} ADID Construction Failed" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#FE8442"></Appearance>
                </telerik:ColumnSeries>

                <telerik:ColumnSeries Name="Waiting for Implementation" DataFieldY="WaitingforImplementation">

                    <TooltipsAppearance DataFormatString="{0} Waiting for Implementation" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#633974"></Appearance>
                </telerik:ColumnSeries>

                <telerik:ColumnSeries Name="None" DataFieldY="None">

                    <TooltipsAppearance DataFormatString="{0} None" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                    <Appearance FillStyle-BackgroundColor="#CCE5FF"></Appearance>
                </telerik:ColumnSeries>

            </Series>
            <XAxis AxisCrossingValue="0" Color="black" MajorTickType="Outside" MinorTickType="Outside"
                Reversed="false">

                <LabelsAppearance RotationAngle="0" Skip="0" Step="1"></LabelsAppearance>

                <TitleAppearance Position="Center" RotationAngle="0" Text="Progress Status">
                </TitleAppearance>

            </XAxis>
        </PlotArea>

    </telerik:RadHtmlChart>
</div>
