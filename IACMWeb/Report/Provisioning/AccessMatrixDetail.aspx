<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccessMatrixDetail.aspx.cs" MasterPageFile="~/Master/ProtoMast4.master" Inherits="DI.Chakra.Web.Report.Provisioning.AccessMatrixDetail" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Report/Provisioning/AccessDetailMatrixUC.ascx" TagPrefix="uc1" TagName="AccessDetailMatrixUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:AccessDetailMatrixUC runat="server" id="AccessDetailMatrixUC" />      
</asp:Content>

