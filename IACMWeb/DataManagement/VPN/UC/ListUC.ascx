<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.DataManagement.VPN.UC.List" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Configuration/Data/Endpoint/XLUploadUC.ascx" TagPrefix="uc1" TagName="XLUploadUC" %>

<style>
    .list-group-horizontal .list-group-item
{
	display: inline-block;
}
.list-group-horizontal .list-group-item
{
	margin-bottom: 0;
	margin-left:-4px;
	margin-right: 0;
 	border-right-width: 0;
}
.list-group-horizontal .list-group-item:first-child
{
	border-top-right-radius:0;
	border-bottom-left-radius:4px;
}
.list-group-horizontal .list-group-item:last-child
{
	border-top-right-radius:4px;
	border-bottom-left-radius:0;
	border-right-width: 1px;
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

    <table border="0" cellpadding="3" cellspacing="3" visible="false" id="tblValidRecordDetails" runat="server">
        <tr>
             <td  style="width:10%;">
                <asp:Label ID="lblValidDisplay" CssClass="wizlbl" runat="server" Text="Display Records :"></asp:Label>
                &nbsp;&nbsp;
            </td>
            <td  style="width:20%;">
                <asp:RadioButton ID="rbAll" runat="server" GroupName="ValidRecords" CssClass="wizlbl" Text="All"
                    OnCheckedChanged="ValidRecordsSelectionChanged" Width="80px" AutoPostBack="true" />
                &nbsp;&nbsp;
                            <asp:RadioButton ID="rbValid" runat="server" GroupName="ValidRecords" CssClass="wizlbl" Text="Valid"
                                OnCheckedChanged="ValidRecordsSelectionChanged" Width="80px" AutoPostBack="true" />
                &nbsp;&nbsp;
                            <asp:RadioButton ID="rbInValid" runat="server" GroupName="ValidRecords" CssClass="wizlbl" Checked="True"
                                OnCheckedChanged="ValidRecordsSelectionChanged" Text="InValid" Width="80px" AutoPostBack="true" />
            </td>
             <td style="padding-left:50px;width:35%;">
                <asp:Label ID="lblDetails" Text="" runat="server"></asp:Label>
            </td>
            <td style="width:35%;">
                <table style="width:100%">
                    <tr class="form-group row">
                    <td class="col-xs-3">
                        <b>Approver Type :</b>
                    </td>
                    <td class="col-xs-9" id="tdLevelApproverType">
                        <div>
                        <ul class="list-group list-group-horizontal">
                          <li class="list-group-item">0 - None</li>
                          <li class="list-group-item">1 - User</li>
                          <li class="list-group-item">3 - Group</li>
                        </ul>
                        </div>
                    </td>
                </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <br />
            </td>
        </tr>
        <tr>
           <td colspan="3">
                <div id="grdHeader" runat="server" class="subTitle">InValid Records</div>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <br />
            </td>
        </tr>
    </table>
    <cc1:IACMRadGridControl ID="grdInvalidEntries" runat="server" OnNeedDataSource="grdInvalidEntries_NeedDataSource" OnSelectedIndexChanged="grdInvalidEntries_SelectedIndexChanged" 
        OnItemDataBound="grdInvalidEntries_ItemDataBound" EnableLinqExpressions="false" AllowFilteringByColumn="true">
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
        <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="Top" EnableHeaderContextMenu="true" DataKeyNames="Id,VPNMatrixType,MatrixType" EditMode="InPlace" InsertItemPageIndexAction="ShowItemOnFirstPage"
            NoDetailRecordsText="No records found." Width="150%">
            <CommandItemSettings ShowAddNewRecordButton="true" ShowRefreshButton="true" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="true"></CommandItemSettings>
            <Columns>
                <telerik:GridTemplateColumn  DataField="Id"  HeaderText="Id"  FilterControlAltText="Id" SortExpression="Id" UniqueName="Id" HeaderStyle-Width="5%" AllowFiltering="false">
                      <ItemTemplate>
                        <asp:Label ID="lblId" Text='<%#Convert.ToString( Eval("Id")) %>' runat="server"></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:Label ID="lblEditId" runat="server"  Text='<%#Convert.ToString( Eval("Id")) %>' ></asp:Label>
                    </EditItemTemplate>
                </telerik:GridTemplateColumn>
                 <telerik:GridTemplateColumn DataField="MatrixType" FilterControlAltText="VPNMatrixType" HeaderText="VPN Type" DataType="System.String" AllowFiltering="true"
                    SortExpression="Type" UniqueName="MatrixType" HeaderStyle-Width="25%" FilterControlWidth="65%">
                     <FilterTemplate>
                        <telerik:RadComboBox ID="MatrixType" AppendDataBoundItems="true" runat="server" OnSelectedIndexChanged="VpnTypeList_SelectedIndexChanged"
                            AutoPostBack="true" OnPreRender="VPNType_PreRender" CssClass="selectpicker AccountsPageComoboWidth radbox100Width" data-width="100%" DropDownAutoWidth="Enabled">
                        </telerik:RadComboBox>
                    </FilterTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblRequestType" Text='<%#Convert.ToString( Eval("Type")) %>' runat="server"></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <telerik:RadComboBox ID="RadRequestType" runat="server"  AutoPostBack="true"  Width="200px" OnSelectedIndexChanged="RadRequestType_SelectedIndexChanged">
                          </telerik:RadComboBox>
                    </EditItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="Name" FilterControlAltText="Name" HeaderText="Name"
                    SortExpression="Name" UniqueName="Name" HeaderStyle-Width="15%" FilterControlWidth="65%">
                </telerik:GridBoundColumn>
                 <telerik:GridTemplateColumn DataField="ProductionADgroups" FilterControlAltText="ProductionADgroups" HeaderText="ProductionADgroups"
                    SortExpression="ProductionADgroups" UniqueName="ProductionADgroups" HeaderStyle-Width="15%" FilterControlWidth="65%">
                      <ItemTemplate>
                        <asp:Label ID="lblProductionADgroups" Text='<%#Convert.ToString( Eval("ProductionADgroups")) %>' runat="server"></asp:Label>
                    </ItemTemplate>
                     <EditItemTemplate>
                        <asp:TextBox ID="txtProductionADgroups" runat="server" Text='<%#(Eval("ProductionADgroups"))== DBNull.Value ? "":( Convert.ToString( Eval("ProductionADgroups"))).ToString() %>'></asp:TextBox>
                    </EditItemTemplate>
                </telerik:GridTemplateColumn>
                 <telerik:GridTemplateColumn DataField="QAADGroups" FilterControlAltText="QAADGroups" HeaderText="QAADGroups"
                    SortExpression="QAADGroups" UniqueName="QAADGroups" HeaderStyle-Width="15%" FilterControlWidth="65%">
                     <ItemTemplate>
                        <asp:Label ID="lblQAADGroups" Text='<%#Convert.ToString( Eval("QAADGroups")) %>' runat="server"></asp:Label>
                    </ItemTemplate>
                     <EditItemTemplate>
                        <asp:TextBox ID="txtQAADGroups" runat="server" Text='<%#(Eval("QAADGroups"))== DBNull.Value ? "":( Convert.ToString( Eval("QAADGroups"))).ToString() %>'></asp:TextBox>
                    </EditItemTemplate>
                </telerik:GridTemplateColumn>
                 <telerik:GridTemplateColumn DataField="DevelpomentADGroups" FilterControlAltText="DevelpomentADGroups" HeaderText="DevelpomentADGroups"
                    SortExpression="DevelpomentADGroups" UniqueName="DevelpomentADGroups" HeaderStyle-Width="15%" FilterControlWidth="65%">
                      <ItemTemplate>
                        <asp:Label ID="lblDevelpomentADGroups" Text='<%#Convert.ToString( Eval("DevelpomentADGroups")) %>' runat="server"></asp:Label>
                    </ItemTemplate>
                     <EditItemTemplate>
                        <asp:TextBox ID="txtDevelpomentADGroups" runat="server" Text='<%#(Eval("DevelpomentADGroups"))== DBNull.Value ? "":( Convert.ToString( Eval("DevelpomentADGroups"))).ToString() %>'></asp:TextBox>
                    </EditItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="ADGroups" FilterControlAltText="ADGroups" HeaderText="ADGroups"
                    SortExpression="ADGroups" UniqueName="ADGroups" HeaderStyle-Width="15%" FilterControlWidth="65%">
                     <ItemTemplate>
                        <asp:Label ID="lblADGroups" Text='<%#Convert.ToString( Eval("ADGroups")) %>' runat="server"></asp:Label>
                    </ItemTemplate>
                      <EditItemTemplate>
                        <asp:TextBox ID="txtADGroups" runat="server" Text='<%#(Eval("ADGroups"))== DBNull.Value ? "":( Convert.ToString( Eval("ADGroups"))).ToString() %>'></asp:TextBox>
                    </EditItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="Level1Approver" FilterControlAltText="Level1Approver" HeaderText="Level1 Approver"
                    SortExpression="Level1Approver" UniqueName="Level1Approver" HeaderStyle-Width="10%" FilterControlWidth="65%">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn DataField="Level1ApproverTypeString" FilterControlAltText="Level1ApproverType" Groupable="false" HeaderText="Level1 Approver Type"
                    SortExpression="Level1ApproverType" UniqueName="Level1ApproverType" HeaderStyle-Width="15%" FilterControlWidth="65%">
                    <ItemTemplate>
                        <asp:Label ID="Label1" Text='<%#Convert.ToInt32( Eval("Level1ApproverType")) %>' runat="server"></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%#(Eval("Level1ApproverType"))== DBNull.Value ? "0":( Convert.ToInt32( Eval("Level1ApproverType"))).ToString() %>'></asp:TextBox>
                    </EditItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridBoundColumn DataField="Level2Approver" FilterControlAltText="Level2Approver" HeaderText="Level2 Approver"
                    SortExpression="Level2Approver" UniqueName="Level2Approver" HeaderStyle-Width="15%" FilterControlWidth="65%">
                </telerik:GridBoundColumn>

                <telerik:GridTemplateColumn DataField="Level2ApproverTypeString" Groupable="false" FilterControlAltText="Level2ApproverType" HeaderText="Level2 Approver Type"
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
                <telerik:GridTemplateColumn DataField="Level3ApproverTypeString" FilterControlAltText="Level3ApproverType" Groupable="false" HeaderText="Level3 Approver Type"
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

