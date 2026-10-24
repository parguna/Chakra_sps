<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="List.ascx.cs" Inherits="DI.Chakra.Web.AppCustomization.AdminModeItem.UC.List" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-wrapper">
    <cc1:IACMRadGridControl ID="gvAdminItem" runat="server" OnNeedDataSource="gvAdminItem_NeedDataSource" OnSelectedIndexChanged="gvAdminItem_SelectedIndexChanged">
         <MasterTableView ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available" Width="100%" 
             DataKeyNames="Id" TableLayout="Fixed">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="Name" FilterControlAltText="Filter Name " HeaderText="Name" SortExpression="Name" UniqueName="Name">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Description" FilterControlAltText="Filter Description" HeaderText="Description" SortExpression="Description" UniqueName="Description">
                </telerik:GridBoundColumn>
                 <telerik:GridBoundColumn DataField="Icon" FilterControlAltText="Filter Icon" HeaderText="Icon" SortExpression="Icon" UniqueName="Icon">
                </telerik:GridBoundColumn>
                  <telerik:GridBoundColumn DataField="Information" FilterControlAltText="Filter Information" HeaderText="Information" SortExpression="Information" UniqueName="Information">
                </telerik:GridBoundColumn>
                  <telerik:GridBoundColumn DataField="Type" FilterControlAltText="Filter Type" HeaderText="Type" SortExpression="Type" UniqueName="Type">
                </telerik:GridBoundColumn>
                  <telerik:GridBoundColumn DataField="TabMenu.MenuName" FilterControlAltText="Filter Menu" HeaderText="Menu" SortExpression="MenuName" UniqueName="MenuName">
                </telerik:GridBoundColumn>
               <telerik:GridBoundColumn DataField="TableName" Display="false" FilterControlAltText="Filter Table Name" HeaderText="Table Name" SortExpression="TableName" UniqueName="TableName">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </cc1:IACMRadGridControl>
</div>