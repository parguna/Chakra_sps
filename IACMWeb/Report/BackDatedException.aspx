<%@ Page Language="C#" AutoEventWireup="true" Title="BackDatedException" CodeBehind="BackDatedException.aspx.cs" MasterPageFile="~/Master/ProtoMast4.master" Inherits="DI.Chakra.Web.Report.BackDatedException" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
     <script type="text/javascript">
         function clearFilter() {
              var ValidFromPicker = $find("<%= DatePickerFromDate.ClientID %>");
             var ValidToPicker = $find("<%= DatePickerToDate.ClientID %>");
             ValidFromPicker.clear();
             ValidToPicker.clear();
         }
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
                                        <telerik:RadDatePicker runat="server" ID="DatePickerFromDate"  EnableTyping="false" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
                                            DatePopupButton-Visible="true" Width="100%" CssClass="form-control" AutoPostBack="false">
                                            <ClientEvents OnDateSelected="OnDateSelected"></ClientEvents>
                                            <Calendar ID="Calendar1" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                                <FooterTemplate>
                                                    <div style="width: 100%; text-align: center; background-color: Gray;">
                                                        <asp:Button ID="EButton" runat="server" Width="100%" Text="Today" OnClientClick="SetToday('FButton')" AutoPostBack="false" />
                                                    </div>
                                                </FooterTemplate>
                                            </Calendar>
                                            <DateInput ID="DateInput1" ReadOnly="false"  runat="server" ToolTip="Date input For From Date"
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
                                    <div class="col-xs-3">
                                    <asp:Button ID="search" runat="server" Text="Search" CssClass="btn btn-default btn-sm" OnClick="search_Click"/>
                                    <asp:Button ID="clear" runat="server" Text="Clear" CssClass="btn btn-default btn-sm" OnClientClick="clearFilter()" /> 
                                    </div>                                   
                                </div>                                                                                  
                                <div class="form-group">
                                    <cc1:IACMRadGridControl ID="grdBackDatedException" runat="server" AllowPaging="True" AllowSorting="True" AllowCustomPaging="false" 
                                        FilterMenu-CssClass="RadMenu_IACMFilterSkin" OnNeedDataSource="grdBackDatedException_NeedDataSource">
                                         
                                        <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" ShowHeadersWhenNoRecords="true"  DataKeyNames="Id" NoMasterRecordsText="No Records Available">
                                            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>
                                            <Columns>                                                  
                                                 <telerik:GridBoundColumn DataField="UserName" FilterControlAltText="UserName" HeaderText="User Name" SortExpression="UserName" UniqueName="UserName" HeaderStyle-Width="9%" FilterControlWidth="66%">
                                                </telerik:GridBoundColumn>                                                                                   
                                                <telerik:GridBoundColumn DataField="FirstName" FilterControlAltText="FirstName" HeaderText="First Name" SortExpression="FirstName" UniqueName="FirstName" HeaderStyle-Width="9%" FilterControlWidth="66%">
                                                </telerik:GridBoundColumn>                                              
                                                <telerik:GridBoundColumn DataField="LastName" FilterControlAltText="LastName" SortExpression="LastName" HeaderText="Last Name" UniqueName="LastName" HeaderStyle-Width="10%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>  
                                                 <telerik:GridBoundColumn DataField="DisplayName" FilterControlAltText="DisplayName" SortExpression="DisplayName" HeaderText="Display Name" UniqueName="DisplayName" HeaderStyle-Width="10%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                  <telerik:GridBoundColumn DataField="Company" FilterControlAltText="Company" SortExpression="Company" HeaderText="Company" UniqueName="Company" HeaderStyle-Width="10%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>      
                                                  <telerik:GridBoundColumn DataField="BusinessUnit" FilterControlAltText="BusinessUnit" SortExpression="BusinessUnit" HeaderText="Business Unit" UniqueName="BusinessUnit" HeaderStyle-Width="10%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>             
                                                  <telerik:GridDateTimeColumn DataField="CreatedDate" FilterControlAltText="CreatedDate" SortExpression="CreatedDate" HeaderText="Created Date" UniqueName="CreatedDate" HeaderStyle-Width="10%" FilterControlWidth="70%">
                                                </telerik:GridDateTimeColumn>
                                                <telerik:GridDateTimeColumn DataField="JoinDate" FilterControlAltText="JoinDate" SortExpression="JoinDate" HeaderText="Join Date" UniqueName="JoinDate" HeaderStyle-Width="10%" FilterControlWidth="70%">
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
                <asp:Button ID="btncancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel"/>             
            </div>   
        </ContentTemplate>
    </asp:UpdatePanel>
    </asp:Content>