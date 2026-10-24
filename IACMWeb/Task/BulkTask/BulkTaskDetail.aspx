<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="BulkTaskDetail.aspx.cs" Inherits="DI.Chakra.Web.Task.BulkTask.BulkTaskDetail1" %>

<%@ Register Src="~/Task/BulkTask/UC/BulkTaskDetail.ascx" TagPrefix="uc1" TagName="BulkTaskDetail" %>




<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <uc1:BulkTaskDetail runat="server" id="BulkTaskDetail" />

</asp:Content>
