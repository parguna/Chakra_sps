<%@ Page Title="Employee Detail" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="DI.Chakra.Web.Employee.Detail" %>

<%@ Register Src="~/Employee/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>
<%@ Register Src="~/UsrControl/Common/CurrentAccessUC.ascx" TagPrefix="uc1" TagName="UserAccountsListUC" %>
<%--<%@ Register Src="~/UsrControl/Common/CurrentAccessInactiveUC.ascx" TagPrefix="uc1" TagName="UserAccountsListInactiveUC" %>--%>
<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <uc1:DetailUC runat="server" ID="DetailUC" UIMode="View" />
            <asp:Panel ID="Panel1" runat="server" CssClass="topBuffer" GroupingText="Active Accounts">
                <uc1:UserAccountsListUC runat="server" ID="EmployeeAccountUC" isFromAccessRequest="false" />
            </asp:Panel>
            <br />
           <%-- <asp:Panel ID="Panel2" runat="server" CssClass="topBuffer" GroupingText="Inactive Accounts">
                <uc1:UserAccountsListInactiveUC runat="server" ID="EmployeeInactiveAccountsUC" isFromAccessRequest="false" />
            </asp:Panel>--%>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
