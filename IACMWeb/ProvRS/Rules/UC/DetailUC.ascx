<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailUC.ascx.cs" Inherits="DI.Chakra.Web.ProvRS.Rules.UC.DetailUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Panel runat="server" ID="contentWrapper" CssClass="content-wrapper">
    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                    <div class="form-group">
                        <asp:HiddenField ID="hfRuleId" runat="server" Value="" />
                    </div>
                    <div class="form-group required">
                        <asp:Label ID="lblCode" AssociatedControlID="txtCode" runat="server" Text="Rule Code" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtCode" runat="server" MaxLength="100" type="text" placeholder="Enter Code" CssClass="form-control" onKeyPress="return AllowAlphaNumber(event);"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group required">
                        <asp:Label ID="lblDescription" AssociatedControlID="txtDescription" runat="server" Text=" Rule Description" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <asp:TextBox ID="txtDescription" runat="server" MaxLength="200" type="text" placeholder="Enter Description" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group required" id="Application" runat="server">
                        <label class="col-xs-4 control-label">Rule Type</label>
                        <div id="DivRuleTypeDropDown" class="col-xs-7" runat="server">
                            <asp:DropDownList ID="drpRuleTypes" runat="server" CssClass="form-control selectpicker" data-width="100%" AutoPostBack="True" OnSelectedIndexChanged="drpRuleTypes_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group" runat="server">
                        <label class="col-xs-4 control-label"></label>
                        <div class="col-xs-7">
                            <asp:Label ID="lblAll" runat="server" AssociatedControlID="chkAll">
                                <asp:CheckBox runat="server" ID="chkAll" TabIndex="2" />
                                <span class="lbl"><%= T("App.Rule.All") %></span>
                            </asp:Label>
                        </div>
                    </div>
                    <div class="form-group required">
                        <asp:Label ID="Label4" runat="server" Text="Value" CssClass="col-xs-4 control-label" AssociatedControlID="ValueAutoCompleteBox"></asp:Label>
                        <div class="col-xs-7">
                            <telerik:RadAutoCompleteBox ID="ValueAutoCompleteBox" runat="server" EmptyMessage="Enter * to view full list" TextSettings-SelectionMode="Single"
                                InputType="Token" CssClass="form-control">
                                <WebServiceSettings Path="../../Services/AutoCompleteDataService.svc" Method="GetRuleSetValues" />
                            </telerik:RadAutoCompleteBox>
                            <asp:Label ID="Label10" runat="server" ForeColor="Red" CssClass="errormsg"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-xs-4"></div>
                        <div class="col-xs-1 buttons text-right">
                            <asp:Button ID="btnAdd" ValidationGroup="Add" runat="server" Text="Add"
                                OnClick="btnAdd_Click" CssClass="btn btn-default btn-sm" />
                        </div>
                    </div>
                </div>

                <div class="form-group">
                    <div class="col-xs-12">
                        <cc1:IACMRadGridControl Width="95%" OnItemCommand="grdRulesets_ItemCommand" OnNeedDataSource="grdRulesets_NeedDataSource" ID="grdRulesets" runat="server">
                            <MasterTableView ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available" DataKeyNames="RuleId,TypeId,Code,Value"  Width="100%">
                                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                                <Columns>
                                    <telerik:GridBoundColumn DataField="RuleSetType.RulesetType" FilterControlAltText="RuleSetType.RulesetType" HeaderText="Rule Type" SortExpression="RuleSetType.RulesetType" UniqueName="RuleSetType.RulesetType" FilterControlWidth="80%" HeaderStyle-Width="20%">
                                        <ColumnValidationSettings>
                                            <ModelErrorMessage Text="" />
                                        </ColumnValidationSettings>
                                    </telerik:GridBoundColumn>

                                    <telerik:GridBoundColumn DataField="Value" FilterControlAltText="Value" HeaderText="Value" SortExpression="Value" UniqueName="Value" FilterControlWidth="80%" HeaderStyle-Width="20%">
                                        <ColumnValidationSettings>
                                            <ModelErrorMessage Text="" />
                                        </ColumnValidationSettings>
                                    </telerik:GridBoundColumn>

                                    <telerik:GridBoundColumn DataField="ValueDescription" FilterControlAltText="ValueDescription" HeaderText="Description" SortExpression="ValueDescription" UniqueName="ValueDescription" FilterControlWidth="80%" HeaderStyle-Width="20%">
                                        <ColumnValidationSettings>
                                            <ModelErrorMessage Text="" />
                                        </ColumnValidationSettings>
                                    </telerik:GridBoundColumn>
                                    <telerik:GridDateTimeColumn DataField="CreatedDate" HeaderText="Created Date" SortExpression="CreatedDate" UniqueName="CreatedDate" FilterControlWidth="90%" HeaderStyle-Width="13%">
                                    </telerik:GridDateTimeColumn>
                                    <telerik:GridBoundColumn DataField="CreatedBy" HeaderText="Created By" SortExpression="CreatedBy" UniqueName="CreatedBy" FilterControlWidth="70%" HeaderStyle-Width="13%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" Text="Delete" ConfirmText="Are you sure you want to delete ?" ConfirmDialogType="RadWindow" HeaderText="Delete"
                                        UniqueName="DeleteColumn" Exportable="false" HeaderStyle-Width="5%">
                                        <ItemStyle HorizontalAlign="Center" CssClass="action-delete" />
                                    </telerik:GridButtonColumn>
                                </Columns>
                            </MasterTableView>
                        </cc1:IACMRadGridControl>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Panel>
<div class="buttons text-right nav-rectangle">
    <asp:Button runat="server" class="btn btn-default btn-sm" ID="Button1" Text="Cancel" OnClick="Button1_Click"></asp:Button>
</div>
