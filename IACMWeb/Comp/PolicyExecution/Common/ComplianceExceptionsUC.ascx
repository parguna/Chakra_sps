<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ComplianceExceptionsUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.PolicyExecution.Common.ComplianceExceptionsUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/ExceptionSearchUC.ascx" TagPrefix="uc1" TagName="ExceptionSearchUC" %>

<div class="content-wrapper" runat="server" id="contentWrapper">
    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                   <div class="form-group">
                        <asp:Label ID="Label5" AssociatedControlID="ExceptionSearch" runat="server" Text="Select Exceptions" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                            <uc1:ExceptionSearchUC runat="server" ID="ExceptionSearch" />
                        </div>
                       <div class="col-xs-1">
                            <asp:Button ID="btnException" ValidationGroup="Add" runat="server" Text="Add" 
                                OnClientClick="return validateAutoCompleteBox(this,'ExceptionAutoCompleteBox','Please select at least one exception')" OnClick="btnException_Click"  CssClass="btn btn-default btn-sm" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-xs-4">
                        </div>
                        <div class="col-xs-7">
                            <telerik:RadGrid ID="grdExceptions" runat="server" AllowPaging="true" AllowSorting="false" Width="100%"
                                OnItemCommand ="grdExceptions_ItemCommand">
                                <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" AlwaysVisible="false" />
                                <ClientSettings>
                                    <Scrolling UseStaticHeaders="true" />
                                    <Selecting AllowRowSelect="false" />
                                </ClientSettings>
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView DataKeyNames="Key" AutoGenerateColumns="False" CommandItemDisplay="None" NoMasterRecordsText="No Exceptions Available" EnableHeaderContextMenu="false">
                                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="Value" FilterControlAltText="Value" SortExpression="Value" HeaderText="Exception Name" HeaderStyle-Width="70%" UniqueName="ExceptionName">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" Text="Delete" ConfirmText="Are you sure you want to delete ?" HeaderText="Delete"
                                            ConfirmDialogType="RadWindow" UniqueName="DeleteColumn" Exportable="false" HeaderStyle-Width="20%">
                                            <ItemStyle HorizontalAlign="Center" CssClass="action-delete" />
                                        </telerik:GridButtonColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                        </div>
                    </div>
                </div>
                </div> 
            </div>
        </div>
    </div>