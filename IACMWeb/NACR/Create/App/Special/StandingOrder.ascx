<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StandingOrder.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.Special.StandingOrder" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<script type="text/javascript">
    function SetToday(Parameter) {
        var datepicker = $find("<%=DatePickerValidTo.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);
        datepicker.hidePopup();
    }

    function OnDateSelected(sender, e) {
        var ValidFromPicker = null; //
        var ValidToPicker = $find("<%= DatePickerValidTo.ClientID %>");
        var now = new Date();
        if (ValidFromPicker != null && ValidToPicker != null) {
            ValidToPicker.clear();
            now = e.get_newDate() > now ? e.get_newDate() : now;            
            if (now != null) {
                ValidToPicker.set_minDate(new Date(now.getFullYear(), 1, 1));
            }
        }
    }

</script>

<div class="form-group" id="selectEndpoint" runat="server">
    <label class="col-xs-3 control-label required">Subsytem Name</label>
    <div class="col-xs-9">
        <asp:DropDownList ID="ApplicationTypeDropDownList" runat="server" CssClass="form-control selectpicker" CausesValidation="true" data-width="100%">
        </asp:DropDownList>
    </div>
</div>
<div class="form-group">
    <label class="col-xs-3 control-label required">Status</label>
    <div class="col-xs-9">
        <asp:TextBox ID="txtReferenceNumber" runat="server" CssClass="form-control" MaxLength="255" TextMode="SingleLine" Rows="3"></asp:TextBox>
    </div>
</div>
<div class="form-group">
    <label class="col-xs-3 control-label required">Approved Date</label>
    <div class="col-xs-9">
        <telerik:RadDatePicker runat="server" ID="DatePickerValidTo" onkeydown="handleDateTimePickerClickEvent(this,event)" EnableTyping="false" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
            DatePopupButton-Visible="true" Width="100%" CssClass="form-control" DateInput-CausesValidation="true">
            <Calendar ID="Calendar2" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                <FooterTemplate>
                    <div style="width: 100%; text-align: center; background-color: Gray;">
                        <asp:Button ID="EButton" runat="server" Width="100%" Text="Today" OnClientClick="SetToday('VTButton')" />

                    </div>
                </FooterTemplate>
            </Calendar>
            <DateInput ID="DateInput2" ReadOnly="false" runat="server" ToolTip="Date input For Approved Date"
                BorderStyle="NotSet" Label="">
            </DateInput>
        </telerik:RadDatePicker>
    </div>
</div>
<div class="form-group">
    <label class="col-xs-3 control-label required">Description</label>
    <div class="col-xs-9">
        <asp:TextBox ID="SODescription" runat="server" CssClass="form-control" MaxLength="255" TextMode="MultiLine" Rows="3"></asp:TextBox>
    </div>
</div>
