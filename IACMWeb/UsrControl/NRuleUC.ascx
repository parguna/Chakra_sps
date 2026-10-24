<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NRuleUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.NRuleUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="form-group required">
    <asp:Label ID="RuleUCLabel" runat="server" CssClass="col-xs-4 control-label" AssociatedControlID="RuleAutoCompleteBox">Rule</asp:Label>
    <div runat="server" id="setClass" class="col-xs-6">
        <telerik:RadAutoCompleteBox ID="RuleAutoCompleteBox" OnClientEntryAdding="OnClientEntryAddingHandler" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control"
            InputType="Token" OnEntryAdded="RuleAutoCompleteBox_EntryAdded" OnEntryRemoved="RuleAutoCompleteBox_EntryRemoved">
            <TextSettings SelectionMode="Single" />
            <WebServiceSettings Method="GetRules" Path="../../Services/AutoCompleteDataService.svc" />
        </telerik:RadAutoCompleteBox>
    </div>
    <div class="col-xs-2">
        <asp:ImageButton ID="InfoButton" ImageUrl="~/Images/action-buttons/detail.png" Enabled="true" AlternateText="Click to see the details" runat="server"
            OnClick="InfoButton_Click"></asp:ImageButton>
    </div>
</div>

<telerik:RadWindow ID="RulesetRadwindow" runat="server" Modal="true" Width="400px" Height="490px" Title="Rule"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false" VisibleOnPageLoad="false">
    <ContentTemplate>
        <asp:UpdatePanel runat="server" UpdateMode="Always">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title">Rule Info</h3>
                </div>
                <div class="modal-body">

                    <telerik:RadGrid runat="server" ID="RuleGrid" AutoGenerateColumns="False" GroupPanelPosition="Top"
                        OnNeedDataSource="RuleGrid_NeedDataSource" AllowPaging="true" AllowSorting="true" FilterMenu-CssClass="RadMenu_IACMFilterSkin">
                        <GroupingSettings CaseSensitive="false" />
                        <MasterTableView AutoGenerateColumns="False" ShowHeadersWhenNoRecords="false" CommandItemDisplay="None" EnableHeaderContextMenu="false" NoMasterRecordsText="No Records Available">
                            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                            <Columns>
                                <telerik:GridTemplateColumn HeaderText="Rule Type">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRulesetType" runat="server" Text='<%# Eval("Key") %>' />

                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblName" runat="server" Text='<%# Eval("Value") %>' />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="OKButton" CssClass="btn btn-primary bordered-button" Text="OK" OnClick="OKButton_Click" UseSubmitBehavior="false" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>



