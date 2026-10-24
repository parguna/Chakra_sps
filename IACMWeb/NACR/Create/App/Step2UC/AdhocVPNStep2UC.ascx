<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocVPNStep2UC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.Step2UC.AdhocVPNStep2UC" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<style type="text/css">
    .auto-style1 {
        font-size: xx-small;
    }
    .label{
        font-size:100%;
    }
    
</style>
<script type="text/javascript">
    function OnClientRequesting(sender, args) {
        var context = args.get_context()
        context["IsFulltimeEmployee"] = "<%= RequsteeIsFullTimeEmployee %>";
        var e = document.getElementById("<%= ddlVPNType.ClientID%>");
        var hiddenVPNType = e.options[e.selectedIndex].value;
        context["vpnType"] = hiddenVPNType;
        context["AdhocActionType"] = "<%= VPNAdhocActionType %>";
        context["endPointUserId"] = "<%= EndpointUserId %>";
        context["endpointId"] = "<%= EndpointSystemId %>";
    }
    function OnClientEntryVPNApplicationAddingHandler(sender, eventArgs) {
            if (sender.get_entries().get_count() > 0) {
                eventArgs.set_cancel(true);
            }
        }
</script>
<asp:HiddenField ID="hiddenVPNType" runat="server" Value="0" />
<div class="form">
    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="col-xs-6">
                <asp:Panel ID="pnlADEndpoint" runat="server" Visible="false">
                    <div class="form-group">
                        <label class="col-xs-4 control-label">AD User Name</label>
                        <div class="col-xs-8">
                            <asp:Label runat="server" ID="lblADAccountName"></asp:Label>
                        </div>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnlEndPointUserName" runat="server" Visible="false">
                    <div class="form-group required">
                        <label class="col-xs-4 control-label">Token Number</label>
                        <div class="col-xs-8">
                            <asp:DropDownList ID="ddlEndpointUser" CssClass="form-control selectpicker" AutoPostBack="true" runat="server" OnSelectedIndexChanged="ddlEndpointUser_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlOneKeyToken" runat="server" Visible="false">
                    <div class="form-group">
                        <label class="col-xs-4 control-label" runat="server" id="lblOneKeyTokenLabel" style="visibility:hidden"></label>
                        <div class="col-xs-8">
                            <div style="float: left;">
                                <asp:TextBox ID="txtOneKeyToken" runat="server" TextMode="SingleLine" MaxLength="24" Width="503px" Visible="false"></asp:TextBox>&nbsp;<br />
                                <asp:CheckBox ID="chkUpdateOneKeyToken" runat="server" Text="Token Replacement/Lost" OnCheckedChanged="chkUpdateOneKeyToken_CheckedChanged" AutoPostBack="true" />
                            </div>
                            <%--<div style="float: left;">
                                <span style="font-size: smaller">
                                    <strong><span class="auto-style1">Note:</span></strong><span class="auto-style1"> All Singapore Citizens or Permanent Residents (PR) MUST have the OneKey token before applying for VPN access.
                                <br />
                                        Registration for OneKey token can be done via this link: </span><b style="color:blue;">http://www.onekey.sg/your-onekey/register.html</b>
                                    <br class="auto-style1" />
                                    <span class="auto-style1">For Non-Singapore Citizens and Non-PR, you do not have to register for the OneKey token, you may leave the Serial Number field empty. </span></span>
                            </div>--%>
                        </div>
                    </div>
                </asp:Panel>
                 <div class="form-group" runat="server" id="divVPNType" visible="false" >
                        <label class="col-xs-4 control-label">VPN Type<span style="color:red" runat="server" id="requiredFieldType" visible="false">*</span></label>
                        <div class="col-xs-8">
                            <asp:DropDownList CssClass="form-control selectpicker" runat="server" ID="ddlVPNType" AutoPostBack="true" OnSelectedIndexChanged="ddlVPNType_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                </div>
                <asp:Panel ID="pnlGUI" runat="server" Visible="true">
                    <div class="form-group">
                        <label class="col-xs-4 control-label"><%= LabelText %><span style="color:red" runat="server" id="requiredFieldAppln" visible="false">*</span></label>
                        <div class="col-xs-8">
                            <telerik:RadAutoCompleteBox ID="ApplicationAutoCompleteBox" OnEntryRemoved="ApplicationAutoCompleteBox_EntryRemoved" OnEntryAdded="ApplicationAutoCompleteBox_EntryAdded"
                                EmptyMessage="Enter * to view full list" runat="server" DropDownHeight="200px" DropDownWidth="300px" InputType="Token" OnClientRequesting="OnClientRequesting" OnClientEntryAdding="OnClientEntryVPNApplicationAddingHandler"
                                CssClass="form-control" AutoPostBack="true">
                                <TextSettings SelectionMode="Single" />
                                <WebServiceSettings Method="GetVPNApplications" Path="../../../../Services/AutoCompleteDataService.svc" />
                            </telerik:RadAutoCompleteBox>
                        </div>
                       
                    </div>
                    <div class="form-group" id="divDefaultAppln" runat="server" visible="false">
                        <div class="col-xs-4"></div>
                        <div class="col-xs-8"><asp:Label Font-Bold="true" runat="server" ID="lblDefaultApplicationText"></asp:Label></div>
                    </div>    
                    <div class="form-group">
                        <div class="col-xs-4">&nbsp;</div>
                        <div class="col-xs-8">
                            <div style='<%=ShowEnviornment ? "" : "display: none" %>'>
                                <div class="row" style='<%=IsProd ? "" : "display: none" %>' >
                                    <div class="col-xs-6">
                                        <asp:CheckBox ID="chkProduction" runat="server" Text="Production" Visible="true"/>
                                    </div>
                                    <div class="col-xs-3">
                                        <asp:Label ID="lblProductionADNames"  runat="server" Visible="false"></asp:Label>
                                    </div>
                                </div>
                                <div class="row" style='<%=IsQA ? "" : "display: none" %>' >
                                    <div class="col-xs-6">
                                        <asp:CheckBox ID="chkQA" runat="server" Text="QA" Visible="true"/>
                                    </div>
                                    
                                    <div  class="col-xs-3">
                                        <asp:Label ID="lblQAADNames" CssClass="label label-primary adgroups" runat="server" Visible="false"></asp:Label>
                                    </div>
                                </div>
                                <div class="row" style='<%=IsDev ? "" : "display: none" %>' >
                                    <div class="col-xs-6">
                                        <asp:CheckBox ID="chkDevelopment" runat="server" Text="Development" Visible="true"/>
                                    </div>
                                    <div class="col-xs-3">
                                        <asp:Label ID="lblDevADNames" CssClass="label label-primary adgroups" runat="server" Visible="false"></asp:Label>
                                    </div>
                                </div>
                                <div class="row" style='<%=IsProject ? "" : "display: none" %>' >
                                    <div class="col-xs-6">
                                        <asp:CheckBox ID="chkProjectEnv" runat="server" Text="Project" Visible="false" />
                                    </div>
                                    <div class="col-xs-3">
                                        <asp:Label ID="lblProjectName" CssClass="label label-primary adgroups" runat="server" Visible="false"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
                <div class="form-group">
                    <div class="col-xs-4"></div>
                    <div class="col-xs-8">
                        <div>
                            <asp:Button ID="btnAddRequest" runat="server" Text="Add" Visible="false" CssClass="btn btn-default" OnClick="btnAddRequest_Click"></asp:Button>
                            <asp:Button ID="btnSave" runat="server" OnClick="AddToCart" Text="Save Request" CssClass="btn btn-default left" Visible="false"/>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row-fluid">
            <div class="form-group">
                <div class="col-xs-12 col-sm-12 col-lg-12 col-md-12">
                    <telerik:RadGrid ID="ApplicationGrid" runat="server" AllowPaging="false" AllowSorting="false" CellSpacing="0" Visible="true"
                        GridLines="None" Width="100%" OnItemCommand="ApplicationGrid_ItemCommand" ShowFooter="false" OnItemDataBound="ApplicationGrid_ItemDataBound"
                        MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Applications Available">
                        <GroupingSettings CaseSensitive="false" />
                        <MasterTableView AutoGenerateColumns="false" DataKeyNames="identity" Width="100%">
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
                                <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="Production">
                                    <ItemTemplate>
                                        <asp:Label runat="server" Visible='<%# Convert.ToBoolean(Eval("Production")) %>'>Yes</asp:Label>
                                        <asp:Label runat="server"  Visible='<%# !Convert.ToBoolean(Eval("Production")) %>'>No</asp:Label>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="QA">
                                    <ItemTemplate>
                                        <asp:Label runat="server"  Visible='<%# Convert.ToBoolean(Eval("QA")) %>'>Yes</asp:Label>
                                        <asp:Label runat="server" Visible='<%# !Convert.ToBoolean(Eval("QA")) %>'>No</asp:Label>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="Development">
                                    <ItemTemplate>
                                        <asp:Label runat="server"  Visible='<%# Convert.ToBoolean(Eval("Dev")) %>'>Yes</asp:Label>
                                        <asp:Label runat="server"  Visible='<%# !Convert.ToBoolean(Eval("Dev")) %>'>No</asp:Label>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>

                                <telerik:GridTemplateColumn HeaderStyle-Width="60px" HeaderText="Others" Visible="false" Display="false">
                                    <ItemTemplate>
                                        <asp:Label runat="server"  Visible='<%# Convert.ToBoolean(Eval("isOthers")) %>'>Yes</asp:Label>
                                        <asp:Label runat="server"  Visible='<%# !Convert.ToBoolean(Eval("isOthers")) %>'>No</asp:Label>
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

        </div>
    </div>
</div>
