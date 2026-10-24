<%@ Page Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="HRMSDetails.aspx.cs" Inherits="DI.Chakra.Web.Admin.AdminUI.HRMSDetails" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxCtrl" %>


<asp:Content ID="Content2" ContentPlaceHolderID="cplMain" runat="server">
    <div class="form">
        <div class="form-horizontal left clearfix">

            <div class="row-fluid">

                <asp:Panel ID="Panel1" runat="server" class="col-xs-8">


                    <div class="row">
                        <div class="form-group">
                            <label class="col-xs-3 control-label">Application Type</label>
                            <div class="col-xs-8">
                                <asp:DropDownList ID="ddlApplicationTypes" CssClass="form-control selectpicker" AutoPostBack="True" runat="server" OnSelectedIndexChanged="ddlApplicationTypes_SelectedIndexChanged" >
                                </asp:DropDownList> 
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="form-group required">
                            <label class="col-xs-3 control-label">PERNR</label>
                            <div class="col-xs-8">
                                <telerik:RadTextBox ID="txtPERNR" MaxLength="8" runat="server" CssClass="form-control">
                                </telerik:RadTextBox>
                            </div>
                        </div>

                    </div>

                    <div class="row">
                        <div class="form-group">
                            <label class="col-xs-3 control-label">Table Names</label>
                            <div class="col-xs-8">
                                <asp:DropDownList ID="ddlTableNames" CssClass="form-control selectpicker" AutoPostBack="True" runat="server">
                                </asp:DropDownList>
                                <asp:Button ID="btn" Text="Get Data" runat="server" OnClick="GetData_Click" />
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-xs-12">
                            <telerik:RadGrid ID="HRMSTableDataGrid" runat="server" EnableLinqExpressions="false" MasterTableView-TableLayout="Auto"
                                AutoGenerateColumns="true"  GroupPanelPosition="Top" AllowPaging="true" 
                                AllowFilteringByColumn="true"  AllowSorting="true"   FilterMenu-CssClass="RadMenu_IACMFilterSkin">
                                <HeaderStyle Font-Bold="True" HorizontalAlign="Center" />
                                <PagerStyle Mode="NextPrevAndNumeric" PageSizes="10" Height="100%" AlwaysVisible="true" />
                                <GroupingSettings CaseSensitive="false" />

                                <ClientSettings>
                                    <Scrolling UseStaticHeaders="false" AllowScroll="true" SaveScrollPosition="true" />
                                </ClientSettings>
                                <MasterTableView TableLayout="Auto" EnableNoRecordsTemplate="true" NoMasterRecordsText="No Records Available" ShowHeadersWhenNoRecords="true" Width="100%">
                                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>

                                </MasterTableView>
                            </telerik:RadGrid>
                        </div>
                    </div>
                </asp:Panel>




                <asp:Panel ID="Panel2" runat="server" class="col-xs-4">
                    <div class="row">
                        <div class="form-group">
                            <label class="col-xs-3 control-label">Events</label>
                            <div class="col-xs-8">
                                <asp:DropDownList ID="ddlEvents" CssClass="form-control selectpicker" AutoPostBack="True" OnSelectedIndexChanged="sapuserselectionchanged" runat="server">
                                </asp:DropDownList>

                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12">
                            <telerik:RadGrid ID="HRMSEventDataGrid" runat="server" EnableLinqExpressions="false" MasterTableView-TableLayout="Auto" Visible="true"
                                AutoGenerateColumns="true" GroupPanelPosition="Top"  OnNeedDataSource="HRMSEventDataGrid_NeedDataSource"  AllowPaging="true"  AllowFilteringByColumn="true" AllowSorting="true" FilterMenu-CssClass="RadMenu_IACMFilterSkin">
                                <HeaderStyle Font-Bold="True" HorizontalAlign="Center" />
                                <PagerStyle Mode="NextPrevAndNumeric" PageSizes="50" Height="100%" AlwaysVisible="true" />

                                <GroupingSettings CaseSensitive="false" />

                                <ClientSettings>
                                    <Scrolling UseStaticHeaders="false" AllowScroll="true" SaveScrollPosition="true" />
                                </ClientSettings>
                                <MasterTableView  PageSize="50" TableLayout="Auto" EnableNoRecordsTemplate="true" NoMasterRecordsText="No Records Available" ShowHeadersWhenNoRecords="true" Width="100%">
                                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>

                                </MasterTableView>
                            </telerik:RadGrid>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>

    </div>
</asp:Content>
