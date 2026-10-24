<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="List.ascx.cs" Inherits="DI.Chakra.Web.ACL.URL.UC.List" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<div class="content-wrapper">
    <cc1:IACMRadGridControl ID="gvurl" runat="server" OnNeedDataSource="gvurl_NeedDataSource" OnSelectedIndexChanged="gvurl_SelectedIndexChanged">
         <MasterTableView DataKeyNames="Id" TableLayout="Fixed" ShowHeadersWhenNoRecords="false" Width="100%" 
            NoMasterRecordsText="There are no urls to display.">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
                 <telerik:GridBoundColumn DataField="Url" FilterControlAltText="Filter URL" HeaderText="URL" SortExpression="Url" UniqueName="Url" HeaderStyle-Width="100%" FilterControlWidth="95.5%">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
   </cc1:IACMRadGridControl>
</div>
