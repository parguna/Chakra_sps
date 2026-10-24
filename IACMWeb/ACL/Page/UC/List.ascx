<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="List.ascx.cs" Inherits="DI.Chakra.Web.ACL.Page.UC.List" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-wrapper">
    <cc1:IACMRadGridControl ID="gvPage" runat="server" OnNeedDataSource="gvPage_NeedDataSource" OnSelectedIndexChanged="gvPage_SelectedIndexChanged">
         <MasterTableView  Width="100%"  ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available"
            DataKeyNames="Id" TableLayout="Fixed">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="UniqueName" FilterControlAltText="Filter UniqueName" HeaderText="Unique Name" SortExpression="UniqueName" UniqueName="UniqueName" HeaderStyle-Width="33.33%" FilterControlWidth="86%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Name" FilterControlAltText="Filter Name " HeaderText="Name" SortExpression="Name" UniqueName="Name" HeaderStyle-Width="33.33%" FilterControlWidth="86%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="PageUrl.Url" FilterControlAltText="Filter PageUrl.Url" HeaderText="URL" SortExpression="PageUrl.Url" UniqueName="PageUrl.Url" HeaderStyle-Width="33.33%" FilterControlWidth="86%">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
   </cc1:IACMRadGridControl>
</div>
