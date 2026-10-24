<%@ Page Title="Accounts with No Groups" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="AccountsWithNoGroups.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.ADS.AccountsWithNoGroups" %>

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
                <MasterTableView TableLayout="fixed"
                    DataKeyNames="UserId" EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
                    <CommandItemSettings ShowAddNewRecordButton="false"
                        ShowRefreshButton="true"></CommandItemSettings>
                    <Columns>
                        <telerik:GridBoundColumn DataField="sAMAccountName" FilterControlAltText="sAMAccountName" HeaderText="User Name" SortExpression="sAMAccountName" UniqueName="sAMAccountName" HeaderStyle-Width="200px" FilterControlWidth="70px">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="displayName" FilterControlAltText="DisplayName" HeaderText="Display Name" SortExpression="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="200px" FilterControlWidth="70px">
                        </telerik:GridBoundColumn>
                        <%--                        <telerik:GridBoundColumn DataField="ou" FilterControlAltText="ou" HeaderText="OU" SortExpression="ou" UniqueName="ou" HeaderStyle-Width="200px" ItemStyle-Width="200px" FilterControlWidth="70px">
                        </telerik:GridBoundColumn>--%>
                        <telerik:GridBoundColumn DataField="department" FilterControlAltText="department" SortExpression="department" HeaderText="Department" UniqueName="department" HeaderStyle-Width="150px" FilterControlWidth="70px">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Owner" FilterControlAltText="Owner" HeaderText="Owner" SortExpression="Owner" UniqueName="Owner" HeaderStyle-Width="200px" FilterControlWidth="75px">
                        </telerik:GridBoundColumn>
                        <%--<telerik:GridBoundColumn DataField="physicalDeliveryOfficeName" FilterControlAltText="physicalDeliveryOfficeName" HeaderText="Employee Number" SortExpression="physicalDeliveryOfficeName" UniqueName="physicalDeliveryOfficeName" HeaderStyle-Width="200px" FilterControlWidth="75px">
                        </telerik:GridBoundColumn>--%>
                        <telerik:GridBoundColumn DataField="company" FilterControlAltText="company" HeaderText="Company" SortExpression="company" UniqueName="company" HeaderStyle-Width="200px" FilterControlWidth="75px">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="businessCategory" FilterControlAltText="businessCategory" HeaderText="Branch" SortExpression="businessCategory" UniqueName="businessCategory" HeaderStyle-Width="200px" FilterControlWidth="80px">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="division" FilterControlAltText="division" HeaderText="Division" SortExpression="division" UniqueName="division" HeaderStyle-Width="200px" FilterControlWidth="80px">
                        </telerik:GridBoundColumn>


                    </Columns>
                </MasterTableView>
            </cc1:IACMRadGridControl>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
