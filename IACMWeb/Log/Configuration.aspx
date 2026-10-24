<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Master/ProtoMast4.master" CodeBehind="Configuration.aspx.cs" Inherits="DI.Chakra.Web.Log.Configuration" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <div class="content-wrapper">
                <div class="form">
                    <div class="form-horizontal left">
                        <div class="row-fluid">
                            <div class="col-xs-12">
                                <div class="form-group">
                                    <asp:Label ID="Label1" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="TableName">TableName</asp:Label>
                                    <div class="col-xs-4">
                                        <asp:TextBox runat="server" ID="TableName" TabIndex="1" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="col-xs-4">
                                        <asp:Button runat="server" ID="Filter" TabIndex="6" CssClass="btn btn-default btn-sm" Text="Filter" OnClick="Filter_Click" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-xs-2">
                                        <h4>Table List</h4>
                                    </div>
                                    <div class="col-xs-2">
                                        <asp:Label ID="Label2" runat="server" CssClass="control-label" AssociatedControlID="CheckIsAuditEnabled">
                                            <asp:CheckBox runat="server" ID="CheckIsAuditEnabled" AutoPostBack="true" OnCheckedChanged="CheckIsAuditEnabled_CheckedChanged"/>
                                            <span class="lbl">Audit Enabled</span>
                                        </asp:Label>
                                    </div>
                                    <div class="col-xs-6">
                                        <h4><asp:Label ID="TableNameHeader" runat="server"></asp:Label> Properties</h4>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-xs-4">
                                        <telerik:RadTreeView RenderMode="Lightweight" runat="Server" ID="RadTreeView" Height="444px"
                                            OnNodeClick="RadTreeView_NodeClick">
                                        </telerik:RadTreeView>
                                    </div>
                                    <div class="col-xs-8">
                                        <cc1:IACMRadGridControl ID="AuditConfigurationGrid" runat="server" AllowMultiRowSelection="true" OnPreRender="AuditConfigurationGrid_PreRender" OnNeedDataSource="AuditConfigurationGrid_NeedDataSource" Width="70%">
                                            <MasterTableView DataKeyNames="ColumnName" ShowHeadersWhenNoRecords="true" TableLayout="fixed" NoMasterRecordsText="No Records Available" Width="100%" >
                                                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                                                <Columns>
                                                    <telerik:GridTemplateColumn UniqueName="CheckTemp" HeaderText="CheckTemp" HeaderStyle-Width="3%" AllowFiltering="false" AllowSorting="false">
                                                        <HeaderTemplate>
                                                            <asp:CheckBox ID="CheckBox" AutoPostBack="true" runat="server" OnCheckedChanged="CheckBox_CheckedChanged" />
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="ClientSelectColumn" runat="server" AutoPostBack="true" OnCheckedChanged="ClientSelectColumn_CheckedChanged"/>
                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridBoundColumn DataField="ColumnName" FilterControlAltText="ColumnName" HeaderText="Fields" SortExpression="ColumnName" UniqueName="ColumnName" HeaderStyle-Width="30%" FilterControlWidth="85%">
                                                    </telerik:GridBoundColumn>
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
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
