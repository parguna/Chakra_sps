<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ActiveAttestationCampaign.aspx.cs" MasterPageFile="~/Master/ProtoMast4.master" Inherits="DI.Chakra.Web.Admin.Attestation.ActiveAttestationCampaign" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/ReportScheduler.ascx" TagPrefix="ReportScheduler" TagName="RptSchd" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Src="~/Report/Attestation/UC/AttestationCampaignDetailUC.ascx" TagPrefix="AttestationCampaignDetailUC" TagName="AttestationCampaignDetailUC" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">

    <telerik:RadWindow runat="server" Modal="true" ID="usrModal" Width="1050px" Height="100%" Title="Cart Items"
        RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
        <ContentTemplate>
            <div class="modal-header">
                <h3 class="modal-title" id="H1">Schedule Report</h3>
            </div>
            <div class="modal-body">
                <ReportScheduler:RptSchd ID="RptSchd" runat="server" />
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
                    <AttestationCampaignDetailUC:AttestationCampaignDetailUC runat="server" id="AttestationCampaignDetailUC" />
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnDetailCancel" class="btn btn-primary bordered-button" Text="OK" OnClick="btnDetailCancel_Click" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>

    <asp:Panel ID="Panel1" runat="server">

        <cc1:IACMRadGridControl ID="CampaignStatusGrid" runat="server" OnItemCommand="CampaignStatusGrid_ItemCommand" OnItemDataBound="CampaignStatusGrid_ItemDataBound" OnSelectedIndexChanged="CampaignStatusGrid_SelectedIndexChanged" OnNeedDataSource="CampaignStatusGrid_NeedDataSource">
            <ClientSettings Selecting-AllowRowSelect="true" EnablePostBackOnRowClick="true">
            </ClientSettings>
            <MasterTableView TableLayout="fixed"
                DataKeyNames="AttestationCampaignId" EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
                <CommandItemSettings ShowAddNewRecordButton="false"
                    ShowRefreshButton="true"></CommandItemSettings>
                <Columns>
                    <telerik:GridBoundColumn SortExpression="AttestationName" HeaderText="Attestation Name" HeaderButtonType="TextButton" DataField="AttestationName" HeaderStyle-Width="30%" FilterControlWidth="70%" >
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="CampaignName" HeaderText="Campaign Name" HeaderButtonType="TextButton" DataField="CampaignName" HeaderStyle-Width="30%" FilterControlWidth="70%" >
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="Description" HeaderText="Description" HeaderButtonType="TextButton" DataField="Description" HeaderStyle-Width="30%" FilterControlWidth="70%" >
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="CreatedDate" HeaderText="Start Date" HeaderButtonType="TextButton" DataField="CreatedDate" HeaderStyle-Width="10%" FilterControlWidth="50%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="CreatedBy" Visible="false" HeaderText="Created By" HeaderButtonType="TextButton" DataField="CreatedBy" HeaderStyle-Width="20%" FilterControlWidth="60%">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="CloseDate" HeaderText="Close Date" HeaderButtonType="TextButton" DataField="CloseDate" HeaderStyle-Width="10%" FilterControlWidth="50%">
                    </telerik:GridBoundColumn>                    
                    <telerik:GridBoundColumn SortExpression="Level" HeaderText="Level" HeaderButtonType="TextButton" DataField="Level" HeaderStyle-Width="7%" AllowFiltering="false">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="TotalItem" HeaderText="Outstanding Task" HeaderButtonType="TextButton" DataField="TotalItem" HeaderStyle-Width="12%" AllowFiltering="false">
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn UniqueName="TemplateStatusColumn" HeaderStyle-Width="7%" AllowFiltering="false" HeaderText="Status">
                        <ItemTemplate>
                            <asp:ImageButton ID="StatusInfo" runat="server" CommandName="StatusInfo"></asp:ImageButton>
                        </ItemTemplate>
                        <ItemStyle CssClass="status-green-info" HorizontalAlign="Center" />
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="TemplateInfoColumn" HeaderStyle-Width="7%" AllowFiltering="false" HeaderText="Detail">
                        <ItemTemplate>
                            <asp:ImageButton ID="ShowIcon" runat="server" CommandName="ShowInfo" src="../../Images/action-buttons/detail.png"></asp:ImageButton>
                        </ItemTemplate>
                        <ItemStyle CssClass="action-info" HorizontalAlign="Center" />
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </cc1:IACMRadGridControl>
    </asp:Panel>
    <div class="buttons text-right nav-rectangle topBuffer">
        <asp:Button ID="btnPageCancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" PostBackUrl="~/Home/Home.aspx" UseSubmitBehavior="false" OnClick="btnPageCancel_Click" />
    </div>
</asp:Content>


