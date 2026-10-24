<%@ Page Title="My Profile" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="DI.Chakra.Web.Profile.Profile" %>

<%@ Register Src="~/UsrControl/Common/NEmployeeForm.ascx" TagPrefix="uc1" TagName="NEmployeeForm" %>
<%@ Register Src="~/NonEmployee/UC/DetailUC.ascx" TagPrefix="uc1" TagName="DetailUC" %>
<%@ Register Src="~/UsrControl/Common/CurrentAccessUC.ascx" TagPrefix="uc1" TagName="UserAccountsListUC" %>
<%--<%@ Register Src="~/UsrControl/Common/CurrentAccessInactiveUC.ascx" TagPrefix="uc1" TagName="UserAccountsListInactiveUC" %>--%>

<asp:Content ID="Content3" ContentPlaceHolderID="cplMain" runat="server">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <uc1:DetailUC runat="server" ID="DetailUC" UIMode="View" />
            <%--<asp:Panel ID="Panel2" runat="server" CssClass="topBuffer" GroupingText="Inactive Accounts">
                <uc1:UserAccountsListInactiveUC runat="server" ID="EmployeeInactiveAccountsUC" isFromAccessRequest="false" />
            </asp:Panel>--%>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="buttons text-right nav-rectangle">
        <asp:Button ID="btncancel" runat="server" UseSubmitBehavior="false" CssClass="btn btn-default btn-sm" Text="Cancel" OnClick="btncancel_Click" />
    </div>
</asp:Content>
