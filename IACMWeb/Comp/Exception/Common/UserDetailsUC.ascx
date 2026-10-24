<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserDetailsUC.ascx.cs"  Inherits="DI.Chakra.Web.Comp.Exception.Common.UserDetailsUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagName="UserSearch" TagPrefix="Ucl" %>
<%@ Register Src="~/Comp/Exception/UC/FunctionUC.ascx" TagName="FunSearch" TagPrefix="Fun" %>



<div class="content-wrapper" runat="server" id="contentWrapper">
    <div class="form clearfix">
        <div class="form-horizontal left">
        <div class="row-fluid">
            
                
            <div class="col-xs-6">
                <div class="form-group">
                    <asp:Label ID="Label3" AssociatedControlID="usrSearch" runat="server" Text="User" required="required" CssClass="col-xs-4 control-label"></asp:Label>
                    <div class="col-xs-7">
                    <Ucl:UserSearch ID="usrSearch" runat="server"/>
                    </div> 
                </div>
                <Fun:FunSearch ID="FunSearch"  runat="server" />
                <Fun:FunSearch ID="FunSearch2" runat="server" />
                <div class="form-group">
                    <div class="col-xs-4"></div>
                 <div class="col-xs-1 buttons text-right">
                     <asp:Button ID="btnExceptionFunction" ValidationGroup="Add" runat="server" Text="Add" 
                                 OnClick="btnExceptionFunction_Click"  CssClass="btn btn-default btn-sm" />
                 </div>
                </div>
            </div>
                
            <div class="form-group">
                <div class="col-xs-4">
                        </div>
                <div class="col-xs-7">
                    <telerik:RadGrid ID="grdExceptionItem" runat="server"  AllowPaging="true" AllowSorting="false" Width="100%"
                         OnItemCommand="grdExceptionItem_ItemCommand"
                        > 
                        <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" AlwaysVisible="false" />
                          <ClientSettings>
                                    <Scrolling UseStaticHeaders="true" />
                                    <Selecting AllowRowSelect="false" />
                                </ClientSettings>
                        <GroupingSettings CaseSensitive="false" />
                         <MasterTableView DataKeyNames="Id" AutoGenerateColumns="False" CommandItemDisplay="None" NoMasterRecordsText="No Functions Available" EnableHeaderContextMenu="false">
                             <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                             <Columns>
                                        <telerik:GridBoundColumn DataField="user.DisplayName" FilterControlAltText="Value" SortExpression="user.DisplayName" HeaderText="User Name" HeaderStyle-Width="70%" UniqueName="user.DisplayName">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="function1.SodFunctionCode" FilterControlAltText="Value" SortExpression="function1.SodFunctionCode" HeaderText="Function 1" HeaderStyle-Width="70%" UniqueName="function1.SodFunctionCode">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="functionitem1.SodRoleDesc" FilterControlAltText="Value" SortExpression="functionitem1.SodRoleDesc" HeaderText="Function Value 1" HeaderStyle-Width="70%" UniqueName="functionitem1.SodRoleDesc">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="function2.SodFunctionCode" FilterControlAltText="Value" SortExpression="function2.SodFunctionCode" HeaderText="Function 2" HeaderStyle-Width="70%" UniqueName="function2.SodFunctionCode">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="functionitem2.SodRoleDesc" FilterControlAltText="Value" SortExpression="functionitem2.SodRoleDesc" HeaderText="Function Value 2" HeaderStyle-Width="70%" UniqueName="functionitem2.SodRoleDesc">
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