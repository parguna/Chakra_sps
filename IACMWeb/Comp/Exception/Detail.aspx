<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="DI.Chakra.Web.Comp.Exception.Detail" %>
<%@ Register Src="~/Comp/Exception/Wizard/ExceptionWizardUC.ascx" TagName="ExptDetail" TagPrefix="Exp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <Exp:ExptDetail ID="ExpDet" runat="server" UIMode="View" />
</asp:Content>
