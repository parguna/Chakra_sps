<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Master/ProtoMast4.master" CodeBehind="AttestationCampaigntaskDetail.aspx.cs" Inherits="DI.Chakra.Web.Report.Attestation.AttestationCampaigntaskDetail" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">

    <telerik:RadWindow runat="server" Modal="true" ID="usrModal" Width="1050px" Height="100%" Title="Cart Items"
        RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
        <ContentTemplate>
            <div class="modal-header">
                <h3 class="modal-title" id="H1">Schedule Report</h3>
            </div>
            <div class="modal-body">
               <%-- <ReportScheduler:RptSchd ID="RptSchd" runat="server" />--%>
            </div>
            <div class="modal-footer">
                <asp:Button runat="server" class="btn btn-primary transparent-button" ID="btnSave" Text="Save" OnClick="btnSave_Click" />
                <asp:Button runat="server" class="btn btn-primary transparent-button" ID="btnCancel" Text="Cancel" OnClick="btnCancel_Click" />
            </div>
        </ContentTemplate>
    </telerik:RadWindow>

    <asp:Panel ID="Panel1" runat="server">

        <cc1:IACMRadGridControl ID="AttestationCampaignTaskGrid" PageSize="50" runat="server" OnNeedDataSource="AttestationCampaignTaskGrid_NeedDataSource" OnSelectedIndexChanged="AttestationCampaignTaskGrid_SelectedIndexChanged" Width="100%">
            <MasterTableView DataKeyNames="TaskId" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available"  Width="100%"  PageSize="50">
                <CommandItemSettings ShowAddNewRecordButton="false"
                    ShowRefreshButton="true"></CommandItemSettings>
                <Columns>
                    <telerik:GridBoundColumn SortExpression="AttestationName" HeaderText="AttestationName" HeaderButtonType="textbutton" AllowFiltering="false" DataField="AttestationName" HeaderStyle-Width="20%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="CampaignName" HeaderText="CampaignName" HeaderButtonType="textbutton" AllowFiltering="false" DataField="CampaignName" HeaderStyle-Width="20%">
                    </telerik:GridBoundColumn>
                   <%-- <telerik:GridBoundColumn SortExpression="TaskId" HeaderText="TaskId" HeaderButtonType="textbutton" FilterControlWidth="40%" DataField="TaskId" HeaderStyle-Width="10%">
                    </telerik:GridBoundColumn>--%>
                     <telerik:GridTemplateColumn SortExpression="TaskId" HeaderText="TaskId" HeaderButtonType="textbutton" FilterControlWidth="40%" DataField="TaskId" HeaderStyle-Width="10%">
                          <ItemTemplate>
                        <asp:LinkButton ID="lnkTaskId" runat="server" Text='<%#Eval("TaskId") %>' CommandArgument='<%#Container.ItemIndex%>' OnCommand="lnkTaskId_Command" />
                    </ItemTemplate>
                     </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn SortExpression="Level" HeaderText="Level" HeaderButtonType="textbutton" DataField="Level" FilterControlWidth="40%" HeaderStyle-Width="10%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="Approver" HeaderText="Reviewer Type" HeaderButtonType="textbutton" AllowFiltering="false" DataField="Approver" HeaderStyle-Width="15%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="UserName" HeaderText="Reviewer" HeaderButtonType="textbutton" DataField="UserName" HeaderStyle-Width="20%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="Action" HeaderText="Status" HeaderButtonType="textbutton" DataField="Action" FilterControlWidth="40%" HeaderStyle-Width="10%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="IsCompleted" Visible="false" HeaderText="Completed" HeaderButtonType="textbutton" DataField="IsCompleted" HeaderStyle-Width="20%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="AttestationCampaignId" Visible="false" HeaderText="AttestationCampaignId" HeaderButtonType="textbutton" DataField="AttestationCampaignId" HeaderStyle-Width="20%">
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn SortExpression="RequestedDate" HeaderText="Requested Date" HeaderButtonType="textbutton" DataField="RequestedDate" FilterControlWidth="62%" HeaderStyle-Width="10%">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridDateTimeColumn SortExpression="SubmittedDate" HeaderText="Submitted Date" HeaderButtonType="textbutton" DataField="SubmittedDate" FilterControlWidth="62%" HeaderStyle-Width="10%">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridDateTimeColumn SortExpression="EndDate" HeaderText="Close Date" HeaderButtonType="textbutton" DataField="EndDate" FilterControlWidth="62%" HeaderStyle-Width="10%">
                    </telerik:GridDateTimeColumn>
                </Columns>

            </MasterTableView>
        </cc1:IACMRadGridControl>
    </asp:Panel>

    <div class="buttons text-right nav-rectangle topBuffer">
        <asp:Button ID="btnPageCancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" UseSubmitBehavior="false" OnClick="btnPageCancel_Click" />
    </div>
</asp:Content>


