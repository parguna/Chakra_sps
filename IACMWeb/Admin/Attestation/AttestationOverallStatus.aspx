<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AttestationOverallStatus.aspx.cs" MasterPageFile="~/Master/ProtoMast4.master" Inherits="DI.Chakra.Web.Admin.Attestation.AttestationOverallStatus" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%--<%@ Register Src="~/UsrControl/Common/ReportScheduler.ascx" TagPrefix="ReportScheduler" TagName="RptSchd" %>--%>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Src="~/Report/Attestation/UC/AttestationCampaignDetailUC.ascx" TagPrefix="AttestationCampaignDetailUC" TagName="AttestationCampaignDetailUC" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
   <style>
    .HoverPopup
        {
            background-color: mediumseagreen;
            color: White;
            padding: 5px;
            display:none;
            border-radius: 30px;
        }
    .HoverPopupltTwoDays
        {
            background-color: #b23d12;
            color: White;
            padding: 5px;
            display:none;
            border-radius: 30px;
        }
    .HoverPopupltTenDays
        {
            background-color: orange;
            color: White;
            padding: 5px;
            display:none;
            border-radius: 30px;
        }
</style>
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

    <telerik:RadWindow runat="server" ID="CampaignDetail" Modal="true" Width="600px" Height="100%" Title="Attestation Campaign Detail"
        RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
        <ContentTemplate>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="modal-header">
                        <h3 class="modal-title">Attestation Campaign Detail</h3>
                    </div>
                    <div class="modal-body">
                        <AttestationCampaignDetailUC:AttestationCampaignDetailUC runat="server" ID="AttestationCampaignDetailUC" />
                    </div>
                    <div class="modal-footer">
                        <asp:Button runat="server" ID="btnDetailCancel" class="btn btn-primary bordered-button" Text="OK" OnClick="btnDetailCancel_Click" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

    <asp:Panel ID="Panel1" runat="server">
        <asp:UpdatePanel ID="upnl" runat="server" UpdateMode="Always"> 
            <ContentTemplate>

            
        <cc1:IACMRadGridControl ID="CampaignStatusGrid" runat="server" OnItemCommand="CampaignStatusGrid_ItemCommand" PageSize="50" OnItemDataBound="CampaignStatusGrid_ItemDataBound" OnSelectedIndexChanged="CampaignStatusGrid_SelectedIndexChanged" OnNeedDataSource="CampaignStatusGrid_NeedDataSource" Width="100%">
            <ClientSettings Selecting-AllowRowSelect="true" EnablePostBackOnRowClick="true" >
            </ClientSettings>
            <MasterTableView TableLayout="fixed" PageSize="50"
                DataKeyNames="AttestationCampaignId" EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available" Width="100%">
                <CommandItemSettings ShowAddNewRecordButton="false"
                    ShowRefreshButton="true"></CommandItemSettings>
                <Columns>
                    <telerik:GridBoundColumn SortExpression="AttestationName" HeaderText="Attestation Name" HeaderButtonType="TextButton" DataField="AttestationName" HeaderStyle-Width="20%" FilterControlWidth="70%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="AttestationType" HeaderText="Type" HeaderButtonType="TextButton" DataField="AttestationType" HeaderStyle-Width="8%" FilterControlWidth="70%" Display="false">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="CampaignName" HeaderText="Campaign Name" HeaderButtonType="TextButton" DataField="CampaignName" HeaderStyle-Width="20%" FilterControlWidth="70%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="Description" HeaderText="Description" HeaderButtonType="TextButton" DataField="Description" HeaderStyle-Width="20%" FilterControlWidth="70%">
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn SortExpression="CreatedDate" DataFormatString="{0:dd MMM yyyy hh:mm tt}" HeaderText="Start Date" HeaderButtonType="TextButton" DataField="CreatedDate" HeaderStyle-Width="15%">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn SortExpression="CreatedBy" Visible="false" HeaderText="Created By" HeaderButtonType="TextButton" DataField="CreatedBy" HeaderStyle-Width="20%" FilterControlWidth="60%">
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn SortExpression="CloseDate" DataFormatString="{0:dd MMM yyyy hh:mm tt}" HeaderText="Close Date" HeaderButtonType="TextButton" DataField="CloseDate" HeaderStyle-Width="15%">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn SortExpression="OverallStatus" HeaderText="Overall Status" HeaderButtonType="TextButton" DataField="OverallStatus" UniqueName="OverallStatus" HeaderStyle-Width="20%" AllowFiltering="true">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="CampaignSplit" HeaderText="Campaign Split" HeaderButtonType="TextButton" DataField="CampaignSplit" HeaderStyle-Width="8%" AllowFiltering="false">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="RequiresAttestationRemediation" HeaderText="Remediation" HeaderButtonType="TextButton" DataField="RequiresAttestationRemediation" HeaderStyle-Width="100px" AllowFiltering="false">
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn UniqueName="TemplateStatusColumn" HeaderStyle-Width="7%" AllowFiltering="false" HeaderText="Status">
                        <ItemTemplate>
                            <asp:ImageButton ID="StatusInfo" runat="server" CommandName="StatusInfo"></asp:ImageButton>
                            <asp:HoverMenuExtender ID="hoverCompletionMessage" runat="server" Enabled="true" PopupControlID="pnlDescription" PopupPosition="left" TargetControlID="StatusInfo"></asp:HoverMenuExtender>
                                    <asp:Panel ID="pnlDescription" runat="server" CssClass="HoverPopup" Width="220px">
                                        <asp:Label ID="lblCompletionMessage" runat="server"></asp:Label>
                                    </asp:Panel>
                        </ItemTemplate>
                        <ItemStyle CssClass="status-green-info" HorizontalAlign="Center" />
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="TemplateInfoColumn" HeaderStyle-Width="7%" AllowFiltering="false" HeaderText="Detail" Display="false">
                        <ItemTemplate>
                            <asp:ImageButton ID="ShowIcon" runat="server" CommandName="ShowInfo" src="../../Images/action-buttons/detail.png"></asp:ImageButton>
                        </ItemTemplate>
                        <ItemStyle CssClass="action-info" HorizontalAlign="Center" />
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </cc1:IACMRadGridControl>
                </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="RefreshButton" />
            </Triggers>
        </asp:UpdatePanel>
    </asp:Panel>
    <div class="buttons text-right nav-rectangle topBuffer">
        <asp:Button ID="RefreshButton" CssClass="btn btn-default btn-sm" runat="server" Text="Refresh" OnClick="RefreshButton_Click" /> &nbsp;<asp:Button ID="btnPageCancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" PostBackUrl="~/Home/Home.aspx" UseSubmitBehavior="false" OnClick="btnPageCancel_Click" />
    </div>
    <telerik:RadWindow runat="server" ID="ConfirmTermination" Modal="true" Width="550px" Height="100%"
        RenderMode="Lightweight" Behaviors="Close" VisibleOnPageLoad="false"  EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
        <ContentTemplate>
            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                <ContentTemplate>
                    <div class="modal-header">
                        <h3 class="modal-title">Terminate Attestation Confirmation</h3>
                    </div>
                    <div class="modal-body">
                        <div class="form-horizontal left clearfix">
                        <div class="row-fluid">
                            <div class="col-xs-12">
                                <div class="form-group">
                                    <label class="col-xs-12 control-label">Are you sure you want to terminate the selected attestation campaign? </label>
                                </div>
                                <div class="form-group required">
                                    <label class="col-xs-3 control-label">Reason: </label>
                                    <div class="col-xs-9">
                                        <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" CssClass="form-control" Rows="5"></asp:TextBox>
                                    </div>
                                    <div class="col-xs-3">&nbsp;</div>
                                    <div class="col-xs-9">
                                        <asp:Label runat="server" ID="lblMsg" ForeColor="Red"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button runat="server" ID="btnCancelAttestationRejection" Text="Cancel" UseSubmitBehavior="false" OnClick="btnCancelSubmit_Click"/>
                        <asp:Button runat="server" ID="btnAttestationConfirmRejectionSubmit" Text="OK" OnClick="btnConfirmSubmit_Click" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
</asp:Content>

