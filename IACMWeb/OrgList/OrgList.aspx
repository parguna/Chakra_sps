<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="OrgList.aspx.cs" Inherits="DI.Chakra.Web.Master.OrgList" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Admin/Role/UC/ActionRightsUC.ascx" TagPrefix="uc1" TagName="ActionRightsUC" %>
<%@ Register Src="~/UsrControl/OrgChartUC.ascx" TagPrefix="uc1" TagName="OrgChartUC" %>




<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <script>
        function ClientNodeClick() {
            var winWidth = $(".RadOrgChart").width();
            $(".RadOrgChart").scrollLeft(winWidth / 2);
        }



    </script>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="content-wrapper">
                <div class="row-fluid">
                    <div class="col-xs-3" style="max-height: 600px; overflow:auto">
                        <%--  <h4>Organization List</h4>--%>
                        <telerik:RadTreeView runat="server" ID="OrgStructure" DataFieldID="Id" OnNodeExpand="OrgStructure_NodeExpand" OnNodeDataBound="OrgStructure_NodeDataBound" DataTextField="Name" DataFieldParentID="ParentOrgId" OnNodeClick="OrgStructure_NodeClick"
                            CheckBoxes="false">
                        </telerik:RadTreeView>
                    </div>
                    <div class="col-xs-9 text-center">
                        <%--  <h4>Organization Chart</h4>--%>
                        <asp:Label ID="Message" runat="server" CssClass="control-label boldedText"></asp:Label>
                        <uc1:OrgChartUC runat="server" isFromOrgList="true" ID="OrgChartUC" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <%-- <div class="buttons text-right nav-rectangle topBuffer">
                <asp:Button runat="server" ID="Cancel" Text="Cancel" CssClass="btn btn-default btn-sm validation" OnClientClick="Redirect();" />
            </div>--%>
</asp:Content>
