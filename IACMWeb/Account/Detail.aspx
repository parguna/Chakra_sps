<%@ Page Title="Account Detail" MasterPageFile="~/Master/ProtoMast4.master" Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="DI.Chakra.Web.Account.Detail" %>
<%@ Register Src="~/Account/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>
<asp:Content ID="Content3" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <uc1:DetailUC runat="server" id="DetailUC" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

