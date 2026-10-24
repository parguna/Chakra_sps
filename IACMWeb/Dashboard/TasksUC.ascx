<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TasksUC.ascx.cs" Inherits="DI.Chakra.Web.Dashboard.TasksUC" %>
<%@ Register Src="~/Dashboard/DashBoardHeader.ascx" TagPrefix="uc1" TagName="DashBoardHeaderUC" %>

<uc1:DashBoardHeaderUC runat="server" ID="DashBoardHeaderUC" />

<div class="dashboard-box dashboard-content panel panel-default">
    <div class="panel-body">

        <div class="dashboard-content  pending-tasks">
            <asp:Panel ScrollBars="Auto" runat="server" ID="pnlTask" Height="165px">
                <ul class="list-group results-item bordered-bottom">
                    <asp:Repeater ID="TaskRepeater" runat="server" OnItemDataBound="TaskRepeater_ItemDataBound">
                        <ItemTemplate>
                            <li class="list-group-item">
                                <asp:LinkButton ID="lnkTaskDetail" runat="server" CommandName='<%# Eval("RequestType") %>' CommandArgument='<%# Eval("TaskId") %>' OnClick="lnkTaskDetail_Click">
                                    <h5 class="list-group-item-heading">
                                        <asp:Label ID="lblRequestType" runat="server" Text='<%# Eval("RequestType").ToString() =="Attestation" ? "Access Review": Eval("RequestType").ToString()  %>'/>
                                        : <%# Eval("TaskName") %>
                                        <asp:Label ID="lblRequestID" runat="server" Text='<%#" (Id - "+ Eval("AccessRequestID")+")" %>' />
                                    </h5>
                                </asp:LinkButton><p class="list-group-item-text">
                                    <span class="task-description">
                                        <asp:Label ID="lblDate" runat="server" Text='<%# Eval("RequestedDate") %>' />
                                        <asp:Label ID="lblRequestedBy" runat="server" Text='<%#"| Requested By - "+ Eval("RequestedBy")%>' />
                                        </br>
                                            <asp:Label ID="lblRequestFor" runat="server" Text='<%#"Requested For - "+ Eval("RequestedFor")%>' />
                                    </span>
                                </p>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </asp:Panel>
        </div>

    </div>
</div>
