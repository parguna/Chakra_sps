<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmployeesListUC.ascx.cs" Inherits="DI.Chakra.Web.Report.User.EmployeesListUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>




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

                                    <div class="form-group">
                        <asp:Label AssociatedControlID="rbActive" CssClass="col-xs-4" runat="server"></asp:Label>
                        <div class="col-xs-7">
                            <asp:RadioButton ID="rbActive" AutoPostBack="true" Checked="true" runat="server" />
                            <asp:Label ID="lblActive" runat="server" CssClass="lbl" Text="Active"></asp:Label>

                            <asp:RadioButton ID="rbInActive" AutoPostBack="true" runat="server" />
                            <asp:Label ID="lblInactive" runat="server" CssClass="lbl" Text="InActive"></asp:Label>
                        </div>
                    </div>
                                    <div class="col-xs-1">
                                    <asp:Button ID="search" runat="server" Text="Search" CssClass="btn btn-default btn-sm" OnClick="search_Click"/>
                                    </div>                                   
                                </div>                              
                                <div class="form-group">
                                    <cc1:IACMRadGridControl ID="EmployeeGrid" runat="server" AllowPaging="True" AllowSorting="True" AllowCustomPaging="True" 
                                        FilterMenu-CssClass="RadMenu_IACMFilterSkin" OnNeedDataSource="EmployeesGrid_NeedDataSource">
                                        <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="false" ShowHeadersWhenNoRecords="false"  DataKeyNames="Id" NoMasterRecordsText="No Records Available">
                                            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>
                                            <Columns>                                                                                        
                                                <telerik:GridBoundColumn DataField="EmployeeId" FilterControlAltText="EmployeeId" HeaderText="Employee Id" SortExpression="EmployeeId" UniqueName="EmployeeId" HeaderStyle-Width="30%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="UserName" FilterControlAltText="UserName" SortExpression="UserName" HeaderText="UserName" UniqueName="UserName" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                            
                                                       <telerik:GridBoundColumn DataField="DisplayName" FilterControlAltText="DisplayName" SortExpression="DisplayName" HeaderText="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                       <telerik:GridBoundColumn DataField="MobileNumber" FilterControlAltText="MobileNumber" SortExpression="MobileNumber" HeaderText="MobileNumber" UniqueName="MobileNumber" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                       <telerik:GridDateTimeColumn DataField="JoinDate" FilterControlAltText="JoinDate" SortExpression="JoinDate" HeaderText="JoinDate" UniqueName="JoinDate" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                                </telerik:GridDateTimeColumn>
                                                       <telerik:GridBoundColumn DataField="OrganisationName" FilterControlAltText="OrganisationName" SortExpression="OrganisationName" HeaderText="Department" UniqueName="OrganisationName" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                       <telerik:GridBoundColumn DataField="Ethinicity" FilterControlAltText="Ethinicity" SortExpression="Ethinicity" HeaderText="Ethinicity" UniqueName="Ethinicity" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                       <telerik:GridBoundColumn DataField="Country" FilterControlAltText="Country" SortExpression="Country" HeaderText="Country" UniqueName="Country" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                  <telerik:GridBoundColumn DataField="BusinessRoleName" FilterControlAltText="BusinessRoleName" SortExpression="BusinessRoleName" HeaderText="Position" UniqueName="BusinessRoleName" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                         <telerik:GridBoundColumn DataField="ManagerName" FilterControlAltText="ManagerName" SortExpression="ManagerName" HeaderText="Supervisor" UniqueName="ManagerName" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                        <telerik:GridBoundColumn DataField="Grade" FilterControlAltText="Grade" SortExpression="Grade" HeaderText="Grade" UniqueName="Grade" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                        <telerik:GridBoundColumn DataField="CSEL" FilterControlAltText="CSEL" SortExpression="CSEL" HeaderText="CSEL" UniqueName="CSEL" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                        <telerik:GridBoundColumn DataField="GoldBar" FilterControlAltText="GoldBar" SortExpression="GoldBar" HeaderText="GoldBar" UniqueName="GoldBar" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                                </telerik:GridBoundColumn>
                                                    <telerik:GridBoundColumn DataField="Company" FilterControlAltText="Company" SortExpression="Company" HeaderText="Company" UniqueName="Company" HeaderStyle-Width="20%" FilterControlWidth="70%">
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
                <asp:Button ID="btncancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel"/>             
            </div>   
        </ContentTemplate>
    </asp:UpdatePanel>




<%--<telerik:RadGrid runat="server" ID="" AutoGenerateColumns="False" GroupPanelPosition="Top"
    OnNeedDataSource="EmployeesGrid_NeedDataSource" AllowPaging="true" FilterMenu-CssClass="RadMenu_IACMFilterSkin" AllowFilteringByColumn="true">
    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="10" Height="100%" AlwaysVisible="true" />
    <ClientSettings>
        <Scrolling UseStaticHeaders="true" />
    </ClientSettings>
    <GroupingSettings CaseSensitive="false" />
    <MasterTableView AutoGenerateColumns="False" ShowHeadersWhenNoRecords="false"
        CommandItemDisplay="None" EnableHeaderContextMenu="false" DataKeyNames="Id"
        NoMasterRecordsText="No Records Available">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn SortExpression="EmployeeId" HeaderText="Employee Id" HeaderButtonType="TextButton" DataField="EmployeeId" HeaderStyle-Width="35%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="UserName" HeaderText="User Name" HeaderButtonType="TextButton" DataField="UserName" HeaderStyle-Width="20%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="DisplayName" HeaderText="Display Name" HeaderButtonType="TextButton" DataField="DisplayName" HeaderStyle-Width="35%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="EmailId" HeaderText="Email Id" HeaderButtonType="TextButton" DataField="EmailId" HeaderStyle-Width="20%">
            </telerik:GridBoundColumn>

            <telerik:GridBoundColumn SortExpression="MobileNumber" HeaderText="MobileNumber" HeaderButtonType="TextButton" DataField="MobileNumber" HeaderStyle-Width="20%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="JoinDate" HeaderText="Join Date" HeaderButtonType="TextButton" DataField="JoinDate" HeaderStyle-Width="35%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="OrganisationName" HeaderText="Organisation Name" HeaderButtonType="TextButton" DataField="OrganisationName" HeaderStyle-Width="20%">
            </telerik:GridBoundColumn>

            <telerik:GridBoundColumn SortExpression="Ethinicity" HeaderText="Ethinicity" HeaderButtonType="TextButton" DataField="Ethinicity" HeaderStyle-Width="35%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="Country" HeaderText="Country" HeaderButtonType="TextButton" DataField="Country" HeaderStyle-Width="20%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="BusinessRoleName" HeaderText="BusinessRole Name" HeaderButtonType="TextButton" DataField="BusinessRoleName" HeaderStyle-Width="35%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="ManagerName" HeaderText="Manager Name" HeaderButtonType="TextButton" DataField="ManagerName" HeaderStyle-Width="20%">
            </telerik:GridBoundColumn>
        </Columns>

    </MasterTableView>
</telerik:RadGrid>--%>