<%@ Page Title="Inhouse User Detail" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="DI.Chakra.Web.InHouse.Detail" %>
<%@ Register Src="~/InHouse/UC/Detail.ascx" TagPrefix="DetailUC" TagName="DetailUCControl" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <DetailUC:DetailUCControl runat="server" ID="DetailUC" UIMode="View"  />
</asp:Content>
