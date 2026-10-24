<%@ Page Title="SAP Profile" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Profiles.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.SAP.Profiles" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="EndpointSystem" TagName="EndpointUC" %>



<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="Server">

    <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left clearfix">
                <div class="row-fluid">
                    <div class="col-xs-7">
                        <endpointsystem:endpointuc runat="server" id="EndpointUC" onendpointchanged="DropDownSelectectionChanged" displaycontrol="Application" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:Panel ID="Panel1" runat="server">
        <cc1:IACMRadGridControl ID="grdProfiles" runat="server" OnNeedDataSource="grdProfiles_NeedDataSource">
<MasterTableView  TableLayout="fixed"
DataKeyNames="SAP_PROFILEID" EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
<CommandItemSettings ShowAddNewRecordButton="false"
    ShowRefreshButton="true"></CommandItemSettings>
<Columns>
        <telerik:GridBoundColumn DataField="PROFILE_CODE" FilterControlAltText="PROFILE_CODE" HeaderText="Profile Code" SortExpression="PROFILE_CODE" UniqueName="PROFILE_CODE" HeaderStyle-Width="25%" FilterControlWidth="85%">
            <ColumnValidationSettings>
                <ModelErrorMessage Text="" />
            </ColumnValidationSettings>
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="PROFILE_DESC" FilterControlAltText="PROFILE_DESC" HeaderText="Profile Description" SortExpression="PROFILE_DESC" UniqueName="PROFILE_DESC" HeaderStyle-Width="25%" FilterControlWidth="85%">
            <ColumnValidationSettings>
                <ModelErrorMessage Text="" />
            </ColumnValidationSettings>
        </telerik:GridBoundColumn>

    </Columns>
</MasterTableView>
</cc1:IACMRadGridControl>   
</asp:Panel>
</asp:Content>

