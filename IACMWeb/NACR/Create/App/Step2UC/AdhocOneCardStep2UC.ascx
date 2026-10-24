<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocOneCardStep2UC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.Step2UC.AdhocOneCardStep2UC" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<style>
    .fullwidth {
        width: 100% !important;
    }
</style>

<div class="form">
    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="col-xs-6">

                <div id="trEndPointUser" runat="server" class="form-group required">
                    <label class="col-xs-3 control-label">EndPoint User Name</label>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlEndpointUser" OnSelectedIndexChanged="ddlEndpointUser_SelectedIndexChanged" CssClass="form-control" AutoPostBack="true" runat="server">
                        </asp:DropDownList>
                    </div>
                </div>



                <div id="TRBuilding" runat="server" class="form-group required">
                    <label class="col-xs-3 control-label">Building</label>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlBuilding" runat="server" OnSelectedIndexChanged="ddlBuilding_SelectedIndexChanged" CssClass="form-control" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>
                </div>

                <div id="TRSection" runat="server" class="form-group required">
                    <label class="col-xs-3 control-label">Section</label>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlSection" runat="server" OnSelectedIndexChanged="ddlSection_SelectedIndexChanged" CssClass="form-control" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>
                </div>

            </div>
        </div>
    </div>

    <div class="form-horizontal left clearfix">

        <div class="row-fluid">
            <div class="col-xs-6">

                <div id="TRRoles" runat="server" class="form-group required">
                    <label class="col-xs-3 control-label">Technical Role</label>
                    <div class="col-xs-8">
                        <telerik:RadComboBox ID="TechnicalRoleCombobox" runat="server" CheckBoxes="true"  EnableCheckAllItemsCheckBox="true" Style="width: 100% !important;"
                            OnSelectedIndexChanged="TechnicalRoleCombobox_SelectedIndexChanged" AutoPostBack="true">
                        </telerik:RadComboBox>
                    </div>
                </div>

                <div class="form-group">
                    <label class="col-xs-3 control-label"></label>
                    <div class="col-xs-8">
                        <asp:Button CssClass="btn btn-default btn-sm" ID="btnAdd" runat="server" Text="Add"   OnClick="btnAdd_Click" />
                        <asp:Button ID="saveRequest" CssClass="btn btn-default btn-sm" OnClick="AddToCart" Text="Save Request" runat="server" Visible="false"/>
                    </div>
                </div>

            </div>

        </div>
    </div>


    <div class="form-horizontal left clearfix">

        <div class="row-fluid">
             <div class="col-xs-12">
            <telerik:RadGrid ID="SMSObjGrid" runat="server" Visible="false" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                GridLines="None" Width="100%" ShowFooter="false" OnItemCommand="SMSObjGrid_ItemCommand"
                MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Records Available" TableLayout="Fixed" CssClass="RadGridtextWrap" OnDataBound="SMSObjGrid_DataBound">
                <GroupingSettings CaseSensitive="false" />
                <ClientSettings>
                    <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true" ScrollHeight="120px" FrozenColumnsCount="2"></Scrolling>
                </ClientSettings>
                <MasterTableView AutoGenerateColumns="False" DataKeyNames="TechnicalRole" TableLayout="Fixed"  Width="100%">
                    <Columns>
                        <telerik:GridBoundColumn DataField="Building" FilterControlAltText="Building" SortExpression="Building" HeaderText="Building" UniqueName="Building" HeaderStyle-Width="200px">
                        </telerik:GridBoundColumn>

                        <telerik:GridBoundColumn DataField="Section" FilterControlAltText="Section" HeaderText="Section" SortExpression="Section" UniqueName="Section" HeaderStyle-Width="150px">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="TechnicalRole" FilterControlAltText="TechnicalRole" SortExpression="TechnicalRole" HeaderText="Technical Role" UniqueName="TechnicalRole">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Description" FilterControlAltText="Description" SortExpression="Description" HeaderText="Description" UniqueName="Description">
                        </telerik:GridBoundColumn>
                        <telerik:GridButtonColumn ConfirmDialogType="RadWindow" ConfirmTitle="Delete" UniqueName="Delete" ConfirmText="Are you sure you want to delete?" CommandName="Delete" ButtonType="ImageButton" ItemStyle-CssClass="action-delete" ItemStyle-Width="1px" HeaderText="" HeaderStyle-Width="50px">
                            <HeaderStyle Width="30px" />

                        </telerik:GridButtonColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
             </div>
        </div>
    </div>
</div>
