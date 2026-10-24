<%@ Page Title="Endpoint System" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="DI.Chakra.Web.Admin.Endpoint.Add" %>

<%@ register src="~/Admin/Endpoint/Wizard/WizardUC.ascx" tagprefix="uc1" tagname="WizardUC" %>


<asp:content id="Content1" contentplaceholderid="cplMain" runat="server">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <uc1:wizarduc runat="server" id="WizardUC" UIMode="Add" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:content>
