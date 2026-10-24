<%@ Page Language="C#" Title="Metadata Editor" AutoEventWireup="true" MasterPageFile="~/Master/ProtoMast4.master" CodeBehind="MetadataEditor.aspx.cs" Inherits="DI.Chakra.Web.Metadata.MetadataEditor" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="Server">
    <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server">
        <div class="content-wrapper">
            <div class="form">
                <div class="form-horizontal left">
                    <div class="row-fluid" style="padding-left: 0px;">
                        <div class="col-sm-6" style="padding-left: 0px;">
                            <label class="col-xs-4 control-label" style="padding-left: 0px;">Metadata Type</label>
                            <div id="DivApplicationDropDown" class="col-xs-7" runat="server">
                                <asp:DropDownList ID="drpMetadataType" runat="server" CssClass="form-control selectpicker" data-width="150px" AutoPostBack="True"
                                    OnSelectedIndexChanged="drpMetadataType_SelectedIndexChanged" DataTextField="MetadataValue" DataValueField="MetadataKey">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        &nbsp;&nbsp;&nbsp;
    <div>

        <telerik:RadGrid ID="grdMetadata" runat="server" FilterMenu-CssClass="RadMenu_IACMFilterSkin" OnItemCommand="grdMetadata_ItemCommand"
            AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" CellSpacing="0" OnNeedDataSource="grdMetadata_NeedDataSource" OnItemDataBound="grdMetadata_ItemDataBound" Width="100%">
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
                    <telerik:GridBoundColumn DataField="Description" FilterControlAltText="Description" HeaderText="Description"
                        SortExpression="Description" UniqueName="Description" HeaderStyle-Width="200px" FilterControlWidth="55%">
                    </telerik:GridBoundColumn>
                    <telerik:GridDropDownColumn UniqueName="IsActive" ListDataMember="IsActive" ColumnEditorID="IsActiveColumnEditor" HeaderStyle-Width="50px"
                        SortExpression="IsActive" ListTextField="IsActive"
                        ListValueField="IsActive" HeaderText="IsActive" DataField="IsActive">
                    </telerik:GridDropDownColumn>
                    <telerik:GridEditCommandColumn ButtonType="ImageButton" ItemStyle-Height="20px" ItemStyle-CssClass="action-edit" HeaderText="" HeaderStyle-Width="20px" UpdateText="Update" CancelText="Cancel" UniqueName="EditCommandColumn" ItemStyle-Width="7px" EditImageUrl="../Images/action-buttons/edit.png" CancelImageUrl="~/Images/action-buttons/reject.png" UpdateImageUrl="~/Images/action-buttons/save.png">
                    </telerik:GridEditCommandColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>

    </div>
    </telerik:RadAjaxPanel>
</asp:Content>
