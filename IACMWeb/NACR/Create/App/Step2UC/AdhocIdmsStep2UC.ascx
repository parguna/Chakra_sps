                                                                  <%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocIdmsStep2UC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.Step2UC.AdhocIdmsStep2UC" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>



<div class="form">
    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="col-xs-6"> 
                    <div id="pnlEndPointUserName" runat="server" class="form-group required">
                        <label class="col-xs-3 control-label">Endpoint User Name</label>
                        <div class="col-xs-9">
                            <asp:DropDownList ID="EndPointUserDropDownList" CssClass="form-control" AutoPostBack="true" runat="server" OnSelectedIndexChanged="EndPointUserDropDownList_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>
                

                 <div class="form-group required">
                        <label class="col-xs-3 control-label">Entitlement Type</label>
                        <div class="col-xs-9">
                            <asp:DropDownList ID="ddlEntitlementType" AutoPostBack="true" runat="server" CssClass="form-control"  OnSelectedIndexChanged="ddlEntitlementType_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>

                <div id="divRole" runat="server" visible="false" class="form-group">
                    <label class="col-xs-3 control-label">IDMS Role</label>
                    <div class="col-xs-9">
                        <telerik:RadAutoCompleteBox ID="RolesAutoCompleteBox" CssClass="form-control" runat="server" EmptyMessage="Enter * to view full list"  OnEntryRemoved="AutoCompleteBox_EntryRemoved" OnEntryAdded="AutoCompleteBox_EntryAdded" TextSettings-SelectionMode="Single" InputType="Token">
                             <TextSettings SelectionMode="Single" />
                            <WebServiceSettings Method="GetIdmsRoles" Path="../../../../Services/AutoCompleteDataService.svc" />
                        </telerik:RadAutoCompleteBox>
                    </div>
                    <div class="col-xs-3">&nbsp;
                    </div>    
                    <div class="col-xs-3">
                         <div style="padding-top:5px">
                        <asp:Button ID="btnRoleAdd" CssClass="btn btn-default btn-sm" OnClick="btnAdd_Click" Text="Add" runat="server" />
                             </div>
                    </div>
                </div>

                <div id="divGroup" runat="server" visible="false" class="form-group">
                    <label class="col-xs-3 control-label">IDMS Group</label>
                    <div class="col-xs-9">
                        <telerik:RadAutoCompleteBox ID="GroupAutoCompleteBox" CssClass="form-control" runat="server" EmptyMessage="Enter * to view full list" OnEntryRemoved="AutoCompleteBox_EntryRemoved" OnEntryAdded="AutoCompleteBox_EntryAdded" TextSettings-SelectionMode="Single" InputType="Token">
                            <TextSettings SelectionMode="Single" />
                            <WebServiceSettings Method="GetIdmsGroups" Path="../../../../Services/AutoCompleteDataService.svc" />
                        </telerik:RadAutoCompleteBox>&nbsp;
                    </div>
                     <div class="col-xs-3">&nbsp;
                    </div>    
                    <div class="col-xs-3">
                    <div style="padding-top:5px">
                        <asp:Button ID="btnGroupAdd" CssClass="btn btn-default btn-sm" OnClick="btnAdd_Click"  Text="Add" runat="server" style="padding-top:3px;" />
                    </div>
                    </div>
                </div>

                <div class="form-group">
                    <label class="col-xs-3 control-label">&nbsp;</label>
                    <div class="col-xs-9">
                        <%--<asp:Button ID="btnGroupAdd" CssClass="btn btn-default btn-sm" OnClick="btnAdd_Click"  Text="Add" runat="server" />
                     --%>   
                        <asp:Button ID="saveRequest" CssClass="btn btn-default btn-sm" OnClick="AddToCart" Text="Save Request" runat="server" Visible="false"/>
                    </div>
                </div> 

                <table border="0" cellpadding="0" cellspacing="0" width="Auto" style="margin-left: 5px">
                    <tr id="trGrdid" runat="server">
                        <td>
                            <telerik:RadGrid ID="RequestGrid" runat="server" AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                                GridLines="None" Width="1200px" OnItemCommand="RequestGrid_ItemCommand" ShowFooter="false"
                                MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Request Available">
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView AutoGenerateColumns="False" DataKeyNames=""  Width="100%">
                                    <Columns>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Entitlement Type" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:Label ID="EntitlementType" runat="server" Text='<%#Eval("EntitlementType") %>' />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="200px" HeaderText="Entitlement" ItemStyle-HorizontalAlign="Left">
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
</div>
