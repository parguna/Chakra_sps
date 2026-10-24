<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="DI.Chakra.Web.InHouse.UC.Detail" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="uc1" TagName="EndpointUC" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="UserSearchUC" %>

<script type="text/javascript">

    function getFullName() {
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
    }

</script>

<asp:Panel class="content-wrapper" ID="ContentWrapper" runat="server">
    <div class="form">
        <div class="form-horizontal left clearfix">
            <div class="row-fluid">
                <div class="col-xs-6">
                    <uc1:EndpointUC runat="server" ID="EndpointUC" OnEndpointChanged="EndpointUC_EndpointChanged" />
                    <div class="form-group">
                        <asp:Label ID="Label14" AssociatedControlID="txtIdentificationNo" runat="server" class="col-xs-4 control-label">Identification No.</asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtIdentificationNo" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="Label3" AssociatedControlID="txtEmail" runat="server" class="col-xs-4 control-label">Email</asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group required">
                        <asp:Label ID="Label4" AssociatedControlID="txtUserName" runat="server" class="col-xs-4 control-label">User Name</asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group required" runat="server" id="divPassword">
                        <asp:Label ID="Label16" AssociatedControlID="txtINHPassword" runat="server" class="col-xs-4 control-label">Password</asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtINHPassword" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="Label5" AssociatedControlID="txtFirstName" runat="server" class="col-xs-4 control-label">First Name</asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="Label7" AssociatedControlID="txtLastName" runat="server" class="col-xs-4 control-label">Last Name</asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtLastName" runat="server" onkeyup="getFullName()" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group required">
                        <asp:Label ID="Label6" AssociatedControlID="txtDisplayName" runat="server" class="col-xs-4 control-label">Display Name</asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtDisplayName" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group required">
                        <asp:Label ID="Label13" AssociatedControlID="TypeofAccounts" runat="server" class="col-xs-4 control-label">Account Type</asp:Label>
                        <div class="col-xs-7">
                            <asp:DropDownList runat="server" ID="TypeofAccounts" CssClass="form-control selectpicker" data-width="100%">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblEndPoint" AssociatedControlID="UserSearchUC" runat="server" CssClass="col-xs-4 control-label">Account Owner</asp:Label>
                        <div class="col-xs-7">
                            <uc1:UserSearchUC runat="server" ID="UserSearchUC" />
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblPermission" AssociatedControlID="InhousePermissionAutoCompleteBox" runat="server" CssClass="col-xs-4 control-label"><%= T("App.Common.INHPermission") %></asp:Label>
                        <div class="col-xs-7">
                            <telerik:RadAutoCompleteBox ID="InhousePermissionAutoCompleteBox" runat="server" EmptyMessage="Enter * to view full list" TextSettings-SelectionMode="Single"
                                InputType="Token" CssClass="form-control" Rows="4" required="required" >
                                <WebServiceSettings Path="../../Services/AutoCompleteDataService.svc" Method="GetInhouseSystemPermissionsByEndPointID" />
                            </telerik:RadAutoCompleteBox>
                            <asp:Label ID="Label2" runat="server" ForeColor="Red" CssClass="errormsg"></asp:Label>
                        </div>
                        <div class="col-xs-1">
                            <asp:Button ID="AddPermissionButton" ValidationGroup="AddPermission" runat="server" Text="Add" OnClick="AddPermissionButton_Click" CssClass="btn btn-default btn-sm" />
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="col-xs-4">
                        </div>
                        <div class="col-xs-7">
                            <telerik:RadGrid ID="Permission_FormGrid" runat="server" AllowPaging="true" AllowSorting="false" Width="100%"
                                OnItemCommand="Permission_ItemCommand" OnItemDataBound="Permission_ItemDataBound" OnPreRender="Permission_FormGrid_PreRender">
                                <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" AlwaysVisible="false" />
                                <ClientSettings>
                                    <Scrolling UseStaticHeaders="true" />
                                    <Selecting AllowRowSelect="false" />
                                </ClientSettings>
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView DataKeyNames="Key" AutoGenerateColumns="False" ShowHeadersWhenNoRecords="false" CommandItemDisplay="None" EnableHeaderContextMenu="false" TableLayout="Auto"  Width="100%" >
                                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="Value" FilterControlAltText="Value" SortExpression="Value" HeaderText="Permission Name" HeaderStyle-Width="70%" UniqueName="FunctionName">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" Text="Delete" ConfirmText="Are you sure you want to delete ?" HeaderText="Delete" ConfirmDialogType="RadWindow"
                                            UniqueName="DeleteColumn" Exportable="false" HeaderStyle-Width="20%">
                                            <ItemStyle HorizontalAlign="Center" CssClass="action-delete" />
                                        </telerik:GridButtonColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6">
                    <div class="form-group">
                        <label class="col-xs-4">Is Disabled</label>
                        <div class="col-xs-7">
                            <asp:CheckBox ID="IsDisabledCheckBox" runat="server"></asp:CheckBox>
                            <span class="lbl"></span>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-4">Is Deleted</label>
                        <div class="col-xs-7">
                            <asp:CheckBox ID="IsDeletedCheckBox" runat="server"></asp:CheckBox>
                            <span class="lbl"></span>
                        </div>
                    </div>                    
                    <div class="form-group">
                        <label class="col-xs-4">Is Locked</label>
                        <div class="col-xs-7">
                            <asp:CheckBox ID="IsLockedCheckBox" runat="server" AutoPostBack="true" OnCheckedChanged="IsLockedCheckBox_CheckedChanged"></asp:CheckBox>
                            <span class="lbl"></span>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lbllock" AssociatedControlID="ddlLockScenario" runat="server" Text="Lock Scenario" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:DropDownList ID="ddlLockScenario" runat="server" CssClass="form-control selectpicker"
                                data-width="100%" AutoPostBack="True">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="Label8" AssociatedControlID="txtRemarks1" runat="server" class="col-xs-4 control-label">Remarks1</asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtRemarks1" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="Label9" AssociatedControlID="txtRemarks2" runat="server" class="col-xs-4 control-label">Remarks2</asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtRemarks2" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="Label10" AssociatedControlID="txtRemarks3" runat="server" class="col-xs-4 control-label">Remarks3</asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtRemarks3" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="Label11" AssociatedControlID="txtRemarks4" runat="server" class="col-xs-4 control-label">Remarks4</asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtRemarks4" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="Label12" AssociatedControlID="txtRemarks5" runat="server" class="col-xs-4 control-label">Remarks5</asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtRemarks5" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Panel>
<div class="buttons text-right nav-rectangle topBuffer">
    <asp:Button runat="server" ID="CancelButton" Text="Cancel" CssClass="btn btn-default btn-sm cancel" OnClick="CancelButton_Click" />
</div>



