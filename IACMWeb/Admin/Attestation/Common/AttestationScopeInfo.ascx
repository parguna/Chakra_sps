<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttestationScopeInfo.ascx.cs" Inherits="DI.Chakra.Web.Attestation.Common.AttestationScopeInfo" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-wrapper">
    <div class="form">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-12">
                    <div class="form-group">
                        <asp:Label ID="lblScope" AssociatedControlID="ddlScopeValue" runat="server" CssClass="col-xs-2 control-label"><%= T("App.Attestation.ScopeType") %><span style="color:red">*</span></asp:Label>
                        <div class="col-xs-3">
                            <asp:DropDownList ID="ddlScopeValue" runat="server" CssClass="form-control selectpicker"
                                OnSelectedIndexChanged="ScopeValue_OnSelectedIndexChanged" data-width="100%"
                                DataTextField="Scope" DataValueField="Id" AutoPostBack="True" >
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblForms" runat="server" CssClass="col-xs-2 control-label" AssociatedControlID="ScopeValueAutoCompleteBox"><%= T("App.Attestation.ScopeValue") %></asp:Label>
                        <div class="col-xs-3">
                            <telerik:RadAutoCompleteBox ID="ScopeValueAutoCompleteBox" runat="server" AutoPostBack="true" ToolTip="Enter * to view full list" TextSettings-SelectionMode="Single"
                                InputType="Token" CssClass="form-control" required="required">
                                <WebServiceSettings Path="../../Services/AutoCompleteDataService.svc" Method="GetOrganisationScopeUsers" />
                            </telerik:RadAutoCompleteBox>
                        </div>
                        <div class="col-xs-1">
                            <asp:Button ID="btnAddScopeType" runat="server" Text="Add" CssClass="btn btn-default btn-sm" formnovalidate="true" OnClick="btnAddScopeType_Click" OnClientClick="return validateAutoCompleteBox(this,'ScopeValueAutoCompleteBox','Please enter atleast one scope value')" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-xs-2">
                        </div>
                        <div class="col-xs-4">
                            <telerik:RadGrid ID="ScopeValuesGrid" runat="server" AllowPaging="false" AllowSorting="false"
                                CellSpacing="0" GridLines="None" Width="420px" OnItemCommand="ScopeValuesGrid_ItemCommand">
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView AutoGenerateColumns="False" DataKeyNames="Id" ShowHeadersWhenNoRecords="false" NoMasterRecordsText="No Records Available">
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="AttestationScopeName" UniqueName="AttestationScopeName" FilterControlAltText="AttestationScopeName" SortExpression="AttestationScopeName" HeaderText="Scope Type" HeaderStyle-Width="200px" >
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="AttestationScopeValueDesc" UniqueName="AttestationScopeValueDesc" FilterControlAltText="AttestationScopeValueDesc" SortExpression="AttestationScopeValueDesc" HeaderText="Scope Value" HeaderStyle-Width="370px">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="AttestationScopeValue" UniqueName="AttestationScopeValue" FilterControlAltText="AttestationScopeValue" SortExpression="AttestationScopeValue" HeaderText="AttestationScopeValue" Display="false">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" Text="Delete" HeaderText="Delete" ConfirmText="Are you sure you want to delete the scope value?" ConfirmDialogType="RadWindow"
                                            UniqueName="DeleteColumn" Exportable="false" HeaderStyle-Width="10%">
                                            <ItemStyle HorizontalAlign="Center" CssClass="action-delete" />
                                        </telerik:GridButtonColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                        </div>
                    </div>   
                    <div class="form-group" runat="server">
                        <asp:Label ID="lblSplitCampaign" AssociatedControlID="ddlSplitValue" runat="server" Text="" CssClass="col-xs-2 control-label"><%= T("App.Attestation.SplitCampaign") %></asp:Label>
                        <div class="col-xs-3">
                            <asp:DropDownList ID="ddlSplitValue" runat="server" CssClass="form-control selectpicker" data-width="100%" DataTextField="Scope" DataValueField="Id" OnSelectedIndexChanged="ddlSplitValue_SelectedIndexChanged" AutoPostBack="True">
                            </asp:DropDownList>
                        </div>
                    </div>                 
                </div>
            </div>
        </div>
    </div>
</div>
