<%@ Page Title="Inhouse Permissions To Users" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="PermissionUsers.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.INH.PermissionUsers" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="EndpointSystem" TagName="EndpointUC" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left clearfix">
                <div class="row-fluid">
                    <div class="col-xs-7">
                        <EndpointSystem:EndpointUC runat="server" ID="EndpointUC" OnEndpointChanged="DropDownSelectectionChanged" DisplayControl="Application" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:Panel ID="Panel1" runat="server">
        <cc1:IACMRadGridControl ID="grdUser" runat="server" OnNeedDataSource="grdUser_NeedDataSource">
            <MasterTableView TableLayout="fixed"
                EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
                <CommandItemSettings ShowAddNewRecordButton="false"
                    ShowRefreshButton="true"></CommandItemSettings>

                <GroupByExpressions>
                    <telerik:GridGroupByExpression>
                        <SelectFields>
                            <telerik:GridGroupByField HeaderText="" FieldName="PermissionName"></telerik:GridGroupByField>
                        </SelectFields>
                        <GroupByFields>
                            <telerik:GridGroupByField FormatString="{0}:{1}" FieldName="PermissionName" SortOrder="None"></telerik:GridGroupByField>
                        </GroupByFields>
                    </telerik:GridGroupByExpression>
                </GroupByExpressions>

                <Columns>
                    <telerik:GridBoundColumn DataField="UserName" ItemStyle-Width="40%" FilterControlAltText="UserName" HeaderText="User Name" SortExpression="UserName" UniqueName="UserName" HeaderStyle-Width="40%" FilterControlWidth="70%">
                    </telerik:GridBoundColumn>

                    <telerik:GridBoundColumn DataField="displayName" ItemStyle-Width="60%" FilterControlAltText="DisplayName" HeaderText="Display Name" SortExpression="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="60%" FilterControlWidth="70%">
                    </telerik:GridBoundColumn>

                </Columns>
            </MasterTableView>
            <ClientSettings Resizing-AllowColumnResize="true" Resizing-ResizeGridOnColumnResize="true">
            </ClientSettings>
        </cc1:IACMRadGridControl>
    </asp:Panel>
</asp:Content>
