<%@ Page Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="MassPropagateO365License.aspx.cs" Inherits="DI.Chakra.Web.Admin.AdminUI.MassPropagateO365License" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cplMain" runat="server">
    <style>
        .licenseWidth{
            width: 325px !important;
        }
    </style>
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
            <div class="content-wrapper">
                <div class="form">
                    <div class="form-horizontal left readonly">
                        <div class="row-fluid form-group">
                            <div class="col-xs-2">
                                <asp:Label ID="Label3" runat="server" class="wizlbl" Text="Propagation Type"></asp:Label>
                            </div>
                            <div class="col-xs-10">
                                <asp:RadioButton ID="Addchkbox" runat="server" Checked="True" Text="Add" OnCheckedChanged="Addchkbox_CheckedChanged" GroupName="PropagationtypeChange" AutoPostBack="true"></asp:RadioButton>
                                <asp:RadioButton ID="Removechkbox" runat="server" Text="Remove" OnCheckedChanged="Addchkbox_CheckedChanged" GroupName="PropagationtypeChange" AutoPostBack="true"></asp:RadioButton>
                            </div>
                        </div>

                        <div class="row-fluid form-group">
                            <div class="col-xs-2">
                                <asp:Label ID="lblLicense" runat="server" class="wizlbl" Text="License"></asp:Label>
                            </div>
                            <div class="col-xs-10">
                                <telerik:RadAutoCompleteBox ID="LicenseAutoCompleteBox" CssClass="form-control" OnEntryRemoved="LicenseAutoCompleteBox_EntryRemoved" OnEntryAdded="LicenseAutoCompleteBox_EntryAdded" InputType="Token" EmptyMessage="Enter * to view full list" runat="server" DropDownHeight="200px" DropDownWidth="300px" Width="325px" AutoPostBack="true">
                                    <TextSettings SelectionMode="Single" />
                                    <WebServiceSettings Method="GetLicenseInfoForBulkReq"
                                        Path="../../Services/AutoCompleteDataService.svc" />
                                </telerik:RadAutoCompleteBox>
                                <asp:CheckBox ID="chkboxRemoveLicense" runat="server" Text="Remove License" Visible="false" AutoPostBack="true" OnCheckedChanged="chkboxRemoveLicense_CheckedChanged" Width="300px" Style="vertical-align: super;" />
                            </div>
                        </div>
                        <div class="row-fluid form-group">
                            <div class="col-xs-2">
                                <asp:Label ID="Label2" runat="server" class="wizlbl" Text="Service Plans"></asp:Label>
                            </div>
                            <div class="col-xs-10">
                                <telerik:RadComboBox ID="ServicePlanComboBox" runat="server" CheckBoxes="true" OnSelectedIndexChanged="ServicePlanComboBox_SelectedIndexChanged" EnableCheckAllItemsCheckBox="true" DropDownHeight="200px" DropDownWidth="300px" Width="325px" CssClass="licenseWidth" AutoPostBack="true"></telerik:RadComboBox>
                                <asp:Button ID="btnAdd" runat="server" Text="Add" Enabled="false" OnClick="btnAdd_Click" CssClass="btn btn-default btn-sm" />
                            </div>
                        </div>
                        <div class="row-fluid form-group">
                            <div class="col-xs-12">
                                <telerik:RadGrid ID="ServicePlanGrid" runat="server" Visible="false" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                                    GridLines="None" Width="800px" ShowFooter="false" OnItemCommand="ServicePlanGrid_ItemCommand"
                                    MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Records Available" TableLayout="Fixed">
                                    <GroupingSettings CaseSensitive="false" />
                                    <MasterTableView AutoGenerateColumns="False" DataKeyNames="License" TableLayout="Fixed"  Width="100%" >
                                        <Columns>
                                            <telerik:GridBoundColumn DataField="License" FilterControlAltText="License" SortExpression="License" HeaderText="License" UniqueName="License" HeaderStyle-Width="300px">
                                            </telerik:GridBoundColumn>

                                            <telerik:GridTemplateColumn DataField="IsRemoveLicense" FilterControlAltText="IsRemoveLicense" HeaderText="Is Remove License" SortExpression="IsRemoveLicense" UniqueName="IsRemoveLicense" HeaderStyle-Width="100px">
                                                <ItemTemplate>
                                                    <asp:Label ID="Label1" runat="server" Text='<%#(Convert.ToBoolean(Eval("IsRemoveLicense"))?"Yes":"No")%>'></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridBoundColumn DataField="ServiceName" FilterControlAltText="ServiceName" SortExpression="ServiceName" HeaderText="Service Plans" UniqueName="ServiceName">
                                            </telerik:GridBoundColumn>
                                            <telerik:GridButtonColumn ConfirmDialogType="RadWindow" ConfirmTitle="Delete" UniqueName="Delete" ConfirmText="Are you sure you want to delete?" CommandName="Delete" ButtonType="ImageButton" ItemStyle-CssClass="action-delete" ItemStyle-Width="1px" HeaderText="" HeaderStyle-Width="50px">
                                                <HeaderStyle Width="30px" />

                                            </telerik:GridButtonColumn>
                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>
                            </div>
                        </div>
                        <div class="row-fluid form-group">
                            <div class="col-xs-6">
                                <asp:Button ID="Savebtn" runat="server" OnClick="Savebtn_Click" Text="Save" CssClass="btn btn-default btn-sm" />
                            </div>
                        </div>
                    </div>
                </div>
                 <div class="buttons text-right nav-rectangle">
        <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="btnCancel_Click" />
    </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

