<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SchedulerMonthly.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Scheduler.SchedulerMonthly" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="form-group">
    <asp:Label ID="lblRecurMonth" Text="Recur every " runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="txtRecurMonth"></asp:Label>
    <div class="col-xs-5">
        <asp:TextBox ID="txtRecurMonth" runat="server" required="required" data-errMessage="Enter no of months to recur." onKeyPress="return AllowNumeric(event,0,9)" onblur="return AllowNumberupto(this, 12)" CssClass="col-xs-12"></asp:TextBox>
    </div>
    <asp:Label ID="lblMonth" Text="month(s)" runat="server" CssClass="col-xs-2 sched-lbl"></asp:Label>
</div>