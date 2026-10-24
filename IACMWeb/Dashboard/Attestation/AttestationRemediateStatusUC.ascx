<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttestationRemediateStatusUC.ascx.cs" Inherits="DI.Chakra.Web.Dashboard.Attestation.AttestationRemediateStatusUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%--<%@ Register Src="~/Dashboard/Attestation/AttestationTypeUC.ascx" TagPrefix="uc1" TagName="AttestationTypeUC" %>--%>
<%@ Register Src="~/Dashboard/DashBoardHeader.ascx" TagPrefix="uc1" TagName="DashBoardHeaderUC" %>
<%@ Register Src="~/Dashboard/Attestation/NoDataUC.ascx" TagPrefix="uc1" TagName="NoDataUC" %>

<script type="text/javascript">
    function resizeRemediateChart() {
        $find("<%=TaskSummaryPie.ClientID%>").get_kendoWidget().resize();
    }

    if (window.addEventListener) {    // most non-IE browsers and IE9
        window.addEventListener("resize", resizeRemediateChart, false);
    } else if (window.attachEvent) {  // Internet Explorer 5 or above
        window.attachEvent("onresize", resizeRemediateChart);
    }
</script>

<uc1:DashBoardHeaderUC runat="server" ID="DashBoardHeaderUC" />
<div id="divDdlType" runat="server">
    <asp:Label runat="server" ID="lblAttestationType" Text="Select Attestation Type :">
    </asp:Label>
    <asp:DropDownList ID="ddlAttestationType" runat="server" OnSelectedIndexChanged="ddlAttestationType_SelectedIndexChanged" Height="36px" Width="185px"></asp:DropDownList>
</div>
<div class="dashboard-box dashboard-content chart panel panel-default" id="divChartControl" runat="server">

    <telerik:RadHtmlChart runat="server" ID="TaskSummaryPie" Width="500" Height="500">
        <ChartTitle Text="">
            <Appearance Align="Center" Position="Top">
            </Appearance>
        </ChartTitle>
        <Legend>
            <Appearance Position="Right" Visible="true">
            </Appearance>
        </Legend>
        <PlotArea>
            <Series>
                <telerik:DonutSeries StartAngle="90" NameField="Status" DataFieldY="StatusCount" ExplodeField="IsExploded" ColorField="BGColor">
                    <LabelsAppearance Position="OutsideEnd" DataFormatString="{0}">
                    </LabelsAppearance>
                    <TooltipsAppearance Color="White" DataFormatString="{0} "></TooltipsAppearance>

                </telerik:DonutSeries>
            </Series>
        </PlotArea>
    </telerik:RadHtmlChart>
</div>
<div id="divNoData" runat="server">
    <uc1:NoDataUC runat="server" ID="NoDataUC" />
</div>



