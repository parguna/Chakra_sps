<%@ Page Title="Home" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" Inherits="DI.Chakra.Web.Home_Home" CodeBehind="Home.aspx.cs" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/Dashboard/RiskDB.ascx" TagPrefix="uc1" TagName="RiskDB" %>
<%@ Register Src="~/Dashboard/TasksUC.ascx" TagPrefix="uc1" TagName="TaskUC" %>
<%@ Register Src="~/Dashboard/IACMBarChart.ascx" TagPrefix="uc1" TagName="IACMBarChart" %>
<%@ Register Src="~/Dashboard/IACMPieChart.ascx" TagPrefix="uc1" TagName="IACMPieChart" %>
<%@ Register Src="~/Dashboard/IACMColumnChart.ascx" TagPrefix="uc1" TagName="IACMColumnChart" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="Server">
    <style>
        .higherZIndex {
            z-index: 2;
        }

        .rdLeft, .rdBottom, .rdRight {
            display: none !important;
        }

        .di-tab-content .nav {
            display: none;
        }

        .di-tab-content .di-tab-main .tab-pane {
            background: transparent;
            border-bottom: 0;
            border-bottom-left-radius: 0;
            border-bottom-right-radius: 0;
            border-left: 0;
            border-right: 0;
            border-top-right-radius: 0;
            padding: 0;
            position: relative;
        }

        .RadDock_IACMSkin {
            margin: 0 0 10px;
            padding: 0 0;
            float: left !important;
        }

        .RadDock.RadDock_IACMSkin:first-of-type {
            margin-right: 28px;
        }
    </style>

    <div style="overflow: hidden; display: inline-table;">
        <telerik:radajaxpanel id="RadAjaxPanel1" runat="server">
        <telerik:RadDockLayout runat="server" ID="RadDockLayout1">

            <telerik:RadDockZone runat="server" ID="RadDockZoneDashboard" Orientation="Horizontal" Skin="IACMSkin">
            </telerik:RadDockZone>
        </telerik:RadDockLayout>
        </telerik:radajaxpanel>
    </div>



</asp:Content>
