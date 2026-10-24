<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubmittedReviews.aspx.cs" MasterPageFile="~/Master/ProtoMast4.master"  Inherits="DI.Chakra.Web.Report.Attestation.SubmittedReviews" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Report/Attestation/SubmittedReviewsUC.ascx" TagPrefix="uc1" TagName="SubmittedReviewsUC" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:SubmittedReviewsUC runat="server" id="SubmittedReviewsUC" />
</asp:Content>