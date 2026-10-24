<%@ Page Title="SAP Tcodes" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="TCodes.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.SAP.TCodes" %>

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
     <cc1:IACMRadGridControl ID="grdTCodes" runat="server" OnNeedDataSource="grdTCodes_NeedDataSource">
<MasterTableView TableLayout="fixed"
    DataKeyNames="TCODE_Id" EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
    <CommandItemSettings ShowAddNewRecordButton="false"
      ShowRefreshButton="true"></CommandItemSettings>
<Columns>
        <telerik:GridBoundColumn DataField="TCODE_Code" FilterControlAltText="TCODE_Code" HeaderText="TCODE" SortExpression="TCODE_Code" UniqueName="TCODE_Code" HeaderStyle-Width="25%" FilterControlWidth="85%">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="TCODE_Desc" FilterControlAltText="TCODE_Desc" HeaderText="TCODE Description" SortExpression="TCODE_Desc" UniqueName="TCODE_Desc" HeaderStyle-Width="25%" FilterControlWidth="85%">
        </telerik:GridBoundColumn>
</Columns>
</MasterTableView>
</cc1:IACMRadGridControl>
</asp:Panel>
</asp:Content>