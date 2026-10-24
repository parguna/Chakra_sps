<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocMoreDetails.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.Common.AdhocMoreDetails" %>


<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>


<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>

        <div id="hiddenDiv" style="display: none; visibility: hidden;">
            <asp:Button ID="hiddenButton" runat="server" />
        </div>

        <asp:Panel ID="CommentsPanel" runat="server">
            <div class="row-fluid">
                <div id="trUsername" visible="false" runat="server" class="form-group">
                    <div class="col-xs-5">
                        <b style="font-size: 18px;">Account Name:</b>
                    </div>
                    <div class="col-xs-5">
                        <asp:Label ID="lblUserName" Text="" runat="server"></asp:Label>
                    </div>
                </div>
            </div>




            <asp:Panel ID="PanelofSSIDS" runat="server" Visible="false">

                <div class="row-fluid">
                    <div class="col-xs-12">
                        <h3>Entitlement Details</h3>
                    </div>
                </div>
                <div class="row-fluid">
                    <div style="height: auto !important;" class="col-xs-12">

                        <telerik:RadGrid ID="RadGridSSIDS" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                            AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true"
                            Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No requests added."
                            MasterTableView-EnableNoRecordsTemplate="true" Visible="true">
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView DataKeyNames="Entitlement" AutoGenerateColumns="False" Width="100%">
                                <Columns>
                                    <telerik:GridBoundColumn DataField="EntitlementType" HeaderText="Entitlement Type" ItemStyle-HorizontalAlign="Left">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="Entitlement" UniqueName="Entitlement" HeaderText="Entitlement" ItemStyle-HorizontalAlign="Left">
                                    </telerik:GridBoundColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>

                    </div>
                </div>
                <%--<div>
                                <div id="divET" visible="false" runat="server" class="form-group">
                                    <div style="background-color: rgb(95, 125, 149); color: white;" class="form-group">
                                        Entitlement Type:
                                        <asp:Label ID="lblssidsET" Text="" runat="server"></asp:Label>

                                    </div>
                                </div>

                                <div id="divHP" visible="false" runat="server" class="form-group">
                                    <div style="background-color: rgb(95, 125, 149); color: white;" class="form-group">
                                        Hand Phone Number:
                                        <asp:Label ID="lblssidsHP" Text="" runat="server"></asp:Label>

                                    </div>
                                </div>

                                 <div id="divRole" visible="false" runat="server" class="form-group">
                                    <div style="background-color: rgb(95, 125, 149); color: white;" class="form-group">
                                        Role:
                                        <asp:Label ID="lblssidsRL" Text="" runat="server"></asp:Label>

                                    </div>
                                </div> 
                            </div>--%>
            </asp:Panel>

            <asp:Panel ID="PanelofAD" runat="server" Visible="false">
                <asp:Panel ID="GroupAccountDetailPanel" runat="server">
                    <div>
                        <div>
                            <h3>Entitlement Details</h3>
                        </div>

                        <div style="height: auto !important;">
                            <telerik:RadGrid ID="RadGridPopup" AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" Visible="false"
                                runat="server" OnItemDataBound="RadGridPopup_ItemDataBound" AllowFilteringByColumn="false" AllowPaging="True" AllowSorting="True" CellSpacing="0" Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No Details Found.">
                                <PagerStyle Mode="NextPrevAndNumeric" Width="99%" />
                                <ExportSettings
                                    HideStructureColumns="true"
                                    IgnorePaging="true" ExportOnlyData="true"
                                    Excel-Format="ExcelML"
                                    OpenInNewWindow="true">
                                    <Pdf PageHeight="210mm" AllowPrinting="true" PageWidth="347mm" DefaultFontFamily="Arial Unicode MS" PageTopMargin="45mm"
                                        BorderStyle="Medium" BorderColor="#666666" BorderType="AllBorders">
                                    </Pdf>
                                </ExportSettings>
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="true" DataKeyNames="Entitlement" Width="100%">

                                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="true" ShowExportToPdfButton="false" ShowExportToExcelButton="false" ShowExportToCsvButton="false"></CommandItemSettings>
                                    <Columns>
                                        <telerik:GridTemplateColumn UniqueName="GrouporRequestorText" HeaderStyle-Width="50px" HeaderText="Groups" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:Label ID="EntitlementLabelAD" Text='<%# string.IsNullOrEmpty((string)Eval("Entitlement")) ? "No Details Found." : Eval("Entitlement")%>' runat="server"></asp:Label>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>

                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>

                        </div>
                    </div>
                </asp:Panel>
                <asp:Panel ID="SharedAccountDetailPanel" runat="server" Visible="false">

                    <div class="row-fluid">
                        <div class="col-xs-12">
                            <h3>Account Details</h3>
                        </div>
                    </div>
                    <div class="row-fluid">
                        <div style="height: auto !important;" class="col-xs-12">
                            <telerik:RadGrid ID="SharedDetailsGridAD" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                                AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true"
                                Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No requests added."
                                MasterTableView-EnableNoRecordsTemplate="true" Visible="false">
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView AutoGenerateColumns="False" Width="100%">
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="AccountName" HeaderText="Account Name" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="DisplayName" UniqueName="DisplayName" HeaderText="Display Name" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="EmailAddress" UniqueName="EmailAddress" HeaderText="Email Address" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Description" UniqueName="Description" HeaderText="Description" Display="false" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>

                        </div>
                    </div>
                </asp:Panel>
            </asp:Panel>

            <asp:Panel ID="pnlINH" runat="server" Visible="false">
                <asp:Panel ID="Panel2" runat="server">
                    <div>
                        <div>
                            <h3>Entitlement Details</h3>
                        </div>

                        <div style="height: auto !important;">
                            <telerik:RadGrid ID="radGridINH" AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" Visible="true"
                                runat="server" OnItemDataBound="radGridINH_ItemDataBound" AllowFilteringByColumn="false" AllowPaging="True" AllowSorting="True" CellSpacing="0" Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No Details Found.">
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="true" Width="100%">
                                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="true" ShowExportToPdfButton="false" ShowExportToExcelButton="false" ShowExportToCsvButton="false"></CommandItemSettings>
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="Permission" HeaderText="Permission" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="PermissionDescription" HeaderText="Permission Description" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Function" HeaderText="Function" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="FunctionDescription" HeaderText="Function Description" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>

                        </div>
                    </div>
                </asp:Panel>
                <asp:Panel ID="Panel3" runat="server" Visible="false">
                    <div class="row-fluid">
                        <div class="col-xs-12">
                            <h3>Account Details</h3>
                        </div>
                    </div>
                    <div class="row-fluid">
                        <div style="height: auto !important;" class="row-fluid">
                            <telerik:RadGrid ID="RadGrid2" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                                AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true"
                                Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No requests added."
                                MasterTableView-EnableNoRecordsTemplate="true" Visible="false">
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView AutoGenerateColumns="False" Width="100%">
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="AccountName" HeaderText="Account Name" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="DisplayName" UniqueName="DisplayName" HeaderText="Display Name" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="EmailAddress" UniqueName="EmailAddress" HeaderText="Email Address" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>

                        </div>
                    </div>
                </asp:Panel>
            </asp:Panel>

            <asp:Panel ID="MoreDetailPanel" runat="server" Visible="false">
                <asp:Panel ID="GroupHRMSPanel" runat="server">
                    <div>

                        <div>
                            <h3>Entitlement Details</h3>
                        </div>
                        <div style="height: auto !important;">
                            <div>

                                <div id="BussinessRoleRow" runat="server" class="form-group" visible="false">

                                    <telerik:RadGrid ID="radGrdbusinessrole" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0" GridLines="None" Width="100%" Height="100%">
                                        <GroupingSettings CaseSensitive="false" />
                                        <MasterTableView AutoGenerateColumns="False" Width="100%">
                                            <Columns>
                                                <telerik:GridTemplateColumn HeaderText="Business Role">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label4" runat="server" Text='<%#Eval("Entitlement") %>' />
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                            </Columns>
                                        </MasterTableView>
                                    </telerik:RadGrid>


                                </div>


                                <div id="trHRMSFLINS" visible="false" runat="server">
                                    <div>
                                        <telerik:RadGrid ID="HRMSGRID" OnItemDataBound="HRMSGRID_ItemDataBound" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0" GridLines="None" Width="100%" Height="100%">
                                            <GroupingSettings CaseSensitive="false" />
                                            <MasterTableView AutoGenerateColumns="False" DataKeyNames="CostCenter,PurchaseOrg,PurchaseGroup,Plant,StorageLocation,MRPController" Width="100%">
                                                <Columns>
                                                    <telerik:GridTemplateColumn HeaderText="Entitlement">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label4" runat="server" Text='<%#Eval("Entitlement") %>' />
                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridTemplateColumn HeaderText="Company">
                                                        <ItemTemplate>
                                                            <asp:Label ID="CompanyLabel" runat="server" Text='<%# Eval("Company") %>' />
                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridTemplateColumn UniqueName="IsCompanyRetain" HeaderText="IsCompany Retain">
                                                        <ItemTemplate>
                                                            <asp:Label ID="IsCompanyRetainLabel" runat="server" Text='<%#Convert.ToBoolean(Eval("RetainCompanyRoles")) ? "Yes" : "No" %>' />
                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridTemplateColumn HeaderText="Variance">
                                                        <ItemTemplate>
                                                            <asp:Label ID="VarianceLabel" runat="server" />
                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>
                                                </Columns>
                                            </MasterTableView>
                                        </telerik:RadGrid>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>
                </asp:Panel>
                <asp:Panel ID="SharedHRMSPanel" runat="server" Visible="false">

                    <div>
                        <div>
                            <div style="width: 95%" class="model-header">
                                <h3 class="model-title">Account Details </h3>
                            </div>
                        </div>
                        <div class="model-body">

                            <telerik:RadGrid ID="SharedHRMSGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                                AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true"
                                Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No requests added."
                                MasterTableView-EnableNoRecordsTemplate="true" Visible="false">
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView AutoGenerateColumns="False" Width="100%">
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="SAPUserType" HeaderText="SAP UserType" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="AccountName" HeaderText="Account Name" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Description" UniqueName="Description" HeaderText="Description" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>

                        </div>
                    </div>

                </asp:Panel>
            </asp:Panel>

            <asp:Panel ID="SRMPanel" runat="server">
                <div class="row-fluid">
                    <div class="col-xs-12">
                        <h3>Access Details</h3>
                    </div>
                </div>
                <div class="row-fluid">
                    <div style="height: auto !important;" class="col-xs-12">
                        <telerik:RadGrid ID="SRMGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                            AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true"
                            Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No requests added."
                            MasterTableView-EnableNoRecordsTemplate="true">
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="False" Width="100%">
                                <Columns>
                                    <telerik:GridBoundColumn DataField="Entitlement" FilterControlAltText="Entitlement" SortExpression="Entitlement" HeaderText="Entitlement" UniqueName="Entitlement">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="PurchaseOrg" FilterControlAltText="PurchaseOrg" HeaderText="Purchase Org" SortExpression="PurchaseOrg" UniqueName="PurchaseOrg">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="PurchaseGroup" FilterControlAltText="PurchaseGroup" SortExpression="PurchaseGroup" HeaderText="Purchase Group" UniqueName="PurchaseGroup">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridCheckBoxColumn UniqueName="RetainMainRole" DataField="RetainMainRole" SortExpression="RetainMainRole" HeaderText="Retain Main Role">
                                    </telerik:GridCheckBoxColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </div>
                </div>

            </asp:Panel>

            <asp:Panel ID="SMSPanel" runat="server">
                <div class="row-fluid">
                    <div class="col-xs-12">
                        <h3>Access Details</h3>
                    </div>
                </div>
                <div class="row-fluid">
                    <div style="height: auto !important;" class="col-xs-12">
                        <telerik:RadGrid ID="SMSGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                            AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true"
                            Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No requests added."
                            MasterTableView-EnableNoRecordsTemplate="true">
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="False" Width="100%">
                                <Columns>
                                    <telerik:GridBoundColumn DataField="TechnicalRole" HeaderText="Technical Role" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="Description" UniqueName="Description" HeaderText="Description" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                    </telerik:GridBoundColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>

                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="O365Panel" runat="server">
                <asp:Panel ID="O365sharedNewAccPanel" runat="server" Visible="false">
                    <div class="row-fluid">
                        <div class="col-xs-12">

                            <h3>Account Details</h3>
                        </div>
                    </div>
                    <div class="row-fluid">
                        <div style="height: auto !important;" class="col-xs-12">
                            <telerik:RadGrid ID="SharedNewAccGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                                AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true"
                                Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No requests added."
                                MasterTableView-EnableNoRecordsTemplate="true">
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView AutoGenerateColumns="False" Width="100%">
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="AccountName" HeaderText="Account Name" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="DisplayName" UniqueName="DisplayName" HeaderText="Display Name" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel ID="O365sharedAmmendmentPanel" runat="server" Visible="false">
                    <div class="row-fluid">
                        <div class="col-xs-12">

                            <h3>Action Details</h3>
                        </div>
                    </div>
                    <div class="row-fluid">
                        <div style="height: auto !important;" class="col-xs-12">
                            <telerik:RadGrid ID="SharedAmendmentgrid" Visible="true" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                                AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true"
                                Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No requests added."
                                MasterTableView-EnableNoRecordsTemplate="true">
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView AutoGenerateColumns="False" Width="100%">
                                    <Columns>
                                        <%-- <telerik:GridTemplateColumn HeaderText="Shared MailBox Action">
                                                            <ItemTemplate>
                                                                <asp:Label ID="Label4" runat="server" Text='<%#Eval("SharedMailBoxAction").ToString() %>' />
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="License Action">
                                                            <ItemTemplate>
                                                                <asp:Label ID="Label4" runat="server" Text='<%#Eval("LicenseAction").ToString() %>' />
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="MailBox Conversion Type">
                                                            <ItemTemplate>
                                                                <asp:Label ID="Label4" runat="server" Text='<%#Eval("MailBoxConversionType").ToString() %>' />
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>--%>
                                        <%--<telerik:GridBoundColumn DataField="SharedMailBoxAction" HeaderText="Shared MailBox Action" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn DataField="LicenseAction" UniqueName="LicenseAction" HeaderText="License Action" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                            </telerik:GridBoundColumn>--%>
                                        <telerik:GridBoundColumn DataField="MailBoxConversionType" UniqueName="MailBoxConversionType" HeaderText="MailBox Conversion Type" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel ID="O365UsrAccAmendment" runat="server" Visible="false">
                    <div class="row-fluid">
                        <div class="col-xs-12">

                            <h3>Action Details</h3>
                        </div>
                    </div>
                    <div class="row-fluid">
                        <div style="height: auto !important;" class="col-xs-12">
                            <telerik:RadGrid ID="UsrAccAmendmntGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                                AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true"
                                Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No requests added."
                                MasterTableView-EnableNoRecordsTemplate="true">
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView AutoGenerateColumns="False" Width="100%">
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="LicenseAction" UniqueName="LicenseAction" HeaderText="License Action" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="ExternalEmailAction" UniqueName="ExternalEmailAction" HeaderText="External Email Action" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel ID="LicensePanel" runat="server">
                    <div class="row-fluid">
                        <div class="col-xs-12">
                            <h3>License Details</h3>
                        </div>
                    </div>
                    <div class="row-fluid">
                        <div style="height: auto !important;" class="col-xs-12">
                            <telerik:RadGrid ID="gridLicense" AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" Visible="false"
                                runat="server" AllowFilteringByColumn="false" AllowPaging="True" AllowSorting="True" CellSpacing="0" Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No Details Found.">
                                <PagerStyle Mode="NextPrevAndNumeric" Width="99%" />
                                <ExportSettings
                                    HideStructureColumns="true"
                                    IgnorePaging="true" ExportOnlyData="true"
                                    Excel-Format="ExcelML"
                                    OpenInNewWindow="true">
                                    <Pdf PageHeight="210mm" AllowPrinting="true" PageWidth="347mm" DefaultFontFamily="Arial Unicode MS" PageTopMargin="45mm"
                                        BorderStyle="Medium" BorderColor="#666666" BorderType="AllBorders">
                                    </Pdf>
                                </ExportSettings>
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="true" DataKeyNames="AccountSkuId" Width="100%">

                                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="true" ShowExportToPdfButton="false" ShowExportToExcelButton="false" ShowExportToCsvButton="false"></CommandItemSettings>
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="AccountSkuId" FilterControlAltText="License" SortExpression="License" HeaderText="License" UniqueName="License" HeaderStyle-Width="300px">
                                        </telerik:GridBoundColumn>

                                        <telerik:GridTemplateColumn DataField="IsRemoveLicense" FilterControlAltText="IsRemoveLicense" HeaderText="Is Remove License" SortExpression="IsRemoveLicense" UniqueName="IsRemoveLicense" HeaderStyle-Width="100px">
                                            <ItemTemplate>
                                                <asp:Label ID="Label1" runat="server" Text='<%#(Convert.ToBoolean(Eval("IsRemoveLicense"))?"Yes":"No")%>'></asp:Label>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn DataField="ServiceName" FilterControlAltText="ServiceName" SortExpression="ServiceName" HeaderText="Service Plans" UniqueName="ServiceName">
                                        </telerik:GridBoundColumn>

                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>

                        </div>
                    </div>
                </asp:Panel>
                <asp:Panel ID="MembersPanel" runat="server">
                    <div class="row-fluid">
                        <div class="col-xs-12">

                            <h3>Members Details</h3>
                        </div>
                    </div>
                    <div class="row-fluid">
                        <div style="height: auto !important;" class="col-xs-12">
                            <telerik:RadGrid ID="gridMembers" AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" Visible="false"
                                runat="server" AllowFilteringByColumn="false" AllowPaging="True" AllowSorting="True" CellSpacing="0" Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No Details Found.">
                                <PagerStyle Mode="NextPrevAndNumeric" Width="99%" />
                                <ExportSettings
                                    HideStructureColumns="true"
                                    IgnorePaging="true" ExportOnlyData="true"
                                    Excel-Format="ExcelML"
                                    OpenInNewWindow="true">
                                    <Pdf PageHeight="210mm" AllowPrinting="true" PageWidth="347mm" DefaultFontFamily="Arial Unicode MS" PageTopMargin="45mm"
                                        BorderStyle="Medium" BorderColor="#666666" BorderType="AllBorders">
                                    </Pdf>
                                </ExportSettings>
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="true" DataKeyNames="key">

                                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="true" ShowExportToPdfButton="false" ShowExportToExcelButton="false" ShowExportToCsvButton="false"></CommandItemSettings>
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="Value" FilterControlAltText="Value" SortExpression="Value" HeaderText="Members" UniqueName="Value" HeaderStyle-Width="300px">
                                        </telerik:GridBoundColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>

                        </div>
                    </div>
                </asp:Panel>
            </asp:Panel>

            <asp:Panel ID="VPNDetailsPanel" runat="server">
                <div class="row-fluid">
                    <div class="col-xs-12">
                        <h3>
                            <asp:Label ID="lblOneKey" runat="server" /></h3>
                    </div>

                </div>
                <div class="row-fluid">
                    <div id="trGrdid" runat="server" style="height: auto !important;" class="col-xs-12">
                        <telerik:RadGrid ID="ApplicationGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                            GridLines="None" Width="800px" ShowFooter="false"
                            MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Applications Available">
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="False" DataKeyNames="identity">
                                <Columns>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="15%" HeaderText="Application" ItemStyle-HorizontalAlign="Center" UniqueName="MatrixType">
                                        <ItemTemplate>
                                            <asp:Label ID="lblMatrixType" Text='<%# Eval("MatrixType").ToString() %>' runat="server"></asp:Label>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="15%" HeaderText="VPN" ItemStyle-HorizontalAlign="Center" UniqueName="VPNType">
                                        <ItemTemplate>
                                            <h4>
                                                <asp:Label ID="lblVPNType" CssClass="label label-primary" Text='<%# Convert.ToString(Eval("VPNType")) %>' runat="server"></asp:Label></h4>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="40%" HeaderText="Name" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="ApplicationNameLabel" runat="server" Text='<%#Eval("ApplicationName").ToString() %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="10%" HeaderText="Production">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Visible='<%# Convert.ToBoolean(Eval("Production")) %>'>Yes</asp:Label>
                                            <asp:Label runat="server" Visible='<%# !Convert.ToBoolean(Eval("Production")) %>'>No</asp:Label>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="10%" HeaderText="QA">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Visible='<%# Convert.ToBoolean(Eval("QA")) %>'>Yes</asp:Label>
                                            <asp:Label runat="server" Visible='<%# !Convert.ToBoolean(Eval("QA")) %>'>No</asp:Label>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="10%" HeaderText="Development">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Visible='<%# Convert.ToBoolean(Eval("Dev")) %>'>Yes</asp:Label>
                                            <asp:Label runat="server" Visible='<%# !Convert.ToBoolean(Eval("Dev")) %>'>No</asp:Label>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                    <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="Others" Visible="false" Display="false">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Visible='<%# Convert.ToBoolean(Eval("isOthers")) %>'>Yes</asp:Label>
                                            <asp:Label runat="server" Visible='<%# !Convert.ToBoolean(Eval("isOthers")) %>'>No</asp:Label>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>

                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="GEMSDetailPanel" runat="server">
                <div>

                    <h3>Entitlement Details</h3>
                </div>

                <div style="text-align: right; border-bottom: 0px;">
                    <telerik:RadGrid ID="GEMSGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                        AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true"
                        Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No requests added."
                        MasterTableView-EnableNoRecordsTemplate="true" Visible="false">
                        <GroupingSettings CaseSensitive="false" />
                        <MasterTableView AutoGenerateColumns="False">
                            <Columns>
                                <telerik:GridTemplateColumn HeaderStyle-Width="300px" HeaderText="Business Role" ItemStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:Label ID="EntitlementLabel" runat="server" Text='<%#Eval("Entitlement").ToString() %>' />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>

                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>

                </div>

            </asp:Panel>

            <asp:Panel ID="NPACDetailPanel" runat="server">
                <div class="row-fluid">
                    <div class="col-xs-12">

                        <h3>Entitlement Details</h3>
                    </div>
                </div>
                <div class="row-fluid">
                    <div style="text-align: right; border-bottom: 0px;" class="col-xs-12">
                        <telerik:RadGrid ID="NPACGrid" OnItemDataBound="NPACGrid_ItemDataBound" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                            AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true"
                            Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No requests added."
                            MasterTableView-EnableNoRecordsTemplate="true" Visible="false">
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView DataKeyNames="Entitlement" AutoGenerateColumns="False">
                                <Columns>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="5%" HeaderText="Level" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="EntitlementLabel1" runat="server" Text='<%#Convert.ToString(Eval("Entitlement")) %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                    <telerik:GridTemplateColumn HeaderStyle-Width="5px" HeaderText="Section" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="EntitlementLabel3" runat="server" Text='<%#Convert.ToString(Eval("Section")) %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                    <telerik:GridTemplateColumn HeaderStyle-Width="25%" HeaderText="Email" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="EntitlementLabel2" runat="server" Text='<%#Convert.ToString(Eval("Email")) %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                    <telerik:GridTemplateColumn HeaderStyle-Width="5px" HeaderText="Hand Phone" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="EntitlementLabel3" runat="server" Text='<%#Convert.ToString(Eval("PhoneNumber")) %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>

                    </div>
                </div>
            </asp:Panel>


            <asp:Panel ID="SAPEPPanel" runat="server">
                <asp:Panel ID="SAPEPGroupPanel" runat="server">
                    <div>
                        <h3>Entitlement Details:</h3>
                    </div>

                    <div>
                        <telerik:RadGrid ID="SAPEPGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                            AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true"
                            Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No requests added."
                            MasterTableView-EnableNoRecordsTemplate="true" Visible="false">
                            <MasterTableView AutoGenerateColumns="False">
                                <Columns>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="300px" HeaderText="Request Details" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="RequestDetailLabel" runat="server" Text='<%#Eval("RequestDetail").ToString() %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>

                    </div>

                </asp:Panel>
                <asp:Panel ID="SAPEPSharedPanel" runat="server" Visible="false">
                    <div>
                        <h3>
                            <asp:Label ID="Label19" runat="server" CssClass="subTitle" Text="Entitlement Details"></asp:Label>
                        </h3>
                    </div>
                    <div>
                        <telerik:RadGrid ID="SAPEPSharedGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                            AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true"
                            Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No requests added."
                            MasterTableView-EnableNoRecordsTemplate="true" Visible="false">
                            <MasterTableView AutoGenerateColumns="False">
                                <Columns>
                                    <telerik:GridBoundColumn DataField="AccountName" HeaderText="Account Name" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="Description" UniqueName="Description" HeaderText="Description" Display="false" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                    </telerik:GridBoundColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </div>
                </asp:Panel>
            </asp:Panel>

            <asp:Panel ID="PanelofMSSL" runat="server" Visible="false">
                <asp:Panel ID="GroupAccountMSSLPanel" runat="server">
                    <div>
                        <h3 class="model-title">Entitlement Details</h3>
                    </div>

                    <div>
                        <telerik:RadGrid ID="SAPMSSLGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                            AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true"
                            Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No requests added."
                            MasterTableView-EnableNoRecordsTemplate="true" Visible="false">
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="False" Width="100%">
                                <Columns>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="300px" HeaderText="Position" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="EntitlementLabel" runat="server" Text='<%# string.IsNullOrEmpty((string)Eval("Position")) ? "" : Eval("Position")%>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="300px" HeaderText="Roles" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="EntitlementLabel" runat="server" Text='<%# string.IsNullOrEmpty((string)Eval("Roles")) ? "" : Eval("Roles")%>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>

                    </div>

                </asp:Panel>
                <asp:Panel ID="SharedAccountMSSLPanel" runat="server" Visible="false">
                    <div>
                        <h3>Account Details</h3>
                        <div>
                            <telerik:RadGrid ID="SharedMSSLGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                                AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true"
                                Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No requests added."
                                MasterTableView-EnableNoRecordsTemplate="true" Visible="false">
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView AutoGenerateColumns="False" Width="100%">
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="SAPUserType" HeaderText="SAP UserType" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="AccountName" HeaderText="Account Name" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Description" UniqueName="Description" HeaderText="Description" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                        </div>
                </asp:Panel>
                <div class="form-group row" runat="server" id="divReAllocate" visible="false">
                    <div class="col-xs-3">
                        <b>Re-allocate</b>
                    </div>
                    <div class="col-xs-9">
                        <asp:Label runat="server" ID="lblIsReAllocate" Text="" CssClass="badge badge-secondary"></asp:Label>
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlSkypeForBusiness" runat="server">
                <div class="form-group row">
                    <div class="col-xs-12">
                        <h3>Entitlement Details</h3>
                    </div>
                </div>
                <div class="form-group row">
                    <label class="col-xs-3 control-label">Request Detail: </label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblType" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="form-group row required" runat="server" visible="false" id="divLineURI">
                    <label class="col-xs-3 control-label">LineURI</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblLineURI" runat="server" Text=""></asp:Label>
                    </div>
                </div>
                <div class="form-group row" runat="server" visible="false" id="pnlDCCExtension">
                    <label class="col-xs-3 control-label">Extension Number</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblDCCExtension" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="form-group row" runat="server" visible="false" id="divRegistrarPool">
                    <label class="col-xs-3 control-label">Registrar Pool:</label>
                    <div class="col-xs-9">
                        <asp:Label ID="lblRegistrarPool" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="form-group row" id="pnlResponseGroup" runat="server" visible="false">
                    <div class="col-xs-12">
                        <telerik:RadGrid ID="ResponseGroupGrid" runat="server" Visible="false" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                            GridLines="None" ShowFooter="false"
                            MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Records Available" tablelayout="Fixed" CssClass="RadGridtextWrap">
                            <GroupingSettings CaseSensitive="false" />
                            <ClientSettings>
                                <Scrolling AllowScroll="true" ScrollHeight="80px" UseStaticHeaders="true" SaveScrollPosition="True" />
                            </ClientSettings>
                            <MasterTableView AutoGenerateColumns="False" DataKeyNames="Id" TableLayout="Fixed" Width="100%">
                                <Columns>
                                    <telerik:GridBoundColumn DataField="Name" FilterControlAltText="Name" SortExpression="Name" HeaderText="Response Group Name" UniqueName="Name">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="Mask" FilterControlAltText="Mask" SortExpression="Mask" HeaderText="Mask" UniqueName="Mask">
                                    </telerik:GridBoundColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="EAM_Panel" runat="server" Visible="false">

                <div>
                    <h3>Entitlement Details</h3>
                </div>
                <div class="row-fluid">
                    <div class="col-xs-12">

                        <asp:Panel ID="EAM_Entitlement_Panel" runat="server" Visible="false">
                            <div>
                                <div>
                                    <div style="width: 95%" class="model-header">
                                        <h3 class="model-title">Account Details </h3>
                                    </div>
                                </div>
                                <div class="model-body">
                                    <telerik:RadGrid ID="eam_AccountGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                                        AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true"
                                        Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No requests added."
                                        MasterTableView-EnableNoRecordsTemplate="true" Visible="false">
                                        <GroupingSettings CaseSensitive="false" />
                                        <MasterTableView AutoGenerateColumns="False" Width="100%">
                                            <Columns>
                                                <telerik:GridBoundColumn DataField="SAPUserType" HeaderText="SAP UserType" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="AccountName" HeaderText="Account Name" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                                </telerik:GridBoundColumn>
                                            </Columns>
                                        </MasterTableView>
                                    </telerik:RadGrid>
                                </div>
                            </div>
                        </asp:Panel>

                        <br />

                        <br />
                        <telerik:RadGrid ID="NovaRoleGrid" runat="server" Visible="false" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                            GridLines="None" ShowFooter="false"
                            MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Records Available" tablelayout="Fixed" CssClass="RadGridtextWrap">
                            <GroupingSettings CaseSensitive="false" />
                            <ClientSettings>
                                <Scrolling AllowScroll="true" ScrollHeight="80px" UseStaticHeaders="true" SaveScrollPosition="True" />
                            </ClientSettings>
                            <MasterTableView AutoGenerateColumns="false" DataKeyNames="Id" TableLayout="Fixed" Width="100%">
                                <Columns>
                                    <telerik:GridBoundColumn HeaderStyle-Width="15%" DataField="NovaMatrixType" HeaderText="MatrixType" ItemStyle-HorizontalAlign="Center" UniqueName="NovaMatrixType">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="15%" DataField="JobDescription" HeaderText="Description" ItemStyle-HorizontalAlign="Center" UniqueName="JobDescription">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblRoleName" Text='<%# Convert.ToString(Eval("JobDescription"))+"("+Convert.ToString(Eval("RoleName"))+")" %>'></asp:Label>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <%-- <telerik:GridBoundColumn HeaderStyle-Width="15%" DataField="RoleName" HeaderText="RoleName" ItemStyle-HorizontalAlign="Center" UniqueName="RoleName">
                                </telerik:GridBoundColumn>--%>
                                    <%--<telerik:GridBoundColumn HeaderStyle-Width="15%" DataField="JobDescription" HeaderText="Description" ItemStyle-HorizontalAlign="Center" UniqueName="JobDescription">
                                </telerik:GridBoundColumn>--%>
                                    <telerik:GridBoundColumn HeaderStyle-Width="15%" DataField="CompanyCode" HeaderText="CompanyCode" ItemStyle-HorizontalAlign="Center" UniqueName="CompanyCode">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn HeaderStyle-Width="15%" DataField="SectionCode" HeaderText="SectionCode" ItemStyle-HorizontalAlign="Center" UniqueName="SectionCode">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn HeaderStyle-Width="15%" DataField="DepartmentCode" HeaderText="DepartmentCode" ItemStyle-HorizontalAlign="Center" UniqueName="DepartmentCode">
                                    </telerik:GridBoundColumn>
                                    <%-- <telerik:GridButtonColumn ConfirmDialogType="RadWindow" ConfirmTitle="Delete" UniqueName="Delete" ConfirmText="Are you sure you want to delete?" CommandName="Delete" ButtonType="ImageButton" ItemStyle-CssClass="action-delete" ItemStyle-Width="1px" HeaderText="">
                                        <HeaderStyle Width="50px" />
                                    </telerik:GridButtonColumn>--%>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </div>
                </div>



            </asp:Panel>

            <asp:Panel ID="PanelofSAPHANA" runat="server" Visible="false">
                <asp:Panel ID="GroupAccountSAPHANAPanel" runat="server">
                    <div>
                        <h3 class="model-title">Entitlement Details</h3>
                    </div>
                    <div>
                        <telerik:RadGrid ID="SAPHANAGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                            AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true"
                            Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No requests added."
                            MasterTableView-EnableNoRecordsTemplate="true" Visible="false">
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="False" Width="100%">
                                <Columns>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="300px" HeaderText="Position" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="EntitlementLabel" runat="server" Text='<%# string.IsNullOrEmpty((string)Eval("Position")) ? "" : Eval("Position")%>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="300px" HeaderText="Roles" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="EntitlementLabel" runat="server" Text='<%# string.IsNullOrEmpty((string)Eval("Roles")) ? "" : Eval("Roles")%>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>

                    </div>

                </asp:Panel>
                <asp:Panel ID="SharedAccountSAPHANAPanel" runat="server" Visible="false">
                    <div>
                        <h3>Account Details</h3>
                        <div>
                            <telerik:RadGrid ID="SharedSAPHANAGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                                AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true"
                                Width="100%" Height="100%" GridLines="None" MasterTableView-NoMasterRecordsText="No requests added."
                                MasterTableView-EnableNoRecordsTemplate="true" Visible="false">
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView AutoGenerateColumns="False" Width="100%">
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="SAPUserType" HeaderText="SAP UserType" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="AccountName" HeaderText="Account Name" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Description" UniqueName="Description" HeaderText="Description" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                                        </telerik:GridBoundColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                        </div>
                </asp:Panel>
                <div class="form-group row" runat="server" id="divSAPHANAReAllocate" visible="false">
                    <div class="col-xs-3">
                        <b>Re-allocate</b>
                    </div>
                    <div class="col-xs-9">
                        <asp:Label runat="server" ID="lblIsSAPHANAReAllocate" Text="" CssClass="badge badge-secondary"></asp:Label>
                    </div>
                </div>
            </asp:Panel>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
