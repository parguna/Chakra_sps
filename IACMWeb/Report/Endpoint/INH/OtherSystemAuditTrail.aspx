<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OtherSystemAuditTrail.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.INH.OtherSystemAuditTrail" MasterPageFile="~/Master/ProtoMast4.master" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="EndpointSystem" TagName="EndpointUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <script type="text/javascript">
        function SetToday(Parameter) {
            var datepicker = $find(Parameter == "FButton" ? "<%=DatePickerValidFrom.ClientID%>" : "<%=DatePickerValidTo.ClientID%>");
            var dt = new Date();
            datepicker.set_selectedDate(dt);
            datepicker.hidePopup();
        }

        function OnDateSelected(sender, e) {
            var ValidFromPicker = $find("<%= DatePickerValidFrom.ClientID %>");
            var ValidToPicker = $find("<%= DatePickerValidTo.ClientID %>");

            var now = new Date();
            if (ValidFromPicker != null && ValidToPicker != null) {
                ValidToPicker.clear();
                now = e.get_newDate();
                if (now != null) {
                    ValidToPicker.set_minDate(new Date(now.getFullYear(), now.getMonth(), now.getDate()));
                }
            }
        }
    </script>
    <div class="content-wrapper">
        <div class="row-fluid">
            <div class="col-xs-12">
                <div class="form-group">
                    <div class="col-xs-3">
                        <asp:Label ID="Label2" runat="server" CssClass="control-label" AssociatedControlID="ddlInhouseSystems">Inhouse system :</asp:Label>
                    </div>
                    <div class="col-xs-4">
                        <asp:DropDownList ID="ddlInhouseSystems" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlInhouseSystems_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
             <div class="row-fluid">
            <div class="col-xs-12">
                &nbsp;
            </div>
        </div>
            <div class="col-xs-12">
                <div class="form-group">
                    <label for="From" class="col-xs-3 control-label">From</label>
                    <div class="col-xs-2">
                        <telerik:RadDatePicker runat="server" ID="DatePickerValidFrom" onkeydown="handleDateTimePickerClickEvent(this,event)" OnSelectedDateChanged="DatePickerValidFrom_SelectedDateChanged" EnableTyping="false" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
                            DatePopupButton-Visible="true" Width="100%" CssClass="form-control" AutoPostBack="true">
                            <ClientEvents OnDateSelected="OnDateSelected"></ClientEvents>
                            <Calendar ID="Calendar1" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                <FooterTemplate>
                                    <div style="width: 100%; text-align: center; background-color: Gray;">
                                        <asp:Button ID="EButton" runat="server" Width="100%" Text="Today" OnClientClick="SetToday('FButton')" AutoPostBack="false" />
                                    </div>
                                </FooterTemplate>
                            </Calendar>
                            <DateInput ID="DateInput1" ReadOnly="false" runat="server" ToolTip="Date input For From Date"
                                BorderStyle="NotSet" Label="" AutoPostBack="false">
                            </DateInput>
                        </telerik:RadDatePicker>
                    </div>
                    <label for="To" class="col-xs-1 control-label">To</label>
                    <div class="col-xs-2">
                        <telerik:RadDatePicker runat="server" onkeydown="handleDateTimePickerClickEvent(this,event)" ID="DatePickerValidTo" EnableTyping="false" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
                            DatePopupButton-Visible="true" Width="100%" CssClass="form-control" AutoPostBack="false">
                            <Calendar ID="Calendar2" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                <FooterTemplate>
                                    <div style="width: 100%; text-align: center; background-color: Gray;">
                                        <asp:Button ID="EButton" runat="server" Width="100%" Text="Today" OnClientClick="SetToday('TButton')" AutoPostBack="false" />
                                    </div>
                                </FooterTemplate>
                            </Calendar>
                            <DateInput ID="DateInput2" ReadOnly="false" runat="server" ToolTip="Date input For End Date"
                                BorderStyle="NotSet" Label="" AutoPostBack="false">
                            </DateInput>
                        </telerik:RadDatePicker>
                    </div>
                    <div class="col-xs-1">
                        <asp:Button runat="server" ID="Filter" CausesValidation="true" CssClass="btn btn-default btn-sm" Text="Filter" OnClick="Filter_Click" />
                    </div>
                    <div class="col-xs-1">
                        <asp:Button runat="server" ID="ClearFilter" CausesValidation="true" CssClass="btn btn-default btn-sm" Text="Clear Filter" OnClick="ClearFilter_Click" />
                    </div>
                </div>
            </div>
        </div>

        <div class="row-fluid">
            <div class="col-xs-12">
                &nbsp;
            </div>
        </div>
        <div class="row-fluid">
            <div class="col-xs-12">
                <asp:Panel ID="Panel1" runat="server" Visible="true">
                    <cc1:IACMRadGridControl ID="grdUser" runat="server" OnNeedDataSource="Users_NeedDataSource" AllowCustomPaging="true" PageSize="50">
                        <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" AlwaysVisible="true" />
                        <MasterTableView TableLayout="fixed"
                            DataKeyNames="Id" EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
                            <CommandItemSettings ShowAddNewRecordButton="false"
                                ShowRefreshButton="true"></CommandItemSettings>

                            <Columns>
                                <telerik:GridBoundColumn DataField="EndpointName" FilterControlAltText="EndpointName" HeaderText="EndpointName" SortExpression="EndpointName" UniqueName="EndpointName" HeaderStyle-Width="10%" FilterControlWidth="80%">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="DisplayName" FilterControlAltText="DisplayName" HeaderText="Name" SortExpression="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="10%" FilterControlWidth="80%">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="UserName" FilterControlAltText="UserName" HeaderText="UserName" SortExpression="UserName" UniqueName="UserName" HeaderStyle-Width="19%" FilterControlWidth="80%">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="AccountType" FilterControlAltText="AccountType" HeaderText="AccountType" SortExpression="AccountType" UniqueName="AccountType" HeaderStyle-Width="10%" FilterControlWidth="80%">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="Permission" FilterControlAltText="Permission" HeaderText="Permission" SortExpression="Permission" UniqueName="Permission" HeaderStyle-Width="10%" FilterControlWidth="80%">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="Remarks" FilterControlAltText="Remarks" HeaderText="Remarks" SortExpression="Remarks" UniqueName="Remarks" HeaderStyle-Width="6%" FilterControlWidth="80%">
                                </telerik:GridBoundColumn>
                                <telerik:GridDateTimeColumn DataType="System.DateTime" DataField="CreatedDate" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" FilterControlAltText="CreatedDate" HeaderText="CreatedDate" SortExpression="CreatedDate" UniqueName="CreatedDate" HeaderStyle-Width="6%" FilterControlWidth="80%">
                                </telerik:GridDateTimeColumn>
                            </Columns>
                        </MasterTableView>
                    </cc1:IACMRadGridControl>
                </asp:Panel>
            </div>
        </div>
    </div>


</asp:Content>
