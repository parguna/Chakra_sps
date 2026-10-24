<%@ Page Language="C#" MasterPageFile="~/Master/ProtoMast4.master"  AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="DI.Chakra.Web.Comp.Conflict.Add" %>

<%@ Register Src="~/Comp/Conflict/Wizard/ConflictWizardUC.ascx" TagPrefix="uc1" TagName="ConflictWizardUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
           <uc1:ConflictWizardUC runat="server" ID="ConflictWizard" UIMode="Add" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>