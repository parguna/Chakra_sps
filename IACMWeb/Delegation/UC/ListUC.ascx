<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Delegation.UC.ListUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="UserSearchUC" %>


<script type="text/javascript">
    function validateInputs() {
        alert("hi");
        var isValid = true;
        var clientId = $("[id$=_Comment]").attr("id");
        if (!validateAutoCompleteBox(this, 'delegateFrom_UserNameAutoCompleteBox', 'Please select from user')) {
            isValid = false;
        }
        else if (!validateAutoCompleteBox(this, 'delegateTo_UserNameAutoCompleteBox', 'Please enter to user')) {
            isValid = false;
        }
        else if (clientId != "" && document.getElementById(clientId).value == "") {
            Alert("Please enter comments");
            isValid = false;
        }
        else {
            var masterTable = $(".rgMasterTable");
            if (masterTable != null) {
                var checkboxes = $("td input:checkbox:checked", masterTable);
                if (checkboxes.length == 0) {
                    Alert("Please select at least 1 task item to delegate");
                    isValid = false;
                }
            }
        }
        return isValid;
    }
</script>


<div class="content-wrapper">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                    <div class="form-group">
                        <label class="col-xs-4 control-label">Delegate From</label>
                        <div class="col-xs-7">
                            <uc1:UserSearchUC runat="server" ID="delegateFrom" Visible="true" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-4 control-label">Delegate To</label>
                        <div class="col-xs-7">
                            <uc1:UserSearchUC runat="server" ID="delegateTo" Visible="true" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-4 control-label">Comments</label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="Comment" runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                            <span class="lbl"></span>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4 control-label">Is Active</label>
                        <div class="col-xs-7">
                            <asp:Label ID="Label1" AssociatedControlID="IsActiveCheckBox" runat="server" CssClass="col-xs-3">
                                <asp:CheckBox ID="IsActiveCheckBox" runat="server" />
                                <span class="lbl"></span>
                            </asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

            <div class="buttons text-right nav-rectangle topBuffer">
                <asp:Button runat="server" ID="SaveButton" Text="Save" CssClass="btn btn-default btn-sm validation" OnClick="SaveButton_Click" />
                <asp:Button runat="server" ID="CancelButton" Text="Cancel" CssClass="btn btn-default btn-sm cancel" OnClick="CancelButton_Click" />
            </div>

<%--OnClientClick ="return validateInputs()"--%>

<div id="gridTitle" runat="server"></div>
<telerik:RadGrid ID="grdTask" runat="server" AllowSorting="True" AllowFilteringByColumn="true" AllowMultiRowSelection="true" FilterMenu-CssClass="RadMenu_IACMFilterSkin"
    ClientSettings-EnablePostBackOnRowClick="true" AllowPaging="True" OnNeedDataSource="grdTask_NeedDataSource" OnItemCommand="grdTask_ItemCommand"
    EnableLinqExpressions="false">

    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" AlwaysVisible="true" />
    <ClientSettings>
        <Scrolling UseStaticHeaders="true" />
        <Selecting AllowRowSelect="true" />
    </ClientSettings>
    <GroupingSettings CaseSensitive="false" />
    <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="true" DataKeyNames="Id" ShowHeadersWhenNoRecords="false" NoMasterRecordsText="No Records Available"
        TableLayout="Fixed" Width="100%">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridTemplateColumn AllowFiltering="false">
                <HeaderTemplate>
                    <asp:CheckBox ID="chkheader" runat="server" OnCheckedChanged="ToggleSelectedState" AutoPostBack="true" />
                    <span class="lbl"></span>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chbxSelect" runat="server" />
                    <span class="lbl"></span>
                    <asp:HiddenField ID="hdnTaskId" runat="server" Value='<%#Eval("TaskId") %>' />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn DataField="AccessRequestID" FilterControlAltText="AccessRequestID" SortExpression="AccessRequestID" HeaderText="Request ID" UniqueName="AccessRequestID">
            </telerik:GridBoundColumn>
            <telerik:GridTemplateColumn DataField="AttestationName" HeaderText="Application Name" FilterControlAltText="AttestationName" SortExpression="AttestationName" UniqueName="AttensationName" Resizable="true">
                <ItemTemplate>
                    <asp:LinkButton ID="ViewTaskButton" runat="server" ForeColor="Blue" Text='<%#Eval("TaskName") %>'
                        ToolTip='Edit' CommandArgument='<%#Eval("TaskId") %>' CommandName='<%#Eval("RequestTypeId") %>' />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn DataField="Type" FilterControlAltText="Type" SortExpression="Type" HeaderText="Request Type" UniqueName="Type">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="RequestType" FilterControlAltText="RequestType" SortExpression="RequestType" HeaderText="Task Type" UniqueName="RequestType">
                <ColumnValidationSettings>
                    <ModelErrorMessage Text=""></ModelErrorMessage>
                </ColumnValidationSettings>
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="RequestedFor" FilterControlAltText="RequesterName" SortExpression="RequesterName" HeaderText="Request for" UniqueName="RequesterName">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="RequestedBy" FilterControlAltText="RequesteeName" SortExpression="RequesteeName" HeaderText="Request by" UniqueName="RequesteeName">
            </telerik:GridBoundColumn>
            <telerik:GridTemplateColumn HeaderText="IsDelegated" SortExpression="IsDelegated" UniqueName="IsDelegated">
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%#(Convert.ToBoolean(Eval("IsDelegated"))? "Yes":"No") %>' />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn DataField="Entitlement" FilterControlAltText="Entitlement" SortExpression="Entitlement" HeaderText="Entitlement" UniqueName="Entitlement">
            </telerik:GridBoundColumn>
            <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" DataField="CreatedDate" FilterControlAltText="CreatedDate" SortExpression="CreatedDate" HeaderText="Request Date" UniqueName="CreatedDate" >
            </telerik:GridDateTimeColumn>
            <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" DataField="StartDate" FilterControlAltText="RunDate" HeaderText="Start Date"  SortExpression="StartDate" UniqueName="StartDate">
            </telerik:GridDateTimeColumn>
            <telerik:GridDateTimeColumn EnableTimeIndependentFiltering="true" DataField="EndDate" FilterControlAltText="EndDate" HeaderText="End Date"  SortExpression="EndDate" UniqueName="EndDate">
            </telerik:GridDateTimeColumn>
            <telerik:GridBoundColumn DataField="Status" FilterControlAltText="Status" SortExpression="Status" HeaderText="Status" UniqueName="Status">
            </telerik:GridBoundColumn>
        </Columns>
    </MasterTableView>
</telerik:RadGrid>

