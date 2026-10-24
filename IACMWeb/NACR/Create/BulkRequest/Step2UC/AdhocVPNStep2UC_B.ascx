<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocVPNStep2UC_B.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC.AdhocVPNStep2UC_B" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<style type="text/css">
   
    .label{
        font-size:100%;
    }

</style>
<script type="text/javascript">
    function OnClientRequesting(sender, args) {
        var context = args.get_context()
        var e = document.getElementById("<%= ddlVPNType.ClientID%>");
        var hiddenVPNType = e.options[e.selectedIndex].value;
        context["vpnType"] = hiddenVPNType;
        context["endpointId"] = "<%= EndpointSystemId %>";
    }
</script>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <div class="form">
            <div class="form-horizontal left clearfix">
                <div class="row-fluid">
                    <asp:Panel ID="pnlEndPointUserName" runat="server" Visible="false">

                        <div class="form-group">
                            <div class="col-xs-4">
                                <label class="control-label">Account Information: </label>
                            </div>
                        </div>
                         <div class="form-group">
                            <div class="col-xs-8">
                                <telerik:RadGrid ID="AccountRepeater" runat="server" AllowAutomaticUpdates="false" EnableAjaxSkinRendering="true" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                            GridLines="None" ShowFooter="false" MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Request Available">
                            <GroupingSettings CaseSensitive="false" />
                            <ClientSettings EnableRowHoverStyle="true">
                                <Scrolling UseStaticHeaders="true" />
                                <Selecting AllowRowSelect="true" />
                            </ClientSettings>
                            <MasterTableView AutoGenerateColumns="False" DataKeyNames="key,Value"  Width="100%">
                                <Columns>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="30%" HeaderText="User" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAcountName" runat="server" CssClass="required" Text='<%# Eval("key") %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                    <telerik:GridTemplateColumn HeaderStyle-Width="30%" HeaderText="Accounts" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <telerik:RadComboBox ID="ddlAccounts" runat="server" CssClass="selectpicker  AccountsPageComoboWidth radbox100Width" DataSource='<%# Eval("Value")%>' DataTextField="value" DataValueField="key"></telerik:RadComboBox>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                                </div>
                             </div>
                    </asp:Panel>
                </div>
                <div class="form-group">
                            <div class="col-xs-4">
                                </div>
                </div>
              
                <asp:Panel ID="pnlType" runat="server" CssClass="row">
                     <div class="required"  runat="server" id="divVPNType" visible="false" >
                            <div class="col-xs-4"><label class=" col-xs-12 control-label required">VPN Type</label>
                            </div>
                         <div class="col-xs-4">
                            <asp:DropDownList CssClass="form-control selectpicker" runat="server" ID="ddlVPNType" AutoPostBack="true" OnSelectedIndexChanged="ddlVPNType_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>    
                </asp:Panel>
                <asp:Panel ID="pnlGUI" runat="server" Visible="true">
                    <div class="form-group row">
                        <label class="col-xs-4 control-label required" style="padding-left:15px;">Application/Project</label>
                        <div class="col-xs-4">
                            <telerik:RadAutoCompleteBox ID="ApplicationAutoCompleteBox" OnEntryRemoved="ApplicationAutoCompleteBox_EntryRemoved" OnEntryAdded="ApplicationAutoCompleteBox_EntryAdded"
                                EmptyMessage="Enter * to view full list" runat="server" DropDownHeight="200px" DropDownWidth="300px" InputType="Token" OnClientRequesting="OnClientRequesting"
                                CssClass="form-control" AutoPostBack="true">
                                <TextSettings SelectionMode="Single" />
                                <WebServiceSettings Method="GetVPNApplicationsForBulkRequest" Path="../../../../Services/AutoCompleteDataService.svc" />
                            </telerik:RadAutoCompleteBox>
                        </div>
                       
                    </div>
                    <div class="form-group">
                        <div class="col-xs-4">&nbsp;</div>
                        <div class="col-xs-7">
                            <div style='<%=ShowEnviornment ? "" : "display: none" %>'>
                                <div class="row" style='<%=IsProd ? "" : "display: none" %>' >
                                    <div class="col-xs-3">
                                        <asp:CheckBox ID="chkProduction" runat="server" Text="Production" Visible="true"/>
                                    </div>
                                    <div class="col-xs-6">
                                        <asp:Label ID="lblProductionADNames" CssClass="label" runat="server" Visible="false"></asp:Label>
                                    </div>
                                </div>
                                <div class="row" style='<%=IsQA ? "" : "display: none" %>' >
                                    <div class="col-xs-3">
                                        <asp:CheckBox ID="chkQA" runat="server" Text="QA" Visible="true"/>
                                    </div>
                                    
                                    <div  class="col-xs-6">
                                        <asp:Label ID="lblQAADNames" CssClass="label" runat="server" Visible="false"></asp:Label>
                                    </div>
                                </div>
                                <div class="row" style='<%=IsDev ? "" : "display: none" %>' >
                                    <div class="col-xs-3">
                                        <asp:CheckBox ID="chkDevelopment" runat="server" Text="Development" Visible="true"/>
                                    </div>
                                    <div class="col-xs-6">
                                        <asp:Label ID="lblDevADNames" CssClass="label" runat="server" Visible="false"></asp:Label>
                                    </div>
                                </div>
                                <div class="row" style='<%=IsProject ? "" : "display: none" %>' >
                                    <div class="col-xs-3">
                                        <asp:CheckBox ID="chkProjectEnv" runat="server" Text="ADGroup" Visible="false"/>
                                    </div>
                                     <div class="col-xs-6">
                                        <asp:Label ID="lblProjectName" CssClass="label" runat="server" Visible="false"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
    
                 <div class="form-group">
                    <div class="col-xs-4">&nbsp;</div>
                    <div class="col-xs-4">
                        <asp:Button ID="btnAddRequest" runat="server" Text="Add" CssClass="btn btn-default" OnClick="btnAddRequest_Click"></asp:Button>
                    </div>
                    
                </div>
              
                <div class="form-group">
                    <div class="col-xs-12 col-sm-12 col-lg-12 col-md-12">
                        <telerik:RadGrid ID="ApplicationGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                            GridLines="None" Width="100%" OnItemCommand="ApplicationGrid_ItemCommand" ShowFooter="false" 
                            MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Applications Available">
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView AutoGenerateColumns="false" DataKeyNames="identity"  Width="100%">
                                <Columns>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="15%" HeaderText="Application" ItemStyle-HorizontalAlign="Center" UniqueName="MatrixType">
                                        <ItemTemplate>
                                            <asp:Label ID="lblMatrixType" Text='<%# Eval("MatrixType").ToString() %>' runat="server" ></asp:Label>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="15%" HeaderText="VPN" ItemStyle-HorizontalAlign="Center" UniqueName="VPNType">
                                        <ItemTemplate>
                                            <h4><asp:Label ID="lblVPNType" CssClass="label label-primary"  Text='<%# Convert.ToString(Eval("VPNType")) %>' runat="server"></asp:Label></h4>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="40%" HeaderText="Name" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="ApplicationNameLabel" runat="server" Text='<%#Eval("ApplicationName").ToString() %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="Production">
                                            <ItemTemplate>
                                                <asp:Label runat="server"   Visible='<%# Convert.ToBoolean(Eval("Production")) %>'>Yes</asp:Label>
                                                <asp:Label runat="server"   Visible='<%# !Convert.ToBoolean(Eval("Production")) %>'>No</asp:Label>
                                    
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="QA">
                                            <ItemTemplate>
                                                <asp:Label runat="server"   Visible='<%# Convert.ToBoolean(Eval("QA")) %>'>Yes</asp:Label>
                                                <asp:Label runat="server"   Visible='<%# !Convert.ToBoolean(Eval("QA")) %>'>No</asp:Label>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="Development">
                                            <ItemTemplate>
                                                <asp:Label runat="server"   Visible='<%# Convert.ToBoolean(Eval("Dev")) %>'>Yes</asp:Label>
                                                <asp:Label runat="server"   Visible='<%# !Convert.ToBoolean(Eval("Dev")) %>'>No</asp:Label>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                            
                                        <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="Others">
                                            <ItemTemplate>
                                                 <asp:Label runat="server"   Visible='<%# Convert.ToBoolean(Eval("isOthers")) %>'>Yes</asp:Label>
                                                <asp:Label runat="server"   Visible='<%# !Convert.ToBoolean(Eval("isOthers")) %>'>No</asp:Label>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                    
                                    <telerik:GridButtonColumn ConfirmDialogType="RadWindow" ConfirmTitle="Delete" UniqueName="Delete" ConfirmText="Are you sure you want to delete?" CommandName="Delete" ButtonType="ImageButton" ItemStyle-CssClass="action-delete" ItemStyle-Width="1px" HeaderText="">
                                        <HeaderStyle Width="60px" />
                                    </telerik:GridButtonColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </div>
                </div>
                <%--<div class="form-group">
                    <div class="col-xs-4">&nbsp;</div>
                    <div class="col-xs-4">
                        <asp:Button ID="btnSave" runat="server" OnClick="Save_Click" Text="Save Request" CssClass="uiboxgray btn-sm" />
                    </div>
                </div>--%>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
