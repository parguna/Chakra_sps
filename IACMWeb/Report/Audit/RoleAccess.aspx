<%@ Page Title="Access Control Matrix" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="RoleAccess.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.Audit.RoleAccess" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="EndpointSystem" TagName="EndpointUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    
    <asp:Panel ID="Panel1" runat="server">
        <cc1:IACMRadGridControl ID="grdUser" runat="server" OnNeedDataSource="grdUser_NeedDataSource" OnDetailTableDataBind="grdUser_DetailTableDataBind">
            <MasterTableView TableLayout="fixed"
                EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
                <CommandItemSettings ShowAddNewRecordButton="false"
                    ShowRefreshButton="true"></CommandItemSettings>
                <GroupByExpressions>
                    <telerik:GridGroupByExpression>
                        <SelectFields>
                            <telerik:GridGroupByField HeaderText="Role Name" FieldName="RoleName"></telerik:GridGroupByField>
                        </SelectFields>
                        <GroupByFields>
                            <telerik:GridGroupByField FieldName="RoleName" SortOrder="None"></telerik:GridGroupByField>
                        </GroupByFields>
                    </telerik:GridGroupByExpression>
                </GroupByExpressions>
                <Columns>
                    <telerik:GridBoundColumn DataField="PageName" HeaderStyle-Width="30%" ItemStyle-Width="30%" FilterControlWidth="80%" FilterControlAltText="PageName" HeaderText="Page Name" SortExpression="PageName" UniqueName="PageName">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="AccessRights" HeaderStyle-Width="70%" ItemStyle-Width="70%" FilterControlWidth="90%" FilterControlAltText="AccessRights" HeaderText="Access Rights" SortExpression="AccessRights" UniqueName="AccessRights">
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings Resizing-AllowColumnResize="true" Resizing-ResizeGridOnColumnResize="true">
            </ClientSettings>
       </cc1:IACMRadGridControl>
    </asp:Panel>
</asp:Content>
