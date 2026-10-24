<%@ Page Title="Member Detail" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="DI.Chakra.Web.Members.Detail" %>

<%@ Register Src="~/Members/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>
<%@ Register Src="~/UsrControl/Common/CurrentAccessUC.ascx" TagPrefix="uc1" TagName="UserAccountsListUC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <uc1:DetailUC runat="server" ID="DetailUC" UIMode="View" />
            <asp:Panel ID="Panel1" runat="server" CssClass="topBuffer">
                <uc1:UserAccountsListUC runat="server" ID="EmployeeAccountUC" isFromAccessRequest="false" />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
