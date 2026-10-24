<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Job.Attestation.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-wrapper">
    <cc1:IACMRadGridControl ID="RadAttestationJobs" runat="server" EnableLinqExpressions="false" OnNeedDataSource="RadAttestationJobs_NeedDataSource" OnSelectedIndexChanged="RadAttestationJobs_SelectedIndexChanged" OnItemDataBound="RadAttestationJobs_ItemDataBound" >
        <MasterTableView  DataKeyNames="AttestationProcessId" Width="100%">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="ProcessName" HeaderText="Process Name" SortExpression="ProcessName" UniqueName="ProcessName" DataType="System.String" FilterControlWidth="79%" HeaderStyle-Width="20%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="AttestationName" HeaderText="Attestation Name" SortExpression="AttestationName" UniqueName="AttestationName" DataType="System.String" FilterControlWidth="72%" HeaderStyle-Width="15%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Occurence" HeaderText="Occurrence" AllowSorting="true" UniqueName="Occurence" AllowFiltering="true" DataType="System.String" HeaderStyle-Width="10%">
                    <FilterTemplate>
                        <telerik:RadComboBox ID="Occurence" AppendDataBoundItems="true" runat="server" OnSelectedIndexChanged="Occurence_SelectedIndexChanged"
                            AutoPostBack="true" OnPreRender="Occurence_PreRender" DropDownAutoWidth="Enabled" CssClass="selectpicker AccountsPageComoboWidth radbox100Width" data-width="100%">
                        </telerik:RadComboBox>
                    </FilterTemplate>
                </telerik:GridBoundColumn>
                <telerik:GridDateTimeColumn DataField="StartDateTime" HeaderText="Start Date Time" AllowSorting="True" UniqueName="StartDateTime" AllowFiltering="true" DataType="System.DateTime" DataFormatString= "{0:dd/MM/yyyy hh:mm:ss tt}" FilterControlWidth="72%" HeaderStyle-Width="15%">
                </telerik:GridDateTimeColumn>
                  <telerik:GridDateTimeColumn DataField="LastRunDate" HeaderText="Last Run DateTime" AllowSorting="True" UniqueName="LastRunDate" AllowFiltering="true" DataType="System.DateTime" DataFormatString= "{0:dd/MM/yyyy hh:mm:ss tt}" FilterControlWidth="72%" HeaderStyle-Width="15%">
                </telerik:GridDateTimeColumn>
                 <telerik:GridBoundColumn DataField="ErrorMessage" HeaderText="Error Message" AllowSorting="True" UniqueName="ErrorMessage" AllowFiltering="true" DataType="System.String" FilterControlWidth="79%" HeaderStyle-Width="20%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Status" HeaderText="Status" AllowSorting="True" UniqueName="Status" AllowFiltering="true" DataType="System.String" FilterControlWidth="57%" HeaderStyle-Width="10%">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </cc1:IACMRadGridControl>
</div>
