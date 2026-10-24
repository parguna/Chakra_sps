<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="DI.Chakra.Web.Comp.Exception.Edit" %>
<%@ Register Src="~/Comp/Exception/Wizard/ExceptionWizardUC.ascx" TagName="ExpEdit" TagPrefix="ExpEdt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
   <ExpEdt:ExpEdit ID="ExpEditId" runat="server" UIMode="Edit" />
</asp:Content> 
