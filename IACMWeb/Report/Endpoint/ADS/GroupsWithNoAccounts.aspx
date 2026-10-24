<%@ Page Title="Groups with No Accounts" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="GroupsWithNoAccounts.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.ADS.GroupsWithNoAccounts" %>

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

            <cc1:IACMRadGridControl ID="grdGroupsWithNoAccounts" runat="server" OnNeedDataSource="grdGroupsWithNoAccounts_NeedDataSource">
                <MasterTableView TableLayout="fixed"
                    DataKeyNames="GroupId" EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
                    <CommandItemSettings ShowAddNewRecordButton="false"
                        ShowRefreshButton="true"></CommandItemSettings>
                    <Columns>
                        <telerik:GridBoundColumn DataField="sAMAccountName" FilterControlAltText="sAMAccountName" HeaderText="User Name" SortExpression="sAMAccountName" UniqueName="sAMAccountName" HeaderStyle-Width="200px" FilterControlWidth="70px">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="cn" FilterControlAltText="DisplayName" HeaderText="Display Name" SortExpression="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="200px" FilterControlWidth="70px">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="whenCreated" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" FilterControlAltText="whenCreated" HeaderText="whenCreated" SortExpression="whenCreated" UniqueName="whenCreated" HeaderStyle-Width="200px" ItemStyle-Width="200px" FilterControlWidth="70px">
                        </telerik:GridBoundColumn>


                    </Columns>
                </MasterTableView>
           </cc1:IACMRadGridControl>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
