<%@ Page Title="Access Request" Language="C#" MasterPageFile="~/Master/ProtoMast4.master"  AutoEventWireup="true" CodeBehind="CreateCart.aspx.cs" Inherits="DI.Chakra.Web.NACR.CreateCart" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/NACR/Create/Wizard/WizardCartUC.ascx" TagPrefix="uc1" TagName="WizardCartUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:WizardCartUC runat="server" id="WizardCartUC" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>