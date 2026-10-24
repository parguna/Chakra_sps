<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.ConfigKey.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Admin/ConfigKey/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>

<telerik:RadWindow ID="modalPopup" runat="server" Width="420px" Height="220px" Modal="true" OffsetElementID="main" VisibleTitlebar="false" VisibleStatusbar="False"
    RenderMode="Lightweight" EnableViewState="true" >
    <ContentTemplate>
        <uc1:DetailUC runat="server" ID="ConfigDetailUC" OnAdd_CancelButtonClicked="btnCancel_Click" OnAdd_SubmitButtonClicked="btnSubmit_Click" />
    </ContentTemplate>
</telerik:RadWindow>
<cc1:IACMRadGridControl ID="ConfigurationMaster" runat="server"  OnNeedDataSource="ConfigurationMaster_NeedDataSource" OnItemDataBound="ConfigurationMaster_ItemDataBound" OnSelectedIndexChanged="ConfigurationMaster_SelectedIndexChanged">
    <MasterTableView  DataKeyNames="Id" TableLayout="Fixed" Width="100%"
        NoMasterRecordsText="No Records Available">
        <ItemStyle Wrap="true" />
        <CommandItemSettings ShowAddNewRecordButton="false"
            ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn DataField="ConfigKey"
                FilterControlAltText="ConfigKey" HeaderText="Config Key"
                SortExpression="ConfigKey" UniqueName="ConfigKey"
                HeaderStyle-Width="45%" FilterControlWidth="70%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderStyle-Width="40%" HeaderText="Config Value"
                DataField="ConfigValue" FilterControlAltText="ConfigValue" SortExpression="ConfigValue"
                UniqueName="ConfigValue" FilterControlWidth="70%">
            </telerik:GridBoundColumn>
             <telerik:GridBoundColumn HeaderStyle-Width="40%" HeaderText="Description"
                DataField="Description" FilterControlAltText="Description" SortExpression="Description"
                UniqueName="Description" FilterControlWidth="70%">
            </telerik:GridBoundColumn>
        </Columns>
    </MasterTableView>
</cc1:IACMRadGridControl>
