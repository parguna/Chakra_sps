<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Account.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="uc1" TagName="EndpointUC" %>
<%@ Register Src="~/UsrControl/Common/AccountDetailsUC.ascx" TagPrefix="uc1" TagName="AccountDetailsUC" %>

<style>
    #ctl00_cplMain_ListUC_Account_ctl00_ctl02_ctl03_FilterCheckBox_IsLocked, #ctl00_cplMain_ListUC_Account_ctl00_ctl02_ctl03_FilterCheckBox_IsDeletedBoolean {
        position: inherit;
        opacity: 100;
        transform: scale(1.2);
    }

    input#ctl00_cplMain_ListUC_Account_ctl00_ctl02_ctl03_FilterCheckBox_IsLocked + label.lbl, input#ctl00_cplMain_ListUC_Account_ctl00_ctl02_ctl03_FilterCheckBox_IsDeletedBoolean + label.lbl {
        display: none;
    }
</style>
<div class="content-wrapper">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                    <uc1:EndpointUC runat="server" ID="EndpointUC" OnApplicationChanged="EndpointUC_ApplicationChanged" OnEndpointChanged="EndpointUC_EndpointChanged" />

                </div>
                <div class="col-xs-6">
                    <div class="form-group">
                        <label class="col-xs-4 control-label">Mapping Types</label>
                        <div class="col-xs-7">
                            <asp:DropDownList ID="MappingTypesField" runat="server" CssClass="form-control selectpicker" data-width="100%" AutoPostBack="True"
                                OnSelectedIndexChanged="TriggerBind">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4 control-label">AccountType</label>
                        <div class="col-xs-7">
                            <telerik:RadComboBox ID="cmbAccountType" AppendDataBoundItems="true" runat="server" OnSelectedIndexChanged="AccountType_SelectedIndexChanged"
                                AutoPostBack="true" OnPreRender="AccountType_PreRender" CssClass="selectpicker AccountsPageComoboWidth radbox100Width" DropDownAutoWidth="Enabled" data-width="100%">
                                <Items>
                                    <telerik:RadComboBoxItem Text="All" Value="All" />
                                </Items>
                            </telerik:RadComboBox>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="from">
    <div class="panel-group" id="accordion">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a data-toggle="collapse" href="#mappingContainer" style="color: #666; font-size: 15px;">Mapping Criteria</a> </h4>
            </div>
            <div class="collapse" id="mappingContainer" style="padding: 1%;">
                <div class="form ">
                    <div class="form-horizontal left">
                        <div class="row-fluid">
                            <div class="col-xs-12">
                                <div class="col-xs-4 ">
                                    <div class="form-group">
                                        <label class="col-xs-4">Mapping Type:</label>
                                        <div class="col-xs-7">
                                            <asp:Label ID="lblMapType" runat="server" class="description" Text=""></asp:Label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="content-wrapper">
                    <telerik:RadGrid ID="MappingGrid" ClientSettings-EnablePostBackOnRowClick="true" FilterMenu-CssClass="RadMenu_IACMFilterSkin" AllowFilteringByColumn="False" OnNeedDataSource="MappingGrid_NeedDataSource"
                        runat="server" AllowPaging="true" AllowSorting="true" CellSpacing="0">
                        <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" AlwaysVisible="false" />
                        <ClientSettings>
                            <Scrolling UseStaticHeaders="true" />
                            <Selecting AllowRowSelect="true" />
                        </ClientSettings>
                        <GroupingSettings CaseSensitive="false" />
                        <MasterTableView Width="100%" AutoGenerateColumns="False" CommandItemDisplay="None" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available" EnableHeaderContextMenu="false" DataKeyNames="Id">
                            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                            <Columns>
                                <telerik:GridBoundColumn DataField="DiColumn" HeaderText="IDMS Field" UniqueName="DiColumn" HeaderStyle-Width="23.25%">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="TgColumn" HeaderText="Application Field" UniqueName="TgColumn" HeaderStyle-Width="23.25%">
                                </telerik:GridBoundColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="content-wrapper">
    <div class="from">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-12">
                    <div class="form-group">
                        <div class="table-header mTop15">
                            <h4>Accounts</h4>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<cc1:IACMRadGridControl ID="Account" runat="server" PageSize="50" AllowCustomPaging="true" OnItemDataBound="Account_ItemDataBound" OnNeedDataSource="Account_NeedDataSource" OnSelectedIndexChanged="Account_SelectedIndexChanged" OnItemCommand="Account_ItemCommand" AssignNullBeforeRebind="true">
    <MasterTableView Width="100%" AutoGenerateColumns="False" ShowHeadersWhenNoRecords="true" CommandItemDisplay="Top" DataKeyNames="ReconciliationId" NoMasterRecordsText="No Records Available" TableLayout="Fixed"  PageSize="50">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn DataField="AccountName" FilterControlAltText="AccountName" HeaderText="Account Name" SortExpression="AccountName" UniqueName="AccountName" HeaderStyle-Width="27%" FilterControlWidth="83.5%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="EndPointName" FilterControlAltText="EndPointName" HeaderText="EndPointName" SortExpression="EndPointName" UniqueName="EndPointName" HeaderStyle-Width="27%" FilterControlWidth="83.5%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="DisplayName" FilterControlAltText="DisplayName" HeaderText="Display Name" SortExpression="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="27%" FilterControlWidth="83.5%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="UserDisplayName" FilterControlAltText="UserDisplayName" HeaderText="Owner Display Name" SortExpression="UserDisplayName" UniqueName="UserDisplayName" HeaderStyle-Width="27%" FilterControlWidth="83.5%">
            </telerik:GridBoundColumn>

            <telerik:GridBoundColumn DataField="OffboardingAction" FilterControlAltText="OffboardingAction" HeaderText="Offboarding Action" SortExpression="OffboardingAction" UniqueName="OffboardingAction" HeaderStyle-Width="27%" FilterControlWidth="83.5%">
            </telerik:GridBoundColumn>

            <telerik:GridBoundColumn DataField="IsLocked" FilterControlAltText="IsLocked" UniqueName="IsLocked" HeaderText="Locked" SortExpression="IsLocked" HeaderStyle-Width="10%" FilterControlWidth="60%">
                <%--<ItemTemplate>
                    <asp:Label ID="lblItemName" runat="server" Text='<%#Convert.ToBoolean(Eval("IsLocked")) ? "Yes" : "No" %>' />
                </ItemTemplate>--%>
            </telerik:GridBoundColumn>
            <%--<telerik:GridBoundColumn DataField="IsDisabled" FilterControlAltText="IsDisabled" UniqueName="IsDisabled" HeaderText="Disabled" HeaderStyle-Width="10%" FilterControlWidth="60%">
                <ItemTemplate>
                            <asp:Label ID="lblItemName" runat="server" Text='<%#Convert.ToBoolean(Eval("IsDisabled")) ? "Disabled" : "Active" %>'/>
                    </ItemTemplate>
            </telerik:GridBoundColumn>--%>
            <telerik:GridBoundColumn DataField="IsDeletedBoolean" FilterControlAltText="IsDeletedBoolean" UniqueName="IsDeletedBoolean" HeaderText="Deleted" SortExpression="IsDeletedBoolean" HeaderStyle-Width="10%" FilterControlWidth="60%">
            </telerik:GridBoundColumn>

            <telerik:GridBoundColumn DataField="AccountType" FilterControlAltText="AccountType" AllowFiltering="false" HeaderText="Account Type" SortExpression="AccountType" UniqueName="AccountType" HeaderStyle-Width="25%" FilterControlWidth="83.5%">
            </telerik:GridBoundColumn>
            <telerik:GridTemplateColumn UniqueName="TemplateInfoColumn" HeaderText="Detail / Info" HeaderStyle-Width="7%" AllowFiltering="false">
                <ItemTemplate>
                    <asp:ImageButton ID="ShowIcon" runat="server" CommandName="ShowInfo" />
                </ItemTemplate>
                <ItemStyle CssClass="action-info" HorizontalAlign="Center" />
            </telerik:GridTemplateColumn>
        </Columns>
    </MasterTableView>
</cc1:IACMRadGridControl>
<telerik:RadWindow runat="server" ID="UserAccountDetail" Modal="true" Width="680px" Height="100%" Title="Account Detail"
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
                    <asp:Button runat="server" ID="btnCancel" class="btn btn-primary bordered-button" Style="width: 87px" Text="OK" OnClick="btnCancel_Click" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>


