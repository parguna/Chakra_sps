<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccessRequestEntitlementDetails.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.AccessRequestEntitlementDetails" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<asp:LinkButton ID="lnkEntitlementDetails" OnClick="lnkEntitlementDetails_Click" runat="server" CssClass="text-indent">Entitlement Info</asp:LinkButton>

<telerik:RadWindow runat="server" ID="EntitlementInfoWindow" Modal="true" Width="650px" Height="100%" Title="Approver Detail"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title">
                        <asp:Label runat="server" ID="lblEntitlementInfoHeader">Entitlement Details</asp:Label></h3>
                </div>
                <div class="modal-body">

                    <div class="row-fluid">
                        <div class="" id="noEntitlementIfo" runat="server">
                            <label runat="server">
                                Request is not processed, Entitlement details will be shown after request excuted.
                            </label>
                        </div>
                        <telerik:RadGrid ID="EntitlementReportGrid" runat="server" AllowPaging="True" AllowSorting="True" EnableViewState="false"
                            FilterMenu-CssClass="RadMenu_IACMFilterSkin" OnNeedDataSource="EntitlementReportGrid_NeedDataSource">
                            <MasterTableView TableLayout="fixed"  Width="100%"
                                EnableNoRecordsTemplate="true" ShowHeadersWhenNoRecords="true" NoMasterRecordsText="No Records Available"
                                AutoGenerateColumns="false">
                                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>
                                <Columns>
                                    <telerik:GridBoundColumn DataField="AccountName" FilterControlAltText="AccountName" SortExpression="AccountName" HeaderText="AccountName" UniqueName="AccountName" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="Entitlement" FilterControlAltText="Entitlement" SortExpression="Entitlement" HeaderText="Entitlement" UniqueName="Entitlement" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="EntitlementType" FilterControlAltText="EntitlementType" SortExpression="EntitlementType" HeaderText="Type" UniqueName="EntitlementType" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="IsDeleted" FilterControlAltText="IsDeleted" SortExpression="IsDeleted" HeaderText="Action" UniqueName="IsDeleted" HeaderStyle-Width="20%" FilterControlWidth="70%">
                                    </telerik:GridBoundColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnOk" class="btn btn-primary bordered-button" Text="OK" OnClick="btnOk_Click" />
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="lnkEntitlementDetails" />
            </Triggers>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>

