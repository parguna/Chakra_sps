<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SAPObjectFieldSearch.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.SAPObjectFieldSearch" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<script type="text/javascript">
    var FunctionSelect = "";
    function SetItems(sender, eventArgs) {
        var FunctionID = sender._clientStateFieldID.substring(0, sender._clientStateFieldID.indexOf("_ClientState"));
        var Function = $find(FunctionID);
        var FunctionValue = $find(FunctionID + 'Fields');
        FunctionValue.get_entries().clear();
        FunctionSelect = eventArgs.get_text();
    }

    function Requesting(sender, eventArgs) {
        var FunctionVal = sender._clientStateFieldID.substring(0, sender._clientStateFieldID.indexOf("SapObjectFields_ClientState"))
        var Function = $find(FunctionVal + 'SapObject');
        FunctionSelect = Function._originalText.substring(0, Function._originalText.indexOf("; "));
        var context = eventArgs.get_context();
        context.Text = context.Text + ',' + FunctionSelect;
    }
    function OnClientEntryAddingHandler(sender, eventArgs) {
        if (sender.get_entries().get_count() > 0) {

            eventArgs.set_cancel(true);
        }

    }
</script>


<div class="form-group">
    <asp:Label ID="Function" AssociatedControlID="SapObject" runat="server" Text="Objects" required="required" CssClass="col-xs-4 control-label"></asp:Label>
    <div class="col-xs-7">
        <telerik:RadAutoCompleteBox ID="SapObject"   OnClientTextChanged="SetItems" OnClientEntryAdding="OnClientEntryAddingHandler"  EmptyMessage="Enter * to view full list" AutoPostBack="false" runat="server" CssClass="form-control" InputType="Token">
            <TextSettings SelectionMode="Single"  />
            <WebServiceSettings Method="GetSAPAuthorizationObjects"  Path="../../Services/AutoCompleteDataService.svc" />
        </telerik:RadAutoCompleteBox>
    </div>
</div>
<div class="form-group">
    <asp:Label ID="FunctionVal" AssociatedControlID="SapObjectFields" OnClientRequesting="Requesting" runat="server" Text="Fields" required="required" CssClass="col-xs-4 control-label"></asp:Label>
    <div class="col-xs-7">
      <telerik:RadAutoCompleteBox ID="SapObjectFields" runat="server"  OnClientRequesting="Requesting" OnClientEntryAdding="OnClientEntryAddingHandler" EmptyMessage="Enter * to view full list"  CssClass="form-control" InputType="Token">
          <TextSettings SelectionMode="Single" />
          <WebServiceSettings Method="GetSapFields" Path="../../Services/AutoCompleteDataService.svc"/>
      </telerik:RadAutoCompleteBox>
    </div>
</div>
