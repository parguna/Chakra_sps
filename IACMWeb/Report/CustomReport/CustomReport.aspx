<%@ Page Title="Custom Report" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="CustomReport.aspx.cs" Inherits="DI.Chakra.Web.Report.CustomReport.CustomReport" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:Panel ID="Panel1" runat="server">
        <cc1:IACMRadGridControl ID="RadReportJobs" runat="server" OnNeedDataSource="RadReportJobs_NeedDataSource" EnableLinqExpressions="false"
            OnSelectedIndexChanged="RadReportJobs_SelectedIndexChanged" OnItemCommand="RadReportJobs_ItemCommand">
            <MasterTableView DataKeyNames="Id">
                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>
                <Columns>
                    <telerik:GridBoundColumn DataField="Name" HeaderText="Name" SortExpression="Name" UniqueName="Name" DataType="System.String" FilterControlWidth="85%" HeaderStyle-Width="20%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Description" HeaderText="Description" SortExpression="Description" UniqueName="Description" DataType="System.String" FilterControlWidth="76.5%" HeaderStyle-Width="30%">
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn DataField="LastRunOn" HeaderText="Last Run DateTime" AllowSorting="True" UniqueName="LastRunOn" AllowFiltering="true" DataFormatString="{0:dd-MM-yyyy hh:mm:ss tt}" DataType="System.Datetime" FilterControlWidth="80.5%" HeaderStyle-Width="10%">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridTemplateColumn AllowFiltering="false" HeaderText="Attachment" HeaderStyle-Width="5%">
                        <ItemTemplate>
                            <asp:UpdatePanel runat="server" ID="UpdatePanelDownload" UpdateMode="Conditional" ChildrenAsTriggers="True" OnUnload="UpdatePanelDownload_Unload">
                                <ContentTemplate>
                                    <asp:ImageButton ID="AttachmentImage" ImageUrl="~/Images/action-buttons/Attachment.png" runat="server" CommandName="file" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:PostBackTrigger ControlID="AttachmentImage" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </ItemTemplate>
                        <ItemStyle CssClass="action-attachment" HorizontalAlign="Center" />
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </cc1:IACMRadGridControl>

    </asp:Panel>
</asp:Content>
