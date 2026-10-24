<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InhouseTaskInfoUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.Task.Applications.InhouseTaskInfoUC" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="uc1" TagName="EndpointUC" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="UserSearchUC" %>


<script type="text/javascript">
    $(window).load(function () {
        var getClassListCtl = document.getElementById('<%=ClassList.ClientID %>');
        var getClassList = "";

        if (getClassListCtl != null)
            getClassList = getClassListCtl.value;

        if (getClassList != "" || getClassList != null) {
            getClassList = getClassList.split(", ");
            var i = 0;
            $('.multi-select').find('.racToken').each(function (e) {
                var className = getClassList[i];
                if (className != null) {
                    $(this).addClass(className);
                }
                i++;
            });
        }
    });
</script>

<script type="text/javascript">

    function fill() {
        var txt1 = document.getElementById('<%= txtFirstName.ClientID %>').value;
        var txt2 = document.getElementById('<%= txtLastName.ClientID %>').value;
        document.getElementById('<%= txtDisplayName.ClientID %>').value = txt1 + " " + txt2;
    }

    function EntryAdding(sender, eventArgs) {
        var entries = sender.get_entries();
        var count = entries.get_count();
        for (var i = 0; i < count; i++) {
            if (entries.getEntry(i)._text == eventArgs.get_entry()._text) {
                var entry1 = sender.get_entries().getEntry(i);
                sender.get_entries().remove(entry1);
                break;
            }
        }

        function SetValidAccCreatedDate(obj) {
            var datepicker = $find("<%=DatePickerAccCreated.ClientID%>");
            var dt = new Date();
            datepicker.set_selectedDate(dt);

        }

        function SetValidLockedDate() {
            var datepicker = $find("<%=DatePickerLockedDate.ClientID%>");
            var dt = new Date();
            datepicker.set_selectedDate(dt);
        }

        function SetValidDisabledDate(obj) {
            var datepicker = $find("<%=DatePickerIsDisabled.ClientID%>");
            var dt = new Date();
            datepicker.set_selectedDate(dt);

        }

        function SetValidDeletedDate() {
            var datepicker = $find("<%=DatePickerIsDeleted.ClientID%>");
            var dt = new Date();
            datepicker.set_selectedDate(dt);
        }
    }



</script>
<style>
    .instrucion-inhouse {
        color: #297ac0;
        font-size: medium;
    }
