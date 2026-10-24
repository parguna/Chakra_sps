<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Master/ProtoMast4.master" CodeBehind="ExceptionAttribute.aspx.cs" Inherits="DI.Chakra.Web.Admin.Recon.ExceptionAttribute" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="uc1" TagName="EndpointUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="content-wrapper">
                <div class="form">
                    <div class="form-horizontal left clearfix">
                        <div class="row-fluid">
                            <div class="col-xs-6">
                                <uc1:EndpointUC runat="server" ID="EndpointUC" DisplayControl="ApplicationType" OnApplicationChanged="EndpointUC_ApplicationChanged"/>
                            </div>
                            <div class="col-xs-6 clearLeft">
                                <div class="form-group required">
                                    <label class="col-xs-4 control-label required">Exception Attributes</label>
                                    <div class="col-xs-7">
                                        <asp:dropdownlist id="ddlAttributes" runat="server" cssclass="form-control selectpicker" data-width="100%" autopostback="True">
                                        </asp:dropdownlist>
                                    </div>
                                    <div class="col-xs-1">
                                        <asp:Button runat="server" ID="AddUser" TabIndex="6" CssClass="btn btn-default btn-sm" Text="Add" OnClick="AddUser_Click" />
                                    </div>
                                </div>
                            </div>
                               
                        </div>
                    </div>
                </div>
            </div>

            <telerik:RadGrid ID="AttributesGrid" ClientSettings-EnablePostBackOnRowClick="true" FilterMenu-CssClass="RadMenu_IACMFilterSkin" AllowFilteringByColumn="False" OnNeedDataSource="AttributesGrid_NeedDataSource"
                runat="server" AllowPaging="true" AllowSorting="true" CellSpacing="0" OnItemCommand="AttributesGrid_ItemCommand" width="44.5%" EnableViewState="true">
                <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" AlwaysVisible="true" />
                <ClientSettings>
                    <Scrolling UseStaticHeaders="true" />
                    <Selecting AllowRowSelect="true" />
                </ClientSettings>
                <GroupingSettings CaseSensitive="false" />
                <MasterTableView AutoGenerateColumns="False" Width="100%"  CommandItemDisplay="None" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available" EnableHeaderContextMenu="false" DataKeyNames="Id">
                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                    <Columns>
                        <telerik:GridBoundColumn DataField="Attributes" FilterControlAltText="Attributes" SortExpression="Attributes" HeaderText="Attributes" AllowFiltering="false" AllowSorting="false">
                        </telerik:GridBoundColumn>
                        <telerik:GridButtonColumn ConfirmText="Are you sure to delete the selected Attribute?" ConfirmDialogType="RadWindow" HeaderText="Delete" HeaderStyle-HorizontalAlign="Center"
                            ConfirmTitle="Delete" ButtonType="ImageButton" CommandName="Delete" Text="Delete"
                            UniqueName="DeleteColumn" Exportable="false" HeaderStyle-Width="150px">
                            <ItemStyle HorizontalAlign="Center" CssClass="action-delete" />
                        </telerik:GridButtonColumn>
                    </Columns>
                </MasterTableView>
                <HeaderContextMenu></HeaderContextMenu>
            </telerik:RadGrid>

            <div class="buttons text-right nav-rectangle">
                <asp:Button runat="server" class="btn btn-default btn-sm" ID="Button1" Text="Cancel" OnClick="Button1_Click" UseSubmitBehavior="false"></asp:Button>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
