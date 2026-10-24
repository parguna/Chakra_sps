<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Organisation.aspx.cs" MasterPageFile="~/Master/ProtoMast4.master" Inherits="DI.Chakra.Web.Report.Provisioning.Organisation" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Report/Provisioning/OrganisationUC.ascx" TagPrefix="uc1" TagName="OrganisationUC" %>



<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:OrganisationUC runat="server" id="OrganisationUC" />
</asp:Content>