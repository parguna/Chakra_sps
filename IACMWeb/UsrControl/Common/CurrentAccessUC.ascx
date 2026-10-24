<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CurrentAccessUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.CurrentAccessUC" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/UserSODConflicts.ascx" TagPrefix="uc1" TagName="UserSODConflicts" %>
<%@ Register Src="~/UsrControl/Common/UserRiskUC.ascx" TagPrefix="uc1" TagName="UserRiskUC" %>
<%@ Register Src="~/UsrControl/Common/AccountDetailsUC.ascx" TagPrefix="uc1" TagName="AccountDetailsUC" %>
<%--<style type="text/css">
    .RadGrid_IACMSkin .rgDataDiv {
        max-height: 370px;
    }
</style>--%>
<div style="float: right; text-align: right">
    <div class="form-group">
        <uc1:UserRiskUC runat="server" ID="UserRiskUC" Visible="true" />
        <%--<asp:LinkButton ID="lnkShowRisk" runat="server" class="description red" Text="Show Risk Info" OnClick="lnkShowRisk_Click" ></asp:LinkButton>--%>
    </div>
</div>

<telerik:RadGrid runat="server" ID="AccountDetail" AutoGenerateColumns="False" OnDetailTableDataBind="AccountDetail_DetailTableDataBind" GroupPanelPosition="Top"
    OnNeedDataSource="AccountDetail_NeedDataSource" AllowPaging="true" AllowSorting="true" FilterMenu-CssClass="RadMenu_IACMFilterSkin" Width="100%" PageSize="50"
    OnItemCommand="AccountDetail_ItemCommand" OnItemCreated="AccountDetail_ItemCreated" OnItemDataBound="AccountDetail_ItemDataBound" OnPreRender="AccountDetail_PreRender" TableLayout="Auto" >
    <ClientSettings AllowExpandCollapse="true">
        <Scrolling UseStaticHeaders="true" AllowScroll="true" />
        <Resizing ClipCellContentOnResize="false" />
    </ClientSettings>
    <GroupingSettings CaseSensitive="false" />
    <PagerStyle PageSizes="10,20,50" PageSizeControlType="RadDropDownList" AlwaysVisible="true" Width="100%" />
    <MasterTableView AutoGenerateColumns="False" Width="100%" ShowHeadersWhenNoRecords="false" CommandItemDisplay="None"  PageSize="50"
        EnableHeaderContextMenu="false" DataKeyNames="EntitlementId,EndPointSystemType,Category,EndpointSystemId" NoMasterRecordsText="No Records Available"
        HierarchyDefaultExpanded="false" HierarchyLoadMode="ServerOnDemand" TableLayout="Auto" >
        <PagerStyle PageSizes="10,20,50" PageSizeControlType="RadDropDownList" AlwaysVisible="true" Width="50%" />
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <SelfHierarchySettings ParentKeyName="ParentEntitlementId" KeyName="EntitlementId" />
        <SortExpressions>
            <telerik:GridSortExpression FieldName="EndpointSystemName" SortOrder="Ascending" />
        </SortExpressions>
        <Columns>
            <telerik:GridBoundColumn SortExpression="Name" UniqueName="Name" HeaderText="Account Name" HeaderButtonType="TextButton" DataField="UserName" HeaderStyle-Width="65%" ItemStyle-Width="15%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="Description" HeaderText="Display Name" HeaderButtonType="TextButton" DataField="Description" HeaderStyle-Width="65%" ItemStyle-Width="15%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn UniqueName="EndpointSystemName" SortExpression="EndpointSystemName" HeaderText="Endpoint System" HeaderButtonType="TextButton" DataField="EndpointSystemName" HeaderStyle-Width="90%" ItemStyle-Width="25%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn UniqueName="Category" SortExpression="Category" HeaderText="Category" HeaderButtonType="TextButton" DataField="Category" HeaderStyle-Width="30%" ItemStyle-Width="10%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn UniqueName="Functions" SortExpression="Functions" HeaderText="Functions" HeaderButtonType="TextButton" DataField="Functions" Visible="false" Display="false" HeaderStyle-Width="65%" ItemStyle-Width="15%">
            </telerik:GridBoundColumn>
            <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" Visible="false" Display="false" DataField="ReviewDate" HeaderText="Last Review Date" SortExpression="ReviewDate" UniqueName="ReviewDate" HeaderStyle-Width="65%" ItemStyle-Width="15%">
            </telerik:GridDateTimeColumn>
            <telerik:GridBoundColumn UniqueName="AccountType" SortExpression="AccountType" HeaderText="Account Type" HeaderButtonType="TextButton" DataField="AccountTypeString" HeaderStyle-Width="30%" ItemStyle-Width="10%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="IsSODConflict" Visible="false" HeaderText="IsSODConflict" HeaderButtonType="TextButton" DataField="IsSODConflict" Display="false" HeaderStyle-Width="65%" ItemStyle-Width="15%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="IsSensitiveAccess" Visible="false" HeaderText="IsSensitiveAccess" HeaderButtonType="TextButton" DataField="IsSensitiveAccess" Display="false" HeaderStyle-Width="65%" ItemStyle-Width="15%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn UniqueName="ValidTo" DataType="System.DateTime" DataFormatString="{0:dd-MM-yyyy}" SortExpression="ValidTo" HeaderText="Valid To" HeaderButtonType="TextButton" DataField="ValidTo" HeaderStyle-Width="65%" ItemStyle-Width="10%">
            </telerik:GridBoundColumn>
            <telerik:GridTemplateColumn UniqueName="TemplateEditColumn" HeaderText="Account Details" Visible="true" Display="true" HeaderStyle-Width="30%" ItemStyle-Width="5%">
                <ItemTemplate>
                    <asp:ImageButton ID="ShowIcon" runat="server" CommandName="ShowInfo"></asp:ImageButton>
                </ItemTemplate>
                <ItemStyle CssClass="action-info" HorizontalAlign="Center" />
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn UniqueName="TemplateRiskColumn" HeaderStyle-Width="65%" ItemStyle-Width="5%">
                <ItemTemplate>
                    <asp:ImageButton ID="ShowWarningIcon" runat="server" CommandName="ShowWarningInfo"></asp:ImageButton>
                </ItemTemplate>
                <ItemStyle CssClass="warning-info" HorizontalAlign="Center" />
            </telerik:GridTemplateColumn>
        </Columns>
    </MasterTableView>
