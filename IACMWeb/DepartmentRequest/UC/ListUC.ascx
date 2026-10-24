<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.DepartmentRequest.UC.ListUC" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Src="~/DepartmentRequest/UC/InfoUC.ascx" TagPrefix="uc1" TagName="InfoUC" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<cc1:IACMRadGridControl ID="grdRequest" runat="server" OnNeedDataSource="grdRequest_NeedDataSource" OnSelectedIndexChanged="grdRequest_SelectedIndexChanged" OnItemCommand="grdRequest_ItemCommand">
    <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="Top" DataKeyNames="AdHocAccessRequestId,AccessRequestId,ProgressStatus" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn HeaderText="Request ID" DataField="AccessRequestID" FilterControlAltText="AccessRequestID" SortExpression="AccessRequestID" UniqueName="AccessRequestID" HeaderStyle-Width="20%" FilterControlWidth="77%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderText="Endpoint Name" DataField="Name" FilterControlAltText="Name" SortExpression="Name" UniqueName="Name" HeaderStyle-Width="25%" FilterControlWidth="81%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderText="Request Type" DataField="RequestType" FilterControlAltText="RequestType" SortExpression="RequestType" UniqueName="RequestType" HeaderStyle-Width="23%" FilterControlWidth="79%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderText="Request By Name" DataField="RequesterName" FilterControlAltText="RequesterName" SortExpression="RequesterName" UniqueName="RequesterName" HeaderStyle-Width="23%" FilterControlWidth="79%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderText="Request By ADID" DataField="RequesterUserName" FilterControlAltText="RequesterUserName" SortExpression="RequesterUserName" UniqueName="RequesterUserName" HeaderStyle-Width="23%" FilterControlWidth="79%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderText="Request For Name" DataField="RequesteeName" FilterControlAltText="RequesteeName" SortExpression="RequesteeName" UniqueName="RequesteeName" HeaderStyle-Width="23%" FilterControlWidth="79%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderText="Request For ADID" DataField="RequesteeUserName" FilterControlAltText="RequesteeUserName" SortExpression="RequesteeUserName" UniqueName="RequesteeUserName" HeaderStyle-Width="23%" FilterControlWidth="79%">
            </telerik:GridBoundColumn>
            <telerik:GridDateTimeColumn HeaderText="Request Date" DataField="RequestedDate" DataType="System.DateTime" AllowFiltering="true" SortExpression="RequestedDate" AllowSorting="true" HeaderStyle-Width="22%">
            </telerik:GridDateTimeColumn>
            <telerik:GridBoundColumn HeaderText="ProgressStatus" DataType="System.String" DataField="ProgressStatusString" SortExpression="ProgressStatusString" AllowSorting="true" HeaderStyle-Width="22%" FilterControlWidth="75%">
            </telerik:GridBoundColumn>
            <telerik:GridTemplateColumn AllowFiltering="false" AllowSorting="false" UniqueName="TemplateInfoColumn" ItemStyle-CssClass="action-info" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="10%" HeaderText="Detail / Info">
                <ItemTemplate>
                    <asp:ImageButton runat="server" CommandName="ShowInfo"></asp:ImageButton>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
        </Columns>
    </MasterTableView>
</cc1:IACMRadGridControl>

<telerik:RadWindow runat="server" ID="RequestInfo" Modal="true" Width="500px" Height="100%" Title="Request Info"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title">Request Info</h3>
                </div>
                <div class="modal-body">
                    <uc1:InfoUC runat="server" ID="InfoUC" />
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnCancel" class="btn btn-primary bordered-button" Text="OK" OnClick="btnCancel_Click" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>


<telerik:RadWindow runat="server" ID="ExtendedAction" Modal="true" Title="Request Extend" Width="500px" Height="100%"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title">Request Extend</h3>
                </div>
                <div class="modal-body">
                    <div class="form-horizontal left clearfix">
                        <div class="row-fluid">
                            <div class="col-xs-12">
                                <div class="form-group required">
                                    <label class="col-xs-4 control-label"><%= T("App.ACR.ValidTo") %></label>
                                    <div class="col-xs-7">
                                        <telerik:RadDatePicker runat="server" ID="ValidToPicker" EnableTyping="false" Skin="IACMSkin" ShowPopupOnFocus="false" CssClass="form-control"
                                            DatePopupButton-Visible="true">
                                            <Calendar ID="Calendar1" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                                <FooterTemplate>
                                                    <div>
                                                        <asp:Button ID="VTButton" runat="server" Width="100%" Text="Today" OnClientClick="SetToday('VTButton')" />
                                                    </div>
                                                </FooterTemplate>
                                            </Calendar>
                                            <DateInput ID="DateInput1" ReadOnly="false" runat="server" ToolTip="Date input For Valid To"
                                                BorderStyle="NotSet" Label="">
                                            </DateInput>
                                        </telerik:RadDatePicker>
                                    </div>
                               </div>
                                <div class="form-group required">
                                    <asp:Label ID="Comments" AssociatedControlID="Comments" runat="server" Text="Comments" class="col-xs-4 control-label"></asp:Label>
                                    <div class="col-xs-7">
                                        <asp:TextBox runat="server" ID="txtComments" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group required">
                                    <asp:Label ID="lblAttachFile" AssociatedControlID="AsyncUploadAttach" runat="server" Text="Attach Files" CssClass="col-xs-4 control-label"></asp:Label>
                                    <div class="col-xs-7">
                                        <telerik:RadAsyncUpload ViewStateMode="Enabled" RenderMode="Lightweight" runat="server" ID="AsyncUploadAttach" OnClientValidationFailed="OnClientValidationFailed"
                                            HideFileInput="true" MaxFileSize="10485760" AllowedFileExtensions=".jpeg,.jpg,.png,.doc,.docx,.xls,.xlsx,.csv,.pdf,.txt,.zip"
                                            Width="100%" EnableInlineProgress="false" MaxFileInputsCount="1" />

                                        <span style="display: none" class="allowed-attachments">Allowed File Formats <span class="allowed-attachments-list">(<%= String.Join( ",", AsyncUploadAttach.AllowedFileExtensions ) %>)</span></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <asp:Button runat="server" ID="Cancel" class="btn btn-primary bordered-button" Text="Cancel" OnClick="Cancel_Click" />
                    <asp:Button runat="server" ID="Save" class="btn btn-primary bordered-button" Text="Update" OnClick="Save_Click"/>

                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>

<div class="buttons text-right nav-rectangle">
    <asp:Button runat="server" ID="RefreshButton" CssClass="btn btn-default btn-sm" Text="Refresh" OnClick="RefreshButton_Click"></asp:Button>
    <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClientClick="Redirect()"></asp:Button>
</div>


<script type="text/javascript">
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
    function SetToday(Parameter) {
        var datepicker = $find(Parameter == "<%=ValidToPicker.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);
        datepicker.hidePopup();
    }

    
    function OnDateSelected(sender, e) {
        
        var ValidToPicker = $find("<%= ValidToPicker.ClientID %>");
        var now = new Date();
        if (ValidToPicker != null) {
            ValidToPicker.clear();
           // ValidFromPicker.set_minDate(new Date(now.getFullYear(), now.getMonth(), now.getDate()));
            now = e.get_newDate();
            if (now != null) {
                ValidToPicker.set_minDate(new Date(now.getFullYear(), now.getMonth(), now.getDate()));
            }
        }
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