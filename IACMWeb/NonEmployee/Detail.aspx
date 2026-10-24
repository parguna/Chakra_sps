<%@ Page Title="Non Employee Detail" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="DI.Chakra.Web.Non_Employee.Detail" %>
<%@ Register Src="~/NonEmployee/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
            <uc1:DetailUC runat="server" ID="DetailUC"  UIMode="View" />
</asp:Content>