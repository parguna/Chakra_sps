<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RequestItemUC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.RequestItemUC" %>
<%@ Register Src="~/NACR/Create/App/AddCartUC.ascx" TagPrefix="uc1" TagName="AddCartUC" %>
<%@ Register Src="~/NACR/Create/App/Special/AddCartSpeicalAcUC.ascx" TagPrefix="uc1" TagName="AddCartSpeicalAcUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/NACR/Create/App/Special/StandingOrder.ascx" TagPrefix="uc1" TagName="StandingOrder" %>



<tr class="rgRow">
    <asp:HiddenField ID="lblEpId" runat="server" Value='<%# Eval("EndpointSystemId") %>' />
    <asp:HiddenField ID="lblEpTypeId" runat="server" Value='<%# Eval("EndpointSystemTypeId") %>' />
    <asp:HiddenField ID="lblKeyId" runat="server" Value='<%# Eval("KeyId") %>' />
    <asp:HiddenField ID="lblEntitlementType" runat="server" Value='<%# Eval("EntitlementType") %>' />
    <td align="left" class="endpoint-system detail">
        <h5>
            <asp:Label ID="lblItemName" runat="server" Text='<%# Eval("ItemDisplayName") %>' />
        </h5>
        <h5>
            Endpoint : 
            <asp:Label class="description" ID="lblDesc" runat="server" Text='<%# Eval("EndpointName") %>' />
            | Description :
            <asp:Label class="description" ID="Label2" runat="server" Text='<%# Eval("ItemDetailDescription") %>' />
        </h5>
    </td>
    <td>
        <asp:Label ID="lblAccessRequestItemType" runat="server" Text='<%# Eval("EntitlementType") %>' /></td>
    <td>
        <asp:Label ID="lblAccessItemType" runat="server" Text='<%# Eval("AccessItemType") %>' /></td>
    <td align="center" style="position:static !important;">
        <div class="whats-this bottom showTooltip action-info">
            <a class="di_tooltip" href="#" data-toggle="tooltip"></a>
            <span class="tooltip right info" role="tooltip" style="width: 200px; display: none; text-align: left">
                <h5>Detail</h5>
                <p>
                    Name :
                    <asp:Label ID="lblItemDetail" runat="server" Text='<%# Eval("ItemName") %>' /> <br />
                    Description :
                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("ItemDescription") %>' />
                </p>
            </span>
        </div>
    </td>

    <td class="visible-overflow">
        <div class="add-item"><div class="add-item-text">
            <uc1:AddCartUC runat="server" ID="AddCartUC" OnItemClicked="AddCartUC_ItemClicked" /></div>
            <div><uc1:AddCartSpeicalAcUC runat="server" ID="AddCartSpeicalAcUC" OnItemClicked="AddCartSpecialAcUC_ItemClicked" Visible="false" /></div>
        </div>

    </td>
</tr>



