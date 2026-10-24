<%@ Page Title="SAP Role" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Roles.aspx.cs" Inherits="DI.Chakra.Web.Report.Endpoint.SAP.Roles" %>

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
    <cc1:IACMRadGridControl ID="grdRoles" runat="server" OnNeedDataSource="grdUser_NeedDataSource">
<MasterTableView  TableLayout="fixed"
    DataKeyNames="SAP_ROLEID" EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
    <CommandItemSettings ShowAddNewRecordButton="false"
        ShowRefreshButton="true"></CommandItemSettings>
    
<Columns>
    <telerik:GridBoundColumn DataField="ROLE_CODE" FilterControlAltText="ROLE_CODE" HeaderText="Role Code" SortExpression="ROLE_CODE" UniqueName="ROLE_CODE" HeaderStyle-Width="20%" FilterControlWidth="80%">
        <ColumnValidationSettings>
            <ModelErrorMessage Text="" />
        </ColumnValidationSettings>
    </telerik:GridBoundColumn>
    <telerik:GridBoundColumn DataField="ROLE_DESC" FilterControlAltText="ROLE_DESC" HeaderText="Role Description" SortExpression="ROLE_DESC" UniqueName="ROLE_DESC" HeaderStyle-Width="25%" FilterControlWidth="85%">
        <ColumnValidationSettings>
            <ModelErrorMessage Text="" />
        </ColumnValidationSettings>
    </telerik:GridBoundColumn>

</Columns>
</MasterTableView>
</cc1:IACMRadGridControl>
</asp:Panel>
                       
</asp:Content>
