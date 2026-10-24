<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Prov.Request.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Src="~/Request/UC/InfoUC.ascx" TagPrefix="uc1" TagName="InfoUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<script type="text/javascript">
    function SelectAll(invoker) {
        var inputElements = document.getElementsByTagName('input');
        for (var i = 0; i < inputElements.length; i++) {
            var myElement = inputElements[i];
            if (myElement.type === "checkbox") {
                myElement.checked = invoker.checked;
            }
        }
    }

    function RowManipulation(sender, args) {
        var TaskGrid = $find("<%=AccessRequestGrid.ClientID %>");
        var count = TaskGrid.get_masterTableView().get_selectedItems().length;
        window.parent.$("#Detail").find("input")
            .attr('disabled', count > 1 ? true : false);
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
    function OnGroupClientRequestingAdminTask(sender, eventArgs) {
        var context = eventArgs.get_context();
        context["hidden"] = "false";
    }
</script>

<div class="content-wrapper">
    <div class="form">
        <div class="form-horizontal left clearfix">
            <div class="row-fluid">
                <div class=" col-xs-4 form-group">
                    <asp:Label ID="Label2" runat="server" CssClass="col-xs-4 control-label" AssociatedControlID="ProgressStatus">Progress Status</asp:Label>
                    <div class="col-xs-7">
                        <asp:DropDownList ID="ProgressStatus" runat="server" CssClass="form-control selectpicker" data-width="100%" AppendDataBoundItems="true" AutoPostBack="True"
                            OnSelectedIndexChanged="ProgressStatus_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="col-xs-4 form-group" runat="server" id="divRequestFor">
                    <label for="From" class="col-xs-3 control-label">Request For</label>
                    <div class="col-xs-8">
                        <telerik:RadAutoCompleteBox ID="RequestForUserName" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control"
                            InputType="Token" AutoPostBack="true" OnClientRequesting="OnGroupClientRequestingAdminTask">
                            <TextSettings SelectionMode="Single" />
                            <WebServiceSettings Path="../../../Services/AutoCompleteDataService.svc" Method="GetUsers" />
                        </telerik:RadAutoCompleteBox>
                    </div>
                </div>
            </div>
        </div>
        <div class="form-horizontal left clearfix">
            <div class="row-fluid" id="dateFilter" runat="server">
                <div class="col-xs-4 form-group">
                    <label for="From" class="col-xs-4 control-label">Endpoint</label>
                    <div class="col-xs-7">
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
            </div>
        </div>
    </div>
</div>
<cc1:IACMRadGridControl ID="AccessRequestGrid" runat="server" AllowPaging="true" PageSize="50" DisableRowSelect="true" OnNeedDataSource="AccessRequest_NeedDataSource" OnSelectedIndexChanged="AccessRequestGrid_SelectedIndexChanged"
    OnItemCommand="AccessRequestGrid_ItemCommand" OnItemDataBound="AccessRequestGrid_ItemDataBound">
    <ClientSettings EnableRowHoverStyle="True">
        <Resizing AllowColumnResize="true" />
        <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true"></Scrolling>
    </ClientSettings>
    <MasterTableView DataKeyNames="Id,ProgressStatus,AdHocAccessRequestId,RequestSource,ProgressStatus,AccessRequestID" Width="150%"  PageSize="50">
        <Columns>
            <%--<telerik:GridClientSelectColumn UniqueName="ClientSelectColumn">
            </telerik:GridClientSelectColumn>--%>
            <telerik:GridTemplateColumn UniqueName="CheckTemp" Display="false" Visible="false" HeaderText="CheckTemp" AllowFiltering="false" AllowSorting="false" Exportable="false" HeaderStyle-Width="60px" FilterControlWidth="65%">
                <HeaderTemplate>
                    <asp:CheckBox ID="CheckBox2" AutoPostBack="true" Visible="true" runat="server" OnCheckedChanged="CheckBox2_CheckedChanged" Style="position: absolute" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="ClientSelectColumn" runat="server" Visible="false" Enabled="false" AutoPostBack="true" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="Request ID" DataField="AccessRequestID" FilterControlAltText="AccessRequestID" SortExpression="AccessRequestID" UniqueName="AccessRequestID" HeaderStyle-Width="100px" FilterControlWidth="75%">
                <ItemTemplate>
                    <asp:LinkButton ID="lnkTaskId" runat="server" Text='<%#Eval("AccessRequestID") %>' CommandArgument='<%#Container.ItemIndex%>' OnCommand="lnkTaskId_Command" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn HeaderText="Trigger ID" DataField="TriggerId" FilterControlAltText="TriggerId" SortExpression="TriggerId" UniqueName="TriggerId" HeaderStyle-Width="80px" FilterControlWidth="75%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderText="Application Name" DataField="Name" FilterControlAltText="Name" SortExpression="Name" UniqueName="Name" HeaderStyle-Width="250px" FilterControlWidth="75%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderText="Request Type" DataField="RequestType" FilterControlAltText="RequestType" SortExpression="RequestType" UniqueName="RequestType" HeaderStyle-Width="150px" FilterControlWidth="79%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="RequesteeUserName" FilterControlAltText="RequesteeUserName" HeaderText="Request For" SortExpression="RequesteeUserName" UniqueName="RequesteeUserName" HeaderStyle-Width="200px" FilterControlWidth="70%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="RequesterUserName" DataFormatString="{0}" FilterControlAltText="RequesterUserName" UniqueName="RequesterUserName" HeaderText="Request By" HeaderStyle-Width="200px">
            </telerik:GridBoundColumn>
            <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" DataField="ExecutionDate" HeaderText="Execution Time" SortExpression="ExecutionDate" FilterControlAltText="ExecutionDate" UniqueName="ExecutionDate" DataFormatString="{0:dd-MM-yyyy HH:mm:ss}" HeaderStyle-Width="140px">
            </telerik:GridDateTimeColumn>
            <telerik:GridBoundColumn HeaderText="Status" DataType="System.String" DataField="ProgressStatusString" SortExpression="ProgressStatusString" AllowSorting="true" HeaderStyle-Width="160px" FilterControlWidth="75%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="RequestSource" FilterControlAltText="RequestSource" HeaderText="Request Source" SortExpression="RequestSource" UniqueName="RequestSource" HeaderStyle-Width="150px" FilterControlWidth="70%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="AccountTypeString" FilterControlAltText="AccountTypeString" HeaderText="Account Type" SortExpression="AccountTypeString" UniqueName="AccountTypeString" FilterControlWidth="70%" HeaderStyle-Width="150px" AllowSorting="true">
            </telerik:GridBoundColumn>
            
            <%--<telerik:GridBoundColumn DataField="RequestedFor" FilterControlAltText="RequestedFor" SortExpression="RequestedFor" HeaderText="Requested For" UniqueName="RequestedFor" HeaderStyle-Width="15%" FilterControlWidth="40%">
            </telerik:GridBoundColumn>--%>
            <%--<telerik:GridBoundColumn DataField="RequestedBy" FilterControlAltText="RequestedBy" UniqueName="RequestedBy" HeaderStyle-Width="15%" HeaderText="Requested By">
            </telerik:GridBoundColumn>--%>
            <telerik:GridBoundColumn DataField="EntitlementString" FilterControlAltText="EntitlementString" SortExpression="EntitlementString" HeaderText="Entitlement" UniqueName="EntitlementString" HeaderStyle-Width="200px" FilterControlWidth="79%" AllowSorting="true">
            </telerik:GridBoundColumn>
            <telerik:GridDateTimeColumn HeaderText="Request Date" DataField="RequestedDate" DataType="System.DateTime" SortExpression="RequestedDate" AllowSorting="true" HeaderStyle-Width="120px">
            </telerik:GridDateTimeColumn>
            <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" DataField="ValidFrom" DataFormatString="{0:dd-MM-yyyy}" FilterControlAltText="ValidFrom" SortExpression="ValidFrom" HeaderText="Start Date" UniqueName="ValidFrom" HeaderStyle-Width="120px" >
            </telerik:GridDateTimeColumn>
            <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" DataField="ValidTo" DataFormatString="{0:dd-MM-yyyy}" FilterControlAltText="ValidTo" SortExpression="ValidTo" HeaderText="End Date" UniqueName="ValidTo" HeaderStyle-Width="120px" FilterControlWidth="70%">
            </telerik:GridDateTimeColumn>
            <telerik:GridBoundColumn DataField="EndpointsystemName" Display="false" Visible="false" FilterControlAltText="EndpointsystemName" HeaderText="Target System" SortExpression="EndpointsystemName" UniqueName="EndpointsystemName" FilterControlWidth="70%" HeaderStyle-Width="20%">
            </telerik:GridBoundColumn>

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
                    <div class="row-fluid">
                        <uc1:InfoUC runat="server" ID="InfoUC" />
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnCancel" class="btn btn-primary bordered-button" Text="OK" OnClick="btnCancel_Click" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>

<div class="buttons text-right nav-rectangle">
    <asp:Button runat="server" ID="Button1" CssClass="btn btn-default btn-sm" Text="Refresh" OnClick="Button1_Click"></asp:Button>
    <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClientClick="Redirect()"></asp:Button>
</div>
