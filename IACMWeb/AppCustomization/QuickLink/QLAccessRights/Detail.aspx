<%@ Page Language="C#" AutoEventWireup="true"   MasterPageFile="~/Master/ProtoMast4.master"  CodeBehind="Detail.aspx.cs" Inherits="DI.Chakra.Web.AppCustomization.QuickLink.QLAccessRights.Detail" %>

<%@ Register Src="~/AppCustomization/QuickLink/QLAccessRights/UC/Detail.ascx" TagPrefix="uc1" TagName="Dtail" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:Dtail runat="server" id="Detail" UIMode="View"/>
</asp:Content>