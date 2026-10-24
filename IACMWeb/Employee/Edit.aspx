<%@ Page Title="Edit Employee" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="DI.Chakra.Web.Employee.Edit" %>

<%@ Register Src="~/UsrControl/Common/NEmployeeForm.ascx" TagPrefix="uc1" TagName="EmployeeForm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
     <script type="text/javascript">
        function Redirect() {
            window.location = "List.aspx"; 
        }
    </script>

    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <uc1:EmployeeForm runat="server" ID="EditForm" UIMode="Edit"  />
           
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

