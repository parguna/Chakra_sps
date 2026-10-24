<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuickNavigator.ascx.cs" Inherits="DI.Chakra.Web.Master.UC.QuickNavigator" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<div class="quicklinks col-xs-8 text-right">
    <ul class="list-inline">
        <asp:Repeater ID="rptQuickLinks" runat="server">
            <ItemTemplate>
                <li>
                    <asp:ImageButton ID="imgDelete" Visible="<%#DeleteIconVisible%>"  ImageUrl="~/Images/action-buttons/remove.png" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "MenuName") %>' OnClick="imgDelete_Click" runat="server" CssClass="remove_button" />
                    <a href='<%# DataBinder.Eval(Container.DataItem, "TargetUrl") %>' class="grow">
                        <span class="glyphicon glyphicon-plus red"></span>
                        <h5 class="quicklink-title"><%# DataBinder.Eval(Container.DataItem, "MenuName") %></h5>
                    </a>
                    <asp:HiddenField runat="server" ID="TabMemnuId" Value='<%# DataBinder.Eval(Container.DataItem, "TabMenuId") %>' />
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
</div> 
<asp:LinkButton ID="btnCustomize" OnClick="btnCustomize_Click"  runat="server" CssClass="customize_button" Visible="false"></asp:LinkButton>
