<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/Master/ProtoMast4.master" CodeBehind="Clone.aspx.cs" Inherits="DI.Chakra.Web.Admin.Role.Clone" %>

<%@ Register Src="~/Admin/Role/UC/CloneUC.ascx" TagPrefix="uc1" TagName="CloneUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:CloneUC runat="server" ID="CloneUC" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>