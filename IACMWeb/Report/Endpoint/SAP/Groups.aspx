<%@ Page Title="SAP Groups" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Groups.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.SAP.Groups" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="EndpointSystem" TagName="EndpointUC" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">

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
    <cc1:IACMRadGridControl ID="grdGroups" runat="server" OnNeedDataSource="grdGroups_NeedDataSource">
<MasterTableView  TableLayout="fixed"
            DataKeyNames="Id" EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
            <CommandItemSettings ShowAddNewRecordButton="false"
                ShowRefreshButton="true"></CommandItemSettings>
<Columns>
<telerik:GridBoundColumn DataField="GROUP_CODE" FilterControlAltText="GROUP_CODE" HeaderText="Group Code" SortExpression="GROUP_CODE" UniqueName="GROUP_CODE" HeaderStyle-Width="25%" FilterControlWidth="85%">
<ColumnValidationSettings>
<ModelErrorMessage Text="" />
</ColumnValidationSettings>
</telerik:GridBoundColumn>
<telerik:GridBoundColumn DataField="GROUP_DESC" FilterControlAltText="GROUP_DESC" HeaderText="Group Description" SortExpression="GROUP_DESC" UniqueName="GROUP_DESC" HeaderStyle-Width="25%" FilterControlWidth="85%">
<ColumnValidationSettings>
<ModelErrorMessage Text="" />
</ColumnValidationSettings>
</telerik:GridBoundColumn>

</Columns>
</MasterTableView>
</cc1:IACMRadGridControl>     
</asp:Panel>
</asp:Content>
