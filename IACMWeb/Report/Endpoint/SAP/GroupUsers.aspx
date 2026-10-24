<%@ Page Title="SAP Group To User" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="GroupUsers.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.SAP.GroupUsers" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="Server">
<asp:Panel ID="Panel1" runat="server"> 
    <cc1:IACMRadGridControl ID="grdUser" runat="server" OnNeedDataSource="grdUser_NeedDataSource">
<MasterTableView  TableLayout="fixed"
            DataKeyNames="UserId" EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
            <CommandItemSettings ShowAddNewRecordButton="false"
                ShowRefreshButton="true"></CommandItemSettings>
<GroupByExpressions>
    <telerik:GridGroupByExpression>
        <SelectFields>
            <telerik:GridGroupByField HeaderText="Group Code" FieldName="GroupCode"></telerik:GridGroupByField>
        </SelectFields>
        <GroupByFields>
            <telerik:GridGroupByField FieldName="GroupCode" SortOrder="None"></telerik:GridGroupByField>
        </GroupByFields>
    </telerik:GridGroupByExpression>
</GroupByExpressions>
<Columns>
    <telerik:GridBoundColumn DataField="GroupCode" FilterControlAltText="GroupCode" HeaderText="Group Code" SortExpression="GroupCode" UniqueName="GroupCode" HeaderStyle-Width="15%" FilterControlWidth="70%">
        <ColumnValidationSettings>
            <ModelErrorMessage Text="" />
        </ColumnValidationSettings>
    </telerik:GridBoundColumn>
    <telerik:GridBoundColumn DataField="GroupDescription" FilterControlAltText="GroupDescription" HeaderText="Group Description" SortExpression="GroupDescription" UniqueName="GroupDescription" HeaderStyle-Width="15%" FilterControlWidth="70%">
        <ColumnValidationSettings>
            <ModelErrorMessage Text="" />
        </ColumnValidationSettings>
    </telerik:GridBoundColumn>

    <telerik:GridBoundColumn DataField="UserName" FilterControlAltText="UserName" HeaderText="User Name" SortExpression="UserName" UniqueName="UserName" HeaderStyle-Width="15%" FilterControlWidth="70%">
        <ColumnValidationSettings>
            <ModelErrorMessage Text="" />
        </ColumnValidationSettings>
    </telerik:GridBoundColumn>
    <telerik:GridBoundColumn DataField="DisplayName" FilterControlAltText="DisplayName" HeaderText="DisplayName" SortExpression="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="15%" FilterControlWidth="70%">
        <ColumnValidationSettings>
            <ModelErrorMessage Text="" />
        </ColumnValidationSettings>
    </telerik:GridBoundColumn>
    <telerik:GridBoundColumn DataField="Department" FilterControlAltText="Department" SortExpression="Department" HeaderText="Department" UniqueName="Department" HeaderStyle-Width="15%" FilterControlWidth="70%">
        <ColumnValidationSettings>
            <ModelErrorMessage Text="" />
        </ColumnValidationSettings>
    </telerik:GridBoundColumn>
    <telerik:GridBoundColumn DataField="IsActive" FilterControlAltText="IsActive" SortExpression="IsActive" HeaderText="Active" UniqueName="IsActive" HeaderStyle-Width="25%" FilterControlWidth="70%">
        <ColumnValidationSettings>
            <ModelErrorMessage Text="" />
        </ColumnValidationSettings>
    </telerik:GridBoundColumn>
</Columns>
</MasterTableView>
</cc1:IACMRadGridControl>
        </asp:Panel>
                      
</asp:Content>
