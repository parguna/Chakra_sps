<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="MyBulkRequestDetails.aspx.cs" Inherits="DI.Chakra.Web.Request.BulkRequest.MyBulkRequestDetails" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Request/BulkRequest/UC/BulkDetail.ascx" TagPrefix="uc1" TagName="BulkDetail" %>



<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">

    <asp:UpdatePanel ID="updSearch" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>
          
            <uc1:BulkDetail runat="server" id="BulkDetail" />

        </ContentTemplate>
    </asp:UpdatePanel>



    
</asp:Content>
