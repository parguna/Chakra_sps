<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RejectedReviews.aspx.cs"  MasterPageFile="~/Master/ProtoMast4.master"  Inherits="DI.Chakra.Web.Report.Attestation.RejectedReviews" %>


<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Report/Attestation/RejectedReviewsUC.ascx" TagPrefix="uc1" TagName="RejectedReviewsUC" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:RejectedReviewsUC runat="server" id="RejectedReviewsUC" />
</asp:Content>