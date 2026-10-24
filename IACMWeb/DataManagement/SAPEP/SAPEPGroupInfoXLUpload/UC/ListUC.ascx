<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.DataManagement.SAPEPGroupInfoXLUpload.UC.List" %>
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

    <table border="0" cellpadding="3" cellspacing="3" width="100%" id="tblValidRecordDetails" runat="server">
        <tr>
            <td width="150px">
                <asp:Label ID="lblIsxluploaded" runat="server" CssClass="wizlbl">Uploaded by Excel:</asp:Label>
            </td>
            <td width="80px" id="tdall" runat="server">
                <asp:RadioButton ID="rbYes" runat="server" GroupName="IsXLUploaded" CssClass="wizlbl" Text="Yes"
                    OnCheckedChanged="IsXLUploaded" Width="80px" AutoPostBack="true" />
            </td>

            <td width="80px">
                <asp:RadioButton ID="rbNo" runat="server" GroupName="IsXLUploaded" CssClass="wizlbl" Text="No"
                    OnCheckedChanged="IsXLUploaded" Width="80px" AutoPostBack="true" />
            </td>

            <td style="padding-left:50px;">
                <asp:Label ID="lblDetails" Text="" runat="server"></asp:Label>
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
        <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="Top" EnableHeaderContextMenu="true" DataKeyNames="Id" EditMode="InPlace" InsertItemPageIndexAction="ShowItemOnFirstPage"
            NoDetailRecordsText="No records found." Width="100%">
            <CommandItemSettings ShowAddNewRecordButton="true" ShowRefreshButton="true" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="true"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="GroupId" FilterControlAltText="GroupId" HeaderText="Group Id"
                    SortExpression="GroupId" UniqueName="GroupId" HeaderStyle-Width="10%" FilterControlWidth="65%" ItemStyle-CssClass="maximize">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="GroupName" FilterControlAltText="GroupName" HeaderText="Group Name"
                    SortExpression="GroupName" UniqueName="GroupName" HeaderStyle-Width="10%" FilterControlWidth="65%" ItemStyle-CssClass="maximize">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="GroupDescription" FilterControlAltText="GroupDescription" HeaderText="Group Description"
                    SortExpression="GroupDescription" UniqueName="GroupDescription" HeaderStyle-Width="10%" FilterControlWidth="65%" ItemStyle-CssClass="maximize">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="GroupUniqueName" FilterControlAltText="GroupUniqueName" HeaderText="GroupUniqueName"
                    SortExpression="GroupUniqueName" UniqueName="GroupUniqueName" HeaderStyle-Width="10%" FilterControlWidth="65%" ItemStyle-CssClass="maximize">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Version" FilterControlAltText="Version" HeaderText="Version"
                    SortExpression="Version" UniqueName="Version" HeaderStyle-Width="10%" FilterControlWidth="65%" ItemStyle-CssClass="maximize">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="ValidationMessage" FilterControlAltText="ValidationMessage" HeaderText="Validation Message"
                    SortExpression="ValidationMessage" UniqueName="ValidationMessage" HeaderStyle-Width="10%" FilterControlWidth="65%" ItemStyle-CssClass="maximize">
                </telerik:GridBoundColumn>
                <telerik:GridEditCommandColumn FilterControlWidth="65%" HeaderStyle-Width="5%" UniqueName="Edit" />
            </Columns>
        </MasterTableView>
    </cc1:IACMRadGridControl>
</div>

