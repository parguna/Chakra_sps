<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaskListUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.Task.TaskListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="UserSearchUC" %>
<%@ Register Src="~/Task/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>

<script type="text/javascript">
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

    function showWindow() {
        var oWnd = $find("<%= modalPopup.ClientID %>");
        oWnd.show();
    }

    function closeWindow() {
        try {
            var oWnd = $find("<%= modalPopup.ClientID %>");
            oWnd.Close();
        } catch (e) {

        }
    }
     function Close() {
         try {
             var oWnd = $find("<%= modalPopup.ClientID %>");
             oWnd.Close();
         } catch (e) {

         }
    }
    function validateGrid() {
        var isValid = true;
        var masterTable = $(".rgMasterTable");
        if (masterTable != null) {
            var checkboxes = $("td input:checkbox:checked", masterTable);
            if (checkboxes.length == 0) {
                Alert("Please select at least 1 task item to delegate");
                isValid = false;
            }
        }
        return isValid;

    }

    function RowManipulation(sender, args) {
        var TaskGrid = $find("<%=TaskGrid.ClientID %>");
        var count = TaskGrid.get_masterTableView().get_selectedItems().length;
        window.parent.$("#Detail").find("input")
            .attr('disabled', count > 1 ? true : false);
    }
    function OnGroupClientRequestingAdminTask(sender, eventArgs) {
        var context = eventArgs.get_context();
        context["hidden"] = "false";
    }

</script>
<style>
    td input[type=checkbox] {
    position: relative;
    left: 13px;
    top: 5px;
}
</style>
<div class="content-wrapper">

    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">



