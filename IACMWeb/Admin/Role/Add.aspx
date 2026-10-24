<%@ Page Title="Add Role" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="DI.Chakra.Web.Admin.Role.Add" %>

<%@ Register Src="~/Admin/Role/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <uc1:DetailUC runat="server" ID="DetailUC" UIMode="Add" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>