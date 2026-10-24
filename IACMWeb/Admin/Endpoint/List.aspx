<%@ Page Title="Endpoint System List" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.Admin.Endpoint.List" %>

<%@ Register Src="~/Admin/Endpoint/List/EndpointListUC.ascx" TagPrefix="uc1" TagName="EndpointListUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server"> 
        <ContentTemplate>
            <%-- Re-Calling JavaScirpt/jQuery function on each Asynchronous post back. --%>
            <script type="text/javascript" language="javascript">
            </script>
            <%-- // Re-Calling function End --%>

            <uc1:EndpointListUC runat="server" ID="EndpointListUC" />
          
        </ContentTemplate>
    </asp:UpdatePanel> 
</asp:Content>