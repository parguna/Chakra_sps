<%@ Page Title="Edit Non Employee" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="DI.Chakra.Web.Non_Employee.Edit" %>

<%@ Register Src="~/UsrControl/Common/NEmployeeForm.ascx" TagPrefix="uc1" TagName="EmployeeForm" %>
<%@ Register Src="~/UsrControl/Common/CurrentAccessUC.ascx" TagPrefix="uc1" TagName="UserAccountsListUC" %>
<%--<%@ Register Src="~/UsrControl/Common/CurrentAccessInactiveUC.ascx" TagPrefix="uc1" TagName="UserAccountsListInactiveUC" %>--%>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <script type="text/javascript">
        function Redirect() {
            window.location = "List.aspx";
        }
    </script>
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <uc1:EmployeeForm runat="server" ID="EditForm" UIMode="Edit" />
            <asp:Panel ID="Panel1" runat="server" CssClass="topBuffer" GroupingText="Active Accounts">
                <uc1:UserAccountsListUC runat="server" ID="EmployeeAccountUC" isFromAccessRequest="false" />
            </asp:Panel>
            <br />
          <%--  <asp:Panel ID="Panel2" runat="server" CssClass="topBuffer" GroupingText="Inactive Accounts">
                <uc1:UserAccountsListInactiveUC runat="server" ID="EmployeeInactiveAccountsUC" isFromAccessRequest="false" />
            </asp:Panel>--%>
            <div class="buttons text-right nav-rectangle">
                <asp:Button runat="server" class="btn btn-default btn-sm" ID="Cancel" TabIndex="7" Text="Cancel" UseSubmitBehavior="false" OnClick="Cancel_Click"></asp:Button>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

