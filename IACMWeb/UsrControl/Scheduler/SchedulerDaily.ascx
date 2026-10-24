<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SchedulerDaily.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Scheduler.SchedulerDaily" %>

<div class="form-group">
    <asp:Label ID="lblRecurDay" Text="Recur every " runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtRecurDay"></asp:Label>
    <div class="col-xs-5">
        <asp:TextBox ID="txtRecurDay" runat="server" required="required" data-errMessage="Enter no of days to recur." onKeyPress="return AllowNumeric(event,0,9)" CssClass="col-xs-12"></asp:TextBox>
    </div>
    <asp:Label ID="lbldays" Text="day(s)" runat="server" CssClass="col-xs-2 sched-lbl"></asp:Label>
</div>