<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ADAccountInfoUC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.Special.ADAccountInfoUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/NACR/Create/App/Special/DefaultAccountInfoUC.ascx" TagPrefix="uc1" TagName="DefaultAccountInfoUC" %>
<%@ Register Src="~/UsrControl/Common/GenericAutoCompleteBox.ascx" TagPrefix="uc1" TagName="GenericAutoCompleteBox" %>

<script type="text/javascript">
    function SetToday(Parameter) {
        var datepicker = $find("<%=ValidToPicker.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);
        datepicker.hidePopup();
    }   
        
    
    function OnDateSelected(sender, e) {
        var ValidToPicker = $find("<%= ValidToPicker.ClientID %>");
        var now = new Date();
        if (ValidToPicker != null) {
            ValidToPicker.clear();
            now = e.get_newDate();
            if (now != null) {
                ValidToPicker.set_minDate(new Date(now.getFullYear(), now.getMonth(), now.getDate()));
            }
        }
    }



    </script>

<uc1:DefaultAccountInfoUC runat="server" ID="DefaultAccountInfoUC" />
<div class="form-group">
    <label class="col-xs-3 control-label required">OU</label>
    <div class="col-xs-9">
        <uc1:GenericAutoCompleteBox runat="server" ID="OUAutoCompleteBox" ServiceMethodName="GetADOrganisationUnits" ServicePath="~/Services/AutoCompleteDataService.svc" />
    </div>
</div>
<asp:Panel ID="RequireClass" runat="server" CssClass="form-group">
    <label class="col-xs-3 control-label">Account Expires</label>
    <div class="col-xs-9">
        <telerik:RadDatePicker runat="server" ID="ValidToPicker" ShowPopupOnFocus="false" onkeydown="handleDateTimePickerClickEvent(this,event)" EnableTyping="false" Skin="IACMSkin" CssClass="form-control"
            DatePopupButton-Visible="true">
            <Calendar ID="Calendar1" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                <FooterTemplate>
                    <div>
                        <asp:Button ID="VTButton" runat="server" Width="100%" Text="Today" OnClientClick="SetToday('VTButton')" />
                    </div>
                </FooterTemplate>
            </Calendar>
            <DateInput ID="DateInput1" ReadOnly="false" runat="server" ToolTip="Date input For Valid To"
                BorderStyle="NotSet" Label="">
            </DateInput>
        </telerik:RadDatePicker>
    </div>
</asp:Panel>
<div class="form-group" id="divPasswordNever" runat="server">
    <asp:Label AssociatedControlID="chkPasswordExpires" CssClass="col-xs-3" Text="Password Never Expires" runat="server"></asp:Label>
    <div class="col-xs-9">
        <asp:CheckBox ID="chkPasswordExpires" runat="server" AutoPostBack="true" OnCheckedChanged="chkPasswordExpires_CheckedChanged" />
    </div>
</div>
