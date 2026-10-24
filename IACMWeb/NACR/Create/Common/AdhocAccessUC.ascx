<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocAccessUC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.Common.AdhocAccessUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/UsrControl/Common/UserInfoUC.ascx" TagPrefix="uc1" TagName="UserInfoUC" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="UserSearchUC" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc2" TagName="UserSearchUC2" %>
<script type="text/javascript">
    function SetToday(Parameter) {
        var datepicker = $find(Parameter == "VFButton" ? "<%=ValidFromPicker.ClientID%>" : "<%=ValidToPicker.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);
        datepicker.hidePopup();
    }


    function OnDateSelected(sender, e) {
        
        var ValidFromPicker = $find("<%=ValidFromPicker.ClientID %>");
        var ValidToPicker = $find("<%=ValidToPicker.ClientID %>");
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
<style>
    @media only screen and (max-width: 1500px) {
       #divAccess1 span, #divAccess2 span {
            padding-right: 0px !important;
            font-size: 12px;
        }
    }

    @media only screen and (max-width: 1390px) {
        #divAccess1 span, #divAccess2 span {
            padding-right: 0px !important;
            font-size: 11px;
        }
    }

    @media only screen and (max-width: 1276px) {
      #divAccess1 span, #divAccess2 span {
            padding-right: 0px !important;
            font-size: 10px;
        }
    }

    @media only screen and (max-width: 1200px) {
       #divAccess1 span, #divAccess2 span {
            padding-right: 0px !important;
            font-size: 9px;
        }
    }
