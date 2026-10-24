<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.Reports.SystemSetupReports.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<div class="content-wrapper" id="ContentWrapper" runat="server">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                    <div class="form-group">
                        <label class="col-xs-4 control-label">Report Type</label>
                        <div id="DivReportDropDown" class="col-xs-7">
                            <asp:DropDownList ID="ReportId" runat="server" CssClass="form-control selectpicker" data-width="100%" AutoPostBack="True" DataTextField="ReportName" DataValueField="Id" OnSelectedIndexChanged="ReportId_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<asp:PlaceHolder ID="gridplaceholder" runat="server"></asp:PlaceHolder>
<cc1:IACMRadGridControl ID="ComplianceReportGrid" runat="server" OnItemCommand="ComplianceReportGrid_ItemCommand" OnNeedDataSource="ComplianceReportGrid_NeedDataSource">
    <MasterTableView  OverrideDataSourceControlSorting="true" TableLayout="Fixed"
        ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
    </MasterTableView>
</cc1:IACMRadGridControl>