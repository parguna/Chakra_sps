<%@ Page Language="C#" Title="Group Tasks" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" Inherits="DI.Chakra.Web.Task.GroupTasks" CodeBehind="GroupTasks.aspx.cs" %>


<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="Server">
    <script type="text/javascript">
        function OnClientEntryAddingHandler(sender, eventArgs) {

            if (sender != null && sender.get_entries().get_count() > 0) {
                eventArgs.set_cancel(true);
                alert("You can select only one entry");
            }

        }
        function showWindow() {
            var oWnd = $find("<%= assignUser.ClientID %>");
            oWnd.show();
        }

        function closeWindow() {
            var oWnd = $find("<%= assignUser.ClientID %>");
            oWnd.close();
        }
        function OnGroupClientRequesting(sender, eventArgs) {
        var context = eventArgs.get_context();
        context["hidden"] = "false";
    }
function SetToday(Parameter) {
        var datepicker = $find(Parameter == "FButton" ? "<%=DatePickerValidFrom.ClientID%>" : "<%=DatePickerValidTo.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);
        datepicker.hidePopup();
    }

    function OnDateSelected(sender, e) {
        var ValidFromPicker = $find("<%= DatePickerValidFrom.ClientID %>");
        var ValidToPicker = $find("<%= DatePickerValidTo.ClientID %>");

        var now = new Date();
        if (ValidFromPicker != null && ValidToPicker != null) {
            ValidToPicker.clear();
            now = e.get_newDate();
            if (now != null) {
                ValidToPicker.set_minDate(new Date(now.getFullYear(), now.getMonth(), now.getDate()));
            }
        }
    }
    </script>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="content-wrapper">
                <div class="form">
                    <div class="form-horizontal left">
                        <div class="row-fluid">
                            <div class="col-xs-6">
                                <div class="form-group">
                                    <asp:Label ID="lblTaskStatus" AssociatedControlID="ddlTaskStatus" runat="server" Text="Task Status" CssClass="col-xs-2 control-label"></asp:Label>
                                    <div class="col-xs-7">
                                        <asp:DropDownList ID="ddlTaskStatus" CssClass="form-control selectpicker" runat="server" data-width="100%" OnSelectedIndexChanged="ChangeGrid" AutoPostBack="true"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div class="col-xs-6">
                                <div class="form-group" runat="server" id="divRequestFor" visible="false">
                                    <asp:Label ID="lblRequestFor" Text="Request For" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="RequestForUserName">
                                    </asp:Label>
                                    <div class="col-xs-7">
                                        <telerik:RadAutoCompleteBox ID="RequestForUserName" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control"
                                            InputType="Token" AutoPostBack="true" OnClientRequesting="OnGroupClientRequesting" >
                                            <TextSettings SelectionMode="Single" />
                                            <WebServiceSettings Path="/Services/AutoCompleteDataService.svc" Method="GetUsers" />
                                        </telerik:RadAutoCompleteBox>
                                    </div>
                                </div>
                            </div>

                        </div>
                        <div class="row-fluid" >
                            <div class="col-xs-6">
                                &nbsp;
                            </div>
                            <div class="col-xs-6">
                                <div class="form-group">
                                    
                                </div>
                            </div>

                        </div>
                        <div class="row-fluid">
                            <div class="col-xs-12">
                                <div class="text-right">
                                    
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="form-horizontal left">
                        <div class="row-fluid">
                            <div class="col-xs-6">
                                 <div class="form-group">
                                     <label for="From" class="col-xs-2 control-label">Application</label>
                                    <div class="col-xs-7">
                                        <asp:DropDownList CssClass="form-control selectpicker" runat="server" ID="ddlEndpointSystemId" AutoPostBack="True" />
                                    </div>
                                </div>
                            </div>
                            <div class="col-xs-3">
                                <div class="form-group">
                                <label for="From" class="col-xs-4 control-label">From</label>
                                <div class="col-xs-8">
                                    <telerik:RadDatePicker runat="server" ID="DatePickerValidFrom" onkeydown="handleDateTimePickerClickEvent(this,event)" EnableTyping="false" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
                                        DatePopupButton-Visible="true" CssClass="form-control" AutoPostBack="true">
                                        <ClientEvents OnDateSelected="OnDateSelected"></ClientEvents>
                                        <Calendar ID="Calendar1" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                            <FooterTemplate>
                                                <div style="width: 100%; text-align: center; background-color: Gray;">
                                                    <asp:Button ID="EButton" runat="server" Width="100%" Text="Today" OnClientClick="SetToday('FButton')" AutoPostBack="false" />
                                                </div>
                                            </FooterTemplate>
                                        </Calendar>
                                        <DateInput ID="DateInput1" ReadOnly="false" runat="server" ToolTip="Date input For From Date"
                                            BorderStyle="NotSet" Label="" AutoPostBack="false">
                                        </DateInput>
                                    </telerik:RadDatePicker>
                                </div>
                                </div>

                            </div>
                            <div class="col-xs-3">
                                <div class="form-group">
                                <label for="To" class="col-xs-3 control-label">To</label>
                                <div class="col-xs-8">
                                    <telerik:RadDatePicker runat="server" onkeydown="handleDateTimePickerClickEvent(this,event)" ID="DatePickerValidTo" EnableTyping="false" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
                                        DatePopupButton-Visible="true" CssClass="form-control" AutoPostBack="false">
                                        <Calendar ID="Calendar2" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                            <FooterTemplate>
                                                <div style="width: 100%; text-align: center; background-color: Gray;">
                                                    <asp:Button ID="EButton" runat="server" Width="100%" Text="Today" OnClientClick="SetToday('TButton')" AutoPostBack="false" />
                                                </div>
                                            </FooterTemplate>
                                        </Calendar>
                                        <DateInput ID="DateInput2" ReadOnly="false" runat="server" ToolTip="Date input For End Date"
                                            BorderStyle="NotSet" Label="" AutoPostBack="false">
                                        </DateInput>
                                    </telerik:RadDatePicker>
                                </div>
                                </div>
                            </div>
                        </div>
                        <div class="row-fluid">
                            <div class="col-xs-4">
                                &nbsp;
                            </div>    
                            <div class="col-xs-4">
                                <asp:Button runat="server" ID="Filter" CausesValidation="true" CssClass="btn btn-default btn-sm" Text="Search" OnClick="Filter_Click" />
                                    <asp:Button runat="server" ID="ClearFilter" CausesValidation="false" CssClass="btn btn-default btn-sm" Text="Clear" OnClick="ClearFilter_Click" />
                            </div>    
                            <div class="col-xs-4">
                                <div class="text-right">
                                <asp:LinkButton ID="MyTasksLinkButton" runat="server" Text="Display My Tasks" PostBackUrl="~/Task/List.aspx"></asp:LinkButton>
                                    &nbsp;&nbsp;<asp:LinkButton ID="BulkAssignLinkButton" runat="server" Text="Bulk Assign Group Tasks" PostBackUrl="~/Task/BulkAssignGroupTasks.aspx"></asp:LinkButton>
                                 </div>   
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="topBuffer">
                <cc2:IACMRadGridControl ID="grdTask" runat="server" FilterMenu-CssClass="RadMenu_IACMFilterSkin" OnNeedDataSource="grdTask_NeedDataSource" DisableRowSelect="true" AllowCustomPaging="true"
                    OnItemCommand="grdTask_ItemCommand" OnItemDataBound="grdTask_ItemDataBound" OnSelectedIndexChanged="grdTask_SelectedIndexChanged" EnterToFilterEnable="true" PageSize="50">
                    <MasterTableView AutoGenerateColumns="False" ShowHeadersWhenNoRecords="true" CommandItemDisplay="Top" NoMasterRecordsText="No Records Available"
                        DataKeyNames="TaskId,AccessRequestID,Type,RequestType,IsCompleted,StatusType,TaskSource,AttestationCampaignId" Width="200%"  PageSize="50">
                        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                        <Columns>
                            <telerik:GridTemplateColumn DataField="TaskId" HeaderText="Task ID" FilterControlAltText="TaskId" SortExpression="TaskId" UniqueName="TaskId" HeaderStyle-Width="10%" FilterControlWidth="65%">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkTaskId" runat="server" Text='<%#Eval("TaskId") %>' CommandArgument='<%#Container.ItemIndex%>' OnCommand="lnkTaskId_Command" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="AccessRequestID" HeaderText="Request ID" SortExpression="AccessRequestID" UniqueName="AccessRequestID" HeaderStyle-Width="10%" FilterControlWidth="40%">
                                <ItemTemplate>
                                    <asp:Label ID="lblRequestId" runat="server" Text='<%#Eval("AccessRequestID") %>'
                                        Visible='<%#Convert.ToInt32(Eval("AccessRequestID")) > 0 %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridButtonColumn CommandArgument="Action" UniqueName="AssignTask" CommandName="ActionEdit" ButtonType="LinkButton" FilterControlAltText="Filter column" HeaderText="Assign Task" Text="Assign Task" Exportable="false" HeaderStyle-Width="170px" FilterControlWidth="40%">
                            </telerik:GridButtonColumn>
                            <telerik:GridBoundColumn DataField="EventName" FilterControlAltText="EventName" SortExpression="EventName" HeaderText="Application Name" AllowFiltering="true" UniqueName="EventName" HeaderStyle-Width="15%" FilterControlWidth="40%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="RequestedFor" FilterControlAltText="RequestedFor" SortExpression="RequestedFor" HeaderText="Request For" UniqueName="RequestedFor" HeaderStyle-Width="15%" FilterControlWidth="40%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="RequestedBy" FilterControlAltText="RequestedBy" UniqueName="RequestedBy" SortExpression="RequestedBy" HeaderText="Request By" HeaderStyle-Width="15%" FilterControlWidth="40%">
                            </telerik:GridBoundColumn>
                             <telerik:GridDateTimeColumn DataField="CompletionDate" DataFormatString="{0:dd-MM-yyyy}" FilterControlAltText="CompletionDate" SortExpression="CompletionDate" HeaderText="Completion Date" UniqueName="CompletionDate" FilterControlWidth="40%" HeaderStyle-Width="8%">
                            </telerik:GridDateTimeColumn>
                            <telerik:GridBoundColumn DataField="Type" FilterControlAltText="Type" SortExpression="Type" HeaderText="Task" UniqueName="Type" HeaderStyle-Width="15%" FilterControlWidth="40%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="TaskSource" FilterControlAltText="TaskSource" SortExpression="TaskSource" HeaderText="Task Source" UniqueName="TaskSource" FilterControlWidth="20%" HeaderStyle-Width="10%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="AccountType" FilterControlAltText="AccountType" HeaderText="Account Type" SortExpression="AccountType" UniqueName="AccountType" FilterControlWidth="40%" HeaderStyle-Width="10%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="AssignedByName" FilterControlAltText="AssignedByName" UniqueName="AssignedByName" HeaderText="Assigned By" AllowFiltering="true" HeaderStyle-Width="15%" FilterControlWidth="40%" Display="true">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="AssignedToName" FilterControlAltText="AssignedToName" UniqueName="AssignedToName" HeaderText="Assigned To" AllowFiltering="true" HeaderStyle-Width="15%" FilterControlWidth="40%" Display="true">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="RequestType" FilterControlAltText="RequestType" SortExpression="RequestType" HeaderText="Task Type" UniqueName="RequestType" FilterControlWidth="40%" HeaderStyle-Width="10%">
                            </telerik:GridBoundColumn>
                            
                            <telerik:GridTemplateColumn DataField="IsDelegated" HeaderText="Is Delegated" SortExpression="IsDelegated" UniqueName="IsDelegated" AllowFiltering="true" HeaderStyle-Width="10%" FilterControlWidth="80%" Display="true">
                                <ItemTemplate>
                                    <asp:Label ID="lblisdelegate" runat="server" Text='<%#(Convert.ToBoolean(Eval("IsDelegated"))? "Yes":"No") %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="DelegatedBy" FilterControlAltText="DelegatedBy" UniqueName="DelegatedBy" HeaderText="Delegated By" AllowFiltering="true" HeaderStyle-Width="15%" FilterControlWidth="40%" Display="true">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Entitlement" FilterControlAltText="Entitlement" UniqueName="Entitlement" HeaderText="Entitlement" HeaderStyle-Width="25%">
                            </telerik:GridBoundColumn>
                            <telerik:GridDateTimeColumn DataField="RequestedDate" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" FilterControlAltText="RequestedDate" SortExpression="RequestedDate" Display="true" HeaderText="Requested Date" UniqueName="RequestedDate" FilterControlWidth="10%" HeaderStyle-Width="10%">
                            </telerik:GridDateTimeColumn>
                            <telerik:GridDateTimeColumn DataField="CreatedDate" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" FilterControlAltText="CreatedDate" SortExpression="CreatedDate" HeaderText="Created Date" UniqueName="CreatedDate" HeaderStyle-Width="10%" FilterControlWidth="40%">
                            </telerik:GridDateTimeColumn>
                            <telerik:GridDateTimeColumn DataField="StartDate" DataFormatString="{0:dd-MM-yyyy}" FilterControlAltText="StartDate" SortExpression="StartDate" Display="true" HeaderText="Start Date"
                                UniqueName="StartDate" FilterControlWidth="10%" HeaderStyle-Width="10%">
                            </telerik:GridDateTimeColumn>
                            <telerik:GridDateTimeColumn DataField="EndDate" DataFormatString="{0:dd-MM-yyyy}" FilterControlAltText="EndDate" SortExpression="EndDate" HeaderText="End Date" UniqueName="EndDate" FilterControlWidth="40%" HeaderStyle-Width="10%">
                            </telerik:GridDateTimeColumn>
                            <telerik:GridBoundColumn HeaderText="Status" DataField="StatusType" FilterControlAltText="StatusType" UniqueName="StatusType" Display="true" HeaderStyle-Width="10%">
                            </telerik:GridBoundColumn>

                            <telerik:GridTemplateColumn Visible="false" Display="false" UniqueName="MoreDetails" HeaderText="More" AllowFiltering="false" HeaderStyle-Width="10%">
                                <ItemTemplate>
                                    <asp:LinkButton ID="MoreLinkButton" ForeColor="Blue" Text="" runat="server" CommandName="MoreDetails" />
                                </ItemTemplate>
                                <ItemStyle CssClass="action-info" HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </cc2:IACMRadGridControl>
            </div>
            <div id="hiddenDivMore" style="display: none; visibility: hidden;">
                <asp:Button runat="server" ID="hiddenButtonMore" />
            </div>
            <asp:ModalPopupExtender ID="MoreDetailModalPopupExtender" runat="server" BehaviorID="ModalPopupMoreBehaviorID"
                BackgroundCssClass="modalBackground" TargetControlID="hiddenButtonMore" PopupControlID="MoreDetailsPopupPanel" CancelControlID="btnCancelMore" />
            <asp:Panel ID="MoreDetailsPopupPanel" runat="server" CssClass="ModalWindow" Width="80%" Height="100%">
                <div class="modal di-modal fade" data-backdrop="static" data-keyboard="false" id="mymodalmore" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                    <div class="modal-dialog" style="width: 50%">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h3 class="modal-title" id="H1">More Details</h3>
                            </div>
                            <div class="modal-body">
                                <div class="form-horizontal left clearfix">
                                    <div class="row-fluid">
                                        <div class="col-xs-12">
                                            <div class="form-group">
                                                <asp:Panel ID="PopupPanel" runat="server">
                                                    <div class="col-xs-6">
                                                        <div class="form-group">
                                                            <label class="col-xs-5">Account Type:</label>
                                                            <div class="col-xs-5">
                                                                <asp:Label ID="LabelAccountType" runat="server" class="description"></asp:Label>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <label class="col-xs-5">Assigned By:</label>
                                                            <div class="col-xs-7">
                                                                <asp:Label ID="LabelAssignedBy" runat="server" class="description"></asp:Label>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <label class="col-xs-5">Request For:</label>
                                                            <div class="col-xs-7">
                                                                <asp:Label ID="LabelRequestFor" runat="server" class="description"></asp:Label>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <label class="col-xs-5">Entitlement:</label>
                                                            <div class="col-xs-5">
                                                                <asp:Label ID="LabelEntitlement" runat="server" class="description"></asp:Label>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <label class="col-xs-5">Start Date:</label>
                                                            <div class="col-xs-5">
                                                                <asp:Label ID="LabelStartDate" runat="server" class="description"></asp:Label>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-xs-6">
                                                        <div class="form-group">
                                                            <label class="col-xs-5">Task Type:</label>
                                                            <div class="col-xs-5">
                                                                <asp:Label ID="LabelTaskType" runat="server" class="description"></asp:Label>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <label class="col-xs-5">Assigned To:</label>
                                                            <div class="col-xs-7">
                                                                <asp:Label ID="LabelAssignedTo" runat="server" class="description"></asp:Label>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <label class="col-xs-5">Request By:</label>
                                                            <div class="col-xs-7">
                                                                <asp:Label ID="LabelRequestBy" runat="server" class="description"></asp:Label>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <label class="col-xs-5">Is Delegated:</label>
                                                            <div class="col-xs-5">
                                                                <asp:Label ID="LabelIsDelegated" runat="server" class="description"></asp:Label>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <label class="col-xs-5">End Date:</label>
                                                            <div class="col-xs-5">
                                                                <asp:Label ID="LabelEndDate" runat="server" class="description"></asp:Label>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <label class="col-xs-5">Delegated Comment:</label>
                                                            <div class="col-xs-5">
                                                                <asp:Label ID="DelegatedComment" runat="server" class="text-wrap description"></asp:Label>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </asp:Panel>
                                            </div>

                                        </div>
                                    </div>
                                </div>

                            </div>
                            <div class="modal-footer">
                                <asp:Button runat="server" CssClass="btn btn-primary bordered-button validation" ID="btnCancelMore" Text="Ok" OnClientClick="closeProgress();" />
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <div id="hiddenDiv" style="display: none; visibility: hidden;">
                <asp:Button runat="server" ID="hiddenButton" />
            </div>
            <telerik:RadWindow runat="server" ID="assignUser" Modal="true" Width="500px" Height="90%" Title="Assign To"
                RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
                <ContentTemplate>
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div class="modal-header">
                                <h3 class="modal-title">
                                    <h3 class="modal-title" id="myModalLabel">Lock Request For Action</h3>
                                </h3>
                            </div>
                            <div class="modal-body">
                                <div class="form-horizontal left row">
                                    <div class="row-fluid">
                                        <div class="col-xs-12">
                                            <div class="form-group">
                                                <div class="col-xs-12">
                                                    <label id="lbl" class="control-label col-xs-3" runat="server">Assign To:</label>
                                                    <div class="col-xs-9">
                                                        <div class="radio-inline">
                                                            <div id="selfAssignDiv" runat="server">
                                                                <asp:Label runat="server" ID="assignToMeLbl" AssociatedControlID="SelfRadioButton">
                                                                    <asp:RadioButton runat="server" ID="SelfRadioButton" AutoPostBack="true" GroupName="Map" OnCheckedChanged="UserSelectionChanged"></asp:RadioButton>
                                                                    <asp:Label ID="Label5" CssClass="lbl" runat="server" Text="Assign To Me"></asp:Label>
                                                                </asp:Label>
                                                            </div>


                                                            <asp:Label runat="server" ID="assignToOthersLbl" AssociatedControlID="OthersRadioButton">
                                                                <asp:RadioButton runat="server" ID="OthersRadioButton" AutoPostBack="true" GroupName="Map" OnCheckedChanged="UserSelectionChanged"></asp:RadioButton>
                                                                <asp:Label ID="Label6" CssClass="lbl" runat="server" Text="Assign To Others"></asp:Label>
                                                                <asp:HiddenField ID="TaskIdHiddenField" runat="server" />
                                                            </asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="form-group">
                                                <div class="col-xs-12">
                                                    <asp:Label ID="Label2" runat="server" Text="User Name" AssociatedControlID="UserNameAutoCompleteBox" class="col-xs-3 control-label"></asp:Label>
                                                    <div class="col-xs-9">
                                                        <telerik:RadAutoCompleteBox ID="UserNameAutoCompleteBox" EmptyMessage="Enter * to view full list" CssClass="form-control" ZIndex="10000000" runat="server"
                                                            InputType="Token" AutoPostBack="true" Enabled="false" OnEntryAdded="UserNameAutoCompleteBox_EntryAdded" OnClientEntryAdding="OnClientEntryAddingHandler">
                                                            <TextSettings SelectionMode="Single" />
                                                            <WebServiceSettings Method="GetUsersByGroup" Path="../Services/AutoCompleteDataService.svc" />
                                                        </telerik:RadAutoCompleteBox>
                                                        <asp:CustomValidator ID="UserNameAutoCompleteValidator" runat="server" ControlToValidate="UserNameAutoCompleteBox" ValidateEmptyText="true"
                                                            ValidationGroup="Save" ErrorMessage="Select a User." OnServerValidate="UserNameAutoCompleteValidator_ServerValidate"></asp:CustomValidator>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <asp:Button runat="server" CssClass="btn btn-primary transparent-button" ID="btnCancel" Text="Cancel" OnClick="btnCancel_Click" />
                                <asp:Button runat="server" CssClass="btn btn-primary bordered-button validation" ID="btnSave" OnClick="btnSave_Click" Text="Save" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </ContentTemplate>
            </telerik:RadWindow>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

