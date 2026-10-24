<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CloneUC.ascx.cs" Inherits="DI.Chakra.Web.Admin.Role.UC.CloneUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="UserSearchUC" %>

<asp:Panel runat="server" ID="ContentPanel" Enabled="true">
    <div class="content-wrapper" id="ContentWrapper" runat="server">
        <div class="form">
            <div class="form-horizontal left">
                <div class="row-fluid">
                    <div class="col-xs-12">
                        <div class="form-group">
                            <asp:Label ID="Label1" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="RoleName">Role Name</asp:Label>
                            <div class="col-xs-4">
                                <asp:TextBox ID="RoleName" TabIndex="1" runat="server" CssClass="form-control numeric" required="required" data-required-msg="Role Name is required"></asp:TextBox>
                            </div>
                            <div class="col-xs-6">
                                <asp:Label ID="Label2" runat="server" CssClass="col-xs-6" AssociatedControlID="CheckIsDefault">
                                    <asp:CheckBox runat="server" ID="CheckIsDefault" TabIndex="2"></asp:CheckBox>
                                    <span class="lbl">Is Default</span>
                                    <div id="" class="whats-this bottom showTooltip" style="margin-top:0px !important;">
                                        <a class="di_tooltip" data-toggle="tooltip" href="#"><span class="icon icon-question-sign t-vertical-align"></span></a>
                                        <span style="display: none" role="tooltip" class="tooltip right info">
                                            <h5><%= T("App.Hint.RoleIsDefault") %></h5>
                                        </span>
                                    </div>
                                </asp:Label>
                            </div>
                        </div>
                    </div>

                    <div class="col-xs-12">
                        <div class="form-group">
                            <label id="lblClone" class="col-xs-2">Clone</label>
                            <div class="col-xs-10">
                                <asp:Label ID="lblMenu" runat="server" AssociatedControlID="chkMenu">
                                    <asp:CheckBox runat="server" ID="chkMenu" TabIndex="6" />
                                    <span class="lbl">Menu</span>
                                </asp:Label>
                                <asp:Label ID="lblQL" runat="server" AssociatedControlID="chkQL">
                                    <asp:CheckBox runat="server" ID="chkQL" TabIndex="7" />
                                    <span class="lbl">QuickLink</span>
                                </asp:Label>
                                <asp:Label ID="lblDB" runat="server" AssociatedControlID="chkDashBoard">
                                    <asp:CheckBox runat="server" ID="chkDashBoard" TabIndex="8" />
                                    <span class="lbl">DashBoard</span>
                                </asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-12">
                        <div class="form-group">
                            <asp:Label ID="Label3" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="RoleDescription">Role Description</asp:Label>
                            <div class="col-xs-4">
                                <asp:TextBox runat="server" TabIndex="3" ID="RoleDescription" CssClass="form-control" TextMode="MultiLine"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Label4" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="RoleOwner">Owner</asp:Label>
                            <div class="col-xs-4">
                                <uc1:UserSearchUC TabIndexUC="4" runat="server" ID="RoleOwner" />
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-12">
                        <div class="form-group" runat="server" visible="false" >
                            <asp:Label ID="Label6" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="RoleMember">Role Member</asp:Label>
                            <div class="col-xs-4">
                                <uc1:UserSearchUC TabIndexUC="5" runat="server" ID="RoleMember" />
                            </div>
                            <div class="col-xs-4">
                                <asp:Button runat="server" ID="AddUser" TabIndex="6" CssClass="btn btn-default btn-sm" Text="Add" OnClick="AddUser_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Panel>

<telerik:RadGrid ID="RoleMemberGrid" runat="server"
    AllowPaging="true" AllowSorting="true" AllowFilteringByColumn="true" 
    OnNeedDataSource="RoleMemberGrid_NeedDataSource" OnSelectedIndexChanged="RoleMemberGrid_SelectedIndexChanged"
     OnItemCommand="RoleMemberGrid_ItemCommand" ClientSettings-EnablePostBackOnRowClick="true"
    FilterMenu-CssClass="RadMenu_IACMFilterSkin" CellSpacing="0" GridLines="None"
    Visible="false">
    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="3" Height="100%" AlwaysVisible="true" />
    <ClientSettings>
        <Scrolling UseStaticHeaders="true" />
        <Selecting AllowRowSelect="true" />
    </ClientSettings>
    <MasterTableView AllowFilteringByColumn="true" AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="false" DataKeyNames="Id" NoMasterRecordsText="No Records Found"  Width="100%" >
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn DataField="UserName" FilterControlAltText="UserName" AllowFiltering="true" SortExpression="UserName" HeaderText="User Name" FilterControlWidth="89%" HeaderStyle-Width="40%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="DisplayName" FilterControlAltText="DisplayName" SortExpression="DisplayName" HeaderText="Members" AllowFiltering="false" AllowSorting="false">
            </telerik:GridBoundColumn>
           <%-- <telerik:GridButtonColumn ConfirmText="Are you sure to delete the selected User?" ConfirmDialogType="RadWindow" HeaderText="Delete" HeaderStyle-HorizontalAlign="Center"
                ConfirmTitle="Delete" ButtonType="ImageButton" CommandName="Delete" Text="Delete"
                UniqueName="DeleteColumn" Exportable="false" HeaderStyle-Width="150px">
                <ItemStyle HorizontalAlign="Center" CssClass="action-delete" />
            </telerik:GridButtonColumn>--%>
        </Columns>
    </MasterTableView>
</telerik:RadGrid>

<div class="buttons text-right nav-rectangle">
    <asp:Button runat="server" class="btn btn-default btn-sm" ID="Button1" TabIndex="7" Text="Cancel" OnClick="btnCancel_Click" UseSubmitBehavior="false"></asp:Button>
</div>


