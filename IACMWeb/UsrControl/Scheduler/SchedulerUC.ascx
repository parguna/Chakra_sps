<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SchedulerUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Scheduler.SchedulerUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register Src="~/UsrControl/Scheduler/SchedulerDaily.ascx" TagPrefix="uc1" TagName="SchedulerDailyUC" %>
<%@ Register Src="~/UsrControl/Scheduler/SchedulerWeekly.ascx" TagPrefix="uc1" TagName="SchedulerWeeklyUC" %>
<%@ Register Src="~/UsrControl/Scheduler/SchedulerMonthly.ascx" TagPrefix="uc1" TagName="SchedulerMonthlyUC" %>

<script type="text/javascript">
    function validatedays() {
        var Recurdays = 0;
        var Total = 0;
        Recurdays = $('input[id$="txtRecurWeeks"]').val();
        $('#divschedulerweek input[type="checkbox"]').each(function () {
            if ($(this).prop('checked')) Total++;
        });
        if (Recurdays >= Total) return true;
        else return false;
    }

    function startChange() {
        var startTime = start.value();

        if (startTime) {
            startTime = new Date(startTime);

            end.max(startTime);

            startTime.setMinutes(startTime.getMinutes() + this.options.interval);

            end.min(startTime);
            end.value(startTime);
        }
    }

    function DateChanged(selecteddate) {

        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0!
        var yyyy = today.getFullYear();
        if (dd < 10) {
            dd = '0' + dd
        }
        if (mm < 10) {
            mm = '0' + mm
        }
        var today = dd + '-' + mm + '-' + yyyy;
        var todaysDate = new Date();
        if (today != selecteddate.value) {
            $('#cplMain_AttestationWizardUC_SchedulerInfo_Kendotimepicker').kendoTimePicker({ min: new Date(todaysDate.getYear(), todaysDate.getMonth(), todaysDate.getDay(), 0, 30, 0) });
        }
        if (today == selecteddate.value) {
            $('#cplMain_AttestationWizardUC_SchedulerInfo_Kendotimepicker').kendoTimePicker({ min: new Date(todaysDate.getYear(), todaysDate.getMonth(), todaysDate.getDay(), todaysDate.getHours(), 30, 0) });
        }
    }
    function SetToday(Parameter) {
        var datepicker = $find(Parameter == "todayDTStartDate" ? "<%=dtStartDate.ClientID%>" : "<%=dtStartDate.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);
        datepicker.hidePopup();
    }

    function checkEmptyDate() {

    }
</script>

<div class="content-wrapper" id="contentWrapper" runat="server">
    <div class="form">
        <div class="form-horizontal left readonly">
            <div class="row-fluid">
                <div class="col-xs-2">
                    <asp:UpdatePanel ID="upUpdatePanelOne" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                            <div class="form-group">
                                <div class="radio">
                                    <asp:Label ID="Label1" runat="server" AssociatedControlID="rblOneTime">
                                        <asp:RadioButton runat="server" ID="rblOneTime" AutoPostBack="true" GroupName="Map" OnCheckedChanged="rblOneTime_CheckedChanged"></asp:RadioButton>
                                        <asp:Label CssClass="lbl" ID="Label2" runat="server" Text="One Time"></asp:Label>
                                    </asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="radio">
                                    <asp:Label ID="Label3" runat="server" AssociatedControlID="rblDaily">
                                        <asp:RadioButton runat="server" ID="rblDaily" AutoPostBack="true" GroupName="Map" OnCheckedChanged="rblDaily_CheckedChanged"></asp:RadioButton>
                                        <asp:Label CssClass="lbl" ID="Label4" runat="server" Text="Daily"></asp:Label>
                                    </asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="radio">
                                    <asp:Label ID="Label6" runat="server" AssociatedControlID="rblWeekly">
                                        <asp:RadioButton runat="server" ID="rblWeekly" AutoPostBack="true" GroupName="Map" OnCheckedChanged="rblWeekly_CheckedChanged"></asp:RadioButton>
                                        <asp:Label CssClass="lbl" ID="Label7" runat="server" Text="Weekly"></asp:Label>
                                    </asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="radio">
                                    <asp:Label ID="Label8" runat="server" AssociatedControlID="rblMonthly">
                                        <asp:RadioButton runat="server" ID="rblMonthly" AutoPostBack="true" GroupName="Map" OnCheckedChanged="rblMonthly_CheckedChanged"></asp:RadioButton>
                                        <asp:Label CssClass="lbl" ID="Label9" runat="server" Text="Monthly"></asp:Label>
                                    </asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="radio">
                                    <asp:Label ID="Label5" runat="server" AssociatedControlID="rblRunNow">
                                        <asp:RadioButton runat="server" ID="rblRunNow" AutoPostBack="true" GroupName="Map" OnCheckedChanged="rblRunNow_CheckedChanged"></asp:RadioButton>
                                        <asp:Label CssClass="lbl" ID="Label10" runat="server" Text="Run Now"></asp:Label>
                                    </asp:Label>
                                </div>
                            </div>                            
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="col-xs-7">
                    <div class="form-group">
                        <asp:Label ID="lblStart" runat="server" Text="Start " CssClass="col-xs-2 control-label" AssociatedControlID="dtStartDate"></asp:Label>
                        <div class="col-xs-5">
                            <telerik:RadDateTimePicker DateInput-ReadOnly="true" runat="server" ID="dtStartDate" Skin="IACMSkin" Width="100%" ShowPopupOnFocus="true" DatePopupButton-Visible="true" Enabled="true" CssClass="form-control" required="required">
                                <Calendar ID="Calendar1" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                    <FooterTemplate>
                                        <div>
                                            <asp:Button ID="todayDTStartDate" runat="server" Width="100%" Text="Today" OnClientClick="SetToday('todayDTStartDate')" />
                                        </div>
                                    </FooterTemplate>
                                </Calendar>
                                <TimeView Interval="00:30:00"></TimeView>
                            </telerik:RadDateTimePicker>
                        </div>

                    </div>
                    <asp:UpdatePanel ID="upUpdatePanelTwo" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Panel ID="pnlTaskScheduler" runat="server" Visible="true">
                            </asp:Panel>
                            <asp:Panel ID="pnlDaily" runat="server" Visible="false">
                                <uc1:SchedulerDailyUC runat="server" ID="SchedulerDailyUC" />
                            </asp:Panel>
                            <asp:Panel ID="pnlWeekly" runat="server" Visible="false">
                                <uc1:SchedulerWeeklyUC runat="server" ID="SchedulerWeeklyUC" />
                            </asp:Panel>
                            <asp:Panel ID="pnlMonthly" runat="server" Visible="false">
                                <uc1:SchedulerMonthlyUC runat="server" ID="SchedulerMonthlyUC" />
                            </asp:Panel>                            
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="rblOneTime" EventName="CheckedChanged" />
                            <asp:AsyncPostBackTrigger ControlID="rblDaily" EventName="CheckedChanged" />
                            <asp:AsyncPostBackTrigger ControlID="rblWeekly" EventName="CheckedChanged" />
                            <asp:AsyncPostBackTrigger ControlID="rblMonthly" EventName="CheckedChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</div>

<script type="text/javascript">

</script>