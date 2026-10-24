<%@ Page Language="C#" Title="Login Status" AutoEventWireup="true" MasterPageFile="~/Master/ProtoMast4.master" CodeBehind="LoginStatus.aspx.cs" Inherits="DI.Chakra.Web.Report.Audit.LoginStatus" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">

    <asp:Panel ID="Panel1" runat="server">

        <cc1:IACMRadGridControl ID="EmployeeGrid" runat="server" OnNeedDataSource="EmployeesGrid_NeedDataSource">
            <MasterTableView TableLayout="fixed"
                DataKeyNames="Message" EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
                <CommandItemSettings ShowAddNewRecordButton="false"
                    ShowRefreshButton="true"></CommandItemSettings>
                <Columns>

                    <telerik:GridBoundColumn SortExpression="Message" HeaderText="Message" HeaderButtonType="TextButton" DataField="Message" HeaderStyle-Width="70%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="CreatedBy" HeaderText="CreatedBy" HeaderButtonType="TextButton" DataField="CreatedBy" HeaderStyle-Width="15%">
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn DataFormatString= "{0:dd-MM-yyyy hh:mm:ss tt}" SortExpression="CreatedDate" HeaderText="CreatedDate" HeaderButtonType="TextButton" DataField="CreatedDate" HeaderStyle-Width="15%">
                    </telerik:GridDateTimeColumn>

                </Columns>

            </MasterTableView>
        </cc1:IACMRadGridControl>
    </asp:Panel>
</asp:Content>


