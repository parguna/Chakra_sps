<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Admin.AppText.UC.ListUC" %>
<%@ Register Src="~/Admin/AppText/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<style>
    .rwTitleRow
    {
        display: none;
    }

    .modal-footer
    {
        background-color: #383f46;
        padding: 8px 15px;
    }
    .rwStatusbarRow
    {
        display: none;
    }

    .modal-header
    {
        text-align: center;
        color: #ff2700;
        border-bottom: 1px solid #383f46;
        font-size: 16px;
    }

        .modal-header h3
        {
            font-size: 1em;
        }
</style>
<div class="content-wrapper">
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnEditText">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="modalPopup" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>

    <telerik:RadWindow ID="modalPopup" runat="server" Width="450px" Height="187px" Modal="true" OffsetElementID="main" VisibleTitlebar="false">
        <ContentTemplate>
            <div class="modal-header">
                <h3 class="modal-title" runat="server" id="H1">Edit Application Text</h3>
            </div>
            <div class="modal-body">
                <uc1:DetailUC runat="server" ID="DetailUC" />
            </div>
            <div class="modal-footer">
                <asp:Button runat="server" class="btn btn-primary transparent-button" ID="btnCancel" Text="Cancel" data-dismiss="modal" OnClick="btnCancel_Click"></asp:Button>
                <asp:Button ID="btnSubmit" CssClass="add btn btn-primary bordered-button" runat="server"
                    Text="Update" OnClick="btnSubmit_Click"></asp:Button>
            </div>
        </ContentTemplate>
    </telerik:RadWindow>
    <asp:XmlDataSource ID="XmlDataSource1" runat="server" DataFile="~/assets/apptexts/AppTexts.xml"
            EnableCaching="false"></asp:XmlDataSource>

    <telerik:RadGrid ID="grdAppTexts" runat="server" DataSourceID="XmlDataSource1" AutoGenerateColumns="False" AllowFilteringByColumn="true" FilterMenu-CssClass="RadMenu_IACMFilterSkin"
       
        AllowPaging="True" AllowSorting="True" CellSpacing="0" OnInit="grdAppTexts_Init" OnSelectedIndexChanged="grdAppTexts_SelectedIndexChanged">  <%--OnSelectedIndexChanged="grdAppTexts_SelectedIndexChanged"--%>
        <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" AlwaysVisible="true" />
        <ClientSettings>
            <Scrolling UseStaticHeaders="true" />
            <Selecting AllowRowSelect="true" />
        </ClientSettings>
        <GroupingSettings CaseSensitive="false" />
        <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="false" DataKeyNames="key"  Width="100%" 
            NoMasterRecordsText="There are no Application Texts to display.">
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
            <Columns>
                <telerik:GridBoundColumn DataField="key" FilterControlAltText="TextKey" HeaderText="Text Key" SortExpression="key" UniqueName="TextKey" HeaderStyle-Width="100" FilterControlWidth="50%">
                    <ColumnValidationSettings>
                        <ModelErrorMessage Text="" />
                    </ColumnValidationSettings>
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="value" FilterControlAltText="TextValue" HeaderText="Text Value" SortExpression="value" UniqueName="TextValue" HeaderStyle-Width="100" FilterControlWidth="70%">
                    <ColumnValidationSettings>
                        <ModelErrorMessage Text="" />
                    </ColumnValidationSettings>
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
    <div class="buttons text-right nav-rectangle">
        <asp:Button ID="btn_Cancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="btn_Cancel_Click" />
    </div>
</div>
