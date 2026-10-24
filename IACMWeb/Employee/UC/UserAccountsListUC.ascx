<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserAccountsListUC.ascx.cs" Inherits="DI.Chakra.Web.Employee.UC.UserAccountsListUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/AccountDetailsUC.ascx" TagPrefix="uc1" TagName="AccountDetailsUC" %>


<telerik:RadGrid runat="server" ID="AccountDetail" AutoGenerateColumns="False" OnDetailTableDataBind="AccountDetail_DetailTableDataBind" GroupPanelPosition="Top"
    OnNeedDataSource="AccountDetail_NeedDataSource" AllowPaging="True" FilterMenu-CssClass="RadMenu_IACMFilterSkin" OnItemCommand="AccountDetail_ItemCommand"
    OnPreRender="AccountDetail_PreRender">
    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" AlwaysVisible="true" />
    <ClientSettings>
        <Scrolling UseStaticHeaders="true" />
    </ClientSettings>
    <GroupingSettings CaseSensitive="false" />
    <MasterTableView AutoGenerateColumns="False" Width="100%"  ShowHeadersWhenNoRecords="false" CommandItemDisplay="None" EnableHeaderContextMenu="false" DataKeyNames="AcountReconcilationId,EndpointUserId,EndPointSystemType" NoMasterRecordsText="No Records Available">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn SortExpression="Name" HeaderText="Name" HeaderButtonType="TextButton" DataField="UserName" HeaderStyle-Width="35%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="DisplayName" HeaderText="Display Name" HeaderButtonType="TextButton" DataField="FullName" HeaderStyle-Width="20%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn SortExpression="LastLogon" HeaderText="Last Logon" HeaderButtonType="TextButton" DataField="LastLogonDate" HeaderStyle-Width="17%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn UniqueName="EndpointSystemName" SortExpression="EndpointSystemName" HeaderText="Endpoint System Name" HeaderButtonType="TextButton"
                DataField="EndpointSystemName" HeaderStyle-Width="20%">
            </telerik:GridBoundColumn>
            <telerik:GridTemplateColumn UniqueName="TemplateEditColumn" HeaderStyle-Width="7%">
                <ItemTemplate>
                    <asp:ImageButton ID="ShowIcon" runat="server" CommandName="ShowInfo"></asp:ImageButton>
                </ItemTemplate>
                <ItemStyle CssClass="action-info" HorizontalAlign="Center" />
            </telerik:GridTemplateColumn>
        </Columns>
        <DetailTables>
            <telerik:GridTableView DataKeyNames="EndpointUserId" Name="Entitlements" ShowHeadersWhenNoRecords="false" NoDetailRecordsText="No Records found">
                <Columns>
                    <telerik:GridBoundColumn SortExpression="Name" HeaderText="Name" HeaderButtonType="TextButton" DataField="Name" HeaderStyle-Width="34%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="Description" HeaderText="Description" HeaderButtonType="TextButton" DataField="Description" HeaderStyle-Width="36%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="Category" HeaderText="Category" HeaderButtonType="TextButton" DataField="Category" HeaderStyle-Width="27.4%">
                    </telerik:GridBoundColumn>
                </Columns>
            </telerik:GridTableView>
        </DetailTables>
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
                    <%--<uc1:AccountDetailUC runat="server" ID="AccountDetailUC" />--%>
                    <uc1:AccountDetailsUC runat="server" id="AccountDetailsUC" />
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnCancel" class="btn btn-primary bordered-button" Text="OK" OnClick="btnCancel_Click" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>
