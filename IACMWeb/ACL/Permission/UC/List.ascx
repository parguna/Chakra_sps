<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="List.ascx.cs" Inherits="DI.Chakra.Web.ACL.Permission.UC.List" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-wrapper">
    <cc1:IACMRadGridControl ID="gvPermission" runat="server" OnNeedDataSource="gvPermission_NeedDataSource" OnSelectedIndexChanged="gvPermission_SelectedIndexChanged">
         <MasterTableView  ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available"  Width="100%" 
             DataKeyNames="Id" TableLayout="Fixed">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="Name" FilterControlAltText="Filter Name column" HeaderText="Name" SortExpression="Name" UniqueName="Name" HeaderStyle-Width="33.33%" FilterControlWidth="86%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="IconURL" FilterControlAltText="Filter IconURL column" HeaderText="Icon URL" SortExpression="IconURL" UniqueName="IconURL" HeaderStyle-Width="33.33%" FilterControlWidth="86%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="ClientScript" FilterControlAltText="Filter ClientScript column" HeaderText="Client Script" SortExpression="ClientScript" UniqueName="ClientScript" HeaderStyle-Width="33.33%" FilterControlWidth="86%">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
   </cc1:IACMRadGridControl>
</div>
