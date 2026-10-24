<%@ Page Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="DI.Chakra.Web.Comp.Mitigation.Add" %>
<%@ Register Src="~/Comp/Mitigation/Wizard/MitigationWized.ascx" TagName="MitigWizard" TagPrefix="Mitig" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <Mitig:MitigWizard ID="MitigWiz" runat="server" UIMode="Add"/>
</asp:Content>