</script>

    </telerik:RadCodeBlock>
    <asp:UpdatePanel ID="UpdateModalPopup" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
            <telerik:RadWindow ID="modalPopup" RenderMode="Lightweight" runat="server" Width="520px" Height="230px" Modal="true" OffsetElementID="main" VisibleTitlebar="false" VisibleStatusbar="false">
                <ContentTemplate>
                    <uc1:DetailUC runat="server" ID="UserDetail" OnAdd_SubmitButtonClicked="btnSubmit_Click" OnAdd_CancelButtonClicked="btnCancel_Click" />
                </ContentTemplate>
            </telerik:RadWindow>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="form">
        <div class="form-horizontal left clearfix">
            <div class="row-fluid">
                <div class="col-xs-4 form-group">
                    <label class="col-xs-3 control-label">Task Status</label>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlTaskStatus" CssClass="form-control selectpicker" runat="server" data-width="100%" OnSelectedIndexChanged="ddlTaskStatus_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                    </div>
                </div>
                <div class="col-xs-4 form-group" runat="server" id="divRequestFor" visible="false">
                    <label for="From" class="col-xs-3 control-label">Request For</label>
                    <div class="col-xs-8">
                        <telerik:RadAutoCompleteBox ID="RequestForUserName" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control"
                            InputType="Token" AutoPostBack="true" OnClientRequesting="OnGroupClientRequestingAdminTask">
                            <TextSettings SelectionMode="Single" />
                            <WebServiceSettings Path="../../../Services/AutoCompleteDataService.svc" Method="GetUsers" />
                        </telerik:RadAutoCompleteBox>
                    </div>
                </div>
                <%-- <div class="col-xs-4 form-group">
                    <asp:Label AssociatedControlID="ShowAllCheckBox" runat="server" CssClass="control-label">
                        <asp:CheckBox ID="ShowAllCheckBox" runat="server" AutoPostBack="True" OnCheckedChanged="ChangeGrid" />
                        <span class="lbl">Show All Tasks</span>
                    </asp:Label>
                </div>--%>
            </div>
        </div>
        <div class="form-horizontal left clearfix">
            <div class="row-fluid" id="dateFilter" runat="server">
                <div class="col-xs-4 form-group">
                    <label for="From" class="col-xs-3 control-label">Application</label>
                    <div class="col-xs-8">
                        <asp:DropDownList CssClass="form-control selectpicker" runat="server" ID="ddlEndpointSystemId" AutoPostBack="True" />
                    </div>
                </div>
                <div class="col-xs-4 form-group">
                    <label for="From" class="col-xs-3 control-label">From</label>
                    <div class="col-xs-8">
                        <telerik:RadDatePicker runat="server" ID="DatePickerValidFrom" onkeydown="handleDateTimePickerClickEvent(this,event)" OnSelectedDateChanged="DatePickerValidTo_SelectedDateChanged" EnableTyping="false" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
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
                <div class="col-xs-4 form-group">
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
            <div class="row-fluid">
                <div class="col-xs-4 form-group">
                    &nbsp;
                </div>
                <div class="col-xs-4">
                    <asp:Button runat="server" ID="Filter" CausesValidation="true" CssClass="btn btn-default btn-sm" Text="Search" OnClick="Filter_Click" />
                    <asp:Button runat="server" ID="ClearFilter" CausesValidation="false" CssClass="btn btn-default btn-sm" Text="Clear" OnClick="ClearFilter_Click" />
                </div>
                <div class="col-xs-4 text-right">
                    <asp:LinkButton ID="GroupTasksLinkButton" runat="server" CssClass="top-buffer" Text="Display Group Tasks" PostBackUrl="~/Task/GroupTasks.aspx"></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="topBuffer">
    <cc1:IACMRadGridControl DisableRowSelect="true" AllowPaging="true" EnableLinqExpressions="true" AllowCustomPaging="true" ID="TaskGrid" EnterToFilterEnable="true" FilterMenu-CssClass="RadMenu_IACMFilterSkin" runat="server" ShowHiddenColumnInContext="true" OnItemCommand="TaskGrid_ItemCommand" OnSelectedIndexChanged="TaskGrid_SelectedIndexChanged"
        OnNeedDataSource="TaskGrid_NeedDataSource" OnItemDataBound="TaskGrid_ItemDataBound" AllowSorting="true" AllowFilteringByColumn="true"  PageSize="50">
        <HeaderStyle Font-Bold="true" HorizontalAlign="Center" />
        <ClientSettings>
            <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true"></Scrolling>
        </ClientSettings>
        <MasterTableView AutoGenerateColumns="False" ShowHeadersWhenNoRecords="true" CommandItemDisplay="Top" NoMasterRecordsText="No Records Available"
            DataKeyNames="Id,TaskId,RequestType,StatusType,AccessRequestID,RequestedBy" TableLayout="Fixed" Width="190%"  PageSize="50"> 
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridTemplateColumn UniqueName="CheckTemp" HeaderText="CheckTemp" AllowFiltering="false" AllowSorting="false" Exportable="false" HeaderStyle-Width="60px" FilterControlWidth="65%">
                    <HeaderTemplate>
                        <asp:CheckBox ID="CheckBox2" AutoPostBack="true" Visible="true" runat="server" OnCheckedChanged="CheckBox2_CheckedChanged" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="ClientSelectColumn" runat="server" AutoPostBack="true" OnCheckedChanged="ClientSelectColumn_CheckedChanged" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="AccessRequestID" FilterControlAltText="AccessRequestID" SortExpression="AccessRequestID" HeaderText="Request ID" UniqueName="AccessRequestID" HeaderStyle-Width="100px" FilterControlWidth="65%">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn DataField="TaskId" HeaderText="Task ID" FilterControlAltText="TaskId" SortExpression="TaskId" UniqueName="TaskId" HeaderStyle-Width="95px" FilterControlWidth="65%">
                    <ItemTemplate>
                        <asp:Label ID="lblTaskId" runat="server" Text='<%#Eval("TaskId") %>' Visible='<%#Convert.ToInt64(Eval("TaskId")) > 0 %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="EventName" HeaderText="Application Name" FilterControlAltText="EventName" SortExpression="EventName" UniqueName="EventName" HeaderStyle-Width="15%" FilterControlWidth="40%">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEventName" runat="server" Text='<%#Eval("EventName") %>' CommandArgument='<%#Container.ItemIndex%>' OnCommand="lnkTaskId_Command" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="ActionUser" DataFormatString="<nobr>{0}</nobr>" FilterControlAltText="ActionUser" UniqueName="ActionUser" HeaderText="Action User/Group" FilterControlWidth="30%" HeaderStyle-Width="10%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="RequestedFor" DataFormatString="{0}" FilterControlAltText="RequestedFor" SortExpression="RequestedFor" HeaderText="Request For" UniqueName="RequestedFor" FilterControlWidth="40%" HeaderStyle-Width="12%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="RequestedBy" DataFormatString="{0}" FilterControlAltText="RequestedBy" UniqueName="RequestedBy" HeaderText="Request By" HeaderStyle-Width="10%">
                </telerik:GridBoundColumn>
                 <telerik:GridDateTimeColumn DataField="CompletionDate" DataFormatString="{0:dd-MM-yyyy}" FilterControlAltText="CompletionDate" SortExpression="CompletionDate" HeaderText="Completion Date" UniqueName="CompletionDate" FilterControlWidth="40%" HeaderStyle-Width="8%">
                </telerik:GridDateTimeColumn>
                <telerik:GridBoundColumn DataField="TaskSource" FilterControlAltText="TaskSource" SortExpression="TaskSource" HeaderText="Task Source" UniqueName="TaskSource" FilterControlWidth="20%" HeaderStyle-Width="9%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="AccountType" FilterControlAltText="AccountType" HeaderText="Account Type" SortExpression="AccountType" UniqueName="AccountType" FilterControlWidth="40%" HeaderStyle-Width="8%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Type" FilterControlAltText="Type" SortExpression="Type" HeaderText="Request Type" UniqueName="Type" FilterControlWidth="40%" HeaderStyle-Width="10%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataFormatString="{0}" DataField="RequestTypeName" FilterControlAltText="RequestTypeName" SortExpression="RequestTypeName" HeaderText="Task Type" UniqueName="RequestTypeName" FilterControlWidth="40%" HeaderStyle-Width="7%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="IsDelegatedByUser" HeaderText="Is Delegated" SortExpression="IsDelegatedByUser" UniqueName="IsDelegate" AllowFiltering="true" HeaderStyle-Width="7%" FilterControlWidth="80%" Display="true">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Entitlement" FilterControlAltText="Entitlement" UniqueName="Entitlement" HeaderText="Entitlement" HeaderStyle-Width="25%">
                </telerik:GridBoundColumn>
                <telerik:GridDateTimeColumn DataField="RequestedDate" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" FilterControlAltText="RequestedDate" SortExpression="RequestedDate" Display="true" HeaderText="Requested Date" UniqueName="RequestedDate" FilterControlWidth="10%" HeaderStyle-Width="8%">
                </telerik:GridDateTimeColumn>
                <telerik:GridDateTimeColumn DataField="CreatedDate" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" FilterControlAltText="CreatedDate" SortExpression="CreatedDate" Display="true" HeaderText="Created Date"
                    UniqueName="CreatedDate" FilterControlWidth="10%" HeaderStyle-Width="8%">
                </telerik:GridDateTimeColumn>
                <telerik:GridDateTimeColumn DataField="StartDate" DataFormatString="{0:dd-MM-yyyy}" FilterControlAltText="StartDate" SortExpression="StartDate" Display="true" HeaderText="Start Date"
                    UniqueName="StartDate" FilterControlWidth="10%" HeaderStyle-Width="8%">
                </telerik:GridDateTimeColumn>
                <telerik:GridDateTimeColumn DataField="EndDate" DataFormatString="{0:dd-MM-yyyy}" FilterControlAltText="EndDate" SortExpression="EndDate" HeaderText="End Date" UniqueName="EndDate" FilterControlWidth="40%" HeaderStyle-Width="8%">
                </telerik:GridDateTimeColumn>
                
                <telerik:GridBoundColumn DataField="StatusType" FilterControlAltText="StatusType" HeaderText="Status" AllowFiltering="false" UniqueName="StatusType" HeaderStyle-Width="10%">
                </telerik:GridBoundColumn>
                <%--<telerik:GridBoundColumn DataField="RequestForDesignation" DataFormatString="<nobr>{0}</nobr>" FilterControlAltText="RequestForDesignation" SortExpression="RequestForDesignation" HeaderText="Designation" UniqueName="RequestForDesignation" FilterControlWidth="40%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="RequestForDept" DataFormatString="<nobr>{0}</nobr>" FilterControlAltText="RequestForDept" SortExpression="RequestForDept" HeaderText="Department" UniqueName="RequestForDept" FilterControlWidth="40%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="RequestForSupDisplayName" DataFormatString="<nobr>{0}</nobr>" FilterControlAltText="RequestForSupDisplayName" SortExpression="RequestForSupDisplayName" HeaderText="Supervisor" UniqueName="RequestForSupDisplayName" FilterControlWidth="40%">
                </telerik:GridBoundColumn>--%>
                <telerik:GridBoundColumn DataField="DelegatedBy" FilterControlAltText="DelegatedBy" UniqueName="DelegatedBy" HeaderText="Routed From" FilterControlWidth="40%" Display="false" Visible="false" DataFormatString="<nobr>{0}</nobr>" HeaderStyle-Width="10%">
                </telerik:GridBoundColumn>

                <telerik:GridBoundColumn DataField="Approver" DataFormatString="<nobr>{0}</nobr>" FilterControlAltText="Approver" UniqueName="Approver" Display="false" Visible="false" HeaderText="Approver Type" HeaderStyle-Width="10%">
                </telerik:GridBoundColumn>

                <telerik:GridBoundColumn DataField="ActionUserADID" DataFormatString="<nobr>{0}</nobr>" FilterControlAltText="ActionUserADID" UniqueName="ActionUserADID" HeaderText="Action User ADID" FilterControlWidth="40%" Display="false" Visible="false" HeaderStyle-Width="10%">
                </telerik:GridBoundColumn>

                <telerik:GridBoundColumn DataField="IsGroup" FilterControlAltText="IsGroup" UniqueName="IsGroup" HeaderText="Is Group" FilterControlWidth="40%" Display="false" Visible="false" HeaderStyle-Width="10%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="GroupName" FilterControlAltText="GroupName" UniqueName="GroupName" HeaderText="Assigned To Group" FilterControlWidth="40%" Display="false" Visible="false" HeaderStyle-Width="10%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="AssignedToName" FilterControlAltText="AssignedToName" UniqueName="AssignedToName" HeaderText="Assigned To" FilterControlWidth="40%" HeaderStyle-Width="10%" Display="false" Visible="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="AssignedToADID" FilterControlAltText="AssignedToADID" UniqueName="AssignedToADID" HeaderText="Assigned To ADID" FilterControlWidth="40%" Display="false" Visible="false" HeaderStyle-Width="10%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="AssignedByName" FilterControlAltText="AssignedByName" UniqueName="AssignedByName" HeaderText="Assigned By" FilterControlWidth="40%" Display="false" Visible="false" HeaderStyle-Width="10%">
                </telerik:GridBoundColumn>



                <%--<telerik:GridTemplateColumn DataField="IsDelegated" HeaderText="Is Delegated" SortExpression="IsDelegated" UniqueName="IsDelegated">
                                <ItemTemplate>
                                    <asp:Label ID="lblisdelegate" runat="server" Text='<%#(Convert.ToBoolean(Eval("IsDelegated"))? "Yes":"No") %>' />
                                </ItemTemplate>
                </telerik:GridTemplateColumn>--%>

                <telerik:GridTemplateColumn UniqueName="MoreDetails" HeaderText="More" AllowFiltering="false" Display="false" Visible="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="MoreLinkButton" ForeColor="Blue" Text="" runat="server" CommandName="MoreDetails" />
                    </ItemTemplate>
                    <ItemStyle CssClass="action-info" HorizontalAlign="Center" />
                </telerik:GridTemplateColumn>

            </Columns>
        </MasterTableView>
    </cc1:IACMRadGridControl>
