<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Configuration.Data.XLUpload.SAPEP.UC.ListUC" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Configuration/Data/Endpoint/XLUploadUC.ascx" TagPrefix="uc1" TagName="XLUploadUC" %>

<div class="content-wrapper">
    <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server">
<%--    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnAddUploud">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="modalPopup" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>--%>

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

    <telerik:RadGrid ID="grdEntries" AllowFilteringByColumn="true" OnItemCommand="grdEntries_ItemCommand" FilterMenu-CssClass="RadMenu_IACMFilterSkin" OnNeedDataSource="grdEntries_NeedDataSource" AllowPaging="True" AllowSorting="True" CellSpacing="0" runat="server">
        <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true" PageSizes="5" Height="100%" />

        <ClientSettings>
            <Scrolling AllowScroll="true" UseStaticHeaders="true" />
            <Selecting AllowRowSelect="true" />
        </ClientSettings>

        <GroupingSettings CaseSensitive="false" />
        <MasterTableView DataKeyNames="Id" EditMode="InPlace" AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="false" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridEditCommandColumn ButtonType="ImageButton" ItemStyle-Height="20px" ItemStyle-CssClass="action-edit" HeaderText="" HeaderStyle-Width="30px" UpdateText="Update" CancelText="Cancel" UniqueName="EditCommandColumn" ItemStyle-Width="7px" EditImageUrl="../Images/action-buttons/edit.png" CancelImageUrl="~/Images/action-buttons/reject.png" UpdateImageUrl="~/Images/action-buttons/save.png">
                </telerik:GridEditCommandColumn>
                <telerik:GridButtonColumn ConfirmDialogType="RadWindow" ConfirmTitle="Delete" ConfirmText="Are you sure you want to delete?" CommandName="Delete" ButtonType="ImageButton" ItemStyle-CssClass="action-delete" HeaderStyle-Width="30px" HeaderText="" ItemStyle-Width="7px" />
                <telerik:GridBoundColumn DataField="CompanyCode" FilterControlAltText="CompanyCode" HeaderText="Company Code"
                    SortExpression="CompanyCode" UniqueName="CompanyCode" HeaderStyle-Width="260px" FilterControlWidth="79%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="DepartmentCode" FilterControlAltText="DepartmentCode" HeaderText="Department Code"
                    SortExpression="DepartmentCode" UniqueName="DepartmentCode" HeaderStyle-Width="260px" FilterControlWidth="79%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="DivisionCode" FilterControlAltText="DivisionCode" HeaderText="Division Code"
                    SortExpression="DivisionCode" UniqueName="DivisionCode" HeaderStyle-Width="260px" FilterControlWidth="79%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="OrganisationId" FilterControlAltText="OrganisationId" HeaderText="Organisation Id"
                    SortExpression="OrganisationId" UniqueName="OrganisationId" HeaderStyle-Width="260px" FilterControlWidth="79%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="OrganisationName" FilterControlAltText="OrganisationName" HeaderText="Organisation Name"
                    SortExpression="OrganisationName" UniqueName="OrganisationName" HeaderStyle-Width="260px" FilterControlWidth="79%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="CostCenterCode" FilterControlAltText="CostCenterCode" HeaderText="Cost Center Code"
                    SortExpression="CostCenterCode" UniqueName="CostCenterCode" HeaderStyle-Width="260px" FilterControlWidth="79%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="CostDistributionCode" FilterControlAltText="CostDistributionCode" HeaderText="Cost Distribution Code"
                    SortExpression="CostDistributionCode" UniqueName="CostDistributionCode" HeaderStyle-Width="260px" FilterControlWidth="79%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="OU" FilterControlAltText="OU" HeaderText="Organisation Unit"
                    SortExpression="OU" UniqueName="OU" HeaderStyle-Width="260px" FilterControlWidth="79%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="DefaultGroups" FilterControlAltText="DefaultGroups" HeaderText="Default Groups"
                    SortExpression="DefaultGroups" UniqueName="DefaultGroups" HeaderStyle-Width="260px" FilterControlWidth="80%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="ValidationMessage" FilterControlAltText="ValidationMessage" HeaderText="Validation Message"
                    SortExpression="ValidationMessage" UniqueName="ValidationMessage" HeaderStyle-Width="260px" FilterControlWidth="80%">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</telerik:RadAjaxPanel>
</div>
<%--</asp:Content>--%>