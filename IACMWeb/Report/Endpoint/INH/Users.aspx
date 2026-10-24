<%@ Page Title="Inhouse Users" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.INH.Users" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="EndpointSystem" TagName="EndpointUC" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">

    <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left clearfix">
                <div class="row-fluid">
                    <div class="col-xs-7">
                        <endpointsystem:endpointuc runat="server" id="EndpointUC" onendpointchanged="DropDownSelectectionChanged" displaycontrol="Application" />
                    </div>
                </div>
            </div>
        </div>
    </div>
        
<asp:Panel ID="Panel1" runat="server">
    <cc1:IACMRadGridControl ID="grdUser" runat="server" OnNeedDataSource="grdUser_NeedDataSource">
        <MasterTableView TableLayout="fixed"
            DataKeyNames="UserId" EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available" 
            AutoGenerateColumns="false">
            <CommandItemSettings ShowAddNewRecordButton="false"
                ShowRefreshButton="true"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="UserName" FilterControlAltText="UserName" HeaderText="User Name" SortExpression="UserName" UniqueName="UserName" HeaderStyle-Width="200px" FilterControlWidth="70px">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="DisplayName" FilterControlAltText="DisplayName" HeaderText="Display Name" SortExpression="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="200px" FilterControlWidth="70px">
                </telerik:GridBoundColumn> 
                <telerik:GridBoundColumn DataField="EmployeeId" FilterControlAltText="EmployeeId" HeaderText="Employee Id" SortExpression="EmployeeId" UniqueName="EmployeeId" HeaderStyle-Width="200px" FilterControlWidth="75px">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Email" FilterControlAltText="Email" HeaderText="Email ID" SortExpression="Email" UniqueName="Email" HeaderStyle-Width="200px" FilterControlWidth="90px">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="NRIC" FilterControlAltText="NRIC" HeaderText="NRIC" SortExpression="NRIC" UniqueName="NRIC" HeaderStyle-Width="200px" FilterControlWidth="90px">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="IsDisabled" FilterControlAltText="IsDisabled" HeaderText="Disabled" SortExpression="IsDisabled" UniqueName="IsDisabled" HeaderStyle-Width="150px" FilterControlWidth="90px">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="IsMapped" FilterControlAltText="IsMapped" HeaderText="IsMapped" SortExpression="IsMapped" UniqueName="IsMapped" HeaderStyle-Width="150px" FilterControlWidth="70px">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Owner" FilterControlAltText="Owner" HeaderText="Owner" SortExpression="Owner" UniqueName="Owner" HeaderStyle-Width="200px" FilterControlWidth="75px">
                </telerik:GridBoundColumn>

            </Columns>
        </MasterTableView>
   </cc1:IACMRadGridControl>
</asp:Panel>
                      
</asp:Content>
