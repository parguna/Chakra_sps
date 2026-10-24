<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Master/ProtoMast4.master" CodeBehind="Clone.aspx.cs" Inherits="DI.Chakra.Web.Comp.Function.Clone" %>

<%@ Register Src="~/Comp/Function/Wizard/FunctionWizardUC.ascx" TagPrefix="uc1" TagName="FunctionWizardUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:FunctionWizardUC runat="server" id="FunctionWizardUC" UIMode="Clone" />
</asp:Content>