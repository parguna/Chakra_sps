<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BulkAdhocAccessRequestUC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.BulkRequest.BulkAdhocAccessRequestUC" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/UserInfoUC.ascx" TagPrefix="uc1" TagName="UserInfoUC" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="UserSearchUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<script type="text/javascript">
    function SetToday() {
        var datepicker = $find("<%=DatePickerValidFrom.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);
        datepicker.hidePopup();
    }

    function OnDateSelected(sender, e) {
        var ValidFromPicker = $find("<%=DatePickerValidFrom.ClientID %>");
        var now = new Date();
        if (ValidFromPicker != null) {
            ValidFromPicker.set_minDate(new Date(now.getFullYear(), now.getMonth(), now.getDate()));
        }
    }

    function OnClientEntryAdded(sender, eventArgs) {
        $telerik.$(eventArgs.get_entry().get_token()).find('a').attr("title", "");
    }

    function fixHiddenfield() {
        return confirm('User cannot have secondary account access for this system. Are you sure you want to continue?');
    }

    function GridCreated(sender, args) {
        var scrollArea = sender.GridDataDiv;
        var parent = $get("trGrdid");
        var gridHeader = sender.GridHeaderDiv;
        scrollArea.style.height = parent.clientHeight -
            gridHeader.clientHeight + "px";
    }

    function OnClientBulkRequesting(sender, args) {
        var context = args.get_context();
        var hiddenIsFullTimeEmployee = document.getElementById("<%= hiddenIsFullTimeEmployee.ClientID%>").value;
        var hiddenRosEmpid = document.getElementById("<%= hiddenRosEmpid.ClientID%>").value;
        context["IsFulltimeEmployee"] = hiddenIsFullTimeEmployee;
        context["RosEmpid"] = hiddenRosEmpid;
    }
</script>
<style>
    .user-grid-scroll {
        height: 190px !important;
        overflow-y: auto;
    }

    .user-grid-image {
        height: 23px;
        line-height: 10px;
        padding-bottom: 0px;
    }

    .rgMasterTable .rgClipCells .rgClipCells {
        height: 0px !important;
    }
</style>
<div class="content-wrapper">
    <asp:HiddenField ID="hiddenIsFullTimeEmployee" runat="server" Value="True" />
    <asp:HiddenField ID="hiddenRosEmpid" runat="server" Value="" />
    <div class="form">
        <div class="form-horizontal left clearfix">
            <div class="row-fluid">
                <asp:HiddenField ID="serverDateTime" runat="server" />
                <div class="col-xs-7">
                    <div id="employeetypetr" runat="server" class="form-group">
                        <asp:Label ID="Label1" AssociatedControlID="EmployeeField" CssClass="col-xs-4" runat="server">Employee Type</asp:Label>
                        <div class="col-xs-7">
                            <asp:RadioButton ID="EmployeeField" runat="server" Checked="True" AutoPostBack="true" OnCheckedChanged="UserSelectionChanged" GroupName="UserSelect" Text="" />
                            <span class="lbl">Employee</span>
                            <asp:RadioButton ID="NonEmployeeField" runat="server" AutoPostBack="true" OnCheckedChanged="UserSelectionChanged" GroupName="UserSelect" Text="" />
                            <span class="lbl">Non-Employee</span>

                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4 control-label">User Name</label>
                        <div class="col-xs-6">
                            <telerik:RadAutoCompleteBox ID="UserNameAutoCompleteBox" OnEntryRemoved="UserNameAutoCompleteBox_EntryRemoved" OnClientRequesting="OnClientBulkRequesting" OnEntryAdded="UserNameAutoCompleteBox_EntryAdded" EmptyMessage="Enter * to view full list" OnClientEntryAdded="OnClientEntryAdded" runat="server" DropDownHeight="200px" DropDownWidth="300px" InputType="Token" CssClass="form-control" AutoPostBack="true">
                                <TextSettings SelectionMode="Single" />
                                <WebServiceSettings Method="GetUsersForBulkRequest" Path="../../../../Services/AutoCompleteDataService.svc" />
                            </telerik:RadAutoCompleteBox>
                        </div>
                        <asp:Button ID="AddRequestButton" OnClick="AddRequestButton_Click" Text="Add" runat="server" CssClass="col-xs-1 btn btn-default btn-sm" />

                    </div>

                    <div class="form-group">
                        <label class="col-xs-4 control-label"><%= T("App.ACR.Supervisor") %></label>
                        <div class="col-xs-7">
                            <uc1:UserSearchUC runat="server" ID="SupervisorAutoCompleteBox" AllowSingleUser="true" DataSourceMethod="GetROUsers"
                                OnUserChanged="SupervisorAutoCompleteBox_EntryAdded" />
                        </div>
                    </div>


                    <div class="form-group required">
                        <label class="col-xs-4 control-label"><%= T("App.ACR.ApplicationType") %></label>
                        <div class="col-xs-7">
                            <asp:DropDownList CssClass="form-control selectpicker" runat="server" ID="ddlEndPointSystemType" AutoPostBack="True" OnSelectedIndexChanged="ddlEndPointSystemType_SelectedIndexChanged" />
                        </div>
                    </div>

                    <div class="form-group required">
                        <label class="col-xs-4 control-label"><%= T("App.ACR.Application") %></label>
                        <div class="col-xs-7">
                            <asp:DropDownList CssClass="form-control selectpicker" runat="server" ID="ddlEndpoint" AutoPostBack="True" OnSelectedIndexChanged="ddlEndpoint_SelectedIndexChanged" />
                        </div>
                    </div>
                </div>
                <div id="trGrdid" runat="server" class="form-group col-xs-5 user-grid-scroll">
                    <div class="col-xs-12">
                        <telerik:RadGrid ID="UserRequestGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0" AutoGenerateColumns="false"
                            GridLines="None" Width="100%" OnItemCommand="UserRequestGrid_ItemCommand" OnNeedDataSource="UserRequestGrid_NeedDataSource" ShowFooter="false"
                            MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Request Available">
                            <ClientSettings EnableRowHoverStyle="true">
                                <Scrolling AllowScroll="true" UseStaticHeaders="true" />
                            </ClientSettings>
                            <MasterTableView CellPadding="0" CellSpacing="0" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available" Width="100%"
                                DataKeyNames="Id">
                                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                                <Columns>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="70%" HeaderText="User" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="Label2" runat="server" Text='<%# Eval("DisplayName") %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                    <telerik:GridTemplateColumn HeaderStyle-Width="15%" HeaderText="Org Info">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="ShowIcon" runat="server" CssClass="user-grid-image" CommandName="OrgInfo" CommandArgument='<%#Eval("Id") %>' src="../../../../Images/action-buttons/detail.png"></asp:ImageButton>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </telerik:GridTemplateColumn>

                                    <telerik:GridTemplateColumn HeaderStyle-Width="15%" HeaderText="Delete">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="LinkButton1" runat="server" CommandName="Delete" ToolTip="Delete" CommandArgument='<%#Eval("Id") %>' src="../../../../Images/action-buttons/delete.png"></asp:ImageButton>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="action-delete" HorizontalAlign="Center" />
                                    </telerik:GridTemplateColumn>

                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </div>
                </div>
            </div>
        </div>
        <div class="form-horizontal left clearfix">
            <div class="row-fluid">
                <div class="form-group">
                    <div class="col-xs-7">
                        <label class="col-xs-4 control-label"><%= T("App.ACR.RequestType") %></label>
                        <div class="col-xs-8">
                            <asp:RadioButton ID="NewAccRadioButton" runat="server" Checked="True" GroupName="AccessSelect" AutoPostBack="true" OnCheckedChanged="RequestTypeSelectionChanged" />
                            <span class="lbl"><%= T("App.ACR.NewAccount") %></span>
                            <asp:RadioButton ID="AmendmentRadioButton" runat="server" GroupName="AccessSelect" AutoPostBack="true" OnCheckedChanged="RequestTypeSelectionChanged" />
                            <span class="lbl"><%= T("App.ACR.Amendment") %></span>
                            <asp:RadioButton ID="DelAccRadioButton" runat="server" GroupName="AccessSelect" AutoPostBack="true" OnCheckedChanged="RequestTypeSelectionChanged" />
                            <span class="lbl"><%= T("App.ACR.DeleteAccount") %></span>
                        </div>
                    </div>

                    <div class="col-xs-4">
                        <asp:Panel runat="server" Visible="false" ID="AmendmentPanel">
                            Type : 
                            <asp:RadioButton ID="AddAmndRadioButton" runat="server" GroupName="AmendmentTypeSelect" AutoPostBack="true" />
                            <span class="lbl"><%= T("App.ACR.AmendAdd") %></span>
                            <asp:RadioButton ID="DeleteAmndRadioButton" runat="server" GroupName="AmendmentTypeSelect" AutoPostBack="true" />
                            <span class="lbl"><%= T("App.ACR.AmendRemove") %></span>
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>
        <div class="form-horizontal left clearfix">
            <div class="row-fluid">
                <div class="col-xs-7">

                    <div class="form-group required">
                        <label class="col-xs-4 control-label">Effective Date</label>
                        <div class="col-xs-7">
                            <telerik:RadDatePicker runat="server" ID="DatePickerValidFrom" ShowPopupOnFocus="false" onkeydown="handleDateTimePickerClickEvent(this,event)" EnableTyping="false" Skin="IACMSkin" CssClass="form-control" DatePopupButton-Visible="true">
                                <ClientEvents OnDateSelected="OnDateSelected"></ClientEvents>
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
                    <div id="RequestReasonDiv" runat="server" class="form-group">
                        <label class="col-xs-4 control-label">Comments to Approver</label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtComments" runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="col-xs-7">
                            <asp:Label ID="lblLicenseMsg" runat="server" ForeColor="Red"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-xs-7">
                            <asp:Label ID="lblInfo" runat="server" ForeColor="Red"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <telerik:RadWindow runat="server" ID="OrgInfoModalPopupExtender" Modal="true" Width="900px" Height="100%"
        RenderMode="Lightweight" Behaviors="Close,Resize" VisibleOnPageLoad="false" CssClass="windowcss" ReloadOnShow="true" ShowContentDuringLoad="false" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
        <ContentTemplate>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="modal-header">
                        <h3 class="modal-title" runat="server" id="H1">Org Information</h3>
                    </div>
                    <div class="modal-body">
                        <div class="form-horizontal left clearfix">
                             <div id="trCostCenter" runat="server" class="form-group">
                                <div class="col-xs-4">
                                    <asp:Label ID="lblCostCenter" runat="server" Text="CostCenter "></asp:Label>
                                </div>
                                <div class="col-xs-8">
                                    <asp:Label ID="txtCostCenter" runat="server" Enabled="false" CssClass="form-control"></asp:Label>
                                </div>
                            </div>
                            <div id="trCompany" runat="server" class="form-group">
                                <div class="col-xs-4">
                                    <asp:Label ID="lblCompany" runat="server" Text="Company "></asp:Label>
                                </div>
                                <div class="col-xs-8">
                                    <asp:Label ID="txtCompany" runat="server" Enabled="false" CssClass="form-control"></asp:Label>
                                </div>
                            </div>
                            <div id="trDepartment" runat="server" class="form-group">
                                <div class="col-xs-4">
                                    <asp:Label ID="lblDepartment" runat="server" Text="Department "></asp:Label>
                                </div>
                                <div class="col-xs-8">
                                    <asp:Label ID="txtDepartment" runat="server" Enabled="false" CssClass="form-control"></asp:Label>
                                </div>
                            </div>
                            <div id="trBranch" runat="server" class="form-group">
                                <div class="col-xs-4">
                                    <asp:Label ID="lblBranch" runat="server" Text="Branch "></asp:Label>
                                </div>
                                <div class="col-xs-8">
                                    <asp:Label ID="txtBranch" runat="server" Enabled="false" CssClass="form-control"></asp:Label>
                                </div>
                            </div>
                            <div id="trSection" runat="server" class="form-group">
                                <div class="col-xs-4">
                                    <asp:Label ID="lblSection" runat="server" Text="Section "></asp:Label>
                                </div>
                                <div class="col-xs-8">
                                    <asp:Label ID="txtSection" runat="server" Enabled="false" CssClass="form-control"></asp:Label>
                                </div>
                            </div>
                             <div id="divUnit" runat="server" class="form-group">
                                <div class="col-xs-4">
                                    <asp:Label ID="lblUnit" runat="server" Text="Unit "></asp:Label>
                                </div>
                                <div class="col-xs-8">
                                    <asp:Label ID="lblUnitName" runat="server" Enabled="false" CssClass="form-control"></asp:Label>
                                </div>
                            </div>
                           
                        </div>
                    </div>
                    <div class="modal-footer">
                        <br />
                        <asp:Button ID="btnOk" runat="server" class="btn btn-primary" OnClick="btnOk_Click" Text="Ok" Width="60px" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
</div>
