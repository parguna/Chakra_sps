<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemsUC.ascx.cs" Inherits="DI.Chakra.Web.Preferences.ItemsUC" %>

<div class="container-fluid content row center">
    <div id="tiles" class="full-width-container row">
        <asp:Repeater ID="Repeater1" runat="server">
            <ItemTemplate>
                <asp:HyperLink ID="lnkPreferenceItem" runat="server" CssClass='<%#SetPreferenceClass((DI.Chakra.Infrastructure.Enums.AdminModeItemType)DataBinder.Eval(Container.DataItem, "Type"))%>'
                    NavigateUrl='<%# Eval("TabMenu.WebPage.PageUrl.Url") %>' >
                    <div class="tile-container">
                        <div class="tile-content">
                            <div class="tile-top-row">
                                <div class="tile-icon">
                                    <span class="tile-container-img">
                                        <img src="<%# Eval("Icon") %>" class="tile-icon-img"/>
                                    </span>
                                </div>
                                <div class="tile-num" style="width:80%;">
                                    <span><%# Eval("Type") %></span>
                                </div>
                            </div>
                            <div class="tile-bottom-row">
                                <div class="tile-title"><%# Eval("Name") %></div>
                                <div class="tile-info tile-info-success"></div>
                            </div>
                        </div>
                    </div>
                </asp:HyperLink>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
