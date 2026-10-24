<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Mail.Template.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <cc1:IACMRadGridControl ID="MailRadGrid" runat="server" OnNeedDataSource="MailRadGrid_NeedDataSource" OnSelectedIndexChanged="MailRadGrid_SelectedIndexChanged">
            <MasterTableView  DataKeyNames="Id" NoMasterRecordsText="No Records Available" ShowHeadersWhenNoRecords="true" TableLayout="Fixed" Width="100%">
                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                <Columns>
                    <telerik:GridBoundColumn DataField="TemplateName" FilterControlAltText="Filter TemplateName column" HeaderText="Template Name" SortExpression="TemplateName" UniqueName="TemplateName" HeaderStyle-Width="25%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="TemplateDescription" FilterControlAltText="Filter TemplateDescription column" HeaderText="Description" SortExpression="TemplateDescription" UniqueName="TemplateDescription" HeaderStyle-Width="25%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="MailFrom" HeaderText="From" SortExpression="MailFrom" UniqueName="MailFrom" HeaderStyle-Width="25%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="MailSubject" HeaderText="Subject" SortExpression="MailSubject" UniqueName="MailSubject" HeaderStyle-Width="25%" >
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
       </cc1:IACMRadGridControl>
    </ContentTemplate>
</asp:UpdatePanel>
<%--<div class="buttons text-right nav-rectangle">
    <asp:Button ID="btncancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="btncancel_Click" />
</div>--%>