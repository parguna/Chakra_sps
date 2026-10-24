<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CloneUC.ascx.cs" Inherits="DI.Chakra.Web.Admin.Group.UC.CloneUC" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="usersearcUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Panel runat="server" ID="ContentPanel">
    <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left">
                <div class="row-fluid">
                    <div class="col-xs-11">
                        <div class="form-group">
                            <asp:Label ID="Label1" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="GroupName">Name</asp:Label>
                            <div class="col-xs-4">
                                <asp:TextBox runat="server" ID="GroupName" TabIndex="1" CssClass="form-control" MaxLength="25" required="required" data-required-msg="Group name is required"></asp:TextBox>
                            </div>
                            <div class="col-xs-6">
                                <asp:Label ID="Label2" CssClass="col-xs-6" runat="server" AssociatedControlID="CheckIsDefault">
                                    <asp:CheckBox runat="server" ID="CheckIsDefault" TabIndex="2" />
                                    <span class="lbl">Is Default</span>
                                    <div id="" class="whats-this bottom showTooltip" style="margin-top: 0px !important;">
                                        <a class="di_tooltip" data-toggle="tooltip" href="#"><span class="icon icon-question-sign t-vertical-align"></span></a>
                                        <span style="display: none" role="tooltip" class="tooltip right info">
                                            <h5><%= T("App.Hint.GroupIsDefault") %></h5>
                                        </span>
                                    </div>
                                </asp:Label>
                            </div>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="Label3" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="GroupDescription">Description</asp:Label>
                            <div class="col-xs-4">
                                <asp:TextBox runat="server" ID="GroupDescription" TabIndex="3" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="Label5" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="GroupDescription">Email Id</asp:Label>
                            <div class="col-xs-4">
                                <asp:TextBox runat="server" ID="Email" TabIndex="4" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="Label4" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="GroupOwner">Owner</asp:Label>
                            <div class="col-xs-4">
                                <uc1:usersearcUC runat="server" ID="GroupOwner" TabIndexUC="4" />
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-11">
                        <div class="form-group" runat="server" visible="false">
                            <asp:Label ID="Label6" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="GroupMember">Group Member</asp:Label>
                            <div class="col-xs-4">
                                <uc1:usersearcUC TabIndexUC="5" runat="server" ID="GroupMember" />
                            </div>
                            <div class="col-xs-4">
                                <asp:Button runat="server" ID="AddUser" CssClass="btn btn-default btn-sm" Text="Add" OnClick="AddUser_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Panel>
<telerik:RadGrid ID="GroupMemberGrid" runat="server" AllowPaging="true" AllowSorting="true" AllowFilteringByColumn="true" OnItemCommand="GroupMemberGrid_ItemCommand" OnNeedDataSource="GroupMemberGrid_NeedDataSource"
    ClientSettings-EnablePostBackOnRowClick="true" FilterMenu-CssClass="RadMenu_IACMFilterSkin" CellSpacing="0" GridLines="None" Visible="false">
    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="3" Height="100%" AlwaysVisible="true" />
    <ClientSettings>
        <Scrolling UseStaticHeaders="true" />
    </ClientSettings>
    <MasterTableView AllowFilteringByColumn="true" AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="false" DataKeyNames="Id" NoMasterRecordsText="No Records Found" Width="100%" >
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn DataField="UserName" FilterControlAltText="UserName" AllowFiltering="true" SortExpression="UserName" HeaderText="User Name" FilterControlWidth="89%" HeaderStyle-Width="40%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="DisplayName" FilterControlAltText="DisplayName" SortExpression="DisplayName" HeaderText="Members" AllowFiltering="false" AllowSorting="false">
            </telerik:GridBoundColumn>
            <%--  <telerik:GridButtonColumn ConfirmText="Are you sure to delete the selected User?" ConfirmDialogType="RadWindow" HeaderText="Delete" HeaderStyle-HorizontalAlign="Center"
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

