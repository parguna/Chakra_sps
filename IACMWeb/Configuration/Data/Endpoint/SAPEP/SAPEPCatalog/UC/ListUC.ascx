<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Configuration.Data.Endpoint.SAPEP.SAPEPCatalog.UC.ListUC" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Configuration/Data/Endpoint/XLUploadUC.ascx" TagPrefix="uc1" TagName="XLUploadUC" %>

<telerik:RadWindow ID="modalPopup" runat="server" Width="600px" Height="170px" Modal="true" OffsetElementID="main" VisibleTitlebar="false" VisibleStatusbar="false">
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
            </div>
        </div>
    </div>
</div>
<telerik:RadGrid ID="grdEntries" runat="server" FilterMenu-CssClass="RadMenu_IACMFilterSkin" OnItemCommand="grdEntries_ItemCommand"
    AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" CellSpacing="0" OnNeedDataSource="grdEntries_NeedDataSource" Width="100%">
    <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true" PageSizes="5" Height="85%" />
    <ClientSettings>
        <Scrolling AllowScroll="true" UseStaticHeaders="true" />
        <Selecting AllowRowSelect="true" />
    </ClientSettings>
    <GroupingSettings CaseSensitive="false" />
    <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="false" DataKeyNames="Id" EditMode="InPlace"
        ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridEditCommandColumn ButtonType="ImageButton" HeaderText="" HeaderStyle-Width="50px" UpdateText="Update" CancelText="Cancel" UniqueName="EditCommandColumn" ItemStyle-CssClass="action-edit" ItemStyle-Width="70px" EditImageUrl="../Images/action-buttons/edit.png" CancelImageUrl="~/Images/action-buttons/reject.png" UpdateImageUrl="~/Images/action-buttons/save.png">
            </telerik:GridEditCommandColumn>
            <telerik:GridButtonColumn ConfirmDialogType="RadWindow" ConfirmTitle="Delete" ConfirmText="Are you sure you want to delete?" CommandName="Delete" ButtonType="ImageButton" ItemStyle-CssClass="action-delete" ItemStyle-Width="1px" HeaderText="" HeaderStyle-Width="50px" />
            <telerik:GridBoundColumn DataField="Application" FilterControlAltText="Application" HeaderText="Application"
                SortExpression="Application" UniqueName="Application" HeaderStyle-Width="200px" FilterControlWidth="70%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="BusinessRole" FilterControlAltText="BusinessRole" HeaderText="Business Role"
                SortExpression="BusinessRole" UniqueName="BusinessRole" HeaderStyle-Width="200px" FilterControlWidth="70%">
            </telerik:GridBoundColumn>

            <telerik:GridBoundColumn DataField="Description" FilterControlAltText="Description" HeaderText="Description"
                SortExpression="Description" UniqueName="Description" HeaderStyle-Width="200px" FilterControlWidth="70%">
            </telerik:GridBoundColumn>

            <telerik:GridBoundColumn DataField="TechnicalRole" FilterControlAltText="TechnicalRole" HeaderText="Technical Role"
                SortExpression="TechnicalRole" UniqueName="TechnicalRole" HeaderStyle-Width="200px" FilterControlWidth="70%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Level1Approver" FilterControlAltText="Level1Approver" HeaderText="Level1 Approver"
                SortExpression="Level1Approver" UniqueName="Level1Approver" HeaderStyle-Width="200px" FilterControlWidth="70%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Level1ApproverType" FilterControlAltText="Level1ApproverType" HeaderText="Level1 Approver Type"
                SortExpression="Level2ApproverType" UniqueName="Level1ApproverType" HeaderStyle-Width="200px" FilterControlWidth="70%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Level2Approver" FilterControlAltText="Level2Approver" HeaderText="Level2Approver"
                SortExpression="Level2Approver" UniqueName="Level2Approver" HeaderStyle-Width="200px" FilterControlWidth="70%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Level2ApproverType" FilterControlAltText="Level2ApproverType" HeaderText="Level2 Approver Type"
                SortExpression="Level2ApproverType" UniqueName="Level2ApproverType" HeaderStyle-Width="200px" FilterControlWidth="70%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Level3Approver" FilterControlAltText="Level3Approver" HeaderText="Level3 Approver"
                SortExpression="Level3Approver" UniqueName="Level3Approver" HeaderStyle-Width="200px" FilterControlWidth="70%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Level3ApproverType" FilterControlAltText="Level3ApproverType" HeaderText="Level3 Approver Type"
                SortExpression="Level3ApproverType" UniqueName="Level3ApproverType" HeaderStyle-Width="200px" FilterControlWidth="70%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="ValidationMessage" FilterControlAltText="ValidationMessage" HeaderText="Validation Message"
                SortExpression="ValidationMessage" UniqueName="ValidationMessage" HeaderStyle-Width="200px" FilterControlWidth="70%">
            </telerik:GridBoundColumn>
        </Columns>
    </MasterTableView>
</telerik:RadGrid>

