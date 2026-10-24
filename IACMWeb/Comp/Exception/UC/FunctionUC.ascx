<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FunctionUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.Exception.UC.FunctionUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<script type="text/javascript">
    var FunctionSelect = "";
    function SetItems(sender, eventArgs) {
        var FunctionID = sender._clientStateFieldID.substring(0, sender._clientStateFieldID.indexOf("_ClientState"));
        var Function = $find(FunctionID);
        var FunctionValue = $find(FunctionID + 'Value');
        FunctionValue.get_entries().clear();
        FunctionSelect = eventArgs.get_context().Text;
    }

    function Requesting(sender, eventArgs) {
        var FunctionVal = sender._clientStateFieldID.substring(0, sender._clientStateFieldID.indexOf("FunctionAutoValue_ClientState"))
        var Function = $find(FunctionVal + 'FunctionAuto');
        //if (FunctionSelect != "*") {
            FunctionSelect = Function._originalText;
        //}
        var context = eventArgs.get_context();
        context.Text = context.Text + ',' + FunctionSelect;        
    }
</script>

<div class="form-group">
    <asp:Label ID="Function" AssociatedControlID="FunctionAuto" runat="server" Text="Function 1" required="required" CssClass="col-xs-4 control-label"></asp:Label>
    <div class="col-xs-7">
        <telerik:RadAutoCompleteBox ID="FunctionAuto"  OnClientRequesting="SetItems"     EmptyMessage="Enter * to view full list" AutoPostBack="false" runat="server" CssClass="form-control" InputType="Token">
            <TextSettings SelectionMode="Single"  />
            <WebServiceSettings Method="GetFunctions"  Path="../../../Services/AutoCompleteDataService.svc" />
        </telerik:RadAutoCompleteBox>
    </div>
</div>
<div class="form-group">
    <asp:Label ID="FunctionVal" AssociatedControlID="FunctionAutoValue" runat="server" Text="Function Value 1"  required="required" CssClass="col-xs-4 control-label"></asp:Label>
    <div class="col-xs-7">
      <telerik:RadAutoCompleteBox ID="FunctionAutoValue" runat="server"  OnClientRequesting="Requesting" EmptyMessage="Enter * to view full list"  CssClass="form-control" InputType="Token">
          <TextSettings SelectionMode="Single" />
          <WebServiceSettings Method="GetFunctionItems" Path="../../../Services/AutoCompleteDataService.svc"/>
      </telerik:RadAutoCompleteBox>
    </div>
</div>
