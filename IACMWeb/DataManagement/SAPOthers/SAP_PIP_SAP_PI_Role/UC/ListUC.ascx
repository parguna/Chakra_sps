<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.DataManagement.SAP_PIP_SAP_PI_Role.UC.List" %>
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
        <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="Top" EnableHeaderContextMenu="true" DataKeyNames="Id" EditMode="InPlace" InsertItemPageIndexAction="ShowItemOnFirstPage"
            NoDetailRecordsText="No records found.">
            <CommandItemSettings ShowAddNewRecordButton="true" ShowRefreshButton="true" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="true"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="Remarks" FilterControlAltText="Remarks" HeaderText="Remarks"
                    SortExpression="Remarks" UniqueName="Remarks" HeaderStyle-Width="10%" FilterControlWidth="65%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Application" FilterControlAltText="Application" HeaderText="Application"
                    SortExpression="Application" UniqueName="Application" HeaderStyle-Width="10%" FilterControlWidth="65%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Module" FilterControlAltText="Module" HeaderText="Module"
                    SortExpression="Module" UniqueName="Module" HeaderStyle-Width="10%" FilterControlWidth="65%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="SubModule" FilterControlAltText="SubModule" HeaderText="Sub Module"
                    SortExpression="SubModule" UniqueName="SubModule" HeaderStyle-Width="10%" FilterControlWidth="65%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="BusinessRoleName" FilterControlAltText="BusinessRoleName" HeaderText="Rolename"
                    SortExpression="BusinessRoleName" UniqueName="BusinessRoleName" HeaderStyle-Width="10%" FilterControlWidth="65%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="TechnicalRole" FilterControlAltText="TechnicalRole" HeaderText="Technical Role"
                    SortExpression="TechnicalRole" UniqueName="TechnicalRole" HeaderStyle-Width="10%" FilterControlWidth="65%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Level1Approver" FilterControlAltText="Level1Approver" HeaderText="Level1 Approver"
                    SortExpression="Level1Approver" UniqueName="Level1Approver" HeaderStyle-Width="10%" FilterControlWidth="65%">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn DataField="Level1ApproverType" FilterControlAltText="Level1ApproverType" Groupable="false" HeaderText="Level1 Approver Type"
                    SortExpression="Level1ApproverType" UniqueName="Level1ApproverType" HeaderStyle-Width="15%" FilterControlWidth="65%">
                    <ItemTemplate>
                        <asp:Label ID="Label1" Text='<%#Convert.ToInt32( Eval("Level1ApproverType")) %>' runat="server"></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%#(Eval("Level1ApproverType"))== DBNull.Value ? "0":( Convert.ToInt32( Eval("Level1ApproverType"))).ToString() %>'></asp:TextBox>
                    </EditItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridBoundColumn DataField="Level2Approver" FilterControlAltText="Level2Approver" HeaderText="Level2 Approver"
                    SortExpression="Level2Approver" UniqueName="Level2Approver" HeaderStyle-Width="10%" FilterControlWidth="65%">
                </telerik:GridBoundColumn>

                <telerik:GridTemplateColumn DataField="Level2ApproverType" Groupable="false" FilterControlAltText="Level2ApproverType" HeaderText="Level2 Approver Type"
                    SortExpression="Level2ApproverType" UniqueName="Level2ApproverType" HeaderStyle-Width="15%" FilterControlWidth="65%">
                    <ItemTemplate>
                        <asp:Label ID="Label2" Text='<%#Convert.ToInt32( Eval("Level2ApproverType")) %>' runat="server"></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox2" runat="server" Text='<%#(Eval("Level2ApproverType"))== DBNull.Value ? "0" : ( Convert.ToInt32( Eval("Level2ApproverType"))).ToString() %>'></asp:TextBox>
                    </EditItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridBoundColumn DataField="Level3Approver" FilterControlAltText="Level3Approver" HeaderText="Level3 Approver"
                    SortExpression="Level3Approver" UniqueName="Level3Approver" HeaderStyle-Width="10%" FilterControlWidth="65%">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn DataField="Level3ApproverType" FilterControlAltText="Level3ApproverType" Groupable="false" HeaderText="Level3 Approver Type"
                    SortExpression="Level3ApproverType" UniqueName="Level3ApproverType" HeaderStyle-Width="15%" FilterControlWidth="65%">
                    <ItemTemplate>
                        <asp:Label ID="Label3" Text='<%#Convert.ToInt32( Eval("Level3ApproverType")) %>' runat="server"></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox3" runat="server" Text='<%#(Eval("Level3ApproverType"))== DBNull.Value ? "0":( Convert.ToInt32( Eval("Level3ApproverType"))).ToString() %>'></asp:TextBox>
                    </EditItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridBoundColumn DataField="ValidationMessage" FilterControlAltText="ValidationMessage" HeaderText="Validation Message"
                    SortExpression="ValidationMessage" UniqueName="ValidationMessage" HeaderStyle-Width="10%" FilterControlWidth="65%">
                </telerik:GridBoundColumn>
                <telerik:GridEditCommandColumn FilterControlWidth="65%" HeaderStyle-Width="10%" UniqueName="Edit" />
            </Columns>
        </MasterTableView>
    </cc1:IACMRadGridControl>
</div>

