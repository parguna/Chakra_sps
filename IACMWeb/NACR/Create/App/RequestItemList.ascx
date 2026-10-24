<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RequestItemList.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.RequestItemList" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%--<%@ Register Src="~/NACR/Create/App/RequestItemUC.ascx" TagPrefix="uc1" TagName="RequestItemUC" %>--%>
<%@ Register Src="~/NACR/Create/App/FilterUC.ascx" TagPrefix="uc1" TagName="FilterUC" %>
<%@ Register Src="~/NACR/Create/App/RequestItemUC.ascx" TagPrefix="uc1" TagName="RequestItemUC" %>


<style type="text/css">
    .RadGrid_IACMSkin .rgHeader .rgSortDesc{
        background-position: -5px -6px;
        width: 10px;
        display: inline-block;
        font-size:0px;
    }
    .RadGrid_IACMSkin .rgHeader .rgSortAsc{
        background-position: -25px 363px;
        width: 10px;
        display: inline-block;
        font-size:0px;
    }
    .RadGrid_IACMSkin .rgHeader .rgSortAsc.none{
        display: none;
    }
</style>
<div class="col-md-3">
    <uc1:FilterUC runat="server" ID="FilterUC" />
</div>
<div class="col-md-9">
    
    <telerik:RadListView runat="server" RenderMode="Lightweight" ID="requestList" PageSize="10" AllowPaging="True" OnNeedDataSource="requestList_NeedDataSource"
        OnItemDataBound="requestList_ItemDataBound" ItemPlaceholderID="itemPlaceholder" AllowStableSort="true" AllowCustomSorting="true" OnSorting="requestList_Sorting" ClientIDMode="Static" AllowMultiFieldSorting="true"
        >
        <ItemTemplate>
            <uc1:RequestItemUC runat="server" ID="RequestItemUC"  OnItemClicked="RequestItemUC_ItemClicked" />
            <%--<uc1:RequestItemUC runat="server" ID="RequestItemUC1" OnItemClicked="RequestItemUC_ItemClicked" />--%>
        </ItemTemplate>
        <LayoutTemplate>
            <asp:UpdatePanel runat="server" UpdateMode="Conditional"><ContentTemplate>
            <div class="RadGrid RadGrid_IACMSkin">
                <table class="rgMasterTableList rgClipCells" style="width: 100%; table-layout: fixed; empty-cells: show;">
                    <thead>
                        <tr>
                            <th class="rgHeader" width="50%">
                                <asp:LinkButton runat="server" ID="SortAccessName" CommandArgument="ItemDisplayName" OnCommand="SortAccessName_Command">Access Name</asp:LinkButton>
                                <asp:ImageButton runat="server" ID="ItemDisplayNameAsc" CssClass="rgSortAsc none" />
                            </th>
                            <th class="rgHeader" width="15%">
                                <asp:LinkButton runat="server" ID="SortCategory" CommandArgument="EntitlementType" OnCommand="SortAccessName_Command">Category</asp:LinkButton>
                                <asp:ImageButton runat="server" ID="EntitlementTypeAsc" CssClass="rgSortAsc none" />
                            </th>
                            <th class="rgHeader" width="15%">
                                <asp:LinkButton runat="server" ID="SortType" CommandArgument="AccessItemType" OnCommand="SortAccessName_Command">Type</asp:LinkButton>
                                <asp:ImageButton runat="server" ID="AccessItemTypeAsc" CssClass="rgSortAsc none" />
                            </th>
                            <th class="rgHeader" width="5%">
                                Info
                            </th>
                            <th class="rgHeader" width="15%">&nbsp;</th>
                        </tr>
                    </thead>
                    <tfoot>
                        <tr>
                            <td colspan="5">
                                <telerik:RadDataPager RenderMode="Lightweight" ID="RadDataPager1" runat="server" Skin="IACMSkin">
                                    <Fields>
                                        <telerik:RadDataPagerButtonField FieldType="FirstPrev"></telerik:RadDataPagerButtonField>
                                        <telerik:RadDataPagerButtonField FieldType="Numeric"></telerik:RadDataPagerButtonField>
                                        <telerik:RadDataPagerButtonField FieldType="NextLast"></telerik:RadDataPagerButtonField>
                                        <telerik:RadDataPagerPageSizeField PageSizeComboWidth="60" PageSizeText="Page size: " />
                                        <telerik:RadDataPagerTemplatePageField>
                                            <PagerTemplate>
                                                <div class="rgInfoPart">
                                                    Displaying items
                                                    <asp:Label runat="server" ID="CurrentPageLabel" Text="<%# Container.Owner.StartRowIndex + 1 %>" />
                                                                                    to
                                                    <asp:Label runat="server" ID="TotalPagesLabel" Text="<%# Container.Owner.StartRowIndex + Container.Owner.PageSize %>" />
                                                                                    of
                                                    <asp:Label runat="server" ID="TotalItemsLabel" Text="<%# Container.Owner.TotalRowCount %>" />
                                                </div>
                                            </PagerTemplate>
                                        </telerik:RadDataPagerTemplatePageField>
                                    </Fields>
                                </telerik:RadDataPager>
                            </td>
                        </tr>
                    </tfoot>
                    <tbody>
                        <tr id="itemPlaceholder" runat="server">
                        </tr>
                    </tbody>

                </table>
                </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="SortAccessName" />
            <asp:AsyncPostBackTrigger ControlID="SortCategory" />
            <asp:AsyncPostBackTrigger ControlID="SortType" />
        </Triggers>
    </asp:UpdatePanel>

            </div>
        </LayoutTemplate>
        
    </telerik:RadListView>
    <%--<asp:Repeater ID="RequestRepeater" runat="server" OnItemDataBound="RequestRepeater_ItemDataBound">
            <ItemTemplate>


                <uc1:RequestItemUC runat="server" ID="RequestItemUC"  OnItemClicked="RequestItemUC_ItemClicked" />

            </ItemTemplate>
        </asp:Repeater>--%>
</div>
