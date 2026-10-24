<%@ Page Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="DI.Chakra.Web.Comp.Mitigation.Edit" %>
<%@ Register Src="~/Comp/Mitigation/Wizard/MitigationWized.ascx" TagName="MitigWizard" TagPrefix="Mitig" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <Mitig:MitigWizard ID="MitigWiz" runat="server"  UIMode="Edit"/>
</asp:Content>
