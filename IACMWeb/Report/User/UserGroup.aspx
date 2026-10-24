<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Master/ProtoMast4.master" CodeBehind="UserGroup.aspx.cs" Inherits="DI.Chakra.Web.Report.User.UserGroup" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">


    <asp:Panel ID="Panel1" runat="server">
        <cc1:IACMRadGridControl ID="UsergroupGrid" runat="server" OnNeedDataSource="Usergroup_NeedDataSource">
            <MasterTableView TableLayout="fixed"  Width="100%"
                DataKeyNames="UserName" EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
                <CommandItemSettings ShowAddNewRecordButton="false"
                    ShowRefreshButton="true"></CommandItemSettings>
                <GroupByExpressions>
                    <telerik:GridGroupByExpression>
                        <SelectFields>
                            <telerik:GridGroupByField HeaderText="" FieldName="GroupName"></telerik:GridGroupByField>
                        </SelectFields>
                        <GroupByFields>
                            <telerik:GridGroupByField FormatString="{0}:{1}" FieldName="GroupName" SortOrder="none"></telerik:GridGroupByField>
                        </GroupByFields>
                        <SelectFields>
                            <telerik:GridGroupByField HeaderText="" FieldName="Description"></telerik:GridGroupByField>
                        </SelectFields>
                        <GroupByFields>
                            <telerik:GridGroupByField FormatString="{0}:{1}" FieldName="Description" SortOrder="none"></telerik:GridGroupByField>
                        </GroupByFields>
                    </telerik:GridGroupByExpression>
                </GroupByExpressions>
                <Columns>
                    <telerik:GridBoundColumn SortExpression="UserName" HeaderText="User Name" HeaderButtonType="TextButton" DataField="UserName" HeaderStyle-Width="50%" FilterControlWidth="80%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="DisplayName" HeaderText="Display Name" HeaderButtonType="TextButton" DataField="DisplayName" HeaderStyle-Width="25%">
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn SortExpression="JoinDate" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" HeaderText="Join Date" HeaderButtonType="TextButton" DataField="JoinDate" HeaderStyle-Width="19%" FilterControlWidth="45%">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn SortExpression="GroupName" HeaderText="Group Name" HeaderButtonType="TextButton" DataField="GroupName" HeaderStyle-Width="40%" FilterControlWidth="80%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="Owner" HeaderText="Group Owner" HeaderButtonType="TextButton" DataField="Owner" HeaderStyle-Width="25%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="IsDefault" HeaderText="IsDefault Group" HeaderButtonType="TextButton" DataField="IsDefault" HeaderStyle-Width="10%">
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn SortExpression="CreatedDate" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" HeaderText="CreatedDate" HeaderButtonType="TextButton" DataField="CreatedDate" HeaderStyle-Width="12%" FilterControlWidth="60%">
                    </telerik:GridDateTimeColumn>
                </Columns>

            </MasterTableView>
            <ClientSettings Resizing-AllowColumnResize="true" Resizing-ResizeGridOnColumnResize="true">
            </ClientSettings>
        </cc1:IACMRadGridControl>
    </asp:Panel>
</asp:Content>


