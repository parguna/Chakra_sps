<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Admin.TrustedSrcConfig.UC.ListUC" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Admin/TrustedSrcConfig/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>

<cc1:IACMRadGridControl ID="grdTrsConfig" runat="server" OnNeedDataSource="grdTrsConfig_NeedDataSource" OnSelectedIndexChanged="grdTrsConfig_SelectedIndexChanged">
    <MasterTableView NoMasterRecordsText="No Data Found" DataKeyNames="Id"  Width="100%" >
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn DataField="AttributeName" HeaderText="Attribute Name" SortExpression="AttributeName" UniqueName="AttributeName" HeaderStyle-Width="25%" FilterControlWidth="81.5%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="TrustedSource" HeaderText="Trusted Source" SortExpression="TrustedSource" UniqueName="TrustedSource" HeaderStyle-Width="25%" FilterControlWidth="81.5%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="TrustedSourceAttribute" HeaderText="Trusted Source Attribute" SortExpression="TrustedSourceAttribute" UniqueName="TrustedSourceAttribute" HeaderStyle-Width="25%" FilterControlWidth="81.5%">
            </telerik:GridBoundColumn>
            <telerik:GridCheckBoxColumn DataField="SelfUpdate" HeaderText="Self Update" SortExpression="SelfUpdate" AllowFiltering="false" UniqueName="SelfUpdate" HeaderStyle-Width="25%">
            </telerik:GridCheckBoxColumn>
        </Columns>
    </MasterTableView>
</cc1:IACMRadGridControl>

<telerik:RadWindow ID="modalPopup" runat="server" Width="600px" Height="230px" Modal="true" OffsetElementID="main" VisibleStatusbar="false" VisibleTitlebar="false">
    <ContentTemplate>
        <uc1:DetailUC runat="server" ID="DetailUC" OnAdd_CancelButtonClicked="btnCancel_Click" OnAdd_SubmitButtonClicked="btnSubmit_Click" />
    </ContentTemplate>
</telerik:RadWindow>