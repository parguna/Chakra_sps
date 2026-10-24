<%@ Page Title="Log" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Log.aspx.cs" Inherits="DI.Chakra.Web.Log.Log" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

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

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="content-wrapper" runat="server" id="contentWrapper">
                <div class="form clearfix">
                    <div class="form-horizontal left">
                        <div class="row-fluid">
                            <div class="col-xs-12">
                                <div class="form-group">
                                    <asp:Label ID="lblLogType" AssociatedControlID="ddlLogRefType" runat="server" Text="Log Type" CssClass="col-xs-2 control-label"></asp:Label>
                                    <div class="col-xs-2">
                                        <asp:DropDownList ID="ddlLogRefType" DataTextField="" DataValueField="" runat="server" CssClass="form-control selectpicker" OnSelectedIndexChanged="ddlLogRefType_SelectedIndexChanged" data-width="100%" AutoPostBack="true">
                                        </asp:DropDownList>
                                    </div>

                                    <label for="From" class="col-xs-2 control-label">Log Level</label>
                                    <div class="col-xs-2">
                                        <asp:DropDownList ID="ddlLogLevel" runat="server" CssClass="form-control selectpicker" data-width="100%" AutoPostBack="false">
                                        </asp:DropDownList>
                                    </div>


                                </div>

                                <div class="form-group" runat="server" id="discoverydiv" visible="false">


                                    <label for="From" class="col-xs-2 control-label">Endpoint System Type</label>
                                    <div class="col-xs-2">
                                        <asp:DropDownList ID="ddlEpType" runat="server" CssClass="form-control selectpicker" OnSelectedIndexChanged="ddlEpType_SelectedIndexChanged" data-width="100%" AutoPostBack="true">
                                        </asp:DropDownList>
                                    </div>


                                    <label for="From" class="col-xs-2 control-label">Endpoint System Name</label>
                                    <div class="col-xs-2">
                                        <asp:DropDownList ID="ddlEpName" runat="server" CssClass="form-control" data-width="100%" AutoPostBack="false">
                                        </asp:DropDownList>
                                    </div>

                                    <label for="From" class="col-xs-2 control-label" style="display:none;">Discovery Object</label>
                                    <div class="col-xs-2" style="display:none;">
                                        <asp:DropDownList ID="ddlDiscoveryObject" runat="server" CssClass="form-control  selectpicker" data-width="100%" AutoPostBack="false">
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="form-group" runat="server" id="RequestIddiv" visible="false">
                                    <label for="From" class="col-xs-2 control-label">Request Id</label>
                                    <div class="col-xs-2">
                                        <asp:TextBox ID="txtRequestId" runat="server" CssClass="form-control  selectpicker" data-width="100%" AutoPostBack="false">
                                        </asp:TextBox>
                                    </div>

                                </div>

                                <div class="form-group">
                                    <label for="From" class="col-xs-2 control-label">From</label>
                                    <div class="col-xs-2">
                                        <telerik:RadDatePicker runat="server" ID="DatePickerValidFrom" onkeydown="handleDateTimePickerClickEvent(this,event)" OnSelectedDateChanged="DatePickerValidTo_SelectedDateChanged" EnableTyping="false" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
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
                                    <label for="To" class="col-xs-2 control-label">To</label>
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

                                <div class="form-group" >
                                    <cc1:IACMRadGridControl ID="grdLogs" DisableRowSelect="true" runat="server" AllowPaging="True" AllowCustomPaging="false" AllowFilteringByColumn="true" AllowSorting="True" 
                                        FilterMenu-CssClass="RadMenu_IACMFilterSkin" OnNeedDataSource="grdLogs_NeedDataSource" >
                                        <MasterTableView NoDetailRecordsText="No Records Available" DataKeyNames="Id" Width="100%" >

                                            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>
                                            <Columns>

                                                <telerik:GridBoundColumn DataField="LogLevel" HeaderText="Log Level" AllowSorting="true" UniqueName="LogLevel" AllowFiltering="true" DataType="System.String" HeaderStyle-Width="6%" FilterControlWidth="65%">
                                                </telerik:GridBoundColumn>
                                                 <telerik:GridBoundColumn  DataField="EndpointSystemTypeFullName" FilterControlAltText="EndpointSystemType" HeaderText="Endpoint System Type" SortExpression="EndpointSystemType" UniqueName="EndpointSystemTypeName" HeaderStyle-Width="9%" FilterControlWidth="70%"  >
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="Message" FilterControlAltText="Message" SortExpression="Message" HeaderText="Message" UniqueName="Message" HeaderStyle-Width="12%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="ExceptionMessage" FilterControlAltText="ExceptionMessage" SortExpression="ExceptionMessage" HeaderText="Exception Message" UniqueName="ExceptionMessage" HeaderStyle-Width="10%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="InnerExceptionMessage" FilterControlAltText="InnerExceptionMessage" SortExpression="InnerExceptionMessage" HeaderText="Inner Exception Message" UniqueName="InnerExceptionMessage" HeaderStyle-Width="10%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" DataField="LogDate" FilterControlAltText="LogDate" HeaderText="Log Date" SortExpression="LogDate" UniqueName="LogDate" HeaderStyle-Width="9%" FilterControlWidth="70%" PickerType="DatePicker" DataFormatString="{0:dd-MM-yyyy hh:mm:ss tt}">
                                                </telerik:GridDateTimeColumn>
                                               <telerik:GridBoundColumn DataField="ReferenceId" FilterControlAltText="ReferenceId" HeaderText="Request Id" SortExpression="ReferenceId" UniqueName="ReferenceId" HeaderStyle-Width="9%" FilterControlWidth="66%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="EndpointSystemName" FilterControlAltText="Endpoint System Name" HeaderText="Endpoint System Name" SortExpression="EndpointSystemName" UniqueName="EndpointSystemName" HeaderStyle-Width="9%" FilterControlWidth="70%" >
                                                </telerik:GridBoundColumn>

                                            </Columns>
                                        </MasterTableView>
                                    </cc1:IACMRadGridControl>
                                     <cc1:IACMRadGridControl ID="grdEmailLog" DisableRowSelect="true" runat="server" AllowPaging="True" AllowCustomPaging="false" AllowFilteringByColumn="true" AllowSorting="True" 
                                        FilterMenu-CssClass="RadMenu_IACMFilterSkin" OnNeedDataSource="grdEmailLog_NeedDataSource" >
                                        <MasterTableView NoDetailRecordsText="No Records Available" DataKeyNames="Id" Width="100%">

                                            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>
                                            <Columns>
                                                <telerik:GridBoundColumn DataField="LogLevel" HeaderText="Log Level" AllowSorting="true" UniqueName="LogLevel" AllowFiltering="true" DataType="System.String" HeaderStyle-Width="6%" FilterControlWidth="65%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="Message" FilterControlAltText="Message" SortExpression="Message" HeaderText="Message" UniqueName="Message" HeaderStyle-Width="12%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="ExceptionMessage" FilterControlAltText="ExceptionMessage" SortExpression="ExceptionMessage" HeaderText="Exception Message" UniqueName="ExceptionMessage" HeaderStyle-Width="10%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="ExceptioSource" FilterControlAltText="ExceptioSource" SortExpression="ExceptioSource" HeaderText="Exceptio Source" UniqueName="ExceptioSource" HeaderStyle-Width="10%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="StackTrace" FilterControlAltText="StackTrace" SortExpression="StackTrace" HeaderText="Stack Trace" UniqueName="StackTrace" HeaderStyle-Width="10%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="InnerExceptionMessage" FilterControlAltText="InnerExceptionMessage" SortExpression="InnerExceptionMessage" HeaderText="Inner Exception Message" UniqueName="InnerExceptionMessage" HeaderStyle-Width="10%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" DataField="LogDate" FilterControlAltText="LogDate" HeaderText="Log Date" SortExpression="LogDate" UniqueName="LogDate" HeaderStyle-Width="9%" FilterControlWidth="70%" PickerType="DatePicker" DataFormatString="{0:dd-MM-yyyy hh:mm:ss tt}">
                                                </telerik:GridDateTimeColumn>
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
                <asp:Button ID="btncancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="btnCancel_Click" />
                <asp:Button ID="btnRefresh" runat="server" CssClass="btn btn-default btn-sm" Text="Refresh" OnClick="btnRefresh_Click" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
