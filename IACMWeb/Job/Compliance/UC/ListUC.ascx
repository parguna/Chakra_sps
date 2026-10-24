<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Job.Compliance.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-wrapper">
    <cc1:IACMRadGridControl ID="RadComplianceJobs" runat="server" EnableLinqExpressions="false"  OnNeedDataSource="RadComplianceJobs_NeedDataSource" OnItemCommand="RadComplianceJobs_ItemCommand" OnItemDataBound="RadComplianceJobs_ItemDataBound" OnSelectedIndexChanged="RadComplianceJobs_SelectedIndexChanged" OnInit="RadComplianceJobs_Init">
        <MasterTableView DataKeyNames="ComplianceProcessId">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="ProcessName" HeaderText="ProcessName" SortExpression="ProcessName" UniqueName="ProcessName" DataType="System.String" FilterControlWidth="85%" HeaderStyle-Width="30%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="PolicyName" HeaderText="PolicyName" SortExpression="PolicyName" UniqueName="PolicyName" DataType="System.String" FilterControlWidth="76.5%" HeaderStyle-Width="20%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Occurence" HeaderText="Occurence" AllowSorting="true" UniqueName="Occurence" AllowFiltering="true" DataType="System.String" HeaderStyle-Width="10%">
                    <FilterTemplate>
                        <telerik:RadComboBox ID="Occurence" AppendDataBoundItems="true" runat="server" OnSelectedIndexChanged="Occurence_SelectedIndexChanged"
                            AutoPostBack="true" OnPreRender="Occurence_PreRender" DropDownAutoWidth="Enabled" CssClass="selectpicker AccountsPageComoboWidth radbox100Width" data-width="100%">
                            <Items>
                                <telerik:RadComboBoxItem Text="All" Value="All" />
                            </Items>
                        </telerik:RadComboBox>
                    </FilterTemplate>
                </telerik:GridBoundColumn>
                <telerik:GridDateTimeColumn DataField="StartDateTime" HeaderText="Start DateTime" AllowSorting="True" UniqueName="StartDateTime" AllowFiltering="true" DataType="System.Datetime" FilterControlWidth="52.5%" HeaderStyle-Width="10%">
                </telerik:GridDateTimeColumn>
                <telerik:GridBoundColumn DataField="Status" HeaderText="Status" AllowSorting="True" UniqueName="Status" AllowFiltering="true" DataType="System.String" FilterControlWidth="52.5%" HeaderStyle-Width="10%">
                    <FilterTemplate>
                        <telerik:RadComboBox ID="Status" AppendDataBoundItems="true" runat="server" OnSelectedIndexChanged="Status_SelectedIndexChanged"
                            AutoPostBack="true" OnPreRender="Status_PreRender" DropDownAutoWidth="Enabled" CssClass="selectpicker AccountsPageComoboWidth radbox100Width" data-width="100%">
                            <Items>
                                <telerik:RadComboBoxItem Text="All" Value="0" />
                                <telerik:RadComboBoxItem Text="Pending" Value="1" />
                                <telerik:RadComboBoxItem Text="InProgress" Value="2" />
                                <telerik:RadComboBoxItem Text="Completed" Value="3" />
                            </Items>
                        </telerik:RadComboBox>
                    </FilterTemplate>
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
  </cc1:IACMRadGridControl>

    <%--<div class="buttons text-right nav-rectangle">
        <asp:Button ID="btncancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="btncancel_Click" />
    </div>--%>
</div>