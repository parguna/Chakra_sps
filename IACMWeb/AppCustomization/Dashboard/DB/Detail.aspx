<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs"  MasterPageFile="~/Master/ProtoMast4.master" Inherits="DI.Chakra.Web.AppCustomization.Dashboard.DB.Detail" %>

<%@ Register Src="~/AppCustomization/Dashboard/DB/UC/Detail.ascx" TagPrefix="uc1" TagName="Dtail" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:Dtail runat="server" id="Detail" UIMode="View"/>
</asp:Content>