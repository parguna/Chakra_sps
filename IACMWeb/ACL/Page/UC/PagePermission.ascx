<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PagePermission.ascx.cs" Inherits="DI.Chakra.Web.ACL.Page.UC.PagePermission" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/ACL/Common/UC/ACLDataMethodSearchUC.ascx" TagPrefix="uc1" TagName="ACLDataMethodSearchUC" %>


<div class="content-wrapper" id="ContentWrapper" runat="server">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-12">
                    <div class="form-group">
                        <div>
                            <asp:Label ID="lblPermission" AssociatedControlID="ddlPermision" runat="server" Text="Permission" CssClass="col-xs-1 control-label"></asp:Label>
                        </div>
                        <div class="col-xs-3">
                            <asp:DropDownList ID="ddlPermision" DataTextField="Name" DataValueField="Id" runat="server" required="required" CssClass="form-control selectpicker">
                                <asp:ListItem Value="0">--Select--</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group">
                        <div>
                            <asp:Label ID="lblURL" AssociatedControlID="ACLDataMethodSearchUC" runat="server" Text="URL" CssClass="col-xs-1 control-label"></asp:Label>
                        </div>
                        <div class="col-xs-5">
                            <uc1:ACLDataMethodSearchUC runat="server" ID="ACLDataMethodSearchUC" DataSourceMethod="GetPageURLs" />
                        </div>
                        <div class="col-xs-1 buttons text-right">
                            <asp:Button ID="btnAddPermission" ValidationGroup="AddPermission" runat="server" Text="Add" CssClass="btn btn-default btn-sm" OnClick="btnAddPermission_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<telerik:RadGrid ID="gdPagePermission" runat="server" AllowPaging="true" AllowSorting="false" Width="57.3%" OnNeedDataSource="gdPagePermission_NeedDataSource"
    OnItemCommand="gdPagePermission_ItemCommand" ClientSettings-EnablePostBackOnRowClick="true">
    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" AlwaysVisible="false" />
    <ClientSettings>
        <Scrolling UseStaticHeaders="true" />
        <Selecting AllowRowSelect="true" />
    </ClientSettings>
    <GroupingSettings CaseSensitive="false" />
    <MasterTableView AutoGenerateColumns="False"  Width="100%"  DataKeyNames="Id" ShowHeadersWhenNoRecords="true" CommandItemDisplay="None" NoMasterRecordsText="No Records Available" EnableHeaderContextMenu="false">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn DataField="PermissionId" UniqueName="PermissionId" Display="false">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="UrlId" UniqueName="UrlId" Display="false">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="PermissionName" FilterControlAltText="PermissionName" SortExpression="PermissionName" HeaderText="Permission" HeaderStyle-Width="20%" UniqueName="PermissionName">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="PageUrlName" FilterControlAltText="PageUrlName" SortExpression="PageUrlName" HeaderText="Page Url" HeaderStyle-Width="75%" UniqueName="PageUrlName">
            </telerik:GridBoundColumn>
            <telerik:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" ConfirmText="Are you sure you want to delete ?" HeaderText="Delete"
                UniqueName="DeleteColumn" Exportable="false" HeaderStyle-Width="5%">
                <ItemStyle HorizontalAlign="Center" CssClass="action-delete" />
            </telerik:GridButtonColumn>
        </Columns>
    </MasterTableView>
</telerik:RadGrid>
<div class="buttons text-right nav-rectangle">
    <asp:Button runat="server" class="btn btn-default btn-sm" ID="btnCancel" Text="Cancel" UseSubmitBehavior="false" OnClick="btnCancel_Click"></asp:Button>
</div>



