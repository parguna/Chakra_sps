<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserSODConflicts.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.UserSODConflicts" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadGrid runat="server" ID="SODConflict"
    AllowFilteringByColumn="False" AllowPaging="false" AllowSorting="True" CellSpacing="0"
    FilterMenu-CssClass="RadMenu_IACMFilterSkin" ClientSettings-EnablePostBackOnRowClick="true"
    OnNeedDataSource="SODConflict_NeedDataSource" OnDetailTableDataBind="SODConflict_DetailTableDataBind">
    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" AlwaysVisible="false" />
    <ClientSettings>
        <Scrolling UseStaticHeaders="true" />
        <Selecting AllowRowSelect="true" />
    </ClientSettings>
    <GroupingSettings CaseSensitive="false" />
    <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" ShowHeadersWhenNoRecords="true" EnableHeaderContextMenu="false" NoMasterRecordsText="No Records Available" DataKeyNames="Risk">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn SortExpression="Risk" HeaderText="Risk" HeaderButtonType="TextButton"
                DataField="Risk">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="RiskDescription" HeaderText="Description" HeaderButtonType="TextButton"
                DataField="RiskDescription">
            </telerik:GridBoundColumn>

        </Columns>
        <DetailTables>
            <telerik:GridTableView Name="Functions" ShowHeadersWhenNoRecords="false" NoDetailRecordsText="No Records found" AutoGenerateColumns="false" DataKeyNames="Function1Name,Function2Name">
                <Columns>
                    <telerik:GridBoundColumn SortExpression="Function1Description" HeaderText="Function 1" HeaderButtonType="TextButton"
                        DataField="Function1Description">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="Function2Description" HeaderText="Function 2" HeaderButtonType="TextButton"
                        DataField="Function2Description">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="CompliancePolicyType" HeaderText="Type" HeaderButtonType="TextButton"
                        DataField="CompliancePolicyType">
                    </telerik:GridBoundColumn>
                </Columns>
                <DetailTables>
                    <telerik:GridTableView NoDetailRecordsText="" Name="FunctionDetails" AutoGenerateColumns="false" ShowHeadersWhenNoRecords="false">
                        <Columns>
                            <telerik:GridBoundColumn SortExpression="ConflictEntitlement1" HeaderText="Entitlement 1" HeaderButtonType="TextButton"
                                DataField="ConflictEntitlement1">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn SortExpression="ConflictEntitlement1Endpoint" HeaderText="Entitlement1 Application" HeaderButtonType="TextButton"
                                DataField="ConflictEntitlement1Endpoint">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn SortExpression="ConflictEntitlement2" HeaderText="Entitlement 2" HeaderButtonType="TextButton"
                                DataField="ConflictEntitlement2">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn SortExpression="ConflictEntitlement2Endpoint" HeaderText="Entitlement2 Application" HeaderButtonType="TextButton"
                                DataField="ConflictEntitlement2Endpoint">
                            </telerik:GridBoundColumn>
                        </Columns>
                    </telerik:GridTableView>
                </DetailTables>

            </telerik:GridTableView>
        </DetailTables>
    </MasterTableView>
</telerik:RadGrid>
