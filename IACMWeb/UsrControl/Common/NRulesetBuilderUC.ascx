<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NRulesetBuilderUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.NRulesetBuilderUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<script>
    function OnClientEntryAddingHandler(sender, eventArgs) {
        if (sender.get_entries().get_count() > 0) {

            eventArgs.set_cancel(true);
        }

    }
</script>
<div class="row-fluid">
    <div class="col-xs-12">
        <div class="form-group">
            <div class="col-xs-2">
                <asp:Label ID="lblRulesetName" runat="server" Text="Ruleset Name" CssClass="col-xs-2 control-label" AssociatedControlID="txtRulesetName"></asp:Label>
            </div>
            <div class="col-xs-2">
                <asp:TextBox ID="txtRulesetName" runat="server" required="required" MaxLength="30" type="text" placeholder="Ruleset Name" CssClass="form-control"></asp:TextBox>
            </div>            
        </div>

        <div class="form-group">
            <div class="col-xs-5">
                <telerik:RadAutoCompleteBox ID="RulesetTypeAutoCompleteBox" OnClientEntryAdding="OnClientEntryAddingHandler" ToolTip="Enter * to view full list" runat="server" CssClass="form-control"
                    InputType="Token" OnEntryAdded="RulesetTypeAutoCompleteBox_EntryAdded">
                    <TextSettings SelectionMode="Single" />
                    <WebServiceSettings Method="GetNRulesetTypes" Path="../../Services/AutoCompleteDataService.svc" />
                </telerik:RadAutoCompleteBox>
            </div>
            <div class="col-xs-1">
                <asp:dropdownlist id="OperatorField" runat="server" cssclass="form-control selectpicker" data-width="100%" >
                </asp:dropdownlist>
            </div>
            <div class="col-xs-5">
                <telerik:RadAutoCompleteBox ID="RulesetValueAutoCompleteBox" OnClientEntryAdding="OnClientEntryAddingHandler" ToolTip="Enter * to view full list" runat="server" CssClass="form-control"
                    InputType="Token" OnEntryAdded="RulesetValueAutoCompleteBox_EntryAdded">
                    <TextSettings SelectionMode="Single" />
                    <WebServiceSettings Method="" Path="../../Services/AutoCompleteDataService.svc" />
                </telerik:RadAutoCompleteBox>
            </div>
            <div class="col-xs-1">
                <asp:Button ID="btnAdd" ValidationGroup="Add" runat="server" Text="Add"
                                OnClick="btnAdd_Click" CssClass="btn btn-default btn-sm" />
            </div>
        </div>

        <div class="form-group">
            <div class="col-xs-12">
                <telerik:RadGrid ID="RulesetGrid" runat="server" AllowPaging="false" AllowSorting="false"
                    CellSpacing="0" GridLines="None" OnItemCommand="FormGrid_ItemCommand" Enabled="true" Visible="true">
                    <GroupingSettings CaseSensitive="false" />
                    <MasterTableView AutoGenerateColumns="False" ShowHeadersWhenNoRecords="false" NoMasterRecordsText="No Records Available">
                        <Columns>
                            <telerik:GridBoundColumn DataField="TypeId" FilterControlAltText="TypeId" SortExpression="TypeId" HeaderText="Ruleset Type" HeaderStyle-Width="300px">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Operator" FilterControlAltText="Operator" SortExpression="Operator" HeaderText="Operator" HeaderStyle-Width="300px">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Value" FilterControlAltText="Value" SortExpression="Value" HeaderText="Value" HeaderStyle-Width="300px">
                            </telerik:GridBoundColumn>
                            <telerik:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" Text="Delete" HeaderText="Delete" ConfirmText="Are you sure you want to delete the Form?" ConfirmDialogType="RadWindow"
                                UniqueName="DeleteColumn" Exportable="false" HeaderStyle-Width="10%">
                                <ItemStyle HorizontalAlign="Center" CssClass="action-delete" />
                            </telerik:GridButtonColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
        </div>

    </div>


</div>
