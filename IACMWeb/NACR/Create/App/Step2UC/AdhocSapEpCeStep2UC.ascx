<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocSapEpCeStep2UC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.Step2UC.AdhocSapEpCeStep2UC" %>



<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/SecurityQuestionUC.ascx" TagPrefix="uc1" TagName="SecurityQuestionUC" %>




<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>

        <div class="form">
            <div class="form-horizontal left clearfix">
                <div class="row-fluid">
                    <div class="col-xs-6">

                        <asp:Panel ID="SharedPanelPLRELearn" runat="server" Visible="false">

                            <div class="form-group required">
                                <label class="col-xs-4 control-label">Account Name</label>
                                <div class="col-xs-7">
                                    <asp:TextBox ID="txtAcntName" runat="server" MaxLength="20">
                                    </asp:TextBox>

                                </div>
                            </div>

                            <div class="form-group">
                                <label class="col-xs-4 control-label">Description</label>
                                <div class="col-xs-7">

                                    <asp:TextBox ID="txtDiscription" runat="server" TextMode="MultiLine" CssClass="form-control" MaxLength="255">
                                    </asp:TextBox>
                                </div>
                            </div>
                        </asp:Panel>


                        <asp:Panel ID="GroupPanelPLRELearn" runat="server">

                            <div id="trEndPointUser" runat="server" visible="false" class="form-group required">
                                <label class="col-xs-4 control-label">EndPoint User</label>
                                <div class="col-xs-7">

                                    <asp:DropDownList ID="ddlEndpointUser" runat="server" AutoPostBack="true" OnSelectedIndexChanged="EndPointUserDropDownList_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <asp:Panel ID="OtherControls" runat="server">

                                <div class="form-group required">
                                    <label class="col-xs-4 control-label">Request Info</label>
                                    <div class="col-xs-7">
                                        <asp:TextBox ID="txtRequestDetails" TextMode="MultiLine" runat="server" CssClass="form-control">
                                        </asp:TextBox>
                                    </div>
                                </div>

                                <div class="form-group required" id="divEntitlementType" runat="server" visible="false">
                                    <label class="col-xs-4 control-label">Entitlement Type</label>
                                    <div class="col-xs-7">
                                        <asp:DropDownList ID="ddlEntitlementType" AutoPostBack="true" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlEntitlementType_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div id="divRole" runat="server" visible="false" class="form-group">
                                    <label class="col-xs-4 control-label">Role</label>
                                    <div class="col-xs-5">
                                        <telerik:RadAutoCompleteBox ID="RolesAutoCompleteBox" CssClass="form-control" runat="server" EmptyMessage="Enter * to view full list" OnEntryRemoved="AutoCompleteBox_EntryRemoved" OnEntryAdded="AutoCompleteBox_EntryAdded" TextSettings-SelectionMode="Single" InputType="Token">
                                            <TextSettings SelectionMode="Single" />
                                            <WebServiceSettings Method="GetEpCeRoles" Path="../../../../Services/AutoCompleteDataService.svc" />
                                        </telerik:RadAutoCompleteBox>
                                    </div>
                                    <div class="col-xs-2">
                                        <asp:Button ID="btnRoleAdd" CssClass="btn btn-default btn-sm" OnClick="btnAdd_Click" Text="Add Role" runat="server" />
                                    </div>
                                </div>

                                <div id="divGroup" runat="server" visible="false" class="form-group">
                                    <label class="col-xs-4 control-label">Group</label>
                                    <div class="col-xs-5">
                                        <telerik:RadAutoCompleteBox ID="GroupAutoCompleteBox" CssClass="form-control" runat="server" EmptyMessage="Enter * to view full list" OnEntryRemoved="AutoCompleteBox_EntryRemoved" OnEntryAdded="AutoCompleteBox_EntryAdded" TextSettings-SelectionMode="Single" InputType="Token">
                                            <TextSettings SelectionMode="Single" />
                                            <WebServiceSettings Method="GetEpCeGroups" Path="../../../../Services/AutoCompleteDataService.svc" />
                                        </telerik:RadAutoCompleteBox>
                                    </div>
                                    <div class="col-xs-2">
                                        <asp:Button ID="btnGroupAdd" CssClass="btn btn-default btn-sm" OnClick="btnAdd_Click" Text="Add Group" runat="server" />
                                    </div>
                                </div>

                                <div class="form-group">
                                    <div class="col-xs-4"></div>

                                    <asp:Panel ID="pnlSecurityQstns" CssClass="col-xs-7" runat="server">
                                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                            <ContentTemplate>
                                                <asp:Panel ID="pnlSecurityPopup" runat="server">
                                                    <uc1:SecurityQuestionUC runat="server" ID="SecurityQuestionUC" OnSecurityQuestionPopUp="SecurityQuestionUC_SecurityQuestionPopUp" OnSecurityResponseSaveClicked="SecurityQuestion_SecurityResponseSaveClicked" LinkText="Service account compliance check" />
                                                </asp:Panel>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </asp:Panel>


                                </div>
                                <div class="form-group">
                                    <div class="col-xs-4"></div>
                                    <div class="col-xs-7">
                                        <asp:Button ID="saveRequest" CssClass="btn btn-default btn-sm" OnClick="AddToCart" Text="Save Request" runat="server" Visible="false"/>

                                    </div>
                                </div>
                            </asp:Panel>
                        </asp:Panel>
                    </div>
                </div>

                <div class="col-xs-8" visible="false">
                    <table border="0" cellpadding="0" cellspacing="0" width="Auto" style="margin-left: 5px">
                        <tr id="trGrdid" runat="server">
                            <td>
                                <telerik:RadGrid ID="RequestGrid" runat="server" AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                                    GridLines="None" Width="1200px" OnItemCommand="RequestGrid_ItemCommand" ShowFooter="false"
                                    MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Request Available">
                                    <GroupingSettings CaseSensitive="false" />
                                    <MasterTableView AutoGenerateColumns="False" DataKeyNames="" Width="100%">
                                        <Columns>
                                            <telerik:GridTemplateColumn HeaderStyle-Width="50px" HeaderText="Role" ItemStyle-HorizontalAlign="Left">
                                                <ItemTemplate>
                                                    <asp:Label ID="EntitlementType" runat="server" Text='<%#Eval("EntitlementType") %>' />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Role" ItemStyle-HorizontalAlign="Left">
                                                <ItemTemplate>
                                                    <asp:Label ID="EntitlementLabel" runat="server" Text='<%#Eval("Entitlement") %>' />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridButtonColumn ConfirmDialogType="RadWindow" ConfirmTitle="Delete" UniqueName="Delete" ConfirmText="Are you sure you want to delete?" CommandName="Delete" ButtonType="ImageButton" ItemStyle-CssClass="action-delete" ItemStyle-Width="1px" HeaderText="">
                                                <HeaderStyle Width="30px" />

                                            </telerik:GridButtonColumn>

                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
