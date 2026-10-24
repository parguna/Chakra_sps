<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Add.aspx.cs"  MasterPageFile="~/Master/ProtoMast4.master" Inherits="DI.Chakra.Web.AppCustomization.Dashboard.DefaultRoleDB.Add" %>

<%@ Register Src="~/AppCustomization/Dashboard/DefaultRoleDB/UC/Detail.ascx" TagPrefix="uc1" TagName="Dtail" %>



<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:Dtail runat="server" id="Detail" UIMode="Add"/>
</asp:Content>