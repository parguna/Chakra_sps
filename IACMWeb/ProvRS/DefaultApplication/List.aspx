<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="DI.Chakra.Web.ProvRS.DefaultApplication.List" %>
<%@ Register Src="~/ProvRS/DefaultApplication/UC/ListUC.ascx" TagPrefix="uc1" TagName="ListUC" %>


<asp:content id="Content1" contentplaceholderid="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:ListUC runat="server" id="ListUC"  />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:content>
