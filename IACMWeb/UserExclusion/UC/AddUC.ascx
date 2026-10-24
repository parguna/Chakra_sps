<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddUC.ascx.cs" Inherits="DI.Chakra.Web.UserExclusion.UC.AddUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="UserSearchUC" %>

<script type="text/javascript">
    $(function () {
        $("body").on("click", ".uploadFile", function () {
            $(".Import").find(".ruFileInput").trigger("click");
        });

    });
    function SetToday(Parameter) {
        var datepicker = $find(Parameter == "VFButton" ? "<%=EffectiveDate.ClientID%>" : "<%=EffectiveDate.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);
        datepicker.hidePopup();
    }
    function OnDateSelected(sender, e) {
        var EffectiveDate = $find("<%= EffectiveDate.ClientID %>");
        var now = new Date();
        if (EffectiveDate != null) {
            EffectiveDate.set_minDate(new Date(now.getFullYear(), now.getMonth(), now.getDate()));
            now = e.get_newDate();
        }
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

</script>

<div class="content-wrapper">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                    <div class="form-group required">
                        <label class="col-xs-3 control-label">User Name</label>
                        <div class="col-xs-7">
                            <uc1:UserSearchUC runat="server" ID="UserSearchUC" />
                        </div>
                    </div>
                    <div class="form-group required">
                        <label class="col-xs-3 control-label ">Event Type</label>
                        <div class="col-xs-7">
                            <asp:DropDownList ID="ddlEventType" runat="server" CssClass="form-control selectpicker" AutoPostBack="True">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group required">
                        <label class="col-xs-3 control-label">Effective Till Date</label>
                        <div class="col-xs-7">
                            <telerik:RadDatePicker runat="server" ID="EffectiveDate" onkeydown="handleDateTimePickerClickEvent(this,event)" EnableTyping="false" Skin="IACMSkin" ShowPopupOnFocus="false" CssClass="form-control" DatePopupButton-Visible="true">
                                <ClientEvents OnDateSelected="OnDateSelected"></ClientEvents>
                                <Calendar ID="Calendar3" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                    <FooterTemplate>
                                        <div>
                                            <asp:Button ID="VFButton" runat="server" Width="100%" Text="Today" OnClientClick="SetToday('VFButton')" />
                                        </div>
                                    </FooterTemplate>
                                </Calendar>
                                <DateInput ID="effecttivedate" ReadOnly="false" runat="server" ToolTip="Date input For Effective Date"
                                    BorderStyle="NotSet" Label="">
                                </DateInput>
                            </telerik:RadDatePicker>
                        </div>

                    </div>
                    <div class="form-group required">
                        <label class="col-xs-3 control-label">Remarks</label>
                        <div class="col-xs-7">
                            <asp:TextBox runat="server" ID="txtComments" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group required">
                        <asp:Label ID="lblAttachFile" AssociatedControlID="AsyncUploadAttach" runat="server" Text="Attach Files" CssClass="col-xs-3 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <telerik:RadAsyncUpload ViewStateMode="Enabled" RenderMode="Lightweight" runat="server" ID="AsyncUploadAttach" OnClientValidationFailed="OnClientValidationFailed"
                                HideFileInput="true" MaxFileSize="10485760" MaxFileInputsCount="1" MultipleFileSelection="Disabled" AllowedFileExtensions=".jpeg,.jpg,.png,.doc,.docx,.xls,.xlsx,.csv,.pdf,.txt,.zip"
                                Width="100%" EnableInlineProgress="false" />

                            <span style="display: none" class="allowed-attachments">Allowed File Formats <span class="allowed-attachments-list">(<%= String.Join( ",", AsyncUploadAttach.AllowedFileExtensions ) %>)</span></span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