</style>
<div class="form">
    <div class="form-horizontal left clearfix">
        <div class="row-fluid form-group">
            <div class="col-xs-12">
                <div class="col-xs-3">
                    Special Remarks
                </div>
                <div class="col-xs-9 instrucion-inhouse">
                    Please update the AD access for requests related to these systems:
                    &nbsp;<asp:Label ID="lblADAccessSystems" runat="server"></asp:Label>
                </div>
            </div>
        </div>
        <div class="row-fluid form-group">
            <asp:Panel ID="AccountOwnerPanel" runat="server" Visible="false">
                <div class="col-xs-12">
                    <div class="col-xs-3 ">
                        <asp:Label ID="lblAccountOwner" AssociatedControlID="UserSearchUC" runat="server">Account Owner</asp:Label>
                    </div>
                    <div class="col-xs-9">
                        <uc1:UserSearchUC runat="server" ID="UserSearchUC" />
                    </div>
                </div>
            </asp:Panel>
        </div>
        <div class="row-fluid">
            <asp:Panel ID="ContentPanel" runat="server" Visible="false">
                <asp:TextBox runat="server" ID="ClassList" Style="display: none;" CssClass="ClassList"></asp:TextBox>
                <div class="col-xs-12">
                    <div class="col-xs-6">
                        <%--<uc1:EndpointUC runat="server" ID="EndpointUC" OnEndpointChanged="EndpointUC_EndpointChanged" />--%>
                        <div class="form-group">
                            <asp:Label ID="Label15" AssociatedControlID="lblApplicationType" runat="server" CssClass="col-xs-6 control-label">Application Type</asp:Label>
                            <div class="col-xs-6">
                                <asp:Label ID="lblApplicationType" runat="server" CssClass="description"></asp:Label>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Label14" AssociatedControlID="txtIdentificationNo" runat="server" class="col-xs-6 control-label">Identification No.</asp:Label>
                            <div class="col-xs-6">
                                <asp:TextBox ID="txtIdentificationNo" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Label3" AssociatedControlID="txtEmail" runat="server" class="col-xs-6 control-label">Email</asp:Label>
                            <div class="col-xs-6">
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Label5" AssociatedControlID="txtFirstName" runat="server" class="col-xs-6 control-label">First Name</asp:Label>
                            <div class="col-xs-6">
                                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Label7" AssociatedControlID="txtLastName" runat="server" class="col-xs-6 control-label">Last Name</asp:Label>
                            <div class="col-xs-6">
                                <asp:TextBox ID="txtLastName" runat="server" onkeyup="fill()" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group required">
                            <asp:Label ID="Label6" AssociatedControlID="txtDisplayName" runat="server" class="col-xs-6 control-label">Display Name</asp:Label>
                            <div class="col-xs-6">
                                <asp:TextBox ID="txtDisplayName" runat="server" CssClass="form-control"></asp:TextBox>
                                <%-- required="required"--%>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-xs-6 control-label">Is Disabled</label>
                            <div class="col-xs-6">
                                <asp:CheckBox ID="IsDisabledCheckBox" runat="server" AutoPostBack="true" OnCheckedChanged="IsDisabledCheckBox_CheckedChanged" CssClass="form-control"></asp:CheckBox>
                                <span class="lbl"></span>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-xs-6 control-label">Is Deleted</label>
                            <div class="col-xs-6">
                                <asp:CheckBox ID="IsDeletedCheckBox" runat="server" AutoPostBack="true" OnCheckedChanged="IsDeletedCheckBox_CheckedChanged" CssClass="form-control"></asp:CheckBox>
                                <span class="lbl"></span>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-xs-6 control-label">Is Locked</label>
                            <div class="col-xs-6">
                                <asp:CheckBox ID="IsLockedCheckBox" runat="server" AutoPostBack="true" OnCheckedChanged="IsLockedCheckBox_CheckedChanged" CssClass="form-control"></asp:CheckBox>
                                <span class="lbl"></span>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Label8" AssociatedControlID="txtRemarks1" runat="server" class="col-xs-6 control-label">Remarks1</asp:Label>
                            <div class="col-xs-6">
                                <asp:TextBox ID="txtRemarks1" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Label9" AssociatedControlID="txtRemarks2" runat="server" class="col-xs-6 control-label">Remarks2</asp:Label>
                            <div class="col-xs-6">
                                <asp:TextBox ID="txtRemarks2" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Label10" AssociatedControlID="txtRemarks3" runat="server" class="col-xs-6 control-label">Remarks3</asp:Label>
                            <div class="col-xs-6">
                                <asp:TextBox ID="txtRemarks3" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-6">
                        <div class="form-group">
                            <asp:Label ID="Label18" AssociatedControlID="lblEndpoint" runat="server" CssClass="col-xs-6 control-label">Application</asp:Label>
                            <div class="col-xs-6">
                                <asp:Label ID="lblEndpoint" runat="server" CssClass="description"></asp:Label>
                            </div>
                        </div>
                        <div class="form-group required">
                            <asp:Label ID="Label4" AssociatedControlID="txtUserName" runat="server" class="col-xs-6 control-label">User Name</asp:Label>
                            <div class="col-xs-6">
                                <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control"></asp:TextBox>
                                <%--required="required"--%>
                            </div>
                        </div>
                        <div class="form-group required" runat="server" id="divPassword">
                            <asp:Label ID="Label16" AssociatedControlID="txtINHPassword" runat="server" class="col-xs-6 control-label">Password</asp:Label>
                            <div class="col-xs-6">
                                <asp:TextBox ID="txtINHPassword" runat="server" TextMode="Password" CssClass="form-control" autocomplete="new-password"></asp:TextBox>
                                <%-- required="required"--%>
                            </div>
                        </div>
                        <div class="form-group required">
                            <asp:Label ID="Label13" AssociatedControlID="TypeofAccounts" runat="server" class="col-xs-6 control-label">Account Type</asp:Label>
                            <div class="col-xs-6">
                                <asp:DropDownList runat="server" ID="TypeofAccounts" CssClass="form-control selectpicker" data-width="100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-xs-6">Account Created</label>
                            <div class="col-xs-5">
                                <telerik:RadDatePicker runat="server" onkeydown="handleDateTimePickerClickEvent(this,event)" ID="DatePickerAccCreated" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
                                    DatePopupButton-Visible="true" Width="100%" CssClass="form-control">

                                    <Calendar ID="Calendar2" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                        <FooterTemplate>
                                            <div style="width: 100%; text-align: center; background-color: Gray;">
                                                <asp:Button ID="EButton" runat="server" Width="100%" Text="Today" OnClientClick="SetValidAccCreatedDate()" />

                                            </div>
                                        </FooterTemplate>
                                    </Calendar>
                                    <DateInput ID="DateInput2" ReadOnly="false" runat="server" ToolTip="Date input For Start Date"
                                        BorderStyle="NotSet" Label="">
                                    </DateInput>
                                </telerik:RadDatePicker>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-xs-6">Disabled Date</label>
                            <div class="col-xs-5">
                                <telerik:RadDatePicker runat="server" ID="DatePickerIsDisabled" onkeydown="handleDateTimePickerClickEvent(this,event)" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
                                    DatePopupButton-Visible="true" Width="100%" CssClass="form-control">

                                    <Calendar ID="Calendar3" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                        <FooterTemplate>
                                            <div style="width: 100%; text-align: center; background-color: Gray;">
                                                <asp:Button ID="EButton" runat="server" Width="100%" Text="Today" OnClientClick="SetValidDisabledDate()" />

                                            </div>
                                        </FooterTemplate>
                                    </Calendar>
                                    <DateInput ID="DateInput3" ReadOnly="false" runat="server" ToolTip="Date input For Start Date"
                                        BorderStyle="NotSet" Label="">
                                    </DateInput>
                                </telerik:RadDatePicker>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-xs-6">Deleted Date</label>
                            <div class="col-xs-5">
                                <telerik:RadDatePicker runat="server" ID="DatePickerIsDeleted" onkeydown="handleDateTimePickerClickEvent(this,event)" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
                                    DatePopupButton-Visible="true" Width="100%" CssClass="form-control">

                                    <Calendar ID="Calendar4" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                        <FooterTemplate>
                                            <div style="width: 100%; text-align: center; background-color: Gray;">
                                                <asp:Button ID="EButton" runat="server" Width="100%" Text="Today" OnClientClick="SetValidDeletedDate()" />

                                            </div>
                                        </FooterTemplate>
                                    </Calendar>
                                    <DateInput ID="DateInput4" ReadOnly="false" runat="server" ToolTip="Date input For Start Date"
                                        BorderStyle="NotSet" Label="">
                                    </DateInput>
                                </telerik:RadDatePicker>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-xs-6">Locked Date</label>
                            <div class="col-xs-5">
                                <telerik:RadDatePicker runat="server" ID="DatePickerLockedDate" onkeydown="handleDateTimePickerClickEvent(this,event)" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
                                    DatePopupButton-Visible="true" Width="100%" CssClass="form-control">

                                    <Calendar ID="Calendar1" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                        <FooterTemplate>
                                            <div style="width: 100%; text-align: center; background-color: Gray;">
                                                <asp:Button ID="EButton" runat="server" Width="100%" Text="Today" OnClientClick="SetValidLockedDate()" />

                                            </div>
                                        </FooterTemplate>
                                    </Calendar>
                                    <DateInput ID="DateInput1" ReadOnly="false" runat="server" ToolTip="Date input For Start Date"
                                        BorderStyle="NotSet" Label="">
                                    </DateInput>
                                </telerik:RadDatePicker>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="lbllock" AssociatedControlID="ddlLockScenario" runat="server" Text="Lock Scenario" CssClass="col-xs-6 control-label"></asp:Label>
                            <div class="col-xs-6">
                                <asp:DropDownList ID="ddlLockScenario" runat="server" CssClass="form-control selectpicker"
                                    data-width="100%" AutoPostBack="True">
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="Label11" AssociatedControlID="txtRemarks4" runat="server" class="col-xs-6 control-label">Remarks4</asp:Label>
                            <div class="col-xs-6">
                                <asp:TextBox ID="txtRemarks4" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Label12" AssociatedControlID="txtRemarks5" runat="server" class="col-xs-6 control-label">Remarks5</asp:Label>
                            <div class="col-xs-6">
                                <asp:TextBox ID="txtRemarks5" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>

                        <%-- <div class="form-group col-xs-10">
                <div class="buttons">
                    <asp:Button ID="SaveButton" runat="server" CssClass="btn btn-default btn-sm" OnClick="SaveButton_Click" Text="Save" />
                </div>
            </div>--%>
                    </div>
                    <div class="col-xs-12 form-group" style="text-align: right;">
                        <asp:Label runat="server" ID="Notifications" class="description orange">Please validate the Account Changes, Click submit to proceed.</asp:Label>
                    </div>
                </div>
            </asp:Panel>
        </div>
        <div class="row-fluid" id="divEntitlements" runat="server" visible="false">
            <div class="col-xs-12 form-group">
                <div class="col-xs-12 table-header clear">
                    <h4 class="form-group">Entitlement Detail</h4>
                </div>
            </div>
            <div class="col-xs-12">
                <div class="col-xs-6">
                    <%--<div class="form-group">
                        <asp:Label ID="Label1" AssociatedControlID="RolesAutoCompleteBox" runat="server" CssClass="col-xs-6 control-label"><%= T("App.Common.INHPermission") %></asp:Label>
                        <div class="col-xs-6">
                            <telerik:RadAutoCompleteBox ID="RolesAutoCompleteBox" runat="server" AutoPostBack="true" EmptyMessage="Enter * to view full list" TextSettings-SelectionMode="Single"
                                InputType="Token" CssClass="form-control" OnEntryAdded="RolesAutoCompleteBox_EntryAdded" OnEntryRemoved="RolesAutoCompleteBox_EntryRemoved">
                                <WebServiceSettings Method="GetEntitlementInHouse" Path="~/Services/AutoCompleteDataService.svc" />
                            </telerik:RadAutoCompleteBox>
                        </div>
                    </div>
                    <div class="form-group" runat="server" id="divFunctions" visible="false">
                        <asp:Label ID="Label2" AssociatedControlID="ddlFunctions" runat="server" CssClass="col-xs-6 control-label">Functions</asp:Label>
                        <div class="col-xs-6">
                            <telerik:RadComboBox CheckBoxes="true" ID="ddlFunctions" runat="server" CssClass="col-xs-12" Style="width: 100%! important">
                            </telerik:RadComboBox>
                        </div>
                    </div>
                    <div class="form-group" id="addPermissionButtonDiv" runat="server">
                        <div class="col-xs-6">
                        </div>
                        <div class="col-xs-6">
                            <asp:Button ID="AddPermissionButton" ValidationGroup="AddPermission" runat="server" Text="Add" OnClick="AddPermissionButton_Click" CssClass="btn btn-default btn-sm" />
                        </div>
                    </div>--%>
                    <div class="form-group" id="divGrid" runat="server">
                        <telerik:RadGrid ID="RequestGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                            GridLines="None" Width="100%" OnItemCommand="RequestGrid_ItemCommand" ShowFooter="false"
                            MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Request Available">
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="False" Width="100%">
                                <Columns>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Permission" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPermission" runat="server" Text='<%# ((KeyValuePair<string,string>)Container.DataItem).Key %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Function" ItemStyle-HorizontalAlign="Left" UniqueName="Function">
                                        <ItemTemplate>
                                            <asp:Label ID="lblFunction" runat="server" Text='<%# ((KeyValuePair<string,string>)Container.DataItem).Value %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <%--<telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="Delete">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="DeleteRoleButton" runat="server" CommandName="Delete" Text="Delete" ToolTip="Delete" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>--%>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
