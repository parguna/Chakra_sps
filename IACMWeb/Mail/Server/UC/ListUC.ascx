<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Mail.Server.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-wrapper">
    <telerik:RadWindow ID="modalPopup" runat="server" Width="360px" Height="180px" Modal="true" OffsetElementID="main" VisibleTitlebar="false" VisibleStatusbar="false">
        <ContentTemplate>
            <div class="modal-header">
                <h3 class="modal-title" id="H1">Send Test Mail</h3>
            </div>
            <div class="modal-body">
                <div class="form-horizontal left clearfix" id="configkey">
                    <div class="row-fluid">
                        <div class="col-md-12">
                            <div class="form-group">
                                <asp:Label ID="Label1" AssociatedControlID="MailTo" runat="server" Text="Mail To" CssClass="col-sm-3 control-label"></asp:Label>
                                <div class="col-sm-9">
                                    <asp:TextBox ID="MailTo" CssClass="form-control" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <asp:Button runat="server" ID="btnSubmit" CssClass="add btn btn-primary bordered-button" Text="Send Mail" OnClick="btnSubmit_Click" />
            </div>
        </ContentTemplate>
    </telerik:RadWindow>
    <cc1:IACMRadGridControl ID="grdMailServer" runat="server"  OnNeedDataSource="grdMailServer_NeedDataSource"  OnSelectedIndexChanged="grdMailServer_SelectedIndexChanged">
        <MasterTableView  DataKeyNames="Id" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available" TableLayout="Fixed" Width="100%">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="EmailID" FilterControlAltText="EmailID" HeaderText="Email ID" SortExpression="EmailID" UniqueName="EmailID" HeaderStyle-Width="30%" FilterControlWidth="81%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="UserName" FilterControlAltText="UserName" HeaderText="User Name" SortExpression="UserName" UniqueName="UserName" FilterControlWidth="77%" HeaderStyle-Width="25%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="HostName" FilterControlAltText="HostName" HeaderText="Host Name" SortExpression="HostName" UniqueName="HostName" FilterControlWidth="77%" HeaderStyle-Width="25%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="PortNumber" FilterControlAltText="PortNumber" HeaderText="Port Number" SortExpression="PortNumber" UniqueName="PortNumber" FilterControlWidth="77%" HeaderStyle-Width="25%">
                </telerik:GridBoundColumn>
                 
                <telerik:GridTemplateColumn DataField="EnableSSL" AllowFiltering="false" HeaderText="Enable SSL" SortExpression="EnableSSL" UniqueName="EnableSSL" HeaderStyle-Width="20%">
                    <ItemTemplate>
                    
                        <asp:Label ID="EnableSSL" runat="server" Text='<%#Convert.ToBoolean( Eval("EnableSSL")) == true ? "Yes" : "No" %>'></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
  </cc1:IACMRadGridControl>

    <div class="buttons text-right nav-rectangle">
        <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClientClick="Redirect()"></asp:Button>
        <asp:Button ID="SendTestMail" runat="server" CssClass="btn btn-default btn-sm" Text="Send Test Mail" OnClick="SendTestMail_Click" />
    </div>
</div>
