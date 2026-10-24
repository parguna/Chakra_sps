<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.Notification.UC.DetailUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/SendNotification.ascx" TagPrefix="uc1" TagName="SendNotification" %>



<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <div class="content-wrapper">
            <script type="text/javascript">
        function RowSelecting(sender, args) {
            var UsingKey1 = args.getDataKeyValue("IsRead");
            var UsingKey2 = args.getDataKeyValue("IsReplied");
            window.parent.$("#MarkAsRead").find("input")
                            .attr('disabled', ((UsingKey1 == 'True') || (UsingKey2 == 'True')) ? true : false);
            window.parent.$("#Reply").find("input")
                            .attr('disabled', (UsingKey2 == 'True') ? true : false);
        }
    </script>

            <div class="form">
                <div class="form-horizontal left clearfix">
                    <div class="row-fluid">
                        <asp:LinkButton ID="Task" runat="server" Text="View Request/Task details" OnClick="Task_Click" CssClass="btn-link pull-right"></asp:LinkButton>
                        <asp:HiddenField ID="Request" runat="server" />
                        <asp:HiddenField ID="TaskId" runat="server" />
                    </div>
                </div>
            </div>
            <div class="topBuffer">
                <telerik:RadGrid ID="radgridNotificationDetail" runat="server" PagerStyle-AlwaysVisible="true" AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" CellSpacing="0"
                    FilterMenu-CssClass="RadMenu_IACMFilterSkin" ClientSettings-EnablePostBackOnRowClick="true" OnSelectedIndexChanged="radgridNotificationDetail_SelectedIndexChanged"
                    OnItemCommand="radgridNotificationDetail_ItemCommand" OnItemDataBound="radgridNotificationDetail_ItemDataBound">
                    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" AlwaysVisible="true" />
                    <ClientSettings>
                        <Scrolling UseStaticHeaders="true" />
                        <Selecting AllowRowSelect="true" />
                        <ClientEvents OnRowSelecting="RowSelecting" />
                    </ClientSettings>
                    <MasterTableView DataKeyNames="Id" ClientDataKeyNames="IsRead,IsReplied" AutoGenerateColumns="False" ShowHeadersWhenNoRecords="true" CommandItemDisplay="None" NoMasterRecordsText="No Records Available" EnableHeaderContextMenu="false">
                        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                        <Columns>
                            <telerik:GridBoundColumn DataField="ReceiverTaskId" UniqueName="ReceiverTaskId" Display="false">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="SenderTaskId" UniqueName="SenderTaskId" Display="false">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="SenderName" FilterControlAltText="SenderName" SortExpression="SenderName" HeaderText="From" AllowFiltering="true" UniqueName="SenderName" HeaderStyle-Width="30%" FilterControlWidth="77%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="ReceiverName" FilterControlAltText="ReceiverName" SortExpression="ReceiverName" HeaderText="To" AllowFiltering="true" UniqueName="ReceiverName" HeaderStyle-Width="30%" FilterControlWidth="68%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Subject" FilterControlAltText="Subject" SortExpression="Subject" HeaderText="Subject" AllowFiltering="true" UniqueName="Subject" HeaderStyle-Width="15%" FilterControlWidth="68%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="HasAttachments" UniqueName="HasAttachments" Display="false">
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn AllowFiltering="false" HeaderText="Attachment" HeaderStyle-Width="15%">
                                <ItemTemplate>
                                    <asp:UpdatePanel runat="server" ID="UpdatePanelDownload" UpdateMode="Conditional" ChildrenAsTriggers="True">
                                        <ContentTemplate>
                                            <asp:ImageButton ID="AttachmentImage" runat="server" CommandName="file"  />
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:PostBackTrigger ControlID="AttachmentImage" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </ItemTemplate>
                                <ItemStyle CssClass="action-attachment" HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
            <div class="content-box content-topBuffer">
                <asp:Label ID="lblContent" AssociatedControlID="txtContent" runat="server" Text="Content" CssClass="col-xs-12"></asp:Label>
                <asp:TextBox ID="txtContent" runat="server" CssClass="form-control" Width="100%" TextMode="MultiLine" Rows="3" ReadOnly="true"></asp:TextBox>
            </div>
        </div>

        <telerik:RadWindow runat="server" ID="RadWinReplyNotification" Modal="true" Width="600px" Height="100%"
            RenderMode="Lightweight" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h3 class="modal-title"><%= T("App.Common.ReplyNotification") %></h3>
                        </div>
                        <div class="modal-body">
                            <div class="row-fluid">
                                <uc1:SendNotification runat="server" ID="UCReplyNotification" />
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button runat="server" ID="btnReply" class="btn btn-primary bordered-button" Text="Reply" OnClick="btnReply_Click" />
                            <asp:Button runat="server" ID="btnCancelReply" class="btn btn-primary bordered-button" Text="Cancel" OnClick="btnCancelReply_Click" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>
    </ContentTemplate>
</asp:UpdatePanel>
<div class="buttons text-right nav-rectangle">
    <asp:Button runat="server" class="btn btn-default btn-sm" TabIndex="8" ID="Cancel" Text="Cancel" OnClick="Cancel_Click" UseSubmitBehavior="false"></asp:Button>
</div>
