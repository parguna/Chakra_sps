<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SchedulerWeekly.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Scheduler.SchedulerWeekly" %>

<div id="divschedulerweek">
    <div class="form-group">
        <asp:Label ID="lblRecurMonth" Text="Recur every " runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtRecurWeeks"></asp:Label>
        <div class="col-xs-5">
            <asp:TextBox ID="txtRecurWeeks" runat="server" required="required" data-errMessage="Enter no of weeks to recur." onKeyPress="return AllowNumeric(event,1,7)" MaxLength="1" CssClass="col-xs-12"></asp:TextBox>
        </div>
        <asp:Label ID="Label2" runat="server" CssClass="col-xs-2 sched-lbl">week(s) on</asp:Label>
    </div>
    <div class="form-group">
        <div class="col-xs-2"></div>
        <div class="col-xs-5" id="chkweek">
            <div class="checkbox">
                <label>
                    <input type="checkbox" value="Sunday" id="chkSunday" runat="server">
                    <span class="lbl">Sunday</span>
                </label>
            </div>
            <div class="checkbox">
                <label>
                    <input type="checkbox" value="Monday" id="chkMonday" runat="server">
                    <span class="lbl">Monday</span>
                </label>
            </div>
            <div class="checkbox">
                <label>
                    <input type="checkbox" value="Tuesday" id="chkTuesday" runat="server">
                    <span class="lbl">Tuesday</span>
                </label>
            </div>
            <div class="checkbox">
                <label>
                    <input type="checkbox" value="Wednesday" id="chkWednesday" runat="server">
                    <span class="lbl">Wednesday</span>
                </label>
            </div>
        </div>
        <div class="col-xs-5">
            <div class="checkbox">
                <label>
                    <input type="checkbox" value="Thursday" id="chkThursday" runat="server">
                    <span class="lbl">Thursday</span>
                </label>
            </div>
            <div class="checkbox">
                <label>
                    <input type="checkbox" value="Friday" id="chkFriday" runat="server">
                    <span class="lbl">Friday</span>
                </label>
            </div>
            <div class="checkbox">
                <label>
                    <input type="checkbox" value="Saturday" id="chkSaturday" runat="server">
                    <span class="lbl">Saturday</span>
                </label>
            </div>
        </div>
    </div>
</div>