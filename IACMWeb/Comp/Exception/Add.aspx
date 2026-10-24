<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="DI.Chakra.Web.Comp.Exception.Add" %>
<%@ Register Src="~/Comp/Exception/Wizard/ExceptionWizardUC.ascx" TagPrefix="Exp" TagName="Exception" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <Exp:Exception ID="Exception" runat="server" UIMode="Add" />
</asp:Content>

