<%@ Page Title="Workflow" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" Inherits="DI.Chakra.Web.WorkFlow.Add" CodeBehind="Add.aspx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/WorkFlow/UC/WorkflowDetailUC.ascx" TagPrefix="uc1" TagName="WorkflowDetailUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="Server">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <uc1:WorkflowDetailUC runat="server" ID="WorkflowDetailControl" OperationMode="Add" ></uc1:WorkflowDetailUC>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
