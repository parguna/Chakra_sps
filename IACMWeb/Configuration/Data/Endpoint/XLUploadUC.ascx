<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="Provisioning_XLUpload_XLUploadUC" CodeBehind="XLUploadUC.ascx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<style type="text/css">
    .RadUpload {
    }

    .auto-style1 {
        width: 160px;
    }

    .btnpos {
        text-align: center;
        margin-top: 5px;
    }
</style>
<script type="text/javascript">
    /* Jquery to set validation when no file is selected for upload */
    var ALERT_TITLE = "Oops!";
    var ALERT_BUTTON_TEXT = "Ok";

    //var validatable = $("form").kendoValidator().data("kendoValidator");
    $(window).load(function () {

        $('.ruFileInput').attr("required", "required");
        $('.ruFileInput').attr("data-required-msg", "Choose any file to upload");
        $("span.k-tooltip-validation").remove();
        //validatable.destroy();

        $(".import").click(function () {
            x = $(".ruFileInput").next("span.k-tooltip-validation").detach();
            $(".ruFileInput").parent("span.ruFileWrap").before(x);
        });

        $('.validation').focusout(function () {
            $("span.k-tooltip-validation").remove();
            //validatable.destroy();

        });

    });

    function OnClientFileSelected(sender, args) {
        $("span.k-tooltip-validation").remove();
        //validatable.destroy();
        var $span = $telerik.$(".ruUploadProgress", args.get_row());
        var name = $span.text();
        $span.attr('title', name);
        //if (name.length > 22) {
        //var newString = name.substring(0, 22) + "...";
        //$span.text(newString);
        //}
    }

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

    function ShowProgress() {
        $("#UpdateProgressBar").show();
    }

</script>
<div id="HeaderText" runat="server" class="col-xs-12">
    <asp:Label ID="lblChooseFile" AssociatedControlID="FileRadAsyncUpload" runat="server" CssClass="col-xs-2 control-label">Choose File:</asp:Label>
    <div class="col-xs-6">
        <telerik:RadAsyncUpload OnClientFileSelected="OnClientFileSelected" MaxFileSize="10485760" MultipleFileSelection="Disabled" OnFileUploaded="FileRadAsyncUpload_FileUploaded" OnClientValidationFailed="OnClientValidationFailed" AllowedFileExtensions="csv" AutoAddFileInputs="False" ID="FileRadAsyncUpload" runat="server">
        </telerik:RadAsyncUpload>
       
    </div>
     <div >
     <asp:Label ID="Label1"  runat="server" CssClass="col-xs-3 control-label"> Allowed format is csv</asp:Label>
         </div>
    <div class="col-xs-4" id="btnCancel" runat="server">
        <asp:Button ID="btnImport" runat="server" Text="Import" OnClick="btnImport_Click" OnClientClick="ShowProgress();" CssClass="btn btn-default btn-sm validation import" />
    </div>
</div>

<div class="col-xs-12">
    <asp:Label ID="lblPartial" runat="server" CssClass="col-xs-2" AssociatedControlID="chkPartial" Text="Partial upload">
    </asp:Label>
    <asp:CheckBox ID="chkPartial" CssClass="col-xs-4" runat="server" Checked="true" />
</div>
