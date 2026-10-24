<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CartItemUC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.Common.CartItemUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/UserSODConflicts.ascx" TagPrefix="uc1" TagName="UserSODConflicts" %>
<%@ Register Src="~/NACR/Create/Common/AdhocMoreDetails.ascx" TagPrefix="uc1" TagName="AdhocMoreDetails" %>


<style>
    .RadWindow_IACMSkin .modal-body {
        /*min-height: 100%;*/
        height: 350px;
        overflow-y: scroll;
    }
</style>
<asp:UpdatePanel runat="server" UpdateMode="Always">
    <ContentTemplate>
        <script>
            function OnClientValidationFailed(sender, args) {
                var fileExtention = args.get_fileName().substring(args.get_fileName().lastIndexOf('.') + 1, args.get_fileName().length);
                if (args.get_fileName().lastIndexOf('.') != -1) {//this checks if the extension is correct
                    if (sender.get_allowedFileExtensions().indexOf(fileExtention) == -1) {
                        showAlert("Wrong Extension!", "Error");
                    }
                    else {
                        showAlert("Wrong file size!", "Error");
                    }
                }
                else {
                    showAlert("not correct extension!", "Error");
                }
                setTimeout(function () { deleteInvalid(sender) }, 250);//wait for the file to actually be added to the async uploads array before trying to delete it
            }

            function deleteInvalid(sender) {
                var invalidIndexes = [];
                $telerik.$(".ruUploadSuccess, .ruUploadFailure", sender.get_element()).each(function (index) {
                    if ($telerik.$(this).hasClass('ruUploadFailure')) {
                        invalidIndexes.push(index);
                    }
                });
                while (invalidIndexes.length) {
                    sender.deleteFileInputAt(invalidIndexes.pop());
                }
            }
        </script>

        <div class="form">
            <div class="form-horizontal left clearfix">
                <div class="form-group">
                    <div class="col-xs-12">
                        <telerik:RadGrid runat="server" ID="gridAllRequests" OnNeedDataSource="gridAllRequests_NeedDataSource"
                            ClientSettings-EnablePostBackOnRowClick="false"
                            OnDeleteCommand="gridAllRequests_DeleteCommand" OnItemDataBound="gridAllRequests_OnItemDataBound" OnItemCommand="gridAllRequests_ItemCommand" EnableLinqExpressions="false" AllowPaging="true" AllowSorting="false" AllowFilteringByColumn="false">
                            <PagerStyle Mode="NextPrevAndNumeric" PageSizes="10" Height="100%" AlwaysVisible="true" />
                            <ClientSettings>
                                <Scrolling UseStaticHeaders="true" AllowScroll="true" />
                            </ClientSettings>
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView TableLayout="Fixed" ShowHeadersWhenNoRecords="true" Width="150%" AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="false" DataKeyNames="Id,AdhocAccessRequest.AdhocActionType,AdhocAccessRequest.EndpointSystemId,AdhocAccessRequest.RequesteeUserId,AdhocAccessRequest.RequesterUserId,RequestedBy">
                                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                                <Columns>
                               <telerik:GridTemplateColumn UniqueName="CheckTemp" HeaderText="CheckTemp" AllowFiltering="false" AllowSorting="false" Exportable="false" HeaderStyle-Width="40px" FilterControlWidth="65%">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="CheckBox2" AutoPostBack="true" Visible="true" Checked="true" runat="server" OnCheckedChanged="CheckBox2_CheckedChanged" style="position:relative"/>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="ClientSelectColumn" runat="server" AutoPostBack="true" Checked="true" OnCheckedChanged="ClientSelectColumn_CheckedChanged" />
                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                     <telerik:GridButtonColumn ConfirmText="Delete Request?" ConfirmDialogType="RadWindow"
                                        ConfirmTitle="Delete" ButtonType="ImageButton" CommandName="Delete" Text="Delete" HeaderText="Delete"
                                        UniqueName="DeleteColumn" Exportable="false" HeaderStyle-Width="80px" ItemStyle-Width="80px">
                                        <ItemStyle HorizontalAlign="Center" CssClass="action-delete" />
                                    </telerik:GridButtonColumn>
                                    <telerik:GridBoundColumn DataField="Id" HeaderText="Task ID" SortExpression="TaskID" UniqueName="Id" HeaderStyle-Width="10%" ItemStyle-Width="10%" Display="false">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridTemplateColumn HeaderText="Application Name" UniqueName="SystemType" HeaderStyle-Width="23%" ItemStyle-Width="23%">
                                        <ItemTemplate>
                                            <asp:Label ID="Label4" runat="server" Text='<%# Eval("AdhocAccessRequest.EndPoint.DisplayName") %>' Style="word-wrap: normal; word-break: break-all;" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Request Type" DataField="AdhocAccessRequest.AdhocActionType" SortExpression="AdhocActionType" UniqueName="SystemType" HeaderStyle-Width="23%" ItemStyle-Width="23%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAdhocActionType" runat="server" Text='<%# Eval("AdhocActionTypeDesc") %>' Style="word-wrap: normal; word-break: break-all;" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                   <%-- <telerik:GridBoundColumn DataField="AdhocAccessRequest.AdhocActionType" SortExpression="AdhocActionType" HeaderText="Request Type"
                                        HeaderStyle-Width="23%" ItemStyle-Width="23%" UniqueName="AdhocActionType">
                                    </telerik:GridBoundColumn>--%>
                                     <%-- <telerik:GridButtonColumn ButtonType="ImageButton" CommandName="Details" Text="Details" HeaderText="Details"
                                        UniqueName="Details" Exportable="false" HeaderStyle-Width="80px" ImageUrl="~/Images/action-buttons/detail.png" ItemStyle-Width="80px">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </telerik:GridButtonColumn>--%>
                                    <telerik:GridTemplateColumn  HeaderText="Details"  UniqueName="Details" Exportable="false" HeaderStyle-Width="80px" ItemStyle-Width="80px">
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:ImageButton ID="Details" runat="server" ImageUrl="~/Images/action-buttons/detail.png" CommandName="Details" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridBoundColumn DataField="AdhocAccessRequest.DisplayName" SortExpression="DisplayName" HeaderText="Request For"
                                        HeaderStyle-Width="23%" ItemStyle-Width="23%" UniqueName="DisplayName">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="RequestedBy" SortExpression="RequestedBy" HeaderText="Request By"
                                        HeaderStyle-Width="23%" ItemStyle-Width="23%" UniqueName="RequestedBy">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="StartDate" DataFormatString="{0:d}" SortExpression="StartDate" HeaderText="Start Date"
                                        HeaderStyle-Width="15%" ItemStyle-Width="15%" UniqueName="StartDate">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="EndDate" DataFormatString="{0:d}" SortExpression="EndDate" HeaderText="End Date"
                                        HeaderStyle-Width="15%" ItemStyle-Width="15%" UniqueName="EndDate">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="AdhocAccessRequest.AccountType" SortExpression="AccountType" HeaderText="Account Type"
                                        HeaderStyle-Width="15%" ItemStyle-Width="15%" UniqueName="AccountType">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="AdditionalInfoFirst" SortExpression="AdditionalInfoFirst" HeaderText="Account Name"
                                        HeaderStyle-Width="23%" ItemStyle-Width="23%" UniqueName="AdditionalInfoFirst">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="AccessItemType" SortExpression="AccessItemType" Visible="false" Display="false" HeaderText="Entitlement Type"
                                        HeaderStyle-Width="15%" ItemStyle-Width="15%" UniqueName="AccessItemType">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="AdhocAccessRequest.Entitlement" SortExpression="Entitlement" HeaderText="Entitlement"
                                        HeaderStyle-Width="23%" ItemStyle-Width="23%" UniqueName="Entitlement">
                                    </telerik:GridBoundColumn>
                                    <%-- <telerik:GridDateTimeColumn DataField="AdhocAccessRequest.CreatedDate" HeaderText="Request Date" SortExpression="RequestDate" UniqueName="RequestDate"
                                        HeaderStyle-Width="15%">
                                    </telerik:GridDateTimeColumn>--%>
                                   
                                  
                                    <telerik:GridBoundColumn DataField="AccessItemType" SortExpression="AccessItemType" Visible="false" Display="false" HeaderText="Entitlement Type"
                                        HeaderStyle-Width="15%" ItemStyle-Width="15%" UniqueName="AccessItemType">
                                    </telerik:GridBoundColumn>
                                    <%--<telerik:GridTemplateColumn AllowFiltering="false" HeaderText="Add Attachment" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="panel" ChildrenAsTriggers="true" OnUnload="panel_Unload">
                                                <ContentTemplate>
                                                    <contenttemplate>
                                                <asp:ImageButton ID="AttachmentImage" runat="server" CommandName="file" ImageUrl='<%# Convert.ToBoolean(Eval("HasAttachment")) == true ? "~/Images/action-buttons/attachmentb.png" : "~/Images/action-buttons/Attachment.png" %>' />
                                            </contenttemplate>
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="AttachmentImage" />
                                                </Triggers>
                                            </asp:UpdatePanel>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="action-attachment" HorizontalAlign="Center" />
                                    </telerik:GridTemplateColumn>--%>


                                    <%--<telerik:GridTemplateColumn UniqueName="TemplateInfoColumn" HeaderStyle-Width="5%" FilterControlWidth="52%" AllowFiltering="false" HeaderText="Risk Info">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="ShowWarningIcon" runat="server" CommandName="ShowWarningInfo"></asp:ImageButton>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="warning-info" HorizontalAlign="Center" />
                                    </telerik:GridTemplateColumn>--%>

                                    <%--<telerik:GridTemplateColumn UniqueName="TemplateEditColumn" HeaderText="Account Details"  HeaderStyle-Width="80px">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="ShowIcon" runat="server" CommandName="Details"></asp:ImageButton>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="action-info" HorizontalAlign="Center" />
                                    </telerik:GridTemplateColumn>--%>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                        <br />
                        <div class="col-xs-12">
                            <div class="buttons text-right topBuffer" runat="server" id="divInterimNotification">
                                <asp:Label Visible="false" runat="server" ID="InterimNotifications" class="description orange">Unsubmitted requests will be deleted after {0} days.</asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>


        <telerik:RadWindow runat="server" ID="SODConflictWindow" Modal="true" Height="100%" Width="700px" Title="Approver Chain"
            RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h3 class="modal-title">Risk Information</h3>
                        </div>
                        <div class="modal-body">
                            <div class="row-fluid">
                                <uc1:UserSODConflicts runat="server" ID="UserSODConflict" isFromAccessRequest="true" ShowNewConflictAlone="true" LoadFromInterimstore="true" />
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button runat="server" ID="btnCancel" class="btn btn-primary bordered-button" Text="OK" OnClick="btnCancel_Click" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>

        <telerik:RadWindow runat="server" ID="detailWindow" Modal="true" Title="Detail" Width="1000px" Height="200%"
            RenderMode="Lightweight" Behaviors="Close" DestroyOnClose="true" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>

                        <div class="modal-header">
                            <h3 class="modal-title">Request Details:</h3>
                        </div>

                        <div class="modal-body">
                            <div class="row-fluid">
                                <uc1:AdhocMoreDetails runat="server" ID="AdhocMoreDetails" />
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button runat="server" ID="Button2" class="btn btn-primary bordered-button" Text="Close" OnClick="detail_OK_Click" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>



        <telerik:RadWindow runat="server" ID="AddAttachmentWindow" Modal="true" Title="Add Attachment" Width="200px" Height="100%"
            RenderMode="Lightweight" Behaviors="Close" DestroyOnClose="true" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h3 class="modal-title">Add Attachment</h3>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal left clearfix">
                                <div class="row-fluid">
                                    <div class="col-xs-12" id="removeAttach" runat="server">
                                        <div class="form-group required">
                                            <asp:Label ID="Label2" runat="server" AssociatedControlID="attachedFileName" Text="Attached File" CssClass="col-xs-4"> Attached File</asp:Label>
                                            <div class="col-xs-7">
                                                <asp:Label ID="attachedFileName" runat="server" Text="">asdfd.pdf</asp:Label>
                                                <asp:LinkButton ID="removeButton" runat="server" Text="X" Font-Bold="true" OnClick="removeButton_Click">   X</asp:LinkButton>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-xs-12">
                                        <div class="form-group required">
                                            <asp:Label ID="Label1" AssociatedControlID="AsyncUploadAttach" runat="server" Text="Attach Files" CssClass="col-xs-4 control-label"></asp:Label>
                                            <div class="col-xs-7">
                                                <telerik:RadAsyncUpload ViewStateMode="Enabled" RenderMode="Lightweight" runat="server" ID="AsyncUploadAttach" OnClientValidationFailed="OnClientValidationFailed"
                                                    HideFileInput="true" MaxFileSize="10485760" AllowedFileExtensions=".jpeg,.jpg,.png,.doc,.docx,.xls,.xlsx,.csv,.pdf,.txt,.zip"
                                                    Width="100%" EnableInlineProgress="false" MultipleFileSelection="Disabled" MaxFileInputsCount="1" />
                                                <span style="display: none" class="allowed-attachments">Allowed File Formats <span class="allowed-attachments-list">(<%= String.Join( ",", AsyncUploadAttach.AllowedFileExtensions ) %>)</span></span>
                                                <br />
                                                <span class="allowed-attachments">Allowed only One file </span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="modal-footer">
                            <asp:Button runat="server" ID="Cancel" class="btn btn-primary bordered-button" Text="Cancel" OnClick="Cancel_Click" />
                            <asp:Button runat="server" ID="Save" class="btn btn-primary bordered-button" Text="OK" OnClick="Save_Click" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>

    </ContentTemplate>
</asp:UpdatePanel>
