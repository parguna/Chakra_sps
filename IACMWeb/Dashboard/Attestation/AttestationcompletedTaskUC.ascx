<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttestationcompletedTaskUC.ascx.cs" Inherits="DI.Chakra.Web.Dashboard.Attestation.AttestationcompletedTaskUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Dashboard/Attestation/AttestationTypeUC.ascx" TagPrefix="uc1" TagName="AttestationTypeUC" %>
<%@ Register Src="~/Dashboard/DashBoardHeader.ascx" TagPrefix="uc1" TagName="DashBoardHeaderUC" %>

<uc1:DashBoardHeaderUC runat="server" ID="DashBoardHeaderUC" />

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



<div class="dashboard-box dashboard-content chart panel panel-default">
    <uc1:AttestationTypeUC runat="server" ID="AttestationTypeUC" />
    
    <table>
        <tr>
            <td style="padding-left: 90px">

                <telerik:RadRadialGauge runat="server" ID="ragTaskCompleteness" Width="50%" Height="50%" Transitions="true">

                    <Pointer Color="#FE8442">
                        <Cap Color="#FE8442" Size="0.05" />
                    </Pointer>

                </telerik:RadRadialGauge>
                <div style="width:inherit;">
                    <div style="float:left;width:30%;">Completed</div>
                    <div style="float:left;text-align:center;width:40%;"><asp:Label runat="server" ID="lblLevel" CssClass="control-label" /></div>
                    <div style="float:right;text-align:right;width:30%;">Imcomplete</div>
                </div>
            </td>
            <td style="padding-left: 90px">

                <div>
                    <asp:Label runat="server" ID="lblEndDate" CssClass="control-label" />
                </div>
                <telerik:RadLinearGauge runat="server" ID="RadLinearGauge1" Width="150px" Height="250px" Transitions="true">
                </telerik:RadLinearGauge>
                <div>
                    <asp:Label runat="server" ID="lblStartDate" CssClass="control-label" />
                </div>
            </td>
            <td></td>
        </tr>
    </table>
    
</div>
