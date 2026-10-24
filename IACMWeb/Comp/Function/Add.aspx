<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="DI.Chakra.Web.Comp.Function.Add" %>

<%@ Register Src="~/Comp/Function/Wizard/FunctionWizardUC.ascx" TagPrefix="uc1" TagName="FunctionWizardUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">

    <uc1:FunctionWizardUC runat="server" id="FunctionWizardUC" UIMode="Add" />

</asp:Content>
