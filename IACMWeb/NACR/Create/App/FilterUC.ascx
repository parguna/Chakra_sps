<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FilterUC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.FilterUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<script type="text/javascript">
    function ClientNodeChecked(sender, eventArgs) {
        var tree = sender;
        var node = eventArgs.get_node();
        var parentNode = node.get_parent();

        if (node.get_level() == 0) {
            node.set_checked(false);
            return;
        }

        if (node.get_level() == 2) {
            if (parentNode.get_checked() == false) {
                node.set_checked(false);
            }
        }

        if (node.get_level() == 1) {
            for (var i = 0; i < tree.get_nodes().get_count() ; i++) {
                var tnode = tree.get_nodes().getNode(i);
                tnode.set_checked(false);
            }
            node.set_checked(true);
        };
    }
</script>

<%--<div class="checkbox">
    <telerik:RadTreeView ID="treeApp"  DataFieldID="Id" DataValueField="Id" DataTextField="Name" DataFieldParentID="P_Id" CheckBoxes="true" CheckChildNodes="true" runat="server">
        
    </telerik:RadTreeView>
</div>--%>
<div class="checkbox">
    <telerik:RadTreeView ID="treeEnt" MultipleSelect="false"
         OnClientNodeChecked="ClientNodeChecked" DataFieldID="uniqId"
         DataValueField="Id" DataTextField="Name" DataFieldParentID="P_Id" CheckChildNodes="true" CheckBoxes="true" runat="server">
    </telerik:RadTreeView>

</div>
