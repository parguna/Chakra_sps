<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SAPAccessListUC.ascx.cs" Inherits="DI.Chakra.Web.Account.UC.SAP.SAPAccessListUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<telerik:RadGrid runat="server" ID="AccessList" OnNeedDataSource="AccessList_NeedDataSource" OnDetailTableDataBind="AccessList_DetailTableDataBind" AutoGenerateColumns="False" GroupPanelPosition="Top">
    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" AlwaysVisible="true" />
    <ClientSettings>
        <Scrolling UseStaticHeaders="true" />
    </ClientSettings>
    <MasterTableView  Width="100%"  AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="false" AllowPaging="true" NoDetailRecordsText="No Record Found"  DataKeyNames="EndPointSystemId,Name">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
             <telerik:GridBoundColumn SortExpression="Name" HeaderText="Name" HeaderButtonType="TextButton" DataField="Name">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="Description" HeaderText="Description" HeaderButtonType="TextButton" DataField="Description">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="Category" HeaderText="Category" HeaderButtonType="TextButton" DataField="Category">
            </telerik:GridBoundColumn>
        </Columns>
        <DetailTables>
            <telerik:GridTableView Name="TCodes" DataKeyNames="Id,TCODE_Desc">
                <Columns>
                    <telerik:GridBoundColumn SortExpression="TCODE_Code" HeaderText="T-Code" HeaderButtonType="TextButton" DataField="TCODE_Code">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="TCODE_Desc" HeaderText="Description" HeaderButtonType="TextButton" DataField="TCODE_Desc">
                    </telerik:GridBoundColumn>
                </Columns>
                <DetailTables>
                    <telerik:GridTableView Name="Authorization">
                        <Columns>
                            <telerik:GridBoundColumn SortExpression="AuthorizationCode" HeaderText="Authorization" HeaderButtonType="TextButton" DataField="AuthCode">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn SortExpression="ObjectCode" HeaderText="Object Code" HeaderButtonType="TextButton" DataField="ObjectCode">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn SortExpression="FieldCode" HeaderText="Field Code" HeaderButtonType="TextButton" DataField="FieldCode">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn SortExpression="FromValue" HeaderText="From Value" HeaderButtonType="TextButton" DataField="FromValue">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn SortExpression="ToValue" HeaderText="To Value" HeaderButtonType="TextButton" DataField="ToValue">
                            </telerik:GridBoundColumn>
                        </Columns>
                    </telerik:GridTableView>
                </DetailTables>
            </telerik:GridTableView>
        </DetailTables>
    </MasterTableView>
</telerik:RadGrid>