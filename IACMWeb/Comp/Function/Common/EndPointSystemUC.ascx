<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EndPointSystemUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.Function.Common.EndPointSystemUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/EndpointUC.ascx" TagPrefix="uc1" TagName="EndpointUC" %>
<%@ Register Src="~/UsrControl/Common/GenericAutoCompleteBox.ascx" TagPrefix="uc1" TagName="GenericAutoCompleteBox" %>
<%@ Register Src="~/UsrControl/Common/SAPObjectFieldSearch.ascx" TagPrefix="uc1" TagName="SAPObjectFieldSearch" %>

<div class="content-wrapper" runat="server" id="contentWrapper">
    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                    <uc1:EndpointUC runat="server" ID="EndpointUC" OnApplicationChanged="EndpointUC_ApplicationChanged" OnEndpointChanged="EndpointUC_EndpointChanged" />
                  
                    <div id="AutoCompleteBoxRow" runat="server" class="form-group">
                        <asp:Label ID="lblFunctionTypeText" AssociatedControlID="GenericAutoCompleteBox" runat="server" required="required" CssClass="col-xs-4 control-label"></asp:Label>

                        <div class="col-xs-7">
                            <uc1:GenericAutoCompleteBox runat="server" ID="GenericAutoCompleteBox" />
                        </div>

                    </div>
                    <div id="objectRow" runat="server" class="form-group">
                        <asp:CheckBox ID="chkObject" runat="server" OnCheckedChanged="chkObject_CheckedChanged" Text="Object" AutoPostBack="true" />
                    </div>
                    <div runat="server" id="SapObjectFieldrow">
                        <uc1:SAPObjectFieldSearch runat="server" ID="SAPObjectFieldSearch" />
                        <div class="form-group">
                            <asp:Label ID="lblvalue" AssociatedControlID="txtFrom" runat="server" Text="From Value" CssClass="col-xs-4 control-label"></asp:Label>
                            <div class="col-xs-7">
                                <asp:TextBox ID="txtFrom" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
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
                        <telerik:RadGrid ID="grdPolicyTemplate" runat="server" AllowPaging="true" AllowSorting="false" Width="82.5%"
                            OnItemCommand="grdPolicyTemplate_ItemCommand" OnPreRender="grdPolicyTemplate_PreRender">
                            <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" AlwaysVisible="true" />
                            <ClientSettings>
                                <Scrolling UseStaticHeaders="true" />
                                <Selecting AllowRowSelect="false" />
                            </ClientSettings>
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" NoMasterRecordsText="No Policy Templates Available" EnableHeaderContextMenu="false">
                                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                                <Columns>
                                    <telerik:GridBoundColumn DataField="EndpointType.DisplayName" FilterControlAltText="EndpointType.DisplayName" HeaderText="Endpoint System Type" SortExpression="EndpointType.DisplayName" UniqueName="EndpointSystemTypeName" HeaderStyle-Width="11.5%">
                                        <ColumnValidationSettings>
                                            <ModelErrorMessage Text="" />
                                        </ColumnValidationSettings>
                                    </telerik:GridBoundColumn>

                                    <telerik:GridBoundColumn DataField="endpoint.DisplayName" FilterControlAltText="endpoint.DisplayName" HeaderText="Endpoint System" SortExpression="endpoint.DisplayName" UniqueName="DisplayName" HeaderStyle-Width="16.5%">
                                        <ColumnValidationSettings>
                                            <ModelErrorMessage Text="" />
                                        </ColumnValidationSettings>
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="SodFunctionType" FilterControlAltText="SodFunctionType" HeaderText="Function Type" SortExpression="SodFunctionType" UniqueName="SodFunctionType" HeaderStyle-Width="9%">
                                        <ColumnValidationSettings>
                                            <ModelErrorMessage Text="" />
                                        </ColumnValidationSettings>
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="SodRoleDesc" FilterControlAltText="SodRoleDesc" HeaderText="Value" SortExpression="SodRoleDesc" UniqueName="SodRoleDesc" HeaderStyle-Width="23.5%">
                                        <ColumnValidationSettings>
                                            <ModelErrorMessage Text="" />
                                        </ColumnValidationSettings>
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="ObjectCode" FilterControlAltText="ObjectCode" HeaderText="Object Code" SortExpression="ObjectCode" UniqueName="ObjectCode" HeaderStyle-Width="9%">
                                        <ColumnValidationSettings>
                                            <ModelErrorMessage Text="" />
                                        </ColumnValidationSettings>
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="FieldCode" FilterControlAltText="FieldCode" HeaderText="Field Code" SortExpression="FieldCode" UniqueName="FieldCode" HeaderStyle-Width="10%">
                                        <ColumnValidationSettings>
                                            <ModelErrorMessage Text="" />
                                        </ColumnValidationSettings>
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="FromValue" FilterControlAltText="FromValue" HeaderText="From Value" SortExpression="FromValue" UniqueName="FromValue" HeaderStyle-Width="13.5%">
                                        <ColumnValidationSettings>
                                            <ModelErrorMessage Text="" />
                                        </ColumnValidationSettings>
                                    </telerik:GridBoundColumn>
                                    <telerik:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" Text="Delete" ConfirmText="Are you sure you want to delete ?" ConfirmDialogType="RadWindow" HeaderText="Delete"
                                        UniqueName="DeleteColumn" Exportable="false" HeaderStyle-Width="5%">
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
