<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Job.CustomReport.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-wrapper">
    <cc1:IACMRadGridControl ID="RadReportJobs" runat="server"  OnNeedDataSource="RadReportJobs_NeedDataSource" EnableLinqExpressions="false"
        OnSelectedIndexChanged="RadReportJobs_SelectedIndexChanged" OnInit="RadReportJobs_Init">
        <MasterTableView DataKeyNames="Id"  Width="100%" >
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="Name" HeaderText="Name" SortExpression="Name" UniqueName="Name" DataType="System.String" FilterControlWidth="85%" HeaderStyle-Width="20%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Description" HeaderText="Description" SortExpression="Description" UniqueName="Description" DataType="System.String" FilterControlWidth="76.5%" HeaderStyle-Width="15%">
                </telerik:GridBoundColumn>
                <telerik:GridDateTimeColumn DataField="CreatedDate" HeaderText="Created Date" AllowSorting="True" UniqueName="CreatedDate" AllowFiltering="true"  DataFormatString= "{0:dd-MM-yyyy hh:mm:ss tt}"  DataType="System.Datetime"  FilterControlWidth="80.5%" HeaderStyle-Width="10%">
                </telerik:GridDateTimeColumn>
                <telerik:GridDateTimeColumn DataField="LastRunOn" HeaderText="Last Run DateTime" AllowSorting="True" UniqueName="LastRunOn" AllowFiltering="true"  DataFormatString= "{0:dd-MM-yyyy hh:mm:ss tt}"  DataType="System.Datetime" FilterControlWidth="80.5%" HeaderStyle-Width="10%">
                </telerik:GridDateTimeColumn>
                <telerik:GridBoundColumn DataField="IsLinkedToMenu" HeaderText="Linked To Menu" AllowSorting="True" UniqueName="IsLinkedToMenu" AllowFiltering="true" FilterControlWidth="80.5%" HeaderStyle-Width="10%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Status" HeaderText="Status" AllowSorting="True" UniqueName="Status" AllowFiltering="true" DataType="System.String" FilterControlWidth="52.5%" HeaderStyle-Width="10%">
                    <FilterTemplate>
                        <telerik:RadComboBox ID="Status" AppendDataBoundItems="true" runat="server" OnSelectedIndexChanged="Status_SelectedIndexChanged"
                            AutoPostBack="true" OnPreRender="Status_PreRender" DropDownAutoWidth="Enabled" CssClass="selectpicker AccountsPageComoboWidth radbox100Width" data-width="100%">
                            <Items>
                                <telerik:RadComboBoxItem Text="All" Value="0" />
                                <telerik:RadComboBoxItem Text="InProgress" Value="2" />
                                <telerik:RadComboBoxItem Text="Completed" Value="3" />
                                <telerik:RadComboBoxItem Text="Failed" Value="4" />
                            </Items>
                        </telerik:RadComboBox>
                    </FilterTemplate>
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
  </cc1:IACMRadGridControl>
</div>