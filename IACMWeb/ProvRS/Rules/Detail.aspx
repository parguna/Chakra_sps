<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="DI.Chakra.Web.ProvRS.Rules.Detail" %>

<%@ Register Src="~/ProvRS/Rules/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
     <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:DetailUC runat="server" id="DetailUC" UIMode ="View" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
