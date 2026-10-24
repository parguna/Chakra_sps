<%@ Page Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="DI.Chakra.Web.Comp.Mitigation.Detail" %>
<%@ Register Src="~/Comp/Mitigation/Wizard/MitigationWized.ascx" TagName="MitigWizard" TagPrefix="Mitig" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <Mitig:MitigWizard ID="MitigWizard" runat="server" UIMode="View" />
</asp:Content>