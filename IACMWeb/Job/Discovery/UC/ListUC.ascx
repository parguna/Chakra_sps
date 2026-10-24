<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Job.Discovery.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="uc1" TagName="EndpointUC" %>

<div class="content-wrapper">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                    <uc1:EndpointUC runat="server" ID="EndpointUC" DisplayControl="ApplicationType" OnApplicationChanged="EndpointTypeUC_OnEndpointTypeChanged" />
                </div>
            </div>
        </div>
    </div>
</div>

<div class="content-wrapper">
    <cc1:IACMRadGridControl ID="RadDiscoveryJobs" runat="server" OnNeedDataSource="RadDiscoveryJobs_NeedDataSource" OnSelectedIndexChanged="RadDiscoveryJobs_SelectedIndexChanged" EnableLinqExpressions="false" OnItemDataBound="RadDiscoveryJobs_ItemDataBound">
        <MasterTableView  DataKeyNames="DiscoveryProcessId,ProcessName" Width="100%">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="ProcessName" HeaderText="Process Name" SortExpression="ProcessName" UniqueName="ProcessName" DataType="System.String" FilterControlWidth="75.5%" HeaderStyle-Width="20%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="EndpointSystem" HeaderText="EndPoint System" SortExpression="EndPointSystem" UniqueName="EndPointSystem" DataType="System.String" FilterControlWidth="82%" HeaderStyle-Width="20%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Occurence" HeaderText="Occurence" AllowSorting="true" UniqueName="Occurence" AllowFiltering="true" DataType="System.String" HeaderStyle-Width="10%" FilterControlWidth="100%" >
                    <FilterTemplate>
                        <telerik:RadComboBox ID="Occurence" AppendDataBoundItems="true" runat="server" OnSelectedIndexChanged="Occurence_SelectedIndexChanged"
                            AutoPostBack="true" OnPreRender="Occurence_PreRender"  DropDownAutoWidth="Enabled" CssClass="selectpicker AccountsPageComoboWidth radbox100Width" data-width="100%" Width="100%">
                            <Items>
                            </Items>
                        </telerik:RadComboBox>
                    </FilterTemplate>
                </telerik:GridBoundColumn>
                <telerik:GridDateTimeColumn DataField="StartDateTime" HeaderText="Start DateTime" AllowSorting="True" UniqueName="StartDateTime" AllowFiltering="true" DataType="System.DateTime" 
                    FilterControlWidth="76%" HeaderStyle-Width="15%" DataFormatString= "{0:dd-MM-yyyy hh:mm:ss tt}" >
                </telerik:GridDateTimeColumn>
                 <telerik:GridDateTimeColumn DataField="EndDateTime" HeaderText="End DateTime" AllowSorting="True" UniqueName="EndDateTime" AllowFiltering="true" DataType="System.DateTime" 
                    FilterControlWidth="76%" HeaderStyle-Width="15%" DataFormatString= "{0:dd-MM-yyyy hh:mm:ss tt}" >
                </telerik:GridDateTimeColumn>
                <telerik:GridDateTimeColumn DataField="LastRunDate" HeaderText="Last Run DateTime" AllowSorting="True" UniqueName="LastRunDate" AllowFiltering="true" DataType="System.DateTime" 
                    FilterControlWidth="76%" HeaderStyle-Width="15%" DataFormatString= "{0:dd-MM-yyyy hh:mm:ss tt}" >
                </telerik:GridDateTimeColumn>
                 <telerik:GridBoundColumn DataField="ErrorMessage" HeaderText="Error Message" AllowSorting="True" UniqueName="ErrorMessage" AllowFiltering="true" DataType="System.String" FilterControlWidth="78%" HeaderStyle-Width="20%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Status" HeaderText="Status" AllowSorting="True" UniqueName="Status" AllowFiltering="true" DataType="System.String" FilterControlWidth="52.5%" HeaderStyle-Width="10%">
                    <FilterTemplate>
                        <telerik:RadComboBox ID="Status" AppendDataBoundItems="true" runat="server" OnSelectedIndexChanged="Status_SelectedIndexChanged"
                            AutoPostBack="true" OnPreRender="Status_PreRender" CssClass="selectpicker AccountsPageComoboWidth radbox100Width" data-width="100%" DropDownAutoWidth="Enabled">
                            <Items>
                                <telerik:RadComboBoxItem Text="All" Value="0" />
                                <telerik:RadComboBoxItem Text="Pending" Value="1" />
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