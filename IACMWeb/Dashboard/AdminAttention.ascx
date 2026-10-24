<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminAttention.ascx.cs" Inherits="DI.Chakra.Web.Dashboard.AdminAttention" %>
<%@ Register Src="~/Dashboard/DashBoardHeader.ascx" TagPrefix="uc1" TagName="DashBoardHeaderUC" %>

<uc1:DashBoardHeaderUC runat="server" ID="DashBoardHeaderUC" />

<div class="dashboard-box dashboard-content panel panel-default">
    <div class="panel-body">

        <div class="dashboard-content  pending-tasks">
            <asp:Panel ScrollBars="Auto" runat="server" ID="pnlTask" Height="165px">
                <div class="col-xs-6">
                    <ul class="list-group results-item bordered-bottom">
                        <asp:Repeater ID="Repeater1" runat="server">
                            <ItemTemplate>
                                <li class="list-group-item">
                                    <asp:LinkButton ID="lnkTaskDetail" runat="server" CommandName='<%# Eval("CommandName") %>' OnClick="lnkTaskDetail_Click">
                                        <h5 class="list-group-item-heading">
                                            <asp:Label ID="lblRequestType" runat="server" Text='<%# Eval("CommandName") %>' />
                                            : <%# Eval("Count") %>
                                        </h5>
                                    </asp:LinkButton>

                                </li>

                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>
                <div class="col-xs-6">
                    <ul class="list-group results-item bordered-bottom">
                        <asp:Repeater ID="TaskRepeater" runat="server">
                            <ItemTemplate>
                                <li class="list-group-item">
                                    <asp:LinkButton ID="lnkTaskDetail" runat="server" CommandName='<%# Eval("CommandName") %>' OnClick="lnkTaskDetail_Click">
                                        <h5 class="list-group-item-heading">
                                            <asp:Label ID="lblRequestType" runat="server" Text='<%# Eval("CommandName") %>' />
                                            : <%# Eval("Count") %>
                                        </h5>
                                    </asp:LinkButton>

                                </li>

                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>
            </asp:Panel>
        </div>

    </div>
</div>
