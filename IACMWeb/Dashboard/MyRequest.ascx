<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyRequest.ascx.cs" Inherits="DI.Chakra.Web.Dashboard.MyRequest" %>
<%@ Register Src="~/Dashboard/DashBoardHeader.ascx" TagPrefix="uc1" TagName="DashBoardHeaderUC" %>

<uc1:DashBoardHeaderUC runat="server" ID="DashBoardHeaderUC" />
<div class="dashboard-box dashboard-content panel panel-default">
    <div class="panel-body">
        <div class="dashboard-content  pending-tasks">
            <asp:Panel ScrollBars="Auto" runat="server" ID="pnlTask" Height="165px">
                <ul class="list-group results-item bordered-bottom">
                    <asp:Repeater ID="MyRequestRepeater" runat="server" OnItemDataBound="MyRequestRepeater_ItemDataBound">
                        <ItemTemplate>
                            <li class="list-group-item">
                                <asp:LinkButton ID="lnkTaskDetail" runat="server" OnClick="lnkTaskDetail_Click">
                                    <h5 class="list-group-item-heading">
                                        <asp:Label ID="lblRequestType" runat="server" Text='<%# Eval("RequestType") %>' />
                                        : <%# Eval("Name") %> (Id - 
                                        <asp:Label ID="lblRequestID" runat="server" Text='<%# Eval("AccessRequestID") %>' />)
                                        <asp:HiddenField ID="lblAdhocRequestID" runat="server" Value='<%# Eval("AdHocAccessRequestId") %>' />
                                        <asp:HiddenField ID="lblBulkRequestId" runat="server" Value='<%# Eval("BulkRequestId") %>' />
                                        <asp:HiddenField ID="hdnRequestSource" runat="server" Value='<%# Eval("RequestSource") %>' />
                                    </h5>
                                </asp:LinkButton>
                                <p class="list-group-item-text">
                                    <span class="task-description">
                                        <asp:Label ID="lblDate" runat="server" Text='<%# Eval("RequestedDate") %>' />
                                        | 
                                        <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("ProgressStatusString") %>' />
                                        <asp:Label ID="lblReqFor" runat="server" Text='<%#"| Requested For - "+ Eval("RequesteeName") %>' />
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
