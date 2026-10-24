<%@ Page  Language="C#" AutoEventWireup="true" MasterPageFile="~/Master/ProtoMast4.master" CodeBehind="DetailType.aspx.cs" Inherits="DI.Chakra.Web.Task.DetailType" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register Src="~/Admin/Attestation/UC/NReviewUC.ascx" TagPrefix="uc1" TagName="NReviewUC" %>
<%@ Register Src="~/Task/BulkTask/UC/BulkTaskDetail.ascx" TagPrefix="uc1" TagName="BulkTaskDetail" %>
<%@ Register Src="~/UsrControl/Common/ActionTaskUC.ascx" TagPrefix="uc1" TagName="ActionTask" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="Server">
<script type="text/javascript">

    $(document).ready(function () {
        $(".floating-menu-right").hide();
    });

</script>



    <uc1:NReviewUC runat="server" ID="NReviewUC" Visible="false" />
    <uc1:BulkTaskDetail runat="server" ID="BulkTaskDetail" CanLoad="false" Visible="false" />
    <uc1:ActionTask runat="server" ID="ActionTask" Visible="false" isFromAccessRequest="false" IsAdminMode="No" />
</asp:Content>


