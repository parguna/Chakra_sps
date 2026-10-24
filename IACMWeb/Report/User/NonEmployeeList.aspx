<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NonEmployeeList.aspx.cs" MasterPageFile="~/Master/ProtoMast4.master"  Inherits="DI.Chakra.Web.Report.User.NonEmployeeList" %>



<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Report/User/EmployeesListUC.ascx" TagPrefix="uc1" TagName="EmployeesListUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
<uc1:EmployeesListUC runat="server" id="EmployeesListUC" EmployeeType ="NonEmployee" />
</asp:Content>