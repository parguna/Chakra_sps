<%@ Page Language="C#" Title="Advance Delegation" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="AddAdvance.aspx.cs" Inherits="DI.Chakra.Web.Delegation.AddAdvance" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="UserSearchUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
      <script type="text/javascript">
          function SetToday(Parameter) {
              var datepicker = $find(Parameter == "SButton" ? "<%=StartDate.ClientID%>" : "<%=EndDate.ClientID%>");
            var dt = new Date();
            datepicker.set_selectedDate(dt);
            datepicker.hidePopup();
        }
    </script>
    <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left">
                <div class="row-fluid">
                    <div class="col-xs-8">
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Delegate From</label>
                            <div class="col-xs-6">
                                <uc1:UserSearchUC runat="server" ID="DelegateFrom" Visible="true" />
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-8">
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Delegate To</label>
                            <div class="col-xs-6">
                                <uc1:UserSearchUC runat="server" ID="DelegateTo" Visible="true" />
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-8">
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Start Date</label>
                            <div class="col-xs-6">
                                <telerik:RadDatePicker runat="server" ID="StartDate" onkeydown="handleDateTimePickerClickEvent(this,event)"  ShowPopupOnFocus="false"
                                    DatePopupButton-Visible="true">
                                    <Calendar ID="Calendar3" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                        <FooterTemplate>
                                            <div style="width: 100%; text-align: center; background-color: Gray;">
                                                <asp:Button ID="SButton" runat="server" Width="100%" Text="Today"  OnClientClick="SetToday('SButton')" />
                                            </div>
                                        </FooterTemplate>
                                    </Calendar>
                                    <DateInput ID="DateInput3" ReadOnly="false" runat="server" ToolTip="Date input For Start Date"
                                         BorderStyle="NotSet" Label="">
                                    </DateInput>
                                </telerik:RadDatePicker>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-8">
                        <div class="form-group">
                            <label class="col-xs-2 control-label">End Date</label>
                            <div class="col-xs-6">
                                <telerik:RadDatePicker runat="server" ID="EndDate" onkeydown="handleDateTimePickerClickEvent(this,event)"  ShowPopupOnFocus="false"
                                    DatePopupButton-Visible="true">
                                    <Calendar runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                        <FooterTemplate>
                                            <div style="width: 100%; text-align: center; background-color: Gray;">
                                                <asp:Button ID="EButton"  runat="server" Width="100%" Text="Today" OnClientClick="SetToday('EButton')" />
                                            </div>
                                        </FooterTemplate>
                                    </Calendar>
                                    <DateInput runat="server" ReadOnly="false" ToolTip="Date input For End Date"
                                         BorderStyle="NotSet" Label="">
                                    </DateInput>
                                </telerik:RadDatePicker>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-8">
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Delegate Attestation Tasks</label>
                            <div class="col-xs-6">
                                <asp:CheckBox ID="Attestation" runat="server" Checked="true"></asp:CheckBox>
                                <span class="lbl"></span>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-8">
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Delegate Provisioning Tasks</label>
                            <div class="col-xs-6">
                                <asp:CheckBox ID="Provisioning" runat="server" Checked="true"></asp:CheckBox>
                                <span class="lbl"></span>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-8">
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Comments</label>
                            <div class="col-xs-6">
                                <asp:TextBox ID="Comment" runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                <span class="lbl"></span>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-8">
                        <div class="form-group">
                            <label class="col-xs-2 control-label">Is Active</label>
                            <div class="col-xs-6">
                                <asp:CheckBox ID="IsActive" runat="server" Checked="true"></asp:CheckBox>
                                <span class="lbl"></span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="buttons text-right nav-rectangle topBuffer">
        <asp:Button runat="server" ID="SaveButton" Text="Save" CssClass="btn btn-default btn-sm validation" OnClick="SaveButton_Click" />
        <asp:Button runat="server" ID="CancelButton" Text="Cancel" CssClass="btn btn-default btn-sm cancel" OnClick="CancelButton_Click" />
    </div>
</asp:Content>
