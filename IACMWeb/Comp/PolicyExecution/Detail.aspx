<%@ Page Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="DI.Chakra.Web.Comp.PolicyExecution.Detail" %>

<%@ Register Src="~/Comp/PolicyExecution/Wizard/PolicyExecutionWizard.ascx" TagPrefix="uc1" TagName="PolicyExecutionWizard" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:PolicyExecutionWizard runat="server" ID="PolicyExecutionWizard"  UIMode="View"/>
</asp:Content>