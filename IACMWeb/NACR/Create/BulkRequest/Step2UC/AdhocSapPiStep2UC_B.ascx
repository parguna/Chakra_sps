<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocSapPiStep2UC_B.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC.AdhocSapPiStep2UC_B" %>



<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<div class="form">

    <div class="form-horizontal left clearfix">
        <asp:HiddenField ID="HiddenField1" runat="server" />
        <asp:HiddenField ID="HiddenField2" runat="server" />
        <div class="row-fluid">
            <div class="form-group">
                <div class="col-xs-8">
                    <asp:Panel ID="Panel1" runat="server" Visible="false">

                        <div class="form-group">
                            <div class="col-xs-4">
                                <label class="control-label">Account Information: </label>
                            </div>
                        </div>

                        <telerik:RadGrid ID="AccountRepeater" runat="server" AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                            GridLines="None" ShowFooter="false" MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Request Available">
                            <GroupingSettings CaseSensitive="false" />
                            <ClientSettings EnableRowHoverStyle="true">
                                <Scrolling UseStaticHeaders="true" />
                                <Selecting AllowRowSelect="true" />
                            </ClientSettings>
                            <MasterTableView AutoGenerateColumns="False" DataKeyNames="key,Value">
                                <Columns>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="60%" HeaderText="User" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAcountName" runat="server" CssClass="required" Text='<%# Eval("key") %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                    <telerik:GridTemplateColumn HeaderStyle-Width="40%" HeaderText="Accounts" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <telerik:RadComboBox ID="ddlAccounts" runat="server" CssClass="selectpicker  AccountsPageComoboWidth radbox100Width" DataSource='<%# Eval("Value")%>' DataTextField="value" DataValueField="key"></telerik:RadComboBox>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>

                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>


    <div class="form-horizontal left clearfix">

        <div class="row-fluid">
            <div class="form-group">
                <div class="col-xs-6">

                    <asp:Panel ID="GroupPanelFLINSBI" runat="server">

                        <asp:Panel ID="OtherControls" runat="server">

                            <div id="Module" class="form-group">
                                <label class="col-xs-3 control-label">Module</label>
                                <div class="col-xs-8">
                                    <asp:DropDownList ID="ModuleDropDownList" runat="server" AutoPostBack="true" CssClass="form-control selectpicker" OnSelectedIndexChanged="ddlModule_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="col-xs-3 control-label">Sub Modules</label>
                                <div class="col-xs-8">
                                    <asp:DropDownList ID="SubModuleDropDownList" runat="server" AutoPostBack="true" CssClass="form-control selectpicker" OnSelectedIndexChanged="ddlSubModule_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div id="BusinessRoleAsterix" class="form-group" runat="server">
                                <label class="col-xs-3 control-label">Business Role</label>
                                <div class="col-xs-8">
                                    <telerik:RadAutoCompleteBox ID="BusinessRoleAutoCompleteBox" runat="server" EmptyMessage="Enter * to view full list" TextSettings-SelectionMode="Single"
                                        InputType="Token" CssClass="form-control" Rows="4" required="required"  EnableAjaxSkinRendering="true" RegisterWithScriptManager="true">
                                        <%--OnClientRequesting="RequestingFromBusinessRole"--%>
                                        <WebServiceSettings Path="../../../../Services/AutoCompleteDataService.svc" Method="GetBusinessRoleFromSAPPIRole" />
                                        <TextSettings SelectionMode="Single" />
                                    </telerik:RadAutoCompleteBox>
                                </div>
                            </div>

                        </asp:Panel>

                    </asp:Panel>

                    <div class="form-group">
                        <label class="col-xs-3 control-label"></label>
                        <div class="col-xs-8">
                            <asp:Button ID="DeleteAccountAdd" CssClass="btn btn-default btn-sm" OnClick="AddButton_Click" Text="Add" runat="server" />

                            <asp:Button ID="Button1" CssClass="btn btn-default btn-sm" OnClick="Save_Click" Text="Save Request" runat="server" Visible="false"/>
                        </div>
                    </div>

                </div>
            </div>


            <table border="0" cellpadding="0" cellspacing="0" width="99%" style="margin-left: 5px">
                <tr>
                    <td>
                        <telerik:RadGrid ID="RequestGrid" runat="server" AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                            GridLines="None" Width="1200px" OnNeedDataSource="RequestGrid_NeedDataSource" OnItemDataBound="RequestGrid_ItemDataBound" OnItemCommand="RequestGrid_ItemCommand" ShowFooter="false"
                            MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Request Available">
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="False" DataKeyNames="" Width="100%">
                                <Columns>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Business Role" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="EntitlementLabel" runat="server" Text="<%# Container.DataItem %>" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="300px" HeaderText="Technical role" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="TechnicalRoleLabel" runat="server" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridButtonColumn ConfirmDialogType="RadWindow" ConfirmTitle="Delete" UniqueName="Delete" ConfirmText="Are you sure you want to delete?" CommandName="Delete" ButtonType="ImageButton" ItemStyle-CssClass="action-delete" ItemStyle-Width="1px" HeaderText="">
                                        <HeaderStyle Width="60px" />

                                    </telerik:GridButtonColumn>
                                    <%--<telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="Delete">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="DeleteRoleButton" runat="server" CommandName="Delete" Text="Delete" ToolTip="Delete" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>--%>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>


<telerik:RadWindow runat="server" ID="ViewTechnicalSourceWindow" Modal="true" Height="100%" Width="700px" Title="Techinical Roles"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Panel ID="panelViewTechnicalSource" CssClass="ModalWindow" Style="height: 68%; width: 47%; position: fixed; z-index: 10001; left: 346.5px; top: 50.5px;" runat="server" Width="75%" Height="80%">

                    <telerik:RadGrid runat="server" ID="ViewTechnicalSource" GridLines="None" AllowAutomaticUpdates="false" Width="100%" EnableAjaxSkinRendering="true" AllowPaging="False" CellSpacing="0">
                        <ClientSettings>
                            <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true" FrozenColumnsCount="2"></Scrolling>
                        </ClientSettings>
                        <PagerStyle Mode="NumericPages" />
                        <GroupingSettings CaseSensitive="false" />
                        <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="true" EditFormSettings-PopUpSettings-ScrollBars="Vertical" Width="100%">
                            <Columns>
                                <telerik:GridBoundColumn DataField="Value" HeaderText="Technical Role" HeaderStyle-Width="10%" FilterControlWidth="94%">
                                </telerik:GridBoundColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>

                    <asp:Button ID="btnCancel" CssClass="uiboxgray" runat="server" Text="Close" OnClick="btnCancel_Click" />
                </asp:Panel>

            </ContentTemplate>

        </asp:UpdatePanel>

    </ContentTemplate>

</telerik:RadWindow>
