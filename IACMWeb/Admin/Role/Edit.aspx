<%@ Page Title="Edit Role" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="DI.Chakra.Web.Admin.Role.Edit" %>
<%@ Register Src="~/Admin/Role/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <uc1:DetailUC runat="server" ID="DetailUC" UIMode="Edit" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
