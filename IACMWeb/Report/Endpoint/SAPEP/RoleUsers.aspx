<%@ Page Title="SAP Enterprise Portal Role To Users Details" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="RoleUsers.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.SAPEP.RoleUsers" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="EndpointSystem" TagName="EndpointUC" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left">
                <div class="row-fluid">
                    <div class="col-xs-12">
                        <div class="form-group">
                            <EndpointSystem:EndpointUC runat="server" ID="EndpointUC" OnEndpointChanged="DropDownSelectectionChanged" DisplayControl="Application" />

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:Panel ID="Panel1" runat="server">
        <telerik:RadGrid ID="grdRole" FilterMenu-CssClass="RadMenu_IACMFilterSkin" runat="server" AllowFilteringByColumn="True"
            OnNeedDataSource="grdRole_NeedDataSource" AllowPaging="True" AllowSorting="True">
            <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" />
            <ClientSettings>
                <Scrolling UseStaticHeaders="true" />
                <Selecting AllowRowSelect="true" />
            </ClientSettings>
            <GroupingSettings CaseSensitive="false" />
            <MasterTableView AutoGenerateColumns="False" ShowHeadersWhenNoRecords="false" CommandItemDisplay="None" NoMasterRecordsText="No Records Available" EnableHeaderContextMenu="false"  Width="100%">
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
                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                <Columns>
                    <telerik:GridBoundColumn DataField="RoleName" FilterControlAltText="RoleName" HeaderText="Role Name" SortExpression="RoleName" UniqueName="RoleName" HeaderStyle-Width="15%" FilterControlWidth="85%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="UserName" FilterControlAltText="UserName" HeaderText="User Name" SortExpression="UserName" UniqueName="UserName" HeaderStyle-Width="15%" FilterControlWidth="85%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="DisplayName" FilterControlAltText="DisplayName" HeaderText="Display Name" SortExpression="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="18%" FilterControlWidth="85%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="InternetAddress" FilterControlAltText="InternetAddress" HeaderText="Email" SortExpression="InternetAddress" UniqueName="InternetAddress" HeaderStyle-Width="20%" FilterControlWidth="85%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="JobTitle" FilterControlAltText="JobTitle" HeaderText="Role" SortExpression="JobTitle" UniqueName="JobTitle" HeaderStyle-Width="15%" FilterControlWidth="80%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Department" FilterControlAltText="Department" HeaderText="Department" SortExpression="Department" UniqueName="Department" HeaderStyle-Width="15%" FilterControlWidth="80%">
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </asp:Panel>
</asp:Content>
