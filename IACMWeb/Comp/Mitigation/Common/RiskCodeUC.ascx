<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RiskCodeUC.ascx.cs" Inherits="DI.Chakra.Web.Comp.Mitigation.Common.RiskCodeUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/RiskCodeUC.ascx" TagName="CodeRisk" TagPrefix="CdeRsk" %>
<div class="content-wrapper" runat="server" id="contentWrapper">
    <div class="form clearfix">
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-6">
                    <div class="form-group">
                        <asp:Label ID="RiskCodes" AssociatedControlID="CodeRisks" runat="server" Text="Risk Code" CssClass="col-xs-4 control-label"></asp:Label>
                        <div class="col-xs-7">
                         <CdeRsk:CodeRisk ID="CodeRisks" runat="server"  />
                        </div>
                         <div class="col-xs-1 buttons text-right">
                     <asp:Button ID="btnMitigationFunction" ValidationGroup="Add" runat="server" Text="Add" 
                                    OnClientClick="return validateAutoCompleteBox(this,'RiskCodeAutobox','Please select at least one Code')" OnClick="btnMitigationFunction_Click"  CssClass="btn btn-default btn-sm" />
                            </div>
                    </div>
                </div>

                <div class="form-group">
                     <div class="col-xs-4">
                        </div>
                    <div class="col-xs-7">
                        <telerik:RadGrid ID="grdMititaionRisk" runat="server" AllowPaging="true" AllowSorting="true"
                            Width="100%" OnItemCommand="grdMititaionRisk_ItemCommand">
                            <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" AlwaysVisible="false" />
                              <ClientSettings>
                                    <Scrolling UseStaticHeaders="true" />
                                    <Selecting AllowRowSelect="false" />
                                </ClientSettings>
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView DataKeyNames="Id" AutoGenerateColumns="False" CommandItemDisplay="None" NoMasterRecordsText="No Risk Codes Available" EnableHeaderContextMenu="false">
                                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                                <Columns>
                                    <telerik:GridBoundColumn DataField="RiskCode" FilterControlAltText="Value" SortExpression="RiskCode" HeaderText="Risk Code" HeaderStyle-Width="70%" UniqueName="RiskCode">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="RiskDescription" FilterControlAltText="Value" SortExpression="RiskDescription" HeaderText="Risk Description" HeaderStyle-Width="70%" UniqueName="RiskDescription"></telerik:GridBoundColumn>
                                    <telerik:GridButtonColumn  ButtonType="ImageButton" CommandName="Delete" Text="Delete" ConfirmText="Are you sure you want to delete ?" HeaderText="Delete"
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