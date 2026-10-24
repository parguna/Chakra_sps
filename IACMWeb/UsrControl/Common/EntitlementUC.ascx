<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EntitlementUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.EntitlementUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<script>
    function OnClientEntryAddingHandler(sender, eventArgs) {
        if (sender.get_entries().get_count() > 0) {
            eventArgs.set_cancel(true);
        }
    }
    function OnClientRequesting(sender, eventArgs) {
        var hiddenVal = $("#"+ '<%= HiddenValue.ClientID %>').val();
        var context = eventArgs.get_context();
        context["hidden"] = hiddenVal;
    }
</script>
<div class="col-xs-6">
    <div class="form-group required">
        <label class="col-xs-4 control-label">Entitlement Type</label>
    <div class="col-xs-6">
            <asp:dropdownlist id="EntitlementType" runat="server" OnSelectedIndexChanged="EntitlementType_SelectedIndexChanged"
                 cssclass="form-control selectpicker" data-width="100%" AutoPostBack="true">
            </asp:dropdownlist>
        </div>
    </div>
</div>
<asp:Panel runat="server" ID="clearLeft" CssClass="clearfix" Visible="false"></asp:Panel>
<div class="col-xs-6">
    <div class="form-group required">
        <label class="col-xs-4 control-label">Entitlement Name</label>
        <div class="col-xs-6">
            <telerik:RadAutoCompleteBox ID="EntitlementNameAutoCompleteBox" OnClientRequesting="OnClientRequesting" OnClientEntryAdding="OnClientEntryAddingHandler" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control" 
                InputType="Token" OnEntryAdded="EntitlementNameAutoCompleteBox_EntryAdded" OnEntryRemoved="EntitlementNameAutoCompleteBox_EntryRemoved">
                <TextSettings SelectionMode="Single" />
                <WebServiceSettings Method="GetEntitlementName" Path="../../Services/AutoCompleteDataService.svc" />
            </telerik:RadAutoCompleteBox>
        </div>
         <div class="col-xs-1">
        <asp:Button runat="server" ID="Add" CssClass="btn btn-default btn-sm" Text="Add" OnClick="Add_Click"/>
        </div>
    </div>
    
</div>
<asp:HiddenField ID="HiddenValue" runat="server" Value="" />

