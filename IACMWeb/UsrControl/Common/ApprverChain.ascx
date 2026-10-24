<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApprverChain.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.ApprverChain" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<style type="text/css">
    .ApproverCommonWidth{
        min-width:120px !important; 
    }
</style>
<telerik:RadGrid ID="grdApprover" runat="server" OnNeedDataSource="grdApprover_NeedDataSource" AllowPaging="false" AllowSorting="false"
    CellSpacing="0" GridLines="None" Width="100%" OnItemDataBound="grdApprover_ItemDataBound" OnDetailTableDataBind="grdApprover_DetailTableDataBind">
    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="20" />
    <ExportSettings
        HideStructureColumns="true"
        IgnorePaging="true" ExportOnlyData="true"
        Excel-Format="ExcelML"
        OpenInNewWindow="true">
        <Pdf PageHeight="210mm" AllowPrinting="true" PageWidth="297mm" DefaultFontFamily="Arial Unicode MS" PageTopMargin="45mm"
            BorderStyle="Medium" BorderColor="#666666" BorderType="AllBorders">
        </Pdf>
    </ExportSettings>
    <GroupingSettings CaseSensitive="false" />
    <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" NoMasterRecordsText="No previous approvals" DataKeyNames="ApprovalState"  Width="100%">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="true" ShowExportToExcelButton="false" ShowExportToPdfButton="false" ShowExportToCsvButton="false"></CommandItemSettings>
        <DetailTables>
            <telerik:GridTableView AutoGenerateColumns="false" EnableHierarchyExpandAll="true" ShowHeader="true" Width="100%"
                runat="server" Name="Entitlements" NoDetailRecordsText="No records found." AllowPaging="false" AllowFilteringByColumn="false">
                <Columns>
                    <telerik:GridBoundColumn DataField="TaskOwner" FilterControlAltText="TaskOwner" SortExpression="TaskOwner" HeaderText="Approver Name" UniqueName="TaskOwner"  HeaderStyle-Width="15%" ItemStyle-CssClass="ApproverCommonWidth">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Status" FilterControlAltText="Status" SortExpression="Status" HeaderText="Status" UniqueName="Status" HeaderStyle-Width="5%"  ItemStyle-CssClass="ApproverCommonWidth">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="ActionDate" FilterControlAltText="ActionDate" SortExpression="ActionDate" HeaderText="ActionDate" UniqueName="ActionDate" HeaderStyle-Width="13%"  ItemStyle-CssClass="ApproverCommonWidth">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Comments" FilterControlAltText="Comments" SortExpression="Comments" HeaderText="Comments" UniqueName="Comments" HeaderStyle-Width="25%"  ItemStyle-CssClass="ApproverCommonWidth">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="DelegatedFrom" FilterControlAltText="DelegatedFrom" SortExpression="DelegatedFrom" HeaderText="Delegated From" UniqueName="DelegatedFrom" HeaderStyle-Width="10%"  ItemStyle-CssClass="ApproverCommonWidth">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="DelegatedBy" FilterControlAltText="DelegatedBy" SortExpression="DelegatedBy" HeaderText="Delegated By" UniqueName="DelegatedBy" HeaderStyle-Width="10%"  ItemStyle-CssClass="ApproverCommonWidth">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="DelegationComments" FilterControlAltText="DelegationComments" SortExpression="DelegationComments" HeaderText="Delegation Comments" UniqueName="DelegationComments" HeaderStyle-Width="22%"  ItemStyle-CssClass="ApproverCommonWidth">
                    </telerik:GridBoundColumn>
                </Columns>
            </telerik:GridTableView>
        </DetailTables>
        <Columns>
            <telerik:GridBoundColumn DataField="ApproverName" FilterControlAltText="ApproverName" SortExpression="ApproverName" HeaderText="Approver Name" UniqueName="ApproverName">
            </telerik:GridBoundColumn>
            <telerik:GridTemplateColumn HeaderText="Approver Role" SortExpression="ApproverType" UniqueName="ApproverType">
                <ItemTemplate>
                    <asp:Label ID="lblApprover" runat="server" Text='<%#Eval("ApproverType") != null && Eval("ApproverType").ToString() == "Group - NPAC Approver"?"Group - NPAC/NMAC/PROTIS/ENIS Approver":Eval("ApproverType") %>' />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn DataField="Status" FilterControlAltText="Status" SortExpression="Status" HeaderText="Status" UniqueName="Status">
            </telerik:GridBoundColumn>
        </Columns>
    </MasterTableView>
</telerik:RadGrid>
