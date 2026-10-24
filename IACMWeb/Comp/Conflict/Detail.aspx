<%@ Page Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="DI.Chakra.Web.Comp.Conflict.Detail" %>
<%@ Register Src="~/Comp/Conflict/Wizard/ConflictWizardUC.ascx" TagPrefix="uc1" TagName="ConflictWizardUC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
   <uc1:ConflictWizardUC runat="server" ID="ConflictWizard" UIMode="View" />
</asp:Content>
