<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="ActiveEmployees.aspx.cs" Inherits="DI.Chakra.Web.Report.User.ActiveEmployees" %>

<%@ Register Src="~/Report/User/UC/ActiveEmployeesListUC.ascx" TagPrefix="uc1" TagName="ActiveEmployeesListUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:ActiveEmployeesListUC runat="server" id="ActiveEmployeesListUC" />
</asp:Content>
