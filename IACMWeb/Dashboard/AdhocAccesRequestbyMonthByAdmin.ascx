<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocAccesRequestbyMonthByAdmin.ascx.cs" Inherits="DI.Chakra.Web.Dashboard.AdhocAccesRequestbyMonthByAdmin" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Dashboard/DashBoardHeader.ascx" TagPrefix="uc1" TagName="DashBoardHeaderUC" %>

<script type="text/javascript">
    function resizeAdhocChart() {
        $find("<%=AdhocRequestColumnChart.ClientID%>").get_kendoWidget().resize();
    }

    if (window.addEventListener) {    // most non-IE browsers and IE9
        window.addEventListener("resize", resizeAdhocChart, false);
    } else if (window.attachEvent) {  // Internet Explorer 5 or above
        window.attachEvent("onresize", resizeAdhocChart);
    }
</script>

<uc1:DashBoardHeaderUC runat="server" ID="DashBoardHeaderUC" />

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

    <telerik:RadHtmlChart ID="AdhocRequestColumnChart" runat="server" Height="100%" Width="100%">

        <ChartTitle Text="Access Request By Account Type">
        </ChartTitle>
        <Legend>
            <Appearance Position="Bottom"></Appearance>
        </Legend>
        <PlotArea>
            <YAxis>
                <TitleAppearance Text="No. of Requests"></TitleAppearance>
            </YAxis>

            <XAxis DataLabelsField="AccountType"></XAxis>

            <Series>

                <telerik:ColumnSeries Name="Create Login-ID" DataFieldY="NewAccount">
                    <TooltipsAppearance DataFormatString="{0} New" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                </telerik:ColumnSeries>

                <telerik:ColumnSeries Name="Add Role" DataFieldY="AmendmentAdd">
                    <TooltipsAppearance DataFormatString="{0} Amendment Add" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                </telerik:ColumnSeries>

                <telerik:ColumnSeries Name="Remove Role" DataFieldY="AmendmentRemove">
                    <TooltipsAppearance DataFormatString="{0} Amendment Remove" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                </telerik:ColumnSeries>

                <telerik:ColumnSeries Name="Remove Login-ID" DataFieldY="DeleteAccount">
                    <TooltipsAppearance DataFormatString="{0} Delete" Color="White"></TooltipsAppearance>
                    <LabelsAppearance Position="Center"></LabelsAppearance>
                </telerik:ColumnSeries>
            </Series>
            <XAxis AxisCrossingValue="0" Color="black" MajorTickType="Outside" MinorTickType="Outside"
                Reversed="false">

                <LabelsAppearance RotationAngle="0" Skip="0" Step="1"></LabelsAppearance>

                <TitleAppearance Position="Center" RotationAngle="0" Text="Account Type">
                </TitleAppearance>

            </XAxis>
        </PlotArea>

    </telerik:RadHtmlChart>
</div>