</div>
<div id="hiddenDivMore" style="display: none; visibility: hidden;">
    <asp:Button runat="server" ID="hiddenButtonMore" />
</div>
<telerik:RadWindow runat="server" ID="Tasklist" Modal="true" Width="500px" Height="100%" Title="Cart Items"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title" id="H1">More Details</h3>
                </div>
                <div class="modal-body">
                    <div class="form-horizontal left clearfix">
                        <div class="row-fluid">
                            <div class="col-xs-12">
                                <asp:Panel ID="PopupPanel" runat="server">
                                    <div class="form-group">
                                        <label class="col-xs-4">Request For:</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="LabelRequestFor" runat="server" class="text-wrap description"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-xs-4">Request By:</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="LabelRequestBy" runat="server" class="text-wrap description"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-xs-4">Account Type:</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="LabelAccountType" runat="server" class="text-wrap description"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-xs-4">Task Type:</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="LabelTaskType" runat="server" class="text-wrap description "></asp:Label>
                                        </div>
                                    </div>
                                    <%--<div class="form-group">
                                        <label class="col-xs-4">Entitlement:</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="LabelEntitlement" runat="server" class="text-wrap description"></asp:Label>
                                        </div>
                                    </div>--%>
                                    <div class="form-group">
                                        <label class="col-xs-4">Is Delegated:</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="LabelIsDelegated" runat="server" class="text-wrap description"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-xs-4">StartDate:</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="LabelStartDate" runat="server" class="text-wrap description"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-xs-4">End Date:</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="LabelEndDate" runat="server" class="text-wrap description"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-xs-4">Request Reason:</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="RequestReason" runat="server" class="text-wrap description"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-xs-4">Delegated By:</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="DelegatedBy" runat="server" class="text-wrap description"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-xs-4">Delegated Comment:</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="DelegatedComment" runat="server" class="text-wrap description"></asp:Label>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label class="col-xs-4">Status:</label>
                                        <div class="col-xs-8">
                                            <asp:Label ID="LBLstatus" runat="server" class="text-wrap description"></asp:Label>
                                        </div>
                                    </div>
                                </asp:Panel>
                            </div>
                        </div>
                    </div>

                </div>

                <div class="modal-footer">
                    <asp:Button ID="btnOk" runat="server" data-dismiss="modal" CssClass="btn btn-primary bordered-button" Text="OK" OnClick="btnOk_Click" />
                </div>

            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>
