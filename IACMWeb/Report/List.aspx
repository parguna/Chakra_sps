<%@ Page Title="Report" Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" MasterPageFile="~/Master/ProtoMast4.master" Inherits="DI.Chakra.Web.Report.List" %>

<%@ Register Src="~/Master/UC/Reports.ascx" TagPrefix="uc1" TagName="Reports" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="Server">

    <div class="row-fluid">

        <asp:Repeater ID="ReportRepeater" runat="server" Visible="true">
            <HeaderTemplate>
                <ul class="nav nav-pills nav-stacked di-md-nav-report tab-left col-xs-2">
            </HeaderTemplate>
            <ItemTemplate>
                <li <%# (Container.ItemIndex) == 0 ? "class=active" : string.Empty %>><a href="#tabReport<%#Container.ItemIndex%>" data-toggle="tab"><%# DataBinder.Eval(Container.DataItem, "MenuName") %></a>
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
                        <div id="tabReport<%# (((Container as RepeaterItem).NamingContainer as Repeater).NamingContainer as RepeaterItem).ItemIndex%>"
                            <%# ((((Container as RepeaterItem).NamingContainer as Repeater).NamingContainer as RepeaterItem).ItemIndex) == 0 ? "class='tab-pane topBorderReportPage active'" : "class='tab-pane topBorderReportPage'" %>>
                            <h3><%# DataBinder.Eval(((RepeaterItem)Container.Parent.Parent).DataItem, "MenuName")%></h3>
                            <div class="MC-list-wrapper" style="display: flex; width: 100%; flex-direction: row; flex-wrap: wrap;">
                                <%--<div class="report-list MultiColumn">--%>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="tempcheck" style="width: 25%;">
                            <li <%# (DataBinder.Eval(Container.DataItem, "MenuName")) == string.Empty ? "style='display:none'" : string.Empty%> class="parent" style="list-style:none;">
                                <h4><%# DataBinder.Eval(Container.DataItem, "MenuName") %></h4>
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
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                       <%-- </div>--%>
                        </div>
                        </div>
                    </FooterTemplate>
                </asp:Repeater>
            </ItemTemplate>
            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
