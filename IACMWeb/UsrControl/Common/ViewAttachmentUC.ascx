<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="true" CodeBehind="ViewAttachmentUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.ViewAttachmentUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <div class="content-wrapper" runat="server" id="contentWrapper">
            <div class="topBuffer">
                <telerik:RadGrid ID="radgridViewAttachment" runat="server" AllowPaging="true" AllowSorting="false"
                    OnNeedDataSource="radgridViewAttachment_NeedDataSource" OnItemCommand="radgridViewAttachment_ItemCommand"
                    OnItemDataBound="radgridViewAttachment_ItemDataBound">
                    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" AlwaysVisible="true" />
                    <ClientSettings>
                        <Scrolling UseStaticHeaders="true" />
                        <Selecting AllowRowSelect="true" />
                    </ClientSettings>
                    <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" DataKeyNames="Id,FileName" NoMasterRecordsText="No Attachments Available" EnableHeaderContextMenu="false">
                        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                        <Columns>
                            <telerik:GridBoundColumn DataField="CreatedBy" FilterControlAltText="CreatedBy" HeaderText="Added By" UniqueName="CreatedBy" HeaderStyle-Width="25%">
                            </telerik:GridBoundColumn>
                            <telerik:GridDateTimeColumn DataField="CreatedDate" FilterControlAltText="CreatedDate" HeaderText="Added Date" UniqueName="CreatedDate" HeaderStyle-Width="25%">
                            </telerik:GridDateTimeColumn>
                            <telerik:GridDateTimeColumn DataField="FileName" FilterControlAltText="FileName" HeaderText="File Name" UniqueName="FileName" HeaderStyle-Width="30%">
                            </telerik:GridDateTimeColumn>
                            <telerik:GridTemplateColumn AllowFiltering="false" HeaderText="Attachment" HeaderStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:UpdatePanel runat="server" ID="UpdatePanelDownload" UpdateMode="Conditional" ChildrenAsTriggers="True" OnUnload="UpdatePanelDownload_Unload">
                                        <ContentTemplate>
                                            <asp:ImageButton ID="AttachmentImage" runat="server" CommandName="file" AlternateText="file" Height="40px" />
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:PostBackTrigger ControlID="AttachmentImage" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
