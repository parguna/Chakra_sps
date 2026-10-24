<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AlertMessage.ascx.cs" Inherits="DI.Chakra.Web.UserControls.AlertMessage" %>
<script>
    //Automatically hides alert box after 5secs

    function ValidationMessageClose() {
        var alertDisplay = $(".alertMessageDiv").css("display");
        if (alertDisplay && alertDisplay != "none") {
            window.setTimeout(function () {
                $(".alert").hide();
            }, 8000);
        }

    }
    $(document).ready(function () {
        ValidationMessageClose();
    });
    ValidationMessageClose();
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(RunThisAfterEachAsyncPostback);
    function RunThisAfterEachAsyncPostback() {
        ValidationMessageClose();
    }
</script>
<div class="row-fluid alertMessageDiv" id="message" runat="server" style="display: none;">
    <div role="alert" data-dismiss="alert" class="alertInnerMessage alert alert-warning col-xs-3 fade" id="innermessage" runat="server">
        <span class="alert-icon"></span>
        <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
        <h5>
            <asp:Label ID="lblTitle" runat="server" CssClass="txtAlertTitle" Text="Label"></asp:Label>
        </h5>
        <asp:Label ID="lblMessage" runat="server" CssClass="txtAlertMsg" Text="Label"></asp:Label>
    </div>
</div>

