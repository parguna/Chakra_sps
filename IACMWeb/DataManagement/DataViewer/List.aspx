<%@ Page Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.DataManagement.DataViewer.List" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cplMain" runat="server">
    <style>
        .RadGrid_IACMSkin .rgHeaderWrapper, .RadGrid_IACMSkin .rgDataDiv {
            overflow-y: hidden !important;
        }
    </style>
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
            <h4 class="modal-title" id="H1">Enter the Query to View Data</h4>
            <div class="row-fluid">
                <div class="col-xs-12">
                    <div class="col-xs-4">
                        <span class="wizlbl">Select Table</span>
                        <telerik:RadAutoCompleteBox CssClass="form-control" RenderMode="Lightweight" ID="ddlTables" runat="server"
                            TextSettings-SelectionMode="Single" EmptyMessage="Enter * to view full list" InputType="Token"
                            OnEntryRemoved="ddlTables_EntryRemoved" OnEntryAdded="ddlTables_EntryAdded">
                            <WebServiceSettings Path="../../Services/AutoCompleteDataService.svc" Method="GetAllTables" />
                        </telerik:RadAutoCompleteBox>
                    </div>
                    <div class="col-xs-4">
                        <span class="wizlbl">Select Columns</span>
                        <telerik:RadComboBox ID="ddlColumns" Style="width: 100% !important" DropDownAutoWidth="Enabled" Width="100%" runat="server" CheckBoxes="true"
                            CausesValidation="true" data-width="100%">
                        </telerik:RadComboBox>
                    </div>
                    <div class="col-xs-4">
                        <asp:Button ID="btnBuildQuery" Style="margin-left: 5px" CssClass="btn btn-default btn-sm" runat="server" Text="Build Query" OnClick="btnBuildQuery_Click" />
                    </div>
                </div>
            </div>
            <div class="row-fluid">
                <div class="col-xs-12" style="line-height: 50px;">
                    <div>Query</div>
                    <div>
                        <asp:TextBox ID="QueryTextBox" runat="server" TextMode="MultiLine" Height="100px" Width="90%"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="row-fluid">
                <div class="col-xs-12">
                    <asp:Button ID="ExecuteQueryButton" CssClass="btn btn-default btn-sm" runat="server" Text="Execute Query" OnClick="ExecuteQueryButton_Click" />
                    <asp:Button ID="RetreiveDataButton" CssClass="btn btn-default btn-sm" runat="server" Text="Retreive Data" OnClick="RetreiveDataButton_Click" />
                </div>
            </div>
            <div class="row-fluid">
                <div class="col-xs-12">
                    <img id="imginfo" src="../Images/info1.gif" runat="server" visible="False" align="middle"
                        alt="info" /><span class="infomsg" />
                    <asp:Label ID="lblinfo" runat="server" Visible="False" Font-Bold="True" Font-Size="Small"></asp:Label>
                    <img id="imgerror" src="../Images/messageError.gif" runat="server" visible="False"
                        align="middle" alt="info" />
                    <span class="infomsg" />
                    <asp:Label ID="lblerror" runat="server" Visible="False" ForeColor="Red" Font-Bold="True"
                        Font-Size="Small"></asp:Label>
                </div>
            </div>
            <%--  <telerik:RadGrid ID="grdUser" OnNeedDataSource="grdUser_NeedDataSource" MasterTableView-TableLayout="Auto"
                    runat="server" AllowFilteringByColumn="True"
                    AllowPaging="True"
                    AllowSorting="True" CellSpacing="0" GridLines="None"
                    EnableAjaxSkinRendering="true">
                    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="50" />
                    <ExportSettings
                        HideStructureColumns="true"
                        IgnorePaging="true" ExportOnlyData="true"
                        Excel-Format="ExcelML"
                        OpenInNewWindow="true">
                        <Pdf PageHeight="210mm" AllowPrinting="true" PageWidth="470mm"
                            DefaultFontFamily="Arial Unicode MS" PageTopMargin="45mm"
                            BorderStyle="Medium" BorderColor="#666666"
                            BorderType="AllBorders">
                        </Pdf>
                    </ExportSettings>
                    <GroupingSettings CaseSensitive="false" />
                    <MasterTableView TableLayout="Auto" Width="150%" EnableNoRecordsTemplate="true" NoMasterRecordsText="No Records Available" ShowHeadersWhenNoRecords="true">
                        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>

                    </MasterTableView>
                </telerik:RadGrid>--%>
            <div class="row-fluid">
                <div class="col-xs-12">
                    <telerik:RadGrid ID="grdUser" OnNeedDataSource="grdUser_NeedDataSource" runat="server" EnableLinqExpressions="false" MasterTableView-TableLayout="Auto" CellSpacing="0"
                        AutoGenerateColumns="true" GroupPanelPosition="Top" AllowPaging="true" AllowCustomPaging="true" AllowFilteringByColumn="true" AllowSorting="true">
                        <HeaderStyle Font-Bold="True" HorizontalAlign="Center" />
                        <PagerStyle Mode="NextPrevAndNumeric" PageSizes="50" Height="100%" AlwaysVisible="true" />
                        <GroupingSettings CaseSensitive="false" />

                        <ClientSettings>
                            <Scrolling AllowScroll="true" SaveScrollPosition="true" />
                        </ClientSettings>
                        <MasterTableView Width="100%" TableLayout="Auto" EnableNoRecordsTemplate="true" NoMasterRecordsText="No Records Available" ShowHeadersWhenNoRecords="true">
                            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>

                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </div>
              <div class="buttons text-right nav-rectangle">
        <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="btnCancel_Click" />
    </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ExecuteQueryButton" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="RetreiveDataButton" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

