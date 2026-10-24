<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Master/ProtoMast4.master" CodeBehind="AllEmployessDetail.aspx.cs" Inherits="DI.Chakra.Web.Report.User.AllEmployessDetail" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <style type="text/css">
        .tab-pane {
            overflow: hidden;
        }
    </style>
    <asp:Panel ID="Panel1" runat="server">
        <cc1:IACMRadGridControl runat="server" ID="EmployeeGrid" OnNeedDataSource="EmployeesGrid_NeedDataSource" ClientSettings-Scrolling-AllowScroll="true">
            <ClientSettings>
                <Scrolling SaveScrollPosition="true" AllowScroll="true" />
            </ClientSettings>
            <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="Top" DataKeyNames="Id" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                <Columns>
                    <telerik:GridBoundColumn SortExpression="EmployeeId" HeaderText="Employee Id" HeaderButtonType="TextButton" DataField="EmployeeId" FilterControlWidth="30">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="UserName" HeaderText="User Name" HeaderButtonType="TextButton" DataField="UserName" FilterControlWidth="25">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="DisplayName" HeaderText="Display Name" HeaderButtonType="TextButton" DataField="DisplayName">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="EmailId" HeaderText="Email Id" HeaderButtonType="TextButton" DataField="EmailId" FilterControlWidth="90">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="OrganisationName" HeaderText="Organisation Name" HeaderButtonType="TextButton" DataField="OrganisationName" FilterControlWidth="100">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="BusinessRoleName" HeaderText="BusinessRole Name" HeaderButtonType="TextButton" DataField="BusinessRoleName">
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn SortExpression="JoinDate" DataFormatString="{0:dd-MM-yyyy}" HeaderText="Join Date" HeaderButtonType="TextButton" DataField="JoinDate" FilterControlWidth="140">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridDateTimeColumn SortExpression="TerminationDate" DataFormatString="{0:dd-MM-yyyy}" HeaderText="Termination Date" HeaderButtonType="TextButton" DataField="TerminationDate" FilterControlWidth="140">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn SortExpression="EmployeeType" HeaderText="Employee Type" HeaderButtonType="TextButton" DataField="EmployeeType" FilterControlWidth="30">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="IsActive" HeaderText="Active" HeaderButtonType="TextButton" DataField="IsActive" FilterControlWidth="20">
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
        </cc1:IACMRadGridControl>
    </asp:Panel>
</asp:Content>
