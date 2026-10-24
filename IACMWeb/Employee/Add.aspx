<%@ Page Title="Add Employee" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="DI.Chakra.Web.Employee.Add" %>

<%@ Register Src="~/UsrControl/Common/NEmployeeForm.ascx" TagPrefix="uc1" TagName="EmployeeForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <script type="text/javascript">
        function Redirect() {
            window.location = "List.aspx";
        }

    </script>
    <uc1:EmployeeForm runat="server" ID="AddForm" UIMode="Add" />
    <div class="buttons text-right nav-rectangle">
        <asp:Button runat="server" class="btn btn-default btn-sm" ID="Button1" TabIndex="7" Text="Cancel" UseSubmitBehavior="false" OnClick ="Button1_Click"></asp:Button>
    </div>
</asp:Content>
