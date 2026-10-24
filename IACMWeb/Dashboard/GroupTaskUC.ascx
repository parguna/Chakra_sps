<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupTaskUC.ascx.cs" Inherits="DI.Chakra.Web.Dashboard.GroupTaskUC" %>
<%@ Register Src="~/Dashboard/DashBoardHeader.ascx" TagPrefix="uc1" TagName="DashBoardHeaderUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<uc1:DashBoardHeaderUC runat="server" ID="DashBoardHeaderUC" />

<div class="dashboard-box dashboard-content panel panel-default">
    <div class="panel-body">
        <div>
            <div class="dashboard-content  Group-tasks">
                <asp:Panel ScrollBars="Auto" runat="server" ID="Panel1" Height="165px">
                    <ul class="list-group results-item bordered-bottom">
                        <asp:Repeater ID="GroupTaskrepeater" runat="server" OnItemDataBound="GroupTaskrepeater_ItemDataBound">
                            <ItemTemplate>
                                <li class="list-group-item">
                                    <asp:LinkButton ID="lnkgroupTaskDetail" runat="server" CommandName='<%# Eval("RequestType") %>' CommandArgument='<%# Eval("TaskId") %>' OnClick="lnkgroupTaskDetail_Click">
                                        <h5 class="list-group-item-heading">
                                            <asp:Label ID="lblRequestType" runat="server" Text='<%# Eval("RequestType").ToString() =="Attestation" ? "Access Review": Eval("RequestType").ToString()  %>' />
                                            : <%# Eval("TaskName") %>
                                            <asp:Label ID="lblRequestID" runat="server" Text='<%#" (Id - "+ Eval("AccessRequestID")+")" %>' />
                                        </h5>
                                    </asp:LinkButton><p class="list-group-item-text">
                                        <span class="task-description">
                                            <asp:Label ID="lblDate" runat="server" Text='<%# Eval("RequestedDate") %>' />
                                            <asp:Label ID="lblRequestedBy" runat="server" Text='<%#"| Requested By - "+ Eval("RequestedBy")%>' />
                                            <br />
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
</div>
