<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExceptionSearchUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.ExceptionSearchUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadAutoCompleteBox ID="ExceptionAutoCompleteBox" OnClientEntryAdding="OnClientEntryAddingHandler" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control" InputType="Token">
    <TextSettings SelectionMode="Single" />
    <WebServiceSettings Method="GetAllSodExceptions" Path="../../Services/AutoCompleteDataService.svc" />
</telerik:RadAutoCompleteBox>