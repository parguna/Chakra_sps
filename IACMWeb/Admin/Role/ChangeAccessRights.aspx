<%@ Page Language="C#" Title="Access Rights" AutoEventWireup="true" MasterPageFile="~/Master/ProtoMast4.master" CodeBehind="ChangeAccessRights.aspx.cs" Inherits="DI.Chakra.Web.Configuration.Role.ChangeAccessRights" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Admin/Role/UC/ActionRightsUC.ascx" TagPrefix="uc1" TagName="ActionRightsUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="Server">

    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <script type="text/javascript">
                function clientNodeChecked(sender, eventArgs) {
                    var currentnode = 1;
                    var action = false;
                    var tree = $find("<%=Menu.ClientID %>");
                    var selectednodeindex = tree._checkedIndexes;
                    var selectednode = [];
                    if (selectednodeindex.length > 0) {
                        for (var i = 0; i < selectednodeindex.length; i++) {
                            //selectednode = selectednode == "" ? selectednodeindex[i].substring(0, selectednodeindex[i].length - 1).replace(":", "") + "," : selectednode + selectednodeindex[i].substring(0, selectednodeindex[i].length - 1).replace(":", "") + + ",";
                            selectednode[i] = selectednodeindex[i].substring(0, selectednodeindex[i].length - 1).replace(":", "").trim();
                        }
                        selectednodeindex.forEach(function (entry) {
                            selectednode[i] = entry.substring(0, 1);
                        });
                        selectednode.forEach(function (entry) {
                            if (currentnode == entry) {
                                action = true;
                                return true;
                            }
                        });
                        var node = eventArgs.get_node();
                        while (node.get_parent().set_checked != null) {
                            node.get_parent().set_checked(action);
                            node = node.get_parent();
                        }
                    }
                }

            </script>
            <div class="content-wrapper">
                <div class="form">
                    <div class="form-horizontal left">
                        
                        <div class="row-fluid">
                            <div class="col-xs-8">
                                    <label class="col-xs-2">Role:</label>
                                    <asp:Label class="col-xs-4" runat="server" Text="" ID="lblRoleName"></asp:Label>

                                </div>
                             <div class="col-xs-4">
                                     <asp:Button ID="ExportExcelButton" Visible="true" runat="server" Onclick="ExportExcelButton_Click" Text="Export To Excel" CssClass="btn btn-default btn-sm" />&nbsp;&nbsp;
                                     <asp:Button ID="ExportCSVButton" Visible="true" runat="server" onclick="ExportCSVButton_Click" Text="Export To CSV" CssClass="btn btn-default btn-sm" />
                                </div>
                        </div>
                       
                        <div class="row-fluid">
                             <div class="col-xs-6">
                                    <h4>Menu</h4>
                                    <telerik:RadTreeView runat="server" ID="Menu" DataValueField="PageId" DataFieldID="Id" DataTextField="MenuName" DataFieldParentID="ParentTabMenuId" OnNodeDataBound="Menu_NodeDataBound" OnNodeClick="Menu_NodeClick"
                                        CheckBoxes="true" CheckChildNodes="true">
                                    </telerik:RadTreeView>
                                </div>
                                <div class="col-xs-6">
                                    <h4>Access Rights</h4>
                                    <uc1:ActionRightsUC runat="server" ID="ActionRightsUC" Visible="false" OnUpdate="UpdateAccessRight" />
                                </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="buttons text-right nav-rectangle topBuffer">
                <asp:Button runat="server" ID="Cancel" Text="Cancel" OnClick="Cancel_Click"
                     CssClass="btn btn-default btn-sm validation" />
            </div>
             <script type="text/javascript">
            $(window).bind('beforeunload', function () {
                try {
                    $("#UpdateProgressBar").hide();
                } catch (e) {

                }
            });    
        </script>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="ExportExcelButton" />
            <asp:PostBackTrigger ControlID="ExportCSVButton" />
        </Triggers>
    </asp:UpdatePanel>
    
</asp:Content>
