<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReconcilationReport.aspx.cs" MasterPageFile="~/Master/ProtoMast4.master" Inherits="DI.Chakra.Web.Report.ReconcilationReport" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <script type="text/javascript">
        function SetToday(Parameter) {
            var datepicker = $find(Parameter == "FButton" ? "<%=DatePickerFromDate.ClientID%>" : "<%=DatePickerToDate.ClientID%>");
            var dt = new Date();
            datepicker.set_selectedDate(dt);
            datepicker.hidePopup();
        }

        function OnDateSelected(sender, e) {
            var ValidFromPicker = $find("<%= DatePickerFromDate.ClientID %>");
            var ValidToPicker = $find("<%= DatePickerToDate.ClientID %>");

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

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="content-wrapper" runat="server" id="contentWrapper">
                <div class="form clearfix">
                    <div class="form-horizontal left">
                        <div class="row-fluid">
                            <div class="col-xs-12">
                                <div class="form-group">
                                    <label for="From" class="col-xs-1 control-label">From</label>
                                    <div class="col-xs-2">
                                        <telerik:RadDatePicker runat="server" ID="DatePickerFromDate" EnableTyping="false" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
                                            DatePopupButton-Visible="true" Width="100%" CssClass="form-control" AutoPostBack="false">
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
                                        <telerik:RadDatePicker runat="server" ID="DatePickerToDate" EnableTyping="false" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
                                            DatePopupButton-Visible="true" Width="100%" CssClass="form-control" AutoPostBack="false">
                                            <Calendar ID="Calendar2" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                                <FooterTemplate>
                                                    <div style="width: 100%; text-align: center; background-color: Gray;">
                                                        <asp:Button ID="Button1" runat="server" Width="100%" Text="Today" OnClientClick="SetToday('TButton')" AutoPostBack="false" />
                                                    </div>
                                                </FooterTemplate>
                                            </Calendar>
                                            <DateInput ID="DateInput2" ReadOnly="false" runat="server" ToolTip="Date input For End Date"
                                                BorderStyle="NotSet" Label="" AutoPostBack="false">
                                            </DateInput>
                                        </telerik:RadDatePicker>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label for="Resolved" class="col-xs-1 control-label">Is Resolved</label>
                                    <div class="col-xs-5">
                                        <asp:RadioButton ID="Resolved" AutoPostBack="true" GroupName="IsResolved" Text="Yes" Checked="true" runat="server" OnCheckedChanged="Resolved_CheckedChanged" />
                                        <asp:RadioButton ID="NotResolved" AutoPostBack="true" GroupName="IsResolved" Text="No" runat="server" OnCheckedChanged="Resolved_CheckedChanged" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label for="Statustype" class="col-xs-1 control-label">Resolved Status</label>
                                    <div class="col-xs-5">
                                        <asp:DropDownList ID="TypeOfStatus" runat="server" data-width="100%" CssClass="form-control selectpicker">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-xs-1">
                                        <asp:Button ID="search" runat="server" Text="Search" CssClass="btn btn-default btn-sm" OnClick="search_Click" />
                                    </div>
                                </div>

                                <div class="form-group">
                                    <cc1:IACMRadGridControl ID="ReconcilationReportGrid" runat="server" AllowPaging="True" AllowSorting="True" AllowCustomPaging="false"
                                        FilterMenu-CssClass="RadMenu_IACMFilterSkin" OnNeedDataSource="ReconcilationReportGrid_NeedDataSource" ClientSettings-EnablePostBackOnRowClick="true">
                                        <ClientSettings Selecting-AllowRowSelect="true" EnablePostBackOnRowClick="true"></ClientSettings>
                                        <MasterTableView TableLayout="fixed"
                                             EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available"
                                            AutoGenerateColumns="false">                                            
                                            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>
                                            <Columns>

                                                <telerik:GridBoundColumn DataField="EndPointName" FilterControlAltText="EndPointName" SortExpression="EndPointName" HeaderText="SubSystem Name" UniqueName="EndPointName" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="AccountName" FilterControlAltText="AccountName" SortExpression="AccountName" HeaderText="AccountName" UniqueName="AccountName" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="DisplayName" FilterControlAltText="DisplayName" SortExpression="DisplayName" HeaderText="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="ReconcilationType" FilterControlAltText="ReconcilationType" SortExpression="ReconcilationType" HeaderText="ReconcilationType" UniqueName="ReconcilationType" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="Entitlement" FilterControlAltText="Entitlement" SortExpression="Entitlement" HeaderText="Entitlement" UniqueName="Entitlement" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="ResolvedStatus" FilterControlAltText="ResolvedStatus" SortExpression="IsResolved" HeaderText="ResolvedStatus" UniqueName="ResolvedStatus" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="ResolvedBy" FilterControlAltText="ResolvedBy" SortExpression="ResolvedBy" HeaderText="ResolvedBy" UniqueName="ResolvedBy" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="Comments" FilterControlAltText="Comments" SortExpression="Comments" HeaderText="Comments" UniqueName="Comments" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="ExceptionType" FilterControlAltText="ExceptionType" SortExpression="ExceptionType" HeaderText="ExceptionType" UniqueName="ExceptionType" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>


                                            </Columns>
                                        </MasterTableView>
                                    </cc1:IACMRadGridControl>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="buttons text-right nav-rectangle">
                <asp:Button ID="btncancel" runat="server" Text="Cancel" CssClass="btn btn-default btn-sm" OnClick="btncancel_Click" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
