<%@ Page Title="Export to Excel" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="ExportToExcel.aspx.cs" Inherits="DI.Chakra.Web.Admin.Attestation.ExportToExcel" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="Server">
    <asp:Button ID="ExportExcelButton" Visible="true" runat="server" OnClick="ExportExcelButton_Click" Text="Export To Excel" CssClass="btn btn-default btn-sm" />&nbsp;&nbsp;
    <telerik:RadGrid ID="grdExport"
        runat="server" AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" CellSpacing="0"
        FilterMenu-CssClass="RadMenu_IACMFilterSkin" 
        OnNeedDataSource="grdExport_NeedDataSource" >
        <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" AlwaysVisible="true" />
        <GroupingSettings CaseSensitive="false" />
        <MasterTableView AutoGenerateColumns="False" ShowHeadersWhenNoRecords="true" CommandItemDisplay="None" NoMasterRecordsText="No Records Available" EnableHeaderContextMenu="false">
            <Columns>
                <telerik:GridBoundColumn DataField="UserName" FilterControlAltText="UserName" HeaderText="User Name" SortExpression="UserName" UniqueName="UserName" HeaderStyle-Width="20%" FilterControlWidth="80%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="RoleCode" FilterControlAltText="RoleCode" HeaderText="Role Code" SortExpression="RoleCode" UniqueName="RoleCode" HeaderStyle-Width="20%" FilterControlWidth="80%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="TCode" FilterControlAltText="TCode" HeaderText="TCode" SortExpression="TCode" UniqueName="TCode" HeaderStyle-Width="20%" FilterControlWidth="80%">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Content>
