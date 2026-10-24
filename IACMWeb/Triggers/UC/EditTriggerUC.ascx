<%@ Control Language="C#" AutoEventWireup="true" Inherits="DI.Chakra.Web.Triggers.UC.EditTriggerUC" CodeBehind="EditTriggerUC.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/Triggers/UC/ViewUserDetailUC.ascx" TagPrefix="uc1" TagName="UserDetailUC" %>
<script>

    function SetToday(Parameter) {
        var datepicker = $find("<%=EffectiveDatePicker.ClientID%>");
            var dt = new Date();
            datepicker.set_selectedDate(dt);
            datepicker.hidePopup();

            var datepicker = $find("<%=EffectiveDatePicker.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);
        datepicker.hidePopup();
    }

    function HideModalPopup() {
        $find("<%= UserDetailWindow.ClientID %>").close();
        window.setTimeout(function () {
            $("#UpdateProgressBar").hide();
        }, 1000);
        return false;
    }
    $(function () {
        $("body").on("click", ".uploadFile", function () {
            $(".Import").find(".ruFileInput").trigger("click");
        });

    });



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
    function setWindowWidth() {
     
    }
</script>
<style>
    .RadWindow_NewAndReply
{
    width:80% !important;
    left:15% !important;
}
</style>
<div class="content-wrapper">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <br />

                <div class="col-xs-6">
                    <div class="form-group">
                        <label class="col-xs-3 control-label">User Name</label>
                        <div class="col-xs-7">
                            <asp:Label ID="UserNameLabel" runat="server" CssClass="form-control"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-3 control-label">Event Type</label>
                        <div class="col-xs-7">
                            <asp:Label ID="EventTypeLabel" runat="server" CssClass="form-control"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-3 control-label">Created Date</label>
                        <div class="col-xs-7">
                            <asp:Label ID="CreatedDateLabel" runat="server" CssClass="form-control"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-3 control-label">Created By</label>
                        <div class="col-xs-7">
                            <asp:Label ID="CreatedByLabel" runat="server" CssClass="form-control"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-3 control-label">Effective Date</label>
                        <div class="col-xs-7">
                            <telerik:RadDatePicker runat="server" ID="EffectiveDatePicker"
                                onkeydown="handleDateTimePickerClickEvent(this,event)" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
                                DatePopupButton-Visible="true" Width="100%" CssClass="form-control">
                                <Calendar ID="Calendar2" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                    <FooterTemplate>
                                        <div style="width: 100%; text-align: center; background-color: Gray;">
                                            <asp:Button ID="EButton" runat="server" Width="100%" Text="Today" OnClientClick="SetToday('EButton')" />
                                        </div>
                                    </FooterTemplate>
                                </Calendar>
                                <DateInput ID="DateInput2" ReadOnly="false" runat="server" ToolTip="Date input For End Date"
                                    BorderStyle="NotSet" Label="">
                                </DateInput>
                            </telerik:RadDatePicker>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-3">Active</label>
                        <div class="col-xs-7">
                            <asp:CheckBox ID="ActiveCheckBox" runat="server" Checked="true" Text=" "></asp:CheckBox>
                        </div>
                    </div>
                    <%--<div class="form-group required" id="frmAttach" runat="server">
                        <asp:Label ID="lblAttachFile" AssociatedControlID="AsyncUploadAttach" runat="server" Text="Attach Files" CssClass="col-xs-3 control-label"></asp:Label>
                        <div class="col-xs-7  form-group">
                            <telerik:RadAsyncUpload ViewStateMode="Enabled" RenderMode="Lightweight" runat="server" ID="AsyncUploadAttach" OnClientValidationFailed="OnClientValidationFailed"
                                HideFileInput="true" MaxFileSize="10485760" MaxFileInputsCount="1" MultipleFileSelection="Disabled" AllowedFileExtensions=".jpeg,.jpg,.png,.doc,.docx,.xls,.xlsx,.csv,.pdf,.txt,.zip"
                                Width="100%" EnableInlineProgress="false" />

                            <span style="display: none" class="allowed-attachments">Allowed File Formats <span class="allowed-attachments-list">(<%= String.Join( ",", AsyncUploadAttach.AllowedFileExtensions ) %>)</span></span>
                        </div>
                    </div>--%>
                </div>

                <div class="col-xs-6">
                    <div class="form-group">
                        <label class="col-xs-3 control-label">Request Created</label>
                        <div class="col-xs-7">
                            <asp:Label ID="RequestCreatedLabel" runat="server" CssClass="form-control"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-3 control-label">Rejoining User</label>
                        <div class="col-xs-7">
                            <asp:Label ID="RejoiningUserLabel" runat="server" CssClass="form-control"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-3 control-label">Admin Check Required</label>
                        <div class="col-xs-7">
                            <asp:Label ID="AdminCheckRequiredLabel" runat="server" CssClass="form-control"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-3 control-label">Original Effective Date</label>
                        <div class="col-xs-7">
                             <asp:Label ID="orgEffectiveDate" runat="server" CssClass="form-control"></asp:Label>
                        </div>
                    </div>
                </div>

                <br />
                <br />
                <br />

                <div class="col-xs-12">

                    <div class="form-group">
                        <div class="col-xs-3 control-label">
                        </div>
                        <div class="buttons text-right nav-rectangle col-xs-4">
                            <asp:Button ID="ShowUserDetailButton" runat="server" CssClass="btn btn-default btn-sm" Text="Show User Detail" OnClick="ShowUserDetailButton_Click" />
                            <asp:Button ID="Button2" runat="server" CssClass="btn btn-default btn-sm cancel" Text="Cancel" PostBackUrl="~/Triggers/List.aspx" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<telerik:RadWindow runat="server" ID="UserDetailWindow" CssClass="RadWindow_NewAndReply" MinWidth="80%" Modal="true" Title="Detail"  AutoSize="True"
    RenderMode="Auto" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <uc1:UserDetailUC ID="UserDetailControl" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>
