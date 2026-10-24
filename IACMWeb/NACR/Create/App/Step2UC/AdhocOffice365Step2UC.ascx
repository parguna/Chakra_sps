<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocOffice365Step2UC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.Step2UC.AdhocOffice365Step2UC" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<style>
    #ctl00_cplMain_WizardCartUC_AccessUC_AdhocOffice365Step2UC_ServicePlanComboBox{
        width:100% !important;
    }
</style>
<div class="form">
    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="form-group">
                <div class="col-xs-6">

                    <asp:Panel ID="CommonPanel" runat="server" Visible="false">

                        <div id="trEndPointUser" class="form-group required" runat="server">
                            <label class="col-xs-3 control-label">EndPoint User Name</label>
                            <div class="col-xs-8">
                                <asp:DropDownList ID="ddlEndpointUser" OnSelectedIndexChanged="ddlEndpointUser_SelectedIndexChanged" AutoPostBack="true" runat="server" CssClass="form-control">
                                </asp:DropDownList>
                            </div>
                        </div>


                    </asp:Panel>

                    <asp:Panel ID="UserAccPanel" runat="server" Visible="false">

                        <div id="TRExternalEmail" class="form-group" runat="server">

                            <label id="lblAllowExternalEmail" runat="server" class="col-xs-3 control-label"></label>
                            <div class="col-xs-8">
                                <asp:CheckBox ID="chkboxExtenalEmail" runat="server" />
                            </div>
                        </div>

                    </asp:Panel>

                    <asp:Panel runat="server" ID="SharedAccPannel" Visible="false">

                        <div class="form-group required">
                            <label id="Label1" runat="server" class="col-xs-3 control-label">Shared Account Type</label>
                            <div class="col-xs-8">

                                <asp:RadioButton ID="rbtninternal" runat="server" AutoPostBack="true" Checked="true" GroupName="groupOfDefaultRadios" />
                                <span id="InternalRadioButton" runat="server" class="lbl"></span>

                                <asp:RadioButton ID="rbtnshared" runat="server" AutoPostBack="true" Checked="false" GroupName="groupOfDefaultRadios" />
                                <span id="SharedRadioButton" runat="server" class="lbl"></span>



                                <%--<asp:RadioButton ID="InternalRadioButton" runat="server" Checked="True" AutoPostBack="true"   GroupName="SharedAccountType" Text="" />

                                <asp:RadioButton ID="SharedRadioButton" runat="server" AutoPostBack="true"  GroupName="SharedAccountType" Text="" />--%>
                            </div>
                        </div>

                        <div class="form-group required">
                            <label class="col-xs-3 control-label">Display Name</label>
                            <div class="col-xs-8">
                                <asp:TextBox ID="txtDisplayName" runat="server" CssClass="form-control" MaxLength="256"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group required">
                            <label class="col-xs-3 control-label">Alias</label>
                            <div class="col-xs-8">
                                <asp:TextBox ID="txtAlias" CssClass="form-control" runat="server" MaxLength="30"></asp:TextBox>
                            </div>
                        </div>

                    </asp:Panel>

                </div>
            </div>
        </div>
    <asp:Panel runat="server" ID="MemberPanel" Visible="false">

        <div>
            <div class="form-horizontal left clearfix">
                <div class="row-fluid">
                    <div class="form-group">
                        <div class="col-xs-6">

                            <div id="TRmember" class="form-group required" runat="server">
                                <label class="col-xs-3 control-label">Members</label>
                                <div class="col-xs-8 input-group" style="padding-left: 15px;">
                                    <table style="width:100%">
                                <tr>
                                    <td style="width:70%">
                                    <telerik:RadAutoCompleteBox ID="MembersAutoCompletebox" CssClass="form-control" runat="server" InputType="Token" EmptyMessage="Enter * to view full list" DropDownHeight="200px" Width="325px" AutoPostBack="true" OnEntryAdded="MembersAutoCompletebox_EntryAdded" OnEntryRemoved="MembersAutoCompletebox_EntryRemoved">
                                        <TextSettings SelectionMode="Single" />
                                        <WebServiceSettings Method="GetO365Users" Path="../../../../Services/AutoCompleteDataService.svc" />
                                    </telerik:RadAutoCompleteBox>
                                    </td>
                                     <td>
                                        <asp:Button ID="btnAddMember" runat="server" CssClass="btn btn-default btn-sm" Text="Add Member" Enabled="true"  OnClick="btnAddMember_Click" />
                                     </td>
                                </tr>
                                  </table>          
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div>
            <div class="form-horizontal left clearfix">
                <div class="row-fluid">
                    <div class="form-group">
                        <%--<label class="col-xs-2"></label>--%>
                        <div class="col-xs-9">
                            <telerik:RadGrid ID="MemeberGrid" runat="server" Visible="false" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                                GridLines="None" ShowFooter="false" OnItemCommand="MemeberGrid_ItemCommand"
                                MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Records Available" TableLayout="Fixed" CssClass="RadGridtextWrap">
                                <GroupingSettings CaseSensitive="false" />
                                <ClientSettings>
                                    <Scrolling AllowScroll="true" ScrollHeight="80px" UseStaticHeaders="true" SaveScrollPosition="True" />
                                </ClientSettings>
                                <MasterTableView AutoGenerateColumns="False" DataKeyNames="key" TableLayout="Fixed"  Width="100%">
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="Value" FilterControlAltText="Value" SortExpression="Value" HeaderText="Member" UniqueName="Value">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridButtonColumn ConfirmDialogType="RadWindow" ConfirmTitle="Delete" UniqueName="Delete" ConfirmText="Are you sure you want to delete?" CommandName="Delete" ButtonType="ImageButton" ItemStyle-CssClass="action-delete" ItemStyle-Width="1px" HeaderText="">
                                            <HeaderStyle Width="50px" />
                                        </telerik:GridButtonColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="ConversionPanel" runat="server">

        <div class="form-horizontal left clearfix">
            <div class="row-fluid">
                <div class="form-group">
                    <div class="col-xs-6">
                        <div runat="server" id="trMailConversionType" class="form-group required">
                            <label id="LblMailBoxConversionType" runat="server" class="col-xs-3 control-label"></label>
                            <div class="col-xs-8">
                                <asp:CheckBox ID="chkboxMailConversionType" Checked="true" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </asp:Panel>

    <asp:Panel runat="server" ID="LicensePanel" Visible="false" CssClass="form-group">
        <div class="form-horizontal left clearfix">
            <div class="row-fluid">
                <div class="col-xs-6">
                    <div class="form-group">
                        <label class="col-xs-3 control-label">License</label>
                        <div class="col-xs-8 form-group" style="padding-left:30px">
                            <telerik:RadAutoCompleteBox ID="LicenseAutoCompleteBox" CssClass="form-control" OnEntryRemoved="LicenseAutoCompleteBox_EntryRemoved" OnEntryAdded="LicenseAutoCompleteBox_EntryAdded" InputType="Token" EmptyMessage="Enter * to view full list" runat="server" DropDownHeight="200px" DropDownWidth="300px" AutoPostBack="true">
                                <TextSettings SelectionMode="Single" />
                                <WebServiceSettings Method="GetAdhocLicenseInfo"
                                    Path="../../../../Services/AutoCompleteDataService.svc" />
                            </telerik:RadAutoCompleteBox>
                            <asp:CheckBox ID="chkboxRemoveLicense" runat="server" Text="Remove License" Visible="false" AutoPostBack="true" OnCheckedChanged="chkboxRemoveLicense_CheckedChanged" Style="vertical-align: super;" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-3 control-label">Service Plans</label>
                        <div class="col-xs-8 form-group" style="padding-left:30px">
                            <table style="width:100%">
                                <tr>
                                    <td style="width:85%">
                                <telerik:RadComboBox ID="ServicePlanComboBox" runat="server" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" DropDownHeight="200px"  AutoPostBack="true"></telerik:RadComboBox>
                                    </td>
                                    <td>
                                    <asp:Button ID="btnAdd" runat="server" Text="Add" Enabled="false" CssClass="btn btn-default btn-sm" OnClick="btnAdd_Click" />
                                    </td>
                                </tr>
                            </table>
                        </div>

                    </div>
                </div>
            </div>

        </div>
                <div class="form-horizontal left clearfix">
                <div class="row-fluid">
                <div class="col-xs-12">
                <div class="form-group">
                <div class="col-xs-12">
            <telerik:RadGrid ID="ServicePlanGrid" runat="server" Visible="false" AllowPaging="false" AllowSorting="false" CellSpacing="0"
            GridLines="None" Width="100%" ShowFooter="false" OnItemCommand="ServicePlanGrid_ItemCommand"
            MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Records Available" TableLayout="Fixed">
            <GroupingSettings CaseSensitive="false" />
            <MasterTableView AutoGenerateColumns="False" DataKeyNames="AccountSkuId" TableLayout="Fixed"  Width="100%">
                <Columns>
                    <telerik:GridBoundColumn DataField="AccountSkuId" FilterControlAltText="License" SortExpression="License" HeaderText="License" UniqueName="License" HeaderStyle-Width="50%">
                    </telerik:GridBoundColumn>

                    <telerik:GridTemplateColumn DataField="IsRemoveLicense" FilterControlAltText="IsRemoveLicense" HeaderText="Is Remove License" SortExpression="IsRemoveLicense" UniqueName="IsRemoveLicense" HeaderStyle-Width="20%">
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%#(Convert.ToBoolean(Eval("IsRemoveLicense"))?"Yes":"No")%>'></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="ServiceName" FilterControlAltText="ServiceName" SortExpression="ServiceName" HeaderText="Service Plans" UniqueName="ServiceName" HeaderStyle-Width="20%">
                    </telerik:GridBoundColumn>
                    <telerik:GridButtonColumn ConfirmDialogType="RadWindow" ConfirmTitle="Delete" UniqueName="Delete" ConfirmText="Are you sure you want to delete?" CommandName="Delete" ButtonType="ImageButton" ItemStyle-CssClass="action-delete" ItemStyle-Width="1px" HeaderText="" HeaderStyle-Width="10%">
                        <HeaderStyle Width="30px" />

                    </telerik:GridButtonColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
        </div>
        </div>
        </div>
        </div>
        </div>
    </asp:Panel>
    <br />
    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="form-group">
                <div class="col-xs-6">

                    <div class="form-group">
                        <label class="col-xs-3 control-label"></label>
                        <div class="col-xs-8">
                            <asp:Button ID="Button1" CssClass="btn btn-default btn-sm" OnClick="AddToCart" Text="Save Request" runat="server" Visible="false"/>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>
    </div>
    

</div>
