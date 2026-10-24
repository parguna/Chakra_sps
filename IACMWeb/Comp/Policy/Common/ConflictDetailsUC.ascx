<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConflictDetailsUC.ascx.cs" Inherits="IACMWeb.Comp.Policy.Common.ConflictDetailsUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/SODConflictFunctionSearchUC.ascx" TagPrefix="uc1" TagName="SODConflictFunctionSearchUC" %>


<div class="content-wrapper" runat="server" id="contentWrapper">
    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                    <div class="form-group">
                        <label class="col-xs-4 control-label">Select Conflicts</label>
                        <div class="col-xs-7">
                            <uc1:SODConflictFunctionSearchUC runat="server" ID="SODConflictFunctionSearch" />
                        </div>
                        <div class="col-xs-1">
                            <asp:Button ID="btnConflict" ValidationGroup="Add" runat="server" Text="Add" 
                                OnClientClick="return validateAutoCompleteBox(this,'ConflictAutoCompleteBox','Please select at least one conflict')" OnClick="btnConflict_Click"  CssClass="btn btn-default btn-sm" />
                        </div>
                    </div>
                   
                    </div> 

                 <div class="form-group">
                        <div class="col-xs-4">
                        </div>
                        <div class="col-xs-7">
                            <telerik:RadGrid ID="grdConflicts" runat="server" AllowPaging="true" AllowSorting="false" Width="100%"
                                OnItemCommand ="grdConflicts_ItemCommand">
                                <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" AlwaysVisible="false" />
                                <ClientSettings>
                                    <Scrolling UseStaticHeaders="true" />
                                    <Selecting AllowRowSelect="false" />
                                </ClientSettings>
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView DataKeyNames="Key" AutoGenerateColumns="False" CommandItemDisplay="None" NoMasterRecordsText="No Functions Available" EnableHeaderContextMenu="false">
                                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="Value" FilterControlAltText="Value" SortExpression="Value" HeaderText="Function Name" HeaderStyle-Width="70%" UniqueName="EndPointSystemName">
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


