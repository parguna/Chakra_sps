<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.DataManagement.InHousePermissions.UC.List" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Configuration/Data/Endpoint/XLUploadUC.ascx" TagPrefix="uc1" TagName="XLUploadUC" %>

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
                                <uc1:XLUploadUC runat="server" ID="XLUploadUC1" OnUserControlButtonClicked="XLUploadUC1_UserControlButtonClicked" IsPartialUpload="true" />
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
            <div class="form-group row">
                <div class="col-xs-6">
                    <div class="application">
                    Application Type
                </div>
                 <div class="application-value">
                    <asp:DropDownList ID="ddlEndPointType" runat="server" AutoPostBack="True" CssClass="wizlbl" Width="323px">
                    </asp:DropDownList>
                </div>
            </div>
            <div class="col-xs-6">
                <div>
                    <div class="row">
                    <div class="col-xs-2" style="margin: 10px;">
                         <b>Approver Type :</b>
                    </div>
                    <div class="col-xs-8">
                        <div>  
                        <ul class="list-group list-group-horizontal">
                          <li class="list-group-item">0 - None</li>
                          <li class="list-group-item">1 - User</li>
                          <li class="list-group-item">3 - Group</li>
                        </ul>
                        </div>
                    </div>
                </div>
                </div>
                 
            </div>
        </div>
        
    </div>
    <br />
    <div style="width: 100%;">
        <div class="form-group row">
                <div class="col-xs-6">
                    <div class="application">
                        Application Name
                    </div>
                    <div class="application-value">
                        <asp:DropDownList ID="ddlEndpoint" runat="server" CssClass="wizlbl" Width="323px" AutoPostBack="true" OnSelectedIndexChanged="ddlEndpoint_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="col-xs-6">
                    <div class="form-group" id="tblValidRecordDetail" runat="server">
                        <div class="col-xs-2">
                            <asp:Label ID="lblcheck" CssClass="wizlbl" runat="server" Text="Display Records :"></asp:Label>
                        </div>
                        <div class="col-xs-10">
                            <asp:RadioButton ID="rbValid" runat="server" GroupName="ValidRecords" CssClass="wizlbl" Text="Valid"
                                OnCheckedChanged="ValidRecordsSelectionChanged" AutoPostBack="true" />
                            &nbsp;&nbsp;
                                    <asp:RadioButton ID="rbInValid" runat="server" GroupName="ValidRecords" CssClass="wizlbl" Checked="True"
                                        OnCheckedChanged="ValidRecordsSelectionChanged" Text="InValid" AutoPostBack="true" />
                        </div>
                    </div>
            </div>
        </div>
    </div>

     <div class="row-fluid">
        <div class="col-xs-12">
            &nbsp;
        </div>
    </div>
     <div class="form-group row">
          <div class="col-xs-6">
            <div class="form-group">
                <div class="col-xs-3">
                    <div id="grdHeader" runat="server" class="subTitle">InValid Records</div>
                </div>
                <div class="col-xs-4">
                    <asp:Label ID="Label1" Text="" runat="server"></asp:Label>
                </div>
            </div>
        </div>
          <div class="col-xs-6">
               <asp:Label ID="lblDetails" runat="server"></asp:Label>
          </div>
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
        <MasterTableView AutoGenerateColumns="False" Width="150%" CommandItemDisplay="Top" EnableHeaderContextMenu="true" DataKeyNames="Id" EditMode="InPlace" InsertItemPageIndexAction="ShowItemOnFirstPage"
            NoDetailRecordsText="No records found.">
            <CommandItemSettings ShowAddNewRecordButton="true" ShowRefreshButton="true" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="true"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="EntitlementName" FilterControlAltText="EntitlementName" HeaderStyle-Width="15%" FilterControlWidth="65%" SortExpression="EntitlementName" HeaderText="EntitlementName" UniqueName="EntitlementName">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="InhouseSystemCode" FilterControlAltText="InhouseSystemCode" HeaderStyle-Width="15%" FilterControlWidth="65%" SortExpression="InhouseSystemCode" HeaderText="InhouseSystemCode" UniqueName="InhouseSystemCode">
                </telerik:GridBoundColumn>
                
                <telerik:GridBoundColumn DataField="Description" FilterControlAltText="Description" HeaderStyle-Width="15%" FilterControlWidth="65%" SortExpression="Description" HeaderText="Description" UniqueName="Description">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="ADGroups" FilterControlAltText="ADGroups" HeaderStyle-Width="15%" FilterControlWidth="65%" SortExpression="ADGroups" HeaderText="ADGroups" UniqueName="ADGroups">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="EPGroups" FilterControlAltText="EPGroups" HeaderStyle-Width="15%" FilterControlWidth="65%" SortExpression="EPGroups" HeaderText="EPGroups" UniqueName="EPGroups">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="EPRoles" FilterControlAltText="EPRoles" HeaderStyle-Width="15%" FilterControlWidth="65%" SortExpression="EPRoles" HeaderText="EPRoles" UniqueName="EPRoles">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="CEGroups" FilterControlAltText="CEGroups" HeaderStyle-Width="15%" FilterControlWidth="65%" SortExpression="CEGroups" HeaderText="CEGroups" UniqueName="CEGroups">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="CERoles" FilterControlAltText="CERoles" HeaderStyle-Width="15%" FilterControlWidth="65%" SortExpression="CERoles" HeaderText="CERoles" UniqueName="CERoles">
                </telerik:GridBoundColumn>
               <telerik:GridTemplateColumn DataField="IsActive" FilterControlAltText="IsActive" Groupable="false" HeaderText="IsActive"
                    SortExpression="IsActive" UniqueName="IsActive" HeaderStyle-Width="15%" FilterControlWidth="65%">
                   <EditItemTemplate>
                        <asp:CheckBox ID="chkIsActive" runat="server"  Checked='<%# Convert.ToBoolean( Eval("IsActive") == DBNull.Value ? false : Eval("IsActive") ) %>' Enabled="true"/>
                   </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Convert.ToBoolean( Eval("IsActive")) ? "Yes":"No" %>' ID="lblIsActive"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="Level1Approver" FilterControlAltText="Level1Approver" HeaderText="Level1 Approver"
                    SortExpression="Level1Approver" UniqueName="Level1Approver" HeaderStyle-Width="15%" FilterControlWidth="65%">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn DataField="Level1ApproverType" FilterControlAltText="Level1ApproverType" Groupable="false" HeaderText="Level1 Approver Type"
                    SortExpression="Level1ApproverType" UniqueName="Level1ApproverType" HeaderStyle-Width="15%" FilterControlWidth="65%">
                    <ItemTemplate>
                        <asp:Label ID="lblLevel1Approver" Text='<%#Convert.ToInt32( Eval("Level1ApproverType")) %>' runat="server"></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%#(Eval("Level1ApproverType"))== DBNull.Value ? "0":( Convert.ToInt32( Eval("Level1ApproverType"))).ToString() %>'></asp:TextBox>
                    </EditItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="Level2Approver" FilterControlAltText="Level2Approver" HeaderText="Level2 Approver"
                    SortExpression="Level2Approver" UniqueName="Level2Approver" HeaderStyle-Width="15%" FilterControlWidth="65%">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn DataField="Level2ApproverType" Groupable="false" FilterControlAltText="Level2ApproverType" HeaderText="Level2 Approver Type"
                    SortExpression="Level2ApproverType" UniqueName="Level2ApproverType" HeaderStyle-Width="15%" FilterControlWidth="65%">
                    <ItemTemplate>
                        <asp:Label ID="Label2" Text='<%#Convert.ToInt32( Eval("Level2ApproverType")) %>' runat="server"></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox2" runat="server" Text='<%#(Eval("Level2ApproverType"))== DBNull.Value ? "0":( Convert.ToInt32( Eval("Level2ApproverType"))).ToString() %>'></asp:TextBox>
                    </EditItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="Level3Approver" FilterControlAltText="Level3Approver" HeaderText="Level3 Approver"
                    SortExpression="Level3Approver" UniqueName="Level3Approver" HeaderStyle-Width="15%" FilterControlWidth="65%">
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

