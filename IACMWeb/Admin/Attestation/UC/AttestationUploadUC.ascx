<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttestationUploadUC.ascx.cs" Inherits="DI.Chakra.Web.Admin.Attestation.UC.AttestationUploadUC" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Configuration/Data/Endpoint/XLUploadUC.ascx" TagPrefix="uc1" TagName="XLUploadUC" %>
<%@ Register Src="~/Admin/Attestation/UC/AttachmentRowUC.ascx" TagPrefix="uc1" TagName="AttachmentRowUC" %>
<%@ Register Src="~/Admin/Attestation/UC/CommentsRowUC.ascx" TagPrefix="uc1" TagName="CommentsRowUC" %>


<script type="text/javascript">
        function onRequestStart(sender, args) {
            if (args.get_eventTarget().indexOf("requestList") >= 0) {
                args.set_enableAjax(false);
            }
        }
    </script>

<div id="exTab1" class="content-wrapper">
    <ul class="nav nav-tabs iacm-tabs" id="AttestationUpload">
        <li class="active"><a href="#attachments" data-toggle="tab">Attachments <span class="attc-count"><asp:Label runat="server" ID="lblAttachmentCount" /></span></a></li>
        <li><a href="#comments" data-toggle="tab">Comments <span class="attc-count"><asp:Label runat="server" ID="lblCommentsCount" /></span></a></li>
    </ul>
    <div class="tab-content clearfix">
        <div class="tab-pane active" id="attachments">
            <div class="row-fluid">
                <div class="ctrl-box">
                    <div class="grd-list attachments">
                        <%-- Attachments list --%>
                        <telerik:RadListView runat="server" RenderMode="Lightweight" ID="requestList" OnNeedDataSource="requestList_NeedDataSource" >
                            <ItemTemplate>
                                <uc1:AttachmentRowUC runat="server" id="AttachmentRowUC" />
                            </ItemTemplate>
                        </telerik:RadListView>
                    </div>                    
                </div>
                <%--Add new attachment--%>
                <div id="AddNewComments" class="form-group" runat="server">
                    <asp:Label ID="lblAttachFile" AssociatedControlID="AsyncUploadAttach" runat="server" Text="Attach Files" CssClass="col-xs-2 control-label"></asp:Label>
                    <div class="col-xs-7">
                        <telerik:RadAsyncUpload RenderMode="Lightweight" runat="server" ID="AsyncUploadAttach" OnClientValidationFailed="OnClientValidationFailed"
                            HideFileInput="true" MaxFileSize="10485760" AllowedFileExtensions=".jpeg,.jpg,.png,.doc,.docx,.xls,.xlsx,.csv,.pdf,.txt,.zip"
                            Width="100%" EnableInlineProgress="false" />

                        <span style="display: none" class="allowed-attachments">Allowed File Formats <span class="allowed-attachments-list">(<%= String.Join( ",", AsyncUploadAttach.AllowedFileExtensions ) %>)</span></span>
                    </div>
                </div>
            </div>
        </div>
        <div class="tab-pane" id="comments">
            <div class="row-fluid">
                <div class="ctrl-box">
                    <div class="grd-list comments">
                        <%-- Comments list --%>
                        <telerik:RadListView runat="server" RenderMode="Lightweight" ID="commentsList" OnNeedDataSource="commentsList_NeedDataSource" >
                            <ItemTemplate>
                                <uc1:CommentsRowUC runat="server" ID="CommentsRowUC" />
                            </ItemTemplate>
                        </telerik:RadListView>
                    </div>
                </div>
            </div>
            <%--Add new comment--%>
            <div class="form-group">
                <label for="CommentsTextBox" class="col-xs-3 control-label">Comment:</label>
                <div class="col-xs-9">
                    <asp:TextBox ID="CommentsTextBox" CssClass="form-control" runat="server" TextMode="MultiLine" Rows="5"></asp:TextBox>
                </div>
            </div>
        </div>
    </div>
</div>
