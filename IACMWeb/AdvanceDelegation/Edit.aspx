<%@ Page Title="Edit Advance Delegation" MasterPageFile="~/Master/ProtoMast4.master" Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="DI.Chakra.Web.AdvanceDelegation.Edit" %>
<%@ Register Src="~/AdvanceDelegation/UC/AdvanceDelegationFormUC.ascx" TagPrefix="uc1" TagName="AdvanceDelegationFormUC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:AdvanceDelegationFormUC runat="server" id="EditForm" UIMode="Edit" />
</asp:Content>

