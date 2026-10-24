<%@ Page Title="Inhouse Permissions" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Permissions.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.INH.Permissions" %>

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
                DataKeyNames="Id" EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
                <CommandItemSettings ShowAddNewRecordButton="false"
                    ShowRefreshButton="true"></CommandItemSettings>

                <Columns>
                    <telerik:GridBoundColumn DataField="name" FilterControlAltText="name" HeaderText="Name" SortExpression="Name" UniqueName="Name" HeaderStyle-Width="10%" FilterControlWidth="80%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="description" FilterControlAltText="Description" HeaderText="Description" SortExpression="Description" UniqueName="Description" HeaderStyle-Width="19%" FilterControlWidth="80%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="CanRequest" FilterControlAltText="CanRequest" HeaderText="Can Request" SortExpression="CanRequest" UniqueName="CanRequest" HeaderStyle-Width="10%" FilterControlWidth="80%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="UserDefinedDesc" FilterControlAltText="UserDefinedDesc" HeaderText="Custom Description" SortExpression="UserDefinedDesc" UniqueName="UserDefinedDesc" HeaderStyle-Width="6%" FilterControlWidth="80%">
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
        </cc1:IACMRadGridControl>
    </asp:Panel>
</asp:Content>
