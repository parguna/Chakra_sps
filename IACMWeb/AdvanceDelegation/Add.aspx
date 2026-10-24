<%@ Page Language="C#" Title="Add Advance Delegation" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="DI.Chakra.Web.AdvanceDelegation.Add" %>
<%@ Register Src="~/AdvanceDelegation/UC/AdvanceDelegationFormUC.ascx" TagPrefix="uc1" TagName="AdvanceDelegationFormUC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:AdvanceDelegationFormUC runat="server" id="AddForm" UIMode="Add" />
</asp:Content>