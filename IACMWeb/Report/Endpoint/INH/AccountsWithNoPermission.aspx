<%@ Page Title="Inhouse Accounts with No Permissions" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="AccountsWithNoPermission.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.INH.AccountsWithNoPermission" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="EndpointSystem" TagName="EndpointUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="ParentPanel" runat="server">
        <ContentTemplate>


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

            <br />
            <cc1:IACMRadGridControl ID="grdAccountsWithNoGroups" runat="server" OnNeedDataSource="grdAccountsWithNoGroups_NeedDataSource">
                <MasterTableView  TableLayout="fixed"
                    DataKeyNames="UserId" EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
                    <CommandItemSettings ShowAddNewRecordButton="false"
                        ShowRefreshButton="true"></CommandItemSettings>
                    <Columns>
                        <telerik:GridBoundColumn DataField="userName" FilterControlAltText="userName" HeaderText="User Name" SortExpression="userName" UniqueName="userName" HeaderStyle-Width="200px" FilterControlWidth="70px">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="displayName" FilterControlAltText="DisplayName" HeaderText="Display Name" SortExpression="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="200px" FilterControlWidth="70px">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Email" FilterControlAltText="Email" SortExpression="Email" HeaderText="Email" UniqueName="Email" HeaderStyle-Width="150px" FilterControlWidth="70px">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Owner" FilterControlAltText="Owner" HeaderText="Owner" SortExpression="Owner" UniqueName="Owner" HeaderStyle-Width="200px" FilterControlWidth="75px">
                        </telerik:GridBoundColumn>


                    </Columns>
                </MasterTableView>
            </cc1:IACMRadGridControl>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
