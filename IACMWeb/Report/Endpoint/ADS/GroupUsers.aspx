<%@ Page Title="Group To Users" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="GroupUsers.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.ADS.GroupUsers" %>

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
                            <telerik:GridGroupByField HeaderText="" FieldName="GroupName"></telerik:GridGroupByField>
                        </SelectFields>
                        <GroupByFields>
                            <telerik:GridGroupByField FormatString="{0}:{1}" FieldName="GroupName" SortOrder="None"></telerik:GridGroupByField>
                        </GroupByFields>
                    </telerik:GridGroupByExpression>
                </GroupByExpressions>

                <Columns>
                    <telerik:GridBoundColumn DataField="UserName" ItemStyle-Width="20%" FilterControlAltText="UserName" HeaderText="User Name" SortExpression="UserName" UniqueName="UserName" HeaderStyle-Width="20%" FilterControlWidth="70%">
                    </telerik:GridBoundColumn>

                    <telerik:GridBoundColumn DataField="displayName" ItemStyle-Width="20%" FilterControlAltText="DisplayName" HeaderText="Display Name" SortExpression="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="20%" FilterControlWidth="70%">
                    </telerik:GridBoundColumn>

                    <telerik:GridDateTimeColumn DataField="AccountCreatedDate" ItemStyle-Width="15%" FilterControlAltText="AccountCreatedDate" HeaderText="Account Created Date" SortExpression="AccountCreatedDate" UniqueName="AccountCreatedDate" HeaderStyle-Width="15%" FilterControlWidth="70%">
                    </telerik:GridDateTimeColumn>

                    <telerik:GridBoundColumn DataField="Email" ItemStyle-Width="5%" FilterControlAltText="Email" HeaderText="Email" SortExpression="Email" UniqueName="Email" HeaderStyle-Width="25%" FilterControlWidth="87%">
                    </telerik:GridBoundColumn>

                    <telerik:GridBoundColumn DataField="Department" ItemStyle-Width="5%" FilterControlAltText="Department" HeaderText="Department" SortExpression="Department" UniqueName="Department" HeaderStyle-Width="20%" FilterControlWidth="70%">
                    </telerik:GridBoundColumn>

                    <telerik:GridBoundColumn DataField="FormattedUserOU" ItemStyle-Width="20%" FilterControlAltText="FormattedUserOU" HeaderText="OU" SortExpression="FormattedUserOU" UniqueName="FormattedUserOU" HeaderStyle-Width="20%" FilterControlWidth="70%">
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings Resizing-AllowColumnResize="true" Resizing-ResizeGridOnColumnResize="true">
            </ClientSettings>
       </cc1:IACMRadGridControl>
    </asp:Panel>
</asp:Content>
