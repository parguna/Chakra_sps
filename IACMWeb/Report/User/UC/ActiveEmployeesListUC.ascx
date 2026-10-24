<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActiveEmployeesListUC.ascx.cs" Inherits="DI.Chakra.Web.Report.User.UC.ActiveEmployeesListUC" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Panel runat="server" ID="ContentPanel" Enabled="true">
    <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left">
                <div class="row-fluid">
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-xs-4 control-label">User Type</label>
                            <div class="col-xs-6">
                                <asp:DropDownList ID="ddlUserType" runat="server" AutoPostBack="True" CssClass="form-control selectpicker" data-width="100%" OnSelectedIndexChanged="ddlUserType_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Panel>

<cc1:IACMRadGridControl ID="IACMGridUserList" runat="server" Width="100%" OnNeedDataSource="IACMGridUserList_NeedDataSource">
    <MasterTableView NoMasterRecordsText="No Data Found" DataKeyNames="EmployeeId"  Width="100%">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn DataField="EmployeeId" SortExpression="EmployeeId" FilterControlAltText="RuleCode" HeaderText="Employee Id"  AllowFiltering="true" AllowSorting="true" HeaderStyle-Width="24%" FilterControlWidth="65%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="DisplayName" SortExpression="DisplayName" FilterControlAltText="DisplayName" HeaderText="Display Name"  AllowFiltering="true" AllowSorting="true" HeaderStyle-Width="24%" FilterControlWidth="65%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="UserName" SortExpression="UserName" FilterControlAltText="UserName" HeaderText="User Name"  AllowFiltering="true" AllowSorting="true" HeaderStyle-Width="24%" FilterControlWidth="65%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn  DataField="OrganisationName" SortExpression="OrganisationName"  FilterControlAltText="OrganisationName" HeaderText="Organisation Name" AllowFiltering="true" AllowSorting="true" HeaderStyle-Width="24%" FilterControlWidth="65%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="BusinessRoleName" SortExpression="BusinessRoleName" FilterControlAltText="BusinessRoleName" HeaderText="BusinessRole Name"  AllowFiltering="true" AllowSorting="true" HeaderStyle-Width="24%" FilterControlWidth="65%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="EmailId" SortExpression="EmailId" FilterControlAltText="EmailId" HeaderText="Email Id" AllowFiltering="true" AllowSorting="true" HeaderStyle-Width="24%" FilterControlWidth="65%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="MobileNumber" SortExpression="MobileNumber" FilterControlAltText="MobileNumber" HeaderText="MobileNumber" AllowFiltering="true" AllowSorting="true" HeaderStyle-Width="24%" FilterControlWidth="65%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="EmployeeType" SortExpression="EmployeeType" FilterControlAltText="EmployeeType" HeaderText="Employee Type" AllowFiltering="true" AllowSorting="true" HeaderStyle-Width="24%" FilterControlWidth="65%">
            </telerik:GridBoundColumn>
            <telerik:GridDateTimeColumn DataField="JoinDate" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" FilterControlAltText="JoinDate" HeaderText="Join Date" SortExpression="JoinDate" AllowSorting="true" HeaderStyle-Width="23%" FilterControlWidth="77%">
            </telerik:GridDateTimeColumn>
            <telerik:GridDateTimeColumn DataField="TeminationDate" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" FilterControlAltText="TeminationDate" HeaderText="Temination Date" SortExpression="TeminationDate" AllowSorting="true" HeaderStyle-Width="23%" FilterControlWidth="77%">
            </telerik:GridDateTimeColumn>
            <telerik:GridBoundColumn DataField="isActiveUser" FilterControlAltText="isActiveUser" HeaderText="Is Active" SortExpression="isActiveUser" AllowFiltering="true" AllowSorting="true" HeaderStyle-Width="16%" FilterControlWidth="77%">
            </telerik:GridBoundColumn>
        </Columns>
    </MasterTableView>
</cc1:IACMRadGridControl>
