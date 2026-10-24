<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Master/ProtoMast4.master" CodeBehind="History.aspx.cs" Inherits="DI.Chakra.Web.Log.History" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="UserSearchUC" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left">
                <div class="row-fluid">
                    <div class="col-xs-12">
                        <div class="form-group required">
                            <asp:Label ID="Label2" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="TableName">TableName</asp:Label>
                            <div class="col-xs-3">
                                <asp:TextBox runat="server" ID="TableName" TabIndex="1" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-xs-1"></div>
                            <div class="col-xs-4">
                                <asp:Button runat="server" ID="Filter" TabIndex="6" CssClass="btn btn-default btn-sm" Text="Filter" OnClick="Filter_Click" />
                            </div>
                            <asp:Label ID="Label3" Visible="false" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="DatePicker">Date</asp:Label>
                            <div class="col-xs-3" style="display: none">
                                <%--<telerik:RadDatePicker runat="server" ID="DatePicker" Skin="IACMSkin" CssClass="form-control">
                                </telerik:RadDatePicker>--%>
                                <cc1:IACMRadDateTimePicker ID="DatePicker" runat="server" DateTimeFormat="Date" Skin="IACMSkin" Width="100%" CssClass="form-control">
                                </cc1:IACMRadDateTimePicker>
                            </div>
                        </div>
                        <div class="form-group" style="display: none">
                            <asp:Label ID="Label1" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="UserSearchUC">User</asp:Label>
                            <div class="col-xs-3">
                                <%--<asp:DropDownList runat="server" ID="User" CssClass="form-control selectpicker" data-width="100%" AutoPostBack="true">
                                </asp:DropDownList>--%>
                                <uc1:UserSearchUC runat="server" ID="UserSearchUC" />
                            </div>
                            <div class="col-xs-1"></div>
                            <asp:Label ID="Label4" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="Filter"></asp:Label>

                        </div>
                        <div class="form-group">
                            <div class="col-xs-4">
                                <h4>Table List</h4>
                            </div>

                            <div class="col-xs-6">
                                <h4>
                                    <asp:Label runat="server" ID="HeaderItem"></asp:Label>
                                    Table History</h4>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-xs-4">
                                <telerik:RadTreeView RenderMode="Lightweight" runat="Server" ID="RadTreeView" Height="444px" OnNodeClick="RadTreeView_NodeClick">
                                </telerik:RadTreeView>
                            </div>
                            <div class="col-xs-8">
                                <cc1:IACMRadGridControl ID="AuditHistoryGrid" runat="server" OnNeedDataSource="AuditHistoryGrid_NeedDataSource" OnItemCommand="AuditHistoryGrid_ItemCommand">
                                    <MasterTableView TableLayout="fixed" DataKeyNames="AuditLogId"  Width="100%" >
                                        <Columns>
                                            <telerik:GridButtonColumn DataTextField="AuditLogId" UniqueName="AuditLogId" CommandArgument="AuditLog" CommandName="AuditLog" ButtonType="LinkButton" FilterControlAltText="Filter column" HeaderText="Id" Text="Id" HeaderStyle-Width="12%" FilterControlWidth="48%">
                                            </telerik:GridButtonColumn>
                                            <%--<telerik:GridHyperLinkColumn FilterControlAltText="AuditLogId" HeaderText="Id" DataTextField="AuditLogId" SortExpression="AuditLogId" UniqueName="AuditLogId" HeaderStyle-Width="12%" FilterControlWidth="48%"
                                            ></telerik:GridHyperLinkColumn>--%>
                                            <%--<telerik:GridTemplateColumn UniqueName="AuditLog" HeaderText="Id" HeaderStyle-Width="12%" FilterControlWidth="48%">
                                                <ItemTemplate>
                                                    <asp:HyperLink runat="server" ID="AuditLog" Text='<%# Eval("AuditLogId") %>' CommandName="AuditLog"></asp:HyperLink>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>--%>
                                            <telerik:GridDateTimeColumn DataField="EventDateUTC" FilterControlAltText="EventDateUTC" HeaderText="AuditDate" SortExpression="EventDateUTC" UniqueName="EventDateUTC" HeaderStyle-Width="22.5%" FilterControlWidth="73.5%">
                                            </telerik:GridDateTimeColumn>
                                            <telerik:GridBoundColumn DataField="UserName" FilterControlAltText="UserName" HeaderText="UserName" SortExpression="UserName" UniqueName="UserName" HeaderStyle-Width="25%" FilterControlWidth="76%">
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn DataField="EventType" FilterControlAltText="EventType" HeaderText="EventType" SortExpression="EventType" UniqueName="EventType" HeaderStyle-Width="22.5%" FilterControlWidth="74%">
                                            </telerik:GridBoundColumn>
                                            <telerik:GridButtonColumn DataTextField="RecordId" UniqueName="RecordId" CommandArgument="Record" CommandName="Record" ButtonType="LinkButton" FilterControlAltText="Filter column" HeaderText="Record Details" Text="RecordId" HeaderStyle-Width="12%" FilterControlWidth="48%">
                                            </telerik:GridButtonColumn>
                                        </Columns>
                                    </MasterTableView>
                                </cc1:IACMRadGridControl>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <telerik:RadWindow runat="server" ID="DetailWindow" Modal="true" Height="300px" Width="500px" Title="AuditLog Detail"
        RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
        <ContentTemplate>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="modal-header">
                        <h3 class="modal-title" runat="server" id="H1">AuditLog Detail</h3>
                    </div>
                    <div class="modal-body">
                        <cc1:IACMRadGridControl ID="AuditLogDetailGrid" runat="server">
                            <MasterTableView TableLayout="fixed" Width="100%" >
                                <Columns>
                                    <telerik:GridBoundColumn DataField="PropertyName" FilterControlAltText="PropertyName" HeaderText="PropertyName" SortExpression="PropertyName" UniqueName="PropertyName" HeaderStyle-Width="22.5%" FilterControlWidth="63.5%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="OriginalValue" FilterControlAltText="OriginalValue" HeaderText="OriginalValue" SortExpression="OriginalValue" UniqueName="OriginalValue" HeaderStyle-Width="25%" FilterControlWidth="63.5%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="NewValue" FilterControlAltText="NewValue" HeaderText="NewValue" SortExpression="NewValue" UniqueName="NewValue" HeaderStyle-Width="22.5%" FilterControlWidth="63.5%">
                                    </telerik:GridBoundColumn>
                                </Columns>
                            </MasterTableView>
                        </cc1:IACMRadGridControl>
                    </div>
                    <div class="modal-footer">
                        <asp:Button runat="server" class="btn btn-primary transparent-button" ID="btnok" Text="Ok" OnClick="btnok_Click"></asp:Button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadWindow runat="server" ID="RecordDetailWindow" Modal="true" Height="300px" Width="800px" Title="AuditLog Detail"
        RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
        <ContentTemplate>
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <div class="modal-header">
                        <h3 class="modal-title" runat="server" id="H2">AuditLog Record Detail</h3>
                    </div>
                    <div class="modal-body">
                        <telerik:RadGrid ID="dynamicGrid" runat="server" ClientSettings-Scrolling-AllowScroll="true" Width="100%" OnNeedDataSource="dynamicGrid_NeedDataSource"></telerik:RadGrid>
                    </div>
                    <div class="modal-footer">
                        <asp:Button runat="server" class="btn btn-primary transparent-button" ID="RecordOk" Text="Ok" OnClick="RecordOk_Click"></asp:Button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
</asp:Content>
