<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RiskLevelSearchUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.RiskLevelSearchUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadAutoCompleteBox ID="RiskLevelAutoCompleteBox" OnClientEntryAdding="OnClientEntryAddingHandler" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control" InputType="Token">
    <TextSettings SelectionMode="Single" />
    <WebServiceSettings Method="GetAllRiskLevel" Path="../../Services/AutoCompleteDataService.svc" />
</telerik:RadAutoCompleteBox>