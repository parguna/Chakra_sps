<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.Admin.AppText.List" %>

<%@ Register Src="~/Admin/AppText/UC/ListUC.ascx" TagPrefix="uc1" TagName="ListUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:ListUC runat="server" ID="ListUC" />

    <div class="buttons text-right nav-rectangle">
        <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="BackButton_Click"></asp:Button>
    </div>

</asp:Content>