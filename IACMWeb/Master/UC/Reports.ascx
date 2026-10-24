<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Reports.ascx.cs" Inherits="DI.Chakra.Web.Master.UC.Reports" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="megadropdown">
    <style>
        .reportMenulink{
            color: #fff;
    text-decoration: none;
    font-weight: 400;
    font-size:16px;
        }
    </style>

    <ul class="nav navbar-nav navbar-left">
        <li class="dropdown d-hover">
            <asp:LinkButton id="mReport" runat="server"  OnClick="mReport_Click" CssClass="reportMenulink"></asp:LinkButton>
            <div class="dropdown-menu" role="menu">
                <div class="row-fluid">

                    <asp:Repeater ID="Report" runat="server">
                        <HeaderTemplate>
                            <ul class="nav nav-pills nav-stacked di-md-nav tab-left col-xs-2">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li <%# (Container.ItemIndex) == 0 ? "class=active" : string.Empty %>><a href="#tab<%#Container.ItemIndex%>" data-toggle="tab"><%# DataBinder.Eval(Container.DataItem, "MenuName") %></a>
                            </li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </asp:Repeater>


                    <asp:Repeater ID="AddReport" runat="server" OnItemDataBound="AddReport_ItemDataBound">
                        <HeaderTemplate>
                            <div class="tab-content col-xs-10 report-list">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Repeater ID="SubReports" runat="server" OnItemDataBound="SubReports_ItemDataBound">
                                <HeaderTemplate>
                                    <div id="tab<%# (((Container as RepeaterItem).NamingContainer as Repeater).NamingContainer as RepeaterItem).ItemIndex%>"
                                        <%# ((((Container as RepeaterItem).NamingContainer as Repeater).NamingContainer as RepeaterItem).ItemIndex) == 0 ? "class='tab-pane active'" : "class='tab-pane'" %>>
                                        <h3><%# DataBinder.Eval(((RepeaterItem)Container.Parent.Parent).DataItem, "MenuName")%></h3>
                                        <div class="MC-list-wrapper">
                                            <ul class="report-list MultiColumn">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <li <%# (DataBinder.Eval(Container.DataItem, "MenuName")) == string.Empty ? "style='display:none'" : string.Empty%> class="parent">
                                       <h4> <%# DataBinder.Eval(Container.DataItem, "MenuName") %></h4>
                                    <asp:Repeater ID="inLineReports" runat="server">
                                        <HeaderTemplate>
                                                <ul>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <li>
                                                <a href="<%# DataBinder.Eval(Container.DataItem, "PageUrl") %>"><%# DataBinder.Eval(Container.DataItem, "MenuName") %></a>
                                            </li>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </ul>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                    </li>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </ul></div></div>
                                </FooterTemplate>
                            </asp:Repeater>
                        </ItemTemplate>
                        <FooterTemplate>
                            </div>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </li>
    </ul>
</div>