</telerik:RadGrid>
<telerik:RadWindow runat="server" ID="UserAccountDetail" Modal="true" Width="700px" Height="100%" Title="Account Detail"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title">Account Detail</h3>
                </div>
                <div class="modal-body">
                    <div class="row-fluid">
                        <uc1:AccountDetailsUC runat="server" ID="AccountDetailsUC" />
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnCancel" class="btn btn-primary bordered-button" Text="OK" OnClick="btnCancel_Click" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>
<telerik:RadWindow runat="server" ID="SensitiveRoleDetails" Modal="true" Width="700px" Height="100%" Title="Account Detail"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title">Risk Information</h3>
                </div>
                <div class="modal-body">
                    <div class="row-fluid">
                        <uc1:UserSODConflicts runat="server" ID="UserSODConflict" />
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnSensitiveRoleCancel" class="btn btn-primary bordered-button" Text="OK" OnClick="btnSensitiveRoleCancel_Click" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>


<%--<div class="content-wrapper">
    <br />
    <p> User Exclusion:</p>
    <telerik:RadGrid ID="ExclusionGrid" runat="server" OnNeedDataSource="ExclusionGrid_NeedDataSource" AllowPaging="true">
        <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" AlwaysVisible="true" />
        <ClientSettings>
            <Scrolling UseStaticHeaders="true" />
            <Selecting AllowRowSelect="true" />
        </ClientSettings>
        <MasterTableView NoMasterRecordsText="No Data Found" DataKeyNames="Id" AutoGenerateColumns="false"  ShowHeadersWhenNoRecords="false">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="EventType" HeaderText="Event Type" HeaderStyle-Width="15%">
                </telerik:GridBoundColumn>
                <telerik:GridDateTimeColumn DataField="EffectiveDate" HeaderText="Effective Till Date" HeaderStyle-Width="15%">
                </telerik:GridDateTimeColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</div>--%>
