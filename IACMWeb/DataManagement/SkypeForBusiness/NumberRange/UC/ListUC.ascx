<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.DataManagement.SkypeForBusiness.NumberRange.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<div class="content-wrapper">
    <div class="row">
        <div class="col-xs-12">
            <cc1:IACMRadGridControl ID="NumberRangesRadGrid" runat="server" OnItemCommand="NumberRangesRadGrid_ItemCommand" AllowMultiRowEdit="false" AllowMultiRowSelection="false" OnNeedDataSource="NumberRangesRadGrid_NeedDataSource" OnSelectedIndexChanged="NumberRangesRadGrid_SelectedIndexChanged" OnItemDataBound="NumberRangesRadGrid_ItemDataBound" Visible="true">
                 <GroupingSettings CaseSensitive="false" />
            <MasterTableView ShowHeadersWhenNoRecords="true"  NoMasterRecordsText="No Records Available" EditMode="InPlace" InsertItemPageIndexAction="ShowItemOnFirstPage"
                DataKeyNames="Id" AutoGenerateColumns="False" CommandItemDisplay="Top" EnableHeaderContextMenu="true" Width="100%">
                <CommandItemSettings ShowAddNewRecordButton="true" ShowRefreshButton="true" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="true"></CommandItemSettings>
               <Columns>
                    <telerik:GridBoundColumn DataField="StartRange" FilterControlAltText="StartRange" HeaderText="StartRange"
                    SortExpression="StartRange" UniqueName="StartRange" HeaderStyle-Width="15%" FilterControlWidth="65%">
                </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="EndRange" FilterControlAltText="EndRange" HeaderText="EndRange"
                    SortExpression="EndRange" UniqueName="EndRange" HeaderStyle-Width="15%" FilterControlWidth="65%">
                </telerik:GridBoundColumn>
                   <%-- <telerik:GridBoundColumn DataField="Priority" FilterControlAltText="Priority" HeaderText="Priority"
                    SortExpression="Priority" UniqueName="Priority" HeaderStyle-Width="15%" FilterControlWidth="65%">
                </telerik:GridBoundColumn>--%>
                  
              <telerik:GridEditCommandColumn FilterControlWidth="65%" HeaderStyle-Width="10%" UniqueName="Edit" />
        </Columns>
            </MasterTableView>
        </cc1:IACMRadGridControl>
        </div>
    </div>
</div>