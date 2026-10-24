<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.DataManagement.PermissionFunction.UC.List" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Configuration/Data/Endpoint/XLUploadUC.ascx" TagPrefix="uc1" TagName="XLUploadUC" %>
<%--<script type="text/javascript">
    function Redirectto() {
        window.location = "List.aspx";
    }

</script>--%>
<style>
    .application {
        width: 35%;
        display: inline;
        padding: 15px;
    }

    .application-value {
        width: 65%;
        display: inline;
        padding: 15px;
    }
</style>
<div class="content-wrapper">
    <telerik:RadWindow ID="modalPopup" runat="server" Width="700px" Height="170px" Modal="true" OffsetElementID="main">
        <ContentTemplate>
            <div class="modal-header">
                <h3 class="modal-title" id="H1">Data Upload</h3>
            </div>
            <div class="modal-body">
                <div class="form-horizontal left clearfix">
                    <div class="row-fluid">
                        <div class="col-xs-12">
                            <div class="form-group">
                                <uc1:XLUploadUC runat="server" ID="XLUploadUC1" OnUserControlButtonClicked="XLUploadUC1_UserControlButtonClicked" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <asp:Button runat="server" class="btn btn-primary transparent-button" ID="btnCancel" Text="Cancel" OnClick="btnCancel_Click"></asp:Button>
            </div>
        </ContentTemplate>
    </telerik:RadWindow>
    <div style="width: 100%;">
        <div class="application">
            Application Type
        </div>
        <div class="application-value">
            <asp:DropDownList ID="ddlEndPointType" runat="server" AutoPostBack="True" CssClass="wizlbl" Width="323px">
            </asp:DropDownList>
        </div>
    </div>
    <br />
    <div style="width: 100%;">
        <div class="application">
            Application Name
        </div>
        <div class="application-value">
            <asp:DropDownList ID="ddlEndpoint" runat="server" CssClass="wizlbl" Width="323px" AutoPostBack="true" OnSelectedIndexChanged="ddlEndpoint_SelectedIndexChanged">
            </asp:DropDownList>
        </div>
    </div>
    <div style="width: 100%; padding: 15px;">
        <asp:Label ID="lblDetails" runat="server"></asp:Label>
    </div>
    <cc1:IACMRadGridControl ID="grdInvalidEntries" runat="server" OnNeedDataSource="grdInvalidEntries_NeedDataSource" OnSelectedIndexChanged="grdInvalidEntries_SelectedIndexChanged" OnItemDataBound="grdInvalidEntries_ItemDataBound">
        <ExportSettings
            HideStructureColumns="true"
            IgnorePaging="true" ExportOnlyData="true"
            Excel-Format="ExcelML"
            OpenInNewWindow="true">
            <Pdf ForceTextWrap="true" PageHeight="210mm" AllowPrinting="true" PageWidth="445mm" DefaultFontFamily="Arial Unicode MS" PageTopMargin="45mm"
                BorderStyle="Medium" BorderColor="#666666" BorderType="AllBorders">
            </Pdf>
        </ExportSettings>
        <GroupingSettings CaseSensitive="false" />
        <MasterTableView AutoGenerateColumns="False" Width="100%" CommandItemDisplay="Top" EnableHeaderContextMenu="true" DataKeyNames="Id" EditMode="InPlace" InsertItemPageIndexAction="ShowItemOnFirstPage"
            NoDetailRecordsText="No records found.">
            <CommandItemSettings ShowAddNewRecordButton="true" ShowRefreshButton="true" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="true"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="UserName" FilterControlAltText="UserName" HeaderStyle-Width="15%" FilterControlWidth="65%" SortExpression="UserName" HeaderText="UserName" UniqueName="UserName">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="EmployeeId" FilterControlAltText="EmployeeId" HeaderStyle-Width="15%" FilterControlWidth="65%" SortExpression="EmployeeId" HeaderText="Employee Id" UniqueName="EmployeeId">
                </telerik:GridBoundColumn>
                <%--<telerik:GridBoundColumn DataField="NRIC" FilterControlAltText="NRIC" HeaderStyle-Width="15%" FilterControlWidth="65%" SortExpression="NRIC" HeaderText="NRIC" UniqueName="NRIC">
                </telerik:GridBoundColumn>--%>
                <telerik:GridBoundColumn DataField="InhouseSystemCode" FilterControlAltText="InhouseSystemCode" HeaderStyle-Width="15%" FilterControlWidth="65%" SortExpression="InhouseSystemCode" HeaderText="Inhouse System Code" UniqueName="InhouseSystemCode">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Permission" FilterControlAltText="Permission" HeaderStyle-Width="15%" FilterControlWidth="65%" SortExpression="Permission" HeaderText="Permission Name" UniqueName="PermissionName">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Functions" FilterControlAltText="Functions" HeaderStyle-Width="15%" FilterControlWidth="65%" SortExpression="Functions" HeaderText="Function Name" UniqueName="Functions">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="ValidationMessage" FilterControlAltText="ValidationMessage" HeaderText="Validation Message"
                    SortExpression="ValidationMessage" UniqueName="ValidationMessage" HeaderStyle-Width="10%" FilterControlWidth="65%">
                </telerik:GridBoundColumn>
                <telerik:GridEditCommandColumn FilterControlWidth="65%" HeaderStyle-Width="10%" UniqueName="Edit" />
            </Columns>
        </MasterTableView>
    </cc1:IACMRadGridControl>
</div>

