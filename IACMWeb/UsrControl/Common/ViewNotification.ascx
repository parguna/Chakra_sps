<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewNotification.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.ViewNotification" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

        <div class="content-wrapper" runat="server" id="contentWrapper">
            <div class="topBuffer">
                <telerik:RadGrid runat="server" ID="radgridViewNotifications" AutoGenerateColumns="False" GroupPanelPosition="Top"
                    AllowPaging="true" AllowSorting="true" ClientSettings-EnablePostBackOnRowClick="true" FilterMenu-CssClass="RadMenu_ IACMFilterSkin" OnSelectedIndexChanged="radgridViewNotifications_SelectedIndexChanged"
                    OnDetailTableDataBind="radgridViewNotifications_DetailTableDataBind" OnItemCommand="radgridViewNotifications_ItemCommand" OnItemDataBound="radgridViewNotifications_ItemDataBound" 
                    OnNeedDataSource="radgridViewNotifications_NeedDataSource" OnSortCommand="radgridViewNotifications_SortCommand">
                    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" AlwaysVisible="true" />
                    <ClientSettings>
                        <Scrolling UseStaticHeaders="true" AllowScroll="true" />
                        <Selecting AllowRowSelect="true" />
                    </ClientSettings>
                    <MasterTableView AutoGenerateColumns="False" Name="Master" CommandItemDisplay="None" EnableHeaderContextMenu="false" DataKeyNames="Id"
                        NoMasterRecordsText="No Records Available">
                        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                        <Columns>
                            <telerik:GridBoundColumn SortExpression="SenderName" HeaderText="From" HeaderButtonType="TextButton" DataField="SenderName" HeaderStyle-Width="25%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn SortExpression="ReceiverName" HeaderText="To" HeaderButtonType="TextButton" DataField="ReceiverName" HeaderStyle-Width="25%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn SortExpression="Subject" HeaderText="Subject" HeaderButtonType="TextButton" DataField="Subject" HeaderStyle-Width="35%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="HasAttachments" UniqueName="HasAttachments" Display="false">
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn AllowFiltering="false" HeaderText="Attachment" HeaderStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:UpdatePanel runat="server" ID="UpdatePanelDownload" UpdateMode="Conditional" ChildrenAsTriggers="True" OnUnload="UpdatePanelDownload_Unload">
                                        <ContentTemplate>
                                            <asp:ImageButton ID="MAttachmentImage" runat="server" CommandName="file"  />
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:PostBackTrigger ControlID="MAttachmentImage" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </ItemTemplate>
                                <ItemStyle CssClass="action-attachment" HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                        </Columns>
                        <DetailTables>
                            <telerik:GridTableView DataKeyNames="Id" Name="NotificationReplies" ShowHeadersWhenNoRecords="false" NoDetailRecordsText="No Records found">
                                <Columns>
                                    <telerik:GridBoundColumn SortExpression="SenderName" HeaderText="From" HeaderButtonType="TextButton" DataField="SenderName" HeaderStyle-Width="25%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn SortExpression="ReceiverName" HeaderText="To" HeaderButtonType="TextButton" DataField="ReceiverName" HeaderStyle-Width="25%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn SortExpression="Subject" HeaderText="Subject" HeaderButtonType="TextButton" DataField="Subject" HeaderStyle-Width="35%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="HasAttachments" UniqueName="HasAttachments" Display="false">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridTemplateColumn AllowFiltering="false" HeaderText="Attachment" HeaderStyle-Width="15%">
                                        <ItemTemplate>
                                            <asp:UpdatePanel runat="server" ID="UpdatePanelDownload" UpdateMode="Conditional" ChildrenAsTriggers="True">
                                                <ContentTemplate>
                                                    <asp:ImageButton ID="AttachmentImage" runat="server" CommandName="file" ImageUrl="~/Images/action-buttons/Attachment.png" />
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:PostBackTrigger ControlID="AttachmentImage" />
                                                </Triggers>
                                            </asp:UpdatePanel>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="action-attachment" HorizontalAlign="Center" />
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </telerik:GridTableView>
                        </DetailTables>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
            <div class="content-box content-topBuffer">
                <asp:Label ID="lblContent" AssociatedControlID="txtContent" runat="server" Text="Content" CssClass="col-xs-12"></asp:Label>
                <asp:TextBox ID="txtContent" runat="server" CssClass="form-control" Width="100%" TextMode="MultiLine" Rows="3" Enabled="false"></asp:TextBox>
            </div>
        </div>
