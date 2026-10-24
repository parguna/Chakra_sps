<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RequestedEntitlementUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.RequestedEntitlementUC" %>
<%@ Register Src="~/UsrControl/Common/AdditionalEntitlementDetailUC.ascx" TagPrefix="uc1" TagName="AdditionalEntitlementDetailUC" %>

<asp:Repeater ID="RequestRepeater" runat="server" OnItemDataBound="RequestRepeater_ItemDataBound">
    <ItemTemplate>
        <span style="display: block"><span class="description"><%# Container.ItemIndex + 1 %> .</span>
            <asp:Label ID="lblItemName" class="description" runat="server" Text='<%# (string.IsNullOrEmpty(Convert.ToString(Eval("ItemDisplayName"))) ? Eval("ItemName")  : Eval("ItemDisplayName")) + " (" +( (Eval("AccessRequestItemType").Equals(DI.Chakra.Infrastructure.AccessRequestItemType.Entitlement)) ? (Eval("AccessItemType").Equals(DI.Chakra.Infrastructure.Enums.AccessItemType.TechnicalRole) ? Eval("EntitlementType") : "Catalog") : Eval("AccountType"))+ ")" %> ' />
            <br />
            <uc1:AdditionalEntitlementDetailUC runat="server" ID="AdditionalEntitlementDetailUC" />
        </span>
    </ItemTemplate>
</asp:Repeater>
