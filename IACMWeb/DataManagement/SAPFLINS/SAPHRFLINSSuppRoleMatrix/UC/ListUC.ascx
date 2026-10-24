<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.DataManagement.SAPHRFLINSSuppRoleMatrix.UC.List" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Configuration/Data/Endpoint/XLUploadUC.ascx" TagPrefix="uc1" TagName="XLUploadUC" %>
<%--<script type="text/javascript">
    function Redirectto() {
        window.location = "List.aspx";
    }

</script>--%>
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

    <table border="0" cellpadding="3" cellspacing="3" visible="false" id="tblValidRecordDetails" runat="server">
        <tr>
            <td>
                <asp:Label ID="lblValidDisplay" CssClass="wizlbl" runat="server" Text="Display Records :"></asp:Label>
                &nbsp;&nbsp;
            </td>
            <td>
                <asp:RadioButton ID="rbAll" runat="server" GroupName="ValidRecords" CssClass="wizlbl" Text="All"
                    OnCheckedChanged="ValidRecordsSelectionChanged" Width="80px" AutoPostBack="true" />
                &nbsp;&nbsp;
                            <asp:RadioButton ID="rbValid" runat="server" GroupName="ValidRecords" CssClass="wizlbl" Text="Valid"
                                OnCheckedChanged="ValidRecordsSelectionChanged" Width="80px" AutoPostBack="true" />
                &nbsp;&nbsp;
                            <asp:RadioButton ID="rbInValid" runat="server" GroupName="ValidRecords" CssClass="wizlbl" Checked="True"
                                OnCheckedChanged="ValidRecordsSelectionChanged" Text="InValid" Width="80px" AutoPostBack="true" />
            </td>
            <td style="padding-left:50px;">
                <asp:Label ID="lblDetails" Text="" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br />
            </td>
        </tr>
        <tr>
            <td>
                <div id="grdHeader" runat="server" class="subTitle">InValid Records</div>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br />
            </td>
        </tr>
    </table>
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
                <telerik:GridBoundColumn DataField="Rule" FilterControlAltText="Rule" HeaderText="Rule"
                    SortExpression="Rule" UniqueName="Rule" HeaderStyle-Width="10%" FilterControlWidth="65%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Company" FilterControlAltText="Company" HeaderText="Company" SortExpression="Company" UniqueName="Company" HeaderStyle-Width="10%" FilterControlWidth="75%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="CostCenter" FilterControlAltText="CostCenter" HeaderText="Cost Center" SortExpression="CostCenter" UniqueName="CostCenter" HeaderStyle-Width="10%" FilterControlWidth="75%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="PurchasingOrg" FilterControlAltText="PurchasingOrg" HeaderText="Purchasing Org" SortExpression="PurchasingOrg" UniqueName="PurchasingOrg" HeaderStyle-Width="10%" FilterControlWidth="75%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="PurchasingGrp" FilterControlAltText="PurchasingGrp" HeaderText="Purchasing Grp" SortExpression="PurchasingGrp" UniqueName="PurchasingGrp" HeaderStyle-Width="10%" FilterControlWidth="75%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Plant" FilterControlAltText="Plant" HeaderText="Plant" SortExpression="Plant" UniqueName="Plant" HeaderStyle-Width="10%" FilterControlWidth="75%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="MRPControl" FilterControlAltText="MRPControl" HeaderText="MRP Controller" SortExpression="MRPControl" UniqueName="MRPControl" HeaderStyle-Width="10%" FilterControlWidth="75%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="StorageLocation" FilterControlAltText="StorageLocation" HeaderText="Storage Location" SortExpression="StorageLocation" UniqueName="StorageLocation" HeaderStyle-Width="10%" FilterControlWidth="75%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Role" FilterControlAltText="Role" HeaderText="Role" SortExpression="Role" UniqueName="Role" HeaderStyle-Width="10%" FilterControlWidth="75%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="RoleName" FilterControlAltText="RoleName" HeaderText="Role Name" SortExpression="RoleName" UniqueName="RoleName" HeaderStyle-Width="10%" FilterControlWidth="75%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderAxis="Validation Message" UniqueName="ValidationMessage" DataField="ValidationMessage" SortExpression="ValidationMessage"></telerik:GridBoundColumn>
                <telerik:GridEditCommandColumn FilterControlWidth="65%" UniqueName="Edit" HeaderStyle-Width="7%"/>
            </Columns>
        </MasterTableView>
    </cc1:IACMRadGridControl>
</div>

