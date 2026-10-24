<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SendNotification.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.SendNotification" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-wrapper" runat="server" id="contentWrapper">
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
    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-12">
                    <div class="form-group">
                        <asp:Label ID="lblFrom" AssociatedControlID="txtFrom" runat="server" Text="From" CssClass="col-xs-2 control-label"></asp:Label>
                        <div class="col-xs-10">
                            <asp:TextBox ID="txtFrom" CssClass="form-control" runat="server" Enabled="false"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblTo" AssociatedControlID="ddlTo" runat="server" Text="To" CssClass="col-xs-2 control-label" required="required"></asp:Label>
                        <div class="col-xs-10">
                            <asp:DropDownList ID="ddlTo" CssClass="form-control selectpicker" runat="server" data-width="100%"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblSubject" AssociatedControlID="txtSubject" runat="server" Text="Subject" CssClass="col-xs-2 control-label" required="required"></asp:Label>
                        <div class="col-xs-10">
                            <asp:TextBox ID="txtSubject" CssClass="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblAttachFile" AssociatedControlID="AsyncUploadAttach" runat="server" Text="Attach File" CssClass="col-xs-2 control-label"></asp:Label>
                        <div class="col-xs-10">
                            <telerik:RadAsyncUpload RenderMode="Lightweight" runat="server" ID="AsyncUploadAttach"
                                HideFileInput="true" AllowedFileExtensions=".jpeg,.jpg,.png,.doc,.docx,.xls,.xlsx,.csv,.pdf,.txt,.zip"
                                Width="100%" EnableInlineProgress="true" MaxFileInputsCount="1"
                                OnClientValidationFailed="OnClientValidationFailed" MaxFileSize="10485760" />
                            <span>(<%= String.Join( ",", AsyncUploadAttach.AllowedFileExtensions ) %>)</span>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblContent" AssociatedControlID="txtContent" runat="server" Text="Content" CssClass="col-xs-2 control-label"  required="required"></asp:Label>
                        <div class="col-xs-10">
                            <asp:TextBox ID="txtContent" runat="server" CssClass="form-control" Width="100%"
                                TextMode="MultiLine" Rows="10"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
