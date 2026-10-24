<%@ Page Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="DI.Chakra.Web.Comp.PolicyExecution.Edit" %>

<%@ Register Src="~/Comp/PolicyExecution/Wizard/PolicyExecutionWizard.ascx" TagPrefix="uc1" TagName="PolicyExecutionWizard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:PolicyExecutionWizard runat="server" ID="PolicyExecutionWizard" UIMode="Edit" />
</asp:Content>