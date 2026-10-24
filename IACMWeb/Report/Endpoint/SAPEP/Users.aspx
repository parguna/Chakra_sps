<%@ Page Title="SAP Enterprise Portal Users" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.SAPEP.Users" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="EndpointSystem" TagName="EndpointUC" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left">
                <div class="row-fluid">
                    <div class="col-xs-12">
                        <div class="form-group">
                            <EndpointSystem:EndpointUC runat="server" ID="EndpointUC" OnEndpointChanged="DropDownSelectectionChanged" DisplayControl="Application" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <asp:Panel ID="Panel1" runat="server">
        <telerik:RadGrid ID="grdUser" FilterMenu-CssClass="RadMenu_IACMFilterSkin" runat="server" AllowFilteringByColumn="True"
            OnNeedDataSource="grdUser_NeedDataSource" AllowPaging="True" AllowSorting="True">
            <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" />
            <ClientSettings>
                <Scrolling UseStaticHeaders="true" />
                <Selecting AllowRowSelect="true" />
            </ClientSettings>
            <GroupingSettings CaseSensitive="false" />
            <MasterTableView AutoGenerateColumns="False" ShowHeadersWhenNoRecords="false" CommandItemDisplay="None" NoMasterRecordsText="No Records Available" EnableHeaderContextMenu="false"  Width="100%"
                DataKeyNames="Id">
                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="true"></CommandItemSettings>
                <Columns>
                    <telerik:GridBoundColumn DataField="UserName" FilterControlAltText="UserName" HeaderText="User Name" SortExpression="UserName" UniqueName="UserName" HeaderStyle-Width="200px" FilterControlWidth="65%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="FirstName" FilterControlAltText="FirstName" HeaderText="First Name" SortExpression="FirstName" UniqueName="FirstName" HeaderStyle-Width="200px" FilterControlWidth="65%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="LastName" FilterControlAltText="LastName" HeaderText="Last Name" SortExpression="LastName" UniqueName="LastName" HeaderStyle-Width="200px" FilterControlWidth="65%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Salutation" FilterControlAltText="Salutation" HeaderText="Salutation" SortExpression="Salutation" UniqueName="Salutation" HeaderStyle-Width="100px" FilterControlWidth="60%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="DisplayName" FilterControlAltText="DisplayName" HeaderText="Display Name" SortExpression="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="200px" FilterControlWidth="65%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="IsMapped" FilterControlAltText="IsMapped" HeaderText="IsMapped" SortExpression="IsMapped" UniqueName="IsMapped" HeaderStyle-Width="100px" FilterControlWidth="75%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Owner" FilterControlAltText="Owner" HeaderText="Owner" SortExpression="Owner" UniqueName="Owner" HeaderStyle-Width="200px" FilterControlWidth="75%">
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn DataField="StartDate" FilterControlAltText="StartDate" HeaderText="Start Date" SortExpression="StartDate" UniqueName="StartDate" HeaderStyle-Width="150px" FilterControlWidth="65%">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridDateTimeColumn DataField="EndDate" FilterControlAltText="EndDate" HeaderText="End Date" SortExpression="EndDate" UniqueName="EndDate" HeaderStyle-Width="150px" FilterControlWidth="65%">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn DataField="Email" FilterControlAltText="Email" HeaderText="Email" SortExpression="Email" UniqueName="Email" HeaderStyle-Width="300px" FilterControlWidth="65%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Department" FilterControlAltText="Department" HeaderText="Department" SortExpression="Department" UniqueName="Department" HeaderStyle-Width="200px" FilterControlWidth="65%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Title" FilterControlAltText="Title" HeaderText="Position" SortExpression="Title" UniqueName="Title" HeaderStyle-Width="200px" FilterControlWidth="65%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Company" FilterControlAltText="Company" HeaderText="Company" SortExpression="Company" UniqueName="Company" HeaderStyle-Width="200px" FilterControlWidth="65%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="IsLocked" FilterControlAltText="IsLocked" HeaderText="IsLocked" SortExpression="IsLocked" UniqueName="IsLocked" HeaderStyle-Width="200px" FilterControlWidth="65%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="LockScenario" FilterControlAltText="LockScenario" HeaderText="LockScenario" SortExpression="LockScenario" UniqueName="LockScenario" HeaderStyle-Width="200px" FilterControlWidth="65%">
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </asp:Panel>
</asp:Content>
