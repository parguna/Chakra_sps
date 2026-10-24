<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MappingDetailUC.ascx.cs" Inherits="DI.Chakra.Web.EndPoints.EndpointSystem.Common.MappingDetailUC" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-wrapper" runat="server" id="contentwrapper">
    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">

                    <div class="form-group">
                        <asp:Label ID="lblUserObject" AssociatedControlID="UserObjectAutoCompleteBox" runat="server" Text="User Object" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <telerik:RadAutoCompleteBox ID="UserObjectAutoCompleteBox" runat="server" Width="320px" EmptyMessage="Enter * to view full list"
                                DropDownWidth="320px" InputType="Token" CssClass="form-control" TextSettings-SelectionMode="Single">
                                <WebServiceSettings Path="../../Services/AutoCompleteDataService.svc" Method="GetUserObjectNameList" />
                            </telerik:RadAutoCompleteBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-xs-7">
                            <div class="radio">
                                <asp:Label ID="Label5" runat="server" AssociatedControlID="rbtMapAnyOne">
                                    <asp:RadioButton runat="server" GroupName="Map" ID="rbtMapAnyOne"></asp:RadioButton>
                                    <span class="lbl">Map Any One</span>
                                </asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-xs-7">
                            <div class="radio">
                                <asp:Label ID="Label1" runat="server" AssociatedControlID="rbtMapAll">
                                    <asp:RadioButton runat="server" GroupName="Map" Checked="True" ID="rbtMapAll"></asp:RadioButton>
                                    <span class="lbl">Map All</span>
                                </asp:Label>
                            </div>
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="">
                            <telerik:RadGrid ID="MappingGrid" ClientSettings-EnablePostBackOnRowClick="true" FilterMenu-CssClass="RadMenu_IACMFilterSkin" AllowFilteringByColumn="False" OnNeedDataSource="MappingGrid_NeedDataSource"
                                runat="server" AllowPaging="true" AllowSorting="true" CellSpacing="0" OnItemCommand="MappingGrid_ItemCommand">
                                <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" AlwaysVisible="false" />
                                <ClientSettings>
                                    <Scrolling UseStaticHeaders="true" />
                                    <Selecting AllowRowSelect="true" />
                                </ClientSettings>
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView Width="100%"  AutoGenerateColumns="False" CommandItemDisplay="None" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available" EnableHeaderContextMenu="false" DataKeyNames="Id">
                                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="DiColumn" HeaderText="IDMS Field" UniqueName="DiColumn" HeaderStyle-Width="23.25%">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="TgColumn" HeaderText="Target Application Field" UniqueName="TgColumn" HeaderStyle-Width="23.25%">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridButtonColumn ConfirmText="Delete Attribute?" ConfirmDialogType="RadWindow"
                                            ConfirmTitle="Delete" ButtonType="ImageButton" CommandName="Delete" Text="Delete"
                                            UniqueName="DeleteColumn" Exportable="false" HeaderText="Delete" HeaderStyle-Width="7%">
                                            <ItemStyle HorizontalAlign="Center" CssClass="action-delete" />
                                        </telerik:GridButtonColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                            <div class="buttons text-left topBuffer">
                                <asp:Button ID="btnAddAttribute" CssClass="btn btn-default btn-sm" OnClick="btnAddAttribute_Click" runat="server" Text="Add Attribute"></asp:Button>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<asp:Button ID="ButtonHidden" Style="display: none;" CssClass="btn btn-default btn-sm" data-toggle="modal" data-target="#myModal" runat="server" Text="Add Attribute"></asp:Button>

<telerik:RadWindow runat="server" ID="AddAttributeradWin" Modal="true" Width="500px" Height="100%" Title="Add Attribute"
    Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false" RenderMode="Lightweight" >
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title">Add Attribute Mapping</h3>
                </div>
                <div class="modal-body">
                    <div class="form-horizontal left clearfix">
                        <div class="row-fluid">
                            <div class="col-xs-12">
                                <div class="form-group">
                                    <asp:Label ID="lblUser" runat="server" Text="User" AssociatedControlID="ddlDi" class="col-xs-4 control-label"></asp:Label>
                                    <div class="col-xs-8">
                                        <asp:DropDownList ID="ddlDi" class="form-control selectpicker" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label ID="lblTg" runat="server" AssociatedControlID="ddlTg" class="col-xs-4 control-label"><%= T("App.Common.Endpoint") %> Columns</asp:Label>
                                    <div class="col-xs-8">
                                        <asp:DropDownList ID="ddlTg" runat="server" class="form-control selectpicker numeric"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnCancel" class="btn btn-primary transparent-button" Text="Cancel" OnClick="btnCancel_Click" />
                    <asp:Button runat="server" ID="btnAdd" class="btn btn-primary bordered-button" Text="OK" OnClick="btnAdd_Click1" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>
