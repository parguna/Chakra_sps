<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FunctionSearchUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.FunctionSearchUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadAutoCompleteBox ID="FunctionAutoCompleteBox" OnClientEntryAdding="OnClientEntryAddingHandler" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control" InputType="Token">
    <TextSettings SelectionMode="Single" />
    <WebServiceSettings Method="GetFunctions" Path="../../Services/AutoCompleteDataService.svc" />
</telerik:RadAutoCompleteBox>















