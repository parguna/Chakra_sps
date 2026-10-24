<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Admin.House_Keeping.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Admin/HouseKeeping/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>


<div class="content-wrapper">
    <telerik:RadWindow ID="modalPopup" runat="server" Width="600px" Height="320px" Modal="true" OffsetElementID="main" VisibleStatusbar="false" VisibleTitlebar="false">
        <ContentTemplate>
            <uc1:DetailUC runat="server" ID="DetailUC" OnAdd_CancelButtonClicked="btnCancel_Click" OnAdd_SubmitButtonClicked="btnSubmit_Click" />
        </ContentTemplate>
    </telerik:RadWindow>
    <cc1:IACMRadGridControl ID="PurgeConfig" runat="server" OnNeedDataSource="PurgeConfig_NeedDataSource" OnSelectedIndexChanged="PurgeConfig_SelectedIndexChanged">
   <%-- <telerik:RadGrid runat="server" ID="PurgeConfig" EnableLinqExpressions="false"
        AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True"
        FilterMenu-CssClass="RadMenu_IACMFilterSkin" ClientSettings-EnablePostBackOnRowClick="true"
        OnNeedDataSource="PurgeConfig_NeedDataSource" OnSelectedIndexChanged="PurgeConfig_SelectedIndexChanged">
        <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" AlwaysVisible="true" />
        <ClientSettings>
            <Scrolling UseStaticHeaders="true" />
            <Selecting AllowRowSelect="true" />
        </ClientSettings>
        <GroupingSettings CaseSensitive="false" />--%>
        <MasterTableView  DataKeyNames="Id" NoMasterRecordsText="No Records Available" TableLayout="Fixed" Width="100%" >
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="TableName" FilterControlAltText="TableName" HeaderText="Table Name" SortExpression="TableName" UniqueName="TableName" HeaderStyle-Width="20%" FilterControlWidth="78%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Description" FilterControlAltText="Description" HeaderText="Description" SortExpression="Description" UniqueName="Description" HeaderStyle-Width="20%" FilterControlWidth="78%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="DaysToKeep" FilterControlAltText="DaysToKeep" HeaderText="Days To Keep" SortExpression="DaysToKeep" UniqueName="DaysToKeep" HeaderStyle-Width="15%" FilterControlWidth="69.5%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="PurgeMode" HeaderText="Purge Mode" UniqueName="PurgeMode" SortExpression="PurgeMode" FilterControlAltText="PurgeMode" HeaderStyle-Width="15%" FilterControlWidth="69.5%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Status" FilterControlAltText="Status" HeaderText="Status" SortExpression="Status" UniqueName="Status" HeaderStyle-Width="15%" FilterControlWidth="69.5%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="LastRunOn" FilterControlAltText="LastRunOn" HeaderText="Last Run On" SortExpression="LastRunOn" UniqueName="LastRunOn" HeaderStyle-Width="15%" FilterControlWidth="69.5%">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </cc1:IACMRadGridControl>

    <div class="modal-footer">
        <asp:Button runat="server" class="btn btn-primary transparent-button" ID="btnCancel" Text="Cancel" data-dismiss="modal" OnClick="btnCancel_Click1"></asp:Button>

    </div>
</div>