<%@ Control Language="C#" AutoEventWireup="true" Inherits="DI.Chakra.Web.WorkFlow.UC.WorkflowDetailUC" CodeBehind="WorkflowDetailUC.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/WorkFlow/UC/ApproversUC.ascx" TagPrefix="uc1" TagName="ApproversUC" %>

<script>

    function OnDateSelected(sender, eventArgs) {
        var date1 = sender.get_selectedDate();
      
        //date1.setDate(date1.getDate() + 31);
        var datepicker = $find("<%= dtValidFrom.ClientID %>");
        datepicker.set_minDate(date1);
    }
    function showApproverWindow() {
        var oWnd = $find("<%= ApproverWindow.ClientID %>");
        oWnd.show();
    }

    function closeApproverWindow() {
        var oWnd = $find("<%= ApproverWindow.ClientID %>");
        oWnd.close();
    }

    function SetToday(Parameter) {
        var datepicker = $find(Parameter == "VFButton" ? "<%=dtValidFrom.ClientID%>" : "<%=dtValidTo.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);
        datepicker.hidePopup();
    }

</script>

<asp:Panel runat="server" CssClass="content-wrapper" ID="MainDiv">
    <div class="form">
        <div class="form-horizontal left clearfix">
            <div class="row">
                <div class="col-xs-12">
                    <div class="col-xs-6">
                        <div class="form-group required">

                            <label class="col-xs-4 control-label control-label">Workflow Name</label>
                            <div class="col-xs-7">
                                <asp:TextBox ID="txtWorkflowName" runat="server" CssClass="form-control" MaxLength="50"
                                    onkeypress="return AllowAlphaNumberic(event);"></asp:TextBox>
                                <%--pattern="[a-zA-Z0-9\_\ \-]*"--%>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-xs-4 control-label">Workflow Description</label>
                            <div class="col-xs-7">
                                <asp:TextBox ID="txtWorkflowDescription" runat="server" CssClass="form-control" TextMode="MultiLine"></asp:TextBox>

                            </div>
                        </div>

                        <div class="form-group required">
                            <label class="col-xs-4 control-label">Valid From</label>
                            <div class="col-xs-7">
                                <%--<telerik:RadDatePicker ID="dtValidFrom" AutoPostBack="true" ClientEvents-OnDateSelected="OnDateSelected" runat="server" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
                                    DatePopupButton-Visible="true" Width="100%" CssClass="form-control">
                                </telerik:RadDatePicker>--%>

                                <telerik:RadDatePicker runat="server" ID="dtValidFrom" ShowPopupOnFocus="false" ClientEvents-OnDateSelected="OnDateSelected" onkeydown="handleDateTimePickerClickEvent(this,event)" EnableTyping="false" Skin="IACMSkin" CssClass="form-control" DatePopupButton-Visible="true">
                                    <Calendar ID="Calendar3" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                        <FooterTemplate>
                                            <div>
                                                <asp:Button ID="VFButton" runat="server" Width="100%" Text="Today" OnClientClick="SetToday('VFButton')" />
                                            </div>
                                        </FooterTemplate>
                                    </Calendar>
                                    <DateInput ID="DateInput3" ReadOnly="false" runat="server" ToolTip="Date input For Valid From"
                                    BorderStyle="NotSet" Label="">
                                    </DateInput>
                                </telerik:RadDatePicker>
                            </div>
                        </div>

                        <div class="form-group required">
                            <label class="col-xs-4 control-label">Valid To</label>
                            <div class="col-xs-7">
                                <%--<telerik:RadDatePicker ID="dtValidTo" AutoPostBack="true" runat="server" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
                                    DatePopupButton-Visible="true" Width="100%" CssClass="form-control" MaxDate="12/31/9999">
                                </telerik:RadDatePicker>
                            </div>--%>

                                <telerik:RadDatePicker runat="server" ID="dtValidTo" ShowPopupOnFocus="false" onkeydown="handleDateTimePickerClickEvent(this,event)" EnableTyping="false" Skin="IACMSkin" CssClass="form-control" MaxDate="12/31/9999"
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

                        <div class="form-group">
                            <label class="col-xs-4 control-label">Is Active</label>
                            <div class="col-xs-7">
                                <div style="width: 40%; float: left;">
                                    <asp:CheckBox ID="IsActiveCheckBox" runat="server" Text=""></asp:CheckBox>
                                    <span class="lbl">Is Active</span>
                                </div>
                                <div id="parentDelimitedDiv" runat="server" visible="false" style="width: 60%; float: left;">
                                    <asp:CheckBox ID="ParentDelimitedCheckBox" runat="server" Text=""></asp:CheckBox>
                                    <span class="lbl">Parent Delimited?</span>
                                </div>
                            </div>
                        </div>

                    </div>

                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-xs-4 control-label">Workflow Type</label>
                            <div class="col-xs-7">
                                <asp:DropDownList ID="ddlRequestType" AutoPostBack="true" OnSelectedIndexChanged="ddlRequestType_SelectedIndexChanged" runat="server" CssClass="form-control selectpicker" data-width="100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-xs-4 control-label"><%= T("App.Common.Endpoint") %></label>
                            <div class="col-xs-7">
                                <asp:DropDownList ID="ApplicationTypeDropDownList" runat="server" AutoPostBack="true" CssClass="form-control selectpicker" OnSelectedIndexChanged="ApplicationTypeDropDownList_SelectedIndexChanged" CausesValidation="true" data-width="100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="form-group">

                            <label class="col-xs-4 control-label">User Type</label>
                            <div class="col-xs-7">
                                <telerik:RadComboBox ID="UserTypeComboBox" Style="width: 100% !important" DropDownAutoWidth="Enabled" Width="100%" runat="server" CheckBoxes="true" CausesValidation="true" data-width="100%" AutoPostBack="true" OnSelectedIndexChanged="ComboBox_SelectedIndexChanged">
                                </telerik:RadComboBox>
                            </div>
                        </div>
                        <div class="form-group">

                            <label class="col-xs-4 control-label">Account Type</label>
                            <div class="col-xs-7">
                                <telerik:RadComboBox ID="AccountTypeComboBox" Style="width: 100% !important" DropDownAutoWidth="Enabled" runat="server" CheckBoxes="true" CausesValidation="true" data-width="100%" AutoPostBack="true" OnSelectedIndexChanged="ComboBox_SelectedIndexChanged">
                                </telerik:RadComboBox>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-xs-4 control-label">Action Type</label>
                            <div class="col-xs-7">
                                <telerik:RadComboBox ID="ActionTypeComboBox" Style="width: 100% !important" DropDownAutoWidth="Enabled" runat="server" CheckBoxes="true" CausesValidation="true" data-width="100%" AutoPostBack="true" OnSelectedIndexChanged="ComboBox_SelectedIndexChanged">
                                </telerik:RadComboBox>
                            </div>
                        </div>
                        <div class="form-group" runat="server" visible="false" id="divRoleType">
                            <label class="col-xs-4 control-label">Entitlement Type</label>
                            <div class="col-xs-7">
                                 <asp:DropDownList ID="ddlEntitlementType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlEntitlementType_SelectedIndexChanged" CssClass="form-control selectpicker" CausesValidation="true" data-width="100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-xs-12 topBuffer">
                    <div class="form-group">
                        <div class="col-xs-3">
                            <asp:LinkButton ID="AddApproverLinkButton" runat="server" Text="Add Approvers" CssClass="btn btn-default btn-sm" OnClick="AddApproverLinkButton_Click"></asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Panel>
<label class="topBuffer">Approvers</label>
<telerik:RadGrid ID="ApproversGrid" runat="server" AllowFilteringByColumn="false" AllowPaging="true" AllowSorting="false" CellSpacing="0"
    OnItemCommand="ApproversGrid_ItemCommand" OnNeedDataSource="ApproversGrid_NeedDataSource" OnItemDataBound="ApproversGrid_ItemDataBound">
    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" AlwaysVisible="true" />
    <ClientSettings>
        <Scrolling UseStaticHeaders="true" />
    </ClientSettings>
    <GroupingSettings CaseSensitive="false" />
    <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="none" EnableHeaderContextMenu="false" ShowHeadersWhenNoRecords="false" DataKeyNames="SequenceNumber" NoMasterRecordsText="No Records Available" TableLayout="Fixed">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" />
        <Columns>
            <telerik:GridTemplateColumn DataField="ApprovalUserType" HeaderText="User Type" FilterControlAltText="ApprovalUserType" SortExpression="ApprovalUserType" UniqueName="ApprovalUserType">
                <ItemTemplate>
                    <asp:LinkButton ID="btnEdit" runat="server" Text='<%#Eval("ApprovalUserType") %>'
                        CommandArgument='<%# Eval("WorkflowConfigurationDetailId") %>' CommandName="EditApprover" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>

            <telerik:GridBoundColumn DataField="SequenceNumber" FilterControlAltText="SequenceNumber" HeaderText="Sequence Number" SortExpression="SequenceNumber" UniqueName="SequenceNumber">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Approver" FilterControlAltText="Approver" HeaderText="Approver" SortExpression="Approver" UniqueName="Approver">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="ResponseTimeInDays" FilterControlAltText="ResponseTimeInDays" HeaderText="Response Time (Days)" SortExpression="ResponseTimeInDays" UniqueName="ResponseTimeInDays">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="IsReminderRequired" FilterControlAltText="IsReminderRequired" HeaderText="Reminder Interval (Days)" SortExpression="IsReminderRequired" UniqueName="IsReminderRequired">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="IsEscalationRequired" FilterControlAltText="IsEscalationRequired" HeaderText="Escalation Interval" SortExpression="IsEscalationRequired" UniqueName="IsEscalationRequired">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="AdditionalInfo" FilterControlAltText="AdditionalInfo" HeaderText="Additional Info" SortExpression="AdditionalInfo" UniqueName="AdditionalInfo">
            </telerik:GridBoundColumn>
            <telerik:GridButtonColumn ConfirmText="Are you sure you want to delete the Approver?" ConfirmDialogType="RadWindow"
                ConfirmTitle="Delete" ButtonType="ImageButton" CommandName="Delete" Text="Delete" HeaderText="Delete"
                UniqueName="DeleteColumn" Exportable="false" HeaderStyle-Width="5%">
                <ItemStyle HorizontalAlign="Center" CssClass="action-delete" />
            </telerik:GridButtonColumn>
        </Columns>
    </MasterTableView>
</telerik:RadGrid>

<telerik:RadWindow runat="server" ID="ApproverWindow" Modal="true" Height="550px" Width="700px" Title="Approver Detail"
    RenderMode="Lightweight" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <uc1:ApproversUC ID="ApproverDetailControl" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>

<div class="buttons text-right nav-rectangle topBuffer">
    <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" PostBackUrl="~/WorkFlow/List.aspx" UseSubmitBehavior="false" OnClick="btnCancel_Click" />
</div>
