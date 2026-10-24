<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CartInfo.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.CartInfo" %>
<%@ Register Src="~/NACR/Create/App/CartItems.ascx" TagPrefix="uc1" TagName="CartItems" %>
<asp:Panel runat="server" CssClass="inlineBlock pull-right">
<asp:UpdatePanel runat="server" ID="UpdateCart" UpdateMode="Conditional"><ContentTemplate>
<span class="cart inline pull-right">
    <asp:LinkButton runat="server" ID="lnkEditCart" OnClick="lnkEditCart_Click" Text="Edit Cart" CssClass="btn btn-primary btn-sm" Font-Bold="true" Font-Size="Small">Edit Cart </asp:LinkButton>
    <asp:LinkButton runat="server" ID="ViewCart"  OnClick="OnServerClick" CssClass="btn btn-primary btn-sm" Font-Bold="true" Font-Size="Small" >View Cart (<asp:Label ID="lblTotalReq" runat="server" Text="0"></asp:Label>)</asp:LinkButton>&nbsp;<img src="../Images/Common/cart.png" class="">
    <%--<a runat="server" href="javascript:;" OnServerClick="OnServerClick">View Cart ()
            <img src="../Images/Common/cart.png" class=""></a></span>--%>
    
<uc1:CartItems runat="server" id="CartItems" />
        </ContentTemplate>
</asp:UpdatePanel>
    </asp:Panel>

