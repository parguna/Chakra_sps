<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NOrgDetail.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.NOrgDetail" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<script type="text/javascript">
    function collapseAll() {
        var panelBar = $find("<%= rdPnlOrgDetail.ClientID %>");
        if (panelBar == null) return;
        for (var i = 0; i < panelBar.get_allItems().length; i++) {
            panelBar.get_allItems()[i].collapse();
        }
    }
</script>

<telerik:RadPanelBar ID="rdPnlOrgDetail" RenderMode="Lightweight" OnDataBound="rdPnlOrgDetail_DataBound" runat="server" OnPreRender="rdPnlOrgDetail_PreRender" AllowCollapseAllItems="true" ExpandMode="SingleExpandedItem" >
</telerik:RadPanelBar>