</style>
<div class="content-wrapper">
    <div class="form">
        <div class="form-horizontal left clearfix">
            <div class="row-fluid">
                <asp:HiddenField ID="serverDateTime" runat="server" />
                <div class="col-xs-12">
                    <div class="form-group">
                        <asp:Label ID="Label1" AssociatedControlID="SelfRadioButton" CssClass="col-xs-3" runat="server"><%= T("App.ACR.AccessRequestFor") %></asp:Label>
                        <div class="col-xs-5">
                            <asp:RadioButton ID="SelfRadioButton" runat="server" AutoPostBack="true" Checked="True" GroupName="UserSelect" OnCheckedChanged="AccessRequestFor" />
                            <span class="lbl"><%= T("App.ACR.Self") %></span>
                            <asp:RadioButton ID="OthersRadioButton" runat="server" AutoPostBack="true" GroupName="UserSelect" OnCheckedChanged="AccessRequestFor" />
                            <span class="lbl"><%= T("App.ACR.Others") %></span>
                            <uc1:UserInfoUC runat="server" ID="UserInfoUC" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-3 control-label"><%= T("App.ACR.UserName") %></label>
                        <div class="col-xs-5">
                            <uc1:UserSearchUC runat="server" ID="UserSearchUC" OnUserChanged="SelectedUser_OnChanged" ConsiderPriorOnBoardDays="true" AllowSingleUser="true" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-3 control-label"><%= T("App.ACR.Supervisor") %></label>
                        <div class="col-xs-5">
                            <uc2:UserSearchUC2 runat="server" ID="SupervisorSearchUC" DataSourceMethod="GetROUsers" AllowSingleUser="true" OnUserChanged="SelectedSupervisor_OnChanged" />
                        </div>
                    </div>
                    <div class="form-group required">
                        <label class="col-xs-3 control-label"><%= T("App.ACR.ApplicationType") %></label>
                        <div class="col-xs-5">
                            <asp:DropDownList CssClass="form-control selectpicker" runat="server" ID="ddlEndPointSystemType" AutoPostBack="True" OnSelectedIndexChanged="ddlEndPointSystemType_SelectedIndexChanged" />
                        </div>
                        <div class="col-xs-4">
                            <asp:HyperLink ID="btnFAQ" NavigateUrl="~/Files/PDF/OneCardFAQ.pdf" Visible="false" CssClass="valign-bottom" runat="server" Target="_blank">FAQ</asp:HyperLink>
                        </div>
                    </div>
                    <div class="form-group required">
                        <label class="col-xs-3 control-label"><%= T("App.ACR.Application") %></label>
                        <div class="col-xs-5">
                            <asp:DropDownList CssClass="form-control selectpicker" runat="server" ID="ddlEndpoint" AutoPostBack="True" OnSelectedIndexChanged="ddlEndpoint_SelectedIndexChanged" />
                        </div>
                    </div>
                    <%-- <div class="form-group required" runat="server" id="divVPNType" visible="false" >
                        <label class="col-xs-3 control-label">VPN Type</label>
                        <div class="col-xs-5">
                            <asp:DropDownList CssClass="form-control selectpicker" runat="server" ID="ddlVPNType"  AutoPostBack="True" OnSelectedIndexChanged="ddlVPNType_SelectedIndexChanged" >
                            </asp:DropDownList>
                        </div>
                    </div>--%>
                    <div class="form-group">

                        <label class="col-xs-3 control-label"><%= T("App.ACR.AccountType") %></label>
                        <div class="col-xs-5">
                            <asp:DropDownList CssClass="form-control selectpicker" runat="server" ID="ddlAccountType" AutoPostBack="True" OnSelectedIndexChanged="ddlAccountType_SelectedIndexChanged" />
                        </div>

                    </div>
                </div>

            </div>
        </div>
        <div class="form-horizontal left clearfix">
            <div class="row-fluid">
                <div class="form-group">
                    <div class="col-xs-12">
                        <label class="col-xs-3 control-label"><%= T("App.ACR.RequestType") %></label>
                        <div class="col-xs-5" id="divAccess1">
                            <div style="width: 33%; float: left;">
                                <asp:RadioButton ID="AddAccess" runat="server" Checked="True" GroupName="AccessSelect" AutoPostBack="true" OnCheckedChanged="RequestTypeSelectionChanged" />
                                <span class="lbl"><%= T("App.ACR.NewAccount") %></span>
                            </div>
                            <div style="width: 33%; float: left;">
                                <asp:RadioButton ID="AmendAccess" runat="server" GroupName="AccessSelect" AutoPostBack="true" OnCheckedChanged="RequestTypeSelectionChanged" />
                                <span class="lbl"><%= T("App.ACR.Amendment") %></span>
                            </div>
                            <div style="width: 33%; float: left;">
                                <asp:RadioButton ID="DeleteAccess" runat="server" GroupName="AccessSelect" AutoPostBack="true" OnCheckedChanged="RequestTypeSelectionChanged" />
                                <span class="lbl"><%= T("App.ACR.DeleteAccount") %></span>
                            </div>
                            <%--<div style="width: 25%; float: left;">
                            <asp:RadioButton ID="ReNewAccess" runat="server" GroupName="AccessSelect" AutoPostBack="true" OnCheckedChanged="RequestTypeSelectionChanged" />
                            <span class="lbl"><%= T("App.ACR.ReNewAccount") %></span>
                            </div>--%>
                        </div>
                        <div class="col-xs-3" id="divAccess2">
                            <asp:Panel runat="server" Visible="false" ID="AmendmentPanel">
                                Type : 
                            <asp:RadioButton ID="AmendAdd" runat="server" GroupName="AmendmentTypeSelect" AutoPostBack="true" OnCheckedChanged="AmendAdd_CheckedChanged" />
                                <span class="lbl"><%= T("App.ACR.AmendAdd") %></span>
                                <asp:RadioButton ID="AmendRemove" runat="server" GroupName="AmendmentTypeSelect" AutoPostBack="true" OnCheckedChanged="AmendRemove_CheckedChanged" />
                                <span class="lbl"><%= T("App.ACR.AmendRemove") %></span>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="form-horizontal left clearfix">
            <div class="row-fluid">
                <div class="col-xs-12">

                    <div class="form-group required">
                        <label class="col-xs-3 control-label"><%= T("App.ACR.ValidFrom") %></label>
                        <div class="col-xs-5">
                            <telerik:RadDatePicker runat="server" ID="ValidFromPicker" AutoPostBack="true" OnSelectedDateChanged="ValidFromPicker_SelectedDateChanged" onkeydown="handleDateTimePickerClickEvent(this,event);" EnableTyping="false" Skin="IACMSkin" CssClass="form-control" DatePopupButton-Visible="true">
                                <ClientEvents OnDateSelected="OnDateSelected"></ClientEvents>
                                <Calendar ID="Calendar3" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                    <FooterTemplate>
                                        <div>
                                            <asp:Button ID="VFButton" runat="server" Width="100%" Text="Today" OnClientClick="SetToday('VFButton')" />
                                        </div>
                                    </FooterTemplate>
                                </Calendar>
                                <DateInput ID="DateInput3" ReadOnly="false" runat="server" ToolTip="Date input For Valid From"
                                    BorderStyle="NotSet" Label="">
                                </DateInput>
                            </telerik:RadDatePicker>
                            <asp:RequiredFieldValidator runat="server" ID="RFVL" ControlToValidate="ValidFromPicker"
                                ErrorMessage="Please Enter the Date" class="error-message"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <asp:Panel ID="ValidToPanel" runat="server" CssClass="form-group">
                        <label class="col-xs-3 control-label"><%= T("App.ACR.ValidTo") %></label>
                        <div class="col-xs-5">
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
                        <div id="" class="whats-this bottom showTooltip">
                            <a class="di_tooltip" data-toggle="tooltip" href="#"><span class="icon icon-question-sign t-vertical-align"></span></a>
                            <span style="display: none" role="tooltip" class="tooltip right info">
                                <h5><%= T("App.Hint.ARValidTo") %></h5>
                            </span>
                        </div>
                    </asp:Panel>
                    <div id="RequestReasonDiv" runat="server" class="form-group">
                        <label class="col-xs-3 control-label"><%= T("App.ACR.RequestReason") %></label>
                        <div class="col-xs-5">
                            <asp:TextBox ID="RequestReason" runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-xs-11">
                            <asp:Label ID="lblLicenseMsg" runat="server" ForeColor="Red"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-xs-11">
                            <asp:Label ID="lblInfo" runat="server" ForeColor="Red"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
