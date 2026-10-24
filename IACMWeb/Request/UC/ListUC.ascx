<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Request.UC.ListUC" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Src="~/Request/UC/InfoUC.ascx" TagPrefix="uc1" TagName="InfoUC" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<%--<style type="text/css">
    .RadGrid_IACMSkin .rgDataDiv {
       overflow-x: scroll!important;
    }
</style>--%>
<div id="divEmpEventChkBox" runat="server" visible="false">
    <asp:CheckBox runat="server" Text="Show event triggerred access requests only" AutoPostBack="True" ID="chkEmpEvent" OnCheckedChanged="chkEmpEvent_CheckedChanged"></asp:CheckBox>
</div>
<cc1:IACMRadGridControl DisableRowSelect="true" ID="grdRequest" runat="server" OnNeedDataSource="grdRequest_NeedDataSource" FilterMenu-CssClass="RadMenu_IACMFilterSkin" OnSelectedIndexChanged="grdRequest_SelectedIndexChanged"
    OnItemCommand="grdRequest_ItemCommand" EnterToFilterEnable="true" AllowSorting="true" PageSize="50">
    <HeaderStyle Font-Bold="true" HorizontalAlign="Center" />
    <ClientSettings>
        <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true"></Scrolling>
    </ClientSettings>
    <MasterTableView TableLayout="Fixed" AutoGenerateColumns="False"  PageSize="50" CommandItemDisplay="Top" Width="150%" DataKeyNames="Id,AdHocAccessRequestId,AccessRequestId,ProgressStatus,RequestSource,BulkRequestId" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridTemplateColumn HeaderText="Request ID" DataField="AccessRequestID" FilterControlAltText="AccessRequestID" SortExpression="AccessRequestID" UniqueName="AccessRequestID" HeaderStyle-Width="8%" >
                <ItemTemplate>
                    <asp:LinkButton ID="lnkTaskId" runat="server" Text='<%#Eval("AccessRequestID") %>' CommandArgument='<%#Container.ItemIndex%>' OnCommand="lnkTaskId_Command" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn HeaderText="Progress Status" DataType="System.String" DataField="ProgressStatusString" SortExpression="ProgressStatusString" HeaderStyle-Width="10%">
            </telerik:GridBoundColumn>
            <%--<telerik:GridBoundColumn HeaderText="Status" DataType="System.String" DataField="RequestStatusString" SortExpression="RequestStatusString" AllowSorting="true" HeaderStyle-Width="10%" AllowFiltering="false" >
            </telerik:GridBoundColumn>--%>
             <telerik:GridTemplateColumn DataField="Name" HeaderText="Re-Submit" HeaderStyle-Width="10%" UniqueName="TemplateInfoColumn" ItemStyle-CssClass="action-info" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:LinkButton ID="Resubmit" runat="server" ForeColor="Blue" Text="Resubmit" Visible='<%# ((Convert.ToString(Eval("Status")) == "Rejected" || Convert.ToString(Eval("Status")) == "Withdrawn") && (Convert.ToBoolean(Eval("IsAdhocRequest"))||Convert.ToBoolean(Eval("IsBulkRequest")))) ? true : false %>'
                        ToolTip='Resubmit' CommandArgument='<%#Eval("AdHocAccessRequestId") %>' CommandName="Resubmit" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn HeaderText="Application Name" DataField="Name" FilterControlAltText="Name" SortExpression="Name" UniqueName="Name" HeaderStyle-Width="15%" >
            </telerik:GridBoundColumn>
            <%--<telerik:GridTemplateColumn HeaderText="Application Name" DataField="Name" FilterControlAltText="Name" SortExpression="Name" UniqueName="Name" HeaderStyle-Width="250px" >
                <ItemTemplate>
                    <asp:LinkButton ID="ViewTaskButton" runat="server" ForeColor="Blue" Text='<%#Eval("Name") %>'
                        ToolTip='Edit' CommandArgument='<%#Eval("AdHocAccessRequestId") %>' CommandName='<%#Eval("IsAdhocRequest") %>' />
                </ItemTemplate>
            </telerik:GridTemplateColumn>--%>
            <telerik:GridBoundColumn DataField="RequestSource" FilterControlAltText="RequestSource" SortExpression="RequestSource" HeaderText="Request Source" UniqueName="RequestSource" HeaderStyle-Width="10%" >
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="AccountTypeString" FilterControlAltText="AccountTypeString" HeaderText="Account Type" SortExpression="AccountTypeString" UniqueName="AccountTypeString"  HeaderStyle-Width="10%" >
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderText="Request Type" DataField="RequestType" FilterControlAltText="RequestType" SortExpression="RequestType" UniqueName="RequestType" HeaderStyle-Width="10%" >
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderText="Request For" DataField="RequesteeName" FilterControlAltText="RequesteeName" SortExpression="RequesteeName" UniqueName="RequesteeName" HeaderStyle-Width="15%" >
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="RequesterName" FilterControlAltText="RequesterName" SortExpression="RequesterName" HeaderText="Request By" UniqueName="RequesterName" HeaderStyle-Width="15%" >
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="EntitlementString" FilterControlAltText="EntitlementString" SortExpression="EntitlementString" HeaderText="Entitlement" UniqueName="EntitlementString" HeaderStyle-Width="25%" >
            </telerik:GridBoundColumn>
            <telerik:GridDateTimeColumn HeaderText="Request Date" DataField="RequestedDate" DataType="System.DateTime" SortExpression="RequestedDate" HeaderStyle-Width="10%" >
            </telerik:GridDateTimeColumn>
            <telerik:GridDateTimeColumn HeaderText="Start Date" DataField="ValidFrom" DataType="System.DateTime" SortExpression="ValidFrom" HeaderStyle-Width="10%" >
            </telerik:GridDateTimeColumn>
            <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" DataField="ValidTo" DataFormatString="{0:dd-MM-yyyy}" FilterControlAltText="ValidTo" SortExpression="ValidTo" HeaderText="End Date" UniqueName="ValidTo" HeaderStyle-Width="10%">
            </telerik:GridDateTimeColumn>
            <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" DataField="ExecutionDate" HeaderText="Execution Time" SortExpression="ExecutionDate" FilterControlAltText="ExecutionDate" UniqueName="ExecutionDate" DataFormatString="{0:dd-MM-yyyy HH:mm:ss}" HeaderStyle-Width="10%">
            </telerik:GridDateTimeColumn>

           
            <%-- <telerik:GridTemplateColumn AllowFiltering="false" AllowSorting="false" UniqueName="TemplateInfoColumn" ItemStyle-CssClass="action-info" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="10%" HeaderText="Detail / Info">
                <ItemTemplate>
                    <asp:ImageButton runat="server" CommandName="ShowInfo"></asp:ImageButton>
                </ItemTemplate>
            </telerik:GridTemplateColumn>--%>
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
                    <asp:Button runat="server" ID="Save" class="btn btn-primary bordered-button" Text="Update" OnClick="Save_Click" />

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
