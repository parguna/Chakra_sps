<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocGemsStep2UC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.Step2UC.AdhocGemsStep2UC" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/SecurityQuestionUC.ascx" TagPrefix="uc1" TagName="SecurityQuestionUC" %>

<div class="form">
    <div class="form-horizontal left clearfix">
        <asp:HiddenField ID="Mandatory1HiddenField" runat="server" />
        <asp:HiddenField ID="Mandatory2HiddenField" runat="server" />
        <div class="row-fluid">
            <div class="form-group">
                <div class="col-xs-6">
                    <asp:Panel ID="SharedPanelGEMS" runat="server" Visible="false">

                        <div class="form-group required">
                            <label class="col-xs-3 control-label">Account Name</label>
                            <div class="col-xs-8">
                                <asp:TextBox ID="txtAccountName" runat="server" CssClass="form-control" MaxLength="20">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group required">
                            <label class="col-xs-3 control-label">Email Id</label>
                            <div class="col-xs-8">
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" MaxLength="60">
                                </asp:TextBox>
                            </div>
                        </div>


                        <div class="form-group required">
                            <label class="col-xs-3 control-label">Display Name</label>
                            <div class="col-xs-8">
                                <asp:TextBox ID="txtDisplayName" runat="server" CssClass="form-control" MaxLength="60">
                                </asp:TextBox>
                            </div>
                        </div>


                        <div class="form-group required">
                            <label class="col-xs-3 control-label">Description</label>
                            <div class="col-xs-8">
                                <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" MaxLength="255">
                                </asp:TextBox>
                            </div>
                        </div>


                    </asp:Panel>


                    <div id="trEndPointUser" runat="server" visible="false" class="form-group">
                        <label class="col-xs-3 control-label">EndPoint User<span style="color:red">*</span></label>
                        <div class="col-xs-8">  
                            <asp:DropDownList ID="EndPointUserDropDownList" runat="server" CssClass="form-control" Width="400px" AutoPostBack="true" OnSelectedIndexChanged="EndPointUserDropDownList_SelectedIndexChanged">
                            </asp:DropDownList><br />
                            <br />
                        </div>
                    </div>

                    <asp:Panel ID="GroupPanelGEMS" runat="server">

                        <div id="RoleAstreix" runat="server" class="form-group required">
                            <label class="col-xs-3 control-label required">Roles</label>
                            <div class="col-xs-8">
                                <telerik:RadAutoCompleteBox ID="RolesAutoCompleteBox" CssClass="form-control" runat="server" EmptyMessage="Enter * to view full list" TextSettings-SelectionMode="Single" InputType="Token" OnClientEntryAdding="OnClientEntryGemsAccessRoleAddingHandler">
                                     <TextSettings SelectionMode="Single" />
                                    <WebServiceSettings Method="GetEntitlementGEMS" Path="../../../../Services/AutoCompleteDataService.svc" />
                                </telerik:RadAutoCompleteBox>
                            </div>
                        </div>

                    </asp:Panel>

                    <div class="form-group" id="trSecurityQstns" runat="server" visible="false">
                        <div class="col-xs-3">
                        </div>
                        <div class="col-xs-8">
                            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                <ContentTemplate>
                                    <asp:Panel ID="Panel2" runat="server">
                                        <uc1:SecurityQuestionUC runat="server" ID="SecurityQuestionUC1" OnSecurityQuestionPopUp="SecurityQuestionUC_SecurityQuestionPopUp" OnSecurityResponseSaveClicked="SecurityQuestion_SecurityResponseSaveClicked" LinkText="Service account compliance check" />
                                    </asp:Panel>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-3 control-label"></label>
                        <div class="col-xs-8">
                            <asp:Button ID="AddRequestButton" CssClass="btn btn-default btn-sm" OnClick="AddRequestButton_Click" Text="Add" runat="server" Visible="true" />

                            <asp:Button ID="Button1" CssClass="btn btn-default btn-sm" OnClick="AddToCart" Text="Save Request" runat="server" Visible="false"/>
                        </div>
                    </div>

                </div>
            </div>
        </div>

        <table border="0" cellpadding="0" cellspacing="0" width="99%" style="margin-left: 5px">
            <tr id="trGrdid" runat="server">
                <td>
                    <telerik:RadGrid ID="RequestGrid" runat="server" AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                        GridLines="None" Width="50%" OnItemCommand="RequestGrid_ItemCommand" ShowFooter="false"
                        MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Request Available">
                        <GroupingSettings CaseSensitive="false" />
                        <MasterTableView AutoGenerateColumns="False" DataKeyNames=""  Width="100%">
                            <Columns>
                                <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Role" ItemStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:Label ID="EntitlementLabel" runat="server" Text="<%# Container.DataItem %>" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridButtonColumn ConfirmDialogType="RadWindow" ConfirmTitle="Delete" UniqueName="Delete" ConfirmText="Are you sure you want to delete?" CommandName="Delete" ButtonType="ImageButton" ItemStyle-CssClass="action-delete" ItemStyle-Width="1px" HeaderText="">
                                    <HeaderStyle Width="30px" />

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
<script type="text/javascript">
    function OnClientEntryGemsAccessRoleAddingHandler(sender, eventArgs) {
            if (sender.get_entries().get_count() > 0) {

                eventArgs.set_cancel(true);
            }
        }
</script>