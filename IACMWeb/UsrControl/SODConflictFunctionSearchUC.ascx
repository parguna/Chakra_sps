<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SODConflictFunctionSearchUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.SODConflictFunctionSearchUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadAutoCompleteBox ID="ConflictAutoCompleteBox" OnClientEntryAdding="OnClientEntryAddingHandler" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control" InputType="Token">
    <TextSettings SelectionMode="Single" />
    <WebServiceSettings Method="GetSODConflictFunctions" Path="../../Services/AutoCompleteDataService.svc" />
</telerik:RadAutoCompleteBox>