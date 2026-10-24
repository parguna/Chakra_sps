<%@ Page  Title="Workflow" Language="C#"  MasterPageFile="~/Master/ProtoMast4.master"  AutoEventWireup="true"  CodeBehind="Clone.aspx.cs" Inherits="DI.Chakra.Web.WorkFlow.Clone" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/WorkFlow/UC/WorkflowDetailUC.ascx" TagPrefix="uc1" TagName="WorkflowDetailUC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="Server">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <uc1:WorkflowDetailUC runat="server" ID="WorkflowDetailControl" OperationMode="Clone"></uc1:WorkflowDetailUC>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>


  

