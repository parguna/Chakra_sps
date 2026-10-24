<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="List.ascx.cs" Inherits="DI.Chakra.Web.AppCustomization.Dashboard.DB.UC.List" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

   
<div class="content-wrapper">
    <cc1:IACMRadGridControl ID="gvDashboard" runat="server" OnNeedDataSource="gvDashboard_NeedDataSource" OnSelectedIndexChanged="gvDashboard_SelectedIndexChanged">
         <MasterTableView  ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available" Width="100%" 
             DataKeyNames="Id" TableLayout="Fixed">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
                
                <telerik:GridBoundColumn DataField="DashboardName" FilterControlAltText="Filter Name " HeaderText="Name" SortExpression="DashboardName" UniqueName="DashboardName">
                </telerik:GridBoundColumn>
                 <telerik:GridBoundColumn DataField="DashboardCategory" FilterControlAltText="Filter Category" HeaderText="Category" SortExpression="DashboardCategory" UniqueName="DashboardCategory">
                </telerik:GridBoundColumn>
                 <telerik:GridBoundColumn DataField="DashBoardType" FilterControlAltText="Filter Type" HeaderText="Type" SortExpression="DashBoardType" UniqueName="DashBoardType">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="TargetUrl" FilterControlAltText="Filter Target Url " HeaderText="Target Url" SortExpression="TargetUrl" UniqueName="TargetUrl">
                </telerik:GridBoundColumn>
                 <telerik:GridBoundColumn DataField="NavigateUrl" FilterControlAltText="Filter Navigate Url" HeaderText="Navigate Url" SortExpression="NavigateUrl" UniqueName="NavigateUrl">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
  </cc1:IACMRadGridControl>
</div>