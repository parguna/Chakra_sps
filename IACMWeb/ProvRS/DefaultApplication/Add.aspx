<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="DI.Chakra.Web.ProvRS.DefaultApplication.Add" %>
<%@ Register Src="~/ProvRS/DefaultApplication/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>


<asp:content id="Content1" contentplaceholderid="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:DetailUC runat="server" id="DetailUC" UIMode="Add" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:content>
