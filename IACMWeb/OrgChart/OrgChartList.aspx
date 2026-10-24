<%@ Page  Title="My Organization Chart" Language="C#" AutoEventWireup="true" CodeBehind="OrgChartList.aspx.cs" MasterPageFile="~/Master/ProtoMast4.master" Inherits="DI.Chakra.Web.OrgChart.OrgChartList" %>
<%@ Register Src="~/UsrControl/OrgChartUC.ascx" TagPrefix="uc1" TagName="OrgChartUC" %>

<asp:content id="Content1" contentplaceholderid="cplMain" runat="server">
    <script>
        $(document).ready(function () {
            var winWidth = $(window).width();
            $(".OrgChartContainer").scrollLeft(winWidth*3);
        });
</script>

    <asp:Panel runat="server" CssClass="OrgChartContainer">
        <uc1:OrgChartUC runat="server" ID="OrgChartUC" />
    </asp:Panel>
</asp:content>
