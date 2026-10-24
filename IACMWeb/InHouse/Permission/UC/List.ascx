<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="List.ascx.cs" Inherits="DI.Chakra.Web.InHouse.Permission.UC.List" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Configuration/Data/Endpoint/XLUploadUC.ascx" TagPrefix="uc1" TagName="XLUploadUC" %>

<div class="content-wrapper">

    <telerik:RadWindow ID="modalPopup" runat="server" Width="700px" Height="170px" Modal="true" OffsetElementID="main" VisibleTitlebar="false" VisibleStatusbar="false">
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

    <div class="content-wrapper">
        <div class="form">
            <div class="form-horizontal left">
                <div class="row-fluid">
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label id="lbl" class="control-label col-xs-3" runat="server">Display Records:</label>
                            <div class="col-xs-6">
                                <div class="radio-inline">
                                    <asp:Label ID="Label5" runat="server" AssociatedControlID="rbValid">
                                        <asp:RadioButton runat="server" ID="rbValid" AutoPostBack="true" GroupName="ValidRecords" OnCheckedChanged="ValidRecordsSelectionChanged"></asp:RadioButton>
                                        <asp:Label ID="Label6" CssClass="lbl" runat="server" Text="Valid"></asp:Label>
                                    </asp:Label>
                                </div>
                                <div class="radio-inline">
                                    <asp:Label ID="Label3" runat="server" AssociatedControlID="rbInValid">
                                        <asp:RadioButton runat="server" ID="rbInValid" AutoPostBack="true" GroupName="ValidRecords" OnCheckedChanged="ValidRecordsSelectionChanged"></asp:RadioButton>
                                        <asp:Label ID="Label4" CssClass="lbl" runat="server" Text="Invalid"></asp:Label>
                                    </asp:Label>
                                </div>
                            </div>
                        </div>

                    </div>
                    <div class="form-horizontal right">
                       <asp:HyperLink ID="btnExportToExcel" runat="server" Target="_blank">Permission Template</asp:HyperLink>
                        <asp:Label ID="lblDownloaded" CssClass="lbl" runat="server" Visible="false" Text="Downloaded..."></asp:Label>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <cc1:IACMRadGridControl ID="Permission" runat="server" OnItemDataBound="Permission_ItemDataBound" OnNeedDataSource="Permission_NeedDataSource" OnSelectedIndexChanged="Permission_SelectedIndexChanged">
        <MasterTableView DataKeyNames="Id" NoMasterRecordsText="No Records Available" TableLayout="Fixed"  Width="100%" >
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="InhouseSystemCode" FilterControlAltText="InhouseSystemCode" HeaderText="Inhouse System Code" SortExpression="InhouseSystemCode" UniqueName="InhouseSystemCode" HeaderStyle-Width="20%" FilterControlWidth="78%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Name" FilterControlAltText="Name" HeaderText="Name" SortExpression="Name" UniqueName="Name" HeaderStyle-Width="20%" FilterControlWidth="78%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Description" FilterControlAltText="Description" HeaderText="Description" SortExpression="Description" UniqueName="Description" HeaderStyle-Width="20%" FilterControlWidth="58%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="CreatedBy" FilterControlAltText="CreatedBy" HeaderText="CreatedBy" SortExpression="CreatedBy" UniqueName="CreatedBy" Visible="false" HeaderStyle-Width="20%" FilterControlWidth="78%" Exportable="true">
                </telerik:GridBoundColumn>
                <telerik:GridDateTimeColumn DataField="CreatedDate" FilterControlAltText="CreatedDate" HeaderText="CreatedDate" SortExpression="CreatedDate" UniqueName="CreatedDate"  Visible="false" HeaderStyle-Width="20%" FilterControlWidth="78%">
                </telerik:GridDateTimeColumn>
                <telerik:GridBoundColumn DataField="IsActive" FilterControlAltText="IsActive" HeaderText="IsActive" SortExpression="IsActive" UniqueName="IsActive" HeaderStyle-Width="20%" FilterControlWidth="88%">
                </telerik:GridBoundColumn> 
                <telerik:GridBoundColumn DataField="ValidationMessage" FilterControlAltText="ValidationMessage" HeaderText="Validation Message" SortExpression="ValidationMessage" UniqueName="ValidationMessage" HeaderStyle-Width="20%" FilterControlWidth="78%">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </cc1:IACMRadGridControl>

</div>
