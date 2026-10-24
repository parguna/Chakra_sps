<%@ Page Title="Users To Groups" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="UserGroups.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.ADS.UserGroups" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="EndpointSystem" TagName="EndpointUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left clearfix">
                <div class="row-fluid">
                    <div class="col-xs-7">
                        <%-- <endpointsystem:endpointsystemuc ID="EndpointUC" OnEndpointChanged="EndpointUC_EndpointChanged" DisplayControl="Application" OnDropDownSelectection="DropDownSelectectionChanged" runat="server"></endpointsystem:endpointsystemuc>--%>
                        <EndpointSystem:EndpointUC runat="server" ID="EndpointUC" OnEndpointChanged="DropDownSelectectionChanged" DisplayControl="Application" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:Panel ID="Panel1" runat="server">
        <cc1:IACMRadGridControl ID="grdUser" runat="server" OnNeedDataSource="grdUser_NeedDataSource" OnDetailTableDataBind="grdUser_DetailTableDataBind">
            <MasterTableView TableLayout="fixed"
                EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
                <CommandItemSettings ShowAddNewRecordButton="false"
                    ShowRefreshButton="true"></CommandItemSettings>
                <GroupByExpressions>
                    <telerik:GridGroupByExpression>
                        <SelectFields>
                            <telerik:GridGroupByField HeaderText="User Name" FieldName="UserName"></telerik:GridGroupByField>
                        </SelectFields>
                        <GroupByFields>
                            <telerik:GridGroupByField FieldName="UserName" SortOrder="None"></telerik:GridGroupByField>
                        </GroupByFields>
                    </telerik:GridGroupByExpression>
                </GroupByExpressions>
                <Columns>
                    <telerik:GridBoundColumn DataField="GroupName" HeaderStyle-Width="30%" ItemStyle-Width="30%" FilterControlWidth="80%" FilterControlAltText="GroupName" HeaderText="Group Name" SortExpression="GroupName" UniqueName="GroupName">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="GroupDescription" HeaderStyle-Width="70%" ItemStyle-Width="70%" FilterControlWidth="90%" FilterControlAltText="GroupDescription" HeaderText="Group Description" SortExpression="GroupDescription" UniqueName="GroupDescription">
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings Resizing-AllowColumnResize="true" Resizing-ResizeGridOnColumnResize="true">
            </ClientSettings>
        </cc1:IACMRadGridControl>
    </asp:Panel>
</asp:Content>
