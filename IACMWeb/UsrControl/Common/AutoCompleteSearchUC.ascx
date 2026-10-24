<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AutoCompleteSearchUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.AutoCompleteSearchUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<script>
    function OnClientEntryAddingHandler(sender, eventArgs) {
        if (sender.get_entries().get_count() > 0) {

            eventArgs.set_cancel(true);
        }
    }

    function OnClientRequesting(sender, eventArgs) {
        var filter = document.getElementById("<%= Filter.ClientID %>");        
        var context = eventArgs.get_context();
        context["Filter"] = filter.value;
    }
</script>
<asp:HiddenField runat="server" ID="Filter" />
<telerik:RadAutoCompleteBox ID="AutoCompleteBox" OnClientEntryAdding="OnClientEntryAddingHandler" ToolTip="Enter * to view full list" runat="server" CssClass="form-control"
    InputType="Token" OnEntryAdded="AutoCompleteBox_EntryAdded" OnClientRequesting="OnClientRequesting">
    <TextSettings SelectionMode="Single" />
    <WebServiceSettings Method="" Path="../../Services/AutoCompleteDataService.svc" />
</telerik:RadAutoCompleteBox>



