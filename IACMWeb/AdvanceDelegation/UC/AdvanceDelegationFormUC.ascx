<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdvanceDelegationFormUC.ascx.cs" Inherits="DI.Chakra.Web.AdvanceDelegation.UC.AdvanceDelegationFormUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="UserSearchUC" %>
<script type="text/javascript">
    function SetToday(Parameter) {
        var datepicker = $find(Parameter == "SButton" ? "<%=StartDate.ClientID%>" : "<%=EndDate.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);
        datepicker.hidePopup();
    }

    function OnDateSelected(sender, e) {
        var ValidFromPicker = $find("<%= StartDate.ClientID %>");
        var ValidToPicker = $find("<%= EndDate.ClientID %>");
        var now = new Date();
        if (ValidFromPicker != null && ValidToPicker != null) {
            ValidToPicker.clear();
            ValidFromPicker.set_minDate(new Date(now.getFullYear(), now.getMonth(), now.getDate()));
            now = e.get_newDate();
            if (now != null) {
                ValidToPicker.set_minDate(new Date(now.getFullYear(), now.getMonth(), now.getDate()));
            }
        }
    }
</script>
<div class="content-wrapper">
    <div class="form">
        <div class="form-horizontal left">
            <div class="col-xs-6">
                <div class="row-fluid">
                    <div class="form-group">
                        <label class="col-xs-4 control-label required">Delegate From</label>
                        <div class="col-xs-7">
                            <uc1:UserSearchUC runat="server" ID="DelegateFrom" Visible="true" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4 control-label"  required="required">Start Date</label>
                        <div class="col-xs-7">
                            <telerik:RadDatePicker runat="server" ID="StartDate" EnableTyping="false" Skin="IACMSkin" ShowPopupOnFocus="false" CssClass="form-control" DatePopupButton-Visible="true">
                                <ClientEvents OnDateSelected="OnDateSelected"></ClientEvents>
                                <Calendar ID="Calendar2" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                    <FooterTemplate>
                                        <div>
                                            <asp:Button ID="VFButton" runat="server" Width="100%" Text="Today" OnClientClick="SetToday('SButton')" />
                                        </div>
                                    </FooterTemplate>
                                </Calendar>
                                <DateInput ID="DateInput2" ReadOnly="false"  runat="server" ToolTip="Date input For Start Date"
                                    BorderStyle="NotSet" Label="">
                                </DateInput>
                            </telerik:RadDatePicker>

                        </div>
                    </div>
                    <div class="form-group" runat="server" visible="true">
                        <label class="col-xs-4 control-label">Delegate Attestation Tasks</label>
                        <div class="col-xs-7">
                            <asp:CheckBox ID="Attestation" runat="server" Checked="true"></asp:CheckBox>
                            <span class="lbl"></span>
                        </div>
                    </div>
                    <div class="form-group" runat="server" visible="true">
                        <label class="col-xs-4 control-label">Delegate Provisioning Tasks</label>
                        <div class="col-xs-7">
                            <asp:CheckBox ID="Provisioning" runat="server" Checked="true"></asp:CheckBox>
                            <span class="lbl"></span>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4 control-label">Is Active</label>
                        <div class="col-xs-7">
                            <asp:CheckBox ID="IsActive" runat="server" Checked="true"></asp:CheckBox>
                            <span class="lbl"></span>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-xs-6">
                <div class="form-group">
                    <label class="col-xs-4 control-label"  required="required">Delegate To</label>
                    <div class="col-xs-7">
                        <uc1:UserSearchUC runat="server" ID="DelegateTo" Visible="true" />
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-xs-4 control-label"  required="required">End Date</label>
                    <div class="col-xs-7">
                        <telerik:RadDatePicker runat="server" ID="EndDate" Skin="IACMSkin" EnableTyping="false" ShowPopupOnFocus="false" CssClass="form-control"
                            DatePopupButton-Visible="true">
                            <Calendar ID="Calendar3" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                <FooterTemplate>
                                    <div>
                                        <asp:Button ID="VTButton" runat="server" Width="100%" Text="Today" OnClientClick="SetToday('EButton')" />
                                    </div>
                                </FooterTemplate>
                            </Calendar>
                            <DateInput ID="DateInput3" ReadOnly="false"  runat="server" ToolTip="Date input For End Date"
                                BorderStyle="NotSet" Label="">
                            </DateInput>
                        </telerik:RadDatePicker>
                    </div>
                </div>
                <div class="row-fluid">
                    <div class="form-group required">
                        <label class="col-xs-4 control-label">Comments</label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="Comment" runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                            <span class="lbl"></span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="buttons text-right nav-rectangle topBuffer">
     <asp:Button runat="server" ID="CancelButton" Text="Cancel" CssClass="btn btn-default btn-sm cancel" OnClick="CancelButton_Click" />
     <asp:Button runat="server" ID="SaveButton" Text="Save" CssClass="btn btn-default btn-sm validation" OnClick="SaveButton_Click" />
</div>