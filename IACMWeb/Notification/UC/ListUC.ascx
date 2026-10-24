<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Notification.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-wrapper">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                    <div class="form-group">
                        <asp:Label ID="lblStatus" AssociatedControlID="ddlStatus" runat="server" Text="Notification Status" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:DropDownList ID="ddlStatus" CssClass="form-control selectpicker" runat="server" data-width="100%" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged"></asp:DropDownList>
                        </div>
                    </div>
                </div>
                <%--                    <div class="col-xs-6">
                        <div class="form-group">
                            <asp:Label AssociatedControlID="ShowAllCheckBox" runat="server" CssClass="control-label">
                                <asp:CheckBox ID="ShowAllCheckBox" runat="server" AutoPostBack="True" />
                                <span class="lbl">Show All Notifications</span>
                            </asp:Label>
                        </div>
                    </div>--%>
            </div>
        </div>
    </div>
</div>
<div class="topBuffer">
    <cc1:IACMRadGridControl ID="radgridNotification" runat="server" OnInit="radgridNotification_Init"
        OnSelectedIndexChanged="radgridNotification_SelectedIndexChanged" OnNeedDataSource="radgridNotification_NeedDataSource"
        OnItemCommand="radgridNotification_ItemCommand" OnItemDataBound="radgridNotification_ItemDataBound">
        <MasterTableView AutoGenerateColumns="False" ShowHeadersWhenNoRecords="true" CommandItemDisplay="Top" NoMasterRecordsText="No Records Available" 
             DataKeyNames="Id">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="SenderName" FilterControlAltText="SenderName" SortExpression="SenderName"
                     HeaderText="From" AllowFiltering="true" UniqueName="SenderName" HeaderStyle-Width="20%" FilterControlWidth="77%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="ReceiverName" FilterControlAltText="ReceiverName" SortExpression="ReceiverName" HeaderText="To" AllowFiltering="true" UniqueName="ReceiverName" HeaderStyle-Width="15%" FilterControlWidth="68%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Subject" FilterControlAltText="Subject" SortExpression="Subject" HeaderText="Subject" AllowFiltering="true" UniqueName="Subject" HeaderStyle-Width="15%" FilterControlWidth="68%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Content" FilterControlAltText="Content" SortExpression="Content" HeaderText="Content" AllowFiltering="true" UniqueName="Content">
                </telerik:GridBoundColumn>
                 <telerik:GridBoundColumn DataField="HasAttachments" UniqueName="HasAttachments" Display="false"  >
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" HeaderText="Attachment">
                    <ItemTemplate>
                        <asp:UpdatePanel runat="server" ID="UpdatePanelDownload" UpdateMode="Conditional" OnUnload="UpdatePanelDownload_Unload" ChildrenAsTriggers="True">
                            <ContentTemplate>
                                <asp:ImageButton ID="AttachmentImage" runat="server" CommandName="file" />
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
   </cc1:IACMRadGridControl>
   <%-- <cc1:IACMRadGridControl ID="Exploring" runat="server" OnNeedDataSource="Exploring_NeedDataSource">
         <MasterTableView AutoGenerateColumns="False" ShowHeadersWhenNoRecords="true" CommandItemDisplay="Top" NoMasterRecordsText="No Records Available"  DataKeyNames="Id">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="Sender.DisplayName" FilterControlAltText="SenderName" SortExpression="SenderName" HeaderText="From" AllowFiltering="true" UniqueName="SenderName" HeaderStyle-Width="20%" FilterControlWidth="77%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Receiver.DisplayName" FilterControlAltText="ReceiverName" SortExpression="ReceiverName" HeaderText="To" AllowFiltering="true" UniqueName="ReceiverName" HeaderStyle-Width="15%" FilterControlWidth="68%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Subject" FilterControlAltText="Subject" SortExpression="Subject" HeaderText="Subject" AllowFiltering="true" UniqueName="Subject" HeaderStyle-Width="15%" FilterControlWidth="68%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Content" FilterControlAltText="Content" SortExpression="Content" HeaderText="Content" AllowFiltering="true" UniqueName="Content">
                </telerik:GridBoundColumn>
                 <telerik:GridBoundColumn DataField="HasAttachments" UniqueName="HasAttachments" Display="false">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" HeaderText="Attachment">
                    <ItemTemplate>
                        <asp:UpdatePanel runat="server" ID="UpdatePanelDownload" UpdateMode="Conditional" OnUnload="UpdatePanelDownload_Unload" ChildrenAsTriggers="True">
                            <ContentTemplate>
                                <asp:ImageButton ID="AttachmentImge" runat="server" CommandName="file" Height="40px" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:PostBackTrigger ControlID="AttachmentImge" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </cc1:IACMRadGridControl>--%>
</div>

